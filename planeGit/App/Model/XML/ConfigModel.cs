using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigModel : AppContext<ConfigModel>
{
    #region Struct

    public struct PlayerSetting
    {
        public int role;
        public string observerIP;
        public string multicastIP;
        public bool debugMode;
    }

    #endregion
    
    #region Properties

    public PlayerSetting Setting { get; set; }

    #endregion
}