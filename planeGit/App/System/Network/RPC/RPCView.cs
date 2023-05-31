using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Library.Util.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using TriInspector;

namespace Library.Network
{
    [DefaultExecutionOrder(-16000)]
    public class RPCView : MonoBehaviour
    {
        #region Fields

        MonoBehaviour[] behaviours;
        [SerializeField, ReadOnly, HideIf(nameof(viewID), 0)] int viewID;

        #endregion
        
        #region Properties

        [field: SerializeField, ReadOnly, LabelText("View ID"), ShowIf(nameof(viewID), 0)] public int SceneViewID { get; set; }

        public int ViewID
        {
            get => viewID;
            set
            {
                if(viewID != 0) return;
                viewID = value;
            }
        }

        public bool IsMine { get; private set; }
        public ReadOnlyDictionary<string, (MonoBehaviour obj, MethodInfo info)[]> RPCMethods { get; private set; }

        #endregion

        #region Unity Lifecycle

        void Awake()
        {
            InitViewID();
            InitRPC();
            
            RPCSession.RegisterNetworkView(this);
        }

        #endregion
        
        #region Public Methods
        
        public static RPCView Of(Component component) => component.GetComponent<RPCView>();
        public static RPCView Of(GameObject gameObject) => gameObject.GetComponent<RPCView>();
        public static RPCView Of(Transform transform) => transform.GetComponent<RPCView>();
        public static RPCView Find(int viewID) => RPCSession.GetNetworkView(viewID);
        
        public void RPC(string methodFullName, params object[] parameters)
        {
            RPCSession.RPC(this, methodFullName, parameters);
        }

        #endregion

        #region Private Methods

        void InitViewID()
        {
            if(ViewID != 0) return;

            var viewId = SceneViewID;
            Assert.IsTrue(viewId <= 1000, "Scene에 미리 등록된 NetworkView가 1000개를 초과했습니다.");
            
            ViewID = viewId;
        }
        
        void InitRPC()
        {
            behaviours = GetComponentsInChildren<MonoBehaviour>();
            
            RPCMethods = new ReadOnlyDictionary<string, (MonoBehaviour, MethodInfo)[]>(
                behaviours
                    .SelectMany(behaviour => ReflectionUtility.GetMethodsByAttribute(behaviour.GetType(), typeof(NetworkRPCAttribute)).Select(info => (behaviour, info)))
                    .GroupBy(value => $"{value.info.Name}({String.Join(",", value.info.GetParameters().Select(param => param.ParameterType.Name))})")
                    .ToDictionary(group => group.Key, group => group.ToArray())
            );
        }

        #endregion
    }
}