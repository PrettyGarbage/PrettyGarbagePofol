using System;
using Common;
using Framework.Common.Template.SceneLoader;
using Library.Network;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(RPCView))]
public class NetworkEventModel : AppContext<NetworkEventModel>
{
    #region Fields

    Subject<string> OnSceneChange { get; } = new();
    Subject<Unit> OnScenarioStart { get; } = new();
    Subject<Unit> OnTrainingPlay { get; } = new();
    Subject<Unit> OnTrainingPause { get; } = new();
    Subject<Unit> OnNextScenarioEvent { get; } = new();
    Subject<Unit> OnTrainingStop { get; } = new();
    Subject<Unit> OnTrainingFinish { get; } = new();

    #endregion

    #region Public Methods

    public void SetNetworkEvent()
    {
        if (DataModel.Instance.IsObserver)
            SetNetworkEventStream();

        SubscribeNetworkEvent();
    }

    #endregion

    #region Private Methods

    void SetNetworkEventStream()
    {
        DataModel.Instance.OPSModel.Status
            .DelayFrame(1)
            .Where(status => status == "TrainingStart")
            .Where(_ => DataModel.Instance.USModels.OnlyClient().OnlyConnected().Any())
            .Where(_ => DataModel.Instance.USModels.OnlyClient().OnlyConnected().All(x => x.Status.Value == Define.Status.Initial || x.Status.Value == Define.Status.TraineeReady))
            .Where(_ => SceneLoader.Instance.LoadState == LoadState.Loaded)
            .Select(_ => DataModel.Instance.OPSModel.Scenario.Value)
            .Subscribe(scenarioCode =>
            {
                Logger.Log($"### OnNext SceneChange");
                RPCView.Of(this).RPC(nameof(RPC_NotifyOnSceneChange), scenarioCode);
            })
            .AddTo(gameObject);

        DataModel.Instance.OPSModel.Status
            .Where(status => status == "TrainingPlay")
            .Where(_ =>
            {
                return DataModel.Instance.USModels.OnlyConnected().All(x => x.Status.Value == Define.Status.Started)
                    || DataModel.Instance.USModels.OnlyConnected().All(x => x.Status.Value == Define.Status.Pause);
            })
            .Subscribe(_ =>
            {
                Logger.Log($"### OnNext TrainingPlay");
                RPCView.Of(this).RPC(nameof(RPC_NotifyOnTrainingPlay));
            })
            .AddTo(gameObject);

        DataModel.Instance.OPSModel.Status
            .Where(status => status == "TrainingPause")
            .Subscribe(_ =>
            {
                Logger.Log($"### OnNext TrainingPause");
                RPCView.Of(this).RPC(nameof(RPC_NotifyOnTrainingPause));
            })
            .AddTo(gameObject);

        DataModel.Instance.USModels.Select(us => us.Status)
            .Merge()
            .Where(_ => DataModel.Instance.USModels.OnlyClient().OnlyConnected().Any())
            .Subscribe(_ =>
            {
                if (DataModel.Instance.IsObserver)
                {
                    if (DataModel.Instance.USModels.OnlyConnected().All(us => us.Status.Value is Define.Status.Initial))
                        DataModel.Instance.Mine.Status.Value = Define.Status.Initial;
                    if (DataModel.Instance.USModels.OnlyConnected().All(us => us.Status.Value is Define.Status.TraineeReady))
                        DataModel.Instance.Mine.Status.Value = Define.Status.TraineeReady;
                }
            })
            .AddTo(gameObject);

        DataModel.Instance.USModels.Select(us => us.Status)
            .Merge()
            .Where(_ => DataModel.Instance.USModels.OnlyConnected().Any())
            .Where(_ => DataModel.Instance.USModels.OnlyConnected().All(us => us.Status.Value is Define.Status.Started))
            .Subscribe(_ =>
            {
                Logger.Log($"### OnNext ScenarioStart");
                RPCView.Of(this).RPC(nameof(RPC_NotifyOnScenarioStart));
            })
            .AddTo(gameObject);

        DataModel.Instance.USModels.OnlyClient().Select(us => us.EventStatus.AsUnitObservable())
            .Concat(DataModel.Instance.USModels.OnlyClient().Select(us => us.EventCode.AsUnitObservable()))
            .Merge()
            .DelayFrame(1)
            .Where(_ => DataModel.Instance.USModels.OnlyClient().OnlyConnected().Any())
            .Where(_ => DataModel.Instance.USModels.OnlyConnected().SameAll(us => us.EventCode.Value))
            .Where(_ => DataModel.Instance.USModels.OnlyClient().OnlyConnected().All(us => us.EventStatus.Value == Define.EventStatus.End))
            .SkipUntil(OnScenarioStart)
            .TakeUntil(OnTrainingStop)
            .TakeUntil(OnTrainingFinish)
            .RepeatUntilDestroy(gameObject)
            .Subscribe(_ =>
            {
                Logger.Log($"### OnNext NextScenarioEvent");
                RPCView.Of(this).RPC(nameof(RPC_NotifyOnNextScenarioEvent));
            })
            .AddTo(gameObject);

        DataModel.Instance.OPSModel.Status
            .DelayFrame(1)
            .Where(status => status == "TrainingStop")
            .Delay(TimeSpan.FromSeconds(3f))
            .Subscribe(_ =>
            {
                Logger.Log($"### OnNext TrainingStop");
                RPCView.Of(this).RPC(nameof(RPC_NotifyOnTrainingStop));
            })
            .AddTo(gameObject);

        DataModel.Instance.OPSModel.Status
            .DelayFrame(1)
            .Where(status => status == "TrainingFinish")
            .Delay(TimeSpan.FromSeconds(3f))
            .Subscribe(_ =>
            {
                Logger.Log($"### OnNext TrainingFinish");
                RPCView.Of(this).RPC(nameof(RPC_NotifyOnTrainingFinish));
            })
            .AddTo(gameObject);
    }

    void SubscribeNetworkEvent()
    {
        OnSceneChange.Subscribe(async scenarioCode =>
        {
            Logger.Log("Subscribe OnSceneChange");

            await SceneLoader.Instance.LoadSceneAsync(CSVManager.Instance.LoadScenarioData().Single(key => key.Key.Equals(scenarioCode)).Value);

            if (DataModel.Instance.IsObserver) ObserverLobbySystem.Instance.HideObserverLobby();

            RaycastSystem.Instance.StartRaycast();

            if (!DataModel.Instance.IsObserver) DataModel.Instance.USModels.Mine().Status.Value = Define.Status.Started;
            else
            {
                if (DataModel.Instance.USModels.OnlyClient().OnlyConnected().Any() && DataModel.Instance.USModels.OnlyClient().OnlyConnected().All(us => us.Status.Value is Define.Status.Started))
                {
                    DataModel.Instance.USModels.Mine().Status.Value = Define.Status.Started;
                }
                else
                {
                    DataModel.Instance.USModels.Select(us => us.Status)
                        .Merge()
                        .Where(_ => DataModel.Instance.USModels.OnlyClient().OnlyConnected().Any())
                        .Where(_ => DataModel.Instance.USModels.OnlyClient().OnlyConnected().All(us => us.Status.Value is Define.Status.Started))
                        .FirstOrDefault()
                        .Subscribe(_ => DataModel.Instance.USModels.Mine().Status.Value = Define.Status.Started)
                        .AddTo(gameObject);
                }
            }

            SpaceTransform.Instance.SendTRS();

            Time.timeScale = 0;
        }).AddTo(gameObject);

        OnTrainingPlay.Subscribe(_ =>
        {
            Logger.Log("Subscribe OnTrainingPlay");
            Time.timeScale = 1;

            DataModel.Instance.USModels.Mine().Status.Value = Define.Status.Playing;
        }).AddTo(gameObject);

        OnTrainingPause.Subscribe(_ =>
        {
            Logger.Log("Subscribe OnTrainingPause");
            Time.timeScale = 0;

            DataModel.Instance.USModels.Mine().Status.Value = Define.Status.Pause;
        }).AddTo(gameObject);

        OnScenarioStart.Subscribe(_ =>
        {
            Logger.Log("Subscribe OnScenarioStart");
            ScenarioSystem.Instance.ScenarioStart();
        }).AddTo(gameObject);

        OnNextScenarioEvent.Subscribe(_ =>
        {
            Logger.Log($"Subscribe NextScenarioEvent");
            ScenarioSystem.Instance.NextScenarioEvent();
        }).AddTo(gameObject);

        OnTrainingStop.Subscribe(async _ =>
        {
            Logger.Log("Subscribe OnTrainingStop");
            await ScenarioSystem.Instance.TrainingStop();
        }).AddTo(gameObject);

        OnTrainingFinish.Subscribe(async _ =>
        {
            Logger.Log("Subscribe OnTrainingFinish");
            await ScenarioSystem.Instance.TrainingFinish();
        }).AddTo(gameObject);
    }

    #endregion

    #region RPC Methods

    [NetworkRPC]
    void RPC_NotifyOnSceneChange(string scenario) => OnSceneChange.OnNext(scenario);

    [NetworkRPC]
    void RPC_NotifyOnTrainingPlay() => OnTrainingPlay.OnNext(default);

    [NetworkRPC]
    void RPC_NotifyOnTrainingPause() => OnTrainingPause.OnNext(default);

    [NetworkRPC]
    void RPC_NotifyOnScenarioStart() => OnScenarioStart.OnNext(default);

    [NetworkRPC]
    void RPC_NotifyOnNextScenarioEvent() => OnNextScenarioEvent.OnNext(default);

    [NetworkRPC]
    void RPC_NotifyOnTrainingStop() => OnTrainingStop.OnNext(default);

    [NetworkRPC]
    void RPC_NotifyOnTrainingFinish() => OnTrainingFinish.OnNext(default);

    #endregion
}