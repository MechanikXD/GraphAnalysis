#if UNITY_EDITOR
using System;
using System.Linq;
using UI.Settings.Scriptable;
using UnityEditor;
using UnityEngine;

namespace Other
{
    [CustomPropertyDrawer(typeof(SettingGroup))]
    public class SettingGroupDrawer : PropertyDrawer
    {
        private static Type[] _settingTypes;

        static SettingGroupDrawer()
        {
            _settingTypes = TypeCache.GetTypesDerivedFrom<SettingData>()
                .Where(t => !t.IsAbstract && !t.IsGenericType)
                .ToArray();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);

            var settingsProp = property.FindPropertyRelative("_settings");

            Rect buttonRect = new Rect(position.x, position.yMax - 22, position.width, 20);

            if (GUI.Button(buttonRect, "Add Setting"))
            {
                GenericMenu menu = new GenericMenu();

                foreach (var type in _settingTypes)
                {
                    menu.AddItem(new GUIContent(type.Name), false, () =>
                    {
                        settingsProp.arraySize++;
                        var element = settingsProp.GetArrayElementAtIndex(settingsProp.arraySize - 1);
                        element.managedReferenceValue = Activator.CreateInstance(type);
                        settingsProp.serializedObject.ApplyModifiedProperties();
                    });
                }

                menu.ShowAsContext();
            }
        }
    }
}
#endif