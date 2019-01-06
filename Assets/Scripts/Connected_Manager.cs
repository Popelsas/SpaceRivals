using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GalaxyLib;

public class Connected_Manager : MonoBehaviour {

    public GameObject player;

    public void OnGalaxyConnect(ErrorCode errorCode)    //параметры метода обязательны, если требуются
    {
        if (errorCode != ErrorCode.none) Debug.LogError("Ошибка: " +
        ErrorMessages.RU[errorCode]);
        else
        {
            Debug.Log("Подключились к серверу");
            
        }

    }

    void OnGalaxyEnterToNewRoom(GGRoom room, ErrorCode errorCode)
    {
        InstabtiatePlayer();
    }

    void OnGalaxyRoomEnter(uint clientId, string nikname, ErrorCode errorCode)
    {
        if (clientId == GalaxyNetwork.Connection.clientId && !GalaxyNetwork.Connection.isHost) InstabtiatePlayer();       
    }
        

    private void InstabtiatePlayer()
    {
        //Destroy(GameObject.Find("Camera"));
        GameObject instPlayer = Instantiate(player, player.transform.position, player.transform.rotation);
        //instPlayer.transform.Find("Camera").gameObject.SetActive(true);
        Messenger<bool, GameObject>.Broadcast(GameEvent.CONNECTED, true, instPlayer);
    }



    private void OnEnable()
    {
        GalaxyEvents.OnGalaxyConnect += OnGalaxyConnect;
        GalaxyEvents.OnGalaxyEnterToNewRoom += OnGalaxyEnterToNewRoom;
        GalaxyEvents.OnGalaxyRoomEnter += OnGalaxyRoomEnter;
    }

    private void OnDisable()
    {
        GalaxyEvents.OnGalaxyConnect -= OnGalaxyConnect;
        GalaxyEvents.OnGalaxyEnterToNewRoom -= OnGalaxyEnterToNewRoom;
        GalaxyEvents.OnGalaxyRoomEnter -= OnGalaxyRoomEnter;
    }
}
