using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using GalaxyLib;

public class FormMyPlayers : MonoBehaviour {

    [Header("Панели добавления персонажей")]
    public GameObject panelPlayer1;
    public GameObject panelPlayer2;
    public GameObject panelPlayer3;

    [Header("Панели добавления персонажей")]
    public GameObject windowNewPlayer;

    [Header("Список новых персонажей")]
    public GameObject[] panelNewPlayers;


    [Header("Фон назначения имени")]
    public GameObject panelNamePlayer;

    [Header("Подсказка, если нет персонажей")]
    public GameObject hintNoPlayers;

    [Header("Место нового выбранного персонажа")]
    public Image addPlayerImg;

    [Header("Инпут ввода имени")]
    public InputField inpF;

    bool existPlayer1 = false;
    bool existPlayer2 = false;
    bool existPlayer3 = false;

    //uint idPlayer1 = 0;
    //uint idPlayer2 = 0;
    //uint idPlayer3 = 0;

    [Header("Спрайт добавления персонажа")]
    public Sprite addPlayerSprite;

    // Временные
//    ushort tAvaId;


    // ПОДПИСКИ
    // Авто подписка
    void OnEnable()
    {
//!!!!!!!!!!!!!!!!        //GalaxyClient.MyPlayer.MyPlayersGet();

//        GalaxyEvents.OnGalaxyMyPlayersList += OnGalaxyMyPlayersList;        // Сервер прислал список моих игроков
        //ClientMain.OnGalaxyGoToWorld += OnGalaxyGoToWorld;            // Пришла информация о твоем персонаже.
    }

    // Авто отписка
    void OnDisable()
    {
//        GalaxyEvents.OnGalaxyMyPlayersList -= OnGalaxyMyPlayersList;
        //ClientMain.OnGalaxyGoToWorld -= OnGalaxyGoToWorld;
    }


    // Use this for initialization
    void Start () {
        windowNewPlayer.SetActive(false);
        panelNamePlayer.SetActive(false);

        int i = 0;
        foreach (var item in panelNewPlayers)
        {
//            item.transform.Find("ImageAdd").GetComponent<Image>().sprite = DataBaseAvatars.Instance.avatar96[i];
            i++;
        }
    }

    // Выбор персонажа или создать новый если пусто
    public void OnButtonPlayerClick(int plNum)
    {
        //GalaxyClient.Send.Room.JoinToRoom(1);
        //GalaxyClient.RPC.SendRPC("Hello", new List<object>((int)1), Route.ToAll, 2);
        if (plNum == 1)
        {
            if (existPlayer1)
            {
                // Выбрали 1
//!                GalaxyClient.SendOperation.Game.GameEnter(1);
            }
            else
            {
                // Новый
                windowNewPlayer.SetActive(true);
            }
        }

        if (plNum == 2)
        {
            if (existPlayer2)
            {
                // Выбрали 2
//!                GalaxyClient.SendOperation.Game.GameEnter(2);
            }
            else
            {
                // Новый
                windowNewPlayer.SetActive(true);
            }
        }

        if (plNum == 3)
        {
            if (existPlayer3)
            {
                // Выбрали 3
//!                GalaxyClient.SendOperation.Game.GameEnter(3);
            }
            else
            {
                // Новый
                windowNewPlayer.SetActive(true);
                
            }
        }
    }

    public void OnButtonClosePlayerCreate()
    {
        panelNamePlayer.SetActive(false);
        windowNewPlayer.SetActive(false);
    }


    public void OnButtonNewSelectClick(int avNum)
    {
        panelNamePlayer.SetActive(true);
//        addPlayerImg.sprite = DataBaseAvatars.Instance.avatar96[avNum];
        inpF.text = "";
//        tAvaId = (ushort)avNum;
    }


    public void OnButtonNewSelectBack()
    {
        panelNamePlayer.SetActive(false);
    }

    // Подтверждение создания персонажа
    public void OnButtonNewSelectCreateOk()
    {
        var plName = inpF.text;
        if(plName.Length >= 4 && plName.Length <= 32)
        {
//!!!!!!!!!!!!!!!            GalaxyClient.MyPlayer.MyPlayerCreateNew(plName, tAvaId);

            OnButtonClosePlayerCreate();
        }
        
    }





    // RPC Сервер прислал список моих игроков
    void OnGalaxyMyPlayersList(List<GGPlayer> playersList)
    {
        existPlayer1 = false;
        existPlayer2 = false;
        existPlayer3 = false;

        panelPlayer1.transform.Find("ImageAdd").GetComponent<Image>().sprite = addPlayerSprite;
        panelPlayer2.transform.Find("ImageAdd").GetComponent<Image>().sprite = addPlayerSprite;
        panelPlayer3.transform.Find("ImageAdd").GetComponent<Image>().sprite = addPlayerSprite;

        if (playersList.Count == 0)
        {
            // show hint
            hintNoPlayers.SetActive(true);
            return;
        }

        hintNoPlayers.SetActive(false);
        int i = 0;
        foreach (var item in playersList)
        {
            i++;

            if(i == 1)
            {
//                panelPlayer1.transform.Find("ImageAdd").GetComponent<Image>().sprite = DataBaseAvatars.Instance.avatar96[item.avaId];
                panelPlayer1.transform.Find("Text").GetComponent<Text>().text = item.name;
                //idPlayer1 = item.playerId;
                existPlayer1 = true;
            }

            if (i == 2)
            {
//                panelPlayer2.transform.Find("ImageAdd").GetComponent<Image>().sprite = DataBaseAvatars.Instance.avatar96[item.avaId];
                panelPlayer2.transform.Find("Text").GetComponent<Text>().text = item.name;
                //idPlayer2 = item.playerId;
                existPlayer2 = true;
            }

            if (i == 3)
            {
//                panelPlayer3.transform.Find("ImageAdd").GetComponent<Image>().sprite = DataBaseAvatars.Instance.avatar96[item.avaId];
                panelPlayer3.transform.Find("Text").GetComponent<Text>().text = item.name;
                //idPlayer3 = item.playerId;
                existPlayer3 = true;
            }

        }
    }

    // [RPC] Пришла информация о твоем персонаже
    //void OnGalaxyGoToWorld()
    //{
    //    //Debug.Log(GalaxyClient.MyPlayer.loadedFromDB);
    //    //Debug.Log(GalaxyClient.MyPlayer.Info.id);
    //    //Debug.Log(GalaxyClient.MyPlayer.Info.name);
    //}

}
