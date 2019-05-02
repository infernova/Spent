using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StarstruckFramework;

public class CategoryScrollview : MonoBehaviour
{
    [SerializeField]
    private ObjectPoolType TemplatePoolType;
    private List<GameObject> mOptions = new List<GameObject>();

    public void UpdateOptions(List<string> options, bool isPrimaryCat)
    {
        PoolMgr poolMgr = PoolMgr.Instance;

        foreach(GameObject gob in mOptions)
        {
            poolMgr.DestroyObj(gob);
        }

        mOptions = new List<GameObject>();

        GetComponent<ScrollRect>().content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                                                                     poolMgr.GetPooledObjRef(TemplatePoolType).GetComponent<RectTransform>().rect.height * options.Count);

        for (int i = 0; i < options.Count; i++)
        {
            GameObject item = poolMgr.InstantiateObj(TemplatePoolType, GetComponent<ScrollRect>().content);
            RectTransform itemRectTrans = item.GetComponent<RectTransform>();
            Vector2 min = itemRectTrans.offsetMin;
            min.x = 2.0f;
            itemRectTrans.offsetMin = min;

            Vector2 max = itemRectTrans.offsetMax;
            max.x = 2.0f;
            itemRectTrans.offsetMax = max;

            item.GetComponent<CategoryScrollItem>().Init(i, options[i], isPrimaryCat);

            mOptions.Add(item);
        }
    }
}
