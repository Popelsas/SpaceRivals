// ===============================
// © GGTeam GalaxyNetwork module
// Модуль: Авторизация
// Версия: 0.21
// ===============================

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GalaxyLib;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[HelpURL("http://wiki.greatgalaxy.ru/index.php?title=GalaxyServer:mod_login")]
public class mod_login : MonoBehaviour {

    // УКАЖИТЕ СЕРВЕР ДЛЯ ПОДКЛЮЧЕНИЯ
    public ServerLocation serverLocation = ServersList.Region.RUS.Moscow.server_moscow_1;
    //public ServerLocation serverLocation = ServersList.Local.LocalServer;

    [Header("Событие при успешной авторизации")]
    public UnityEvent OnLoginSucesseEvent;

    [Header("Объекты, отображаемые при подключении")]
    public GameObject[] showOnConnectGO;

    [Header("Объекты, скрываемые при отключении")]
    public GameObject[] hideOnConnectGO;

    [Header("Время ожидания коннекта")]
    public float connect_time_out = 8.0f;

    [Space(32)]

    [SerializeField]
    InputField InputLogin;
    [SerializeField]
    InputField InputPass;
    [SerializeField]
    Text TextErrorMess;
    [SerializeField]
    Button bntLogin;
    [SerializeField]
    Toggle togSavePass;

    //[Header("Окно Авторизации")]
    [SerializeField]
    GameObject FormLogin;

    //[Header("Окно регистрации")]
    [SerializeField]
    GameObject FormReg;

    [SerializeField]
    InputField InputRegLogin;
    [SerializeField]
    InputField InputRegPass;
    [SerializeField]
    InputField InputRegPass2;
    [SerializeField]
    InputField InputRegEmail;
    [SerializeField]
    Text TextRegErrorMess;
    [SerializeField]
    GameObject PanelRegErrorMess;
    [SerializeField]
    GameObject background;

    bool local_connect = false;
    bool connect_process = false;

    Animator anim;
    bool showed = false;

    void OnEnable()
    {
        GalaxyEvents.OnGalaxyConnect += OnGalaxyConnect;
        GalaxyEvents.OnGalaxyDisconnect += OnGalaxyDisconnect;
    }

    void OnDisable()
    {
        GalaxyEvents.OnGalaxyConnect -= OnGalaxyConnect;
        GalaxyEvents.OnGalaxyDisconnect -= OnGalaxyDisconnect;
    }

    void Awake()
    {
        background.SetActive(true);
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        if (!GalaxyNetwork.Connection.connected) Show(true);

        int l = PlayerPrefs.GetInt("save_pass", 0);
        if (l == 1)
        {
            togSavePass.isOn = true;
            string pass_str = PlayerPrefs.GetString("login_pass", "");
            InputPass.text = pass_str;
        }
        else
        {
            togSavePass.isOn = false;
        }

        string login_str = PlayerPrefs.GetString("login_name", "");
        InputLogin.text = login_str;


        InputPass.onEndEdit.AddListener(val =>
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                OnButtonLoginCheck();
        });

        InputPass.ActivateInputField();
    }

    public void ToggleLocal()
    {
        if (togSavePass.isOn)
        {
            PlayerPrefs.SetInt("save_pass", 1);
        }
        else
        {
            PlayerPrefs.SetInt("save_pass", 0);
        }
    }

    // Кнопка авторизации
    public void OnButtonLoginCheck()
    {
        ShowLoginError("");
        if (InputLogin.text.Length == 0 || InputPass.text.Length == 0)
        {
            TextErrorMess.text = "Не указан Логин или Пароль";
            return;
        }

        if (InputLogin.text.Length < 3 || InputPass.text.Length < 4)
        {
            TextErrorMess.text = "Логин или Пароль слишком короткие";
            return;
        }

//        SelectServer();
//        GalaxyNetwork.Config.app_key = GalaxyEvents.Instance.app_key;

        if (GalaxyNetwork.Connection.connected)
        {
            Debug.Log("Вы уже авторизованы");
        }
        else
        {
            bntLogin.interactable = false;
            connect_process = true;
            Invoke("OnConnectTimeOut", connect_time_out);

            GalaxyNetwork.Connection.Connect(InputLogin.text, InputPass.text, serverLocation);
        }

        PlayerPrefs.SetString("login_name", InputLogin.text);
        

        if (togSavePass.isOn)
        {
            PlayerPrefs.SetString("login_pass", InputPass.text);
            PlayerPrefs.SetInt("save_pass", 1);
        }
        else
        {
            PlayerPrefs.SetString("login_pass", "");
            PlayerPrefs.SetInt("save_pass", 0);
        }

    }


    void OnConnectTimeOut()
    {
        connect_process = false;
        bntLogin.interactable = true;
        ShowLoginError("Сервер не отвечает. Попробуйте через несколько секунд.");
    }

    void SelectServer()
    {
        if(local_connect)
        {
            GalaxyNetwork.Connection.SelectServer(ServersList.Local.LocalServer);
        }
        else
        {
            GalaxyNetwork.Connection.SelectServer(serverLocation);
        }
    }

    public void ShowLoginError(string mes)
    {
        TextErrorMess.text = mes;
    }

    public void Show(bool panel_visible)
    {
        if (showed != panel_visible)
        {
            showed = panel_visible;

            FormLogin.SetActive(panel_visible);

            if (!panel_visible)
                FormReg.SetActive(false);

            if (panel_visible)
            {
                anim.Play("mod_login_show");
            }
            else
            {
                anim.Play("mod_login_hide");
            }
        }
    }

    // Вызывается при подключении к серверу
    void OnGalaxyConnect(ErrorCode errorCode)
    {
        CancelInvoke("OnConnectTimeOut");
        bntLogin.interactable = true;
        connect_process = false;

        if (errorCode == ErrorCode.none)
        {
            if (OnLoginSucesseEvent != null) OnLoginSucesseEvent.Invoke();

            foreach (var item in showOnConnectGO)
            {
                if(item != null)
                    item.SetActive(true);
            }
            
            Show(false);
        }
        else
        {
            ShowLoginError(ErrorMessages.RU[errorCode]);
            //Debug.Log("!!! " + ErrorMessages.RU[errorCode]);
        }
    }

    // Вызывается при отключении от сервера
    void OnGalaxyDisconnect()
    {
        if (connect_process)
        {
            CancelInvoke("OnConnectTimeOut");
            ShowLoginError("Ошибка подключения или введен не верный логин/пароль");
        }
        else
        {
            //ShowLoginError("Сервер не отвечает или введен не верный логин/пароль.");
        }
        connect_process = false;
        
        bntLogin.interactable = true;
        foreach (var item in hideOnConnectGO)
        {
            if (item != null)
                item.SetActive(false);
        }

        Show(true);
        //ModRooms.Instance.Show(false);
    }

    // Кнопка открытия окна регистрации
    public void OnButtonRegShow()
    {
        FormReg.SetActive(true);
    }

    // Кнопка закрытия окна регистрации
    public void OnButtonRegHide()
    {
        FormReg.SetActive(false);
    }

    // Кнопка РЕГИСТРАЦИЯ
    public void OnButtonRegistration()
    {
        if (InputRegLogin.text.Length < 6)
        {
            TextRegErrorMess.text = "Укажите эл.почту";
            ShowRegError();
            return;
        }

        if (InputRegPass.text.Length < 4)
        {
            TextRegErrorMess.text = "Пароль слишком короткий";
            ShowRegError();
            return;
        }

        if (InputRegPass2.text.Length == 0)
        {
            TextRegErrorMess.text = "Повторите пароль еще раз";
            ShowRegError();
            return;
        }

        if (InputRegPass.text != InputRegPass2.text)
        {
            TextRegErrorMess.text = "Пароли не совпадают";
            ShowRegError();
            return;
        }

        Registration(InputRegLogin.text, InputRegPass.text);
    }

    void ShowRegError()
    {
        PanelRegErrorMess.SetActive(true);
    }

    void OnRegistration(byte errorCode)
    {
        TextRegErrorMess.text = "errorCode: " + errorCode;
        ShowRegError();

        if (errorCode == 0)
        {
            TextRegErrorMess.text = "Регистрация прошла успешно";
            ShowRegError();
            InputLogin.text = InputRegLogin.text;
            InputPass.text = InputRegPass.text;
            //if (!GalaxyClient.Connection.connected)
            //    GalaxyClient.Connection.Connect(PlayerPrefs.GetString("login"), PlayerPrefs.GetString("paswods"));
        }

        if (errorCode == 2)
        {
            TextRegErrorMess.text = "Не указан емейл";
            ShowRegError();
        }

        if (errorCode == 3)
        {
            TextRegErrorMess.text = "Не указан пароль";
            ShowRegError();
        }

        if (errorCode == 4)
        {
            TextRegErrorMess.text = "Такой емейл уже зарегистрирован";
            ShowRegError();
        }

        if (errorCode == 5)
        {
            TextRegErrorMess.text = "Ошибка. Сообщите разработчику";
            //TextRegErrorMess.text = "Не указан APP_KEY";
            ShowRegError();
        }

        if (errorCode == 6)
        {
            TextRegErrorMess.text = "Обновите приложение";
            //TextRegErrorMess.text = "APP_KEY не верен";
            ShowRegError();
        }

        if (errorCode == 9)
        {
            TextRegErrorMess.text = "Неизвестная ошибка";
            ShowRegError();
        }
    }

    private void Registration(string login, string password, Dictionary<string, string> arr = null)
    {
        // Выбираем сервер, на котором регистрируемся
        SelectServer();

        string url = "http://" + GalaxyNetwork.Connection.selected_host_web + "reg";

        Debug.Log(url);

        //string url = "http://" + GalaxyNetwork.Connection.selected_host_web + "galaxy-reg.php";
        WWWForm form = new WWWForm();
        form.AddField("email", login);
        form.AddField("password", GetMD5(password));
        form.AddField("app_key", GalaxyEvents.Instance.app_key);
        if (arr != null)
        {
            foreach (KeyValuePair<string, string> item in arr) form.AddField(item.Key, item.Value);
        }
        WWW www = new WWW(url, form);
        StartCoroutine(WaitForRequest(www));
    }

    private IEnumerator WaitForRequest(WWW www)
    {
        yield return www;
        if (www.error == null)
        {
            if (www.text.Length > 0)
            {
                switch (www.text[1])
                {
                    case '0': OnRegistration(0); break;
                    case '1': OnRegistration(1); break;
                    case '2': OnRegistration(2); break;
                    case '3': OnRegistration(3); break;
                    case '4': OnRegistration(4); break;

                    case '5': OnRegistration(5); break;
                    case '6': OnRegistration(6); break;

                    case '9': OnRegistration(9); break;

                    default:
                        {
                            TextRegErrorMess.text = "Error: " + www.text[1];
                            ShowRegError();
                        }
                        break;
                }
            }
            else
                OnRegistration(9);
        }
        else
        {
            TextRegErrorMess.text = "Error: " + www.error;
            ShowRegError();
        }
    }

    private string GetMD5(string input)
    {
        MD5 md5Hasher = MD5.Create();
        byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
        StringBuilder sBuilder = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }
        return sBuilder.ToString();
    }

}
