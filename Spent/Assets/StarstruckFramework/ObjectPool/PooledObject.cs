using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarstruckFramework
{
	public class PooledObject : MonoBehaviour
	{
		[SerializeField]
		protected ObjectPoolType mPoolType;

		public ObjectPoolType PoolType
		{
			get { return mPoolType; }
		}

		public virtual void Reinit ()
		{
			gameObject.SetActive (true);
		}

        public virtual void OnDestory()
        {

        }
    }
}