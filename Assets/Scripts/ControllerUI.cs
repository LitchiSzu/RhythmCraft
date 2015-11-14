﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ControllerUI : MonoBehaviour 
{
	public Image m_UpArrow = null;
	public Image m_RightArrow = null;
	public Image m_DownArrow = null;
	public Image m_LeftArrow = null;

	public Image m_ValidationFeedback = null;

	public Color m_ActiveDirectionColor = Color.white;
	public Color m_InactiveDirectionColor = Color.white;

	public Color m_DefaultFeedbackColor = Color.white;
	public Color m_PositiveFeedbackColor = Color.white;
	public Color m_NegativeFeedbackColor = Color.white;
	private Coroutine m_TemporaryColorChangeCoroutine = null;

	private Image m_currentDirection = null;
	
	void Awake()
	{
		Controller.OnChangeDirection += UpdateDirection;
		CraftToken.OnTokenCheck += FeedbackCheck;
	}

	void UpdateDirection(Direction newDirection)
	{
		if(m_currentDirection != null)
			StartCoroutine(ChangeImageColor(m_currentDirection, m_InactiveDirectionColor));

		switch(newDirection)
		{
			case Direction.Up:
				m_currentDirection = m_UpArrow;				
				break;

			case Direction.Right:
				m_currentDirection = m_RightArrow;
				break;

			case Direction.Down:
				m_currentDirection = m_DownArrow;
				break;

			case Direction.Left:
				m_currentDirection = m_LeftArrow;
				break;

			case Direction.None:
				m_currentDirection = null;
				break;
		}
		if(m_currentDirection != null)
			StartCoroutine(ChangeImageColor(m_currentDirection, m_ActiveDirectionColor));
	}

	void FeedbackCheck(bool state)
	{
		if (state)
		{
			m_TemporaryColorChangeCoroutine = StartCoroutine(ChangeImageColorTemporary(m_ValidationFeedback, m_PositiveFeedbackColor, m_DefaultFeedbackColor, 1.0f));
		}
		else
		{
			m_TemporaryColorChangeCoroutine = StartCoroutine(ChangeImageColorTemporary(m_ValidationFeedback, m_NegativeFeedbackColor, m_DefaultFeedbackColor, 1.0f));
		}
	}

	IEnumerator ChangeImageColor(Image image, Color newColor)
	{
		image.color = newColor;
		yield return 0;
	}

	IEnumerator ChangeImageColorTemporary(Image image, Color newColor, Color defaultColor, float duration)
	{
		if(m_TemporaryColorChangeCoroutine != null) StopCoroutine(m_TemporaryColorChangeCoroutine);

		StartCoroutine(ChangeImageColor(image, newColor));

		yield return new WaitForSeconds(duration);

		StartCoroutine(ChangeImageColor(image, defaultColor));
	}

}
