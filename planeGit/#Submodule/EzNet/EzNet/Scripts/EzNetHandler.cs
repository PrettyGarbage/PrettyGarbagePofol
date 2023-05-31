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
using EzNetLibrary;

/// <summary>
/// 핸들러 컴포넌트
/// </summary>
[DefaultExecutionOrder(-17000)]
public class EzNetHandler : MonoBehaviour
{
    public bool showPacketLog = false;

    [System.Serializable]
    public enum HandlerType
    {
        Client,
        Observer,
        OPS
    }
    public HandlerType handlerType;
    [Header("호스트일 경우")]
    public bool openTCPServer = false;
    public int openPort = 20000;
    [Header("플레이어->옵저버IP | 옵저버->운영통제IP")]
    public string observerIP = "192.168.0.10";
    public int observerPort = 20001;
    [Header("플레이어->옵저버IP | 옵저버->운영통제IP")]
    public string opsIP = "192.168.0.11";
    public int opsPort = 20002;
    [Header("내부망")]
    public int broadCastPort = 30000;
    [Header("현재 space")]
    private Dictionary<Channel, ChannelData> channelDatas;
    public TextAsset channelJsonFile;

    #region base variables

    public static string mySenderUUID;
    public List<Channel> joinedChannels = new List<Channel>();
    public static EzNetLibrary.Space mySpace;

    #endregion

    #region internal variables

    internal static EzNetHandler main;
    internal BroadcastReceiver udpReceiver;
    internal BraodcastSender udpSender;
    internal TCPServer tcpServer;
    internal TCPClient tcpClient;
    internal MulticastSender multicastSender;
    internal MulticastReceiver multicastReceiver;

    internal bool isUdpSenderInitialized = false;
    internal bool isUdpReceiverInitialized = false;
    internal bool isTcpConnected = false;
    internal bool isTcpServerOpened = false;

    // Scene내 인스턴스
    internal Dictionary<string, EzNetObject> netObjects;

    #endregion

    #region private variables

    Queue<string> recvQueue = new Queue<string>();
    JObject jObject;
    Dictionary<string, string> recvData;
    CancellationTokenSource tokenSource = new CancellationTokenSource();

    #endregion

    #region events

    //public event EventHandler OnDisconnected;
    //public event EventHandler OnConnected;

    #endregion

    #region unity

    private void Awake()
    {
    }
    private void OnEnable()
    {
        if (main == null)
        {
            main = this;
            if (handlerType == HandlerType.Observer || handlerType == HandlerType.Observer)
            {
                openTCPServer = true;
            }

            try
            {
                channelDatas = JsonConvert.DeserializeObject<Dictionary<Channel, ChannelData>>(channelJsonFile.text);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Debug.LogError("[EzNetHandler] 채널 정보 Json 파일이 없습니다!");
                throw;
            }
            mySenderUUID = Guid.NewGuid().ToString();

            jObject = new JObject();
            netObjects = new Dictionary<string, EzNetObject>();

            // Receiver 세팅
            udpReceiver = new BroadcastReceiver();
            new Task(new Action(udpReceiver.Start)).Start();
            // Receiver 세팅
            multicastReceiver = new MulticastReceiver();
            new Task(new Action(multicastReceiver.Start)).Start();

            if (openTCPServer)
            {
                // Server 세팅
                tcpServer = new TCPServer();
                new Task(new Action(tcpServer.Start)).Start();
            }

            // Sender 세팅
            udpSender = new BraodcastSender();
            new Task(new Action(udpSender.Start)).Start();
            multicastSender = new MulticastSender();
            new Task(new Action(multicastSender.Start)).Start();
            if (!openTCPServer)
            {
                // Client 세팅
                tcpClient = new TCPClient();
                new Task(new Action(tcpClient.Start)).Start();
            }
        }
        else
        {
            Debug.LogError("[EzNet] EzNetHandler는 2개 이상 존재할 수 없습니다.");
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if (recvQueue.Count > 0)
        {
            string json = recvQueue.Dequeue();
            recvData = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            if (showPacketLog)
                Debug.Log(json);
            _FindTargetAndCallMethod();
        }
    }
    private void OnApplicationQuit()
    {
        main.tokenSource.Dispose();
        Debug.Log("[EzNet] Task disposing successful!");
    }

    #endregion

    #region functions

    private void _FindTargetAndCallMethod()
    {
        // 해당 인스턴스 탐색 후 메서드 호출
        EzNetPacketBase eznetBase = EzNet.Read<EzNetPacketBase>("eznetBase");
        if (showPacketLog)
            Debug.Log("Incoming: " + eznetBase);

        if (eznetBase.sender_uuid == mySenderUUID)
        {
            if (eznetBase.send_to == EzNet.SendTo.All
                || eznetBase.send_to == EzNet.SendTo.Host)
            {
                return;
            }
        }
        switch (eznetBase.where_space)
        {
            case EzNet.WhereSpace.MySpace:
                if (EzNet.GetMySpace() != eznetBase.sent_space)
                {
                    return;
                }
                break;
            case EzNet.WhereSpace.AllSpace:
                break;
            case EzNet.WhereSpace.OtherSpace:
                if (EzNet.GetMySpace() == eznetBase.sent_space)
                {
                    return;
                }
                break;
            default:
                break;
        }
        string id = eznetBase.from_id;
        if (eznetBase.target_type == EzNetObjectType.Dynamic)
        {
            if (netObjects.ContainsKey(id))
            {
                netObjects[id].gameObject.SendMessage(eznetBase.target_method);
            }
            else
            {
                // 다이나믹 객체가 없을 경우 생성
                EzNet._CANCEL_ONENABLE = true;
                try
                {
                    GameObject prefab = Resources.Load<GameObject>(eznetBase.prefab_path);// PrefabUtilityExtensions.FindPrefabByName(eznetBase.prefab_name);
                    if (prefab != null)
                    {
                        // Instantiate the prefab
                        GameObject newObject = Instantiate(prefab);
                        newObject.GetComponent<PlayerController>().isOwn = false;
                        newObject.GetComponent<EzNetObject>().id = EzNet.ReadPacketBase().from_id;
                        newObject.GetComponent<EzNetObject>().prefabPath = eznetBase.prefab_path;

                        EzNet.RegisterNetObject(newObject.GetComponent<EzNetObject>());
                        newObject.gameObject.SendMessage(eznetBase.target_method);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                EzNet._CANCEL_ONENABLE = false;

                // 콜백
                foreach (var item in netObjects)
                {
                    try
                    {
                        item.Value.SendMessage(eznetBase.target_methodn);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
        else
        {
            if (netObjects.ContainsKey(id))
            {
                netObjects[id].gameObject.SendMessage(eznetBase.target_method);
            }
        }
    }
    internal string GetRecvData(string key)
    {
        return recvData[key];
    }
    internal JObject GetJObject()
    {
        return jObject;
    }
    internal string GetSerializedJObject()
    {
        return JsonConvert.SerializeObject(EzNetHandler.main.GetJObject());
    }

    #endregion

    #region internal classes Broadcast

    internal class BroadcastReceiver
    {
        public async void Start()
        {
            try
            {
                UdpClient listener = new UdpClient(new IPEndPoint(IPAddress.Any, main.broadCastPort));
                IPEndPoint any = new IPEndPoint(IPAddress.Any, 0);

                Debug.Log("[EzNet][BroadcastReceiver] Receiver started on " + main.broadCastPort);
                EzNetHandler.main.isUdpReceiverInitialized = true;
                while (true)
                {
                    try
                    {
                        if (listener.Available > 0)
                        {
                            byte[] buf = listener.Receive(ref any);
                            string recvData = Encoding.UTF8.GetString(buf);
                            //Debug.Log(recvData);
                            EzNetHandler.main.recvQueue.Enqueue(recvData);
                        }
                        await Task.Delay(10, main.tokenSource.Token);
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        continue;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }
    }
    internal class BraodcastSender
    {
        UdpClient udp;
        IPEndPoint broad;
        public async void Start()
        {
            await Task.Delay(10);
            udp = new UdpClient();
            udp.EnableBroadcast = true;
            udp.Client.ReceiveTimeout = 1000;
            broad = new IPEndPoint(IPAddress.Broadcast, main.broadCastPort);
            EzNetHandler.main.isUdpSenderInitialized = true;

            // 콜백 호출
            Delegate[] connectedCallbacks = EzNet.UDP_BROADCAST.GetInvocationOnConnected();
            if (connectedCallbacks != null)
            {
                foreach (Action callback in connectedCallbacks)
                {
                    try
                    {
                        callback();
                    }
                    catch (Exception e)
                    {
                        EzNet.UDP_BROADCAST.UnregisterConnectedCallback(callback);
                    }
                }
            }
        }
        public void Send(string sendData)
        {
            byte[] buf = Encoding.UTF8.GetBytes(sendData);
            udp.Send(buf, buf.Length, broad);
        }
        public void Send(string sendData, IPEndPoint multicastEP)
        {
            // Multicast 종단점 설정
            IPEndPoint multicast = new IPEndPoint(IPAddress.Parse("225.0.0.0"), 5500);
            byte[] buf = Encoding.UTF8.GetBytes(sendData);
            udp.Send(buf, buf.Length, multicastEP);
        }
        ~BraodcastSender()
        {
            // 콜백 호출
            foreach (Action callback in EzNet.UDP_BROADCAST.GetInvocationOnDisconnected())
            {
                try
                {
                    callback();
                }
                catch (Exception e)
                {
                    EzNet.UDP_BROADCAST.UnregisterDisconnectedCallback(callback);
                }
            }
        }
    }

    #endregion

    #region internal classes Multicast

    internal class MulticastReceiver
    {
        class MulticastSocket
        {
            public UdpClient udpClient;
            public ChannelData channelData;
            public MulticastSocket(ChannelData channelData)
            {
                udpClient = new UdpClient();
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, channelData.port));
                udpClient.JoinMulticastGroup(IPAddress.Parse(channelData.ip));
                Debug.Log("[EzNet][MulticastReceiver] new UdpClient");
            }
            ~MulticastSocket()
            {
                udpClient.DropMulticastGroup(IPAddress.Parse(((IPEndPoint)udpClient.Client.LocalEndPoint).Address.ToString()));
                udpClient.Close();
            }
        }
        List<MulticastSocket> multicastSockets = new List<MulticastSocket>();
        public async void Start()
        {
            try
            {
                IPEndPoint any = new IPEndPoint(IPAddress.Any, 0);

                foreach (var channel in main.joinedChannels)
                {
                    ChannelData channelData = main.channelDatas[channel];

                }
                foreach (var channel in main.joinedChannels)
                {
                    ChannelData channelData = main.channelDatas[channel];
                    multicastSockets.Add(new MulticastSocket(channelData));
                    Debug.Log("[EzNet][MulticastReceiver] Receiver started on");
                    Debug.Log("[EzNet][MulticastReceiver] IP: " + channelData.ip + ", Port: " + channelData.port);
                }

                while (true)
                {
                    try
                    {
                        foreach (var listener in multicastSockets)
                        {
                            if (listener.udpClient.Available > 0)
                            {
                                byte[] buf = listener.udpClient.Receive(ref any);
                                string recvData = Encoding.UTF8.GetString(buf);
                                //Debug.Log("[EzNet][MulticastReceiver] Received data: " + recvData);
                                //Debug.Log(recvData);
                                EzNetHandler.main.recvQueue.Enqueue(recvData);
                            }
                        }
                        await Task.Delay(10, main.tokenSource.Token);
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        continue;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }
    }
    internal class MulticastSender
    {
        class MulticastSocket
        {
            public UdpClient udpClient;
            public ChannelData channelData;
            public MulticastSocket(ChannelData channelData)
            {
                this.channelData = channelData;
                udpClient = new UdpClient();
                udpClient.JoinMulticastGroup(IPAddress.Parse(channelData.ip));
                Debug.Log("[EzNet][MulticastSender] new UdpClient");
            }
            ~MulticastSocket()
            {
                udpClient.DropMulticastGroup(IPAddress.Parse(((IPEndPoint)udpClient.Client.LocalEndPoint).Address.ToString()));
                udpClient.Close();
            }
        }
        List<MulticastSocket> multicastSockets = new List<MulticastSocket>();
        public async void Start()
        {
            try
            {
                await Task.Delay(10, main.tokenSource.Token);
                foreach (var channel in main.joinedChannels)
                {
                    ChannelData channelData = main.channelDatas[channel];

                    multicastSockets.Add(new MulticastSocket(channelData));
                    Debug.Log("[EzNet][MulticastSender] Sender started on");
                    Debug.Log("[EzNet][MulticastSender] IP: " + channelData.ip + ", Port: " + channelData.port);
                }

                // 콜백 호출
                Delegate[] connectedCallbacks = EzNet.UDP_MULTICAST.GetInvocationOnConnected();
                if (connectedCallbacks != null)
                {
                    foreach (Action callback in connectedCallbacks)
                    {
                        try
                        {
                            callback();
                        }
                        catch (Exception e)
                        {
                            EzNet.UDP_MULTICAST.UnregisterConnectedCallback(callback);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        ~MulticastSender()
        {
            // 콜백 호출
            Delegate[] connectedCallbacks = EzNet.UDP_MULTICAST.GetInvocationOnDisconnected();
            if (connectedCallbacks != null)
            {
                foreach (Action callback in connectedCallbacks)
                {
                    try
                    {
                        callback();
                    }
                    catch (Exception e)
                    {
                        EzNet.UDP_MULTICAST.UnregisterDisconnectedCallback(callback);
                    }
                }
            }
        }

        public void Send(Channel channel, string sendData)
        {
            try
            {
                ChannelData channelData = main.channelDatas[channel];

                Debug.Log("[EzNet][MulticastSender] Sending data to IP: " + channelData.ip + ", Port: " + channelData.port);

                foreach (MulticastSocket socket in multicastSockets)
                {
                    if (socket.channelData.ip == channelData.ip && socket.channelData.port == channelData.port)
                    {
                        IPEndPoint localEndPoint = (IPEndPoint)socket.udpClient.Client.LocalEndPoint;
                        byte[] buf = Encoding.UTF8.GetBytes(sendData);
                        socket.udpClient.Send(buf, buf.Length, new IPEndPoint(IPAddress.Parse(channelData.ip), channelData.port));
                        Debug.Log("[EzNet][MulticastSender] Send Data on " + channel);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }
    }

    #endregion

    #region internal classes TCP

    internal class TCPServer
    {
        public List<AccpetedClient> clients = new List<AccpetedClient>();
        public Queue<AccpetedClient> removeClientQueue = new Queue<AccpetedClient>();
        private TcpListener tcpListener;

        public async void Start()
        {
            tcpListener = new TcpListener(IPAddress.Any, main.openPort);
            tcpListener.Start();
            Debug.Log("[EzNet][TCPServer] Server started on " + main.openPort);
            EzNetHandler.main.isTcpServerOpened = true;
            while (true)
            {
                while (removeClientQueue.Count > 0)
                {
                    main.tcpServer.clients.Remove(removeClientQueue.Dequeue());
                    Debug.Log("[EzNet][TCPServer] Client Successfully Disconnected.");
                }
                if (tcpListener.Pending())
                {
                    if (EzNetHandler.main.showPacketLog)
                        Debug.Log("[EzNet][TCPServer] AcceptTcpClient... " + main.openPort);
                    clients.Add(new AccpetedClient(tcpListener.AcceptTcpClient()));
                }
                /*for (int i = 0; i < clients.Count; i++)
                {
                    if (EzNetHandler.main.showPacketLog)
                        Debug.Log("[EzNet][TCPServer] Loop 3 " + main.openPort);
                    clients[i].Receive();
                }*/
                await Task.Delay(10, main.tokenSource.Token);
            }
        }
        public class AccpetedClient
        {
            bool isConnected = false;
            public TcpClient acceptedClient;
            public StreamWriter writer;
            public StreamReader reader;
            NetworkStream ns;
            public AccpetedClient(TcpClient _acceptedClient)
            {
                acceptedClient = _acceptedClient;
                ns = acceptedClient.GetStream();

                UTF8Encoding jsonEncoding = new UTF8Encoding(false);
                writer = new StreamWriter(ns, jsonEncoding);
                reader = new StreamReader(ns, jsonEncoding);
                isConnected = true;
                Debug.Log("[EzNet][TCPServer] New client accepted!");
                new Task(new Action(Receive)).Start();
            }
            public void Send(string sendData)
            {
                try
                {
                    writer.WriteLine(sendData);
                    writer.Flush();
                }
                catch (Exception)
                {
                    isConnected = false;
                    main.tcpServer.removeClientQueue.Enqueue(this);
                }
            }
            public async void Receive()
            {
                try
                {
                    while (isConnected)
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;

                        while ((bytesRead = await ns.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            string recvData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            EzNetHandler.main.recvQueue.Enqueue(recvData);
                        }
                    }
                    /*if (EzNetHandler.main.showPacketLog)
                        Debug.Log("[EzNet][TCPServer] Wait Data..." + reader.Peek());
                    if (!reader.EndOfStream)
                    {
                        if (EzNetHandler.main.showPacketLog)
                            Debug.Log("[EzNet][TCPServer] !reader.EndOfStream..." + reader.Peek());
                        string recvData = reader.ReadLine();
                        if (EzNetHandler.main.showPacketLog)
                            Debug.Log("[EzNet][TCPServer] Received Data: " + recvData);
                        EzNetHandler.main.recvQueue.Enqueue(recvData);
                    }*/
                }
                catch (Exception e)
                {
                    Debug.Log("[EzNet][TCPServer] Error receiving data from client: " + e.Message);
                }
            }
        }
    }
    internal class TCPClient
    {
        TcpClient client;
        NetworkStream ns;
        StreamWriter writer;
        StreamReader reader;
        public async void Start()
        {
            try
            {
                Debug.Log("[EzNet][TCPClient] Connecting to server..." + main.observerIP + ":" + main.observerPort);
                client = new TcpClient(main.observerIP, main.observerPort);

                ns = client.GetStream();

                UTF8Encoding jsonEncoding = new UTF8Encoding(false);
                writer = new StreamWriter(ns, jsonEncoding);
                reader = new StreamReader(ns, jsonEncoding);

                new Task(new Action(ListenerThread)).Start();
                Debug.Log("[EzNet][TCPClient] Connected successfully!");
                //main.OnConnected(main, EventArgs.Empty);
                EzNetHandler.main.isTcpConnected = true;

                // 콜백 호출
                Delegate[] connectedCallbacks = EzNet.TCP_CLIENT.GetInvocationOnConnected();
                if (connectedCallbacks != null)
                {
                    foreach (Action callback in connectedCallbacks)
                    {
                        try
                        {
                            callback();
                        }
                        catch (Exception e)
                        {
                            EzNet.TCP_CLIENT.UnregisterConnectedCallback(callback);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //Error
                Debug.LogError("[EzNet][TCPClient] Connection failed!");
                Debug.LogError(e.Message);
                main.isTcpConnected = false;

                await Task.Delay(1000, main.tokenSource.Token);
                Debug.LogError("[EzNet][TCPClient] ReConnecting to server...");
                new Task(new Action(Start)).Start();
            }
        }
        public async void ListenerThread()
        {
            Debug.Log("[EzNet][TCPClient] Listenning started.");
            while (true)
            {
                await Task.Delay(10, main.tokenSource.Token);
                try
                {
                    if (!reader.EndOfStream)
                    {
                        string readedData = reader.ReadLine();
                        try
                        {
                            EzNetHandler.main.recvQueue.Enqueue(readedData);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("[EzNet][TCPClient] JSON Parsing failed!");
                            Debug.LogError(e);
                            Debug.LogError(readedData);
                            break;
                        }
                    }
                }
                catch (ObjectDisposedException e)
                {
                    Debug.LogError(e);
                    break;
                }
                catch (Exception e)
                {
                    // 콜백 호출
                    Delegate[] connectedCallbacks = EzNet.TCP_CLIENT.GetInvocationOnDisconnected();
                    if (connectedCallbacks != null)
                    {
                        foreach (Action callback in connectedCallbacks)
                        {
                            try
                            {
                                callback();
                            }
                            catch (Exception)
                            {
                                EzNet.TCP_CLIENT.UnregisterDisconnectedCallback(callback);
                            }
                        }
                    }
                    Debug.LogError("[EzNet][TCPClient] Connection lost!");
                    Debug.LogError(e);

                    await Task.Delay(1000);
                    new Task(new Action(Start)).Start();
                    //main.OnDisconnected(this, EventArgs.Empty);
                    main.isTcpConnected = false;
                    break;
                }
            }
        }
        public void Send(string sendData)
        {
            try
            {
                if (EzNetHandler.main.showPacketLog)
                    Debug.Log("[EzNet][TCPClient] Send Data: " + sendData);
                writer.WriteLine(sendData);
                writer.Flush();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error sending message: {ex.Message}");
            }
        }
    }

    #endregion
}