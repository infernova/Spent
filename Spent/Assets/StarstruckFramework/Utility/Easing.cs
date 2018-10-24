using UnityEngine;

namespace StarstruckFramework
{
	public class Easing
	{
		public enum TYPE
		{
			LERP,
			HERMITE,
			SINERP,
			COSERP,
			BERP,
            REVERSE_BERP,
			CLERP,
			QUADOUT,
			PARABOLIC,
			BOUNCE
		}

		/// This method will interpolate while easing in and out at the limits.
		public static float Hermite (float start, float end, float value)
		{
			return Mathf.Lerp (start, end, value * value * (3.0f - 2.0f * value));
		}

		/// Short for 'sinusoidal interpolation', this method will interpolate while easing around the end, when value is near one.
		public static float Sinerp (float start, float end, float value)
		{
			return Mathf.Lerp (start, end, Mathf.Sin (value * Mathf.PI * 0.5f));
		}

		/// Short for 'sinusoidal interpolation', this method will interpolate while easing around the end, when value is near one.
		public static float QuadOut (float start, float end, float value)
		{
			value /= 1;
			return (end - start) * value * value + start;
		}

		/// Similar to Sinerp, except it eases in, when value is near zero, instead of easing out (and uses cosine instead of sine).
		public static float Coserp (float start, float end, float value)
		{
			return Mathf.Lerp (start, end, 1.0f - Mathf.Cos (value * Mathf.PI * 0.5f));
		}

		/**
		 * Short for 'boing-like interpolation', this method will first overshoot, 
		 * then waver back and forth around the end value before coming to a rest.
		 */
		public static float Berp (float start, float end, float value)
		{
			value = Mathf.Clamp01 (value);
			value = (Mathf.Sin (value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow (1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
			return start + (end - start) * value;
		}

        public static float ReverseBerp(float start, float end, float value)
        {
            value = 1 - value;
            value = Mathf.Clamp01(value);
            value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
            return end + (start - end) * value;
        }

		/*
	      * CLerp - Circular Lerp - is like lerp but handles the wraparound from 0 to 360.
	      * This is useful when interpolating eulerAngles and the object
	      * crosses the 0/360 boundary.  The standard Lerp function causes the object
	      * to rotate in the wrong direction and looks stupid. Clerp fixes that.
	      */
		public static float Clerp (float start, float end, float value)
		{
			float min = 0.0f;
			float max = 360.0f;
			float half = Mathf.Abs ((max - min) / 2.0f);
			float retval = 0.0f;
			float diff = 0.0f;

			if ((end - start) < -half) {
				diff = ((max - start) + end) * value;
				retval = start + diff;
			} else if ((end - start) > half) {
				diff = -((max - end) + start) * value;
				retval = start + diff;
			} else {
				retval = start + (end - start) * value;
			}

			return retval;
		}

		public static Vector2 Parabola (Vector2 start, Vector2 end, float height, float t)
		{
			float parabolicT = -((t * 2) - 1) * ((t * 2) - 1) + 1;
			Vector2 travelVec = end - start;
			Vector2 normalisedNormal = new Vector2 (travelVec.y, -travelVec.x).normalized;

			return Vector2.Lerp (start, end, t) + (normalisedNormal * parabolicT * height);
		}

		public static float Bounce (float start, float height, float t)
		{
			float displacement = Mathf.Sin (Mathf.PI * t) * height;
			return start + displacement;
		}
	}
}