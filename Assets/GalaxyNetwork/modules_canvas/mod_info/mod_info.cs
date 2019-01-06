// ===============================
// © GGTeam GalaxyNetwork module
// Модуль: Всплывающие сообщения чата
// Версия: 0.1
// ===============================

using GalaxyLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[HelpURL("http://wiki.greatgalaxy.ru/index.php?title=GalaxyServer:mod_info")]
public class mod_info : MonoBehaviour {

    [Header("Скорость прокрутки")]
    [Range(10.0f, 90.0f)]
    public float scrollSpeed = 40.0f;

    [Header("Время отображения на символ")]
    [Tooltip("в секундах")]
    [Range(0.05f, 5.0f)]
    public float showTime = 0.3f;

    [Header("Канал вывода сообщений")]
    [Tooltip("Канал прослушки сообщений. Обычный чат по умолчанию, передает по каналу 0")]
    [Range(0, 255)]
    public byte channel = 0;

    [Space(32)]

    public Text textContainer;
    Animator anim;
    bool showed = false;

    Vector3 startpos;
    float showTimeAll = 0;

    // Авто подписка
    void OnEnable()
    {
        GalaxyEvents.OnGalaxyChatMessage += OnGalaxyChatMessage;
    }

    // Авто отписка
    void OnDisable()
    {
        GalaxyEvents.OnGalaxyChatMessage -= OnGalaxyChatMessage;
        //GalaxyEvents.OnGalaxyConnect -= OnGalaxyConnect;
        //GalaxyEvents.OnGalaxyDisconnect -= OnGalaxyDisconnect;
    }

    // Сервер прислал сообщение чата
    //UInt32 clientId, String nikname, String textMessage, Byte channel
    public void OnGalaxyChatMessage(uint clientId, string nikname, string textMessage, byte channel)
    {
        if (this.channel == channel)
        {
            string message = "<color=#F1C15FFF>" + nikname + " </color>" + textMessage;
            ShowMessage(message);
        }
    }


    // Use this for initialization
    void Awake () {
        anim = GetComponent<Animator>();
        startpos = textContainer.rectTransform.localPosition;
    }
	
	// Update is called once per frame
	void Update () {
		if(showed)
        {
            Vector3 pos = textContainer.rectTransform.localPosition;
            pos.x -= (Time.deltaTime * scrollSpeed);
            textContainer.rectTransform.localPosition = pos;
        }
	}

    /// <summary>
    /// Отобразить сообщение
    /// </summary>
    /// <param name="message"></param>
    /// <param name="time_show"></param>
    public void ShowMessage(string message)
    {
        if (message.Length <= 0) return;

        showTimeAll = showTime * message.Length + 0.8f;
        textContainer.text = message;

        Vector3 pos = startpos;// new Vector3(-16.0f, 0.0f, 0.0f);

        textContainer.rectTransform.localPosition = pos;

        anim.Play("mod_info_show");


        showed = true;

        Invoke("HideMessage", showTimeAll);
    }


    public void HideMessage()
    {
        anim.Play("mod_info_hide");
        showed = false;
    }

    

}
