// ===============================
// © GGTeam GalaxyNetwork module
// Модуль: Чат
// Версия: 0.12
// ===============================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using GalaxyLib;
using UnityEngine.EventSystems;


[HelpURL("http://wiki.greatgalaxy.ru/index.php?title=GalaxyServer:mod_chat")]
public class mod_chat : MonoBehaviour {

    [System.Serializable]
    public class ChatCh
    {
        public int ch_id;
        public string ch_name;
    }

    [Header("Каналы чата (id/name)")]
    [SerializeField]
    public ChatCh[] channels;

    [Header("Открыт")]
    public bool showed = true;

    [Header("Канал передачи сообщений")]
    [Range(0,128)]
    public byte channel = 0;

    [Header("Отображать название канала")]
    public bool showed_ch = true;

    [Space(32)]

    [Header("Префаб строки чата")]
    public GameObject RowChat;

    [Header("Контент чата")]
    public GameObject ContentChat;

    [Header("Строка ввода текста")]
    public InputField InputText;

    [Header("Высота текста")]
    public float rowHeight = 35.0f;

    [Header("Панель для скрытия")]
    public GameObject PanelTop;
    public GameObject PanelBottom;

    private float rowSpacing;
    private List<string> history = new List<string>();

    RectTransform rt;       //??
    private static mod_chat _instance;
    public static mod_chat Instance
    {
        get { return _instance; }
    }

    // Авто подписка
    void OnEnable()
    {
        GalaxyEvents.OnGalaxyChatMessage += OnGalaxyChatMessage;
        GalaxyEvents.OnGalaxyConnect += OnGalaxyConnect;
        GalaxyEvents.OnGalaxyDisconnect += OnGalaxyDisconnect;
        if(GalaxyNetwork.Connection.connected)
            if (showed) PanelBottom.SetActive(true);
    }

    // Авто отписка
    void OnDisable()
    {
        GalaxyEvents.OnGalaxyChatMessage -= OnGalaxyChatMessage;
        GalaxyEvents.OnGalaxyConnect -= OnGalaxyConnect;
        GalaxyEvents.OnGalaxyDisconnect -= OnGalaxyDisconnect;
    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        PanelTop.SetActive(false);
    }

    void Start()
    {
        rt = ContentChat.GetComponent<RectTransform>();
        rowSpacing = ContentChat.GetComponent<VerticalLayoutGroup>().spacing;

        /*
        EventTrigger trigger = InputText.gameObject.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Submit;
        //entry.callback.AddListener((data) => { OnPointerDownDelegate((PointerEventData)data); });
        entry.callback.AddListener((data) => { OnSubmit((BaseEventData)data); });
        trigger.triggers.Add(entry);
        */

        InputText.onEndEdit.AddListener(val =>
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                OnButtonPostMessage();
        });

    }



    /// <summary>
    /// Каналы чата
    /// </summary>
    //public enum ChatChannalsNames : byte
    //{
    //    Основной = 0,
    //    Дополнительный = 1,
    //}

    // Добавить сообщение
    public void AddMessage(uint clientId, string nikname, string textMessage, byte channel)
    {
        //ChatChannalsNames ch = (ChatChannalsNames)channel;
        if (channel > channels.Length) return;

        string ch_s = "";
        if (showed_ch) ch_s = "<color=#00aaffFF>[" + channels[channel].ch_name + "]</color> ";

        //textMessage = "["+ channel + "] [" + clientId + "] <b><color=#F1C15FFF>" + strTmp[0] + ":</color></b> " + strTmp[1];
        //string message = "[" + clientId + "] <b><color=#F1C15FFF>" + nikname + ": </color></b>" + textMessage;
        string message = ch_s+ "<color=#F1C15FFF>" + nikname + ": </color>" + textMessage;

        history.Add(message);

        rt.sizeDelta = new Vector2(0, history.Count * (rowHeight + rowSpacing));

        var row = Instantiate(RowChat, ContentChat.transform) as GameObject;
        row.GetComponent<Text>().text = message;
        row.SetActive(true);
    }


    // Написать сообщение
    public void OnButtonPostMessage()
    {
        if(GalaxyNetwork.Connection.connected)
            GalaxyNetwork.SendOperation.Chat.SendMessage(InputText.text, channel);
        InputText.text = "";
    }


    // Сервер прислал сообщение чата
    //UInt32 clientId, String nikname, String textMessage, Byte channel
    public void OnGalaxyChatMessage(uint clientId, string nikname, string textMessage, byte channel)
    {
        if (channel > channels.Length) return;

        if (showed)
            PanelTop.SetActive(true);
        if(textMessage.Length > 0)
            if(textMessage[0] != '#')
                AddMessage(clientId, nikname, textMessage, channel);
    }

    /// <summary>
    /// Показать/скрыть
    /// </summary>
    public void OnButtonShowHide()
    {
        showed = !showed;
        PanelTop.SetActive(showed);
    }

    /// <summary>
    /// Показать/скрыть
    /// </summary>
    public void ShowChat(bool show = true)
    {
        showed = show;
//1        PanelTop.SetActive(showed);
        PanelBottom.SetActive(showed);
    }

    public void OnSubmit(BaseEventData data)
    {
        OnButtonPostMessage();
    }

    void OnGalaxyConnect(ErrorCode errorCode)
    {
        if (errorCode != ErrorCode.none) return;
        if(showed) PanelBottom.SetActive(true);
    }

    void OnGalaxyDisconnect()
    {
        PanelTop.SetActive(false);
        PanelBottom.SetActive(false);
    }

}
