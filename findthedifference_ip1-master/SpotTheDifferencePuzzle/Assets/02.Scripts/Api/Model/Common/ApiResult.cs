[System.Serializable]
public class ApiResult<T>{

    public ApiUserInfo user;
    public bool isSuccess;
    public GameErrorInfo errorInfo;
    public T result;

    public ApiResult(bool isSuccess, T result)
    {
        this.isSuccess = isSuccess;
        this.result = result;
    }

}