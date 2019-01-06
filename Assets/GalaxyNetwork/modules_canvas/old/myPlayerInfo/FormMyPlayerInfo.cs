using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GalaxyLib;

public class FormMyPlayerInfo : MonoBehaviour
{
    public Text TextClientId;
    public Text TextPlayerId;
    public Image ImgAva;
    public Text TextName;
    public Text TextSector;
    public Text TextPositionX;
    public Text TextPositionY;
    public Text TextPositionZ;

    uint old_clientId;

    uint old_playerId;   // Порядковый номер твоего выбранного персонажа

    ushort old_avaId;

    string old_plName;

    ushort old_sectorX = ushort.MaxValue;
    ushort old_sectorY = ushort.MaxValue;
    ushort old_sectorZ = ushort.MaxValue;

    Vector3 old_pos;
    //float old_posX;
    //float old_posY;
    //float old_posZ;

    // ПОДПИСКИ
    // Авто подписка
    //void OnEnable()
    //{
    //    ClientMain.OnGalaxyMyPlayerInfo += OnGalaxyMyPlayerInfo;            // Пришла информация о твоем персонаже.
    //}

    // Авто отписка
    //void OnDisable()
    //{
    //    ClientMain.OnGalaxyMyPlayerInfo -= OnGalaxyMyPlayerInfo;
    //}

    // Update is called once per frame
    void Update()
    {
        // Поменялся ID клиента
        //if (GalaxyClient.MyPlayer.player.clientId != old_clientId)
        if (GalaxyNetwork.Players.MyPlayer.clientId != old_clientId)
        {
            TextClientId.text = GalaxyNetwork.Players.MyPlayer.clientId.ToString();
            old_clientId = GalaxyNetwork.Players.MyPlayer.clientId;
        }

        // Поменялся ID игрока (Выбранный порядковый номер)
//        if (GalaxyClient.Players.MyPlayer.playerId != old_playerId)
//        {
//            TextPlayerId.text = GalaxyClient.Players.MyPlayer.playerId.ToString();
//            old_playerId = GalaxyClient.Players.MyPlayer.playerId;
//        }

        // Поменялась аватарка
        if (GalaxyNetwork.Players.MyPlayer.avaId != old_avaId)
        {
//            ImgAva.sprite = DataBaseAvatars.Instance.avatar96[GalaxyClient.MyPlayer.player.avaId];
            old_avaId = GalaxyNetwork.Players.MyPlayer.avaId;
        }

        // Поменялось имя
        if (GalaxyNetwork.Players.MyPlayer.name != old_plName)
        {
            TextName.text = GalaxyNetwork.Players.MyPlayer.name;
            old_plName = GalaxyNetwork.Players.MyPlayer.name;
        }

        // Поменялся сектор X
        if (GalaxyNetwork.Players.MyPlayer.sector_x != old_sectorX)
        {
            TextSector.text = GalaxyNetwork.Players.MyPlayer.sector_x + ", " + GalaxyNetwork.Players.MyPlayer.sector_y + ", " + GalaxyNetwork.Players.MyPlayer.sector_z;
            old_sectorX = GalaxyNetwork.Players.MyPlayer.sector_x;
        }

        // Поменялся сектор Y
        if (GalaxyNetwork.Players.MyPlayer.sector_y != old_sectorY)
        {
            TextSector.text = GalaxyNetwork.Players.MyPlayer.sector_x + ", " + GalaxyNetwork.Players.MyPlayer.sector_y + ", " + GalaxyNetwork.Players.MyPlayer.sector_z;
            old_sectorY = GalaxyNetwork.Players.MyPlayer.sector_y;
        }

        // Поменялся сектор Z
        if (GalaxyNetwork.Players.MyPlayer.sector_z != old_sectorZ)
        {
            TextSector.text = GalaxyNetwork.Players.MyPlayer.sector_x + ", " + GalaxyNetwork.Players.MyPlayer.sector_y + ", " + GalaxyNetwork.Players.MyPlayer.sector_z;
            old_sectorZ = GalaxyNetwork.Players.MyPlayer.sector_z;
        }

        // Поменялось положение
        if (GalaxyNetwork.Players.MyPlayer.position != old_pos)
        {
            //TextPosition.text = string.Format("{0:N1}, {1:0.0}, {2:0.0}", GalaxyClient.MyPlayer.player.pos_x.ToString(), GalaxyClient.MyPlayer.player.pos_y.ToString(), GalaxyClient.MyPlayer.player.pos_z.ToString());

            //TextPositionX.text = string.Format("X: {0:#.##}\r\nY: {1:#.##}\r\nZ: {2:#.##}", GalaxyClient.Players.MyPlayer.position.x, GalaxyClient.Players.MyPlayer.position.y, GalaxyClient.Players.MyPlayer.position.z);
            TextPositionX.text = string.Format("{0:#.##}", GalaxyNetwork.Players.MyPlayer.position.x);
            TextPositionY.text = string.Format("{0:#.##}", GalaxyNetwork.Players.MyPlayer.position.y);
            TextPositionZ.text = string.Format("{0:#.##}", GalaxyNetwork.Players.MyPlayer.position.z);

            //TextPosition.text = GalaxyClient.MyPlayer.player.pos_x + ", " + GalaxyClient.MyPlayer.player.pos_y + ", " + GalaxyClient.MyPlayer.player.pos_z;
            old_pos = GalaxyNetwork.Players.MyPlayer.position;
        }


    }


    //void OnGalaxyMyPlayerInfo()
    //{ }

}
