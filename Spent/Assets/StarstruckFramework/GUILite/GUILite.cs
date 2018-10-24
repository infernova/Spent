using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace StarstruckFramework
{
    public class GUILite : MonoBehaviour
    {
        protected string m_id;
        private RectTransform m_rectTransform;

        public delegate void AnimationEndDelegate();
        public delegate void OffsetAnimationEndDelegate();
        public delegate void ScalingEndDelegate();
        public delegate void RotatingEndDelegate();

        protected AnimationEndDelegate m_onAnimationEnd;
        protected OffsetAnimationEndDelegate m_onOffsetAnimationEnd;
        protected ScalingEndDelegate m_onScaleEnd;
        protected RotatingEndDelegate m_onRotateEnd;
        private List<GUILiteAnimationData> m_guiAnimation = new List<GUILiteAnimationData>();
        private List<GUILiteOffsetAnimationData> m_guiOffsetAnimation = new List<GUILiteOffsetAnimationData>();
        private List<GUILiteScaleData> m_guiScaling = new List<GUILiteScaleData>();
        private List<GUILiteRotationData> m_guiRotating = new List<GUILiteRotationData>();
        private int mNumAnimations;
        private int mNumOffsetAnimations;
        private int mNumScalings;
        private int mNumRotations;
        protected float m_elapsedAnimationTime;
        protected float m_elapsedOffsetAnimationTime;
        protected float m_elapsedScalingTime;
        protected float m_elapsedRotatingTime;
        public WrapMode AnimationMode { get; set; }
        public WrapMode OffsetAnimationMode { get; set; }
        public WrapMode ScalingMode { get; set; }
        public WrapMode RotationMode { get; set; }

        protected bool m_isAnimationPlaying;

        protected float m_fadeIntensityUpper;

        protected AnimationEndDelegate m_onFadeEnd;
        protected FadeState m_fade = FadeState.NONE;
        public WrapMode FadeMode { get; set; }
        protected float m_fadeIntensity = 0.5f;
        protected float m_fadeEndAlpha = 1.0f;
        protected bool m_fadeIsUnscaledTime = false;

        public FadeState FadeState
        {
            get { return m_fade; }
        }

        public RectTransform RectTrans
        {
            get
            {
                if (m_rectTransform == null)
                    m_rectTransform = GetComponent<RectTransform>();

                return m_rectTransform;
            }
        }

        public string ID
        {
            get { return m_id; }
            set
            {
                m_id = value;
                gameObject.name = value;
            }
        }

        public virtual void Awake()
        {
            m_id = gameObject.name;
        }

        public void SetSize(Vector2 size)
        {
            RectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                size.x);
            RectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                size.y);
        }

        public virtual void SetAlpha(float alpha)
        {
        }

        public void SetAlphaOfSelfAndChildren(float alpha, bool ignoreSelf)
        {
            if (!ignoreSelf)
            {
                SetAlpha(alpha);
            }

            foreach (Transform child in RectTrans)
            {
                if (child.gameObject.GetComponent<GUILite>() != null)
                {
                    child.gameObject.GetComponent<GUILite>().SetAlphaOfSelfAndChildren(alpha, false);
                }
            }
        }

        public virtual void SetColor(Color color, bool overrideAlpha = false)
        {
        }

        public virtual void SetBrightness(float brightness)
        {
        }

        public void SetColorOfSelfAndChildren(Color color, bool ignoreSelf)
        {
            if (!ignoreSelf)
            {
                SetColor(color);
            }

            foreach (Transform child in RectTrans)
            {
                if (child.gameObject.GetComponent<GUILite>() != null)
                {
                    child.gameObject.GetComponent<GUILite>().SetColorOfSelfAndChildren(color, false);
                }
            }
        }

        public void SetBrightnessOfSelfAndChildren(float brightness, bool ignoreSelf)
        {
            if (!ignoreSelf)
            {
                SetBrightness(brightness);
            }

            foreach (Transform child in RectTrans)
            {
                if (child.gameObject.GetComponent<GUILite>() != null)
                {
                    child.gameObject.GetComponent<GUILite>().SetBrightnessOfSelfAndChildren(brightness, false);
                }
            }
        }

        protected bool AnimatedTo(Vector2 start,
                                  Vector2 end,
                                  float duration,
                                  float intensity,
                                  Easing.TYPE easing,
                                  float height,
                                  bool isUnscaledTime)
        {
            float deltaTime = isUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            m_elapsedAnimationTime += deltaTime;
            if (m_elapsedAnimationTime >= duration)
            {
                m_elapsedAnimationTime = 0.0f;

                if (easing == Easing.TYPE.BOUNCE)
                {
                    RectTrans.anchoredPosition = start;
                }
                else
                {
                    RectTrans.anchoredPosition = end;
                }

                return false;
            }
            else
            {
                Vector2 newPosition = new Vector2();
                float progress = m_elapsedAnimationTime * intensity;

                switch (easing)
                {
                    case Easing.TYPE.LERP:
                        newPosition.x = Mathf.Lerp(start.x, end.x, progress);
                        newPosition.y = Mathf.Lerp(start.y, end.y, progress);
                        break;
                    case Easing.TYPE.BERP:
                        newPosition.x = Easing.Berp(start.x, end.x, progress);
                        newPosition.y = Easing.Berp(start.y, end.y, progress);
                        break;
                    case Easing.TYPE.REVERSE_BERP:
                        newPosition.x = Easing.ReverseBerp(start.x, end.x, progress);
                        newPosition.y = Easing.ReverseBerp(start.y, end.y, progress);
                        break;
                    case Easing.TYPE.CLERP:
                        newPosition.x = Easing.Clerp(start.x, end.x, progress);
                        newPosition.y = Easing.Clerp(start.y, end.y, progress);
                        break;
                    case Easing.TYPE.HERMITE:
                        newPosition.x = Easing.Hermite(start.x, end.x, progress);
                        newPosition.y = Easing.Hermite(start.y, end.y, progress);
                        break;
                    case Easing.TYPE.SINERP:
                        newPosition.x = Easing.Sinerp(start.x, end.x, progress);
                        newPosition.y = Easing.Sinerp(start.y, end.y, progress);
                        break;
                    case Easing.TYPE.COSERP:
                        newPosition.x = Easing.Coserp(start.x, end.x, progress);
                        newPosition.y = Easing.Coserp(start.y, end.y, progress);
                        break;
                    case Easing.TYPE.QUADOUT:
                        newPosition.x = Easing.QuadOut(start.x, end.x, progress);
                        newPosition.y = Easing.QuadOut(start.y, end.y, progress);
                        break;
                    case Easing.TYPE.PARABOLIC:
                        newPosition = Easing.Parabola(start, end, height, progress);
                        break;
                    case Easing.TYPE.BOUNCE:
                        newPosition.x = start.x;
                        newPosition.y = Easing.Bounce(start.y, height, progress);
                        break;
                }

                RectTrans.anchoredPosition = newPosition;
                return true;
            }
        }

        protected bool OffsetAnimatedTo(Vector2 startMin,
                                        Vector2 startMax,
                                        Vector2 endMin,
                                        Vector2 endMax,
                                        float duration,
                                        float intensity,
                                        Easing.TYPE easing,
                                        bool isUnscaledTime)
        {
            float deltaTime = isUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            m_elapsedOffsetAnimationTime += deltaTime;
            if (m_elapsedOffsetAnimationTime >= duration)
            {
                m_elapsedOffsetAnimationTime = 0.0f;
                RectTrans.offsetMin = endMin;
                RectTrans.offsetMax = endMax;

                return false;
            }
            else
            {
                Vector2 newMin = new Vector2();
                Vector2 newMax = new Vector2();

                float progress = m_elapsedOffsetAnimationTime * intensity;

                switch (easing)
                {
                    case Easing.TYPE.LERP:
                        newMin.x = Mathf.Lerp(startMin.x, endMin.x, progress);
                        newMin.y = Mathf.Lerp(startMin.y, endMin.y, progress);

                        newMax.x = Mathf.Lerp(startMax.x, endMax.x, progress);
                        newMax.y = Mathf.Lerp(startMax.y, endMax.y, progress);
                        break;
                    case Easing.TYPE.BERP:
                        newMin.x = Easing.Berp(startMin.x, endMin.x, progress);
                        newMin.y = Easing.Berp(startMin.y, endMin.y, progress);

                        newMax.x = Easing.Berp(startMax.x, endMax.x, progress);
                        newMax.y = Easing.Berp(startMax.y, endMax.y, progress);
                        break;
                    case Easing.TYPE.REVERSE_BERP:
                        newMin.x = Easing.ReverseBerp(startMin.x, endMin.x, progress);
                        newMin.y = Easing.ReverseBerp(startMin.y, endMin.y, progress);

                        newMax.x = Easing.ReverseBerp(startMax.x, endMax.x, progress);
                        newMax.y = Easing.ReverseBerp(startMax.y, endMax.y, progress);
                        break;
                    case Easing.TYPE.CLERP:
                        newMin.x = Easing.Clerp(startMin.x, endMin.x, progress);
                        newMin.y = Easing.Clerp(startMin.y, endMin.y, progress);

                        newMax.x = Easing.Clerp(startMax.x, endMax.x, progress);
                        newMax.y = Easing.Clerp(startMax.y, endMax.y, progress);
                        break;
                    case Easing.TYPE.HERMITE:
                        newMin.x = Easing.Hermite(startMin.x, endMin.x, progress);
                        newMin.y = Easing.Hermite(startMin.y, endMin.y, progress);

                        newMax.x = Easing.Hermite(startMax.x, endMax.x, progress);
                        newMax.y = Easing.Hermite(startMax.y, endMax.y, progress);
                        break;
                    case Easing.TYPE.SINERP:
                        newMin.x = Easing.Sinerp(startMin.x, endMin.x, progress);
                        newMin.y = Easing.Sinerp(startMin.y, endMin.y, progress);

                        newMax.x = Easing.Sinerp(startMax.x, endMax.x, progress);
                        newMax.y = Easing.Sinerp(startMax.y, endMax.y, progress);
                        break;
                    case Easing.TYPE.COSERP:
                        newMin.x = Easing.Coserp(startMin.x, endMin.x, progress);
                        newMin.y = Easing.Coserp(startMin.y, endMin.y, progress);

                        newMax.x = Easing.Coserp(startMax.x, endMax.x, progress);
                        newMax.y = Easing.Coserp(startMax.y, endMax.y, progress);
                        break;
                    case Easing.TYPE.QUADOUT:
                        newMin.x = Easing.QuadOut(startMin.x, endMin.x, progress);
                        newMin.y = Easing.QuadOut(startMin.y, endMin.y, progress);

                        newMax.x = Easing.QuadOut(startMax.x, endMax.x, progress);
                        newMax.y = Easing.QuadOut(startMax.y, endMax.y, progress);
                        break;
                }

                RectTrans.offsetMin = newMin;
                RectTrans.offsetMax = newMax;
                return true;
            }
        }

        protected bool ScalingTo(Vector3 start,
                                 Vector3 end,
                                 float duration,
                                 float intensity,
                                 Easing.TYPE easing,
                                 bool isUnscaledTime)
        {
            float deltaTime = isUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            m_elapsedScalingTime += deltaTime;

            if (m_elapsedScalingTime >= duration)
            {
                m_elapsedScalingTime = 0.0f;

                RectTrans.localScale = end;

                return false;
            }
            else
            {
                Vector3 newScale = new Vector3();
                float progress = m_elapsedScalingTime * intensity;

                switch (easing)
                {
                    case Easing.TYPE.LERP:
                        newScale.x = Mathf.Lerp(start.x, end.x, progress);
                        newScale.y = Mathf.Lerp(start.y, end.y, progress);
                        newScale.z = Mathf.Lerp(start.y, end.y, progress);
                        break;
                    case Easing.TYPE.BERP:
                        newScale.x = Easing.Berp(start.x, end.x, progress);
                        newScale.y = Easing.Berp(start.y, end.y, progress);
                        newScale.z = Easing.Berp(start.y, end.y, progress);
                        break;
                    case Easing.TYPE.REVERSE_BERP:
                        newScale.x = Easing.ReverseBerp(start.x, end.x, progress);
                        newScale.y = Easing.ReverseBerp(start.y, end.y, progress);
                        newScale.z = Easing.ReverseBerp(start.y, end.y, progress);
                        break;
                    case Easing.TYPE.CLERP:
                        newScale.x = Easing.Clerp(start.x, end.x, progress);
                        newScale.y = Easing.Clerp(start.y, end.y, progress);
                        newScale.z = Easing.Clerp(start.y, end.y, progress);
                        break;
                    case Easing.TYPE.HERMITE:
                        newScale.x = Easing.Hermite(start.x, end.x, progress);
                        newScale.y = Easing.Hermite(start.y, end.y, progress);
                        newScale.z = Easing.Hermite(start.y, end.y, progress);
                        break;
                    case Easing.TYPE.SINERP:
                        newScale.x = Easing.Sinerp(start.x, end.x, progress);
                        newScale.y = Easing.Sinerp(start.y, end.y, progress);
                        newScale.z = Easing.Sinerp(start.y, end.y, progress);
                        break;
                    case Easing.TYPE.COSERP:
                        newScale.x = Easing.Coserp(start.x, end.x, progress);
                        newScale.y = Easing.Coserp(start.y, end.y, progress);
                        newScale.z = Easing.Coserp(start.y, end.y, progress);
                        break;
                    case Easing.TYPE.QUADOUT:
                        newScale.x = Easing.QuadOut(start.x, end.x, progress);
                        newScale.y = Easing.QuadOut(start.y, end.y, progress);
                        newScale.z = Easing.QuadOut(start.y, end.y, progress);
                        break;
                }

                RectTrans.localScale = newScale;

                return true;
            }
        }

        protected bool RotatingTo(float start,
                                  float end,
                                  float duration,
                                  float intensity,
                                  Easing.TYPE easing,
                                  bool isUnscaledTime = false)
        {
            float deltaTime = isUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            m_elapsedRotatingTime += deltaTime;

            if (m_elapsedRotatingTime >= duration)
            {
                m_elapsedRotatingTime = 0.0f;

                RectTrans.localEulerAngles = new Vector3(RectTrans.localEulerAngles.x,
                    RectTrans.localEulerAngles.y,
                    end);

                return false;
            }
            else
            {
                float currAngle = 0;
                float progress = m_elapsedRotatingTime * intensity;

                switch (easing)
                {
                    case Easing.TYPE.LERP:
                        currAngle = Mathf.Lerp(start, end, progress);
                        break;
                    case Easing.TYPE.BERP:
                        currAngle = Easing.Berp(start, end, progress);
                        break;
                    case Easing.TYPE.REVERSE_BERP:
                        currAngle = Easing.ReverseBerp(start, end, progress);
                        break;
                    case Easing.TYPE.CLERP:
                        currAngle = Easing.Clerp(start, end, progress);
                        break;
                    case Easing.TYPE.HERMITE:
                        currAngle = Easing.Hermite(start, end, progress);
                        break;
                    case Easing.TYPE.SINERP:
                        currAngle = Easing.Sinerp(start, end, progress);
                        break;
                    case Easing.TYPE.COSERP:
                        currAngle = Easing.Coserp(start, end, progress);
                        break;
                    case Easing.TYPE.QUADOUT:
                        currAngle = Easing.QuadOut(start, end, progress);
                        break;
                }

                RectTrans.localEulerAngles = new Vector3(RectTrans.localEulerAngles.x,
                    RectTrans.localEulerAngles.y,
                    currAngle);
                return true;
            }
        }

        public void OnFadeEnd(AnimationEndDelegate onFadeEnd)
        {
            m_onFadeEnd = onFadeEnd;
        }

        public void OnAnimationEnd(AnimationEndDelegate onAnimationEnd)
        {
            m_onAnimationEnd = onAnimationEnd;
        }

        public void OnOffsetAnimationEnd(OffsetAnimationEndDelegate onOffsetAnimationEnd)
        {
            m_onOffsetAnimationEnd = onOffsetAnimationEnd;
        }

        public void OnScaleEnd(ScalingEndDelegate onScaleEnd)
        {
            m_onScaleEnd = onScaleEnd;
        }

        public void OnRotateEnd(RotatingEndDelegate onRotateEnd)
        {
            m_onRotateEnd = onRotateEnd;
        }

        public bool HasAnimation()
        {
            return mNumAnimations > 0;
        }

        public bool HasOffsetAnimation()
        {
            return mNumOffsetAnimations > 0;
        }

        public bool HasScaling()
        {
            return mNumScalings > 0;
        }

        public bool HasRotation()
        {
            return mNumRotations > 0;
        }

        public void AddAnimation(Vector2 targetPos,
                                 float duration,
                                 Easing.TYPE easing = Easing.TYPE.LERP,
                                 float height = 0,
                                bool isUnscaledTime = false)
        {
            m_isAnimationPlaying = true;

            m_guiAnimation.Add(new GUILiteAnimationData(RectTrans.anchoredPosition,
                                                        targetPos,
                                                        duration,
                                                        easing,
                                                        height,
                                                        isUnscaledTime));
            mNumAnimations++;
        }

        public void AddAnimation(Vector2 fromPos,
                                 Vector2 targetPos,
                                 float duration,
                                 Easing.TYPE easing = Easing.TYPE.LERP,
                                 float height = 0,
                                 bool isUnscaledTime = false)
        {
            m_isAnimationPlaying = true;

            if (mNumAnimations == 0)
            {
                RectTrans.anchoredPosition = fromPos;
            }

            m_guiAnimation.Add(new GUILiteAnimationData(fromPos,
                                                        targetPos,
                                                        duration,
                                                        easing,
                                                        height,
                                                        isUnscaledTime));
            mNumAnimations++;
        }

        public void AddOffsetAnimation(Vector2 targetMinPos,
                                       Vector2 targetMaxPos,
                                       float duration,
                                       Easing.TYPE easing = Easing.TYPE.LERP,
                                       bool isUnscaledTime = false)
        {
            m_isAnimationPlaying = true;

            m_guiOffsetAnimation.Add(new GUILiteOffsetAnimationData(RectTrans.offsetMin,
                                                                    RectTrans.offsetMax,
                                                                    targetMinPos,
                                                                    targetMaxPos,
                                                                    duration,
                                                                    easing,
                                                                    isUnscaledTime));
            mNumOffsetAnimations++;
        }

        public void AddOffsetAnimation(Vector2 fromMinPos,
                                 Vector2 fromMaxPos,
                                 Vector2 targetMinPos,
                                 Vector2 targetMaxPos,
                                 float duration,
                                       Easing.TYPE easing = Easing.TYPE.LERP,
                                bool isUnscaledTime = false)
        {
            m_isAnimationPlaying = true;

            if (mNumOffsetAnimations == 0)
            {
                RectTrans.offsetMin = fromMinPos;
                RectTrans.offsetMax = fromMaxPos;
            }

            m_guiOffsetAnimation.Add(new GUILiteOffsetAnimationData(fromMinPos,
                                                                    fromMaxPos,
                                                                    targetMinPos,
                                                                    targetMaxPos,
                                                                    duration,
                                                                    easing,
                                                        isUnscaledTime));
            mNumOffsetAnimations++;
        }

        public void AddScaling(Vector3 start,
                               Vector3 end,
                               float duration,
                               Easing.TYPE easing = Easing.TYPE.LERP,
                                bool isUnscaledTime = false)
        {
            m_isAnimationPlaying = true;

            if (mNumScalings == 0)
            {
                RectTrans.localScale = start;
            }

            m_guiScaling.Add(new GUILiteScaleData(start, end, duration, easing, isUnscaledTime));
            mNumScalings++;
        }

        public void AddScaling(Vector3 end,
                               float duration,
                               Easing.TYPE easing = Easing.TYPE.LERP,
                                bool isUnscaledTime = false)
        {
            m_isAnimationPlaying = true;
            m_guiScaling.Add(new GUILiteScaleData(RectTrans.localScale, end, duration, easing, isUnscaledTime));
            mNumScalings++;
        }

        public void AddScaling(float start,
                               float end,
                               float duration,
                               Easing.TYPE easing = Easing.TYPE.LERP,
                                bool isUnscaledTime = false)
        {
            m_isAnimationPlaying = true;
            Vector3 realStart = RectTrans.localScale * start;
            Vector3 realEnd = RectTrans.localScale * end;

            if (mNumScalings == 0)
            {
                RectTrans.localScale = realStart;
            }

            m_guiScaling.Add(new GUILiteScaleData(realStart, realEnd, duration, easing, isUnscaledTime));
            mNumScalings++;

        }

        public void AddRotation(float start,
                                float end,
                                float duration,
                                Easing.TYPE easing = Easing.TYPE.LERP,
                                bool isUnscaledTime = false)
        {
            m_isAnimationPlaying = true;

            if (mNumRotations == 0)
            {
                RectTrans.localEulerAngles = new Vector3(RectTrans.localEulerAngles.x,
                    RectTrans.localEulerAngles.y,
                    start);
            }

            m_guiRotating.Add(new GUILiteRotationData(start, end, duration, easing, isUnscaledTime));
            mNumRotations++;
        }

        public virtual void FadeOut(float intensity,
                                    float startAlpha = -1f,
                                    bool isRippleDown = false,
                                    float endAlpha = 0.0f,
                                    bool isUnscaledTime = false)
        {
            m_isAnimationPlaying = true;

            m_fade = FadeState.OUT;
            m_fadeIntensity = Mathf.Abs(intensity);
            m_fadeEndAlpha = endAlpha;
            m_fadeIsUnscaledTime = isUnscaledTime;

            if (isRippleDown)
            {
                foreach (Transform childTrans in RectTrans)
                {
                    if (childTrans.gameObject.GetComponent<GUILite>() != null)
                    {
                        childTrans.gameObject.GetComponent<GUILite>().FadeOut(intensity,
                                                                              startAlpha,
                                                                              isRippleDown,
                                                                              endAlpha,
                                                                              isUnscaledTime);
                        childTrans.gameObject.GetComponent<GUILite>().FadeMode = FadeMode;
                    }
                }
            }
        }

        public virtual void FadeIn(float intensity,
                                   float endAlpha = 1f,
                                   bool isRippleDown = false,
                                   float startAlpha = 0.0f,
                                   bool isUnscaledTime = false)
        {
            m_isAnimationPlaying = true;

            m_fade = FadeState.IN;
            m_fadeIntensity = Mathf.Abs(intensity);
            m_fadeEndAlpha = endAlpha;

            m_fadeIntensityUpper = endAlpha;
            m_fadeIsUnscaledTime = isUnscaledTime;

            if (isRippleDown)
            {
                foreach (Transform childTrans in RectTrans)
                {
                    if (childTrans.gameObject.GetComponent<GUILite>() != null)
                    {
                        childTrans.gameObject.GetComponent<GUILite>().FadeIn(intensity,
                                                                             endAlpha,
                                                                             isRippleDown,
                                                                             startAlpha,
                                                                             isUnscaledTime);
                        childTrans.gameObject.GetComponent<GUILite>().FadeMode = FadeMode;
                    }
                }
            }
        }

        public void StopAllAnimations(bool isRippleDown = false)
        {
            m_elapsedAnimationTime = 0.0f;
            m_elapsedOffsetAnimationTime = 0.0f;
            m_elapsedRotatingTime = 0.0f;
            m_elapsedScalingTime = 0.0f;

            m_isAnimationPlaying = false;
            m_guiAnimation.Clear();
            m_guiOffsetAnimation.Clear();
            m_guiRotating.Clear();
            m_guiScaling.Clear();

            mNumAnimations = 0;
            mNumOffsetAnimations = 0;
            mNumRotations = 0;
            mNumScalings = 0;

            m_fade = FadeState.NONE;
            AnimationMode = WrapMode.Once;
            OffsetAnimationMode = WrapMode.Once;
            RotationMode = WrapMode.Once;

            if (isRippleDown)
            {
                foreach (Transform childTrans in RectTrans)
                {
                    if (childTrans.gameObject.GetComponent<GUILite>() != null)
                    {
                        childTrans.gameObject.GetComponent<GUILite>().StopAllAnimations(isRippleDown);
                    }
                }
            }
        }

        public void PauseAllAnimations()
        {
            m_isAnimationPlaying = false;
        }

        public void ResumeAllAnimations()
        {
            m_isAnimationPlaying = true;
        }

        public virtual void Update()
        {
            if (mNumAnimations > 0 && m_isAnimationPlaying)
            {
                GUILiteAnimationData orig = m_guiAnimation[0];
                if (!AnimatedTo(orig.StartPosition,
                                orig.EndPosition,
                                orig.AnimationDuration,
                                orig.AnimationIntensity,
                                orig.Easing,
                                orig.Height,
                                orig.IsUnscaledTime))
                {
                    switch (AnimationMode)
                    {
                        case WrapMode.Loop:
                            // add this to end of list, pop
                            m_guiAnimation.Add(orig);
                            m_guiAnimation.RemoveAt(0);
                            break;

                        case WrapMode.PingPong:
                            GUILiteAnimationData reverse = new GUILiteAnimationData(orig.EndPosition,
                                                                                    orig.StartPosition,
                                                                                    orig.AnimationDuration,
                                                                                    orig.Easing,
                                                                                    orig.Height,
                                                                                    orig.IsUnscaledTime);
                            m_guiAnimation.Add(reverse);
                            m_guiAnimation.RemoveAt(0);
                            break;

                        default:
                            // pop this animation
                            m_guiAnimation.RemoveAt(0);
                            mNumAnimations--;

                            // no more animation queued and animation end delegate was set
                            if (m_onAnimationEnd != null && mNumAnimations == 0)
                            {
                                m_onAnimationEnd();
                            }
                            break;
                    }
                }
            }

            if (mNumOffsetAnimations > 0 && m_isAnimationPlaying)
            {
                GUILiteOffsetAnimationData orig = m_guiOffsetAnimation[0];
                if (!OffsetAnimatedTo(orig.StartMinPosition,
                                      orig.StartMaxPosition,
                                      orig.EndMinPosition,
                                      orig.EndMaxPosition,
                                      orig.AnimationDuration,
                                      orig.AnimationIntensity,
                                      orig.Easing,
                                      orig.IsUnscaledTime))
                {
                    switch (AnimationMode)
                    {
                        case WrapMode.Loop:
                            // add this to end of list, pop
                            m_guiOffsetAnimation.Add(orig);
                            m_guiOffsetAnimation.RemoveAt(0);
                            break;

                        case WrapMode.PingPong:
                            GUILiteOffsetAnimationData reverse = new GUILiteOffsetAnimationData(orig.EndMinPosition,
                                                                                                orig.EndMaxPosition,
                                                                                                orig.StartMinPosition,
                                                                                                orig.StartMaxPosition,
                                                                                                orig.AnimationDuration,
                                                                                                orig.Easing,
                                                                                                orig.IsUnscaledTime);
                            m_guiOffsetAnimation.Add(reverse);
                            m_guiOffsetAnimation.RemoveAt(0);
                            break;

                        default:
                            // pop this animation
                            m_guiOffsetAnimation.RemoveAt(0);
                            mNumOffsetAnimations--;

                            // no more animation queued and animation end delegate was set
                            if (m_onOffsetAnimationEnd != null && mNumOffsetAnimations == 0)
                            {
                                m_onOffsetAnimationEnd();
                            }
                            break;
                    }
                }
            }

            if (mNumScalings > 0 && m_isAnimationPlaying)
            {
                GUILiteScaleData orig = m_guiScaling[0];
                if (!ScalingTo(orig.StartScale,
                               orig.EndScale,
                               orig.ScaleDuration,
                               orig.ScaleIntensity,
                               orig.Easing,
                               orig.IsUnscaledTime))
                {
                    switch (ScalingMode)
                    {
                        case WrapMode.Loop:
                            // add this to end of list, pop
                            m_guiScaling.Add(orig);
                            m_guiScaling.RemoveAt(0);
                            break;

                        case WrapMode.PingPong:
                            GUILiteScaleData reverse = new GUILiteScaleData(orig.EndScale,
                                                                            orig.StartScale,
                                                                            orig.ScaleDuration,
                                                                            orig.Easing,
                                                                            orig.IsUnscaledTime);

                            m_guiScaling.Add(reverse);
                            m_guiScaling.RemoveAt(0);
                            break;

                        default:
                            // pop this animation
                            m_guiScaling.RemoveAt(0);
                            mNumScalings--;

                            // no more animation queued and animation end delegate was set
                            if (m_onScaleEnd != null && m_guiScaling.Count == 0)
                            {
                                m_onScaleEnd();
                            }
                            break;
                    }
                }
            }

            if (mNumRotations > 0 && m_isAnimationPlaying)
            {
                GUILiteRotationData orig = m_guiRotating[0];
                if (!RotatingTo(orig.StartgAngle,
                                orig.EndAngle,
                                orig.Duration,
                                orig.Intensity,
                                orig.Easing,
                                orig.IsUnscaledTime))
                {
                    switch (RotationMode)
                    {
                        case WrapMode.Loop:
                            // add this to end of list, pop
                            m_guiRotating.Add(orig);
                            m_guiRotating.RemoveAt(0);
                            break;

                        case WrapMode.PingPong:
                            GUILiteRotationData reverse = new GUILiteRotationData(orig.EndAngle,
                                                                                  orig.StartgAngle,
                                                                                  orig.Duration,
                                                                                  orig.Easing,
                                                                                  orig.IsUnscaledTime);

                            m_guiRotating.Add(reverse);
                            m_guiRotating.RemoveAt(0);
                            break;

                        default:
                            m_guiRotating.RemoveAt(0);
                            mNumRotations--;

                            // no more animation queued and animation end delegate was set
                            if (m_onRotateEnd != null && mNumRotations == 0)
                            {
                                m_onRotateEnd();
                            }
                            break;
                    }
                }
            }
        }
    }

    public struct GUILiteAnimationData
    {
        public Vector2 StartPosition;
        public Vector2 EndPosition;
        public float AnimationDuration;
        public float AnimationIntensity;
        public Easing.TYPE Easing;
        public float Height;
        public bool IsUnscaledTime;

        public GUILiteAnimationData(Vector2 startPos,
                                    Vector2 endPos,
                                    float duration,
                                    Easing.TYPE easing,
                                    float height = 0,
                                    bool isUnscaledTime = false)
        {
            StartPosition = startPos;
            EndPosition = endPos;
            AnimationDuration = duration;
            AnimationIntensity = 1.0f / duration;
            Easing = easing;
            Height = height;
            IsUnscaledTime = isUnscaledTime;
        }
    }

    public struct GUILiteOffsetAnimationData
    {
        public Vector2 StartMinPosition;
        public Vector2 StartMaxPosition;
        public Vector2 EndMinPosition;
        public Vector2 EndMaxPosition;
        public float AnimationDuration;
        public float AnimationIntensity;
        public Easing.TYPE Easing;
        public bool IsUnscaledTime;

        public GUILiteOffsetAnimationData(Vector2 startMinOffset,
                                          Vector2 startMaxOffset,
                                          Vector2 endMinOffset,
                                          Vector2 endMaxOffset,
                                          float duration,
                                          Easing.TYPE easing,
                                          bool isUnscaledTime = false)
        {
            StartMinPosition = startMinOffset;
            StartMaxPosition = startMaxOffset;
            EndMinPosition = endMinOffset;
            EndMaxPosition = endMaxOffset;
            AnimationDuration = duration;
            AnimationIntensity = 1.0f / duration;
            Easing = easing;
            IsUnscaledTime = isUnscaledTime;
        }
    }

    public struct GUILiteScaleData
    {
        public Vector2 StartScale;
        public Vector2 EndScale;
        public float ScaleDuration;
        public float ScaleIntensity;
        public Easing.TYPE Easing;
        public bool IsUnscaledTime;

        public GUILiteScaleData(Vector3 start, Vector3 end, float duration, Easing.TYPE easing, bool isUnscaledTime = false)
        {
            StartScale = start;
            EndScale = end;
            ScaleDuration = duration;
            ScaleIntensity = 1.0f / duration;
            Easing = easing;
            IsUnscaledTime = isUnscaledTime;
        }
    }

    public struct GUILiteRotationData
    {
        public float StartgAngle;
        public float EndAngle;
        public float Duration;
        public float Intensity;
        public Easing.TYPE Easing;
        public bool IsUnscaledTime;

        public GUILiteRotationData(float start, float end, float duration, Easing.TYPE easing, bool isUnscaledTime = false)
        {
            StartgAngle = start;
            EndAngle = end;
            Duration = duration;
            Intensity = 1.0f / duration;
            Easing = easing;
            IsUnscaledTime = isUnscaledTime;
        }
    }

    public enum FadeState
    {
        NONE,
        IN,
        OUT,
        CROSSFADE
    }
}