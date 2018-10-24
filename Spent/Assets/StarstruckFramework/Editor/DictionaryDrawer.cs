using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

namespace StarstruckFramework
{
	[CustomPropertyDrawer(typeof(DrawableDictionary), true)]
	public class DictionaryPropertyDrawer : PropertyDrawer
	{

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (property.isExpanded)
			{
				var keysProp = property.FindPropertyRelative("_keys");
				return (keysProp.arraySize + 2.5f) * (EditorGUIUtility.singleLineHeight + 2);
			}
			else
			{
				return EditorGUIUtility.singleLineHeight;
			}
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			bool expanded = property.isExpanded;
			var r = GetNextRect(ref position);
			property.isExpanded = EditorGUI.Foldout(r, property.isExpanded, label);

			if (expanded)
			{
                int indentLevel = EditorGUI.indentLevel;

				EditorGUI.indentLevel++;

				var keysProp = property.FindPropertyRelative("_keys");
				var valuesProp = property.FindPropertyRelative("_values");

				int cnt = keysProp.arraySize;
				if (valuesProp.arraySize != cnt)
					valuesProp.arraySize = cnt;

				for (int i = 0; i < cnt; i++)
				{
					r = GetNextRect(ref position);
					r = EditorGUI.IndentedRect(r);
					var w = r.width / 2f;
					var r0 = new Rect(r.xMin, r.yMin, w, r.height);
					var r1 = new Rect(r0.xMax, r.yMin, w, r.height);

					var keyProp = keysProp.GetArrayElementAtIndex(i);
					var valueProp = valuesProp.GetArrayElementAtIndex(i);
					EditorGUI.PropertyField(r0, keyProp, GUIContent.none, false);
					EditorGUI.PropertyField(r1, valueProp, GUIContent.none, false);
				}

				r = GetNextRect(ref position);
				var pRect = new Rect(r.xMax - 60f,
					            r.yMin + (EditorGUIUtility.singleLineHeight * 0.25f),
					            30f,
					            EditorGUIUtility.singleLineHeight);
				var mRect = new Rect(r.xMax - 30f,
					            r.yMin + (EditorGUIUtility.singleLineHeight * 0.25f),
					            30f,
					            EditorGUIUtility.singleLineHeight);

				if (GUI.Button(pRect, "+"))
				{
					keysProp.arraySize++;
					EditorHelper.SetPropertyValue(keysProp.GetArrayElementAtIndex(keysProp.arraySize - 1), null);
					valuesProp.arraySize = keysProp.arraySize;
				}
				if (GUI.Button(mRect, "-"))
				{
					keysProp.arraySize = Mathf.Max(keysProp.arraySize - 1, 0);
					valuesProp.arraySize = keysProp.arraySize;
				}

                EditorGUI.indentLevel = indentLevel;
			}
		}


		private Rect GetNextRect(ref Rect position)
		{
			var r = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
			var h = EditorGUIUtility.singleLineHeight + 2;
			position = new Rect(position.xMin, position.yMin + h, position.width, position.height = h);
			return r;
		}
	}


	public static class EditorHelper
	{

		public const string PROP_SCRIPT = "m_Script";


		private static Texture2D s_WhiteTexture;

		public static Texture2D WhiteTexture
		{
			get
			{
				if (s_WhiteTexture == null)
				{
					s_WhiteTexture = new Texture2D(1, 1);
					s_WhiteTexture.SetPixel(0, 0, Color.white);
					s_WhiteTexture.Apply();
				}
				return s_WhiteTexture;
			}
		}

		private static GUIStyle s_WhiteTextureStyle;

		public static GUIStyle WhiteTextureStyle
		{
			get
			{
				if (s_WhiteTextureStyle == null)
				{
					s_WhiteTextureStyle = new GUIStyle();
					s_WhiteTextureStyle.normal.background = EditorHelper.WhiteTexture;
				}
				return s_WhiteTextureStyle;
			}
		}


		static EditorHelper()
		{
		}

		public static void SetPropertyValue(SerializedProperty prop, object value)
		{
			if (prop == null)
				throw new System.ArgumentNullException("prop");

			switch (prop.propertyType)
			{
				case SerializedPropertyType.Integer:
					prop.intValue = (int)value;
					break;
				case SerializedPropertyType.Boolean:
					prop.boolValue = (bool)value;
					break;
				case SerializedPropertyType.Float:
					prop.floatValue = (float)value;
					break;
				case SerializedPropertyType.String:
					prop.stringValue = (string)value;
					break;
				case SerializedPropertyType.Color:
					prop.colorValue = (Color)value;
					break;
				case SerializedPropertyType.ObjectReference:
					prop.objectReferenceValue = value as Object;
					break;
				case SerializedPropertyType.LayerMask:
					prop.intValue = (value is LayerMask) ? ((LayerMask)value).value : (int)value;
					break;
				case SerializedPropertyType.Enum:
					prop.SetEnumValue(value);
					break;
				case SerializedPropertyType.Vector2:
					prop.vector2Value = (Vector2)value;
					break;
				case SerializedPropertyType.Vector3:
					prop.vector3Value = (Vector3)value;
					break;
				case SerializedPropertyType.Vector4:
					prop.vector4Value = (Vector4)value;
					break;
				case SerializedPropertyType.Rect:
					prop.rectValue = (Rect)value;
					break;
				case SerializedPropertyType.ArraySize:
					prop.arraySize = (int)value;
					break;
				case SerializedPropertyType.Character:
					prop.intValue = (int)value;
					break;
				case SerializedPropertyType.AnimationCurve:
					prop.animationCurveValue = value as AnimationCurve;
					break;
				case SerializedPropertyType.Bounds:
					prop.boundsValue = (Bounds)value;
					break;
				case SerializedPropertyType.Gradient:
					throw new System.InvalidOperationException("Can not handle Gradient types.");
			}
		}

		public static void SetEnumValue(this SerializedProperty prop, object value)
		{
			if (prop == null)
				throw new System.ArgumentNullException("prop");
			if (prop.propertyType != SerializedPropertyType.Enum)
				throw new System.ArgumentException(
					"SerializedProperty is not an enum type.",
					"prop");

			if (value == null)
			{
				prop.enumValueIndex = 0;
				return;
			}

			var tp = value.GetType();
			if (tp.IsEnum)
			{
				int i = System.Array.IndexOf(prop.enumNames, System.Enum.GetName(tp, value));
				if (i < 0)
					i = 0;
				prop.enumValueIndex = i;
			}
			else
			{
				int i = (int)value;
				if (i < 0 || i >= prop.enumNames.Length)
					i = 0;
				prop.enumValueIndex = i;
			}
		}
	}
}