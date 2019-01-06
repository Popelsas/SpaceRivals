// ===============================
// © GGTeam GalaxyNetwork module
// Модуль: Подбор игры
// Версия: 0.31
// ===============================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using GalaxyLib;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEditor;

[HelpURL("http://wiki.greatgalaxy.ru/index.php?title=GalaxyServer:mod_matchmaker")]
public class mod_matchmaker : MonoBehaviour
{
    public static Log log = Log.GetLogger(typeof(mod_matchmaker));

    [Header("Кол-во игроков для старта игры")]
    [Tooltip("0 = игра начнется, когда полностью заполнится комната")]
    [Range(0, 99)]
    public int players_cnt_to_start = 2;

    [Header("Событие при старте матча")]
    public UnityEvent OnStartGameEvent;

    [Header("Префаб игрока")]
    [Tooltip("Создание на сцене игрока, после запуска игры.")]
    public GameObject playerPref;

#if UNITY_EDITOR
    [Header("Сцена игры")]
    [Tooltip("Загрузится выбранная сцена, перед стартом игры")]
    public UnityEditor.SceneAsset scene_Game;
#endif

    [HideInInspector]
    public string scene_Game_name = "";

    [Header("Объекы, отображаемые при старте")]
    public GameObject[] showOnStartGO;

    [Header("Объекы, скрываемые при старте")]
    public GameObject[] hideOnStartGO;

    [Header("Возможность запуска игры раньше")]
    public bool startEarler;


    [Header("Отобразить Имя комнаты")]
    public bool showRoomName = false;

    [Header("Отобразить фон")]
    public bool show_background = true;

    [Space(32)]

    [Header("Авто-подбор с созданием комнат")]
    [Tooltip("Компонент автоматически подключиться к имеющейся комнате или создаст новую.")]
    public bool auto_matchmaker = false;

    [Header("Авто-старт подбора при логине")]
    public bool start_find_match_on_login = false;

    [Header("Фильтр поиска свободной комнаты")]
    public parameters filter_params;

    [Serializable]
    public struct parameters
    {
        public int filter_param1;
        public int filter_param2;
        public int filter_param3;
    }


    [Space(32.0f)]
    public Text waitPlayersCnt_text;


    bool game_started = false;
    ushort cur_players = 0;


    [Header("Шаблон строки")]
    public GameObject rowPref;

    [Header("Контент скролла")]
    public Transform scrollContent;

    [Header("Окно (Панель модуля)")]
    public GameObject panelModule;

    [Header("Кнопка начала игры")]
    public GameObject buttonStart;

    [Header("Вывод времени ожидания")]
    public Text timeText;

    [Header("Название комнаты")]
    public Text textRoomName;


    int timeMinutes = 0;
    int timeSeconds = 0;
    bool showsep = true;

    // Список строк-объектов комнат
    Dictionary<uint, row_mod_machmaker> players = new Dictionary<uint, row_mod_machmaker>();

#if UNITY_EDITOR
    [MenuItem("GalaxyNetwork/Модули/Подбор игры")]
    private static void Menu_Add_GalaxyModule_Matchmaker()
    {
        string module_name = "mod_matchmaker";                          // Название модуля
        string module_path = "Assets/GalaxyNetwork/modules_canvas/";    // Путь к префабу
        bool once = true;                                               // Модуль может быть только один на сцене

        if (once)
        {
            mod_matchmaker module_f = FindObjectOfType<mod_matchmaker>();
            if (module_f != null)
            {
                Debug.LogWarning("Модуль <color=#A01010ff>[" + module_name + "]</color> уже присутствует на сцене");
                Selection.activeGameObject = module_f.gameObject;
                return;
            }
        }

        // Создаем канву
        GameObject go = GameObject.Find("GalaxyCANVAS");
        if (go == null)
        {
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
        }


        GameObject module = Instantiate(AssetDatabase.LoadAssetAtPath(module_path + module_name + "/" + module_name + ".prefab", typeof(GameObject)) as GameObject, go.transform);
        if (module != null)
        {
            module.name = module_name;
            Selection.activeGameObject = module;
        }
        else
        {
            Debug.LogError("Ошибка. Возможно повреждена структура фреймворка (" + module_path + "...)");
        }
    }
#endif

    private static mod_matchmaker _instance;
    public static mod_matchmaker Instance
    {
        get { return _instance; }
    }

    Animator anim;
    bool showed = false;

    // Авто подписка
    void OnEnable()
    {
        GalaxyEvents.OnGalaxyEnterToNewRoom += OnGalaxyEnterToNewRoom;
        GalaxyEvents.OnGalaxyRoomEnter += OnGalaxyRoomEnter;
        GalaxyEvents.OnGalaxyRoomExit += OnGalaxyRoomExit;
        GalaxyEvents.OnGalaxyRoomUpdate += OnGalaxyRoomUpdate;
        GalaxyEvents.OnGalaxyDisconnect += OnGalaxyDisconnect;
        GalaxyEvents.OnGalaxyRoomList += OnGalaxyRoomList;  //if (auto_matchmaker) 
        GalaxyEvents.OnGalaxyConnect += OnGalaxyConnect;    //if (auto_matchmaker) 
        GalaxyEvents.OnGalaxyChatMessage += OnGalaxyChatMessage;
    }


    // Авто отписка
    void OnDisable()
    {
        GalaxyEvents.OnGalaxyEnterToNewRoom -= OnGalaxyEnterToNewRoom;
        GalaxyEvents.OnGalaxyRoomEnter -= OnGalaxyRoomEnter;
        GalaxyEvents.OnGalaxyRoomExit -= OnGalaxyRoomExit;
        GalaxyEvents.OnGalaxyRoomUpdate -= OnGalaxyRoomUpdate;
        GalaxyEvents.OnGalaxyDisconnect -= OnGalaxyDisconnect;
        GalaxyEvents.OnGalaxyRoomList -= OnGalaxyRoomList;  //if (auto_matchmaker) 
        GalaxyEvents.OnGalaxyConnect -= OnGalaxyConnect;    //if (auto_matchmaker)
        GalaxyEvents.OnGalaxyChatMessage -= OnGalaxyChatMessage;
    }


    void OnValidate()
    {
#if UNITY_EDITOR
        scene_Game_name = "";
        if (scene_Game != null)
            scene_Game_name = scene_Game.name;
#endif
    }

    void Awake()
    {
        if (startEarler) buttonStart.SetActive(true);
        else buttonStart.SetActive(false);

        if (players_cnt_to_start == 0) auto_matchmaker = false;

        if (showRoomName) textRoomName.enabled = true;
        else textRoomName.enabled = false;

        anim = GetComponent<Animator>();

        // Отключаем модуль комнат, если включен авто-подбор
        if (auto_matchmaker)
        {
            mod_rooms mr = transform.parent.GetComponentInChildren<mod_rooms>();
            if(mr != null)
            {
                //log.Warn("Модуль mod_rooms не нужен, при использовании авто-подбора. Рекомендуем убрать его со сцены.");
                //Debug.LogWarning("Модуль mod_rooms не нужен, при использовании авто-подбора. Рекомендуем убрать его со сцены.");
                mr.enabled = false;
            }
        }

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

    }


    void Start()
    {
        rowPref.SetActive(false);
        //rowList.Clear();
        players.Clear();    // Еще пробежать и удалить строки-го (активные)
    }


    public void StartFindMatch()
    {
        // Запрашиваем список комнат
        if (auto_matchmaker)
        {
            GalaxyNetwork.SendOperation.Room.GetRoomsList(3);
        }
    }


    public void FormClose()
    {
        Show(false);
    }

    /// <summary>
    /// Показать/скрыть окно
    /// </summary>
    public void Show(bool panel_visible = true)
    {
        if (showed != panel_visible)
        {
            if (panel_visible)
            {
                InvokeRepeating("IncTime", 0, 1.0f);
                timeMinutes = 0;
                timeSeconds = 0;
            }
            else
            {
                CancelInvoke("IncTime");
                timeMinutes = 0;
                timeSeconds = 0;
            }

            showed = panel_visible;
            // Отображение фона
            if (show_background) gameObject.transform.Find("background").gameObject.SetActive(panel_visible);

            panelModule.SetActive(panel_visible);

            if (panel_visible)
            {
                anim.Play("mod_matchmaker_show");
            }
            else
            {
                anim.Play("mod_matchmaker_hide");
            }
        }
    }


    // Очистить строку
    void RowClear()
    {
        /*
        foreach (var item in rowList)
        {
            Destroy(item.Value);
        }
        rowList.Clear();
        */
        foreach (var item in players)
        {
            Destroy(item.Value.gameObject);
        }
        players.Clear();
    }

    // Добавить строку
    /*
    void RowCreate(UInt32 clientId, string clientName)
    {
        if (rowList.ContainsKey(clientId)) { Debug.Log("!!!!!!!!!!!!!"); return; }
        //GalaxyClient.PlayerPrefs.SaveAllToLocalPlayerprefs();

        GameObject row = Instantiate(rowPref, scrollContent) as GameObject;
        row.transform.Find("col1").Find("Text").GetComponent<Text>().text = "";         // room.name;
        row.transform.Find("col2").Find("Text").GetComponent<Text>().text = clientName; // room.currentPlayersCount_for_lobby.ToString() + "/" + room.maxCntPlayers.ToString();

        // Не у хоста не определяет GalaxyNetwork.Room.owner_client_id
        if (GalaxyNetwork.Players.GetPlayer(clientId).clientId == GalaxyNetwork.Room.owner_client_id)    //client_host_id
        {
            row.transform.Find("col1").Find("Image").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        else
        {
            row.transform.Find("col1").Find("Image").GetComponent<Image>().color = new Color32(255, 255, 255, 10);
        }

        rowList.Add(clientId, row);
        row.SetActive(true);
    }
    */

    // Создаем строку игрока
    void CreateRow(GGPlayer player)
    {
        if (players.ContainsKey(player.clientId)) return;

        GameObject rowGo = Instantiate(rowPref, scrollContent) as GameObject;
        row_mod_machmaker row = rowGo.GetComponent<row_mod_machmaker>();

        row.clientId = player.clientId;
        row.isMy = player.isMy;
        if (GalaxyNetwork.Room.owner_client_id == player.clientId)
        {
            row.isOwner = true;
        }
        else row.isOwner = false;
        row.playerName = player.name;
        row.avaId = player.avaId;
        row.position = player.position;
        row.rotation = player.rotation;
        players.Add(player.clientId, row);
        rowGo.SetActive(true);
    }


    // Удалить строку
    void RowDelete(UInt32 clientId)
    {
        if (!players.ContainsKey(clientId)) return;
        // Если вышел создатель комнаты, то переназначаем картинку - новому
        if (players[clientId].isOwner)
        {
            foreach (var item in players)
            {
                // NullReferenceException: Object reference not set to an instance of an object
                //if (item.Key == GalaxyNetwork.Room.owner_client.clientId)

                // Все ОК. Но не изменяется при смене владельца
                if (item.Key == GalaxyNetwork.Room.owner_client_id)
                {
                    //Debug.Log("Удачно! " + item.Key);
                    item.Value.isOwner = true;
                }
            }
        }
        Destroy(players[clientId].gameObject);
        players.Remove(clientId);
    }

    // Создана новая комната
    void OnGalaxyEnterToNewRoom(GGRoom room, ErrorCode errorCode)
    {
        // Ошибка создания комнаты
        if (errorCode != ErrorCode.none)
        {
            log.Error(ErrorMessages.RU[errorCode]);
            //ShowError(ErrorMessages.RU[errorCode]);
            return;
        }
        textRoomName.text = room.name;
        Show(true);
    }

    // Вызовется при выходе создалеля комнаты из нее
    void OnGalaxyRoomUpdate(GGRoom room, ErrorCode errorCode)
    {
        if (errorCode != ErrorCode.none)
        {
            log.Error(ErrorMessages.RU[errorCode]);
            return;
        }

        textRoomName.text = room.name;

        // Вы новый владелец комнаты
        if (players.ContainsKey(room.owner_client_id))
        {
            players[room.owner_client_id].isOwner = true;
        }
    }

    void OnGalaxyRoomEnter(UInt32 clientId, String nikname, ErrorCode errorCode)
    {
        if (errorCode != ErrorCode.none)
        {
            log.Error(ErrorMessages.RU[errorCode]);
            return;
        }

        textRoomName.text = GalaxyNetwork.Room.name;

        GGPlayer player = GalaxyNetwork.Players.GetPlayer(clientId);
        
        // Если входим мы, то дополнительно получаем список игроков, уже находившихся в комнате
        if (player.isMy)
        {
            //if (auto_open) Show(true);
            foreach (var item in GalaxyNetwork.Players.GetPlayersList())
            {
                //RowCreate(item.clientId, item.name);
                CreateRow(item);
            }
        }
        else
            CreateRow(player);

        StartWaitGame();
    }

    void OnGalaxyRoomExit(uint clientId, string nikname, ErrorCode errorCode)
    {
        RowDelete(clientId);
    }


    // Ожидание игроков
    public void StartWaitGame()
    {
        //return;
        if (game_started) return;
        cur_players = GalaxyNetwork.Players.Count;
        waitPlayersCnt_text.text = cur_players + " / " + players_cnt_to_start;

        if (GalaxyNetwork.Players.Count == players_cnt_to_start)
        {
            StartGame();
        }
        else
        {
            Show();
            Invoke("StartWaitGame", 1.0f);
        }
    }

    // Сервер прислал сообщение чата
    public void OnGalaxyChatMessage(uint clientId, string nikname, string textMessage, byte channel)
    {
        if(channel == 240 && textMessage == "#startgame")
        {
            StartGame(false);
        }
    }

    public void OnButtonStart()
    {
        GalaxyNetwork.SendOperation.Chat.SendMessage("#startgame", 240);
    }

    public void OnButtonExit()
    {
        GalaxyNetwork.SendOperation.Room.RoomExitToLobby();
        CancelInvoke("StartWaitGame");
        RowClear();
        Show(false);
    }

    // Запуск игры
    void StartGame(bool isMy = true)
    {
        if (game_started) return;
        game_started = true;

        if (OnStartGameEvent != null) OnStartGameEvent.Invoke();

        if (showOnStartGO.Length > 0)
            foreach (var item in showOnStartGO)
            {
                if (item != null)
                    item.SetActive(true);
            }

        if (hideOnStartGO.Length > 0)
            foreach (var item in hideOnStartGO)
            {
                if (item != null)
                    item.SetActive(false);
            }

        Show(false);

        if (scene_Game_name.Length > 0) StartCoroutine(LoadYourAsyncScene());

        PlayerSpawn();

    }


    IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background at the same time as the current Scene.
        //This is particularly good for creating loading screens. You could also load the Scene by build //number.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene_Game_name);

        //Wait until the last operation fully loads to return anything
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
    }



    void PlayerSpawn()
    {
        GalaxySpawnPoint[] spawnPoints = FindObjectsOfType<GalaxySpawnPoint>();

        Vector3 pos = new Vector3();
        Quaternion rot = Quaternion.identity;
        if (spawnPoints.Length > 0)
        {
            int r = UnityEngine.Random.Range(0, spawnPoints.Length);
            pos = spawnPoints[r].transform.position;
            rot = spawnPoints[r].transform.rotation;
        }
        if (playerPref != null)
        {
            Instantiate(playerPref, pos, rot);
        }
    }


    void OnGalaxyRoomList(List<GGRoom> roomsList, ErrorCode errorCode)
    {
        if (!auto_matchmaker) return;
        if (errorCode != ErrorCode.none)
        {
            log.Error("OnGalaxyRoomList: " + ErrorMessages.RU[errorCode]);
            return;
        }

        int finding_room_id = -1;

        foreach (var room in roomsList)
        {
            // Проверяем, есть ли для нас подходящая комната
            if (filter_params.filter_param1 == room.param_int1 &&
                filter_params.filter_param2 == room.param_int2 &&
                filter_params.filter_param3 == room.param_int3 &&
                room.clients_count_max == players_cnt_to_start 
                //room.clients_count == players_cnt_to_start -1
                )
            {
                finding_room_id = room.id;
                
            }
        }

        if(finding_room_id != -1)
        {
            // Комната найдена
            GalaxyNetwork.SendOperation.Room.RoomEnter((ushort)finding_room_id);
        }
        else
        {
            // Подходящей комнаты не нашли, создаем новую
            GalaxyNetwork.SendOperation.Room.EnterToNewRoom(new GGRoom("room_" + GalaxyNetwork.Connection.clientId, (ushort)players_cnt_to_start, true, filter_params.filter_param1, filter_params.filter_param2, filter_params.filter_param3));
        }

    }

    void OnGalaxyConnect(ErrorCode errorCode)
    {
        if (errorCode == ErrorCode.none)
        {
            if (auto_matchmaker && start_find_match_on_login)
            {
                float t = UnityEngine.Random.Range(0.0f, 1.0f);
                Invoke("StartFindMatch", t);
                //StartFindMatch();
            }
        }
    }

    void OnGalaxyDisconnect()
    {
        RowClear();
        Show(false);
    }


    void IncTime()
    {
        string sep = ":";
        timeSeconds++;
        if(timeSeconds >= 60)
        {
            timeSeconds = 0;
            timeMinutes++;
            if(timeMinutes >= 99)
            {
                timeMinutes = 0;
                timeSeconds = 0;
            }
        }

        showsep = !showsep;
        if(showsep)
            timeText.text = timeMinutes.ToString("00") + sep + timeSeconds.ToString("00");
        else
            timeText.text = timeMinutes.ToString("00") + "<color=#ffffff00>" + sep + "</color>" + timeSeconds.ToString("00");
    }
}
