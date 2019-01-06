using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(mod_matchmaker))]
public class mod_matchmaker_Editor : Editor
{

    public override void OnInspectorGUI()
    {
        mod_matchmaker myTarget = (mod_matchmaker)target;
        if (myTarget.players_cnt_to_start == 0)
        {
            EditorGUILayout.LabelField("Игра начнется, как полностью заполнится комната");
        }
        else
        {
            string s = "игроков";
            switch (myTarget.players_cnt_to_start)
            {
                case 1: s = "игрок"; break;
                case 2: s = "игрока"; break;
                case 3: s = "игрока"; break;
                case 4: s = "игрока"; break;
                case 21: s = "игрок"; break;
                case 22: s = "игрока"; break;
                case 23: s = "игрока"; break;
                case 24: s = "игрока"; break;
                case 31: s = "игрок"; break;
                case 32: s = "игрока"; break;
                case 33: s = "игрока"; break;
                case 34: s = "игрока"; break;

                default:
                    break;
            }

            EditorGUILayout.LabelField("Игра начнется, когда подключится " + myTarget.players_cnt_to_start.ToString() + " " + s);
        }
        

        //GUILayout.Label("Проверка");

        EditorGUILayout.Separator();

        //DrawDefaultInspector();

        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, "m_Script");
        serializedObject.ApplyModifiedProperties();

    }
}
