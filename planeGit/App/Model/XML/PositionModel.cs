using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionModel : AppContext<PositionModel>
{
    #region Struct

    public struct PositionSetting
    {
        public string sceneCode;
        public bool isSetEachPosition;
        public bool isDebug;
        public Vector3 position;
    }

    #endregion

    #region Properties
   
    public List<PositionSetting> Settings { get; set; }

    #endregion
}
