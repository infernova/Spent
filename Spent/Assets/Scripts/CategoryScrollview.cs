using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryScrollview : MonoBehaviour
{
    [SerializeField]
    private GameObject Template;
    private List<GameObject> mOptions = new List<GameObject>();

    public void UpdateOptions(List<string> options, bool isPrimaryCat)
    {
        foreach(GameObject gob in mOptions)
        {
            Destroy(gob);
        }

        mOptions = new List<GameObject>();

        GetComponent<ScrollRect>().content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
            Template.GetComponent<RectTransform>().rect.height * options.Count);

        for (int i = 0; i < options.Count; i++)
        {
            GameObject item = Instantiate(Template, GetComponent<ScrollRect>().content);
            item.GetComponent<CategoryScrollItem>().Init(i, options[i], isPrimaryCat);

            mOptions.Add(item);
        }
    }
}
