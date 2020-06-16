
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameRate : MonoBehaviour {

    public float updateInterval = 0.5f;
    public Text fpsText;

    private float m_Accum = 0.0f; // FPS accumulated over the interval
    private float m_Frames = 0f; // Frames drawn over the interval
    private float m_Timeleft  = 0f; // Left time for current interval


    private void Awake()
    {
        fpsText = this.gameObject.GetOrAddComponent<Text>();
    }

    void Start()
    {
        m_Timeleft = this.updateInterval;
    }

    void Update()
    {
        m_Timeleft -= Time.deltaTime;
        m_Accum += Time.timeScale / Time.deltaTime;
        ++m_Frames;

        // Interval ended - update GUI text and start new interval
        if (m_Timeleft <= 0.0f)
        {
            fpsText.text = "µ±Ç°Ö¡ÂÊ:" + (m_Accum / m_Frames).ToString("f2");
            m_Timeleft = this.updateInterval;
            m_Accum = 0.0f;
            m_Frames = 0;
        }
    }
}


