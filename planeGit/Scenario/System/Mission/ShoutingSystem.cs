using System.Linq;
using System.Threading;
using __MyAssets._02.Scripts.VoiceChat.Remake;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;


public class ShoutingSystem : SceneContext<ShoutingSystem>
{
    #region Fields

    float voiceTime;
    Subject<Unit> onEndMission = new Subject<Unit>();

    #endregion

    #region Public Methods

    public async UniTask<CrewTrainingResultPacket.PacketData.Mission> ShoutingMissionAsync(Dialogue dialogue, float waitSeconds)
    {
        var cts = new CancellationTokenSource();

        voiceTime = 0;
        float missionTime = Mathf.Max(waitSeconds, dialogue.Body.Where(body => body.Clip).Sum(body => body.Clip.length)) / 4f;

        if (!ConfigModel.Instance.Setting.debugMode)
        {
            VoiceChatManager.Instance.OnCheckVoiceMissionDeltaTime.TakeUntil(onEndMission).Subscribe(_ => voiceTime += Time.deltaTime).AddTo(gameObject);
        }

        await SubtitleSystem.Instance.ShowSubtitleAsync(dialogue, waitSeconds, cts.Token);

        onEndMission.OnNext(Unit.Default);

        cts.Cancel();

        var missionResult = new CrewTrainingResultPacket.PacketData.Mission()
        {
            missionDescription = dialogue.Body.Select(body => body.Message).Aggregate((a, b) => $"{a} {b}"),
            result = (voiceTime >= missionTime).ToString(),
            resultDescription = voiceTime >= missionTime ? "성공" : "주어진 시간 안에 대사를 말하지 못했습니다."
        };

        return missionResult;
    }

    #endregion
}