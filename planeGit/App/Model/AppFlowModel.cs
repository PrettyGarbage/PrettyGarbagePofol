using Common;

public class AppFlowModel : AppContext<AppFlowModel>
{
    ///<summary>
    ///AppFlowState에 대한 StateMachine
    ///AppFlowPresenter의 Start에서 시작된다.
    ///고로 각 State에 대한 이벤트 구독 처리는 Awake에서 하는 것을 권장한다.
    ///</summary>
    public GenericStateMachine<Define.AppFlowState> AppFlow { get; } = new();
}