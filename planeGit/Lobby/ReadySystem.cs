using Common;
using UniRx;
using UnityEngine;

public class ReadySystem : SceneContext<ReadySystem>
{
    protected override void Awake()
    {
        base.Awake();

        SteamVRInputSystem.Instance.OnTriggerStateDown.Subscribe(_ =>
        {
            if (DataModel.Instance.Mine.Status.Value == Define.Status.Initial) DataModel.Instance.Mine.Status.Value = Define.Status.TraineeReady;
            else if (DataModel.Instance.Mine.Status.Value == Define.Status.TraineeReady) DataModel.Instance.Mine.Status.Value = Define.Status.Initial;
        }).AddTo(gameObject);
        
        Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Space)).Subscribe(_ =>
        {
            if (DataModel.Instance.Mine.Status.Value == Define.Status.Initial) DataModel.Instance.Mine.Status.Value = Define.Status.TraineeReady;
            else if (DataModel.Instance.Mine.Status.Value == Define.Status.TraineeReady) DataModel.Instance.Mine.Status.Value = Define.Status.Initial;
        }).AddTo(gameObject);
    }
}