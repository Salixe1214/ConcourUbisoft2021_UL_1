using UnityEditor;
using UnityEditor.UI;

namespace TechSupport.Informations.Editor
{
	[CustomEditor(typeof(AccordionElement), true)]
	public class AccordionElementEditor : ToggleEditor {
	
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("minHeight"));
			serializedObject.ApplyModifiedProperties();
			
			serializedObject.Update();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_IsOn"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Interactable"));
			serializedObject.ApplyModifiedProperties();
		}
	}
}