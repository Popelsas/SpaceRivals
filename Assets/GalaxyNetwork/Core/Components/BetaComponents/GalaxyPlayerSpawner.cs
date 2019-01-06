using GalaxyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum SpawnPosition
{
    Standart,
    onDatabase,
    onSpawnPoints
}

[HelpURL("https://docs.google.com/document/d/1u43Jxeq4LfcfxcCSLvWMvHALlEjTz8oxh9adJRqdVN4/edit#heading=h.fuq5v4ygo4eo")]
public class GalaxyPlayerSpawner : MonoBehaviour {


    [Header("Префаб игрока")]
    public GameObject playerPref;

    [Header("Задержка появления игрока (сек.)")]
    public float spawnDelay = 0.0f;

    [Header("Место появления игрока")]
    [SerializeField]
    SpawnPosition spawnPosition = SpawnPosition.Standart;

#if UNITY_EDITOR
    [G_HelpBox("Помещает игрока в свою позицию \n\n" +
                "'playerPref' Префаб с компонентом [GalaxyNetId], расположенный в папке Resources.\n\n" +
                "Который появится в точке спауна компонента [GalaxySpawnPoint]\n\n" +
                " \n\n" +
                "", UnityEditor.MessageType.None, null, null, false, G_PropertyDrawerUtility.Space.Nothing)]
    public float f_help;
#endif

    public static Log log = Log.GetLogger(typeof(GalaxyPlayerSpawner));

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "GalaxyPlayerSpawn", false);
    }

    // Авто подписка
    void OnEnable()
    {
        GalaxyEvents.OnGalaxyRoomEnter += OnGalaxyRoomEnter;
    }

    void OnDisable()
    {
        GalaxyEvents.OnGalaxyRoomEnter -= OnGalaxyRoomEnter;
    }

    /// <summary>
    /// Создать игрока на сцене
    /// </summary>
    public void Spawn () {
        if (!GalaxyNetwork.Connection.connected) return;
        if (!playerPref) { log.Warn("Не указан префаб игрока"); return; }

        switch (spawnPosition)
        {
            case SpawnPosition.Standart:
                Instantiate(playerPref, transform.position, transform.rotation);
                break;
            case SpawnPosition.onDatabase:
                Instantiate(playerPref, GalaxyNetwork.Players.MyPlayer.position, GalaxyNetwork.Players.MyPlayer.rotation);
                break;
            case SpawnPosition.onSpawnPoints:
                break;
            default:
                break;
        }

        
    }

    void OnGalaxyRoomEnter(UInt32 clientId, String nikname, ErrorCode errorCode)
    {
        if (errorCode != ErrorCode.none)
        {
            log.Error(ErrorMessages.RU[errorCode]);
            return;
        }

        if (clientId == GalaxyNetwork.Connection.clientId)
        {
            Invoke("Spawn", spawnDelay);
        }
    }


}
