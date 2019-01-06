using UnityEngine;
using System.Collections;
using UnityEditor;
using GalaxyLib;
using UnityEditor.SceneManagement;
using UnityEngine.Events;

[CustomEditor(typeof(GalaxyEvents))]
public class GalaxyEventsEditor : Editor
{
    string version_dll = GalaxyNetwork.version;
    string version_sdk = "3";

    Texture2D texture;
    static string _app_key;

    void OnEnable()
    {
        if (texture == null)
        {
            texture = Resources.Load<Texture2D>("Gizmos/logo");
        }       
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //DrawDefaultInspector();
        GUILayout.Label(texture);

        EditorGUILayout.LabelField("Версия DLL: ", version_dll + "." + version_sdk);

        //if(GalaxyNetwork.Connection.selected_host_server != null && GalaxyNetwork.Connection.selected_host_server.Length > 0)
        //    EditorGUILayout.LabelField("Сервер: ", GalaxyNetwork.Connection.selected_host_server + ":" + GalaxyNetwork.Connection.selected_port_server);

        if (GalaxyNetwork.Connection.server_name != null && GalaxyNetwork.Connection.server_name.Length > 0)
            EditorGUILayout.LabelField("Сервер: ", GalaxyNetwork.Connection.server_name);

        EditorGUILayout.LabelField("Ширина канала: ", GalaxyNetwork.Config.MAX_RATE_SEND_BYTES_IN_SECOND + " байт/сек");
        //if (Application.isPlaying)
        //{
        //limit send rate
        //EditorGUILayout.LabelField("Текущее: ", GalaxyClient.Info.SendByteRateAverage + " байт/сек");
        //}

        EditorGUILayout.Separator();

        if (!Application.isPlaying)
        {
            EditorGUILayout.Separator();

            GalaxyEvents myTarget = (GalaxyEvents)target;
            myTarget.app_key = EditorGUILayout.TextField("Ключ приложения", myTarget.app_key);
            // ======================

            //SerializedProperty sprop = ser_target.FindProperty("On_Connect");
            //EditorGUILayout.PropertyField(sprop);

            //EditorGUIUtility.LookLikeControls();



            //EditorGUILayout. LabelField("Узнать его можно на сайте, в личном кабинете");
            //EditorGUILayout.HelpBox("Состоит из 16 символов. \r\nУзнать его можно на сайте, в личном кабинете.", MessageType.Info);

            if (myTarget.app_key == null)
            {
                EditorGUILayout.HelpBox("Узнать его можно на сайте, в личном кабинете.", MessageType.Info);
            }
            else
            if (myTarget.app_key.Length == 0)
            {
                EditorGUILayout.HelpBox("Узнать его можно на сайте, в личном кабинете.", MessageType.Info);
                EditorGUILayout.HelpBox("Внимание! Внесите изменения в сцену и пересохраните ее, для сохранения ключа.", MessageType.Warning);
            }
            else
            {
                /*
                if(_app_key != myTarget.app_key)
                {
                    _app_key = myTarget.app_key;
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());     // Пометить, что сцена требует сохранения!!
                }
                */

                if (myTarget.app_key.Length != 16)
                {

                    bool ok = true;
                    for (int i = 0; i < myTarget.app_key.Length; ++i)
                    {
                        if (((myTarget.app_key[i] >= 'a') && (myTarget.app_key[i] <= 'z')) || ((myTarget.app_key[i] >= 'A') && (myTarget.app_key[i] <= 'Z')))
                        {

                        }
                        else
                        {
                            ok = false;
                        }
                    }

                    if (ok) { EditorGUILayout.HelpBox("Состоит из 16 символов. Вы ввели " + myTarget.app_key.Length, MessageType.Warning); }
                    else
                        EditorGUILayout.HelpBox("Допустимы только символы латинского алфавита.", MessageType.Error);
                }
                else
                {
                    EditorGUILayout.HelpBox("Рекомендуется, при каждом крупном обновлении вашего приложения, генерировать новый ключ из личного кабинета на сайте.", MessageType.None);

                    //myTarget.app_key
                    if (_app_key != myTarget.app_key)
                    {
                        _app_key = myTarget.app_key;
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());     // Пометить, что сцена требует сохранения!!
                    }

                }
            }

            SerializedProperty sprop_connect = serializedObject.FindProperty("On_Connect");
            //EditorGUILayout.LabelField("Вызов события:", "Подключились к серверу");
            EditorGUILayout.PropertyField(sprop_connect);

            SerializedProperty sprop_disconnect = serializedObject.FindProperty("On_Disconnect");
            EditorGUILayout.PropertyField(sprop_disconnect);


        }

        //myTarget.experience = EditorGUILayout.IntField("Experience", myTarget.experience);
        //EditorGUILayout.LabelField("Level", myTarget.Level.ToString());

        //GUIContent gc = new GUIContent("ffffffdsss");
        //EditorGUI.PrefixLabel(new Rect(0, 0, 100, 100), 0, gc);


        //GUILayout.Label(myTarget.textureToDisplay);
        //GUILayout.Label(textureLogo);   //, GUILayout.ExpandWidth(true)

        //EditorGUILayout.LabelField("ClientID:", myTarget.clientId.ToString());

        //if (Application.isPlaying)
        //{
        //    myTarget.clientId = EditorGUILayout.IntField("ClientID: ", myTarget.clientId);
        //if (GalaxyClient.Connection.connected)
        //    {
        //EditorGUILayout.LabelField("ClientID:", GalaxyClient.Connection.clientId.ToString());
        //EditorGUILayout.LabelField("Client Name:", GalaxyClient.Connection.clientNikName.ToString());
        //    }
        //}
        //EditorGUILayout.LabelField("Level", myTarget.Level.ToString());


        //        if (GalaxyClient.Connection.connected)
        //        {
        //            EditorGUILayout.LabelField("ClientID:", GalaxyClient.Connection.clientId.ToString());
        //            EditorGUILayout.LabelField("Client Name:", GalaxyClient.Connection.clientNikName.ToString());
        //        }
        //EditorGUILayout.LabelField("NetObjects", myTarget.Level.ToString());

        //EditorGUI.PrefixLabel(Rect(25, 45, 100, 15), 0, GUIContent("Preview:"));
        //EditorGUI.DrawPreviewTexture(Rect(25, 60, 100, 100), texture);

        //serializedObject.Update();
        
        //serializedObject.ApplyModifiedProperties();
//        base.OnInspectorGUI();
        //serializedObject.Update();
        //serializedObject.ApplyModifiedProperties();
        serializedObject.ApplyModifiedProperties();
        //DrawPropertiesExcluding(serializedObject, "m_Script");

//        base.OnInspectorGUI();
    }
}