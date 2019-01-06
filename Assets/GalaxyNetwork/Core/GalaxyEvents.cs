/* =================================
 * GGTeam © GalaxyNetwork components
 * ---------------------------------
 * All right reserved 2017-2018
 * Murnik Roman, Totmin Deyn
 * =================================
*/

using System;
using System.Collections.Generic;
using UnityEngine;
using GalaxyLib;
using GalaxyCommon;
using UnityEngine.Events;

namespace GalaxyLib
{
    [HelpURL("http://wiki.greatgalaxy.ru/index.php?title=GalaxyServer:GalaxyEvents")]
    public class GalaxyEvents : MonoBehaviour
    {
        public static Log log = Log.GetLogger(typeof(GalaxyEvents));

        public string app_key;

        #region ===== С О Б Ы Т И Я =====

        /// <summary>
        /// Подключение к Серверу
        /// </summary>
        /// <param name="errorCode">код ошибки</param>
        public delegate void DelegateOnGalaxyConnect(ErrorCode errorCode);
        /// <summary>
        /// <para>Подключение к Серверу</para>
        /// OnGalaxyConnect(ErrorCode errorCode)
        /// </summary>
        public static event DelegateOnGalaxyConnect OnGalaxyConnect;

        /// <summary>
        /// Отключение от Сервера
        /// </summary>
        public delegate void DelegateOnGalaxyDisconnect();
        /// <summary>
        /// <para>Отключение от Сервера</para>
        /// OnGalaxyDisconnect()
        /// </summary>
        public static event DelegateOnGalaxyDisconnect OnGalaxyDisconnect;

        /// <summary>
        /// Пришло сообщение чата
        /// </summary>
        public delegate void DelegateOnGalaxyChatMessage(UInt32 clientId, String nikname, String textMessage, Byte channel);      
        /// <summary>
        /// <para>Пришло сообщение чата</para>
        /// OnGalaxyChatMessage(UInt32 clientId, String nikname, String textMessage, Byte channel)
        /// </summary>
        public static event DelegateOnGalaxyChatMessage OnGalaxyChatMessage;

        /// <summary>
        /// Вы создали и вошли в новую комнату
        /// </summary>
        public delegate void DelegateOnGalaxyEnterToNewRoom(GGRoom room, ErrorCode errorCode);
        /// <summary>
        /// <para>Вы создали и вошли в новую комнату</para>
        /// OnGalaxyEnterToNewRoom(GGRoom room, ErrorCode errorCode)
        /// </summary>
        public static event DelegateOnGalaxyEnterToNewRoom OnGalaxyEnterToNewRoom;

        /// <summary>
        /// Вход в комнату
        /// </summary>
        public delegate void DelegateOnGalaxyRoomEnter(UInt32 clientId, String nickname, ErrorCode errorCode);
        /// <summary>
        /// <para>Вход в комнату</para>
        /// OnGalaxyRoomEnter (UInt32 clientId, String nickname, ErrorCode errorCode)
        /// </summary>
        public static event DelegateOnGalaxyRoomEnter OnGalaxyRoomEnter;

        /// <summary>
        /// Выход из комнаты
        /// </summary>
        public delegate void DelegateOnGalaxyRoomExit(UInt32 clientId, String nikname, ErrorCode errorCode);
        /// <summary>
        /// <para>Выход из комнаты</para>
        /// OnGalaxyRoomExit(UInt32 clientId, String nikname, ErrorCode errorCode)
        /// </summary>
        public static event DelegateOnGalaxyRoomExit OnGalaxyRoomExit;

        /// <summary>
        /// Параметры комнаты изменились
        /// </summary>
        public delegate void DelegateOnGalaxyRoomUpdate(GGRoom room, ErrorCode errorCode);
        /// <summary>
        /// <para>Параметры комнаты изменились</para>
        /// OnGalaxyRoomUpdate(GGRoom room, ErrorCode errorCode)
        /// </summary>
        public static event DelegateOnGalaxyRoomUpdate OnGalaxyRoomUpdate;

        /// <summary>
        /// Пришел список видимых комнат в лобби
        /// </summary>
        public delegate void DelegateOnGalaxyRoomList(List<GGRoom> roomsList, ErrorCode errorCode);
        /// <summary>
        /// <para>Пришел список видимых комнат в лобби</para>
        /// OnGalaxyRoomList(List<GGRoom> roomsList, ErrorCode errorCode)
        /// </summary>
        public static event DelegateOnGalaxyRoomList OnGalaxyRoomList;

        /// <summary>
        /// Пришли потоковые данные
        /// </summary>
        public delegate void DelegateOnGalaxyStreamData(UInt32 clientId, Byte channel, Byte[] data);
        /// <summary>
        /// <para>Пришли потоковые данные</para>
        /// OnGalaxyStreamData(UInt32 clientId, Byte channel, Byte[] data)
        /// </summary>
        public static event DelegateOnGalaxyStreamData OnGalaxyStreamData;

        /// <summary>
        /// Создание сетевого объекта
        /// </summary>
        public delegate void DelegateOnGalaxyObjectInstantiate(GGObject netObject, ErrorCode errorCode);
        /// <summary>
        /// <para>Создание сетевого объекта</para>
        /// OnGalaxyObjectInstantiate(GGObject netObject, ErrorCode errorCode)
        /// </summary>
        public static event DelegateOnGalaxyObjectInstantiate OnGalaxyObjectInstantiate;

        /// <summary>
        /// Мы стали владельцем сетевого объекта
        /// </summary>
        public delegate void DelegateOnGalaxyObjectTransfer(GGObject netObject);
        /// <summary>
        /// <para>Мы стали владельцем сетевого объекта</para>
        /// OnGalaxyObjectTransfer(GGObject netObject)
        /// </summary>
        public static event DelegateOnGalaxyObjectTransfer OnGalaxyObjectTransfer;

        /// <summary>
        /// Изменение состояния аниматора
        /// </summary>
        // public delegate void DelegateOnGalaxyObjectAnimatorState(GGObject netObject, ErrorCode errorCode);
        // public static event DelegateOnGalaxyObjectAnimatorState OnGalaxyObjectAnimatorState;

        /// <summary>
        /// Изменение сетевых переменных
        /// </summary>
        public delegate void DelegateOnGalaxyObjectNetVars(GGObject netObject);
        /// <summary>
        /// <para>Изменение сетевых переменных</para>
        /// OnGalaxyObjectNetVars(GGObject netObject)
        /// </summary>
        public static event DelegateOnGalaxyObjectNetVars OnGalaxyObjectNetVars;

        /// <summary>
        /// Вызвана RPC
        /// </summary>
        //public delegate void DelegateOnGalaxyRPC(GGObject netObject_to, GGObject netObject_from, String rpc_name, object data);
        //public static event DelegateOnGalaxyRPC OnGalaxyRPC;

        /// <summary>
        /// Перемещение
        /// </summary>
        //    public delegate void DelegateOnGalaxyTransformPosition(GGPlayer player, Vector3 position);  //UInt32 clientId
        //    public static event DelegateOnGalaxyTransformPosition OnGalaxyTransformPosition;

        /// <summary>
        /// Вращение
        /// </summary>
        //    public delegate void DelegateOnGalaxyTransformRotation(GGPlayer player, Quaternion rotation);   //UInt32 clientId
        //    public static event DelegateOnGalaxyTransformRotation OnGalaxyTransformRotation;

        /// <summary>
        /// Удаление сетевого объекта
        /// </summary>
        //public delegate void DelegateOnGalaxyObjectDestroy(ushort id, ErrorCode errorCode);
        //public static event DelegateOnGalaxyObjectDestroy OnGalaxyObjectDestroy;


        /// <summary>
        /// Статистика эвентов
        /// </summary>
        public delegate void DelegateOnEventStatistic(ServerEvent serverEvent);
        public static event DelegateOnEventStatistic OnEventStatistic;


        #endregion

        [Space(16)]
        public UnityEvent On_Connect;
        public UnityEvent On_Disconnect;

        // Вспомогательные
        Texture2D textureToDisplay;
        bool showConnectedLogo = true;
        bool showPing = false;
        float timeConnectedLogo = 8.0f;

        public static Dictionary<ushort, GalaxyNetID> netObjects = new Dictionary<ushort, GalaxyNetID>();       // <id, NetID>
        public static Dictionary<ushort, GalaxyNetID> netObjectsLocal = new Dictionary<ushort, GalaxyNetID>();  // <local_id, NetID>
        public static List<ushort> netObjectsDeleted = new List<ushort>();                                      // <local_id> объекты на удаление

        public static List<GameObject> netGameObjects = new List<GameObject>();

        // Сетевые объекты в ресурсах
        Dictionary<string, GameObject> listNetObjects = new Dictionary<string, GameObject>();

        private static GalaxyEvents _instance;
        public static GalaxyEvents Instance
        {
            get { return _instance; }
        }


        #region на удаление
        /*
        public class LogAdapter : ILogAdapter
        {
            GalaxyConsole console = (GalaxyConsole)Behaviour.FindObjectOfType(typeof(GalaxyConsole));
            //GalaxyConsole console;  // = (GalaxyConsole)Behaviour.FindObjectOfType(typeof(GalaxyConsole));

            //public LogAdapter()
            //{
            //    console = FindObjectOfType<GalaxyConsole>();
            //}

            public void Log(Log.LogEvent @event)
            {
                //if (console == null)
                //{
                //    Debug.LogError("Не удалось найти GalaxyConsole скрипт");
                //    return;
                //}
                string logLevelText;

                switch (@event.Level)
                {
                    case GalaxyCommon.LogLevel.None:
                        logLevelText = "<color=#FFFFFFff>" + @event.Level + "</color>";
                        break;

                    case GalaxyCommon.LogLevel.Trace:
                        logLevelText = "<color=#595959ff>" + @event.Level + "</color>";
                        break;

                    case GalaxyCommon.LogLevel.Info:
                        logLevelText = "<color=#50F0F0ff>" + @event.Level + "</color>";
                        break;

                    case GalaxyCommon.LogLevel.Debug:
                        logLevelText = "<color=#999999ff>" + @event.Level + "</color>";
                        break;

                    case GalaxyCommon.LogLevel.Warn:
                        logLevelText = "<color=#F0F040ff>" + @event.Level + "</color>";
                        break;

                    case GalaxyCommon.LogLevel.Error:
                        logLevelText = "<color=#F05050ff>" + @event.Level + "</color>";
                        break;

                    case GalaxyCommon.LogLevel.All:
                        logLevelText = "<color=#30E030ff>" + @event.Level + "</color>";
                        break;

                    default:
                        logLevelText = "<color=#FFFFFFff>" + @event.Level + "</color>";
                        break;
                }


                //console.WriteLine("<color=#999999ff>" + @event.Time.Milliseconds + "</color>" + "[" + @event.Log.Name + "] [" + logLevelText + "] " + @event.Message);

                if (console != null)
                    console.WriteLine("[" + @event.Log.Name + "] [" + logLevelText + "] " + @event.Message);
            }
        }
        */
        #endregion

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            Application.runInBackground = true;
            _instance = this;

            #region Подготовка ПРЕФАБОВ
            foreach (var item in Resources.LoadAll<GameObject>(""))
            {
                GalaxyNetID gid = item.GetComponent<GalaxyNetID>();
                if (gid != null)
                {
                    if (listNetObjects.ContainsKey(item.name))
                    {
                        Debug.LogWarning("Найден дубликат объекта "+item.name+". Переименуйте его пожалуйста.");
                    }
                    else
                        listNetObjects.Add(item.name, item);
                }
            }
            Resources.UnloadUnusedAssets();
            #endregion

            // Инициализация log4net adapter
            Log.SetAdapter(new GalaxyConsole.LogAdapter());

            Application.targetFrameRate = 60;

            //            GalaxyNetwork.Config.MAX_RATE_SEND_BYTES_IN_SECOND = 16384;     // 6144; 5120;
            GalaxyNetwork.Config.MAX_RATE_SEND_BYTES_IN_SECOND = 65535;

            textureToDisplay = Resources.Load<Texture2D>("Gizmos/GalaxyIcon-32");

            // Вводим ключ
            if (app_key.Length < 5) Debug.LogWarning(" <color=#ff0000ff>[GalaxyEvents]</color> Не указан ключ. Его можно сгенерировать на сайте.");
            GalaxyNetwork.Config.app_key = GalaxyEvents.Instance.app_key;


            // !!!!!!! Устарело. Причина: НЕ СТАБИЛЬНАЯ РАБОТА. прописать 185.76.145.23:30200
           //GetServer();

        }

        #region На удаление
        // !!!! Устарело. Будет переделано в получение списка серверов (дубликатов)
        /*
        private void GetServer()
        {
            string url = "http://server.champer.ru/getconnect.php";
            WWWForm form = new WWWForm();
            form.AddField("app_key", app_key);
            form.AddField("version", "0.0.0");
            WWW www = new WWW(url, form);
            StartCoroutine(WaitForRequest(www));
        }
        */

        /*
        private IEnumerator WaitForRequest(WWW www)
        {
            yield return www;
            if (www.error == null)
            {
                if (www.text.Length > 0)
                {
                    string[] data = www.text.Remove(0, 1).Split(';');
                    string serv_name = "";
                    string server_port = "";
                    string serv_ip = "";
                    if (data.Length >= 2) serv_ip = data[1];
                    if (data.Length >= 3) server_port = data[2];
                    if (data.Length >= 4) serv_name = data[3];

                    switch (System.Convert.ToInt32(data[0]))
                    {
                        
                        case 0:
                            SetServerInfo(serv_ip, System.Convert.ToInt32(server_port), serv_name, "");
                            break;
                        case 1:
                            SetServerInfo("", 0, serv_name, "Сервера с таким APP_KEY не существует");
                            break;
                        case 2:
                            SetServerInfo("", 0, serv_name, "Сервер с таким APP_KEY заблокирован");
                            break;
                        case 3:
                            SetServerInfo("", 0, serv_name, "Ваша версия клиента устарела, пожалуйста обновите приложение");
                            break;
                    }
                }
            }
        }
        */

        /*
         * УСТАРЕЛО!
        private void SetServerInfo(string ip, int port, string serv_name, string error)
        {
            if (error.Length == 0)
            {
                //Debug.Log("Server: " + ip + ":" + port);
                GalaxyNetwork.Connection.SelectServer(ip, port, serv_name);
            }
            else
            {
                Debug.LogError(error);
            }
        }
        */

        #endregion

        [ExecuteInEditMode]
        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "GalaxyCore", false);
        }

        void Update()
        {
            //NetObjectsUpdate();
            //_timeEventsPeriod += Time.deltaTime;
            //if (_timeEventsPeriod >= (timeEventsPeriodF))
            //{
            //    _timeEventsPeriod = 0;

            //if(GalaxyClient.Connection.Connected)
            int cnt = GalaxyNetwork.Events.count;

            while (cnt > 0)
            {
                cnt--;
                ServerEvent eventCurrent = GalaxyNetwork.Events.GetNext();

                // Для статистики эвентов
                if (OnEventStatistic != null)
                    OnEventStatistic(eventCurrent);

                // === ПРИШЛО СОБЫТИЕ С СЕРВЕРА ===
                switch (eventCurrent.eventCode)
                {
                    #region Перемещение
                    case EventCode.OnGalaxyObjectPosition:
                        {
                            GGObject netObject = (GGObject)eventCurrent.eventData[0];
                            if (!netObject.isMy)
                                netObjects[netObject.id].receivedPosition(netObject.position);
                        }
                        break;
                    #endregion

                    #region Перемещение и Вращение
                    case EventCode.OnGalaxyObjectPositionAndRotation:
                        {
                            GGObject netObject = (GGObject)eventCurrent.eventData[0];
                            if (!netObject.isMy)
                            {
                                netObjects[netObject.id].receivedPosition(netObject.position);
                                netObjects[netObject.id].receivedRotation(netObject.rotation);
                            }
                        }
                        break;
                    #endregion

                    #region Поворот
                    case EventCode.OnGalaxyObjectRotation:
                        {
                            GGObject netObject = (GGObject)eventCurrent.eventData[0];

                            if (!netObject.isMy)
                                netObjects[netObject.id].receivedRotation(netObject.rotation);
                        }
                        break;
                    #endregion

                    #region (в разработке)Состояние аниматора
                    /*
                    case EventCode.OnGalaxyObjectAnimatorState:
                        {
                            GGObject netObject = (GGObject)eventCurrent.eventData[0];
                            //1 Quaternion rotation = (Quaternion)eventCurrent.eventData[1];

                            if (!netObject.isMy)
                                //{
                                //Debug.Log("11111");
                        //        netObjects[netObject.id].receivedAnimatorState(netObject.animator_state, netObject.animator_layer);
                            //}
                            //else
                            //    Debug.Log("22222222");

                            //1 netObjects[ggo.id].receivedRotation(rotation);


                            //netObjects[ggo.id].gameObject.transform.rotation = rotation;

                            //if (OnGalaxyObjectRotation != null)
                            //    OnGalaxyObjectRotation(player, position);
                            //OnGalaxyObjectAnimatorState()
                        }
                        break;
                    */
                    #endregion

                    #region Состояние сетевых переменных
                    case EventCode.OnGalaxyObjectNetVars:
                        {
                            GGObject netObject = (GGObject)eventCurrent.eventData[0];

                            if (OnGalaxyObjectNetVars != null)
                                OnGalaxyObjectNetVars(netObject);

                            if (!netObject.isMy)
                            {
                                if(netObjects.ContainsKey(netObject.id))
                                    netObjects[netObject.id].receivedNetVars();
                                else
                                    Debug.LogWarning("[GalaxyNetwork] Синхронизируемая переменная не найдена или она не помечена аттрибутом [NetVar]");
                            }
                        }
                        break;
                    #endregion

                    #region Вызов RPC
                    case EventCode.OnGalaxyObjectRPC:
                        {
                            GGObject netObject_to = (GGObject)eventCurrent.eventData[0];        // кому
                            string rpc_name = (String)eventCurrent.eventData[1];                // имя RPC
                            object[] rpc_data = (object[])eventCurrent.eventData[2];            // параметры RPC

                            if (netObject_to.user_rpc_method.ContainsKey(rpc_name) && netObject_to.user_rpc_method[rpc_name] != null && netObject_to.user_rpc_component.ContainsKey(rpc_name))
                                netObject_to.user_rpc_method[rpc_name].Invoke(netObject_to.user_rpc_component[rpc_name], rpc_data);
                            else
                                Debug.LogWarning("[GalaxyNetwork] Метод RPC не найден. Он должен быть публичным (public) и помечен аттрибутом [NetRPC]");
                        }
                        break;
                    #endregion

                    #region Новый сетевой объект
                    case EventCode.OnGalaxyObjectInstantiate:
                        {
                            //log.Info("Event> " + eventCurrent.name.ToString());

                            ErrorCode errorCode = (ErrorCode)eventCurrent.eventData[0];
                            GGObject netObject = (GGObject)eventCurrent.eventData[1];

                            // Проверка владельца пришедшего объекта
                            if (netObject.owner_id == GalaxyNetwork.Connection.clientId)
                            {
                                // Этот объект уже был удален?
                                if (netObjectsDeleted.Contains(netObject.local_id))
                                {
                                    netObjectsDeleted.Remove(netObject.local_id);

                                    // Запрос на удаление объекта
                                    GalaxyNetwork.SendOperation.NetObject.Destroy(netObject.id);         //?
                                }
                                else
                                {
                                    if (netObjectsLocal.ContainsKey(netObject.local_id))
                                    {
                                        // Инициализируем
                                        netObjectsLocal[netObject.local_id].Init(netObject);
                                    }
                                    else
                                    {
                                        // Удаляем из списков (объекта не существует)
                                        GalaxyNetwork.NetObjects.Remove(netObject.id);
                                    }
                                }

                            }
                            else
                            {
                                //Debug.Log("=" + netObject.id);
                                if(!listNetObjects.ContainsKey(netObject.prefName))
                                {
                                    Debug.LogWarning("Не могу создать сетевой объект ["+ netObject.prefName + "]");
                                    return;
                                }
                                GameObject go = Instantiate(listNetObjects[netObject.prefName], netObject.position, netObject.rotation);
                                //GameObject go = Instantiate(Resources.Load<GameObject>(netObject.prefName), netObject.position, netObject.rotation);
                                GalaxyNetID nId = go.GetComponent<GalaxyNetID>();
                                if (nId == null) log.Debug("Не найден компонент [GalaxyNetID]");
                                //if (nId == null) Debug.LogError("Не найден компонент [GalaxyNetID]");
                                nId.Init(netObject);
                            }

                            if (OnGalaxyObjectInstantiate != null)
                            {
                                OnGalaxyObjectInstantiate(netObject, errorCode);
                            }
                        }
                        break;
                    #endregion

                    #region Удаление сетевого объекта
                    case EventCode.OnGalaxyObjectDestroy:
                        {
                            //!ErrorCode errorCode = (ErrorCode)eventCurrent.eventData[0];
                            ushort id = (ushort)eventCurrent.eventData[1];

                            //if (OnGalaxyObjectDestroy != null)
                            //{
                            //OnGalaxyObjectDestroy(id, errorCode);
                            //}

                            if (netObjects.ContainsKey(id))
                                if (netObjects[id] != null)                      //!!!!!!!!!!!!
                                    Destroy(netObjects[id].gameObject);

                            netObjects.Remove(id);

                            //}
                        }
                        break;
                    #endregion

                    #region Стали владельцем сетевого объекта
                    case EventCode.OnGalaxyObjectTransfer:
                        {
                            GGObject netObj = (GGObject)eventCurrent.eventData[0];
                            if (netObjects.ContainsKey(netObj.id))
                            {
                                netObjects[netObj.id].StartSync();
                                if (OnGalaxyObjectTransfer != null)
                                {
                                    OnGalaxyObjectTransfer(netObj);
                                }
                            }
                            else
                                Debug.LogWarning("NetObject [" + netObj.id + "] not exist");
                        }
                        break;
                    #endregion

                    #region ===== Подключение =====
                    case EventCode.OnGalaxyConnect:
                        {
                            log.Info("Event> " + eventCurrent.eventCode.ToString());
                            if (On_Connect != null) On_Connect.Invoke();
                            ErrorCode errorCode = (ErrorCode)eventCurrent.eventData[0];
                            //log.Info("!!!> " + errorCode.ToString());
                            if (OnGalaxyConnect != null)
                                OnGalaxyConnect(errorCode);

                            timeConnectedLogo = 8.0f;
                            showConnectedLogo = true;
                        }
                        break;
                    #endregion

                    #region ===== Отключение =====
                    case EventCode.OnGalaxyDisconnect:
                        {
                            //log.Info("Event> " + eventCurrent.eventCode.ToString());
                            if (On_Disconnect != null) On_Disconnect.Invoke();
                            if (OnGalaxyDisconnect != null)
                                OnGalaxyDisconnect();

                            timeConnectedLogo = 8.0f;
                            showConnectedLogo = true;
                        }
                        break;
                    #endregion

                    #region ===== Сообщение чата =====
                    // (UInt32)clientId
                    // (String)nikname
                    // (String)textMessage
                    // (Byte)channel
                    case EventCode.OnGalaxyChatMessage:
                        {
                            //log.Info("Event> " + eventCurrent.eventCode.ToString());
                            UInt32 clientId = (UInt32)eventCurrent.eventData[0];
                            String nikname = (String)eventCurrent.eventData[1];
                            String textMessage = (String)eventCurrent.eventData[2];
                            Byte channel = (Byte)eventCurrent.eventData[3];
                            if (OnGalaxyChatMessage != null)
                                OnGalaxyChatMessage(clientId, nikname, textMessage, channel);
                        }
                        break;
                    #endregion

                    #region ===== Потоковые данные =====
                    // (UInt32)clientId
                    // (String)nikname
                    // (String)textMessage
                    // (Byte)channel
                    case EventCode.OnGalaxyStreamData:
                        {
                            //log.Info("Event> " + eventCurrent.eventCode.ToString());
                            UInt32 clientId = (UInt32)eventCurrent.eventData[0];
                            Byte channel = (Byte)eventCurrent.eventData[1];
                            Byte[] data = (byte[])eventCurrent.eventData[2];
                            if (OnGalaxyStreamData != null)
                                OnGalaxyStreamData(clientId, channel, data);
                        }
                        break;
                    #endregion

                    #region ===== Создана новая комната =====
                    // (Byte)ErrorCode
                    // (UInt16)room.Id
                    // (String)room.name
                    // (UInt32)room.hostClientId
                    // (UInt16)room.maxPlayersCount
                    // (Boolean)room.visible
                    case EventCode.OnGalaxyEnterToNewRoom:
                        {
                            //log.Info("Event> " + eventCurrent.eventCode.ToString());
                            ErrorCode errorCode = (ErrorCode)eventCurrent.eventData[0];
                            UInt16 roomId = (UInt16)eventCurrent.eventData[1];
                            String roomName = (String)eventCurrent.eventData[2];
                            UInt32 roomHostClientId = (UInt32)eventCurrent.eventData[3];
                            UInt16 roomMaxPlayersCount = (UInt16)eventCurrent.eventData[4];
                            Boolean roomVisible = (Boolean)eventCurrent.eventData[5];

                            GGRoom room = new GGRoom(roomName, roomMaxPlayersCount, roomVisible);
                            room.owner_client_id = roomHostClientId;    //client_host_id
                            room.id = roomId;
                            //                        room.currentPlayersCount ++;

                            if (OnGalaxyEnterToNewRoom != null)
                                //OnGalaxyEnterToNewRoom(GalaxyClient.Room, errorCode);
                                OnGalaxyEnterToNewRoom(room, errorCode);
                        }
                        break;
                    #endregion

                    #region ===== Вход игрока в комнату =====
                    // (Byte)ErrorCode
                    // (UInt32)client.Id
                    // (String)client.nikname
                    case EventCode.OnGalaxyRoomEnter:
                        {
                            //log.Info("Event> " + eventCurrent.eventCode.ToString());
                            ErrorCode errorCode = (ErrorCode)eventCurrent.eventData[0];
                            UInt32 clientId = (UInt32)eventCurrent.eventData[1];
                            String clientNikname = (String)eventCurrent.eventData[2];

                            if (OnGalaxyRoomEnter != null)
                                OnGalaxyRoomEnter(clientId, clientNikname, errorCode);
                            return;
                        }
                        //break;
                    #endregion

                    #region ===== Выход игрока из комнаты =====
                    // (Byte)ErrorCode
                    // (UInt32)client.Id
                    // (String)client.nikName
                    case EventCode.OnGalaxyRoomExit:
                        {
                            //log.Info("Event> " + eventCurrent.eventCode.ToString());
                            ErrorCode errorCode = (ErrorCode)eventCurrent.eventData[0];
                            UInt32 clientId = (UInt32)eventCurrent.eventData[1];
                            String clientNikname = (String)eventCurrent.eventData[2];

                            if (OnGalaxyRoomExit != null)
                                OnGalaxyRoomExit(clientId, clientNikname, errorCode);
                        }
                        break;
                    #endregion

                    #region ===== Параметры комнаты изменились =====
                    // (Byte)ErrorCode
                    // (UInt16)room.Id
                    // (String)room.name
                    // (UInt32)room.hostClientId
                    // (UInt16)room.maxPlayersCount
                    // (Boolean)room.visible
                    // (UInt16)room.currentPlayersCount
                    case EventCode.OnGalaxyRoomUpdate:
                        {
                            //log.Info("Event> " + eventCurrent.eventCode.ToString());
                            ErrorCode errorCode = (ErrorCode)eventCurrent.eventData[0];
                            GGRoom room = (GGRoom)eventCurrent.eventData[1];
                            if (OnGalaxyRoomUpdate != null)
                                OnGalaxyRoomUpdate(room, errorCode);
                        }
                        break;
                    #endregion

                    #region ===== Пришел список комнат =====
                    // (Byte) ErrorCode
                    // (Byte)roomList.count (кол-во элементов списка)
                    // == элемент списка ==
                    // (List<GGRoom>)roomsList список
                    case EventCode.OnGalaxyRoomList:
                        {
                            //log.Info("Event> " + eventCurrent.eventCode.ToString());
                            ErrorCode errorCode = (ErrorCode)eventCurrent.eventData[0];
                            List<GGRoom> roomsList = new List<GGRoom>((List<GGRoom>)eventCurrent.eventData[1]);

                            if (OnGalaxyRoomList != null)
                                OnGalaxyRoomList(roomsList, errorCode);
                        }
                        break;
                    #endregion

                    #region ===== Известная не известная комманда =====
                    case EventCode.uncnown:
                        Debug.LogError("Event> Ошибка. Не известный евент");
                        break;
                    #endregion

                    #region ===== Не известная комманда =====
                    default:
                        Debug.LogError("Event> Ошибка. Не известный евент: [" + (byte)eventCurrent.eventCode + "] " + eventCurrent.eventCode);
                        break;
                    #endregion
                }
            }
        }


#if UNITY_STANDALONE_WIN
    void OnGUI()
    {
        if (showConnectedLogo)
        {
            if (GalaxyNetwork.Connection.connected)
            {
                timeConnectedLogo -= Time.deltaTime*2;
                GUI.color = new Color(1, 1, 1, timeConnectedLogo / 9.0f);
            }
            else
            {
                GUI.color = new Color(0.2f, 0.2f, 0.2f, 0.4f);
            }
            GUI.Label(new Rect(16, 16, 24, 24), textureToDisplay);
            GUI.Label(new Rect(34, 19, 100, 20), "alaxy");
            if (timeConnectedLogo <= 0) showConnectedLogo = false;
        }

        if (showPing)
        {
            GUI.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
            //float p = GalaxyClient.Connection.GetPingReal * 1000;
            float p = GalaxyNetwork.Connection.GetPing * 1000;
            GUIContent pingText = new GUIContent(String.Format("{0} ms ", p.ToString()));
            Vector2 s = GUI.skin.label.CalcSize(pingText);
            GUI.Label(new Rect(Screen.width - s.x, Screen.height - s.y, s.x, s.y), pingText);
        }
    }
#endif

        void OnApplicationQuit()
        {
            if (GalaxyNetwork.Connection.connected)
                GalaxyNetwork.Connection.Disconnect();
        }

    }
}