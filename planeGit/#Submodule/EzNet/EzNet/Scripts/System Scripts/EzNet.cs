using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading;
using UnityEditor;

namespace EzNetLibrary
{
    /// <summary>
    /// EzNet 라이브러리
    /// </summary>
    public static class EzNet
    {
        #region EzNet Static Methods

        /// <summary>
        /// 보낼 데이터를 추가합니다.
        /// </summary>
        public static void Add(string key, object obj)
        {
            EzNetHandler.main.GetJObject().Add(key, JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
        }

        /// <summary>
        /// <para>특정 Channel로 UDP Multicast 전송합니다. 목적지 인스턴스의 메서드로 데이터를 전송합니다.</para>
        /// 식별을 위해 EzNetObject 컴포넌트가 필요합니다.<br/>
        /// methodNameForNotFound 지정시 인스턴스가 없을 경우 정적 인스턴스로 전달합니다.
        /// </summary>
        /// <param name="whereChannel">보낼 Channel</param>
        public static void SendMulticast(Channel whereChannel, EzNetObject netObj, string methodName, string methodNameForNotFound = "")
        {
            try
            {
                EzNet.Add("eznetBase", new EzNetPacketBase(netObj.prefabPath,
                SendTo.All, WhereSpace.AllSpace, GetMySpace(), EzNetHandler.mySenderUUID, netObj.objectType, netObj.myOwner, netObj.id, methodName, methodNameForNotFound));

                EzNetHandler.main.multicastSender.Send(whereChannel, EzNetHandler.main.GetSerializedJObject());
                EzNetHandler.main.GetJObject().RemoveAll();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                EzNetHandler.main.GetJObject().RemoveAll();
            }
        }

        /// <summary>
        /// <para>특정 Space로 UDP Broadcast 전송합니다. 목적지 인스턴스의 메서드로 데이터를 전송합니다.</para>
        /// 식별을 위해 EzNetObject 컴포넌트가 필요합니다.<br/>
        /// methodNameForNotFound 지정시 인스턴스가 없을 경우 정적 인스턴스로 전달합니다.
        /// </summary>
        /// <param name="whereSpace">보낼 Space</param>
        public static void SendBroadcast(WhereSpace whereSpace, EzNetObject netObj, string methodName, string methodNameForNotFound = "")
        {
            try
            {
                EzNet.Add("eznetBase", new EzNetPacketBase(netObj.prefabPath,
                SendTo.All, whereSpace, GetMySpace(), EzNetHandler.mySenderUUID, netObj.objectType, netObj.myOwner, netObj.id, methodName, methodNameForNotFound));

                EzNetHandler.main.udpSender.Send(EzNetHandler.main.GetSerializedJObject());
                EzNetHandler.main.GetJObject().RemoveAll();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                EzNetHandler.main.GetJObject().RemoveAll();
            }
        }

        /// <summary>
        /// <para>특정 Space로 TCP 전송합니다. 목적지 인스턴스의 메서드로 데이터를 전송합니다.</para>
        /// 식별을 위해 EzNetObject 컴포넌트가 필요합니다.<br/>
        /// methodNameForNotFound 지정시 인스턴스가 없을 경우 정적 인스턴스로 전달합니다.
        /// </summary>
        /// <param name="sendToWho">보낼 대상</param>
        /// <param name="whereSpace">보낼 Space</param>
        /// 
        //TODO SendTo 승무원들 간에 어떻게 할지 결정
        public static void SendTCP(SendTo sendToWho, WhereSpace whereSpace, EzNetObject netObj, string methodName, string methodNameForNotFound = "")
        {
            try
            {
                EzNet.Add("eznetBase", new EzNetPacketBase(netObj.prefabPath,
                    sendToWho, whereSpace, GetMySpace(), EzNetHandler.mySenderUUID, netObj.objectType, netObj.myOwner, netObj.id, methodName, methodNameForNotFound));

                switch (sendToWho)
                {
                    case SendTo.All:
                        // TODO
                        break;
                    case SendTo.Host:
                        EzNetHandler.main.tcpClient.Send(EzNetHandler.main.GetSerializedJObject());
                        break;
                    case SendTo.Me:
                        // TODO
                        break;
                    case SendTo.AllMe:
                        // TODO
                        break;
                    case SendTo.AllHost:
                        EzNetHandler.main.tcpClient.Send(EzNetHandler.main.GetSerializedJObject());
                        break;
                    case SendTo.AllHostMe:
                        EzNetHandler.main.tcpClient.Send(EzNetHandler.main.GetSerializedJObject());
                        break;
                    case SendTo.Client:
                        if (EzNetHandler.main.openTCPServer)
                        {
                            foreach (var client in EzNetHandler.main.tcpServer.clients)
                            {
                                client.Send(EzNetHandler.main.GetSerializedJObject());
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            finally
            {
                EzNetHandler.main.GetJObject().RemoveAll();
            }
        }

        public class EzNetObjectOwner
        {
            public EzNetObjectOwner(string localIP)
            {
                _localIP = localIP;
            }
            public EzNetObjectOwner()
            {
                _localIP = GetLocalIPAddress();
            }
            public string _localIP;
            string GetLocalIPAddress()
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                throw new System.Exception("No network adapters with an IPv4 address in the system!");
            }
        }
        /// <summary>
        /// <para>수신된 데이터를 읽습니다.</para>
        /// </summary>
        public static T Read<T>(string key)
        {
            return JsonConvert.DeserializeObject<T>(EzNetHandler.main.GetRecvData(key));
        }
        /// <summary>
        /// <para>수신된 데이터를 읽습니다.</para>
        /// </summary>
        public static EzNetPacketBase ReadPacketBase()
        {
            return Read<EzNetPacketBase>("eznetBase");
        }
        /// <summary>
        /// <para>보낼 대상을 지정합니다.</para>
        /// All: 모두에게 (자신 및 호스트 제외)<br/>
        /// Host: 호스트에게<br/>
        /// Me: 자신에게 (핑 측정 등에 사용)<br/>
        /// AllMe: 자신과 모두에게 (호스트 제외)<br/>
        /// AllHost: 호스트와 모두에게 (자신 제외)<br/>
        /// AllHostMe: 자신, 모두, 호스트에게<br/>
        /// Client: 자신을 호스트로 두고 있는 클라이언트들에게
        /// </summary>
        public enum SendTo
        {
            ///<summary>모두에게 (자신 및 호스트 제외)</summary>
            All,
            ///<summary>호스트에게</summary>
            Host,
            ///<summary>자신에게 (핑 측정 등에 사용)</summary>
            Me,
            ///<summary>자신과 모두에게 (호스트 제외)</summary>
            AllMe,
            ///<summary>호스트와 모두에게 (자신 제외)</summary>
            AllHost,
            ///<summary>자신, 모두, 호스트에게</summary>
            AllHostMe,
            ///<summary>자신을 호스트로 두고 있는 클라이언트들에게</summary>
            Client
        }
        /// <summary>
        /// <para>보낼 Space을 지정합니다.</para>
        /// MySpace: 현재 Space<br/>
        /// AllSpace: 모든 Space<br/>
        /// OtherSpace: 현재 제외한 모든 Space
        /// </summary>
        public enum WhereSpace
        {
            ///<summary>현재 Space</summary>
            MySpace,
            ///<summary>모든 Space</summary>
            AllSpace,
            ///<summary>현재 제외한 모든 Space</summary>
            OtherSpace
        }
        /// <summary>
        /// <para>자신의 Space를 설정합니다.</para>
        /// </summary>
        public static void SetMySpace(Space space)
        {
            EzNetHandler.mySpace = space;
        }
        /// <summary>
        /// <para>Channel에 가입합니다.</para>
        /// </summary>
        public static void JoinChannel(Channel channel)
        {
            EzNetHandler.main.joinedChannels.Clear();
            EzNetHandler.main.joinedChannels.Add(channel);
        }
        /// <summary>
        /// <para>Channel을 탈퇴합니다.</para>
        /// </summary>
        public static void LeaveChannel(Channel channel)
        {
            EzNetHandler.main.joinedChannels.Remove(channel);
        }
        /// <summary>
        /// <para>자신의 현재 Space를 반환합니다.</para>
        /// </summary>
        public static Space GetMySpace()
        {
            return EzNetHandler.mySpace;
        }
        /// <summary>
        /// <para>자신의 현재 Channel을 반환합니다.</para>
        /// </summary>
        public static List<Channel> GetMyChannels()
        {
            return EzNetHandler.main.joinedChannels;
        }

        #endregion

        #region for network object

        public static void RegisterNetObject(EzNetObject netObj)
        {
            if (!EzNetHandler.main.netObjects.ContainsKey(netObj.id))
                EzNetHandler.main.netObjects.Add(netObj.id, netObj);
        }
        /// <summary>
        /// <para>id를 통해 EzNetObject를 찾습니다. 없을 경우 null.</para>
        /// </summary>
        public static EzNetObject FindNetObjectById(string id)
        {
            if (EzNetHandler.main.netObjects.ContainsKey(id))
                return EzNetHandler.main.netObjects[id];
            return null;
        }

        #endregion

        #region for status

        public static bool _CANCEL_ONENABLE = false;

        public static class UDP_BROADCAST
        {
            public static event Action OnDisconnected;
            public static event Action OnConnected;
            /// <summary>
            /// <para>연결이 끊겼을 때 호출될 콜백을 등록합니다.</para>
            /// </summary>
            public static void RegisterDisconnectedCallback(Action callback)
            {
                OnDisconnected += callback;
            }
            /// <summary>
            /// <para>연결되었을 때 호출될 콜백을 등록합니다.</para>
            /// </summary>
            public static void RegisterConnectedCallback(Action callback)
            {
                OnConnected += callback;
            }
            public static void UnregisterDisconnectedCallback(Action callback)
            {
                OnDisconnected -= callback;
            }
            public static void UnregisterConnectedCallback(Action callback)
            {
                OnConnected -= callback;
            }
            public static Delegate[] GetInvocationOnConnected()
            {
                return OnConnected?.GetInvocationList();
            }
            public static Delegate[] GetInvocationOnDisconnected()
            {
                return OnDisconnected?.GetInvocationList();
            }

            /// <summary>
            /// <para>해당 UDP 포트에 추가 연결합니다.</para>
            /// </summary>
            public static void ConnectToNewPort(int port)
            {
            }
            /// <summary>
            /// <para>UDP가 정상적으로 사용가능한 상태일 경우 true를 반환합니다.</para>
            /// </summary>
            public static bool isAvailable()
            {
                return EzNetHandler.main.isUdpReceiverInitialized && EzNetHandler.main.isUdpSenderInitialized;
            }
            /// <summary>
            /// <para>해당 포트의 UDP가 정상적으로 사용가능한 상태일 경우 true를 반환합니다.</para>
            /// </summary>
            public static bool isAvailable(int port)
            {
                return EzNetHandler.main.isUdpReceiverInitialized && EzNetHandler.main.isUdpSenderInitialized;
            }
        }
        public static class UDP_MULTICAST
        {
            public static event Action OnDisconnected;
            public static event Action OnConnected;
            /// <summary>
            /// <para>연결이 끊겼을 때 호출될 콜백을 등록합니다.</para>
            /// </summary>
            public static void RegisterDisconnectedCallback(Action callback)
            {
                OnDisconnected += callback;
            }
            /// <summary>
            /// <para>연결되었을 때 호출될 콜백을 등록합니다.</para>
            /// </summary>
            public static void RegisterConnectedCallback(Action callback)
            {
                OnConnected += callback;
            }
            public static void UnregisterDisconnectedCallback(Action callback)
            {
                OnDisconnected -= callback;
            }
            public static void UnregisterConnectedCallback(Action callback)
            {
                OnConnected -= callback;
            }
            public static Delegate[] GetInvocationOnConnected()
            {
                return OnConnected?.GetInvocationList();
            }
            public static Delegate[] GetInvocationOnDisconnected()
            {
                return OnDisconnected?.GetInvocationList();
            }
            /// <summary>
            /// <para>해당 채널의 UDP 멀티캐스트가 정상적으로 사용가능한 상태일 경우 true를 반환합니다.</para>
            /// </summary>
            public static bool isAvailable(EzNetLibrary.Channel channel)
            {
                return EzNetHandler.main.joinedChannels.Contains(channel);
            }
        }
        public static class TCP_SERVER
        {
            /// <summary>
            /// <para>TCP 서버가 정상적으로 열어져 있는 상태이면 true를 반환합니다.</para>
            /// </summary>
            public static bool isAvailable()
            {
                return EzNetHandler.main.isTcpServerOpened;
            }
            /// <summary>
            /// <para>TCP 서버일 경우 true를 반환합니다.</para>
            /// </summary>
            public static bool isServer()
            {
                return EzNetHandler.main.openTCPServer;
            }
        }
        public static class TCP_CLIENT
        {
            public static event Action OnDisconnected;
            public static event Action OnConnected;
            /// <summary>
            /// <para>연결이 끊겼을 때 호출될 콜백을 등록합니다.</para>
            /// </summary>
            public static void RegisterDisconnectedCallback(Action callback)
            {
                OnDisconnected += callback;
            }
            /// <summary>
            /// <para>연결되었을 때 호출될 콜백을 등록합니다.</para>
            /// </summary>
            public static void RegisterConnectedCallback(Action callback)
            {
                OnConnected += callback;
            }
            public static void UnregisterDisconnectedCallback(Action callback)
            {
                OnDisconnected -= callback;
            }
            public static void UnregisterConnectedCallback(Action callback)
            {
                OnConnected -= callback;
            }
            public static Delegate[] GetInvocationOnConnected()
            {
                return OnConnected?.GetInvocationList();
            }
            public static Delegate[] GetInvocationOnDisconnected()
            {
                return OnDisconnected?.GetInvocationList();
            }
            /// <summary>
            /// <para>TCP 서버에 정상적으로 연결된 상태이면 true를 반환합니다.</para>
            /// </summary>
            public static bool isAvailable()
            {
                return EzNetHandler.main.isTcpConnected;
            }
        }
    }

    #region EzNet Packet Base

    // EzNet 패킷 베이스
    public enum EzNetObjectType
    {
        Dynamic,
        Static
    }
    public class EzNetPacketBase
    {
        [Newtonsoft.Json.JsonConstructor]
        public EzNetPacketBase(string prefab_path, EzNet.SendTo send_to, EzNet.WhereSpace where_space, Space sent_space, string sender_uuid, EzNetObjectType target_type, EzNet.EzNetObjectOwner ezNetObjectOwner, string from_id, string target_method, string target_methodn = "")
        {
            this.prefab_path = prefab_path;
            this.send_to = send_to;
            this.where_space = where_space;
            this.sent_space = sent_space;
            this.sender_uuid = sender_uuid;
            this.target_type = target_type;
            this.ezNetObjectOwner = ezNetObjectOwner;
            this.from_id = from_id;
            this.target_method = target_method;
            this.target_methodn = target_methodn;
        }
        public string prefab_path;
        public EzNet.SendTo send_to;
        public EzNet.WhereSpace where_space;
        public Space sent_space;
        public string sender_uuid;
        public EzNetObjectType target_type;
        public EzNet.EzNetObjectOwner ezNetObjectOwner;
        public string from_id;
        public string target_method;
        public string target_methodn;
    }

    #endregion


    #endregion

}