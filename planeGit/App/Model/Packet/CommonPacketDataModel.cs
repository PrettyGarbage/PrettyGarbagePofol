using Common;
using Newtonsoft.Json.Linq;
using System;
using UniRx;

public class CommonPacketData
{
    public static int serialNumber = 0;
    
    public string name;
    public string unique;
    public int version;
    public string sender;
    public string receiver;
    public int deviceIndex;
    public int serial;
    public int tick;
    public string dateTime;
    
    public static JObject Create(CommonPacketData commonPacketData)
    {
        commonPacketData.unique = Guid.NewGuid().ToString();
        commonPacketData.sender ??= DataModel.Instance.USModels.Mine().Role.ToString();
        commonPacketData.receiver ??= "All";
        commonPacketData.serial = serialNumber++;
        commonPacketData.dateTime = DateTime.UtcNow.ToString("o");
        JObject commonPacket = JObject.FromObject(commonPacketData);
        
        return commonPacket;
    }
}