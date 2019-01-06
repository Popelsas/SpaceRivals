using UnityEngine;
using System.Collections;
using UnityEditor;
using GalaxyLib;

[CustomEditor(typeof(GalaxyNetID))]
[CanEditMultipleObjects]
public class GalaxyNetIDEditor : Editor
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
        //if (texture == null)
        //{
        //    texture = Resources.Load<Texture2D>("Gizmos/line");
        //}

        GUILayout.Label(texture);

        GalaxyNetID myTarget = (GalaxyNetID)target;

        //myTarget.id = EditorGUILayout.IntField("Id", myTarget.id);

        if (Application.isPlaying)
        {
            EditorGUILayout.LabelField("NetID: ", myTarget.id.ToString());
            EditorGUILayout.LabelField("isMy: ", myTarget.isMy.ToString());
        }

        //EditorGUILayout.Separator();


        //        EditorGUI.< span class="posthilit">DrawPreviewTexture</span>(new Rect(50,50,100,100), TestTexture);
        //EditorGUILayout.

        //DrawDefaultInspector();
        //base.OnInspectorGUI();

        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, "m_Script");
        serializedObject.ApplyModifiedProperties();

    }

    //public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
    //{
    //    //base.OnInteractivePreviewGUI(r, background);
    //    TestTexture = new Texture2D(100, 100);
    //    TestTexture.filterMode = FilterMode.Point;
    //    TestTexture.Apply();
    //    EditorGUI.DrawPreviewTexture(new UnityEngine.Rect(0, 0, 128, 32), TestTexture);
    //    EditorGUI.DrawPreviewTexture(r, TestTexture);
    //}
    
}
