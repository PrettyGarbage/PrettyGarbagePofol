using System.Threading;
using Cysharp.Threading.Tasks;
using Manager;

public class Managers : AppContext<Managers>
{
    #region Singleton
    public static Managers GetInstance() => Instance;

    private readonly ResourceManager resource = new ResourceManager();
    private readonly SceneManagerEx scene = new SceneManagerEx();
    public static ResourceManager Resource => Instance.resource;
    public static SceneManagerEx Scene => Instance.scene;

    #endregion

    #region LifeCycle

    //비 싱글톤 생성자 사용 방지
    protected Managers() { }

    private async void Start()
    {
        if(Instance != this) return;
        
        await InitAsync();
        
        //Forget() 은 await을 사용하지 못하는 경우나 사용하지 않을 경우
        //경고 처리를 무시하기 위함.
        UpdateLoop(this.GetCancellationTokenOnDestroy()).Forget();
    }

    ///<summary>
    ///각 매니저들 초기화
    ///</summary>
    private async UniTask InitAsync()
    {
        Instance.resource.Initialize();
        Instance.scene.Initialize();

        await UniTask.Yield();
    }
    
    private async UniTaskVoid UpdateLoop(CancellationToken cancellationToken)
    {
        while (true)
        {
            //Update로 실행되는 친구들 처리는 여기에서 실행 
            //ex) PlayerController에서 움직임을 처리한다면 여기서
            //PlayerController의 Update를 호출해주면 됨.

            //Yield() 루프 타이밍을 양보
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }
        //ReSharper disable once FunctionNeverReturns
    }

    #endregion
}
