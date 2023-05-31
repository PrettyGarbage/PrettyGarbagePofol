using System;
using Common;
using UnityEngine;

public class Player : PlayerBase, IIdentifier
{
    #region Fields

    [SerializeField] VideoCapture videoCapture;

    public Define.Role Role { get; set; }

    #endregion

    #region Life Cycle

    protected override void Awake()
    {
        base.Awake();
        videoCapture = GetComponentInChildren<VideoCapture>();
    }

    void Start()
    {
        foreach (var hand in Valve.VR.InteractionSystem.Player.instance.hands)
        {
            if(hand && hand.mainRenderModel) hand.mainRenderModel.GetSkeleton().updatePose = true;
        }
    }

    #endregion

    #region Public Method

    public void SetPlugin()
    {
        var plugin = Managers.Resource.Instantiate(Constants.VrPluginPath);
        plugin.transform.SetParent(transform);
    }

    ///<summary>
    ///비디오 컴포넌트 가져오는 API
    ///VideoCapture는 자신(Mine Client)에서만 이벤트가 발생해야하기 때문에
    ///IsMine일 때만 컴포넌트를 반환한다.
    ///</summary>
    ///<returns></returns>
    /// todo : 자기 자신일 때만 가져오도록 세이프티 코드 추가 필요
    public override VideoCapture GetVideoCapture() => videoCapture;

    #endregion
}
