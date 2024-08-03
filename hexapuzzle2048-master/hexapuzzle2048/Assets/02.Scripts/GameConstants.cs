using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
	public static readonly string SAVEFILE_PATH = Application.persistentDataPath + "/save.dat";
	public const string SCENE_MAIN = "Main";

	public static readonly Color TRANSPARENT = new Color(1.0f, 1.0f, 1.0f, 0.0f);

	public const float DEFAULT_SCREEN_WIDTH = 1080.0f;
	public const float DEFAULT_SCREEN_HEIGHT = 1920.0f;

    public const int CELL_ARRAY_SIZE = 7;
	public const float HEXA_STEP_ANGLE = 60.0f;

	public const int MAX_CELLS = 2;
	public const int LOWEST_CELL_NUMBER = 2;
	public const int HIGHEST_CELL_NUMBER = 11; // 2^11 == 2048

	public const string STATENAME_INTRO = "Intro";
	public const string STATENAME_TUTORIAL = "Tutorial";
	public const string STATENAME_INGAME = "InGame";
	public const string STATENAME_LOBBY = "Lobby";
	public const string STATENAME_POPUPSETTING = "PopupSetting";
	public const string STATENAME_POPUPPAUSE = "PopupPause";
	public const string STATENAME_POPUPCONTINUE = "PopupContinue";
	public const string STATENAME_POPUPGAMEOVER = "PopupGameover";
	public const string STATENAME_POPUPDAILYREWARD = "PopupDailyReward";
	public const string STATENAME_POPUPCOINBOX = "PopupCoinBox";
	public const string STATENAME_POPUPINSTANCEMAG = "PopupInstanceMag";

	
	#region BGM
	public const string BGM_LOBBY = "bgm_lobby";
 	public const string BGM_INGAME = "bgm_ingame";
	#endregion BGM

	#region SOUNDS
	public const string FX_BLOCK_GRAP = "eff_block_pickup";
	public const string FX_BLOCK_DROP = "eff_block_insert";
	public const string FX_BLOCK_ROTATE = "eff_block_rotation";
	public const string FX_BLOCK_BACK_TO_PLACE = "eff_block_return";

	public const string FX_CELL_COMBINING = "eff_block_sliding";
	public const string FX_CELL_COMBINED = "eff_block_merge";
	public const string FX_CELL_COMBINED_LAST_NUMBER = "eff_bonusblock_make";
	public const string FX_CELL_EXPLODE = "eff_bonusblock_removecell";

	public const string FX_USE_HAMMER = "eff_item_hammer_use";
	public const string FX_MISSION_APPEAR = "ui_mission_nextpopup";

	public static readonly string[] FX_COMBO = {
		"eff_matching_combo1",
		"eff_matching_combo2",
		"eff_matching_combo3",
		"eff_matching_combo4",
		"eff_matching_combo5",
		"eff_matching_combo6",
	};
	

	public const string UIFX_GAME_OVER = "ui_title_gameOver";
	public const string UIFX_TOUCH_BUTTON_DEFAULT = "ui_button_common";
	public const string UIFX_TOUCH_BUTTON_PAUSE = "ui_button_pause";
	public const string UIFX_TOUCH_BUTTON_ITEM = "ui_button_item";
	public const string UIFX_TOUCH_LOCKED_BUTTON_ITEM = "ui_lock_item";
	public const string UIFX_DECREASE_COINS = "eff_get_coin";
	public const string UIFX_THROW_TO_TRASH_CAN = "ui_recycle_block";
	public const string UIFX_RECORD_RENEWAL = "ui_update_hiscore";
	public const string UIFX_NEW_RECORD_STEMP = "ui_bestscore_stemp";
	#endregion SOUNDS

	//AD
	public const string ADUNIT_BANNER_BOTTOM = "banner_bottom";
	public const string ADUNIT_INTERSTITIAL_GAMEEND_REPLAY = "interstitial_gameend_retry";
	public const string ADUNIT_REWARD = "reward_game";
	public const string AD_FAIL_MSG = "No ads from ad server";

	#region TEXTS
	public const string MISSION_TARGET_TEXT = "<color=\"white\">NEXT MISSION</color>  {0:#,##0}";
	public const string MISSION_REWARD_TEXT = "GET {0:#,##0} GOLD";
	#endregion TEXTS

	//Analytics
	public const string EVENTLOG_GBROS_LOGO_SHOW = "Gbros_Logo_show";
	public const string EVENTLOG_LOBBY_SHOW  = "Lobby_show";
	public const string EVENTLOG_BASICMODE_START   = "BasicMode_Start";
	public const string EVENTLOG_BASICMODE_CONTINUE   = "BasicMode_Continue";
	public const string EVENTLOG_BASICMODE_RESULT   = "BasicMode_Result";
}