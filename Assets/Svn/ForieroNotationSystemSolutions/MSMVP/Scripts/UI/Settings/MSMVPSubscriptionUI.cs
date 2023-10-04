/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if FORIERO_INAPP
using ForieroEngine.Purchasing;
#endif

public class MSMVPSubscriptionUI : MonoBehaviour
{
    public Button buyMonthButton;
    public Button restoreMonthButton;
    public Button buyThreeMonthsButton;
    public Button restoreThreeMonthsButton;
    public Button buyYearButton;
    public Button restoreYearButton;

    public TextMeshProUGUI infoText;
    
    private bool _purchasedMonth = false;
    private bool _purchasedThreeMonths = false;
    private bool _purchasedYear = false;

    private void Awake()
    {
    #if FORIERO_INAPP
        _purchasedMonth = Store.Purchased("month");
        _purchasedThreeMonths = Store.Purchased("threemonths");
        _purchasedYear = Store.Purchased("year");
    #endif
        HookButtons();
    }

    private void HookButtons()
    {
        if (_purchasedMonth || _purchasedThreeMonths || _purchasedYear)
        {
            buyMonthButton.enabled = false;
            buyThreeMonthsButton.enabled = false;
            buyYearButton.enabled = false;

            if (_purchasedMonth) infoText.text = "You are currently on the 'monthly' subscription plan!";
            else if (buyThreeMonthsButton) infoText.text = "You are currently on the 'three months' subscription plan!";
            else if (buyYearButton) infoText.text = "You are currently on the 'yearly' subscription plan!";
        }

        buyMonthButton.onClick.AddListener(OnBuyMonth);
        restoreMonthButton.onClick.AddListener(OnRestoreMonth);

        buyThreeMonthsButton.onClick.AddListener(OnBuyThreeMonths);
        restoreThreeMonthsButton.onClick.AddListener(OnRestoreThreeMonths);

        buyYearButton.onClick.AddListener(OnBuyYear);
        restoreYearButton.onClick.AddListener(OnRestoreYear);
    }

    private void OnBuyYear()
    {
    }

    private void OnRestoreYear()
    {
    }

    private void OnRestoreThreeMonths()
    {
    }

    private void OnBuyThreeMonths()
    {
    }

    private void OnRestoreMonth()
    {
    }

    private void OnBuyMonth()
    {
    }
}
