#if HAVE_VFX_GRAPH

using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using System;

namespace UnityEditor.VFX.Block
{
    [CustomEditor(typeof(CustomBlock))]
    public class CustomBlockEditor : Editor
    {

        SerializedProperty BlockName;
        SerializedProperty ContextType;
        SerializedProperty CompatibleData;
        SerializedProperty Attributes;
        SerializedProperty Properties;
        SerializedProperty UseTotalTime;
        SerializedProperty UseDeltaTime;
        SerializedProperty UseRandom;
        SerializedProperty SourceCode;

        ReorderableList attributeList;
        ReorderableList propertiesList;


        bool dirty;

        private void OnEnable()
        {
            Reload();
            
            BlockName = serializedObject.FindProperty("BlockName");
            ContextType = serializedObject.FindProperty("ContextType");
            CompatibleData = serializedObject.FindProperty("CompatibleData");
            Attributes = serializedObject.FindProperty("Attributes");
            Properties = serializedObject.FindProperty("Properties");
            UseTotalTime = serializedObject.FindProperty("UseTotalTime");
            UseDeltaTime = serializedObject.FindProperty("UseDeltaTime");
            UseRandom = serializedObject.FindProperty("UseRandom");
            SourceCode = serializedObject.FindProperty("SourceCode");

            dirty = false;
            serializedObject.Update();

            if (attributeList == null)
            {
                attributeList = new ReorderableList(serializedObject, Attributes, true, true, true, true);
                attributeList.drawHeaderCallback = (r) => { GUI.Label(r, "Attributes"); };
                attributeList.onAddCallback = OnAddAttribute;
                attributeList.onRemoveCallback = OnRemoveAttribute;
                attributeList.drawElementCallback = OnDrawAttribute;
                attributeList.onReorderCallback = OnReorderAttribute;
            }

            if (propertiesList == null)
            {
                propertiesList = new ReorderableList(serializedObject, Properties, true, true, true, true);
                propertiesList.drawHeaderCallback = (r) => { GUI.Label(r, "Properties"); };
                propertiesList.onAddCallback = OnAddProperty;
                propertiesList.onRemoveCallback = OnRemoveProperty;
                propertiesList.drawElementCallback = OnDrawProperty;
                propertiesList.onReorderCallback = OnReorderProperty;
            }
        }

        void OnAddAttribute(ReorderableList list)
        {
            Attributes.InsertArrayElementAtIndex(0);
            var sp = Attributes.GetArrayElementAtIndex(0);
            sp.FindPropertyRelative("name").stringValue = "position";
            sp.FindPropertyRelative("mode").enumValueIndex = 3; // ReadWrite
            dirty = true;
            Apply();
        }
        void OnRemoveAttribute(ReorderableList list)
        {
            if (list.index != -1)
                Attributes.DeleteArrayElementAtIndex(list.index);
            dirty = true;
            Apply();
        }

        void OnReorderAttribute(ReorderableList list)
        {
            dirty = true;
            Apply();
        }

        void OnDrawAttribute(Rect rect, int index, bool isActive, bool isFocused)
        {
            var sp = Attributes.GetArrayElementAtIndex(index);
            rect.yMin += 2;
            var nameRect = rect;
            float split = rect.width / 2;

            nameRect.width = split - 40;
            string name = sp.FindPropertyRelative("name").stringValue;
            int attribvalue = EditorGUI.Popup(nameRect, Array.IndexOf(VFXAttribute.All, name), VFXAttribute.All);
            sp.FindPropertyRelative("name").stringValue = VFXAttribute.All[attribvalue];

            var modeRect = rect;
            modeRect.xMin = split;
            var mode = sp.FindPropertyRelative("mode");
            var value = EditorGUI.EnumFlagsField(modeRect, (VFXAttributeMode)mode.intValue);
            mode.intValue = (int)System.Convert.ChangeType(value, typeof(VFXAttributeMode));

            if(GUI.changed)
                Apply();
        }

        void OnAddProperty(ReorderableList list)
        {
            Properties.InsertArrayElementAtIndex(0);
            var sp = Properties.GetArrayElementAtIndex(0);
            sp.FindPropertyRelative("name").stringValue = "newProperty";
            sp.FindPropertyRelative("type").stringValue = "float";
            dirty = true;
            Apply();
        }

        void OnRemoveProperty(ReorderableList list)
        {

            if (list.index != -1)
                Properties.DeleteArrayElementAtIndex(list.index);
            dirty = true;
            Apply();
        }
        
        void OnReorderProperty(ReorderableList list)
        {
            dirty = true;
            Apply();
        }

        void OnDrawProperty(Rect rect, int index, bool isActive, bool isFocused)
        {
            var sp = Properties.GetArrayElementAtIndex(index);
            rect.yMin += 2;
            rect.height = 16;
            var nameRect = rect;
            float split = rect.width / 2 ;

            nameRect.width = split - 40;
            string name = sp.FindPropertyRelative("name").stringValue;
            sp.FindPropertyRelative("name").stringValue = EditorGUI.TextField(nameRect, name);

            var modeRect = rect;
            modeRect.xMin = split;

            var knownTypes = CustomBlock.knownTypes.Keys.ToArray();
            var type = sp.FindPropertyRelative("type");
            var value = EditorGUI.Popup(modeRect, Array.IndexOf(knownTypes, type.stringValue), knownTypes);
            type.stringValue = knownTypes[value];

            if(GUI.changed)
                Apply();
        }

        public override void OnInspectorGUI()
        {

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(BlockName);
            EditorGUILayout.PropertyField(ContextType);
            EditorGUILayout.PropertyField(CompatibleData);
            attributeList.DoLayoutList();
            propertiesList.DoLayoutList();
            EditorGUILayout.PropertyField(UseTotalTime);
            EditorGUILayout.PropertyField(UseDeltaTime);
            EditorGUILayout.PropertyField(UseRandom);
            EditorGUILayout.PropertyField(SourceCode);

            if (EditorGUI.EndChangeCheck())
                dirty = true;

            using (new EditorGUI.DisabledGroupScope(!dirty))
            {
                if (GUILayout.Button("Apply"))
                {
                    Apply();
                }

            }

        }

        void Apply()
        {
            serializedObject.ApplyModifiedProperties();
            dirty = false;   
            serializedObject.Update();
            Reload();
        }


        void Reload()
        {
            (serializedObject.targetObject as VFXBlock).Invalidate(VFXModel.InvalidationCause.kSettingChanged);
        }

    }

}

#endif