using Common;
using Cysharp.Threading.Tasks;
using MJ.Network;
using Newtonsoft.Json.Linq;
using System;
using UniRx;
using UnityEngine;

[Serializable]
public class UserStateModel : PacketModel
{
    [Serializable]
    public class PacketData
    {
        public string localIp;
        public bool isConnected;
        public string role;
        public string status;
        public string eventStatus;
        public string eventCode;
        public int missionCode;
        public float positionX;
        public float positionY;
        public float positionZ;
        public float heading;
        public string action;
    }

    public UserStateModel(Define.Role role)
    {
        Role.Value = role;

        IsConnected.Subscribe(value =>
        {
            if (value) Player = PlayerSpawnSystem.Instance.Spawn(role);
            else
            {
                Logger.Log($"{Role.Value} 연결 종료");
                PlayerSpawnSystem.Instance.Despawn(Player);
                Player = null;
            }
        });

        //OnReceivePacket.Throttle(TimeSpan.FromSeconds(15f)).Subscribe(_ =>
        //{
        //    Logger.LogError($"{Role.Value}가 15초간 패킷 수신이 없어 연결을 종료합니다.");
        //    Disconnect();
        //});
    }

    public override CommonPacketData CommonPacketData => new()
    {
        name = Define.PacketNameType.US.ToString(),
        version = 1,
        unique = "0",
        sender = "CC",
        receiver = Define.Role.TCS.ToString(),
        deviceIndex = (int)Role.Value,
        tick = 1000
    };

    public override object Data
    {
        get
        {
            return new PacketData
            {
                localIp = LocalIP,
                isConnected = IsConnected.Value,
                role = Role.Value.ToString(),
                status = Status.Value.ToString(),
                eventCode = EventCode.Value,
                eventStatus = EventStatus.Value.ToString(),
                missionCode = MissionCode.Value,
                positionX = Position.Value.x,
                positionY = Position.Value.y,
                positionZ = Position.Value.z,
                heading = Heading.Value,
                action = Action.Value.ToString()
            };
        }
        set
        {
            var packet = (PacketData)value;
            LocalIP = packet.localIp;
            IsConnected.Value = packet.isConnected;
            Role.Value = (Define.Role)Enum.Parse(typeof(Define.Role), packet.role);
            Status.Value = (Define.Status)Enum.Parse(typeof(Define.Status), packet.status);
            EventCode.Value = packet.eventCode;
            EventStatus.Value = (Define.EventStatus)Enum.Parse(typeof(Define.EventStatus), packet.eventStatus);
            MissionCode.Value = packet.missionCode;
            Position.Value = new Vector3(packet.positionX, packet.positionY, packet.positionZ);
            Heading.Value = packet.heading;
            Action.Value = (Define.Action)Enum.Parse(typeof(Define.Action), packet.action);
        }
    }

    public string LocalIP { get; set; }

    public BoolReactiveProperty IsConnected { get; private set; } = new();

    //User의 Index
    public ReactiveProperty<Define.Role> Role { get; private set; } = new();

    //User의 현재 상태
    public ReactiveProperty<Define.Status> Status { get; private set; } = new();

    //User의 현재 진행중인 Event 코드
    public StringReactiveProperty EventCode { get; private set; } = new();

    //진행중인 Event의 상태
    public ReactiveProperty<Define.EventStatus> EventStatus { get; private set; } = new();

    //현재 미션에 대한 코드 
    public ReactiveProperty<int> MissionCode { get; private set; } = new();

    //User의 현재 위치
    public ReactiveProperty<Vector3> Position { get; private set; } = new();

    //User의 Heading Direction (Rotation의 z값)
    public ReactiveProperty<float> Heading { get; private set; } = new();

    public ReactiveProperty<Define.Action> Action { get; private set; } = new();

    public Player Player { get; set; }
    public TcpClient RpcClient { get; set; }

    public Subject<PacketData> OnReceivePacket { get; private set; } = new();

    #region Public Methods

    public override bool DeserializationCondition(JObject packet)
    {
        string role = packet["data"]["role"].ToString();
        return role == Role.Value.ToString()
               && role != DataModel.Instance.USModels.Mine().Role.Value.ToString();
    }

    public override void Deserialize(JObject packet)
    {
        PacketData packetData = packet["data"].ToObject<PacketData>();
        
        Data = packetData;
        
        OnReceivePacket.OnNext(packetData);
    }

    #endregion

    #region Private Methods

    public void Reconnect()
    {
        Status.Value = Define.Status.Initial;
        EventStatus.Value = Define.EventStatus.Unknown;
        EventCode.Value = string.Empty;
        MissionCode.Value = 0;
        Position.Value = Vector3.zero;
        Heading.Value = 0f;
        Action.Value = Define.Action.Unknown;
    }
    
    public void Disconnect() => Data = new PacketData
    {
        role = Role.Value.ToString(), 
        status = Define.Status.Unknown.ToString(), 
        eventStatus =  Define.EventStatus.Unknown.ToString(),
        action = Define.Action.Unknown.ToString()
    };

    #endregion
}