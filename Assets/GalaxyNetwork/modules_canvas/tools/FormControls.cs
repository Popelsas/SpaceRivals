using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GalaxyLib;


public class FormControls : MonoBehaviour {

    [Header("Окно: Авторизации")]
    public mod_login formLogin;

    [Header("Окно: Выбора персонажа")]
    public FormMyPlayers formPlayerSelect;

    [Header("Окно: Характеристик персонажа")]
    public FormMyPlayerInfo formPlayerInfo;

    [Header("Окно: Список комнат")]
    public mod_rooms formRoomList;

    [Header("Окно: Чат")]
    public mod_chat formChat;


    // ПОДПИСКИ
    // Авто подписка
    void OnEnable()
    {
        GalaxyEvents.OnGalaxyConnect += OnGalaxyConnect;
        GalaxyEvents.OnGalaxyDisconnect += OnGalaxyDisconnect;
    }

    // Авто отписка
    void OnDisable()
    {
        GalaxyEvents.OnGalaxyConnect -= OnGalaxyConnect;
        GalaxyEvents.OnGalaxyDisconnect -= OnGalaxyDisconnect;
    }


    // Use this for initialization
    void Start () {
        formLogin.gameObject.SetActive(true);
        formPlayerSelect.gameObject.SetActive(false);
    }
	





    // ====== RPC ======

    // [RPC] успешное подключение
    public void OnGalaxyConnect(ErrorCode errorCode)
    {
/*
        if (errorCode == ErrorCode.none)
        {
            formLogin.gameObject.SetActive(false);
            formPlayerSelect.gameObject.SetActive(true);

            formRoomList.Show(true);
            formChat.ShowChat(true);
        }
        else
        {
            formLogin.gameObject.SetActive(true);
            formPlayerSelect.gameObject.SetActive(false);
            formRoomList.Show(false);
            formChat.ShowChat(false);
        }
 */       


        //GalaxyClient.SendOperation.Room.GetRoomsList();
        //GalaxyClient.SendOperation.Room.EnterToNewRoom(new GGRoom("My First room", 4, true));

    }


    // [RPC] ошибка подключения
//    public void OnGalaxyConnectError(ErrorCode errorCode)
//    {
//        formLogin.gameObject.SetActive(true);
//        formLogin.SetErrorMes("[" + (byte)errorCode + "] " + ErrorMessages.RU[errorCode] + "\r\n");
//    }


    // [RPC] отключение
    public void OnGalaxyDisconnect()
    {
        formLogin.gameObject.SetActive(true);
        formPlayerSelect.gameObject.SetActive(false);
        formPlayerInfo.gameObject.SetActive(false);
        formRoomList.Show(false);
        formChat.ShowChat(false);

        formLogin.ShowLoginError("Отключились");
    }



}
