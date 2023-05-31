using Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AllCrewTrainingResultPacket
{
    [Serializable]
    public class PacketData
    {
        public CrewTrainingResultPacket.PacketData[] results;
    }

    public static void SendPacket(PacketData data)
    {
        var packet = CommonPacketData.Create(new CommonPacketData()
        {
            name = Define.PacketNameType.ATR.ToString(),
            deviceIndex = DataModel.Instance.MyId
        });
        
        packet.Add("data", JToken.FromObject(data));
        
        NetworkSystem.Instance.SendCTRPacket(packet.ToString());
    }
}

[Serializable]
public class CrewTrainingResultPacket
{
    [Serializable]
    public class PacketData
    {
        [Serializable]
        public class Mission
        {
            public string missionDescription;
            public string result;
            public string resultDescription;
        }
        
        public string eventCode; //(캐싱되어 있음)
        public int testNo; //(캐싱되어 있음)
        public string role; //(캐싱되어 있음)
        public string playerID; //플레이어 id (캐싱되어 있음)
        public string eventStartTime;
        public string eventEndTime;
        public string result;
        public Mission[] missions;
    }

    public static void SendPacket(PacketData data)
    {
        var packet = CommonPacketData.Create(new CommonPacketData()
        {
            name = Define.PacketNameType.CTR.ToString(),
            deviceIndex = DataModel.Instance.MyId
        });
        
        packet.Add("data", JToken.FromObject(data));

        Debug.Log(packet.ToString());
        
        NetworkSystem.Instance.SendCTRPacket(packet.ToString());
    }
}
