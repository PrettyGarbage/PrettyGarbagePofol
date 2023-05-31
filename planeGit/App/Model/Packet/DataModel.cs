using Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public interface ISerializableData
{
    CommonPacketData CommonPacketData { get; }
    object Data { get; }
    string Serialize();
}

public interface IDeserializableData
{
    CommonPacketData CommonPacketData { get; }
    object Data { set; }
    bool DeserializationCondition(JObject packet);
    void Deserialize(JObject packet);
    bool TryDeserialize(JObject packet);
}

public static class USModelsExtension
{
    public static IEnumerable<UserStateModel> OnlyOther(this IEnumerable<UserStateModel> usModels) => usModels.Where(us => (int)us.Role.Value != ConfigModel.Instance.Setting.role);
    public static IEnumerable<UserStateModel> OnlyClient(this IEnumerable<UserStateModel> usModels) => usModels.Where(us => us.Role.Value != Define.Role.CCS);
    public static IEnumerable<UserStateModel> OnlyConnected(this IEnumerable<UserStateModel> usModels) => usModels.Where(us => us.IsConnected.Value);
    public static UserStateModel Observer(this IEnumerable<UserStateModel> usModels) => usModels.FirstOrDefault(us => us.Role.Value == Define.Role.CCS);
    public static UserStateModel Mine(this IEnumerable<UserStateModel> usModels) => usModels.FirstOrDefault(us => (int)us.Role.Value == ConfigModel.Instance.Setting.role);
    public static bool IsMine(this UserStateModel usModel) => (int)usModel.Role.Value == ConfigModel.Instance.Setting.role;
}

public class DataModel : AppContext<DataModel>
{
    #region Properties
    
    public OperationStatusModel OPSModel { get; private set; } = new();
    
    public UserStateModel[] USModels { get; private set; }
    
    public int MyId => ConfigModel.Instance.Setting.role;
    public bool IsObserver => Mine.Role.Value == Define.Role.CCS;
    public UserStateModel Mine => USModels[MyId];

    #endregion

    protected override void Awake()
    {
        base.Awake();
        
        if(Instance != this) return;
        
        USModels = new UserStateModel[] { 
            new(Define.Role.CCS),
            new(Define.Role.CC1),
            new(Define.Role.CC2),
            new(Define.Role.CC3),
            new(Define.Role.CC4),
        };

        NetworkSystem.Instance.AddReceiveData(OPSModel);
        USModels.ForEach(model => NetworkSystem.Instance.AddReceiveData(model));
        
        Logger.Log($"DataModel Awake {GetInstanceID()}");
    }

    void OnDestroy()
    {
        Logger.LogError($"{typeof(DataModel)} : {GetInstanceID()} 파괴됩니다.");
    }
}

