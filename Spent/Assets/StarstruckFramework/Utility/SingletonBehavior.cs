using UnityEngine;

namespace StarstruckFramework
{
	public abstract class SingletonBehavior<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T instance;

		/**
	       Returns the instance of this singleton.
	    */
		public static T Instance
		{
			get
			{
				if (instance == null)
				{
					instance = (T)FindObjectOfType(typeof(T));

					if (instance == null)
					{
						if (Debug.isDebugBuild)
							Debug.LogWarning("An instance of " + typeof(T) +
								" is needed in the scene, but there is none.");
					}
				}

				return instance;
			}
		}
	}
}