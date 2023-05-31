using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkModel : AppContext<NetworkModel>
{
    #region Struct

    public struct NetworkSetting
    {
        public string networkName;
        public string ip;
        public string port;
        public string localIP;
    }

    #endregion

    #region Properties

    public List<NetworkSetting> Settings { get; set; }

    #endregion
}