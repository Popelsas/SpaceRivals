using GalaxyCommon;
using GalaxyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
//using UnityEditor;
using UnityEngine;


namespace GalaxyLib
{
    [HelpURL("http://wiki.greatgalaxy.ru/index.php?title=GalaxyServer:GalaxyNetID")]
    public class GalaxyNetID : MonoBehaviour    //MonoBehaviour
    {
        //        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

        public static GalaxyNetID NetId;

        //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

        //[ContextMenuItem("Описание", "Remarks")]
        //[Multiline(8)]
        //public string playerBiography = "";
        //void ResetBiography()
        //{
        //    playerBiography = "";
        //}
        //[ContextMenu("менюха")]
        //void DoSomething()
        //{
        //    Debug.Log("интерполяция");
        //}

        [Header("Условие создания объекта")]
        [Tooltip("Внимание: Если указать onlyHost, то объект может по умолчанию присутствовать на сцене и хозяин его будет - хост")]
        public CreationCondition creationCondition = CreationCondition.anyClient;

//#if UNITY_EDITOR
//        [G_HelpBox("'CreationCondition' определяет режим начального появления объекта.\n\nУказав OnlyHost - объект должен быть заранее помещен на сцену.", UnityEditor.MessageType.None, null, null, false, G_PropertyDrawerUtility.Space.Nothing)]
//        public float f_creationCondition;
//#endif

        /// <summary>
        /// Параметры синхронизации
        /// </summary>
        [Header("Параметры синхронизации")]
        [Tooltip("Какие параметры необходимо синхронизировать у этого объекта на всех клиентах")]
        public SyncType syncType = SyncType.PositionAndRotation;
        SyncType _syncType
        {
            get
            {
                return syncType;
            }
            set
            {
                syncType = value;
            }
        }

        /// <summary>
        /// Способ доставки
        /// </summary>
        [Tooltip("Способ доставки пакетов синхронизации (скорость или гарантия)")]
        public SyncDelivery syncDelivery = SyncDelivery.DeliverySpeed;

        /// <summary>
        /// Параметры интерполяции
        /// </summary>
        [Tooltip("(бета версия) Включение сглаживания перемещения")]
        public InterpolationType interpolationPosition = InterpolationType.none;
        InterpolationType _interpolationPosition = InterpolationType.none;

        [Tooltip("(бета версия) Скорость сглаживания. Если =0, то будет задействован устаревший метод")]
        public float speedInterpolationPosition = 0.0f;

        float inter_timer_position_start = 0;
        float inter_timer_position;

        [HideInInspector]
        public Vector3 newPosition;
        [HideInInspector]
        public Vector3 oldPosition;
        [HideInInspector]
        public float interPosFrameOne = 0;
        float interPosFrameOne2 = 0;
        [HideInInspector]
        public float interPosFrameAll = 0;

        [Tooltip("(бета версия) Включение сглаживания вращения")]
        public InterpolationType interpolationRotation = InterpolationType.none;
//!        InterpolationType _interpolationRotation = InterpolationType.none;

        float inter_timer_rotation_start = 0;
        float inter_timer_rotation;
        [HideInInspector]
        public Quaternion newRotation;
        [HideInInspector]
        public Quaternion oldRotation;
        [HideInInspector]
        public float interRotFrameOne = 0;
        [HideInInspector]
        public float interRotFrameAll = 0;

        //[Header("Не интерполировать, невидимый объект")]
        //public bool offInterpolationForInvisible;
        //bool _offInterpolationForInvisible;

        Quaternion rotation_cash;
        Vector3 position_cash;
        //2bool rotation_bool = false;
        //2bool position_bool = false;

        //2float periodTimeRate = 0;
        bool go_visible = true;

        //    System.Diagnostics.Stopwatch swPosition = new System.Diagnostics.Stopwatch();

        [Range(1, 30)]
        public int sendInterval = 6;
        int _sendInterval = 0;

        [Header("Порог срабатывания перемещения (юниты)")]
        [Tooltip("Позиция будет передана только если объект переместиться на данное расстояние или более")]
        public float thresholdPosition = 0.025f;
        Vector3 oldSendedPosition;
        //float oldThresholdPositionX;

        [Header("Порог срабатывания вращения (град.)")]
        [Tooltip("Поворот будет передана только если объект повернется на данное расстояние или более")]
        public float thresholdRotation = 1.5f;
        Quaternion oldSendedRotation;


#if UNITY_EDITOR
        [G_HelpBox("'CreationCondition' (бета версия) определяет режим начального появления объекта. Указав OnlyHost - объект должен быть заранее помещен на сцену.\n\n" +
            "'SpeedInterpolationPosition' (бета версия) Скорость сглаживания. Если '0', то будет задействован устаревший метод интерполяции.\n\n" +
            "'ThresholdPosition' Позиция будет передана только если объект переместиться на данное расстояние или более. \n\n" +
            "'ThresholdRotation' Поворот будет передана только если объект повернется на данное расстояние или более.", UnityEditor.MessageType.None, null, null, false, G_PropertyDrawerUtility.Space.Nothing)]
        public float f_speedInterpolationPosition;
#endif


        /// <summary>
        /// Уникальный идентификатор (в пределах комнаты)
        /// </summary>
        public ushort id
        {
            get
            {
                if (netObject != null)
                {
                    return (netObject.id);
                }
                //Debug.LogError("Объект не успел получить id");
                return 0;
            }
            //private set {}
        }

        #region (бета) Аниматор
        /// <summary>
        /// (бета) Аниматор
        /// </summary>
/*2
        public GalaxyAnimator galaxyAnimator   //public
        {
            get
            {
                return _galaxyAnimator;
            }
            set
            {
                _galaxyAnimator = value;
                if (_galaxyAnimator != null)
                    animation_sync = true;
                else
                    animation_sync = false;
            }
        }
        private GalaxyAnimator _galaxyAnimator;
        */

//2        bool animation_sync = false;    //public

//2        int anim_curState = -1;       // Текущее проигрываемое состояние
//2        int anim_curState_old = -1;
//!        float anim_dur = 0;

//2        int anim_curLayer = -1;
//2        int anim_newState = -1;
//!        int anim_newLayer = 0;
//2        bool anim_state_bool = false;
        #endregion

        #region NetVar сетевые переменные
//!        Component[] user_components;
//        public Dictionary<Component, List<object>> user_syncVars = new Dictionary<Component, List<object>>();
//        public Dictionary<Component, List<FieldInfo>> user_syncVarsArray = new Dictionary<Component, List<FieldInfo>>();
            Dictionary<Component, List<FieldInfo>> new_NetVars = new Dictionary<Component, List<FieldInfo>>();
        #endregion

        #region (old) РПЦ
//        public Dictionary<string, MethodInfo> user_rpc_method = new Dictionary<string, MethodInfo>();
//        public Dictionary<string, Component> user_rpc_component = new Dictionary<string, Component>();
        //MethodInfo
        #endregion

        /// <summary>
        /// Локальный уникальный идентификатор (в пределах текущего клиента)
        /// </summary>
        public ushort local_id { get; private set; }

        /// <summary>
        /// Владелец объекта
        /// </summary>
        public uint owner_id
        {
            get
            {
                if (netObject != null)
                {
                    return (netObject.owner_id);
                }
                //Debug.LogError("Произошел слишком ранний вызов. Объект не успел получить данные от сервера.");
                return 0;
            }
            //private set {}
        }

        /// <summary>
        /// Этот объект мой
        /// </summary>
        public bool isMy
        {
            get
            {
                if (_isMy) return true;

                if (netObject != null)
                {
                    return (netObject.isMy);
                }

                //Debug.LogError("Произошел слишком ранний вызов. Объект не успел получить данные от сервера.");
                return false;
            }
            //private set { }
        }

        bool _isMy = false;

        /// <summary>
        /// Ссылка на объект в DLL
        /// </summary>
        public GGObject netObject
        {
            get
            {
                if (_netObject != null)
                {
                    return (_netObject);
                }
                //Debug.LogWarning("Произошел слишком ранний вызов. Объект не успел получить данные от сервера.");
                return null;
            }
            private set
            {
                _netObject = value;
                inited = true;
            }
        }
        private GGObject _netObject;



        //netObject.sendedDataCount
        /// <summary>
        /// Сколько есть непрочитанных сообщений для объекта
        /// </summary>
        /*
        public int sendedDataCount
        {
            get
            {
                //if (_isMy) return true;
                if (netObject != null)
                {
                    return netObject.sendedDataCount;
                }
                return 0;
            }
            private set { }
        }
        */

        /// <summary>
        /// Прочитать сообщение, присланное этому объекту
        /// </summary>
        /*
        public object sendedDataRead
        {
            get
            {
                //if (_isMy) return true;
                if (netObject != null)
                {
                    return netObject.sendedDataRead;
                }
                return null;
            }
            private set { }
        }
        */


        [HideInInspector]
        public bool started = false;
        [HideInInspector]
        public bool inited = false;

        /*
        void OnValidate()
        {
            if (_offInterpolationForInvisible != offInterpolationForInvisible)
            {
                if (offInterpolationForInvisible)
                {
                    var col = gameObject.transform.GetComponents<MeshRenderer>();
                    if (col.Length != 0)
                    {
                        _offInterpolationForInvisible = offInterpolationForInvisible;
                    }
                    else
                    {
                        offInterpolationForInvisible = true;

                        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
                        mr.receiveShadows = false;
                        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                        mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

                        Debug.LogWarning("Для этой опции добавлен компонент <MeshRenderer>");
                    }
                }
                else
                {
                    _offInterpolationForInvisible = offInterpolationForInvisible;
                }
            }
            
            if (_sendInterval != sendInterval)
            {
                _sendInterval = sendInterval;

                if (IsInvoking("SyncSend"))
                {
                    CancelInvoke("SyncSend");
                    periodTimeRate = 1 / (float)_sendInterval;
                    InvokeRepeating("SyncSend", 0, periodTimeRate);
                }
            }

            if (_syncType != syncType)
            {
                _syncType = syncType;
                if (syncType == SyncType.none)
                {
                    if (IsInvoking("SyncSend")) CancelInvoke("SyncSend");
                }
                else
                {
                    if (IsInvoking("SyncSend")) CancelInvoke("SyncSend");
                    periodTimeRate = 1 / (float)_sendInterval;
                    InvokeRepeating("SyncSend", 0, periodTimeRate);
                }
            }
        }
        */

        // Запись сетевых переменных [NetVar] и методов [NetRPC] в словари
        void InitNetVarAndRpc()
        {
            #region Подготовка NetVar
//3            List<Component> tmp_components = new List<Component>();
            foreach (Component com in gameObject.GetComponents<Component>())
            {
                List<MemberInfo> options = new List<MemberInfo>();
                options = com.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(n => Attribute.IsDefined(n, typeof(NetVar))).ToList();


                List<MemberInfo> options_rpc = new List<MemberInfo>();
                options_rpc = com.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(n => Attribute.IsDefined(n, typeof(NetRPC))).ToList();

                List<object> fi = new List<object>();
                List<FieldInfo> fi_inf = new List<FieldInfo>();
                foreach (var item in options)
                {
                    FieldInfo fiin = item as FieldInfo;
                    //Debug.Log(fiin.GetValue(com));
                    fi.Add(fiin.GetValue(com));
                    fi_inf.Add(fiin);
                }

                // NetVar
                if (options.Count > 0)
                {
                    netObject.user_syncVars.Add(com, fi);   //5 netObject.user_syncVars.Add(com, fi);
                    netObject.user_syncVarsArray.Add(com, fi_inf); //5 netObject.user_syncVarsArray.Add(com, fi_inf);
//3                    tmp_components.Add(com);
                }

                // NetRPC
                if (options_rpc.Count > 0)
                {
                    foreach (var rpc in options_rpc)
                    {
                        //var type = typeof(myTestRPC);
                        var type = com.GetType();
                        MethodInfo mi = type.GetMethod(rpc.Name);
                        //mi.Invoke(com, null);
                        netObject.user_rpc_method.Add(rpc.Name, mi);
                        netObject.user_rpc_component.Add(rpc.Name, com);
                    }
                }

            }

            #endregion
        }

        void Awake()
        {
            #region Условие создания объекта
            if (GalaxyNetwork.Room.owner_client_id != GalaxyNetwork.Connection.clientId)    // client_host_id
            {
                if (creationCondition == CreationCondition.onlyHost)
                {
                    Destroy(gameObject);
                    return;
                }
            }
            #endregion

            //DontDestroyOnLoad(this);

            _sendInterval = sendInterval;
            NetId = this;

            // Присваиваем новый локальный id
            local_id = GalaxyNetwork.NetObjects.GetNewLocalId;

            GalaxyEvents.netObjectsLocal.Add(local_id, this);   // Добавляем объект в локальный список
            //2periodTimeRate = 1 / (float)_sendInterval;

            oldPosition = transform.position;
            newPosition = transform.position;
            inter_timer_position_start = Time.time;
            inter_timer_position = Time.deltaTime;
            interPosFrameAll = 999;   //periodTimeRate                    //999

            newRotation = transform.rotation;
            oldRotation = transform.rotation;
            inter_timer_rotation_start = Time.time;
            inter_timer_rotation = Time.deltaTime;
            interRotFrameAll = 999;                       //999

            // ===============================================================================

        }

        /*
        void OnBecameVisible()
        {
            if (interpolationPosition == InterpolationType.Linear)
            {
                receivedPosition(transform.position);
            }

            if (interpolationRotation == InterpolationType.Linear)
            {
                receivedRotation(transform.rotation);
            }

            go_visible = true;
        }
        */

        /*
        void OnBecameInvisible()
        {
            if (offInterpolationForInvisible)
                go_visible = false;
        }
        */

        void Start()
        {
            if (!started)
            {
                if (!GalaxyNetwork.NetObjects.Exist(id))
                {
                    // Отправляем запрос на создание нового сетевого объета
                    string myPathGO = gameObject.name;
                    myPathGO = myPathGO.Split('(')[0];
                    myPathGO = myPathGO.TrimEnd(' ');

                    GalaxyNetwork.SendOperation.NetObject.Instantiate(new GGObject(id, local_id, GalaxyNetwork.Connection.clientId, myPathGO, gameObject.transform.position, gameObject.transform.rotation));
                    //return;
                }

                StartSync();
                started = true;
            }
        }

        public void StartSync()
        {
            if (syncType != SyncType.none)
            {
                //if (isMy)
                //{
                float t = 1 / (float)_sendInterval;
                //InvokeRepeating("SyncSend", t, t);
                InvokeRepeating("SyncSend", 0, t);
                //}
            }
        }

        public void Init(GGObject netObject)
        {
            this.netObject = netObject;
            GalaxyEvents.netObjects.Add(netObject.id, this);
            InitNetVarAndRpc();
        }

        /// <summary>
        /// Отправить данные синхронизации (измененные) немедленно.
        /// Внимание! Повышает трафик.
        /// </summary>
        public void FlushSyncSend()
        {
            SyncSend();
            CancelInvoke("SyncSend");
            StartSync();
        }

        // Вызывается sendInterval раз в секунду
        // Отправить объект на синхронизацию
        void SyncSend()
        {
            if (netObject == null)
            {
                //Debug.LogWarning("Объект не инициализирован");
                return;
            }

            if (!isMy)
            {
                CancelInvoke("SyncSend");
                return;
            }

            #region (в разработке)Анимации
            /*
            if (animation_sync)
            {
                int layer = 0;

                if (_galaxyAnimator.animator.isActiveAndEnabled && _galaxyAnimator.animator.runtimeAnimatorController != null)
                {
                    //animator.GetAnimatorTransitionInfo(layer).
                    anim_curState = _galaxyAnimator.animator.GetCurrentAnimatorStateInfo(layer).shortNameHash;
                    if (anim_curState != anim_curState_old)
                    {
                        anim_curState_old = anim_curState;
//!                        anim_dur = _galaxyAnimator.animator.GetCurrentAnimatorStateInfo(layer).length;// normalizedTime
                                                                                                      //norm = animator.GetCurrentAnimatorStateInfo(layer).normalizedTime;
                        //_galaxyAnimator.animator.GetAnimatorTransitionInfo(0).
                        GalaxyNetwork.SendOperation.NetObject.AnimatorState(netObject, anim_curState, layer);

                        // Управляем аниматором сетевого клиента
                        //clientAnimator.Play(curState, layer);
                        // проигрываем                    sendAnimator.CrossFade(anim_CurState, dur, layer);
                    }
                }
                //_animator.readAnimator.GetAnimatorTransitionInfo(0).
            }
            */
            #endregion

            #region Позиция
            if (syncType == SyncType.Position)
            {
                if (Vector3.Distance(oldSendedPosition, transform.position) > thresholdPosition)
                {
                    oldSendedPosition = transform.position;
                    GalaxyNetwork.SendOperation.NetObject.Position(netObject, transform.position, syncDelivery);
                }
                //return;
            }
            #endregion

            #region Поворот
            if (syncType == SyncType.Rotation)
            {
                if (Vector3.Distance(oldSendedRotation.eulerAngles, transform.rotation.eulerAngles) > thresholdRotation)
                {
                    oldSendedRotation = transform.rotation;
                    GalaxyNetwork.SendOperation.NetObject.Rotation(netObject, transform.rotation, syncDelivery);
                }
                //return;
            }
            #endregion

            #region Позиция и Поворот
            if (syncType == SyncType.PositionAndRotation)
            {
                bool c_pos = false;
                bool c_rot = false;
                if (Vector3.Distance(oldSendedPosition, transform.position) > thresholdPosition)
                {
                    c_pos = true;
                    oldSendedPosition = transform.position;
                }

                float old_x = Math.Abs(oldSendedRotation.eulerAngles.x);
                float old_y = Math.Abs(oldSendedRotation.eulerAngles.y);
                float old_z = Math.Abs(oldSendedRotation.eulerAngles.z);
                float new_x = Math.Abs(transform.rotation.eulerAngles.x);
                float new_y = Math.Abs(transform.rotation.eulerAngles.y);
                float new_z = Math.Abs(transform.rotation.eulerAngles.z);
                float dx = Math.Abs(old_x - new_x);
                float dy = Math.Abs(old_y - new_y);
                float dz = Math.Abs(old_z - new_z);

                //if (Vector3.Distance(oldSendedRotation.eulerAngles, transform.rotation.eulerAngles) > snapThresholdRotation)
                if (dx > thresholdRotation || dy > thresholdRotation || dz > thresholdRotation)
                {
                    c_rot = true;
                    oldSendedRotation = transform.rotation;
                }

                if (c_pos && c_rot)
                {
                    GalaxyNetwork.SendOperation.NetObject.PositionAndRotation(netObject, transform.position, transform.rotation, syncDelivery);
                    c_pos = false; c_rot = false;
                    //return;
                }

                if (c_pos)
                {
                    GalaxyNetwork.SendOperation.NetObject.Position(netObject, transform.position, syncDelivery);
                    //return;
                }

                if (c_rot)
                {
                    GalaxyNetwork.SendOperation.NetObject.Rotation(netObject, transform.rotation, syncDelivery);
                    //return;
                }

                //return;
            }
            #endregion

            #region Синхронизируемые переменные
            //Dictionary<Component, List<FieldInfo>> new_NetVars = new Dictionary<Component, List<FieldInfo>>();
            new_NetVars.Clear();

            // Получаем все сетевые переменные
            foreach (Component com in netObject.user_syncVars.Keys)
            {
                List<MemberInfo> options = new List<MemberInfo>();
                options = com.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(n => Attribute.IsDefined(n, typeof(NetVar))).ToList();

                List<FieldInfo> fi = new List<FieldInfo>();
                foreach (var item in options)
                {
                    fi.Add(item as FieldInfo);
                }
                new_NetVars.Add(com, fi);
            }

            // Проверяем изменения
            netObject.NetVarClear();

            bool changed = false;
            byte com_num = 0;
            foreach (var comp in new_NetVars)
            {
                byte var_num = 0;
                foreach (var pole in comp.Value)
                {
                    object val = pole.GetValue(comp.Key);
                    if (!netObject.user_syncVars.ElementAt(com_num).Value.ElementAt(var_num).Equals(val))
                    {
                        netObject.user_syncVars[comp.Key][var_num] = val;
                        netObject.NetVarAddChanged(com_num, var_num, val);

                        changed = true;
                        //Debug.Log("Send: " + (byte)com_num + ", " + (byte)var_num + " = " + val.ToString());
                    }
                    var_num++;
                }
                com_num++;
            }

            if (changed)
                GalaxyNetwork.SendOperation.NetObject.NetVar(netObject, SyncDelivery.DeliveryGuaranty);
            #endregion
        }


        // Пришли новые переменные, которые загружены в netObject.GetNetVarsList()
        public void receivedNetVars()
        {
            if (isMy) return;
            List<NetVarStruct> nerVarList = netObject.GetNetVarsListCopy();
            foreach (var item in nerVarList)
            {
                Component com = netObject.user_syncVarsArray.ElementAt(item.num_class).Key;
                FieldInfo fi = netObject.user_syncVarsArray.ElementAt(item.num_class).Value.ElementAt(item.num_var);
                fi.SetValue(com, item.data);
            }
        }


        void OnDestroy()
        {
            if (GalaxyEvents.netObjectsLocal.ContainsKey(local_id))
            {
                GalaxyEvents.netObjectsLocal.Remove(local_id);
            }

            // Добавляем в список на удаление
            if (id == 0)
            {
                GalaxyEvents.netObjectsDeleted.Add(local_id);
                if (IsInvoking("CheckRemoveLocal"))
                {
                    CancelInvoke("CheckRemoveLocal");
                    Invoke("CheckRemoveLocal", 30);
                }
            }
            else
            {
                if (!GalaxyNetwork.NetObjects.Remove(id))
                    Debug.LogWarning("Внимание. Объект не удалился из списка.");

                // Отправка запроса на удаление
                GalaxyNetwork.SendOperation.NetObject.Destroy(id);
            }

        }

        // Чистка списка удаленных объектов
        void CheckRemoveLocal()
        {
            GalaxyEvents.netObjectsDeleted.Clear();
        }

        /// <summary>
        /// Пришла новая позиция
        /// </summary>
        /// <param name="position"></param>
        public void receivedPositionFixed(Vector3 position)
        {
            //2position_bool = false;

            // ИНТОРПОЛЯЦИЯ ОТКЛЮЧЕНА. ПЕРЕМЕЩАЕМ СРАЗУ
            if (interpolationPosition == InterpolationType.none || !go_visible) // || !go_visible или невидим объект
            {
                gameObject.transform.position = position;
                return;
            }

            // // ИНТОРПОЛЯЦИЯ ВКЛЮЧЕНА. СГЛАЖИВАЕМ
            if (interpolationPosition == InterpolationType.Linear)
            {
                float newTime = Time.time;
                inter_timer_position = newTime - inter_timer_position_start + 0.000000f; // считаем временную разницу между скоростью отправки координат 
                
                interPosFrameOne = 1.0f / inter_timer_position;

                if(interPosFrameOne2 != 0)
                {
                    float n_interPosFrameOne = (interPosFrameOne + interPosFrameOne2) / 2;
                    interPosFrameOne2 = interPosFrameOne;
                    interPosFrameOne = n_interPosFrameOne;
                }

                interPosFrameAll = 0;
                inter_timer_position_start = newTime;
                oldPosition = transform.position;
                newPosition = position;

                return;
            }
        }

        /// <summary>
        /// Пришел новый поворот
        /// </summary>
        /// <param name="rotation"></param>
        public void receivedRotationFixed(Quaternion rotation)
        {
            //2rotation_bool = false;

            if (interpolationRotation == InterpolationType.none || !go_visible) // || !go_visible или невидим объект
            {
                gameObject.transform.rotation = rotation;
                return;
            }

            // Если включена интерполяция
            if (interpolationRotation == InterpolationType.Linear)
            {
                inter_timer_rotation = Time.time - inter_timer_rotation_start; // считаем временную разницу между скоростью отправки координат 

                interRotFrameOne = 1 / inter_timer_rotation;
                
                interRotFrameAll = 0;
                inter_timer_rotation_start = Time.time;
                oldRotation = transform.rotation;
                newRotation = rotation;
                return;
            }
        }

        /// <summary>
        /// Пришло новое состояние аниматора
        /// </summary>
/*
        public void receivedAnimatorStateFixed()    //int state, int layer
        {
            anim_state_bool = false;
            // Управляем аниматором сетевого клиента
            if (!isMy)
            {
                if (anim_newState != anim_curState)
                {
//2                    galaxyAnimator.animator.Play(anim_newState, anim_curLayer);
                    anim_curState = anim_newState;
                    anim_state_bool = true;
                    // проигрываем                    sendAnimator.CrossFade(anim_CurState, dur, layer);
                }
            }
        }
*/

        // Пришла новая позиция из DLL
        public void receivedPosition(Vector3 position)
        {
            //if (Vector3.Distance(position_cash, position) > snapThresholdPosition)
            //{
            //        inter_timer_position_start = Time.time;
          //5  position_cash = position;
          //5  position_bool = true;
            if (!isMy) receivedPositionFixed(position);
            //}
        }

        // Пришел новый поворот из DLL
        public void receivedRotation(Quaternion rotation)
        {
            //if (Vector3.Distance(rotation.eulerAngles, rotation_cash.eulerAngles) > snapThresholdRotation)
            //{
            //        inter_timer_rotation_start = Time.time;
         //5   rotation_cash = rotation;
         //5   rotation_bool = true;

            if(!isMy) receivedRotationFixed(rotation);
            //}
        }

        /*
        public void receivedAnimatorState(int state, int layer)
        {
//2            anim_newState = state;
//!            anim_newLayer = layer;
//2            anim_state_bool = true;
        }
        */

        /*5
        void FixedUpdateQ()
        {
            if (!isMy)
            {
                if (position_bool) receivedPositionFixed(position_cash);
                if (rotation_bool) receivedRotationFixed(rotation_cash);
                //if (anim_state_bool) receivedAnimatorStateFixed();
            }
        }
        */

        void Update()
        {
            
            //if (!started) return;
            if (!isMy && go_visible && started)
            {
                
                if (_interpolationPosition != interpolationPosition)
                {
                    //Vector3 position_current = transform.position;              // Текущая позиция
                    //Vector3 position_new = transform.position;                  // Новая позиция
                    newPosition = transform.position;
                    oldPosition = transform.position;
                    _interpolationPosition = interpolationPosition;
                }

                // Новая интерполяция
                if (interpolationPosition == InterpolationType.Linear)
                {
                    if(speedInterpolationPosition == 0)
                    {
                        if(interPosFrameAll <= 1.0f)
                        {
                            interPosFrameAll += (interPosFrameOne * Time.deltaTime);
                            transform.position = Vector3.Lerp(oldPosition, newPosition, interPosFrameAll);
                        }
                        
                    }
                    else
                    {
                        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * speedInterpolationPosition);
                    }
                }


                // Старая интерполяция
                /*
                if (interpolationPosition == InterpolationType.Linear && interPosFrameAll <= 10.0f) // && interPosFrameAll <= 1    //interpolate_timer_position > 0 //+ interPosFrameOne
                {
                    interPosFrameAll += (interPosFrameOne * Time.deltaTime);
                    transform.position = Vector3.Lerp(oldPosition, newPosition, interPosFrameAll);

                    //1transform.position = Vector3.Lerp(oldPosition, newPosition, Time.deltaTime);
                    //1oldPosition = transform.position;

                    //2transform.position = Vector3.Lerp(oldPosition, newPosition, Time.deltaTime);
                    //transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * 6.0f);
                    //2oldPosition = transform.position;

                }
                else
                {
                //    interPosFrameAll -= (interPosFrameOne * Time.deltaTime);   //interPosFrameOne
                    //transform.position = newPosition;
                //    oldPosition = newPosition;
                    //oldPosition = transform.position;
                    //Debug.Log("!!!!");
                }
                */

                if (interpolationRotation == InterpolationType.Linear && interRotFrameAll <= 1)
                {
                    interRotFrameAll += (interRotFrameOne * Time.deltaTime);
                    transform.rotation = Quaternion.Lerp(oldRotation, newRotation, interRotFrameAll);
                }

            }

            //if(sendedDataCount > 0)
            //{
            //    object data = sendedDataRead;
            //    Debug.Log("[" + name + "] " + data.ToString());
            //}
            //swm = sw.Elapsed.TotalMilliseconds;
            //Debug.Log(sw.Elapsed.TotalMilliseconds.ToString());
            //sw.Stop();
            
        }


        /// <summary>
        /// Вызов удаленной процедуры (RPC)
        /// </summary>
        /// <param name="rpc_name">Имя процедуры</param>
        /// <param name="data">Передаваемые параметры процедуры</param>
        public void CallRPC(string rpc_name, params object[] data)
        {
            if(netObject != null)
                netObject.CallRPC(rpc_name, data);
        }


        /// <summary>
        /// (в разработке) Отправить сообщение другому объекту
        /// </summary>
        /// <param name="netId"></param>
        /// <param name="data"></param>
        public void SendData(ushort netId, object data)
        {
            if (netObject == null) return;
            if (data == null) return;

            netObject.SendData(data, netId);

        }


        [ExecuteInEditMode]
        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "GalaxyNetID", false);
        }


        /// <summary>
        /// Условие создания объекта
        /// </summary>
        public enum CreationCondition
        {
            /// <summary>
            /// Любой клиент
            /// </summary>
            anyClient = 0,

            /// <summary>
            /// Только Хост
            /// </summary>
            onlyHost = 1,
        }


    }
}