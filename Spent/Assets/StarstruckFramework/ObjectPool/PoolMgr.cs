using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarstruckFramework
{
	public enum ObjectPoolType
	{
		UNDEFINED,
        PIE_SLICE,
        CATEGORY_DAILY_SET,
        CATEGORY_DAILY_ITEM,
        DAILY_SET,
        DAILY_ITEM,
        RECURRING_ITEM,
        COST_BREAKDOWN_ITEM,
        BAR_CHART_ITEM,
        CATEGORY_INPUT_ITEM,
        DESCRIPTION_ITEM
    }

	public class PoolMgr : SingletonBehavior<PoolMgr>
	{
		[SerializeField]
		private GameObject mPoolContainer;
		private List<GameObject> mIndividualPoolContainers;
		private List<GameObject>[] mPooledObjects;

        [System.Serializable]
        public class PooledObjectDictionary : SerializableDictionary<ObjectPoolType, GameObject> {}

		[SerializeField]
        private PooledObjectDictionary PooledObjectTemplates;

		void Awake()
		{
			string[] pooledObjNames = System.Enum.GetNames(typeof(ObjectPoolType));
			mPooledObjects = new List<GameObject>[pooledObjNames.Length];
			mIndividualPoolContainers = new List<GameObject>();
			mIndividualPoolContainers.Add(null);

			for (int i = 1; i < pooledObjNames.Length; i++)
			{
				mPooledObjects[i] = new List<GameObject>();

				GameObject pool = new GameObject(pooledObjNames[i]);
				pool.transform.SetParent(mPoolContainer.transform);
				mIndividualPoolContainers.Add(pool);
			}
		}

        public GameObject GetPooledObjRef(ObjectPoolType type)
        {
            return PooledObjectTemplates[type];
        }

		public GameObject InstantiateObj(ObjectPoolType type, Vector3 pos, Transform parent, bool useWorldPos = false)
		{
			if (mPooledObjects[(int)type].Count > 0)
			{
				GameObject gob = mPooledObjects[(int)type][0];
				mPooledObjects[(int)type].RemoveAt(0);

                if (gob.transform.parent != mIndividualPoolContainers[(int)type].transform)
                {
                    return InstantiateObj(type, pos, parent, useWorldPos);
                }
                
				gob.transform.SetParent(parent);

                if (useWorldPos)
                {
                    gob.transform.position = pos;
                }
                else
                {
                    gob.transform.localPosition = pos;
                }

				gob.GetComponent<PooledObject>().Reinit();

				return gob;
			}
			else
			{
                GameObject gob = Instantiate(PooledObjectTemplates[type],
                    pos,
                    Quaternion.identity,
                    parent);

                if (useWorldPos)
                {
                    gob.transform.position = pos;
                }

                return gob;
			}
		}

        public GameObject InstantiateObj(ObjectPoolType type, Transform parent)
        {
            if (mPooledObjects[(int)type].Count > 0)
            {
                GameObject gob = mPooledObjects[(int)type][0];
                mPooledObjects[(int)type].RemoveAt(0);

                if (gob.transform.parent != mIndividualPoolContainers[(int)type].transform)
                {
                    return InstantiateObj(type, parent);
                }

                gob.transform.SetParent(parent);

                gob.GetComponent<PooledObject>().Reinit();

                return gob;
            }
            else
            {
                return Instantiate(PooledObjectTemplates[type], parent);
            }
        }

		public void DestroyObj(GameObject gob)
		{
            if (gob == null)
            {
                return;
            }

            PooledObject comp = gob.GetComponent<PooledObject>();
			if (comp != null && comp.PoolType != ObjectPoolType.UNDEFINED)
			{
                gob.SetActive(false);
				gob.transform.SetParent(mIndividualPoolContainers[(int)comp.PoolType].transform);
				mPooledObjects[(int)comp.PoolType].Add(gob);
                comp.OnDestory();
			}
			else
			{
				Destroy(gob);
			}
		}
	}
}