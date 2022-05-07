using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// インスペクタ上で確認専用の変更不可表示ができる。 https://kandycodings.jp/2019/03/24/unidev-noneditable/ 参考
// [SerializeField,NotEditable] を冒頭につける。

public sealed class NotEditableAttribute : PropertyAttribute { }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(NotEditableAttribute))]
public sealed class NonEditableAttributeDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
#endif
