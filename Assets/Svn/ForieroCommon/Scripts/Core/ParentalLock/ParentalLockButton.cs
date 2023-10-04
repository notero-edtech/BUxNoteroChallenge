using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ParentalLockButton : MonoBehaviour
{
    public ParentalLock parentalLock;
    public Button button;
    public TextMeshProUGUI text;

    bool interactable = true;
    int _number = 0;

    public int number
    {
        set
        {
            _number = value;
            text.text = _number.ToString();
        }
        get { return _number; }
    }

    public void OnClick()
    {
        interactable = !interactable;
        button.image.color = interactable ? button.colors.normalColor : button.colors.disabledColor;        
        parentalLock.OnButtonClick(number, interactable);
    }
}
