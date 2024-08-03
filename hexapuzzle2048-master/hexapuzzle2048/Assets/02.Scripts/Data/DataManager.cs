using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using FlatBuffers;
using FlatBuffersModel;

public class DataManager : MonoBehaviour
{
    private static DataManager _instance;
    private UserData _userData;

    [SerializeField]
    private GameData _gameData;

    public static DataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = ObjectUtil.CreateInstance<DataManager>("DataManager");
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    public UserData UserData
    {
        get
        {
            return _userData;
        }
    }

    public GameData GameData
    {
        get
        {
            return _gameData;
        }
    }

    public IEnumerator Init()
    {
        LoadUserData();
        yield return null;

    }

    private void LoadUserData()
    {
        if (File.Exists(GameConstants.SAVEFILE_PATH))
        {
            LoadUserDataFile();
        }
        else
        {
            _userData = new UserData(0, GameData.InitUserCoin, null, new UserRewardInfo());
            SaveUserDataFile();
        }
    }

    public void LoadUserDataFile()
    {
        Debug.Log("=============LoadUserDataFile " + GameConstants.SAVEFILE_PATH);
        ByteBuffer bb = new ByteBuffer(File.ReadAllBytes(GameConstants.SAVEFILE_PATH));
        UserModel userMode = UserModel.GetRootAsUserModel(bb);

        int score = userMode.Score;
        int coin = userMode.Coin;
        PlayData playData = null;
        if (userMode.PlayData.HasValue)
        {
            //lastparts
            LastBlocks lastParts = null;
            PlayModel playModel = userMode.PlayData.Value;
            LastPartsModel? lastPartsModel = playModel.LastPartsData;
            if (lastPartsModel.HasValue)
            {
                lastParts = new LastBlocks(lastPartsModel.Value.GetCellPartsArrayArray(), lastPartsModel.Value.AngleValue);
            }

            //cells
            int[,] cells = new int[GameConstants.CELL_ARRAY_SIZE, GameConstants.CELL_ARRAY_SIZE];

            for (int i = 0; i < playModel.CellArrayLength; i++)
            {
                //Debug.Log("CellArrayLength = " + i + " / CellArrayLength : " + playModel.CellArrayLength);
                int x = playModel.CellArray(i).Value.X;
                int y = playModel.CellArray(i).Value.Y;
                int value = playModel.CellArray(i).Value.Value;
                cells[x, y] = value;
            }

            ItemUseCountInfo itemUseCountInfo = new ItemUseCountInfo();
            for (int i = 0; i < playModel.ItemUseCountArrayLength; i++)
            {
                itemUseCountInfo.AddCount((ItemType)i, playModel.ItemUseCountArray(i));
            }

            playData = new PlayData(playModel.Score, playModel.Coin, cells, lastParts, itemUseCountInfo, playModel.AdContinueWatchCount);
        }
        UserRewardInfo userRewardInfo = new UserRewardInfo(userMode.DailyRewardDay, userMode.CoinBoxRewardDay, userMode.CointBoxRewardCount, userMode.CointBoxRewardTimeStamp);
        _userData = new UserData(score, coin, playData, userRewardInfo);
        Debug.Log("Score : " + _userData.Score);
        Debug.Log("Coin : " + _userData.Coin);
        Debug.Log("DailyRewardDay : " + _userData.UserRewardInfo.DailyRewardDay);
        Debug.Log("CoinBoxRewardDay : " + _userData.UserRewardInfo.CoinBoxRewardDay);
        Debug.Log("CointBoxRewardCount : " + _userData.UserRewardInfo.CoinBoxRewardCount);
        Debug.Log("CointBoxRewardTimeStamp : " + _userData.UserRewardInfo.CointBoxRewardTimeStamp);
        // if(_userData.PlayData !=null)
        //     _userData.PlayData.Print();
    }

    public void SaveUserDataFile()
    {
        FlatBufferBuilder fbb = new FlatBufferBuilder(1024);
        Offset<PlayModel> playModeOffset = new Offset<PlayModel>();
        if (UserData.PlayData != null)
        {
            //lastParts
            int[] lastPartsCells = UserData.PlayData.LastParts.Cells;
            LastPartsModel.StartCellPartsArrayVector(fbb, lastPartsCells.Length);
            for (int i = lastPartsCells.Length - 1; i >= 0; i--)
            {
                fbb.AddInt(lastPartsCells[i]);
            }
            VectorOffset lastPartsCellsOffset = fbb.EndVector();

            LastPartsModel.StartLastPartsModel(fbb);
            LastPartsModel.AddAngleValue(fbb, UserData.PlayData.LastParts.AngleValue);
            LastPartsModel.AddCellPartsArray(fbb, lastPartsCellsOffset);
            Offset<LastPartsModel> lastPartsOffset = LastPartsModel.EndLastPartsModel(fbb);

            //cells
            Offset<int>[] cells = new Offset<int>[UserData.PlayData.LastParts.Cells.Length];

            Offset<CellModel>[] offsetCellModes = new Offset<CellModel>[GameConstants.CELL_ARRAY_SIZE * GameConstants.CELL_ARRAY_SIZE];
            int index = 0;
            for (int i = 0; i < GameConstants.CELL_ARRAY_SIZE; i++)
            {
                for (int j = 0; j < GameConstants.CELL_ARRAY_SIZE; j++)
                {
                    offsetCellModes[index] = CellModel.CreateCellModel(fbb, (short)i, (short)j, UserData.PlayData.Cells[i, j]);
                    index++;
                }
            }

            VectorOffset cellsVectorOffset = PlayModel.CreateCellArrayVector(fbb, offsetCellModes);

            //ItemUserCountArray
            int[] itemUseCountArray = UserData.PlayData.ItemUseCountInfo.ItemUseCountArray;
            PlayModel.StartItemUseCountArrayVector(fbb, itemUseCountArray.Length);
            for (int i = itemUseCountArray.Length - 1; i >= 0; i--)
            {
                Debug.Log((ItemType)i + " - " + itemUseCountArray[i]);
                fbb.AddInt(itemUseCountArray[i]);
            }
            VectorOffset itemUserCountArrayOffset = fbb.EndVector();

            //playmode
            PlayModel.StartPlayModel(fbb);
            PlayModel.AddScore(fbb, UserData.PlayData.Score);
            PlayModel.AddCoin(fbb, UserData.PlayData.Coin);
            PlayModel.AddAdContinueWatchCount(fbb, UserData.PlayData.AdContinueWatchCount);
            PlayModel.AddCellArray(fbb, cellsVectorOffset);
            PlayModel.AddLastPartsData(fbb, lastPartsOffset);
            PlayModel.AddItemUseCountArray(fbb, itemUserCountArrayOffset);
            playModeOffset = PlayModel.EndPlayModel(fbb);

        }

        StringOffset dailyRewardDayOffset = fbb.CreateString(UserData.UserRewardInfo.DailyRewardDay);
        StringOffset coinBoxRewardDayoffset = fbb.CreateString(UserData.UserRewardInfo.CoinBoxRewardDay);

        UserModel.StartUserModel(fbb);
        UserModel.AddScore(fbb, UserData.Score);
        UserModel.AddCoin(fbb, UserData.Coin);
        UserModel.AddDailyRewardDay(fbb, dailyRewardDayOffset);
        UserModel.AddCoinBoxRewardDay(fbb, coinBoxRewardDayoffset);
        UserModel.AddCointBoxRewardCount(fbb, UserData.UserRewardInfo.CoinBoxRewardCount);
        UserModel.AddCointBoxRewardTimeStamp(fbb, UserData.UserRewardInfo.CointBoxRewardTimeStamp);
        UserModel.AddPlayData(fbb, playModeOffset);
        Offset<UserModel> userModelOffset = UserModel.EndUserModel(fbb);

        fbb.Finish(userModelOffset.Value);

        using (var ms = new MemoryStream(fbb.SizedByteArray()))
        {
            File.WriteAllBytes(GameConstants.SAVEFILE_PATH, ms.ToArray());

            Debug.Log("SaveUserDataFile File Write end ");
        }

    }

    public void SaveGameResult(int updateScore, int getCoin, int[] useItems)
    {
        _userData.AddCoin(getCoin);
        if (_userData.UpdateScore(updateScore))
        {
            //Upload Score rank.
            NetworkManager.Instance.SetScoreToLeaderBorad(updateScore);
        }

        ItemUseCountInfo itemUseCountInfo = ItemUseCountInfo.GetInstance(useItems);
        AnalyticsEventParam analyticsEventParam = new AnalyticsEventParam()
                                .AddParam("Score", updateScore)
                                .AddParam("Coin", getCoin)
                                .AddParam("Item_Recycle", itemUseCountInfo.GetCount(ItemType.TRASH))
                                .AddParam("Item_Hammer", itemUseCountInfo.GetCount(ItemType.BREAK))
                                .AddParam("Item_Rewind", itemUseCountInfo.GetCount(ItemType.UNDO));;
        
        AnalyticsManager.Instance.EventLog(GameConstants.EVENTLOG_BASICMODE_RESULT, analyticsEventParam);

        DeletePlayData();
        SaveUserDataFile();
    }

    public void SavePlayData(int score, int coin, int[,] cells, LastBlocks lastBlocks, int[] useItems, int adContinueWatchCount)
    {
        ItemUseCountInfo itemUseCountInfo = ItemUseCountInfo.GetInstance(useItems);
        _userData.SetPlayData(new PlayData(score, coin, cells, lastBlocks, itemUseCountInfo, adContinueWatchCount));
        SaveUserDataFile();
    }

    public PlayData GetPlayData()
    {
        return _userData.PlayData;
    }

    public void DeletePlayData()
    {
        _userData.SetPlayData(null);
    }

    public bool IsExistPlayData()
    {
        return _userData.PlayData != null;
    }

    public void UseItem(ItemType itemType)
    {
        _userData.AddCoin(-(int)itemType);
    }

    /// <summary>
    /// Callback sent to all game objects before the application is quit.
    /// </summary>
    void OnApplicationQuit()
    {
        Debug.Log("DataManager OnApplicationQuit");
        if (CommonManager.Instance && CommonManager.Instance.IsInitComplete)
            SaveUserDataFile();
    }

}