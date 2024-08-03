[System.Serializable]
public class AppInfo
{
    public int version;
    public string clientVersion;
    public string cdnUrl;
    public bool maintenanceYn;

    //url
    public string privacyUrl;
    public string eulaUrl;
    public string faqUrl;
    public string jppurchasePolicyUrl;
    public string policyUrl;
    public string qnaUrl;

    public override string ToString()
    {
        return "\n version : " + version
            + "\n clientVersion : " + clientVersion
            + "\n cdnUrl : " + cdnUrl
            + "\n maintenanceYn : " + maintenanceYn
            + "\n qnaUrl : " + qnaUrl
             ;
    }

}