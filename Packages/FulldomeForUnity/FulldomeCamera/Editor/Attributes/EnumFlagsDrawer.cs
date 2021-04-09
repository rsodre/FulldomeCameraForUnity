using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Avante
{
	[CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
	public class EnumFlagsDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{

			EnumFlagsAttribute flagSettings = (EnumFlagsAttribute)attribute;
			Enum targetEnum = (Enum)Enum.ToObject(fieldInfo.FieldType, property.intValue);

			string propName = flagSettings.enumName;
			if (string.IsNullOrEmpty(propName))
				propName = ObjectNames.NicifyVariableName(property.name);

			EditorGUI.BeginChangeCheck();
			EditorGUI.BeginProperty(position, label, property);
		    Enum enumNew = EditorGUI.EnumFlagsField(position, propName, targetEnum);
			if (!property.hasMultipleDifferentValues || EditorGUI.EndChangeCheck())
				property.intValue = (int)Convert.ChangeType(enumNew, targetEnum.GetType());

			EditorGUI.EndProperty();
		}
	}
}
