using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class mod_graph : MonoBehaviour {

    [Header("Масштаб")]
    public float zoom = 10.0f;

    [Header("Макс превышение")]
    public byte max1 = 5;
    public byte max2 = 10;
    public byte max3 = 30;


    //[Header("Макс значение")]
    //public float max_value = 999;


    [Header("Скорость обновления (сек) 0-нет")]
    public float refreshTime = 0.0f;
    float _refreshTime = 0.0f;

    [Header("Прозрачный цвет")]
    public Color32 colA = new Color32(0, 0, 0, 40);

    [Header("Обычный цвет")]
    public Color32 col0 = Color.white;

    [Header("Цвета превышения")]
    public Color32 col1;
    public Color32 col2;
    public Color32 col3;

    [Header("Переменная сохранения состояния")]
    public string SavePrefsName = "";

    Texture2D tex;
    public RawImage ri;
    int ri_w;   // ширина поля (px)
    int ri_h;   // высота поля (px)

    public int[] arr;// = new int[128];

    public bool working = true;

    Text maxValueText;
    Text averageValueText;
    Text currentValueText;
    Text zoomText;
    float zoomColorA = 0;
    Color32 zoomColorStart;

    float maxValueF = 0;
    float averageValueF = 0;
    float allValueF = 0;
    int cntValueI = 0;

    float lastValue = 0;


    Color32 col; // = Color.white;
    int x, y;
    Color32[] tempArray;


    void Awake()
    {
        maxValueText = gameObject.transform.Find("TextMaxValue").GetComponent<Text>();
        averageValueText = gameObject.transform.Find("TextAverageValue").GetComponent<Text>();
        currentValueText = gameObject.transform.Find("TextCurrentValue").GetComponent<Text>();
        zoomText = gameObject.transform.Find("TextZoomValue").GetComponent<Text>();
        if (zoomText != null) zoomColorStart = zoomText.color;

        ri_w = (int)ri.rectTransform.sizeDelta.x;
        ri_h = (int)ri.rectTransform.sizeDelta.y;
        tex = new Texture2D(ri_w, ri_h);  // 128, 64
        arr = new int[ri_w];


        tex.anisoLevel = 0;
        //tex.Compress(false);
        tex.filterMode = FilterMode.Point;

        ri.texture = tex;
        tempArray = tex.GetPixels32();
        Refresh();
    }

    void Start()
    {
        //zoom_old = zoom;
        //tex = new Texture2D(128, 64);
        //Color32[] tempArray = tex.GetPixels32();
    }

    public void ZoomChange(float deltaZoom)
    {
        zoomText.gameObject.SetActive(true);
        float t = zoom + deltaZoom;
        if (t > 0) zoom = zoom + deltaZoom;
        zoomColorA = zoomColorStart.a;
        zoomText.color = zoomColorStart;
        zoomText.text = zoom.ToString("0.00");
        Refresh();
            //zoomColorStart = zoomText.color;
    }


    void Update()
    {
        if (zoomColorA != 0)
        {
            zoomColorA -= Time.deltaTime * 300;
            zoomText.color = new Color32(zoomColorStart.r, zoomColorStart.g, zoomColorStart.b, (byte)zoomColorA);
            if (zoomColorA <= 0)
            {
                zoomColorA = 0;
                zoomText.gameObject.SetActive(false);
            }
        }



        if (refreshTime !=0 )
        {
            

            _refreshTime += Time.deltaTime;
            if(_refreshTime >= refreshTime)
            {
                AddValue(lastValue);
                _refreshTime = 0;
            }
        }
    }


    void Refresh () {
        //if (!working) return;
        for (x = 0; x <= ri_w-1; x++)
        {
            // пустота
            for (y = arr[x]; y <= ri_h-1; y++)   // амплитуда 63
            {
                int n = y * ri_w + x;
                tempArray[n] = colA;
            }

            // амплитуда
            col = col0;
            if (arr[x] > max1) col = col1;
            if (arr[x] > max2) col = col2;
            if (arr[x] > max3) col = col3;

            for (y = 0; y <= arr[x]; y++)   // амплитуда 63
            {
                int n = y * ri_w + x;
                tempArray[n] = col;
            }

        }

        tex.SetPixels32(tempArray);
        tex.Apply();

    }
	
	
    /// <summary>
    /// Добавить значение в график
    /// </summary>
    /// <param name="v"></param>
    public void AddValue(float v)
    {
        lastValue = v;
        if (!gameObject.activeSelf) return;
        cntValueI++;
        allValueF += v;
        averageValueF = allValueF / cntValueI;

        averageValueText.text = "Среднее: " + System.Math.Round(averageValueF, 2).ToString();
        currentValueText.text = "Текущее: " + System.Math.Round(v, 2).ToString();

        if (v > maxValueF)
        {
            maxValueF = v;
            maxValueText.text = "Макс: " + System.Math.Round(maxValueF, 2).ToString();
        }

        /*
        if((maxValueF * zoom) < 60)
        {
            zoom += 1f;
        }
        if ((maxValueF * zoom) > 60)
        {
            zoom -= 1f;
        }
        */

        if (!working) return;

        int iv;
        v = v * zoom;
        if (v >= ri_h) v = ri_h-1;
        iv = (int)v;

        Array.Copy(arr, 0, arr, 1, arr.Length - 1); //Array.Copy(arr, 1, arr, 0, arr.Length - 1);
        arr[0] = iv;

        Refresh();
    }

    public void CloseWindow()
    {
        working = false;
        this.gameObject.SetActive(false);

        PlayerPrefs.SetInt(SavePrefsName, 0);
    }

}
