// ===============================
// © GGTeam GalaxyNetwork module
// Модуль: Список онлайн-игроков
// Версия: 0.1
// ===============================

using UnityEngine;
using System.Collections.Generic;
using GalaxyLib;

public class mod_online_players : MonoBehaviour {

    [Header("Авто открытие окна при входе в комнату")]
    public bool auto_open = true;

    [Space(32)]

    public GameObject rowPref;
    public Transform scrollContent;
    public GameObject panelModule;

    Dictionary<uint, row_online_players> players = new Dictionary<uint, row_online_players>();
    bool visibled = false;

    // Авто подписка
    void OnEnable()
    {
        GalaxyEvents.OnGalaxyRoomEnter += OnGalaxyRoomEnter;        // Кто то вошел в комнату.
        GalaxyEvents.OnGalaxyRoomExit += OnGalaxyRoomExit;          // Кто то вышел из комнаты.
        GalaxyEvents.OnGalaxyDisconnect += OnGalaxyDisconnect;
        GalaxyEvents.OnGalaxyRoomUpdate += OnGalaxyRoomUpdate;
    }

    // Авто отписка
    void OnDisable()
    {
        GalaxyEvents.OnGalaxyRoomEnter -= OnGalaxyRoomEnter;
        GalaxyEvents.OnGalaxyRoomExit -= OnGalaxyRoomExit;
        GalaxyEvents.OnGalaxyDisconnect -= OnGalaxyDisconnect;
        GalaxyEvents.OnGalaxyRoomUpdate -= OnGalaxyRoomUpdate;
    }

    void Start () {
        rowPref.SetActive(false);
        players.Clear();
    }

    /// <summary>
    /// Показать/скрыть окно
    /// </summary>
    /// <param name="showed"></param>
    public void Show(bool showed)
    {
        if (showed)
        {
            if (!visibled)
            {
                panelModule.SetActive(true);
                visibled = true;
            }
        }
        else
        {
            if (visibled)
            {
                panelModule.SetActive(false);
                visibled = false;
            }
        }
    }

    /// <summary>
    /// Очистить список игроков
    /// </summary>
    public void Clear()
    {
        foreach (var playerRow in players)
        {
            Destroy(playerRow.Value.gameObject);
            players.Remove(playerRow.Key);          //?
        }
    }

    /// <summary>
    /// Новый игрок зашел в вашу комнату
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="nikname"></param>
    /// <param name="errorCode"></param>
    void OnGalaxyRoomEnter(uint clientId, string nikname, ErrorCode errorCode)
    {
        if (errorCode != ErrorCode.none) return;
        GGPlayer player = GalaxyNetwork.Players.GetPlayer(clientId);

        // Если входим мы, то дополнительно получаем список игроков, уже находившихся в комнате
        if(player.isMy)
        {
            if (auto_open) Show(true);
            foreach (var item in GalaxyNetwork.Players.GetPlayersList())
            {
                CreateRow(item);
            }
        }
        else
            CreateRow(player);
    }

    // Вызовется при выходе создалеля комнаты из нее
    void OnGalaxyRoomUpdate(GGRoom room, ErrorCode errorCode)
    {
        // Вы новый владелец комнаты
        //Debug.Log("Owner:" + room.owner_client_id);
    }

    // Создаем строку игрока
    void CreateRow(GGPlayer player)
    {
        if (players.ContainsKey(player.clientId)) return;

        GameObject rowGo = Instantiate(rowPref, scrollContent);
        row_online_players row = rowGo.GetComponent<row_online_players>();
        row.clientId = player.clientId;
        row.isMy = player.isMy;
        if (GalaxyNetwork.Room.owner_client_id == player.clientId) row.isOwner = true;
        else row.isOwner = false;
        row.playerName = player.name;
        row.avaId = player.avaId;
        row.position = player.position;
        row.rotation = player.rotation;
        players.Add(player.clientId, row);
        rowGo.SetActive(true);
    }

    /// <summary>
    /// Игрок покинул комнату
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="nikname"></param>
    /// <param name="errorCode"></param>
    void OnGalaxyRoomExit(uint clientId, string nikname, ErrorCode errorCode)
    {
        if (errorCode != ErrorCode.none) return;
        if (!players.ContainsKey(clientId)) return;

        // Если вышел создатель комнаты, то переназначаем картинку - новому
        if (players[clientId].isOwner)
        {
            //Debug.Log("Вышел владелец " + clientId);
            foreach (var item in players)
            {
                // NullReferenceException: Object reference not set to an instance of an object
                //if (item.Key == GalaxyNetwork.Room.owner_client.clientId)

                // Все ОК. Но не изменяется при смене владельца
                if (item.Key == GalaxyNetwork.Room.owner_client_id)         
                {
                    //Debug.Log("Удачно! " + item.Key);
                    item.Value.isOwner = true;
                }
            }
        }

        Destroy(players[clientId].gameObject);
        players.Remove(clientId);
    }


    /// <summary>
    /// Произошло отключение от сервера
    /// </summary>
    void OnGalaxyDisconnect()
    {
        Clear();
    }

}
