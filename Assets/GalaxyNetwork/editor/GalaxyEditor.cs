using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;
using GalaxyLib;
using System.Collections.Generic;

public class GalaxyEditor : EditorWindow
{
    //private static GalaxyEditor _instance = null;

    // ========== Основные инструменты ==========

    [MenuItem("GalaxyNetwork/Основные инструменты/Ядро (События) [Events]")]
    private static void Menu_Add_GalaxyEvent()
    {
        if(GameObject.FindObjectOfType<GalaxyEvents>() != null)
        {
            Debug.ClearDeveloperConsole();
            Debug.LogWarning("<color=#A01010ff>[GalaxyEvents]</color> уже присутствует в проекта");
            return;
        }

        GameObject go = GameObject.Find("GalaxyNETWORK");
        if(go == null)
        {
            go = new GameObject("GalaxyNETWORK");
        }

        go.AddComponent<GalaxyEvents>();
        Selection.activeGameObject = go;
    }

    [MenuItem("GalaxyNetwork/Основные инструменты/Менеджер сцен")]
    private static void Menu_Add_GalaxySceneManager()
    {
        if (GameObject.FindObjectOfType<GalaxySceneManager>() != null)
        {
            Debug.Log("<color=#A01010ff>[GalaxySceneManager]</color> уже присутствует в проекте");
            return;
        }

        GameObject go = GameObject.Find("GalaxyNETWORK");
        if (go == null)
        {
            go = new GameObject("GalaxyNETWORK");
        }

        if (GameObject.FindObjectOfType<GalaxyEvents>() == null)
            go.AddComponent<GalaxyEvents>();

        go.AddComponent<GalaxySceneManager>();

        Selection.activeGameObject = go;
    }


    [MenuItem("GalaxyNetwork/Основные инструменты/Консоль")]
    private static void Menu_Add_GalaxyTerminal()
    {
        if (GameObject.FindObjectOfType<GalaxyConsole>() != null)
        {
            Debug.Log("<color=#A01010ff>[GalaxyConsole]</color> уже присутствует на сцене проекта");
            return;
        }

        GameObject go = GameObject.Find("GalaxyNETWORK");
        if (go == null)
        {
            go = new GameObject("GalaxyNETWORK");
        }

        if (GameObject.FindObjectOfType<GalaxyEvents>() == null)
            go.AddComponent<GalaxyEvents>();

        go.AddComponent<GalaxyConsole>();

        Selection.activeGameObject = go;
    }

    // ========== Модули ==========
    [MenuItem("GalaxyNetwork/Модули/Статистика")]
    private static void Menu_Add_GalaxyModule_Statistics()
    {
        Insert_Module("mod_statistics", true);
    }

    [MenuItem("GalaxyNetwork/Модули/Авторизация")]
    private static void Menu_Add_GalaxyModule_Login()
    {
        if (GameObject.FindObjectOfType<mod_login>() != null)
        {
            Debug.Log("<color=#A01010ff>[mod_login]</color> уже присутствует на сцене проекта");
            return;
        }
        Insert_Module("mod_login", true);
    }

    [MenuItem("GalaxyNetwork/Модули/Список комнат")]
    private static void Menu_Add_GalaxyModule_Rooms()
    {
        Insert_Module("mod_rooms", false);
    }

    /*
    [MenuItem("GalaxyNetwork/Модули/Ожидание игроков")]
    private static void Menu_Add_GalaxyModule_Matchmaker()
    {
        if (GameObject.FindObjectOfType<mod_matchmaker>() != null)
        {
            Debug.Log("<color=#A01010ff>[mod_matchmaker]</color> уже присутствует на сцене проекта");
            return;
        }

        Insert_Module("mod_matchmaker", false);
    }
    */

    [MenuItem("GalaxyNetwork/Модули/Список онлайн-игроков")]
    private static void Menu_Add_GalaxyModule_OnlinePlayers()
    {
        Insert_Module("mod_online_players", false);
    }

    [MenuItem("GalaxyNetwork/Модули/Чат")]
    private static void Menu_Add_GalaxyModule_Chat()
    {
        Insert_Module("mod_chat", false);
    }


    [MenuItem("GalaxyNetwork/Модули/Всплывающие сообщения")]
    private static void Menu_Add_GalaxyModule_Info()
    {
        Insert_Module("mod_info", false);
    }


    [MenuItem("GalaxyNetwork/Модули/")]
    private static void Menu_Add_Empty02()
    {
    }


    [MenuItem("GalaxyNetwork/Модули/Документация")]
    private static void Menu_Add_Documentations02()
    {
        Application.OpenURL("http://wiki.greatgalaxy.ru/index.php?title=GalaxyServer:Modules");
    }



    // ========== Сетевые объекты ==========
    [MenuItem("GalaxyNetwork/Сетевые объекты/Новый объект")]
    private static void Menu_NetObjects_Add_NetIdObject()
    {
        GameObject go = Selection.activeGameObject;
        if (go != null)
        {
            if (go.GetComponent<GalaxyNetID>() == null)
            {
                go.AddComponent<GalaxyNetID>();
            }
            else
            {
                go = new GameObject("NetworkObject");
                go.AddComponent<GalaxyNetID>();
                //Debug.Log("Допускается только один элемент GalaxyNetID на одном объекте");
            }
        }
        else
        {
            go = new GameObject("NetworkObject");
            go.AddComponent<GalaxyNetID>();
        }
        Selection.activeGameObject = go;
        Debug.Log("<color=#A01010ff>Внимание!</color> Не забудте создать префаб каждого сетевого объекта и поместить его в корень папки <color=#A01010ff>[Resources]</color>");
    }


    [MenuItem("GalaxyNetwork/Сетевые объекты/")]
    private static void Menu_Add_Empty01()
    {
    }


    [MenuItem("GalaxyNetwork/Сетевые объекты/Документация")]
    private static void Menu_Add_Documentations01()
    {
        Application.OpenURL("http://wiki.greatgalaxy.ru/index.php?title=GalaxyServer:GalaxyNetID");
    }







    //=====================================================

    [MenuItem("GalaxyNetwork/")]
    private static void Menu_Add_Empty()
    {
    }


    [MenuItem("GalaxyNetwork/Документация")]
    private static void Menu_Add_Documentations()
    {
        Application.OpenURL("http://wiki.greatgalaxy.ru/index.php?title=GalaxyServer");
    }

    [MenuItem("GalaxyNetwork/Группа ВК")]
    private static void Menu_Add_Link_VK()
    {
        Application.OpenURL("https://vk.com/galaxynetworkunity");
    }


    /// <summary>
    /// Добавляет модуль
    /// </summary>
    /// <param name="module_name"></param>
    /// <param name="once"></param>
    static void Insert_Module(string module_name, bool once)
    {
        if (once)
        {
            GameObject module_f = GameObject.Find(module_name);
            if (module_f != null)
            {
                Debug.Log("Модуль <color=#A01010ff>[" + module_name + "]</color> уже присутствует на сцене");
                Selection.activeGameObject = module_f;
                return;
            }
        }

        // Создаем канву
        GameObject go = Create_GalaxyCanvas();

        GameObject module = Instantiate(AssetDatabase.LoadAssetAtPath("Assets/GalaxyNetwork/modules_canvas/" + module_name + "/" + module_name + ".prefab", typeof(GameObject)) as GameObject, go.transform);
        if (module != null)
        {
            module.name = module_name;
            Selection.activeGameObject = module;
        }
        else
        {
            Debug.LogError("Ошибка. Возможно повреждена структура фреймворка (Assets/GalaxyNetwork/modules_canvas/...)");
        }
    }

    // Создаем канву
    static GameObject Create_GalaxyCanvas()
    {
        GameObject go = GameObject.Find("GalaxyCANVAS");
        if(go != null)
            return go;

        go = new GameObject("GalaxyCANVAS");
        go.layer = 5;

        if (go.GetComponent<Canvas>() == null)
        {
            Canvas canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = true;
        }

        if (go.GetComponent<CanvasScaler>() == null)
        {
            go.AddComponent<CanvasScaler>();
        }

        if (go.GetComponent<GraphicRaycaster>() == null)
        {
            go.AddComponent<GraphicRaycaster>();
        }

        // Проверяем, есть ли EventSystem
        if (GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esGO = new GameObject("EventSystem");
            esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        return go;
    }

    // ========== Настройки ==========

/*
    [MenuItem("GalaxyNetwork/Настройки")]
    private static void Menu_Config()
    {
        _instance = GetWindow<GalaxyEditor>("Настройка");
        _instance.wantsMouseMove = true;
        _instance.autoRepaintOnSceneChange = true;
    }
*/

    public void AddPoint(int t)
    {
        return;
        
    }


    private void OnGUI()
    {
        if (GUI.Button(new Rect(20, 20, 180, 30), "Кнопка 1"))
            AddPoint(0);

        if (GUI.Button(new Rect(20, 60, 180, 30), "Кнопка 2"))
            AddPoint(1);

        // сцука
        if (GUI.Button(new Rect(20, 100, 180, 30), "Кнопка 3"))
            AddPoint(11);

        if (GUI.Button(new Rect(20, 140, 180, 30), "Кнопка 4"))
            AddPoint(2);


        //if (_editorCamera != null)
        //{
        //    // NOTE: This is not a perfect rectangle for the window.  Adjust the size to get the desired result
        //    Rect cameraRect = new Rect(0f, 0f, position.width, position.height);
        //    Handles.DrawCamera(cameraRect, _editorCamera, DrawCameraMode.Textured);
        //}


    }

    // Custom Gizmos, Create as many as you'd like
    //    [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Selected)]
    //    //TODO: Replace first argument with the type you are editing
//    private static void GizmoTest(Transform aTarget, GizmoType aGizmoType)
//    {
//        if (_instance == null)
//        {
//            return;
//        }

//        // TODO: Perform gizmo drawing here

//#if UNITY_EDITOR
//        // TODO: Place calls to handle drawing functions in here, otherwise build errors will result
//#endif
//    }
}


// Отображение типов файлов
[InitializeOnLoad]
internal class CustomProjectWindow
{
    //static readonly Color labelColor = new Color(0.75f, 0.75f, 0.75f, 1.0f);
    static readonly Color labelColor = new Color(0.75f, 0.75f, 0.75f, 0.2f);

    static CustomProjectWindow()
    {
        EditorApplication.projectWindowItemOnGUI += OnProjectWindowGUI;
    }

    static void OnProjectWindowGUI(string pGUID, Rect pDrawingRect)
    {
        string assetpath = AssetDatabase.GUIDToAssetPath(pGUID);
        string extension = System.IO.Path.GetExtension(assetpath);
        bool icons = pDrawingRect.height > 20;

        if (icons || assetpath.Length == 0)
            return;

        GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
        Vector2 labelSize = labelStyle.CalcSize(new GUIContent(extension));

        Rect newRect = pDrawingRect;
        newRect.width += pDrawingRect.x;
        newRect.x = newRect.width - labelSize.x - 24;

        Color prevGuiColor = GUI.color;
        GUI.color = labelColor;
        GUI.Label(newRect, extension, labelStyle);
        GUI.color = prevGuiColor;
    }

}

//[ExecuteInEditMode]
//public class CustomHierarchySorting : BaseHierarchySort
//{
//    public override int Compare(GameObject lhs, GameObject rhs)
//    {
//        if (lhs == rhs) return 0;
//        if (lhs == null) return -1;
//        if (rhs == null) return 1;

//        if (lhs.tag == "Auxiliary" && rhs.tag != "Auxiliary")
//            return 1;
//        if (lhs.tag != "Auxiliary" && rhs.tag == "Auxiliary")
//            return -1;

//        return System.String.Compare(lhs.name, rhs.name);
//    }
//}

[ExecuteInEditMode]
[InitializeOnLoad]
public class HierarchyIcons
{

    static readonly Texture2D Cyan;
    static readonly Texture2D Orange;
    static readonly Texture2D Yellow;
    static readonly Texture2D Gray;
    static readonly Texture2D GGCore;

    static readonly List<int> CyanMarked = new List<int>();
    static readonly List<int> OrangeMarked = new List<int>();
    static readonly List<int> YellowMarked = new List<int>();
    static readonly List<int> GrayMarked = new List<int>();
    static readonly List<int> GalaxyCoreMarked = new List<int>();

    static string[] tags;
    static Dictionary<string, int> tagsDict = new Dictionary<string, int>();


    static HierarchyIcons()
    {
        Cyan = AssetDatabase.LoadAssetAtPath("Assets/GalaxyNetwork/Resources/Gizmos/IconCyan.png", typeof(Texture2D)) as Texture2D;
        Orange = AssetDatabase.LoadAssetAtPath("Assets/GalaxyNetwork/Resources/Gizmos/IconOrange.png", typeof(Texture2D)) as Texture2D;
        Yellow = AssetDatabase.LoadAssetAtPath("Assets/GalaxyNetwork/Resources/Gizmos/IconYellow.png", typeof(Texture2D)) as Texture2D;
        Gray = AssetDatabase.LoadAssetAtPath("Assets/GalaxyNetwork/Resources/Gizmos/IconGray.png", typeof(Texture2D)) as Texture2D;

        GGCore = AssetDatabase.LoadAssetAtPath("Assets/GalaxyNetwork/Resources/Gizmos/GalaxyIcon-32.png", typeof(Texture2D)) as Texture2D;

        string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
        tagsDict.Clear();
        int i = 0;
        foreach (var item in tags)
        {
            tagsDict.Add(item, i);
            i++;
        }

        EditorApplication.hierarchyChanged += Update;
        //2EditorApplication.hierarchyWindowChanged += Update;
        EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchyItemIcon;
    }

    static void Update()
    {
        
        OrangeMarked.Clear();
        CyanMarked.Clear();
        YellowMarked.Clear();
        GrayMarked.Clear();

        GalaxyCoreMarked.Clear();

        GameObject[] go = Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];

        foreach (GameObject g in go)
        {
            if (g == null) continue;

            int instanceId = g.GetInstanceID();

            if (g.GetComponent<GalaxyEvents>() != null)
                GalaxyCoreMarked.Add(instanceId);
            //if (g.tag == "Player")
            //    OrangeMarked.Add(instanceId);
            //else if (g.tag == "Interactive")
            //    CyanMarked.Add(instanceId);
            //else if (g.tag == "Auxiliary")
            //    YellowMarked.Add(instanceId);
            //else
            //    GrayMarked.Add(instanceId);

            if (tagsDict.ContainsKey(g.tag))
            {
                int i = tagsDict[g.tag];
                if (i == 7)
                    OrangeMarked.Add(instanceId);
                if (i == 8)
                    CyanMarked.Add(instanceId);
                if (i == 9)
                    YellowMarked.Add(instanceId);
                if (i == 10)
                    GrayMarked.Add(instanceId);
            }


        }
    }

    static void DrawHierarchyItemIcon(int instanceId, Rect selectionRect)
    {
        Rect r = new Rect(selectionRect);
        r.x += r.width - 25;
        r.width = 18;

        if (CyanMarked.Contains(instanceId))
            GUI.Label(r, Cyan);
        if (OrangeMarked.Contains(instanceId))
            GUI.Label(r, Orange);
        if (YellowMarked.Contains(instanceId))
            GUI.Label(r, Yellow);
        if (GrayMarked.Contains(instanceId))
            GUI.Label(r, Gray);

        if (GalaxyCoreMarked.Contains(instanceId))
            GUI.Label(r, GGCore);
    }
}

