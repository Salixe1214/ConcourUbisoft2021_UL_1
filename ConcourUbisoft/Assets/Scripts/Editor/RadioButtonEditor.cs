using Buttons;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(RadioButton), true)]
    public class RadioButtonEditor : ToggleEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("value"));
            serializedObject.ApplyModifiedProperties();
            DrawDefaultInspector ();
        }
    }
}
