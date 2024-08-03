[System.Serializable]
public class UserCollectionInfo {

    public int userId;
    public string collectionId;
    public long regTimestamp;
    
    public override string ToString()
    {
        return "\n userId : " + userId
            + "\n collectionId : " + collectionId
            + "\n regTimestamp : " + regTimestamp
             ;
    }

}
