using UnityEngine;
using System.Collections.Generic;

namespace StarstruckFramework
{
	public enum AnimatedPanelControl
	{
		STOP,
		PLAY,
		PLAYTOFRAME,
		REVERSE,
		LOOP
	}

	public class GUILiteAnimatedPanel : GUILiteImage
	{
		[SerializeField]
		private List<Sprite> m_textureList;

		[SerializeField]
		private AnimatedPanelControl m_control;

		private int m_currentFrame;
		[SerializeField]
		private int m_targetFrame;

		public int FPS;
		private float m_SPF;
		// seconds per frame

		private float m_elapseTime;

		public delegate void AnimationStopDelegate ();

		private AnimationStopDelegate m_onLastFrame;

		private bool m_isSpriteBased;

		public override void Awake ()
		{
			base.Awake ();

			m_currentFrame = 0;
			m_targetFrame = 0;

			SetFPS (FPS);

			switch (m_control)
			{
				case AnimatedPanelControl.LOOP:
					Loop ();
					break;
				case AnimatedPanelControl.PLAY:
				case AnimatedPanelControl.PLAYTOFRAME:
				case AnimatedPanelControl.REVERSE:
					Play ();
					break;
			}
		}

		public void OnLastFrame (AnimationStopDelegate onLastFrame)
		{
			m_onLastFrame = onLastFrame;
		}

		public void SetFPS (int fps)
		{
			m_SPF = 1.0f / fps;
		}

		public void SetControl (AnimatedPanelControl control)
		{
			m_control = control;
		}

		public void Play ()
		{
			m_elapseTime = m_SPF;
			Resume ();
		}

		public void Resume ()
		{
			SetControl (AnimatedPanelControl.PLAY);
		}

		public void Stop ()
		{
			SetControl (AnimatedPanelControl.STOP);
		}

		public void Loop ()
		{
			m_elapseTime = m_SPF;
			SetControl (AnimatedPanelControl.LOOP);
		}

		public void Play (int frameNo)
		{
			m_elapseTime = m_SPF;
			if (frameNo < 0 || frameNo > m_textureList.Count) return;

			m_targetFrame = frameNo;
			m_control = AnimatedPanelControl.PLAYTOFRAME;
		}

		public void GoToFrame (int frame)
		{
			if (frame <= m_textureList.Count && frame >= 0)
			{
				m_currentFrame = frame;
				GuiImage.sprite = m_textureList [m_currentFrame];
			}
		}

		public override void Update ()
		{
			if (m_control != AnimatedPanelControl.STOP)
			{
				m_elapseTime += Time.deltaTime;
			}

			if (m_elapseTime > m_SPF)
			{
				switch (m_control)
				{
					case AnimatedPanelControl.PLAYTOFRAME:

						if (m_currentFrame == m_targetFrame)
						{
							m_control = AnimatedPanelControl.STOP;
							if (m_onLastFrame != null) m_onLastFrame ();
						}
						else
						{
							m_currentFrame++;
							if (m_currentFrame >= m_textureList.Count)
							{
								m_currentFrame = 0;
							}

							GuiImage.sprite = m_textureList [m_currentFrame];
						}

						m_elapseTime -= m_SPF;
						break;
					case AnimatedPanelControl.PLAY:
						m_currentFrame++;
						if (m_currentFrame >= m_textureList.Count)
						{
							m_control = AnimatedPanelControl.STOP;
							m_currentFrame = m_textureList.Count - 1;

							if (m_onLastFrame != null) m_onLastFrame ();
						}
						else
						{
							GuiImage.sprite = m_textureList [m_currentFrame];
						}

						m_elapseTime -= m_SPF;
						break;

					case AnimatedPanelControl.LOOP:
						m_currentFrame++;
						if (m_currentFrame >= m_textureList.Count)
						{
							m_currentFrame = 0;
						}

						GuiImage.sprite = m_textureList [m_currentFrame];

						m_elapseTime -= m_SPF;
						break;

					case AnimatedPanelControl.REVERSE:
						m_currentFrame--;
						if (m_currentFrame < 0)
						{
							m_control = AnimatedPanelControl.STOP;
							m_currentFrame = 0;
						}
						else
						{
							GuiImage.sprite = m_textureList [m_currentFrame];
						}

						m_elapseTime -= m_SPF;
						break;
				}
			}
		}
	}
}