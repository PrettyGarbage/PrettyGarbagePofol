using System.Linq;
using Mine.Code.Framework.Manager.UINavigator.Runtime;
using Mine.Code.Framework.Manager.UINavigator.Runtime.Modal;
using UnityEditor;
using UnityEngine;

namespace Mine.Code.Framework.Manager.UINavigator.Editor
{
    [CustomEditor(typeof(ModalContainer))]
    public class ModalContainerEditor : UIContainerEditor
    {
        #region Fields

        SerializedProperty instantiateType;
        SerializedProperty registerModalsByPrefab;
        SerializedProperty registerModalsByAddressable;

        #endregion

        #region Properties

        ModalContainer Target => target as ModalContainer;
        protected override string[] PropertyToExclude() => base.PropertyToExclude().Concat(new[]
        {
            $"<{nameof(ModalContainer.InstantiateType)}>k__BackingField", 
            $"<{nameof(ModalContainer.RegisterModalsByPrefab)}>k__BackingField",
#if ADDRESSABLE_SUPORT
            $"<{nameof(ModalContainer.RegisterModalsByAddressable)}>k__BackingField"
#endif
        }).ToArray();

        #endregion

        #region Unity Lifecycle

        protected override void OnEnable()
        {
            base.OnEnable();
            instantiateType = serializedObject.FindProperty($"<{nameof(ModalContainer.InstantiateType)}>k__BackingField");
            registerModalsByPrefab = serializedObject.FindProperty($"<{nameof(ModalContainer.RegisterModalsByPrefab)}>k__BackingField");
#if ADDRESSABLE_SUPORT
            registerModalsByAddressable = serializedObject.FindProperty($"<{nameof(ModalContainer.RegisterModalsByAddressable)}>k__BackingField");
#endif
        }

        #endregion

        #region GUI Process

        protected override void AdditionalGUIProcess()
        {
            var area = EditorGUILayout.BeginVertical();
            {
                GUI.Box(area, GUIContent.none);
                DrawTitleField("Initialize Setting");
                EditorGUI.indentLevel++;
                {
                    EditorGUILayout.PropertyField(instantiateType, GUIContent.none);
                    switch (Target.InstantiateType)
                    {
                        case InstantiateType.InstantiateByPrefab:
                            EditorGUILayout.PropertyField(registerModalsByPrefab);
                            break;
                        case InstantiateType.InstantiateByAddressable:
                            EditorGUILayout.PropertyField(registerModalsByAddressable);
                            break;
                    }
                }
                EditorGUILayout.Space(9);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        #endregion
    }
}