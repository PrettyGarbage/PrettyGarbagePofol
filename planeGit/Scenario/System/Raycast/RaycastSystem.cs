using Common;
using UnityEngine;
using UniRx;

public class RaycastSystem : SceneContext<RaycastSystem>
{
    public class RayCast
    {
        public RaycastPresenter _presenter;
        public RaycastModel _model;
        public RaycastView _view;
    }
    private RayCast _raycastR;
    private RayCast _raycastL;

    ///<summary>
    ///래이캐스트 이벤트를 시작한다
    ///=>손에 Ray 붙여서 확인
    ///RaycastSystem.Instance.StartRaycast("EVENTCODE");
    ///</summary>
    ///<param name="eventCode"></param>
    public void StartRaycast()
    {
        Logger.Log("### StartRaycast");
        if (_raycastR == null)
            _raycastR = CreateRaycastObj(false);
        if (_raycastL == null)
            _raycastL = CreateRaycastObj(true);

        StartRayEvent(_raycastR);
        StartRayEvent(_raycastL);
        
        
        SteamVRInputSystem.Instance.OnTriggerStateDown.Where(_ => _raycastR._model.IsRay)
            .Subscribe(_ => Logger.Log("in Right TriggerEnter"));
        SteamVRInputSystem.Instance.OnTriggerStateDown.Where(_ => _raycastL._model.IsRay)
            .Subscribe(_ => Logger.Log("in Left TriggerEnter"));
    }

    ///<summary>
    ///레이캐스트 이벤트 종료
    ///</summary>
    public void EndRaycast()
    {
        StopRayEvent(_raycastR);
        StopRayEvent(_raycastL);
    }

    ///<summary>
    ///래이이벤트에 오브젝트가 있는지 확인
    ///</summary>
    public bool GetIsRay(int index) => index == 0 ? _raycastL._model.IsRay : _raycastR._model.IsRay;

    ///<summary>
    ///래이이벤트에 오브젝트 return
    ///</summary>
    public GameObject GetRayItem(int index)
    {
        return index == 0 ? _raycastL._model.RayObj : _raycastR._model.RayObj;
    }

    private RayCast CreateRaycastObj(bool isLeft)
    {
        Logger.Log("### CreateRaycastObj");
        var rayObj = Managers.Resource.Instantiate(
            Constants.PrefabModule("Raycast"), ((isLeft) ?  Valve.VR.InteractionSystem.Player.instance.hands[0].transform : Valve.VR.InteractionSystem.Player.instance.hands[1].transform) 
        );

        return new RayCast()
        {
            _model =  new RaycastModel(),
            _presenter =   rayObj.GetComponent<RaycastPresenter>(),
            _view = rayObj.GetComponent<RaycastView>()
        };
    }

    private void StartRayEvent(RayCast raycast)
    {
        raycast._presenter.OnColliderEnter.Subscribe(hit =>
        {
            raycast._model.SetRayRslt(hit.point, hit.collider.gameObject);
            raycast._view.ShowLines(hit.collider.bounds.center);
        }).AddTo();
        raycast._presenter.OnColliderExit.Subscribe(_ =>
        {
            raycast._model.RemoveRayRslt();
            raycast._view.HideLines();
        }).AddTo();
        raycast._presenter.StartRaycastCheck();
    }

    private void StopRayEvent(RayCast raycast)
    {
        if(raycast == null) return;
        
        raycast._presenter.StopRaycast();
        raycast._model.RemoveRayRslt();
        raycast._view.HideLines();
    }
}
