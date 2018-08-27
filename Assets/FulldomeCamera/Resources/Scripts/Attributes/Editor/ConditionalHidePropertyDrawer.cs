using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Avante
{
	// From: http://www.brechtos.com/hiding-or-disabling-inspector-properties-using-propertydrawers-within-unity-5/

	[CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
	public class ConditionalHidePropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
			bool enabled = GetConditionalHideAttributeResult(condHAtt, property);
			bool wasEnabled = GUI.enabled;
			GUI.enabled = enabled;
			if (!condHAtt.HideInInspector || enabled)
			{
				EditorGUI.PropertyField(position, property, label, true);
			}
			GUI.enabled = wasEnabled;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
			bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

			if (!condHAtt.HideInInspector || enabled)
			{
				return EditorGUI.GetPropertyHeight(property, label);
			}
			else
			{
				return -EditorGUIUtility.standardVerticalSpacing;
			}
		}

		private bool GetConditionalHideAttributeResult(ConditionalHideAttribute condHAtt, SerializedProperty property)
		{
			bool enabled = true;
			string propertyPath = property.propertyPath; //returns the property path of the property we want to apply the attribute to
			string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField); //changes the path to the conditionalsource property path
			SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

			if (sourcePropertyValue != null)
			{
				enabled = CheckPropertyType(sourcePropertyValue);
			}
			else
			{
				var propertyInfo = property.serializedObject.targetObject.GetType().GetProperty(conditionPath, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				if (propertyInfo != null)
				{
					var value = propertyInfo.GetValue(property.serializedObject.targetObject,null);
					enabled = CheckPropertyType(value);
				}
			}
			return enabled;
		}

		private static bool CheckPropertyType(object val)
		{
			if (val is bool)
			{
				return (bool)val;
			}
			return true;
		}
	}
}
