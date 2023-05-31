using System.Collections.Generic;

public class XMLManager : AppContext<XMLManager>
{
    public delegate void OnFinishReadXML();
    public OnFinishReadXML onFinishReadXML;

    private XMLReader _xmlReader = new XMLReader();

    ///<summary>
    ///네트워크 Config파일 취득
    ///</summary>
    public List<NetworkModel.NetworkSetting> GetNetworkConfig()
    {
        return _xmlReader.ReadNetworkXML();
    }

    ///<summary>
    ///Player Setting파일 취득
    ///</summary>
    public ConfigModel.PlayerSetting GetPlayerConfig()
    {
        return _xmlReader.ReadConfigFile();
    }

    ///<summary>
    ///Position Setting 파일 취득
    ///</summary>
    public List<PositionModel.PositionSetting> GetPositionConfig()
    {
        return _xmlReader.ReadPositionFile();
    }

}
