using GalaxyCommon;
using GalaxyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class mod_statistic_events : MonoBehaviour {

    [Header("Включить. Сильно нагружат систему")]
    public bool mod_enable = false;

    [Header("Максимум событий в списке")]
    [Range(0, 500)]
    public int max_events = 100;

    public GameObject captionPanel;
    public GameObject scrollPanel;
    public Transform content;
    public GameObject rowPref;
    public Text textAllBytes;

    int all_size = 0;           // Всего (размер байт)
    int all_events_cnt = 0;     // Всего (евентов)
    Stopwatch stopWatch = new Stopwatch();  // Замер времени


    List<mod_statistic_events_row> rowsList = new List<mod_statistic_events_row>();

    private void Start()
    {
        if (mod_enable)
        {
            rowPref.SetActive(false);
            //stopWatch.Start();
        }
    }

    // Авто подписка
    void OnEnable()
    {
        if (mod_enable)
            GalaxyEvents.OnEventStatistic += OnEventStatistic;
    }

    // Авто отписка
    void OnDisable()
    {
        if (mod_enable)
            GalaxyEvents.OnEventStatistic -= OnEventStatistic;
    }


    void OnEventStatistic(ServerEvent serverEvent)
    {
        Add(serverEvent.eventCode, serverEvent.eventSizeInByte);
    }



    public void Show(bool show)
    {
        if (!mod_enable) return;
        if(show)
        {
            captionPanel.SetActive(true);
            scrollPanel.SetActive(true);
        }
        else
        {
            captionPanel.SetActive(false);
            scrollPanel.SetActive(false);
        }
    }

    public void Clear()
    {
        if (!mod_enable) return;
        foreach (var item in rowsList)
        {
            Destroy(item.gameObject);
        }
        rowsList.Clear();
        all_size = 0;
        all_events_cnt = 0;

        stopWatch.Stop();
        stopWatch.Reset();
    }

    void Add(EventCode eventCode, int size)
    {
        stopWatch.Stop();
        TimeSpan ts = stopWatch.Elapsed;
        //string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
        //    ts.Hours, ts.Minutes, ts.Seconds,
        //    ts.Milliseconds / 10);
        string elapsedTime = String.Format("{0:00}.{1:00}.{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        stopWatch.Reset();
        stopWatch.Start();

        all_size += size;
        all_events_cnt++;

        textAllBytes.text = all_size + " Байт";

        GameObject go = Instantiate(rowPref, content);
        go.transform.SetSiblingIndex(0);
        mod_statistic_events_row row = go.GetComponent<mod_statistic_events_row>();
        row.text_col1.text = elapsedTime;
        row.text_col2.text = eventCode.ToString();
        row.text_col3.text = size.ToString();
        rowsList.Add(row);
        go.SetActive(true);


        // Превышен лимит. Удаляем самое раннее событие
        if (rowsList.Count > max_events)
        {
            Destroy(rowsList[0].gameObject);
            rowsList.RemoveAt(0);
        }

    }




}
