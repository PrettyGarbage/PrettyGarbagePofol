[System.Serializable]
public class AssetBundleInfo {

    public AssetBundleType assetBundleType;

    public int id;
    public string url;
    public string crc;
    public string hash;
    public string rootName;
    public long fileSize;

    public override string ToString()
    {
        return "id : " + id
            + "\n url : " + url
            + "\n crc : " + crc
            + "\n hash : " + hash
            + "\n fileSize : " + fileSize
             ;
    }
}
