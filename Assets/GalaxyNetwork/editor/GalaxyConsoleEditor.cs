using GalaxyLib;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GalaxyConsole))]
public class GalaxyConsoleEditor : Editor {

    Texture2D texture;
    void OnEnable()
    {
        if (texture == null)
        {
            texture = Resources.Load<Texture2D>("Gizmos/line");
        }
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Label(texture);
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, "m_Script");
        serializedObject.ApplyModifiedProperties();
    }
}
