using UnityEngine;
using System.Collections;
using GalaxyLib;

public class PlayersControl : MonoBehaviour {

    [Header("Префаб твоего игрока")]
    public GameObject PlayerPref;

    [Header("Контейнер мира")]
    public GameObject MapContainer;

    [Header("Твой персонаж после входа в мир")]
    public GameObject myPlayerGo;


    // Авто подписка
    void OnEnable()
    {
//        GalaxyEvents.OnGalaxyPlayerGameEnter += OnGalaxyPlayerGameEnter;            // Пришла информация о твоем персонаже.
        GalaxyEvents.OnGalaxyDisconnect += OnGalaxyDisconnect;                // Отключение от сервера
    }

    // Авто отписка
    void OnDisable()
    {
//        GalaxyEvents.OnGalaxyPlayerGameEnter -= OnGalaxyPlayerGameEnter;
        GalaxyEvents.OnGalaxyDisconnect -= OnGalaxyDisconnect;                // Отключение от сервера
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}



    // ========= [RPC] ==========

    // [RPC] Кто то Заходит в мир
    void OnGalaxyPlayerGameEnter(GGPlayer player)
    {
        myPlayerGo = Instantiate(PlayerPref);
        myPlayerGo.transform.SetParent(MapContainer.transform);

    }

    private void OnGalaxyDisconnect()
    {
        Destroy(myPlayerGo);
    }
}
