using UnityEngine;
using System.Collections;

namespace StarstruckFramework
{
    public abstract class Singleton<T> where T : new()
    {
        /// Singleton Pattern from http://csharpindepth.com/Articles/General/Singleton.aspx
        private readonly static T instance = new T();

        /**
	       Returns the instance of this singleton.
	    */
        public static T Instance { get { return instance; } }

        static Singleton()
        {
        }

        protected Singleton()
        {
        }
    }
}