using StarstruckFramework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpenditureListItem : SelectableListItem
{
    [SerializeField]
    private Button mButton;
    [SerializeField]
    private TextMeshProUGUI mDate;
    [SerializeField]
    private TextMeshProUGUI mAmount;
    [SerializeField]
    private TextMeshProUGUI mPrimaryCat;
    [SerializeField]
    private TextMeshProUGUI mSecondaryCat;

	private void Start()
	{
        mButton.onClick.AddListener(() => MainScreen.Instance.SelectExpenditureItem(mIndex));
	}

	public override void ResetPosition(int index)
	{
        base.ResetPosition(index);

        ExpenditureItem item = MainScreen.Instance.DisplayedItems[index];
        mDate.text = item.Date.ToString("dd/MM/yy");
        mAmount.text = "$" + item.Amount.ToString("0.00");
        mPrimaryCat.text = item.PrimaryCategory;
        mSecondaryCat.text = item.SecondaryCategory;

        SetAsSelected(index == MainScreen.Instance.EditIndex);
	}
}
