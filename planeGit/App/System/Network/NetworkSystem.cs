using Common;
using Library.Network;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using __MyAssets._02.Scripts.VoiceChat.Remake;
using UniRx;
using UnityEngine;

public class NetworkSystem : AppContext<NetworkSystem>
{
    #region Fields

    string observerIP = "192.168.0.0";
    string multicastIP = "225.0.1.0";
    
    List<IDeserializableData> receiveDataList = new();

    #endregion

    #region Properties

    public UdpReceiver USReceiver { get; private set; }
    public UdpSender USSender { get; private set; }
    public UdpReceiver OPSReceiver { get; private set; }
    public UdpSender CTRSender { get; private set; }
    public UdpReceiver CTRReceiver { get; private set; }

    #endregion

    #region Unity Lifecycle

    void OnApplicationQuit()
    {
        DataModel.Instance.USModels.Mine().Disconnect();
        USSender?.SendDatagram(DataModel.Instance.USModels.Mine().Serialize());
        USSender?.Dispose();
        USReceiver?.Dispose();
        OPSReceiver?.Dispose();
        CTRSender?.Dispose();
        CTRReceiver?.Dispose();
        RPCSession.Dispose();
    }

    #endregion

    #region Public Methods

    public void Connect()
    {
        DataModel.Instance.USModels.Mine().LocalIP = NetworkUtil.LocalIP;
        DataModel.Instance.USModels.Mine().IsConnected.Value = true;
        DataModel.Instance.USModels.Mine().Status.Value = Define.Status.Initial;

        StartNetwork();
    }

    public void AddReceiveData(IDeserializableData data)
    {
        if (!receiveDataList.Contains(data)) receiveDataList.Add(data);
    }

    public void SendCTRPacket(string packet) => CTRSender?.SendDatagram(packet);

    #endregion

    #region Private Methods

    void StartNetwork()
    {
        if(ConfigModel.Instance.Setting.debugMode) return;
        
        observerIP = ConfigModel.Instance.Setting.observerIP;
        multicastIP = ConfigModel.Instance.Setting.multicastIP;
        
        StartRPCServer();
        StartRPCClient();
        StartUSSender();
        StartUSAndOPSReceiver();
        StartCTRSender();
        StartCTRReceiver();
    }

    private void StartRPCServer()
    {
        RPCSession.BindingServer();
    }

    private void StartRPCClient()
    {
        DataModel.Instance.USModels.OnlyOther().ForEach(us =>
        {
            us.IsConnected.Where(isConnected => isConnected).Subscribe(async _ => us.RpcClient = await RPCSession.ConnectClient(us.LocalIP));
            us.IsConnected.Where(isConnected => !isConnected).Subscribe(_ => RPCSession.DisconnectClient(us.RpcClient));
        });
    }

    private void StartUSSender()
    {
        USSender = UdpSender.Bind(
            ip: DataModel.Instance.USModels.Observer().IsMine() ? multicastIP : observerIP,
            port: DataModel.Instance.USModels.Observer().IsMine() ? 20231 : 20230
        );
        // USSender.OnSend.Subscribe(bytes => Logger.Log($"Send : {Encoding.UTF8.GetString(bytes)}"));

        Observable.Interval(TimeSpan.FromSeconds(1), Scheduler.MainThreadIgnoreTimeScale).Subscribe(_ =>
        {
            if (DataModel.Instance.USModels.Observer().IsMine())
                DataModel.Instance.USModels.ForEach(data => USSender.SendDatagram(data.Serialize()));
            else
                USSender.SendDatagram(DataModel.Instance.USModels.Mine().Serialize());
        }).AddTo(gameObject);
    }

    private void StartUSAndOPSReceiver()
    {
        USReceiver = UdpReceiver.Bind(
            ip: DataModel.Instance.USModels.Observer().IsMine() ? observerIP : multicastIP,
            port: DataModel.Instance.USModels.Observer().IsMine() ? 20230 : 20231,
            type: DataModel.Instance.USModels.Observer().IsMine() ? UDPType.Broadcast : UDPType.Multicast
        );

        OPSReceiver = UdpReceiver.Bind(
            ip: multicastIP,
            port: 20211,
            type: UDPType.Multicast
        );

        USReceiver.OnReceived.Merge(OPSReceiver.OnReceived).Subscribe(bytes =>
        {
            string json = Encoding.UTF8.GetString(bytes);
            var packet = JObject.Parse(json);
            
            foreach (var data in receiveDataList)
            {
                if (data.TryDeserialize(packet))
                {
                    // Logger.Log($"Received : {json}");
                    break;
                }
            }
        }).AddTo(gameObject);
    }

    private void StartCTRSender()
    {
        CTRSender = UdpSender.Bind(
            ip: DataModel.Instance.USModels.Observer().IsMine() ? multicastIP : observerIP,
            port: DataModel.Instance.USModels.Observer().IsMine() ? 20211 : 20239
        );
    }

    private void StartCTRReceiver()
    {
        if (DataModel.Instance.USModels.Observer().IsMine())
        {
            CTRReceiver = UdpReceiver.Bind(
                ip: observerIP,
                port: 20239,
                type: UDPType.Broadcast
            );

            CTRReceiver.OnReceived.Subscribe(bytes => CTRSender.SendDatagram(bytes)).AddTo(gameObject);
        }
    }

    #endregion
}