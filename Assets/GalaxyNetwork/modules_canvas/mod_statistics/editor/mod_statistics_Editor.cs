using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(mod_statistics))]
public class mod_statistics_Editor : Editor
{
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


        DrawDefaultInspector();

    }


    




}
