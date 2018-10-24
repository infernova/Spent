using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace StarstruckFramework
{
	public class GUILiteText : GUILite
	{
		private TextMeshProUGUI m_text;

		private TextMeshProUGUI TextComp
		{
			get
			{
				if (m_text == null)
				{
					m_text = gameObject.GetComponent<TextMeshProUGUI>();
				}

				return m_text;
			}
		}

		public string Text
		{
			get { return TextComp.text; }
		}

		private bool m_hasRect;
		private bool m_isIconedText;

		public void SetText(string text)
		{
			TextComp.text = text;
		}

		public override void SetColor(Color color, bool overrideAlpha = false)
		{
            base.SetColor(color, overrideAlpha);

            if (!overrideAlpha)
            {
                color.a = TextComp.color.a;
            }

			TextComp.color = color;
		}

		public override void SetBrightness(float brightness)
		{
			base.SetBrightness(brightness);

            SetAlpha(brightness);
		}

		public override void SetAlpha(float alpha)
		{
			Color color = TextComp.color;
			color.a = alpha;
			TextComp.color = color;
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
                Color newColour = TextComp.color;
                newColour.a = startAlpha;
                TextComp.color = newColour;
            }
		}

        public override void FadeOut(float intensity,
                                     float startAlpha = -1f,
                                     bool isRippleDown = false,
                                     float endAlpha = 0.0f,
                                     bool isUnscaledTime = false)
        {
            base.FadeOut(intensity, startAlpha, isRippleDown, endAlpha, isUnscaledTime);

            Color newColor = TextComp.color;

            float alpha = System.Math.Abs(startAlpha - -1f) < Mathf.Epsilon ? newColor.a : startAlpha;

            newColor.a = alpha;
            TextComp.color = newColor;

            m_fadeIntensityUpper = alpha;
        }

		public override void Update()
		{
			base.Update();

			if (m_isAnimationPlaying)
			{
                bool nextState = false;

                Color colour = TextComp.color;
                float deltaTime = m_fadeIsUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

				switch (m_fade)
				{
					case FadeState.IN:
						if (colour.a < m_fadeEndAlpha)
						{
                            colour.a += (m_fadeIntensity * deltaTime);
							TextComp.color = colour;
						}
						else
						{
							colour.a = m_fadeEndAlpha;
							TextComp.color = colour;
							nextState = true;
						}
						break;

					case FadeState.OUT:
					
						if (colour.a > m_fadeEndAlpha)
						{
                            colour.a -= (m_fadeIntensity * deltaTime);
							TextComp.color = colour;
						}
						else
						{
							colour.a = m_fadeEndAlpha;
							TextComp.color = colour;
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