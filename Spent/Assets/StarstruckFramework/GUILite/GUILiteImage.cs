using UnityEngine;
using UnityEngine.UI;

namespace StarstruckFramework
{
	public class GUILiteImage : GUILite
	{
		private Image m_guiImage;

		public Image GuiImage
		{
			get
			{
				if (m_guiImage == null)
					m_guiImage = GetComponent<Image>();
				return m_guiImage;
			}
		}

		protected Color m_originalColor;

		protected GameObject guiPointer;

		public override void Awake()
		{
			base.Awake();
		}

        public override void SetAlpha(float alpha)
        {
            Color colour = GuiImage.color;
            colour.a = alpha;
            GuiImage.color = colour;
        }

        public override void FadeIn(float intensity,
                                    float endAlpha = 1f,
                                    bool isRippleDown = false,
                                    float startAlpha = 0.0f,
                                    bool isUnscaledTime = false)
        {
            base.FadeIn(intensity, endAlpha, isRippleDown, startAlpha, isUnscaledTime);

            if (startAlpha >= 0.0f)
            {
                Color currColour = GuiImage.color;
                currColour.a = startAlpha;
                GuiImage.color = currColour;
            }
        }

        public override void FadeOut(float intensity,
                                     float startAlpha = -1f,
                                     bool isRippleDown = false,
                                     float endAlpha = 0.0f,
                                     bool isUnscaledTime = false)
        {
            base.FadeOut(intensity, startAlpha, isRippleDown, endAlpha, isUnscaledTime);

            Color currColor = GuiImage.color;

            float alpha = System.Math.Abs(startAlpha - -1f) < Mathf.Epsilon ? currColor.a : startAlpha;

            currColor.a = alpha;
            GuiImage.color = currColor;

            m_fadeIntensityUpper = alpha;
        }

        public override void SetColor(Color color, bool overrideAlpha = false)
		{
            base.SetColor(color, overrideAlpha);

            if (!overrideAlpha)
            {
                color.a = GuiImage.color.a;
            }

			GuiImage.color = color;
		}

		public override void SetBrightness(float brightness)
		{
			base.SetBrightness(brightness);

			GuiImage.color = new Color(brightness, brightness, brightness, 1.0f);
		}

		public override void Update()
		{
			base.Update();

			if (m_isAnimationPlaying)
			{
                bool nextState = false;
				Color fadeColour = GuiImage.color;

                float deltaTime = m_fadeIsUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

				switch (m_fade)
				{
					case FadeState.IN:
                        if (fadeColour.a < m_fadeEndAlpha)
						{
                            fadeColour.a += (m_fadeIntensity * deltaTime);
							GuiImage.color = fadeColour;
						}
						else
						{
							fadeColour.a = m_fadeEndAlpha;
							GuiImage.color = fadeColour;
							nextState = true;
						}
						break;

					case FadeState.OUT:
						if (fadeColour.a > m_fadeEndAlpha)
						{
                            fadeColour.a -= (m_fadeIntensity * deltaTime);
							GuiImage.color = fadeColour;
						}
						else
						{
							fadeColour.a = m_fadeEndAlpha;
							GuiImage.color = fadeColour;
							nextState = true;
						}
						break;

					case FadeState.NONE:
						break;
				}

				if (nextState)
				{
					switch (FadeMode)
					{
						case WrapMode.Loop:
							switch (m_fade)
							{
								case FadeState.IN:
									FadeIn(m_fadeIntensity, m_fadeIntensityUpper, false, 0, m_fadeIsUnscaledTime);
									break;

								case FadeState.OUT:
									FadeOut(m_fadeIntensity, m_fadeIntensityUpper, false, 0, m_fadeIsUnscaledTime);
									break;
							}
							break;

						case WrapMode.PingPong:
							switch (m_fade)
							{
								case FadeState.IN:
									FadeOut(m_fadeIntensity, m_fadeIntensityUpper, false, 0, m_fadeIsUnscaledTime);
									break;

								case FadeState.OUT:
									FadeIn(m_fadeIntensity, m_fadeIntensityUpper, false, 0, m_fadeIsUnscaledTime);
									break;
							}
							break;

						default:
							m_fade = FadeState.NONE;
							if (m_onFadeEnd != null)
							{
								m_onFadeEnd();
							}
							break;
					}
				}
			}
		}
	}
}