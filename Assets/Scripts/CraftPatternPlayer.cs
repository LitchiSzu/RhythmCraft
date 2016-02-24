﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum CraftState
{
    Invalid,
    Success,
    Failure,
    NearSuccess,
}

public class CraftPatternPlayer : MonoBehaviour 
{
	List<CraftToken> m_currentTokens = new List<CraftToken>();
	
	public Animator m_Animator = null;
    private bool m_toEnd = false;

    private int m_totalTokenCount = 0;
    private int m_successTokenCount = 0;

    private ItemData m_currentItemData;

    public delegate void CraftSequenceStarted(ItemData item);
    public static CraftSequenceStarted s_craftSequenceStarted;

    public delegate void CraftSequenceEnded(ItemData item, CraftState state);
    public static CraftSequenceEnded s_craftSequenceEnded;

    public bool m_IsPlaying = false;

    void Start()
    {
    }

    void Reset()
    {
        m_totalTokenCount = 0;
        m_successTokenCount = 0;
        m_toEnd = false;
        m_currentTokens.Clear();
        m_currentItemData = null;
    }

    public void StartPattern(ItemData item)
    {
        Reset();
        if(item != null)
        {
            m_currentItemData = item;
            m_Animator.Play(item.m_CraftPattern.name);
            if(s_craftSequenceStarted != null)
            {
                s_craftSequenceStarted(item);
            }
            m_IsPlaying = true;
        }
    }

	public void AddNewToken(AnimationEvent eventInfo)
	{
		Direction tokenDirection = (Direction)Enum.Parse(typeof(Direction), eventInfo.stringParameter);
		TokenType tokenType = (TokenType)eventInfo.intParameter;
		float delay = eventInfo.floatParameter;				

        m_totalTokenCount++;
		m_currentTokens.Add(new CraftToken(tokenDirection, tokenType, delay));
	}

	void Update()
	{
		for (int i = m_currentTokens.Count - 1; i >= 0; i--)
		{
			TokenState state = m_currentTokens[i].SubUpdate();
            if (state != TokenState.Running)
            {
                if(state == TokenState.Success)
                    m_successTokenCount++;
                m_currentTokens.RemoveAt(i);                
            }
		}

        if(m_toEnd && m_currentTokens.Count == 0)
        {
            EndPattern();
        }
	}

    public void SignalEnd()
    {
        if(m_currentTokens.Count < 0)
        {
            EndPattern();
        }
        else
        {
            m_toEnd = true;
        }
    }

    void EndPattern()
    {                
        CraftState endState = CheckEndCraftState();
        
        m_IsPlaying = false;
        if(s_craftSequenceEnded != null)
        {
            s_craftSequenceEnded(m_currentItemData, endState);
        }        

        Reset();
    }

    CraftState CheckEndCraftState()
    {
        float successRate = (float)m_successTokenCount / (float)m_totalTokenCount; 
        
        Debug.Log("EndPattern - Success Rate: " + successRate * 100.0f + "%");        

        if(successRate >= 1.0f)
        {
            return CraftState.Success;
        }
        else if(successRate >= 0.8f)
        {
            return CraftState.NearSuccess;
        }

        return CraftState.Failure;
    }
}
