using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using MJ.Network;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;

namespace Library.Network
{
    public static class RPCSession
    {
        #region Properties

        static Dictionary<int, RPCView> rpcViews = new();

        static List<TcpClient> rpcClients = new();
        static TcpServer rpcServer;

        #endregion

        #region Public Methods

        public static RPCView GetNetworkView(int viewID) => rpcViews[viewID];

        public static void RegisterNetworkView(RPCView view)
        {
            Logger.Log($"Register RPCView : {view.ViewID} {view.gameObject.name}");
            rpcViews.TryAdd(view.ViewID, view);
        }

        public static void RPC(RPCView view, string methodFullName, params object[] parameters)
        {
            // 메세지 생성
            string message = $"{methodFullName}({string.Join(", ", parameters.Select(param => param.GetType().Name))})";

            // 패킷 생성
            JObject packet = new();
            packet.Add("viewID", view.ViewID);
            packet.Add("message", message);
            packet.Add("parameters", JArray.FromObject(parameters));
            Logger.Log($"### Send RPC : {packet}");

            if (!view.RPCMethods.TryGetValue(message, out var methods))
            {
                Logger.LogError($"RPC method '{methodFullName}' not found.");
            }
            else
            {
                // 로컬 호출
                foreach (var method in methods) method.info.Invoke(method.obj, parameters);

                // 원격 호출 요청 송신
                rpcClients.ForEach(rpc => rpc.Send(packet.ToString()));
            }
        }

        #endregion

        #region Private Methods

        // RPC 송신용 TCP Client Binding
        public static async UniTask<TcpClient> ConnectClient(string ip)
        {
            IPEndPoint endpoint = new(IPAddress.Parse(ip), 50001);
            var rpcClient = new TcpClient();
            await rpcClient.Bind(endpoint);
            rpcClient.OnReceive.Subscribe(x =>
            {
                string s = Encoding.ASCII.GetString(x);
                Logger.Log(s);
            });
            rpcClients.Add(rpcClient);
            Logger.Log($"### ConnectClient {ip}");

            return rpcClient;
        }

        public static void DisconnectClient(TcpClient rpcClient)
        {
            if(rpcClients.Contains(rpcClient))
            {
                rpcClients.Remove(rpcClient);
                rpcClient.Dispose();
                Logger.Log($"### DisconnectClient");
            }
        }

        // RPC 수신용 TCP Server Binding
        public static void BindingServer()
        {
            rpcServer = new TcpServer();
            IPEndPoint endpoint = new(IPAddress.Parse(NetworkUtil.LocalIP), 50001);
            rpcServer.Bind(endpoint);
            
            Logger.Log($"### BindingServer {NetworkUtil.LocalIP}");
            
            rpcServer.ReceiveDataObservable.ObserveOnMainThread().Subscribe(data =>
            {
                var json = Encoding.ASCII.GetString(data);
                var packet = JObject.Parse(json);
                var viewID = packet["viewID"].Value<int>();
                var message = packet["message"].Value<string>();
                var parameters = packet["parameters"].ToObject<object[]>();

                Logger.Log($"### Received RPC : {packet}");
                
                RPCView view = GetNetworkView(viewID);
                if (view == null) Logger.LogError($"ViewID {viewID} not found");
                else
                {
                    if (!view.RPCMethods.TryGetValue(message, out var methods)) Logger.LogError($"RPC method '{message}' not found.");
                    else
                    {
                        foreach (var method in methods)
                        {
                            method.info.Invoke(method.obj, parameters);
                        }
                    }
                }
            });
        }

        public static void Dispose()
        {
            rpcServer?.Dispose();
            foreach (var client in rpcClients) client?.Dispose();
        }

        #endregion
    }
}