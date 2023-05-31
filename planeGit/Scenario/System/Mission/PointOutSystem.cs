using System;
using System.Linq;
using System.Threading;
using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

public class PointOutSystem : SceneContext<PointOutSystem>
{
    [SerializeField] InteractableObjectModel dummy;

    #region Public Methods

    public async UniTask<CrewTrainingResultPacket.PacketData.Mission> PointOutMissionAsync(Dialogue dialogue, float waitSeconds, bool isSubtitle = true)
    {
        var cts = new CancellationTokenSource();

        if (!dialogue.InteractableObjects.All(x => x))
        {
            var mine = DataModel.Instance.Mine;
            Logger.LogError($"{mine.Role.Value} {mine.EventCode.Value}-{mine.MissionCode.Value} : InteractableObjects에 Null이 존재합니다.");
        }

        // var interactableObjects = dialogue.InteractableObjects.Where(x => x);
        var interactableObjects = dialogue.InteractableObjects.Select(x => x ? x : dummy);

        try
        {
            interactableObjects.ForEach(model => model.IsInteractable = true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        var winIndex = await UniTask.WhenAny(
            isSubtitle ? SubtitleSystem.Instance.ShowSubtitleAsync(dialogue, waitSeconds, cts.Token) : UniTask.Delay(TimeSpan.FromSeconds(waitSeconds), cancellationToken: this.GetCancellationTokenOnDestroy()),
            UniTask.WaitUntil(() => interactableObjects.All(model => model.IsInteractable == false), cancellationToken: this.GetCancellationTokenOnDestroy())
        );

        try
        {
            interactableObjects.ForEach(model => model.IsInteractable = false);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        cts.Cancel();

        var missionResult = new CrewTrainingResultPacket.PacketData.Mission()
        {
            missionDescription = dialogue.Body.Select(body => body.Message).Aggregate((a, b) => $"{a} {b}"),
            result = (winIndex == 1).ToString(),
            resultDescription = (winIndex == 1) ? "성공" : "주어진 시간 안에 지목을 실패했습니다."
        };

        return missionResult;
    }

    #endregion
}