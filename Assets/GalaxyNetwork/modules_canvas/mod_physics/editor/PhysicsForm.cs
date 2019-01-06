//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

[CanEditMultipleObjects]
//2[CustomEditor(typeof(PhysicsForm), true)]
public class PhysicsForm : EditorWindow
{
    // Сцены
    string scene_current_name = "none";
    int scene_current_id = -1;
    int _scene_current_id = -1;

    //Scene scene;

    string errorMes = "";

    //string myString = "Main";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.05f;

    [MenuItem("Window/Galaxy physics")]
    public static void ShowWindow()
    {
        //PhysicsForm wnd = (PhysicsForm)EditorWindow.GetWindow<NetworkBehaviourInspector

        PhysicsForm wnd = (PhysicsForm)EditorWindow. GetWindow(typeof(PhysicsForm));
        wnd.titleContent.text = "Galaxy Physics";
        wnd.titleContent.tooltip = "Сетевая физика";

        //PhysicsForm window = ScriptableObject.CreateInstance<PhysicsForm>();
        //window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 150);
        //window.ShowPopup();
    }


    void OnGUI()
    {
        GUILayout.Label("[В разработке]", EditorStyles.boldLabel);

        GUILayout.Label("Основные настройки", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Имя сцены", "["+ scene_current_id + "]"+scene_current_name, EditorStyles.wordWrappedLabel);

        //myString = EditorGUILayout.TextField("Имя сцены", scene_current_name);

        physics_core.Settings.groupEnabled = EditorGUILayout.BeginToggleGroup("Расширение", physics_core.Settings.groupEnabled);
        myBool = EditorGUILayout.Toggle("Активировать", myBool);
        myFloat = EditorGUILayout.Slider("Размер", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup();

        GUILayout.Space(32);
        if (errorMes.Length > 0)
        {
            GUI.enabled = false;
        }
        else
        {
            GUI.enabled = true;
        }
        if (GUILayout.Button("Запечь физику")) this.Close();
        GUI.enabled = true;

        if (errorMes.Length > 0) EditorGUILayout.LabelField(errorMes, EditorStyles.helpBox);

        //SceneManager.activeSceneChanged
        //SceneManager.sceneCountInBuildSettings
        //SceneManager.GetSceneByBuildIndex

    }

    private void OnInspectorUpdate()
    {
        _scene_current_id = SceneManager.GetActiveScene().buildIndex;
        if(_scene_current_id == -1)
        {
            errorMes = "Добавьте текущую сцену в BuildSettings";
            return;
        }
        else
        {
            errorMes = "";
            if (_scene_current_id != scene_current_id)
            {
                _scene_current_id = GetCurrentScene();
                scene_current_id = _scene_current_id;
            }
        }
    }

    //private void OnEnable()
    //{
    //    GetCurrentScene();
    //}

    int GetCurrentScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        //if (scene == null) return -1;
        if (!scene.isLoaded) return -1;
        scene_current_id = scene.buildIndex;
        scene_current_name = scene.name;
        return scene_current_id;
    }


    void MarkAllColliders()
    {

    }

}
