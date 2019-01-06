/*
 * (c) GGTeam GalaxyClient
 * Консоль
 */

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using GalaxyLib;

namespace GalaxyLib
{
    [HelpURL("http://wiki.greatgalaxy.ru/index.php?title=GalaxyServer:GalaxyConsole")]
    public class GalaxyConsole : MonoBehaviour
    {
        //public static GalaxyConsole console;// = (GalaxyConsole)Behaviour.FindObjectOfType(typeof(GalaxyConsole));
        //static Galaxy.Unity.Log log = Galaxy.Unity.Log.GetLogger(typeof(GalaxyConsole));
        //public static Log log = Log.GetLogger(typeof(GalaxyConsole));
        //    public static LogAdapter logAd = new LogAdapter();
        public class LogAdapter : ILogAdapter
        {
            GalaxyConsole console = (GalaxyConsole)Behaviour.FindObjectOfType(typeof(GalaxyConsole));

            public void Log(Log.LogEvent @event)
            {
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
                else
                    Debug.Log("[" + @event.Log.Name + "] [" + logLevelText + "] " + @event.Message);
            }
        }

        [SerializeField]
        GalaxyCommon.LogLevel logLevel = GalaxyCommon.LogLevel.All;

        //[SerializeField]
        int maxLines = 150;

        [SerializeField]
        bool visibleConsole = false;

        [SerializeField]
        bool visibleStats = false;

        //[SerializeField]
        Font font;

        [SerializeField]
        int fontSize = 10;

        [SerializeField]
        float consoleHeight = 0.5f;

        [SerializeField]
        KeyCode toggleKey = KeyCode.Tab;

        //[SerializeField]
        bool autoFocus = true;

        //[Header("Симуляция плохого соединения")]
        //public bool lagSimulate = false;

        [Header("Эмуляция плохого соединения")]
        [Tooltip("Эмуляция пинга. (мс)")]
        [Range(0, 1000)]
        public int ping_simulate = 0;

        [Tooltip("Эмуляция потерь сообщений. (% в сек)")]
        [Range(0.0f, 100.0f)]
        public float loss_simulate = 0.0f;


        //[SerializeField]
        //Texture2D backgroundTexture;

        //[SerializeField]
        Color textColor = new Color32(164, 208, 195, 255);
        Color textColorError = new Color32(255, 100, 100, 255);
        //Color textColorError = new Color32(250, 160, 160, 255);

        int screenWidth;
        float totalHeight = 0f;
        float lineH = 0f;

        GUIStyle textStyle;
        GUIStyle inputStyle;
        GUIStyle boxStyle;

        GUIStyle boxStyleStat;
        GUIStyle textStyleStat;
        GUIStyle textStyleStatError;
        GUIStyle buttonStyleStat;

        string text = "";
        string input = "";

        int cntLineText = 0;

        float fps = 0;
        //float fps_old = 0;
        float showFps = 0;
        bool show_fps = true;

        Vector2 scrollPos = Vector2.zero;

        LinkedList<string> lines = new LinkedList<string>();
        StringBuilder buffer = new StringBuilder(1024 * 64);
        Dictionary<string, Action<string>> commands = new Dictionary<string, Action<string>>();

        private static GalaxyConsole _instance;
        public static GalaxyConsole Instance
        {
            get { return _instance; }
        }

        void Awake()
        {
            // Эмуляция качества связи
            GalaxyNetwork.Config.simulate_latency = ping_simulate;
            GalaxyNetwork.Config.simulate_loss = loss_simulate;

            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            _instance = this;


            // Инициализация log4net adapter
            Log.SetAdapter(new GalaxyConsole.LogAdapter());  //new GalaxyConsole.LogAdapter()

            font = Resources.Load<Font>("Fonts/CONSOLA");

            if (font == null)
                Debug.LogWarning("Не найден шрифт для консоли (Resources/Fonts/CONSOLA)");

            if (visibleConsole)
                WriteLine("<color=#106010ff>Нажмите </color><color=#A01010ff>[TAB]</color><color=#106010ff> для скрытия констоли.</color>");
            else
                Debug.Log("<color=#106010ff>Нажмите </color><color=#A01010ff>[TAB]</color><color=#106010ff> для открытия констоли.</color>");

            //Log.SetAdapter( new LogUnityAdapter());  // в юнити консоль
            //Log.SetLevel(GalaxyCommon.LogLevel.All);

            Log.SetLevel(logLevel);

            InitStyles();
            InvokeRepeating("ShowFPS", 1, 0.25f);
        }


        public void WriteLine(string text, params object[] args)
        {
            lines.AddLast(String.Format(text, args));

            if (lines.Count > maxLines || cntLineText > 9000) // || cntLineText > 60000
            {
                lines.RemoveFirst();
            }

            updateBuffer();
        }


        public void RegisterCommand(string command, Action<string> act)
        {
            commands.Add(command, act);
        }


        void updateBuffer()
        {
            //Start();
            buffer.Remove(0, buffer.Length);
            int i = 0;
            cntLineText = 0;
            foreach (string line in lines)
            {
                cntLineText += line.Length;
                buffer.AppendLine(line);
                i++;
            }
            text = buffer.ToString().Trim();

            //        screenWidth = Screen.width;
            totalHeight = lineH * i + 18;
            //        scrollPos = new Vector2(0, totalHeight);
            //        if (textStyle != null) totalHeight = textStyle.CalcHeight(new GUIContent(text), Screen.width - 25) + 8;
            screenWidth = Screen.width;
            scrollPos = new Vector2(0, totalHeight);
            //scrollPos = new Vector2(0, totalHeight);
            //  totalHeight = lineH * i; // textStyle.lineHeight * i;
        }

        void evalCommand(string command)
        {
            command = (command ?? "").Trim();

            if (command.Length > 0)
            {
                if (command[0] != '/')
                {
                    WriteLine("<color=#008000ff>Недопустимая команда</color> '{0}'", command);
                    return;
                }

                foreach (var item in commands)
                {
                    if (item.Key == command)
                        item.Value(item.Key);
                }

            }
        }


        void InitStyles()
        {
            if (textStyle == null)
            {
                consoleHeight = Mathf.Clamp01(consoleHeight);

                textStyle = new GUIStyle();
                textStyle.fontSize = fontSize;
                textStyle.font = font;
                textStyle.wordWrap = true;
                textStyle.normal.textColor = textColor;

                inputStyle = new GUIStyle();
                inputStyle.fontSize = textStyle.fontSize;
                inputStyle.font = textStyle.font;
                inputStyle.normal.textColor = textStyle.normal.textColor;
                lineH = textStyle.lineHeight + 0.0f;
            }


            if (boxStyle == null)
            {
                boxStyle = new GUIStyle("box");
                //if (backgroundTexture != null)
                //{
                boxStyle.padding = new RectOffset(0, 0, 0, 0);
                boxStyle.fixedHeight = 0;
                boxStyle.fixedWidth = 0;
                boxStyle.stretchHeight = false;
                boxStyle.stretchWidth = true;
                //    boxStyle.normal.background = backgroundTexture;
                //boxStyle.normal.background.Apply();
                //boxStyle.normal.background = backgroundTexture;
                //}
                boxStyle.richText = true;
                boxStyle.padding = new RectOffset(0, 0, 0, 0);
                boxStyle.fixedHeight = 0;
                boxStyle.fixedWidth = 0;
                boxStyle.stretchHeight = true;
                boxStyle.stretchWidth = true;
                Texture2D tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                tex.SetPixel(0, 0, new Color(0.05f, 0.07f, 0.07f, 0.6f));
                tex.Apply();
                boxStyle.normal.background = tex;
            }

            // Статистика
            boxStyleStat = new GUIStyle("box");
            boxStyleStat.richText = false;
            boxStyleStat.padding = new RectOffset(0, 0, 0, 0);
            boxStyleStat.fixedHeight = 0;
            boxStyleStat.fixedWidth = 0;
            boxStyleStat.stretchHeight = true;
            boxStyleStat.stretchWidth = true;
            Texture2D texStat = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            texStat.SetPixel(0, 0, new Color(0.05f, 0.07f, 0.07f, 0.2f));
            texStat.Apply();
            boxStyleStat.normal.background = texStat;

            textStyleStat = new GUIStyle();
            textStyleStat.fontSize = 10;
            textStyleStat.font = textStyle.font;
            textStyleStat.normal.textColor = textStyle.normal.textColor;

            textStyleStatError = new GUIStyle(textStyleStat);
            textStyleStatError.normal.textColor = textColorError;

            //======

            buttonStyleStat = new GUIStyle("button");
            buttonStyleStat.richText = false;
            buttonStyleStat.padding = new RectOffset(4, 4, 4, 4);
            buttonStyleStat.fixedHeight = 0;
            buttonStyleStat.fixedWidth = 0;
            buttonStyleStat.stretchHeight = true;
            buttonStyleStat.stretchWidth = true;

            buttonStyleStat.fontSize = 12;
            buttonStyleStat.font = textStyle.font;
            buttonStyleStat.normal.textColor = new Color32(164, 208, 195, 80); // new Color(0.3f, 0.3f, 0.35f, 1.0f);// textStyle.normal.textColor; new Color32(164, 208, 195, 255);

            Texture2D texBut = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            texBut.SetPixel(0, 0, new Color(0.07f, 0.07f, 0.15f, 0.2f));
            texBut.Apply();
            buttonStyleStat.normal.background = texBut;


        }

        [ExecuteInEditMode]
        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "GalaxyTerminal", false);
        }


        void OnGUI()
        {
            if (Event.current.Equals(Event.KeyboardEvent(toggleKey.ToString())))
            {
                visibleConsole = !visibleConsole;
            }

            if (screenWidth != Screen.width)
            {
                updateBuffer();
            }

            if (visibleConsole)
            {
                int height = (int)((float)Screen.height * consoleHeight);

                GUI.Box(new Rect(0, 0, Screen.width, height), "", boxStyle);

                scrollPos = GUI.BeginScrollView(new Rect(0, 0, Screen.width, height - 25), scrollPos, new Rect(0, 0, Screen.width - 25, totalHeight));
                GUI.TextArea(new Rect(5, 5, screenWidth - 5, totalHeight), text, textStyle);
                GUI.EndScrollView();

                if (Event.current.Equals(Event.KeyboardEvent("return")))
                {
                    if (input.Length > 0)
                    {
                        evalCommand(input);
                    }

                    input = "";
                }

                GUI.Label(new Rect(5, height - 20, 10, 25), ">", inputStyle);

                if (autoFocus)
                {
                    GUI.SetNextControlName("GalaxyConsoleInput");
                }

                input = GUI.TextField(new Rect(15, height - 20, screenWidth - 15, 25), input, inputStyle);

                if (autoFocus)
                {
                    GUI.FocusControl("GalaxyConsoleInput");
                }
            }

            // Отображаем статистику
            if (visibleStats)
            {
                GUI.Box(new Rect(Screen.width - 628, 4, 320, 64), "", boxStyleStat);
                string s_rec = GalaxyNetwork.Info.ReceivedBytes.ToString();
                if (GalaxyNetwork.Info.ReceivedBytes >= 1024) s_rec = (GalaxyNetwork.Info.ReceivedBytes / 1024).ToString() + "КБ";
                if (GalaxyNetwork.Info.ReceivedBytes >= 1048576) s_rec = (GalaxyNetwork.Info.ReceivedBytes / 1048576).ToString() + "МБ";

                string s_snd = GalaxyNetwork.Info.SentBytes.ToString();
                if (GalaxyNetwork.Info.SentBytes >= 1024) s_snd = (GalaxyNetwork.Info.SentBytes / 1024).ToString() + "КБ";
                if (GalaxyNetwork.Info.SentBytes >= 1048576) s_snd = (GalaxyNetwork.Info.SentBytes / 1048576).ToString() + "МБ";


                GUI.Label(new Rect(Screen.width - 620, 9, 300, 128), "   Принято (байт/сообщ./пак.) " + s_rec + " / " + GalaxyNetwork.Info.ReceivedMessages + " / " + GalaxyNetwork.Info.ReceivedPackets, textStyleStat);
                GUI.Label(new Rect(Screen.width - 620, 18, 300, 128), "Отправлено (байт/сообщ./пак.) " + s_snd + " / " + GalaxyNetwork.Info.SentMessages + " / " + GalaxyNetwork.Info.SentPackets, textStyleStat);

                if (ConnectionHeader.Info.ExcessSendRate)
                    GUI.Label(new Rect(Screen.width - 620, 27, 300, 128), " Внимание ПРЕВЫШЕН (байт/сек) " + ConnectionHeader.Info.ReadByteRate + " / " + ConnectionHeader.Info.SendByteRate, textStyleStatError);
                else
                    GUI.Label(new Rect(Screen.width - 620, 27, 300, 128), " Реальная Вход/исх (байт/сек) " + ConnectionHeader.Info.ReadByteRate + " / " + ConnectionHeader.Info.SendByteRate, textStyleStat);

                if (ConnectionHeader.Info.ExcessSendRateAverage)
                    GUI.Label(new Rect(Screen.width - 620, 36, 300, 128), " Превышен Вход/исх (байт/сек) " + ConnectionHeader.Info.ReadByteRateAverage + " / " + ConnectionHeader.Info.SendByteRateAverage, textStyleStatError);
                else
                    GUI.Label(new Rect(Screen.width - 620, 36, 300, 128), "  Средняя Вход/исх (байт/сек) " + ConnectionHeader.Info.ReadByteRateAverage + " / " + ConnectionHeader.Info.SendByteRateAverage, textStyleStat);

                if (GalaxyNetwork.Connection.connected)
                {
                    if (GalaxyNetwork.Room.id == 0)
                    {
                        // Лобби
                        //!GUI.Label(new Rect(Screen.width - 620, 45, 300, 128), "   Начальная комната [" + GalaxyNetwork.Room.roomId + "] " + GalaxyNetwork.Room.name + ", Игроков: " + GalaxyNetwork.Room.currentPlayersCount + "/" + GalaxyNetwork.Room.currentPlayersCount_for_lobby + " GalaxyNetwork.Players.Count: " + GalaxyNetwork.Players.Count, textStyleStat);  //maxCntPlayers
                        GUI.Label(new Rect(Screen.width - 620, 45, 300, 128), "   Начальная комната [" + GalaxyNetwork.Room.id + "] " + GalaxyNetwork.Room.name + ", Игроков: " + GalaxyNetwork.Room.clients_count + "/" + GalaxyNetwork.Room.clients_count, textStyleStat);  //maxCntPlayers       + " GalaxyNetwork.Players.Count: " + GalaxyNetwork.Players.Count
                    }
                    else
                    {
                        // Не лобби
                        GUI.Label(new Rect(Screen.width - 620, 45, 300, 128), "   Комната [" + GalaxyNetwork.Room.id + "] " + GalaxyNetwork.Room.name + ", Игроков: " + GalaxyNetwork.Room.clients_count + "/" + GalaxyNetwork.Room.clients_count_max, textStyleStat);  // + " GalaxyNetwork.Players.Count: " + GalaxyNetwork.Players.Count
                    }
                }
                else
                {
                    GUI.Label(new Rect(Screen.width - 620, 45, 300, 128), "   Подключение отсутствует", textStyleStat);
                }

                string playerName = "";
                string playerId = "";
                bool isMy = false;
                string isM = "    Клиент #";

                if (GalaxyNetwork.Players.MyPlayer != null)
                {
                    playerName = GalaxyNetwork.Players.MyPlayer.name;
                    playerId = GalaxyNetwork.Players.MyPlayer.clientId.ToString();
                    isMy = GalaxyNetwork.Players.MyPlayer.isMy;
                    if (isMy) isM = "         Я #";
                    if (GalaxyNetwork.Connection.clientId != 0)
                        GUI.Label(new Rect(Screen.width - 620, 54, 300, 128), isM + GalaxyNetwork.Connection.clientId + ", " + GalaxyNetwork.Connection.clientNikName + ", Игрок #" + playerId + ", " + playerName, textStyleStat);
                }
                else
                {
                    if (GalaxyNetwork.Connection.clientId != 0)
                        GUI.Label(new Rect(Screen.width - 620, 54, 300, 128), isM + GalaxyNetwork.Connection.clientId + ", " + GalaxyNetwork.Connection.clientNikName, textStyleStat);
                }

                if (GUI.Button(new Rect(Screen.width - 326, 52, 18, 15), "▲", buttonStyleStat)) visibleStats = false;    //☺☻♥♦♣♠•◘○◙♂♀♪♫☼►◄↕‼¶§▬↨↑↓→←∟↔▲▼ !

            }
            else
            {
                //fps = 1.0f / Time.deltaTime;

                GUI.Box(new Rect(Screen.width - 628, 4, 320, 16), "", boxStyleStat);
                //GUI.Label(new Rect(Screen.width - 620, 9, 300, 128), "   Принято (байт/сообщ./пак.) " + GalaxyClient.Info.ReceivedBytes + " / " + GalaxyClient.Info.ReceivedMessages + " / " + GalaxyClient.Info.ReceivedPackets, textStyleStat);

                GUI.Label(new Rect(Screen.width - 620, 9, 100, 20), "Краткая информация:", textStyleStat);

                float ping = GalaxyNetwork.Connection.GetPing * 1000;

                /*
                float showFps = 99;
                if (Math.Abs(fps - fps_old) < 2)
                {
                    showFps = fps_old;
                    //fps_old = fps;
                }
                else
                {
                    showFps = fps;
                    fps_old = fps;
                }
                */

                if (show_fps)
                {
                    show_fps = false;
                    showFps = fps;
                }
                //GUIContent pingText = new GUIContent(String.Format("пинг: {0} ms ", p.ToString()));

                GUI.Label(new Rect(Screen.width - 400, 9, 100, 20), String.Format("ping: {0} ms", ping.ToString()), textStyleStat);

                //GUIContent fpsText = new GUIContent(String.Format("FPS: {0}", ((int)fps).ToString()));
                GUI.Label(new Rect(Screen.width - 460, 9, 100, 20), String.Format("fps: {0},", ((int)showFps).ToString()), textStyleStat);

                if (GUI.Button(new Rect(Screen.width - 326, 4, 18, 15), "▼", buttonStyleStat)) visibleStats = true;
            }



        }

        //void Update()
        //{
        //    fps = 1.0f / Time.deltaTime;
        //}

        void ShowFPS()
        {
            fps = 1.0f / Time.deltaTime;
            show_fps = true;// !show_fps;
        }
    }
}