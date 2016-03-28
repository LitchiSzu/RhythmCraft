﻿using UnityEngine;
using System.Collections;

public enum HUDSectionState
{
    NotInitialized = 0,
    Minimized,
    Minimizing,
    Maximized,
    Maximizing,
}

public class HUDSection : MonoBehaviour 
{
    public HUDSectionState m_State = HUDSectionState.NotInitialized;
    public Buttons m_Input = Buttons.None;
    public bool m_ButtonIsToggle = false;

    public Vector3 m_MaximizedScale = new Vector3(2.0f, 2.0f, 2.0f);
    public Vector3 m_MinimizedScale = new Vector3(1.0f, 1.0f, 1.0f);

    public float m_ScaleDuration = 0.5f;

    private RectTransform m_RectTransform = null;

    public void Initialize()
    {
        m_State = HUDSectionState.Minimized;
    }

    public bool WantsFocus()
    {
        if(!m_ButtonIsToggle)
        {
            if(XInput.GetButton(m_Input, 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if(m_State == HUDSectionState.Maximized)
            {
                if(XInput.GetButtonUp(m_Input, 0))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else if(m_State == HUDSectionState.Minimized)
            {
                if(XInput.GetButtonUp(m_Input, 0))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }           
        }
        
        return false;
    }

    public void Maximize()
    {
        if(m_RectTransform == null)
        {
            m_RectTransform = GetComponent<RectTransform>();
        }

        m_State = HUDSectionState.Maximizing;
        OnStartMaximize();
        StartCoroutine(ScaleCoroutine(m_MaximizedScale, HUDSectionState.Maximized));
    }

    public void Minimize()
    {
        if(m_RectTransform == null)
        {
            m_RectTransform = GetComponent<RectTransform>();
        }

        m_State = HUDSectionState.Minimizing;
        OnStartMinimize();
        StartCoroutine(ScaleCoroutine(m_MinimizedScale, HUDSectionState.Minimized));
    }

    IEnumerator ScaleCoroutine(Vector3 targetScale, HUDSectionState endState)
    {
        float timer = 0.0f;
        Vector3 initialScale = m_RectTransform.localScale;
        while(timer < m_ScaleDuration)
        {            
            m_RectTransform.localScale = Vector3.Lerp(initialScale, targetScale, timer / m_ScaleDuration);

            yield return 0;
            timer += Time.unscaledDeltaTime;
        }

        m_RectTransform.localScale = targetScale;

        m_State = endState;

        if(endState == HUDSectionState.Maximized)
        {
            OnMaximized();
        }
        else if(endState == HUDSectionState.Minimized)
        {
            OnMinimized();
        }
    }

    public virtual void SubUpdate()
    {

    }

    public virtual void OnStartMaximize()
    {

    }
    public virtual void OnMaximized()
    {

    }
    public virtual void OnStartMinimize()
    {

    }
    public virtual void OnMinimized()
    {

    }
}
