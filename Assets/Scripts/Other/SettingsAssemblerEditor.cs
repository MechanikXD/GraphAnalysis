#if UNITY_EDITOR
using System;
using System.Linq;
using UI.Settings.Scriptable;
using UnityEditor;
using UnityEngine;

namespace Other
{
    [CustomEditor(typeof(SettingGroup))]
    public class SettingsAssemblerEditor : Editor
    {
        private Type[] _types;

        private void OnEnable()
        {
            _types = TypeCache.GetTypesDerivedFrom<SettingData>()
                .Where(t => !t.IsAbstract && !t.IsGenericType)
                .ToArray();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var prop = serializedObject.FindProperty("settings");

            for (int i = 0; i < prop.arraySize; i++)
            {
                var element = prop.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(element, true);
            }

            if (GUILayout.Button("Add Setting"))
            {
                var menu = new GenericMenu();
                foreach (var type in _types)
                {
                    menu.AddItem(new GUIContent(type.Name), false, () =>
                    {
                        prop.arraySize++;
                        var newElement = prop.GetArrayElementAtIndex(prop.arraySize - 1);
                        newElement.managedReferenceValue = Activator.CreateInstance(type);
                        serializedObject.ApplyModifiedProperties();
                    });
                }
                menu.ShowAsContext();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif