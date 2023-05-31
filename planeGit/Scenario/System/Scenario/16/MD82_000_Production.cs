using Cysharp.Threading.Tasks;


public class MD82_000_Production : ScenarioEventProduction
{
    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("MD82_000_Production.OnPrevStartMission");

        await UniTask.Yield();
    }

    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("MD82_000 종료");
    }
}