using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using EzNetLibrary;

[DefaultExecutionOrder(-16000)]
public class EzNetObject : MonoBehaviour
{
    public EzNetObjectType objectType = EzNetObjectType.Static;
    public string id = "";
    public string prefabPath = "";
    bool isRemoteDestroy = false;
    public EzNet.EzNetObjectOwner myOwner;

    #region unity

    void OnEnable()
    {
        if (!EzNet._CANCEL_ONENABLE)
        {
            if (id == "")
            {
                switch (objectType)
                {
                    case EzNetObjectType.Dynamic:
                        id = Guid.NewGuid().ToString();
                        myOwner = new EzNet.EzNetObjectOwner();
                        try
                        {
                            //prefabPath = PrefabUtility.GetCorrespondingObjectFromSource(gameObject).name;
                        }
                        catch (Exception)
                        {
                        }
                        break;
                    case EzNetObjectType.Static:
                        id = gameObject.GetInstanceID().ToString();
                        break;
                    default:
                        break;
                }
            }
            EzNet.RegisterNetObject(this);
        }
    }
    private void OnDestroy()
    {
        if (!isRemoteDestroy)
            SendBroadcast(EzNet.WhereSpace.AllSpace, nameof(RemoteDestroy));
    }
    private void OnApplicationQuit()
    {
        if (!isRemoteDestroy)
            SendBroadcast(EzNet.WhereSpace.AllSpace, nameof(RemoteDestroy));
    }
    public void RemoteDestroy()
    {
        isRemoteDestroy = true;
        Destroy(gameObject);
    }
    #endregion

    #region EzNet Methods

    /// <summary>
    /// <para>특정 Channel로 UDP Multicast 전송합니다. 목적지 인스턴스의 메서드로 데이터를 전송합니다.</para>
    /// 식별을 위해 EzNetObject 컴포넌트가 필요합니다.<br/>
    /// methodNameForNotFound 지정시 인스턴스가 없을 경우 정적 인스턴스로 전달합니다.
    /// </summary>
    /// <param name="whereChannel">보낼 Channel</param>
    public void SendMulticast(Channel whereChannel, string methodName, string methodNameForNotFound = "")
    {
        EzNet.SendMulticast(whereChannel, this, methodName, methodNameForNotFound);
    }

    /// <summary>
    /// <para>특정 Space로 UDP Broadcast 전송합니다. 목적지 인스턴스의 메서드로 데이터를 전송합니다.</para>
    /// 식별을 위해 EzNetObject 컴포넌트가 필요합니다.<br/>
    /// methodNameForNotFound 지정시 인스턴스가 없을 경우 정적 인스턴스로 전달합니다.
    /// </summary>
    /// <param name="whereSpace">보낼 Space</param>
    public void SendBroadcast(EzNet.WhereSpace whereSpace, string methodName, string methodNameForNotFound = "")
    {
        EzNet.SendBroadcast(whereSpace, this, methodName, methodNameForNotFound);
    }

    /// <summary>
    /// <para>특정 Space로 TCP 전송합니다. 목적지 인스턴스의 메서드로 데이터를 전송합니다.</para>
    /// 식별을 위해 EzNetObject 컴포넌트가 필요합니다.<br/>
    /// methodNameForNotFound 지정시 인스턴스가 없을 경우 정적 인스턴스로 전달합니다.
    /// </summary>
    /// <param name="sendToWho">보낼 대상</param>
    /// <param name="whereSpace">보낼 Space</param>
    /// 
    public void SendTCP(EzNet.SendTo sendToWho, EzNet.WhereSpace whereSpace, string methodName, string methodNameForNotFound = "")
    {
        EzNet.SendTCP(sendToWho, whereSpace, this, methodName, methodNameForNotFound);
    }

    #endregion
}