// ===============================
// © GGTeam GalaxyNetwork module
// Модуль: Список комнат
// Версия: 0.21
// ===============================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using GalaxyLib;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

[HelpURL("http://wiki.greatgalaxy.ru/index.php?title=GalaxyServer:mod_rooms")]
public class mod_rooms : MonoBehaviour
{
    public static Log log = Log.GetLogger(typeof(mod_rooms));

#if UNITY_EDITOR
    [Header("Сцена игры")]
    public UnityEditor.SceneAsset scene_Game;
#endif

    [Header("Событие при входе в комнату")]
    public UnityEvent OnRoomEnterEvent;

    [HideInInspector]
    public string scene_Game_name = "";

    [Header("Список комнат по умолчанию")]
    public bool default_roomList = true;

    //[Header("Включить фон по умолчанию")]
    //public bool default_bg = true;

    [Space(32)]

    [Header("Шаблон строки")]
    public GameObject rowGo;

    [Header("Контент скролла")]
    public Transform scrollContent;

    [Header("Окно (Панель модуля)")]
    public GameObject panelModule;

    [Header("Панель создания комнаты")]
    public GameObject panelCreateRoom;

    [Header("Создание комнаты: Имя")]
    public InputField panelCreateRoomName;

    [Header("Создание комнаты: Кол-во")]
    public InputField panelCreateRoomCnt;

    [Header("Создание комнаты: Панель ошибки")]
    public GameObject textErrorGO;

    [Header("Создание комнаты: Текст ошибки")]
    public Text textError;

    [Header("Вход в комнату: Панель текста ошибки")]
    public GameObject roomPanelTextError;

    [Header("Вход в комнату: Текст ошибки")]
    public Text roomTextError;

    Animator anim;

    //[Header("Вход в комнату: Время отображения ошибки (сек)")]
    //public float timeErrorShow = 12.0f;
    //float _timeErrorShow = 0f;

    // Список строк-объектов комнат
    //public Dictionary<uint, GameObject> rowList = new Dictionary<uint, GameObject>();
    Dictionary<uint, mod_rooms_row> rowList = new Dictionary<uint, mod_rooms_row>();

    private static mod_rooms _instance;
    public static mod_rooms Instance
    {
        get { return _instance; }
    }

    // Авто подписка
    void OnEnable()
    {
        GalaxyEvents.OnGalaxyConnect += OnGalaxyConnect;
        GalaxyEvents.OnGalaxyDisconnect += OnGalaxyDisconnect;
        GalaxyEvents.OnGalaxyEnterToNewRoom += OnGalaxyEnterToNewRoom;
        GalaxyEvents.OnGalaxyRoomEnter += OnGalaxyRoomEnter;
        GalaxyEvents.OnGalaxyRoomUpdate += OnGalaxyRoomUpdate;
        GalaxyEvents.OnGalaxyRoomList += OnGalaxyRoomList;
    }

    // Авто отписка
    void OnDisable()
    {
        GalaxyEvents.OnGalaxyConnect -= OnGalaxyConnect;
        GalaxyEvents.OnGalaxyDisconnect -= OnGalaxyDisconnect;
        GalaxyEvents.OnGalaxyEnterToNewRoom -= OnGalaxyEnterToNewRoom;
        GalaxyEvents.OnGalaxyRoomEnter -= OnGalaxyRoomEnter;
        GalaxyEvents.OnGalaxyRoomUpdate -= OnGalaxyRoomUpdate;
        GalaxyEvents.OnGalaxyRoomList -= OnGalaxyRoomList;
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
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        rowGo.SetActive(false);
        rowList.Clear();

        if (Application.isPlaying)
        {
            if (scene_Game_name == "")
            {
                GalaxySceneManager sm = FindObjectOfType<GalaxySceneManager>();
                if (sm != null)
                {
                    if (sm.scene_Game_name == "")
                    {
                        //Debug.LogWarning("<color=#A01010ff>[mod_rooms]</color> <color=#106010ff>Не указана игровая сцена.</color>");
                    }
                }
                else
                {
                    if (OnRoomEnterEvent == null)
                        Debug.LogWarning("<color=#A01010ff>[mod_rooms]</color> <color=#106010ff>Не указана игровая сцена.</color>");
                }
            }
        }
    }

    void Update()
    {
        /*
        if (_timeErrorShow > 0)
        {
            _timeErrorShow -= Time.deltaTime;
            if (_timeErrorShow <= 0)
            {
                _timeErrorShow = 0;
                roomTextError.transform.parent.gameObject.GetComponent<Animator>().SetTrigger("Hide");
            }
        }
        */
    }

    /// <summary>
    /// Показать/скрыть окно
    /// </summary>
    public void ShowHide()
    {
        panelModule.SetActive((!panelModule.activeSelf));
        textError.text = "";
//2        textErrorGO.SetActive(false);
    }

    /// <summary>
    /// Показать/скрыть окно
    /// </summary>
    public void Show(bool show = true)
    {
        // Отображение фона
        //if (show) gameObject.transform.Find("background").gameObject.SetActive(default_bg);

        panelModule.SetActive(show);
        textError.text = "";
        textErrorGO.SetActive(false);
    }



    /// <summary>
    /// Показать/скрыть панель создания комнаты
    /// </summary>
    public void ShowRoomCreatePanel(bool show = true)
    {
//2        panelCreateRoom.SetActive(show);
        textError.text = "";
//2        textErrorGO.SetActive(false);

        if(show)
            anim.Play("mod_rooms_CrROpen");
        else
            anim.Play("mod_rooms_CrRClose");
    }

    public void OnButtonConnectToRoom(ushort roomId, Button btn)
    {
        btn.interactable = false;
        GalaxyNetwork.SendOperation.Room.RoomEnter(roomId);
    }

    // Обновить список комнат
    public void OnButtonRefresh()
    {
        GalaxyNetwork.SendOperation.Room.GetRoomsList(15);
    }

    // Создать новую комнату
    public void OnButtonCreateAndEnterRoom()
    {
        textError.text = "";
        textErrorGO.SetActive(false);
        string roomName = panelCreateRoomName.text.Trim();
        string roomCnt = panelCreateRoomCnt.text;
        if (roomCnt.Length == 0) roomCnt = "0";
        ushort roomCntI = Convert.ToUInt16(roomCnt);

        if (roomName.Length < 4)
        {
            textErrorGO.SetActive(true);
            textError.text = "Название комнаты менее 4х символов.";
            anim.Play("mod_rooms_CrRErrorOpen");
            return;
        }

        if (roomCntI <= 1)
        {
            textErrorGO.SetActive(true);
            textError.text = "Мин. кол-во человек в комнате 1.";
            anim.Play("mod_rooms_CrRErrorOpen");
            return;
        }

        if (roomCntI > 65534)
        {
            textErrorGO.SetActive(true);
            textError.text = "Максимально кол-во человек в комнате 64к";
            anim.Play("mod_rooms_CrRErrorOpen");
            return;
        }

        GalaxyNetwork.SendOperation.Room.EnterToNewRoom(new GGRoom(roomName, roomCntI, true));
        ShowRoomCreatePanel(false);
    }

    public void ShowError(string errorMes)
    {
        roomTextError.text = errorMes;
//1        _timeErrorShow = timeErrorShow;
//1        roomTextError.transform.parent.gameObject.GetComponent<Animator>().SetTrigger("Show");   // Play("mod_rooms_ErrorMes");
//1        roomTextError.transform.parent.gameObject.SetActive(true);
        roomPanelTextError.SetActive(true);        
    }

    public void HideError()
    {
//1        _timeErrorShow = 0;
//1        roomTextError.transform.parent.gameObject.GetComponent<Animator>().SetTrigger("Hide");
        roomPanelTextError.SetActive(false);
    }

    void RowClear()
    {
        foreach (var item in rowList)
        {
            Destroy(item.Value);
        }
        rowList.Clear();
    }

    // Создание в панели строки комнаты
    void RowCreate(GGRoom room)
    {
        GameObject row_go = Instantiate(rowGo, scrollContent) as GameObject;
        mod_rooms_row row = row_go.GetComponent<mod_rooms_row>();

        row.row_name.text = room.name;
        row.row_count.text = room.clients_count.ToString() + "/" + room.clients_count_max.ToString();

        //row.transform.Find("col2").Find("Text").GetComponent<Text>().text = room.name;
        //row.transform.Find("col3").Find("Text").GetComponent<Text>().text = room.clients_count.ToString() + "/" + room.clients_count_max.ToString();

        //Button btn = row.transform.Find("ButtonConnect").GetComponent<Button>();
        if (room.clients_count == room.clients_count_max) row.row_button.interactable = false;
        else
        {
            row.row_button.interactable = true;
            row.row_button.onClick.AddListener(() => OnButtonConnectToRoom(room.id, row.row_button));
        }

        rowList.Add(room.id, row);
        row_go.SetActive(true);
    }

    // Обновить строку комнанты
    void UpdateRow(uint rowId, GGRoom room)
    {
        //GameObject row = rowList[rowId];
        mod_rooms_row row = rowList[rowId];

        if (room.clients_count == 0)
        {
            Destroy(row.gameObject);
            return;
        }

        row.row_name.text = room.name;
        row.row_count.text = room.clients_count.ToString() + "/" + room.clients_count_max.ToString();

        //row.transform.Find("col2").Find("Text").GetComponent<Text>().text = room.name;
        //row.transform.Find("col3").Find("Text").GetComponent<Text>().text = room.clients_count.ToString() + "/" + room.clients_count_max.ToString();
        //Button btn = row.transform.Find("ButtonConnect").GetComponent<Button>();

        if (room.clients_count == room.clients_count_max) row.row_button.interactable = false;
        else
            row.row_button.interactable = true;
        //btn.onClick.AddListener(() => OnButtonConnectToRoom(room.roomId, btn));
        //rowList.Add(room.roomId, row);
        //row.SetActive(true);
    }



    void OnGalaxyRoomUpdate(GGRoom room, ErrorCode errorCode)
    {
        if (errorCode != ErrorCode.none)
        {
            log.Error("OnGalaxyRoomUpdate: " + ErrorMessages.RU[errorCode]);
            ShowError(ErrorMessages.RU[errorCode]);
            return;
        }

        // Если комната уже есть, обновляем данные
        if (rowList.ContainsKey(room.id))
        {
            UpdateRow(room.id, room);
        }
        // Комнаты такой в нашем списке нет, добавляем
        else
        {
            RowCreate(room);
        }
    }


    void OnGalaxyRoomList(List<GGRoom> roomsList, ErrorCode errorCode)
    {
        if (errorCode != ErrorCode.none)
        {
            log.Error("OnGalaxyRoomList: " + ErrorMessages.RU[errorCode]);
            ShowError(ErrorMessages.RU[errorCode]);
            return;
        }

        foreach (var item in roomsList)
        {
            // Если комната уже есть, обновляем данные
            if (rowList.ContainsKey(item.id))
            {
                UpdateRow(item.id, item);
            }
            // Комнаты такой в нашем списке нет, добавляем
            else
            {
                RowCreate(item);
            }
        }
    }

    // Создана новая комната
    void OnGalaxyEnterToNewRoom(GGRoom room, ErrorCode errorCode)
    {
        // Ошибка создания комнаты
        if (errorCode != ErrorCode.none)
        {
            log.Error(ErrorMessages.RU[errorCode]);
            ShowError(ErrorMessages.RU[errorCode]);
            return;
        }
    }


    void OnGalaxyRoomEnter(UInt32 clientId, String nikname, ErrorCode errorCode)
    {
        if (errorCode != ErrorCode.none)
        {
            log.Error(ErrorMessages.RU[errorCode]);
            ShowError(ErrorMessages.RU[errorCode]);
            return;
        }
        //log.Debug("[OnGalaxyRoomEnter] clientId:{0}, nikname:{1}", clientId, nikname);

        if (clientId == GalaxyNetwork.Connection.clientId)
        {
            RowClear();
            Show(false);

            if (OnRoomEnterEvent != null) OnRoomEnterEvent.Invoke();

            if (scene_Game_name != "")
            {
                //int i = SceneUtility.GetBuildIndexByScenePath(SceneManager.GetSceneByName(scene_Game_name).path);
                //int i2 = SceneManager.GetSceneByName(scene_Game_name).buildIndex;
                //Debug.Log("1) " + i + ", " + scene_Game_name);
                //SceneManager.GetSceneByBuildIndex
                SceneManager.LoadScene(scene_Game_name);
            }
            else
            {
                GalaxySceneManager sm = FindObjectOfType<GalaxySceneManager>();
                if (sm != null)
                {
                    if (sm.scene_Game_name != "")
                    {
                        SceneManager.LoadScene(sm.scene_Game_name);
                    }
                }
            }
        }
    }

    void OnGalaxyConnect(ErrorCode errorCode)
    {
        if (errorCode == ErrorCode.none)
        {
            if (GalaxyNetwork.Room.id == 0)
            {
                GalaxyNetwork.SendOperation.Room.GetRoomsList(15);
                if (default_roomList) Show();
            }
        }
    }


    void OnGalaxyDisconnect()
    {
        if (GalaxyNetwork.Room.id == 0)
        {
            if (default_roomList)
            {
                RowClear();
                Show(false);
            }
        }
    }





}
