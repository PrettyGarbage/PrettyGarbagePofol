public interface IMobilePlatform {

    void Init();
    void LogIn(ActionBool onLogIn = null);
    void ShowLeaderBoard();
    void AddScoreToLeaderBorad(long score);
    void LogOut();
    bool IsLoggedIn();
    string GetUserId();


}
