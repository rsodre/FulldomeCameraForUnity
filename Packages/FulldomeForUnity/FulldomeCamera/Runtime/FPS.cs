using System;
using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour
{
	Text _text;

	void Update ()
	{
		CalcFramerate();
		_text = GetComponent<Text>();
		_text.text = "" + String.Format("{0:0.##}", m_lastFramerate);
	}

	//
	// Framerate
	// http://answers.unity3d.com/questions/46745/how-do-i-find-the-frames-per-second-of-my-game.html
	int m_frameCounter = 0;
	float m_timeCounter = 0.0f;
	float m_lastFramerate = 0.0f;
	public float m_refreshTime = 0.5f;
	void CalcFramerate()
	{
		if( m_timeCounter < m_refreshTime )
		{
			m_timeCounter += Time.deltaTime;
			m_frameCounter++;
		}
		else
		{
			//This code will break if you set your m_refreshTime to 0, which makes no sense.
			m_lastFramerate = ((float)m_frameCounter/m_timeCounter) * Time.timeScale;
			m_frameCounter = 0;
			m_timeCounter = 0.0f;
		}
	}
}
