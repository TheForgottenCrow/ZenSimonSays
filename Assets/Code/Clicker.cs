using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum e_Level
{
    Button = 0,
    Slider,
    MultiTouch,
    DragDrop
}

public class Clicker : MonoBehaviour
{
    [Header("Global")]
    [SerializeField] private e_Level m_Level = new e_Level();
    [SerializeField] private GameObject m_Panel_Button, m_Panel_Slider, m_Panel_MultiTouch, m_Panel_DragDrop;
    [SerializeField] bool m_CoRWait;

    [Header("Button")]
    [SerializeField] private Text m_ButtonText;
    private int m_ClickCount;

    [Header("Slider")]
    [SerializeField] private List<Slider> m_Sliders;
    [SerializeField] private List<Text> m_SliderText;
    [SerializeField] private int[] m_SliderCode;
    [SerializeField] private AudioClip a_Click,a_Unlock;

    [Header("MultiTouch")]
    [SerializeField] private bool m_SnakeMode = false;
    [SerializeField] private Slider m_SliderNext;


    private void Start()
    {
        SetPanel(m_Panel_Button);
    }

    private void Update()
    {
        switch (m_Level)
        {
            case e_Level.Button:
                Button();
                break;

            case e_Level.Slider:
                Slider();
                break;

            case e_Level.MultiTouch:
                MultiTouch();
                break;

            case e_Level.DragDrop:
                SetPanel(m_Panel_DragDrop);
                DragDrop();
                break;
        }
    }


    public void Button()
    {
        if (m_ClickCount >= 10)
        {
            SetPanel(m_Panel_Slider);
            m_Level++;
        }
    }

    public void Slider()
    {
        for (int i = 0; i < m_Sliders.Count; i++)
        {
            m_SliderText[i].text = m_Sliders[i].value.ToString();
        }

        int slidersCorrect = 0;
        for (int i = 0; i < m_Sliders.Count; i++)
        {
            if (m_Sliders[i].value == m_SliderCode[i])
            {
                slidersCorrect++;
            }
        }
        if (slidersCorrect >= 4 && m_CoRWait == false)
        { 
            StartCoroutine(i_Wait(2));
        }
    }
    

    public void MultiTouch()
    {
        if (m_SnakeMode)
        {
            Vector3 oldTouchPos = Vector3.zero;
            for (int i = 0; i < Input.touchCount; i++)
            { 
                Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.touches[i].position);
                touchPos.z = 0f;
                Debug.DrawLine(oldTouchPos, touchPos, Color.red);
                oldTouchPos = touchPos;
            }
        }
        else
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.touches[i].position);
                Debug.DrawLine(Vector3.zero, touchPos, Color.red);
            }
        }
    }

    public void DragDrop()
    {

    }


    IEnumerator i_Wait(int seconds)
    {
        m_CoRWait = true;
        AudioSource.PlayClipAtPoint(a_Unlock, new Vector3(0, 0, -10));
        yield return new WaitForSecondsRealtime(seconds);
        SetPanel(m_Panel_MultiTouch);
        m_Level++;
        m_CoRWait = false;
    }

    public void SetPanel(GameObject panel)
    {
        m_Panel_Button.SetActive(false);
        m_Panel_DragDrop.SetActive(false);
        m_Panel_Slider.SetActive(false);
        m_Panel_MultiTouch.SetActive(false);

        panel.SetActive(true);
    }

    public void ButtonClick()
    {
        m_ClickCount++;
        m_ButtonText.text = m_ClickCount.ToString();
    }

    public void SliderClick(Slider slider)
    {
        int index = m_Sliders.IndexOf(slider);
        Debug.Log(index.ToString());
        if (slider.value == m_SliderCode[index])
        {
            Debug.Log("Click");
            AudioSource.PlayClipAtPoint(a_Click, new Vector3(0,0,-10));
        }
    }

    public void SetSnakeMode(bool b)
    {
        m_SnakeMode = b;
    }
}

