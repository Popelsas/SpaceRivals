using UnityEngine;
using System.Collections;
using GalaxyLib;
using System;
using UnityEngine.SceneManagement;

namespace GalaxyLib
{
    [ExecuteInEditMode()]
    [HelpURL("https://docs.google.com/document/d/1u43Jxeq4LfcfxcCSLvWMvHALlEjTz8oxh9adJRqdVN4/edit#heading=h.xmgsmoiaxwyc")]

    //[System.Serializable]
    public class GalaxySceneManager : MonoBehaviour
    {

        public static Log log = Log.GetLogger(typeof(GalaxySceneManager));

#if UNITY_EDITOR
        [Header("Сцена Авторизации")]
        public UnityEditor.SceneAsset scene_Login;

        [Header("Сцена Лобби")]
        public UnityEditor.SceneAsset scene_Lobby;

        [Header("Сцена Игры")]
        public UnityEditor.SceneAsset scene_Game;

        [Header("Сцена процесса загрузки (Ассинхронной)")]
        public UnityEditor.SceneAsset LoadingProcessScene;

#endif
        [Space]

        [Header("Ассинхронная загрузка сцены")]
        public bool accync_load = false;



        //[ContextMenuItem("Описание", "ResetBiography")]


        //[Multiline(8)]
        //public string playerBiography = "Внимание! \r\nВсе сцены, должны быть \r\nдобавлены в BuildSettings...";
        //void ResetBiography()
        //{
        //    playerBiography = "";
        //}


        public string scene_Login_name { get; set; }
        public string scene_Lobby_name { get; set; }
        public string scene_Game_name { get; private set; }
        public string scene_Loading_name { get; set; }

        //    Scene scene_Login_scene;
        //    Scene scene_Lobby_scene;
        //    Scene scene_Game_scene;

        //public List<string> scenes = new List<string>();

        //[Header("Порог срабатывания")]
        //public float snapThreshold = 0.001f;

        private static GalaxySceneManager _instance;
        public static GalaxySceneManager Instance
        {
            get { return _instance; }
        }

        void OnValidate()
        {
#if UNITY_EDITOR

            scene_Login_name = "";
            scene_Lobby_name = "";
            scene_Game_name = "";


            if (scene_Login != null)
                scene_Login_name = scene_Login.name;

            if (scene_Lobby != null)
                scene_Lobby_name = scene_Lobby.name;

            if (scene_Game != null)
                scene_Game_name = scene_Game.name;

            if (LoadingProcessScene != null)
                scene_Loading_name = LoadingProcessScene.name;

            if (accync_load)
                if (LoadingProcessScene == null)
                    Debug.LogWarning("<color=#106010ff>[GalaxySceneManager]</color> Сцена <color=#A01010ff>процесса загрузки</color> не указана");

#endif


        }



        private void OnEnable()
        {
            GalaxyEvents.OnGalaxyConnect += OnGalaxyConnect;
            GalaxyEvents.OnGalaxyDisconnect += OnGalaxyDisconnect;
            GalaxyEvents.OnGalaxyRoomEnter += OnGalaxyRoomEnter;
            GalaxyEvents.OnGalaxyRoomExit += OnGalaxyRoomExit;
            //GalaxyEvents.OnGalaxyRoomExit += OnGalaxyRoomExit;
        }

        private void OnDisable()
        {
            GalaxyEvents.OnGalaxyConnect -= OnGalaxyConnect;
            GalaxyEvents.OnGalaxyRoomEnter -= OnGalaxyRoomEnter;
            GalaxyEvents.OnGalaxyRoomExit -= OnGalaxyRoomExit;
            //GalaxyEvents.OnGalaxyRoomEnter -= OnGalaxyRoomExit;
        }


        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "GalaxyTransform.png", false);
        }


        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }


        // Use this for initialization
        void Start()
        {
            if (Application.isPlaying)
            {
                if (scene_Login_name == "" && scene_Lobby_name == "" && scene_Game_name == "")
                    Debug.LogWarning("<color=#106010ff>[SceneManager]</color> Не указана ни одна сцена.");
            }

        }



        void OnGalaxyConnect(ErrorCode errorCode)
        {
            if (errorCode == ErrorCode.none)
            {
                if (scene_Lobby_name != "")
                {
                    SceneManager.LoadScene(scene_Lobby_name);
                }
                //            ModRooms.Instance.Show();
            }
        }

        void OnGalaxyDisconnect()
        {
            if (scene_Login_name != "")
                SceneManager.LoadScene(scene_Login_name);
        }


        void OnGalaxyRoomEnter(UInt32 clientId, String nikname, ErrorCode errorCode)
        {
            if (errorCode == ErrorCode.none)
            {
                //if (clientId == GalaxyClient.Connection.clientId)
                //{
                if (scene_Game_name != "")
                {
                    if (accync_load && scene_Loading_name != "")
                    {
                        SceneManager.LoadScene(scene_Loading_name);
                        SceneManager.LoadSceneAsync(scene_Game_name, LoadSceneMode.Additive);
                    }
                    else
                    {
                        SceneManager.LoadScene(scene_Game_name);
                    }
                }
                //}
            }
        }

        void OnGalaxyRoomExit(UInt32 clientId, String nikname, ErrorCode errorCode)
        {
            if (errorCode == ErrorCode.none)
            {
                //if (clientId == GalaxyClient.Connection.clientId)
                //{
                if (scene_Lobby_name != "")
                {
                    SceneManager.LoadScene(scene_Lobby_name);
                }
                //}
            }
        }
    }
}
