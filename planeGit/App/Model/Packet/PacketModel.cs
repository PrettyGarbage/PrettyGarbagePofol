using Newtonsoft.Json.Linq;
using UnityEngine;

public abstract class PacketModel : ISerializableData, IDeserializableData
{
    public abstract CommonPacketData CommonPacketData { get; }
    public abstract object Data { get; set; }
    public virtual bool DeserializationCondition(JObject packet) => true;

    public string Serialize()
    {
        var packet = CommonPacketData.Create(CommonPacketData);
        packet.Add("data", JToken.FromObject(Data));
        
        return packet.ToString();
    }

    public virtual void Deserialize(JObject packet) { }

    public bool TryDeserialize(JObject packet)
    {
        if (packet["name"].Value<string>() != CommonPacketData.name) return false;
        if (!DeserializationCondition(packet)) return false;

        Deserialize(packet);
        return true;
    }
}
