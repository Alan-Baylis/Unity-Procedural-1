using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Reflection;

//[UnityEditor.CustomEditor(typeof(InputSystem))]
public class InputSystemEditor : Editor {

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        InputSystem component = (InputSystem)target;

        foreach (FieldInfo fi in component.GetType().GetFields())
        {
            if (fi.FieldType == typeof(float))
            {
                fi.SetValue(component, EditorGUILayout.FloatField(fi.Name, (float)fi.GetValue(component)));
                continue;
            }

            if (fi.FieldType == typeof(KeyCode))
            {
                fi.SetValue(component, EditorGUILayout.EnumPopup(fi.Name, (Enum)fi.GetValue(component)));
                continue;
            }
        }
    }


}
