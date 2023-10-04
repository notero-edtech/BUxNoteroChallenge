using System;
using System.Collections.Generic;
using ForieroEngine.Extensions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayerPrefs = ForieroEngine.PlayerPrefs;

public class ParentalLock : MonoBehaviour
{
    static readonly string PARENTAL_LOCK = "PARENTAL_LOCK";
    static bool? _active = null;
    public static bool active
    {
        get
        {
            if (_active == null) _active = PlayerPrefs.GetInt(PARENTAL_LOCK, 1) == 1 ? true : false;   
            return (bool)_active;
        }
        set
        {
            if (_active != value)
            {
                PlayerPrefs.SetInt(PARENTAL_LOCK, value ? 1 : 0);
                _active = value;
            }
        }
    }

    readonly string ASK_PARENT = "ASK_PARENT";
    readonly string TAP_ASCENDING_ORDER = "TAP_ASCENDING_ORDER";
    readonly string TAP_DESCENDING_ORDER = "TAP_DESCENDING_ORDER";
    readonly string OR = "OR";

    public static GameObject PREFAB_Parental_Control = null;
    public static GameObject parentalControl = null;

    public static void Show(int buttonsCount, Action<bool> finished)
    {
        Debug.Log("Parental Lock : Show");
        if (parentalControl)
        {
            Debug.LogError("Parental Control already in exists!");
            return;
        }

        if (active)
        {
            if (PREFAB_Parental_Control == null)
            {
                PREFAB_Parental_Control = Resources.Load<GameObject>("PREFAB_Parental_Lock");
            }

            if (PREFAB_Parental_Control == null)
            {
                Debug.LogError("ParentalControl prefab not found!");
                return;
            }

            parentalControl = Instantiate(PREFAB_Parental_Control, Vector3.zero, Quaternion.identity) as GameObject;
            ParentalLock parentalLock = parentalControl.GetComponent<ParentalLock>();
            parentalLock.numberOfButtons = buttonsCount > 2 ? buttonsCount : 3;
            parentalLock.finished = finished;
            parentalLock.CreateButtons();
        }
        else
        {
            if (finished != null)
            {
                finished(true);
            }
        }
    }

    public enum Direction
    {
        Ascending = 0,
        Descending = 1
    }

    public Direction direction = Direction.Ascending;

    public RectTransform panel;
    public RectTransform container;
    public TextMeshProUGUI textAskParent;
    public TextMeshProUGUI textTapOrder;
    public TextMeshProUGUI textOr;

    public int numberOfButtons = 4;
    public GameObject PREFAB_ParentalLockButton;

    List<ParentalLockButton> buttons = new List<ParentalLockButton>();

    List<int> tapNumbers = new List<int>();

    public Action<bool> finished;

    void Awake()
    {

    }

    void OnDestroy()
    {
        parentalControl = null;
    }

    List<int> GetNumbers()
    {
        List<int> result = new List<int>();
        for (int i = 0; i < numberOfButtons; i++)
        {
            result.Add(GetNumber(result));
        }
        return result;
    }

    int GetNumber(List<int> numbers)
    {
        int i = UnityEngine.Random.Range(1, 100);
        while (numbers.Contains(i))
        {
            i = UnityEngine.Random.Range(1, 100);
        }
        return i;
    }

    void CreateButtons()
    {
        buttons = new List<ParentalLockButton>();
        direction = (Direction)UnityEngine.Random.Range(0, 2);
        textAskParent.text = Lang.GetText("Foriero_Foriero", ASK_PARENT, "Ask your parent to continue!");
        textOr.text = Lang.GetText("Foriero", "OR", "or");
        switch (direction)
        {
            case Direction.Ascending:
                textTapOrder.text = "( " + Lang.GetText("Foriero_Foriero", TAP_ASCENDING_ORDER, "Tap " + direction.ToString().ToLower() + " order") + " )";
                break;
            case Direction.Descending:
                textTapOrder.text = "( " + Lang.GetText("Foriero_Foriero", TAP_DESCENDING_ORDER, "Tap " + direction.ToString().ToLower() + " order") + " )";
                break;
        }

        List<int> numbers = GetNumbers();
        while (IsAscending(numbers) || IsDescending(numbers))
        {
            numbers = GetNumbers();
        }

        for (int i = 0; i < numberOfButtons; i++)
        {
            GameObject buttonGO = Instantiate(PREFAB_ParentalLockButton);
            buttonGO.transform.SetParent(container);
            buttonGO.transform.localScale = Vector3.one;
            ParentalLockButton plb = buttonGO.GetComponent<ParentalLockButton>();
            plb.parentalLock = this;
            plb.number = numbers[i];
            buttons.Add(plb);
            Vector2 size = (plb.transform as RectTransform).GetSize();
            (plb.transform as RectTransform).anchoredPosition = new Vector2(
                -(size.x * numberOfButtons / 2f) + size.x / 2f + i * size.x,
                0
            );
        }
    }

    public void OnButtonClick(int nr, bool interactable)
    {
        if (interactable)
        {
            tapNumbers.Remove(nr);
            return;
        }
        else
        {
            tapNumbers.Add(nr);
        }

        if (tapNumbers.Count == numberOfButtons)
        {
            bool result = true;

            switch (direction)
            {
                case Direction.Ascending:
                    result = IsAscending(tapNumbers);
                    break;
                case Direction.Descending:
                    result = IsDescending(tapNumbers);
                    break;
            }

            Debug.Log("PARENTAL LOCK : " + result.ToString());
            finished?.Invoke(result);            
            Destroy(gameObject);
        }
    }

    public void OnCloseClick()
    {
        Debug.Log("PARENTAL LOCK : " + false.ToString());
        finished?.Invoke(false);
        Destroy(gameObject);
    }

    bool IsAscending(List<int> numbers)
    {
        bool result = true;
        int lastNumber = -1;
        foreach (int i in numbers)
        {
            if (lastNumber > i)
            {
                result = false;
            }
            lastNumber = i;
        }
        return result;
    }

    bool IsDescending(List<int> numbers)
    {
        bool result = true;
        int lastNumber = 100;
        foreach (int i in numbers)
        {
            if (lastNumber < i)
            {
                result = false;
            }
            lastNumber = i;
        }
        return result;
    }

}
