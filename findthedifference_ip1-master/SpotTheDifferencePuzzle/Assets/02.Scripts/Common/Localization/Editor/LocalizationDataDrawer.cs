using UnityEditor;
using UnityEngine;

//[CustomPropertyDrawer(typeof(LocalizationItem))]
public class LocalizationDataDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.LabelField(position, label);

        var nameRect = new Rect(position.x, position.y + 18, position.width, 16);
        var ageRect = new Rect(position.x, position.y + 36, position.width, 16);
        var genderRect = new Rect(position.x, position.y + 54, position.width, 16);

        EditorGUI.indentLevel++;

        EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("key"));
        EditorGUI.PropertyField(ageRect, property.FindPropertyRelative("valueKr"));
        EditorGUI.PropertyField(genderRect, property.FindPropertyRelative("valueEn"));
        EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("valueJp"));

        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }

}
