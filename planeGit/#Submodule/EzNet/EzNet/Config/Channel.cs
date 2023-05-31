using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EzNetLibrary
{
    /// <summary>
    /// <para>미리 정의되어 있는 Multicast 채널입니다.</para>
    /// TSC1: Trainning System Control<br/>
    /// TCS1: Trainning Control System<br/>
    /// APS1: AirPlain Simulator<br/>
    /// CCS1: Cabin Crew Secretaryl<br/>
    /// CC1: Cabin Crew 1<br/>
    /// CC2: Cabin Crew 2<br/>
    /// CC3: Cabin Crew 3<br/>
    /// CC4: Cabin Crew 4<br/>
    /// APW1: AirPlain Window<br/>
    /// GC1: Ground Crew<br/>
    /// </summary>
    [System.Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Channel
    {
        ///<summary>Trainning System Control</summary>
        TSC1,
        ///<summary>Trainning Control System</summary>
        TCS1,
        ///<summary>AirPlain Simulator</summary>
        APS1,
        ///<summary>Cabin Crew Secretary</summary>
        CCS1,
        ///<summary>Cabin Crew 1</summary>
        CC1,
        ///<summary>Cabin Crew 2</summary>
        CC2,
        ///<summary>Cabin Crew 3</summary>
        CC3,
        ///<summary>Cabin Crew 4</summary>
        CC4,
        ///<summary>AirPlain Window</summary>
        APW1,
        ///<summary>Ground Crew</summary>
        GC1,
    }

    [System.Serializable]
    public class ChannelData
    {
        public string ip;
        public int port;
    }
}