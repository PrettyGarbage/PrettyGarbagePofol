using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants {

    //Scene
    public const string SCENE_MAIN = "Main";

    //API
    public const string PLAYERPREFS_API_TOKEN = "apijwt";
    
    public const string API_TOKEN_PREFIX = "Bearer ";
    public const string API_HEADER_AUTHORIZATION = "Authorization";
    public const string API_CONTENT_TYPE_TEXT = "Content-Type";
    public const string API_CONTENT_TYPE_JSON = "application/json";

    //USER DATA
    public const int USER_HEART_MAX_COUNT = 5;
    public const string DATA_ISONSOUND = "IsOnSound";
    public const string DATA_ITEM_COUNT = "ItemCount";

    // time_bonus(이어하기) 50초    
    public const int INGAME_ADD_TIME_TIMEBONUS = 50;
    // time_bonus(이어하기 - 광고) 30초    
    public const int INGAME_ADD_TIME_TIMEBONUS_AD = 30;
    // extra_time(추가시간) 30초
    public const int INGAME_ADD_TIME_EXTRATIME = 30;
    public const float INGAME_ITEM_FREEZE_TIME = 9f;

    //Puzzle Size
    public const float PUZZLE_IMAGE_GAME_SIZE = 950;
    public const float PUZZLE_IMAGE_SIZE = 512;
    public const float PUZZLE_IMAGE_SIZE_RATE = PUZZLE_IMAGE_GAME_SIZE / PUZZLE_IMAGE_SIZE;
	
    // Color
    public static readonly Color TRANSPARENT = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    public static readonly Color OPAQUE = new Color(1.0f, 1.0f, 1.0f,1.0f);

    // Messages
    public const string MSG_DATA_ERROR_TITLE = "Data Error";
    public const string MSG_NOT_MATCHED_THEME_LEVEL_TOKENS = "The <color=#FF0000>{0}</color> theme doesn't have any image parts to match the <color=#FF0000>{1}</color> difficulty({2}).";
    public const string MSG_NOT_MATCHED_CHALLENGE_LEVE_TOKENS = "Images doesn't have  any image parts to match the <color=#FF0000>{0}</color> difficulty({1}).";

    public const string MSG_CHALLENGE_RESULT_BEST_SCORE = "Best: {0}";
    public const string MSG_CHALLENGE_RESULT_SCORE = "Score: {0}";

    public const string MSG_THEME_COMPLETE_BASIC = "{0}\nMaster OPEN!!";
    public const string MSG_THEME_COMPLETE_MASTER = "{0}\nComplete!!";

    // Ad
    public const int INTERSTITIAL_AD_THEME_ROUND_COMPLETE_INTERVAL = 10;
    public const string AD_BANNER_HOME = "banner_main_bottom_left";
    public const string AD_BANNER_INGAME = "banner_playing_top_left";
    public const string AD_INTERSTITIAL_THEME_ROUND_COMPLETE = "interstitial_thememode_ending";
    public const string AD_INTERSTITIAL_CHALLENGE_END = "interstitial_challengemode_ending";
    public const string AD_VIDEO_REWARD_EXTRA_TIME = "reward_challengemode_extratime";
    public const string AD_VIDEO_REWARD_HINT_ITEM = "reward_thememode_hint";

    //rewarItem
    public const int REWARD_ATTAINABLE_STARPOINT_IN_STAGE = 5;

    //Combo Info
    public const string COMBO = "COMBO";
    
    public const float STATE_OPEN_FADE_TIME = 0.03f;
    public const float STATE_CLOSE_FADE_TIME = 0.01f;

    //Game Effect Animation
    public const string IS_EFFECT = "isEffect";

    //DailyReward
    public const int DAILY_REWARD_ITEMCOUNT = 7;

    //Text
    public const string TEXT_THEMENAME = "THEMENAME_";
    public const string TEXT_DAILY_REWARD_DAY = "LOBBY_EVENT_WEEKLY_{0}DAY";

    //Theme Card
    public const string STAGE_DESC = "Stage ";

    //Ready passive item use or unuse
    public const string GAME_READY_PASSIVE_ITEM_USE = "use";
    public const string GAME_READY_PASSIVE_ITEM_UNUSE = "unuse";

    //spiin event
    public const string SPIN_EVENT_JACKPOT = "jackpot";

    //bundle
    public const string ASSETBUNDLE_THUMB = "_thumb";
    public const string ASSETBUNDLE_THEME_PUZZLE = "_puzzle_";

    public const string DATA_USERTHEMEDIFFICULTY = "UserThemeDifficulty";

    public const int COLLECTION_MAX_ZOOM = 3;
}
