using System;
using Common;
using UnityEngine;

namespace Manager
{
    public class ManagerEx : MonoBehaviour
    {
        private void Start()
        {
            var testGo = Managers.Resource.Instantiate(Constants.PrefabChar("player1"));
            testGo.transform.position = Vector3.one;
            testGo.transform.rotation = Quaternion.identity;
            //이하 속성 세팅 testGo.property setting!
            
            Managers.Resource.Destroy(testGo);
        }
    }
}