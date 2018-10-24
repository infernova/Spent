using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SerializableDatetime), true)]
public class DateTimeDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		SerializedProperty s = property.FindPropertyRelative("mDateTimeString");

		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, position.height),
			s,
			label);
		EditorGUI.indentLevel = indent;
	}
}