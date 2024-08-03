[System.Serializable]
public class WebReqeustErrorInfo
{
    //"timestamp": 1553578953992,
    //"status": 401,
    //"error": "Unauthorized",
    //"message": "Invalid JWT token: ",
    //"path":
    public long timestamp;
    public int status;
    public string error;
    public string message;
    public string path;

    public override string ToString()
    {
        return "timestamp : " + timestamp
            + "\n status : " + status
            + "\n error : " + error
            + "\n message : " + message
            + "\n path : " + path
            ;
    }
}
