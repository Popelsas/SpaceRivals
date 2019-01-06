// ===============================
// © GGTeam GalaxyNetwork module
// Модуль: Статистика
// Версия: 0.21
// ===============================

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GalaxyLib;

[HelpURL("http://wiki.greatgalaxy.ru/index.php?title=GalaxyServer:mod_statistics")]
public class mod_statistics : MonoBehaviour {

    public Text textMessCountCaption;
    public Text textMessCountIO;
    public Text textMessCntSec; //
    public Text textMessCntSecMAX; //
    public Text textMyID;
    public Text textPlayersCount;
    public Text textEventName;
    public Text textEventsCount;
    public Text textPing;
    public Text textRoomId;
    public Text textStatConnected;
    public Text textNetObjects;
    public Text textNetObjectsDll;
    public Text textNetObjectsLocal;
    public Text textNetObjectsDeleted;

    public Text textPingLatency;
    public Text textPingRoundtripTime;
    public Text textPingLocalTime;
    public Text textPingRemoteTime;
    public Text textPingRemoteTimeOffset;
    public Text textPingDroppedMessages;

    [Header("Основная панель")]
    public GameObject StatusPanel;
    int PrefStatusPanel = 0;

    [Header("График пинга")]
    public GameObject GrPingGO;
    mod_graph GrPing;
    int PrefPing = 0;

    [Header("График пакеты в сек(IN)")]
    public GameObject GrMessSecInGO;
    mod_graph GrMessSecIn;
    int PrefMessSecIn = 0;

    [Header("График пакеты в сек(OUT)")]
    public GameObject GrMessSecOutGO;
    mod_graph GrMessSecOut;
    int PrefMessSecOut = 0;

    //[Header("Панель списка игроков")]
    //public GameObject PanelPlayers;
    //public Transform PanelPlayersContent;
    //public GameObject PanelPlayersRow;


    byte swichShowStatInOut = 0;
    string swichShowStatInOutText1 = "Байты in/out:";
    string swichShowStatInOutText2 = "Сообщ. in/out:";
    string swichShowStatInOutText3 = "Пакеты in/out:";

    
    // Принятые сообщения
    long iMessCountIn;
    long old_iMessCountIn;
    long tMessCountIn;

    // Входящие байты
    long iByteCountIn;
    long old_iByteCountIn;
    long tByteCountIn;

    // Входящие пакеты
    long iPackageCountIn;
    long old_iPackageCountIn;
    long tPackageCountIn;

    // Отправленные сообщения
    long iMessCountOut;
    long old_iMessCountOut;
    long tMessCountOut;

    // Отправленные байты
    long iByteCountOut;
    long old_iByteCountOut;
    long tByteCountOut;

    // Отправленные пакеты
    long iPacketsCountOut;
    long old_iPacketsCountOut;
    long tPacketsCountOut;

    // ClientId
    uint myClientId;
    uint old_myClientId;

    // Игроков рядом
    int iPlayerCount;
    int old_iPlayerCount;

    GalaxyCommon.EventCode sEvent_name;
    GalaxyCommon.EventCode old_sEvent_name;

    uint iEvents_count;
    uint old_iEvents_count;

    float fPing;
    float old_fPing;

    ushort iRUDP_id;
    ushort old_iRUDP_id;

    bool bStatConnected;
    bool old_bStatConnected;

    ushort iRoomId;
    ushort old_iRoomId;

    int iNetObjects;
    int old_iNetObjects;

    int iNetObjectsLocal;
    int old_iNetObjectsLocal;

    int iNetObjectsDeleted;
    int old_iNetObjectsDeleted;

    int iNetObjectsDll;
    int old_iNetObjectsDll;

    // байт в сек
    float iByteCntSecIn = 0;
    float old_iByteCntSecIn;
    uint iByteCntSecIn_MAX;

    float iByteCntSecOut = 0;
    float old_iByteCntSecOut;
    uint iByteCntSecOut_MAX;

    // сообщений в сек
    float iMessageCntSecIn = 0;
    float old_iMessageCntSecIn;
    uint iMessageCntSecIn_MAX;

    float iMessageCntSecOut = 0;
    float old_iMessageCntSecOut;
    uint iMessageCntSecOut_MAX;

    // пакетов в сек
    float iPacketsCntSecIn = 0;
    float old_iPacketsCntSecIn;
    uint iPacketsCntSecIn_MAX;

    float iPacketsCntSecOut = 0;
    float old_iPacketsCntSecOut;
    uint iPacketsCntSecOut_MAX;

    long tmp = 0;
    Color textMessCntSec_color;

    bool lowFPC = false;
    float loFPC_Timer;
    //[Header("Скорость обновления при lowFPC(сек)")]
    float tTimeRefresh = 2.0f;


    // Пинг
    float fPingLatency;
    float _fPingLatency;

    float fPingRoundtripTime;
    float _fPingRoundtripTime;

    double fPingLocalTime;
    double _fPingLocalTime;

    double fPingRemoteTime;
    double _fPingRemoteTime;

    float fPingRemoteTimeOffset;
    float _fPingRemoteTimeOffset;

    long fPingDroppedMessages;
    long _fPingDroppedMessages;


    float tTime;
//    float tTimeRefresh = 1.0f;


    public void SetLowFPC()
    {
        lowFPC = !lowFPC;
    }

    void Awake()
    {
        textMessCntSec_color = textMessCntSec.color;
        PrefStatusPanel = PlayerPrefs.GetInt("GG_PrefStatusPanel", 0);
        PrefPing = PlayerPrefs.GetInt("GG_PrefPing", 0);
        PrefMessSecIn = PlayerPrefs.GetInt("GG_PrefMessSecIn", 0);
        PrefMessSecOut = PlayerPrefs.GetInt("GG_PrefMessSecOut", 0);

        GrPing = GrPingGO.GetComponent<mod_graph>();
        GrMessSecIn = GrMessSecInGO.GetComponent<mod_graph>();
        GrMessSecOut = GrMessSecOutGO.GetComponent<mod_graph>();

        if (PrefStatusPanel == 1) StatusPanel.SetActive(true);
        if (PrefPing == 1) GrPingGO.SetActive(true);
        if (PrefMessSecIn == 1) GrMessSecInGO.SetActive(true);
        if (PrefMessSecOut == 1) GrMessSecOutGO.SetActive(true);

        OnButtosSwichBytesMesPack();
    }

	void Update () {

        tTime += Time.deltaTime;
        
        if(tTime >= 1.0f)
        {
            //if(GalaxyClient.Connection != null)
            if (GalaxyNetwork.Connection.connected)
            {
                // Байты
                tmp = GalaxyNetwork.Connection.GetCountBytesIn;
                iByteCntSecIn = (tmp - tByteCountIn) / tTime;
                tByteCountIn = tmp;

                tmp = GalaxyNetwork.Connection.GetCountBytesOut;
                iByteCntSecOut = (tmp - tByteCountOut) / tTime;
                tByteCountOut = tmp;

                // Сообщения
                tmp = GalaxyNetwork.Connection.GetCountMessagesIn;
                iMessageCntSecIn = (tmp - tMessCountIn) / tTime;
                tMessCountIn = tmp;

                tmp = GalaxyNetwork.Connection.GetCountMessagesOut;
                iMessageCntSecOut = (tmp - tMessCountOut) / tTime;
                tMessCountOut = tmp;

                // Пакеты
                tmp = GalaxyNetwork.Connection.GetCountPacketsIn;
                iPacketsCntSecIn = (tmp - tPackageCountIn) / tTime;
                tPackageCountIn = tmp;

                tmp = GalaxyNetwork.Connection.GetCountPacketsOut;
                iPacketsCntSecOut = (tmp - tPacketsCountOut) / tTime;
                tPacketsCountOut = tmp;

                tTime = 0;
            }

            if (ConnectionHeader.Info.ExcessSendRate)
                textMessCntSec.color = new Color(1.0f, 0.5f, 0.5f);
            else
                textMessCntSec.color = textMessCntSec_color;
        }
        
        if ((!lowFPC) || (tTime >= tTimeRefresh))
        {
            iMessCountIn = GalaxyNetwork.Info.ReceivedMessages;
            iByteCountIn = GalaxyNetwork.Info.ReceivedBytes;
            iPackageCountIn = GalaxyNetwork.Info.ReceivedPackets;

            iMessCountOut = GalaxyNetwork.Info.SentMessages;
            iByteCountOut = GalaxyNetwork.Info.SentBytes;
            iPacketsCountOut = GalaxyNetwork.Info.SentPackets;
            myClientId = GalaxyNetwork.Info.myClientId;
            iPlayerCount = GalaxyNetwork.Players.Count;

            sEvent_name = GalaxyNetwork.Info.lastEventName;
            iEvents_count = GalaxyNetwork.Info.eventsCount;

            fPing = GalaxyNetwork.Connection.GetPing * 1000;

            // пинги
            //fPingLatency = GalaxyNetwork.Connection.GetPing;
            fPingRoundtripTime = GalaxyNetwork.Connection.Info_RoundtripTime;
            fPingLocalTime = GalaxyNetwork.Connection.Info_LocalTime;
            fPingRemoteTime = GalaxyNetwork.Connection.Info_RemoteTime;
            fPingRemoteTimeOffset = GalaxyNetwork.Connection.Info_RemoteTimeOffset;
            fPingDroppedMessages = GalaxyNetwork.Connection.Info_DroppedMessages;

            fPingLatency = (float)(fPingRemoteTime - fPingLocalTime - fPingRemoteTimeOffset);




            bStatConnected = GalaxyNetwork.Connection.connected;
            iRoomId = GalaxyNetwork.Room.id;

            iNetObjectsDll = GalaxyNetwork.NetObjects.Count;
            iNetObjects = GalaxyEvents.netObjects.Count;
            iNetObjectsLocal = GalaxyEvents.netObjectsLocal.Count;
            iNetObjectsDeleted = GalaxyEvents.netObjectsDeleted.Count;

            // Байты
            if (swichShowStatInOut == 1)
            {
                if (iByteCountIn != old_iByteCountIn)
                {
                    textMessCountIO.text = iByteCountIn.ToString() + " / " + iByteCountOut.ToString();
                    old_iByteCountIn = iByteCountIn;
                }

                if (iByteCountOut != old_iByteCountOut)
                {
                    textMessCountIO.text = iByteCountIn.ToString() + " / " + iByteCountOut.ToString();
                    old_iByteCountOut = iByteCountOut;
                }
            }

            // Сообщения
            if (swichShowStatInOut == 2)
            {
                if (iMessCountIn != old_iMessCountIn)
                {
                    textMessCountIO.text = iMessCountIn.ToString() + " / " + iMessCountOut.ToString();
                    old_iMessCountIn = iMessCountIn;
                }

                if (iMessCountOut != old_iMessCountOut)
                {
                    textMessCountIO.text = iMessCountIn.ToString() + " / " + iMessCountOut.ToString();
                    old_iMessCountOut = iMessCountOut;
                }
            }

            // Пакеты
            if (swichShowStatInOut == 3)
            {
                if (iPackageCountIn != old_iPackageCountIn)
                {
                    textMessCountIO.text = iPackageCountIn.ToString() + " / " + iPacketsCountOut.ToString();
                    old_iPackageCountIn = iPackageCountIn;
                }

                if (iPacketsCountOut != old_iPacketsCountOut)
                {
                    textMessCountIO.text = iPackageCountIn.ToString() + " / " + iPacketsCountOut.ToString();
                    old_iPacketsCountOut = iPacketsCountOut;
                }
            }

            if (myClientId != old_myClientId)
            {
                textMyID.text = myClientId.ToString();
                old_myClientId = myClientId;
            }

            // Байты
            if (swichShowStatInOut == 1)
            {
                // Отправляем в ГРАФИК Байт в сек (Входящие)   //textMessCntSecMAX
                if (iByteCntSecIn != old_iByteCntSecIn)
                {
                    if (iByteCntSecIn > iByteCntSecIn_MAX) { iByteCntSecIn_MAX = (uint)iByteCntSecIn; textMessCntSecMAX.text = iByteCntSecIn_MAX + " / " + iByteCntSecOut_MAX; }
                    textMessCntSec.text = System.Math.Round(iByteCntSecIn, 1).ToString() + " / " + System.Math.Round(iByteCntSecOut, 1).ToString();
                    old_iByteCntSecIn = iByteCntSecIn;
                    GrMessSecIn.AddValue(iByteCntSecIn);
                }

                // Отправляем в ГРАФИК Байт в сек (Исходящие)
                if (iByteCntSecOut != old_iByteCntSecOut)
                {
                    if (iByteCntSecOut > iByteCntSecOut_MAX) { iByteCntSecOut_MAX = (uint)iByteCntSecOut; textMessCntSecMAX.text = iByteCntSecIn_MAX + " / " + iByteCntSecOut_MAX; }
                    textMessCntSec.text = System.Math.Round(iByteCntSecIn, 1).ToString() + " / " + System.Math.Round(iByteCntSecOut, 1).ToString();
                    old_iByteCntSecOut = iByteCntSecOut;
                    GrMessSecOut.AddValue(iByteCntSecOut);
                }
            }

            // Сообщения
            if (swichShowStatInOut == 2)
            {
                // Отправляем в ГРАФИК Сообщений в сек (Входящие)
                if (iMessageCntSecIn != old_iMessageCntSecIn)
                {
                    if (iMessageCntSecIn > iMessageCntSecIn_MAX) { iMessageCntSecIn_MAX = (uint)iMessageCntSecIn; textMessCntSecMAX.text = iMessageCntSecIn_MAX + " / " + iMessageCntSecOut_MAX; }
                    textMessCntSec.text = System.Math.Round(iMessageCntSecIn, 1).ToString() + " / " + System.Math.Round(iMessageCntSecOut, 1).ToString();
                    old_iMessageCntSecIn = iMessageCntSecIn;
                    GrMessSecIn.AddValue(iMessageCntSecIn);
                }

                // Отправляем в ГРАФИК Сообщений в сек (Исходящие)
                if (iMessageCntSecOut != old_iMessageCntSecOut)
                {
                    if (iMessageCntSecOut > iMessageCntSecOut_MAX) { iMessageCntSecOut_MAX = (uint)iMessageCntSecOut; textMessCntSecMAX.text = iMessageCntSecIn_MAX + " / " + iMessageCntSecOut_MAX; }
                    textMessCntSec.text = System.Math.Round(iMessageCntSecIn, 1).ToString() + " / " + System.Math.Round(iMessageCntSecOut, 1).ToString();
                    old_iMessageCntSecOut = iMessageCntSecOut;
                    GrMessSecOut.AddValue(iMessageCntSecOut);
                }
            }

            // Пакеты
            if (swichShowStatInOut == 3)
            {
                // Отправляем в ГРАФИК Пакетов в сек (Входящие)
                if (iPacketsCntSecIn != old_iPacketsCntSecIn)
                {
                    if (iPacketsCntSecIn > iPacketsCntSecIn_MAX) { iPacketsCntSecIn_MAX = (uint)iPacketsCntSecIn; textMessCntSecMAX.text = iPacketsCntSecIn_MAX + " / " + iPacketsCntSecOut_MAX; }
                    textMessCntSec.text = System.Math.Round(iPacketsCntSecIn, 1).ToString() + " / " + System.Math.Round(iPacketsCntSecOut, 1).ToString();
                    old_iPacketsCntSecIn = iPacketsCntSecIn;
                    GrMessSecIn.AddValue(iPacketsCntSecIn);
                }

                // Отправляем в ГРАФИК Пакетов в сек (Исходящие)
                if (iPacketsCntSecOut != old_iPacketsCntSecOut)
                {
                    if (iPacketsCntSecOut > iPacketsCntSecOut_MAX) { iPacketsCntSecOut_MAX = (uint)iPacketsCntSecOut; textMessCntSecMAX.text = iPacketsCntSecIn_MAX + " / " + iPacketsCntSecOut_MAX; }
                    textMessCntSec.text = System.Math.Round(iPacketsCntSecIn, 1).ToString() + " / " + System.Math.Round(iPacketsCntSecOut, 1).ToString();
                    old_iPacketsCntSecOut = iPacketsCntSecOut;
                    GrMessSecOut.AddValue(iPacketsCntSecOut);
                }
            }

            // Отправляем в ГРАФИК значение пинга
            if (fPing != old_fPing)
            {
                if (fPing > 0)
                {
                    //textPing.text = System.Math.Round(fPing, 2) + " мс";
                    textPing.text = fPing.ToString("#") + " мс";   //"#.#"
                    GrPing.AddValue(fPing);
                }
                else
                    textPing.text = "-";
                old_fPing = fPing;
            }

            // пинги
            if (_fPingLatency != fPingLatency)
            {
                textPingLatency.text = fPingLatency.ToString("0.000");
                _fPingLatency = fPingLatency;
            }

            if (_fPingRoundtripTime != fPingRoundtripTime)
            {
                textPingRoundtripTime.text = fPingRoundtripTime.ToString("0.00");
                _fPingRoundtripTime = fPingRoundtripTime;
            }

            if (_fPingLocalTime != fPingLocalTime)
            {
                textPingLocalTime.text = fPingLocalTime.ToString("0.00");
                _fPingLocalTime = fPingLocalTime;
            }

            if (_fPingRemoteTime != fPingRemoteTime)
            {
                textPingRemoteTime.text = fPingRemoteTime.ToString("0.00");
                _fPingRemoteTime = fPingRemoteTime;
            }

            if (_fPingRemoteTimeOffset != fPingRemoteTimeOffset)
            {
                textPingRemoteTimeOffset.text = fPingRemoteTimeOffset.ToString("0.000");
                _fPingRemoteTimeOffset = fPingRemoteTimeOffset;
            }

            if (_fPingDroppedMessages != fPingDroppedMessages)
            {
                textPingDroppedMessages.text = fPingDroppedMessages.ToString();
                _fPingDroppedMessages = fPingDroppedMessages;
            }



            if (iPlayerCount != old_iPlayerCount)
            {
                textPlayersCount.text = iPlayerCount.ToString();
                old_iPlayerCount = iPlayerCount;
                //UpdatePanelPlayers();
            }

            if (sEvent_name != old_sEvent_name)
            {
                textEventName.text = sEvent_name.ToString();
                old_sEvent_name = sEvent_name;
            }

            if (iEvents_count != old_iEvents_count)
            {
                textEventsCount.text = iEvents_count.ToString();
                old_iEvents_count = iEvents_count;
            }

            /*
            if (iRUDP_id != old_iRUDP_id)
            {
                textRUDP.text = iRUDP_id.ToString();
                old_iRUDP_id = iRUDP_id;
            }
            */

            if (bStatConnected != old_bStatConnected)
            {
                if (bStatConnected)
                    textStatConnected.text = "<color=#ffb400>Да</color>";  //ffb400  008fbe
                else
                    textStatConnected.text = "<color=#008fbe>Нет</color>";
                old_bStatConnected = bStatConnected;
            }

            if (iRoomId != old_iRoomId)
            {
                textRoomId.text = iRoomId.ToString();
                if (iRoomId == 0) textRoomId.text = "[0] Лобби";
                old_iRoomId = iRoomId;
            }

            if (iNetObjects != old_iNetObjects)
            {
                textNetObjects.text = iNetObjects.ToString();
                if (iNetObjects == 0) textNetObjects.text = "-";
                old_iNetObjects = iNetObjects;
            }

            if (iNetObjectsLocal != old_iNetObjectsLocal)
            {
                textNetObjectsLocal.text = iNetObjectsLocal.ToString();
                if (iNetObjectsLocal == 0) textNetObjectsLocal.text = "-";
                old_iNetObjectsLocal = iNetObjectsLocal;
            }

            if (iNetObjectsDeleted != old_iNetObjectsDeleted)
            {
                textNetObjectsDeleted.text = iNetObjectsDeleted.ToString();
                if (iNetObjectsDeleted == 0) textNetObjectsDeleted.text = "-";
                old_iNetObjectsDeleted = iNetObjectsDeleted;
            }

            if (iNetObjectsDll != old_iNetObjectsDll)
            {
                textNetObjectsDll.text = iNetObjectsDll.ToString();
                if (iNetObjectsDll == 0) textNetObjectsDll.text = "-";
                old_iNetObjectsDll = iNetObjectsDll;
            }
        }
    }

    // Отобразить модуль списка игроков
    public void ShowPanelPlayers()
    {
        mod_online_players _mod_online_players = FindObjectOfType<mod_online_players>();
        if (_mod_online_players != null) _mod_online_players.Show(true);
        else Debug.LogWarning("На сцене не обнаружен модуль [Список онлайн-игроков]. Его можно добавить из меню GalaxyNetwork.");
    }


    public void ShowHidePanel()
    {
        StatusPanel.SetActive(!StatusPanel.activeSelf);

        if (StatusPanel.activeSelf)
            PlayerPrefs.SetInt("GG_PrefStatusPanel", 1);
        else
            PlayerPrefs.SetInt("GG_PrefStatusPanel", 0);

        if (StatusPanel.activeSelf == true) lowFPC = false;
        else lowFPC = true;

    }

    public void ShowGraphPing()
    {
        GrPing.gameObject.SetActive(true);
        GrPing.working = true;

        if (GrPing.gameObject.activeSelf)
            PlayerPrefs.SetInt("GG_PrefPing", 1);
        else
            PlayerPrefs.SetInt("GG_PrefPing", 0);
    }

    public void ShowGraphPackIn()
    {
        GrMessSecIn.gameObject.SetActive(true);
        GrMessSecIn.working = true;

        if (GrMessSecIn.gameObject.activeSelf)
            PlayerPrefs.SetInt("GG_PrefMessSecIn", 1);
        else
            PlayerPrefs.SetInt("GG_PrefMessSecIn", 0);
    }

    public void ShowGraphPackOut()
    {
        GrMessSecOut.gameObject.SetActive(true);
        GrMessSecOut.working = true;

        if (GrMessSecOut.gameObject.activeSelf)
            PlayerPrefs.SetInt("GG_PrefMessSecOut", 1);
        else
            PlayerPrefs.SetInt("GG_PrefMessSecOut", 0);
    }

    public void OnButtosSwichBytesMesPack()
    {
        swichShowStatInOut++;
        if (swichShowStatInOut > 3) swichShowStatInOut = 1;

        if (swichShowStatInOut == 1)
        {
            textMessCountCaption.text = swichShowStatInOutText1;
            old_iByteCountIn = -1;
            old_iByteCountOut = -1;
        }


        if (swichShowStatInOut == 2)
        {
            textMessCountCaption.text = swichShowStatInOutText2;
            old_iMessCountIn = -1;
            old_iMessCountOut = -1;
        }

        if (swichShowStatInOut == 3)
        {
            textMessCountCaption.text = swichShowStatInOutText3;
            old_iPackageCountIn = -1;
            old_iPacketsCountOut = -1;
        }
    }
}
