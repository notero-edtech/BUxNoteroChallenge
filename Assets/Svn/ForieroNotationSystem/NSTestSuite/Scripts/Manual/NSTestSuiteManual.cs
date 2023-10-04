/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Extensions;
using UnityEngine;
using UnityEngine.UI;

public partial class NSTestSuiteManual : MonoBehaviour
{
    NSBehaviour nsBehaviour { get { return NSBehaviour.instance; } }
    NSDebug nsDebug { get { return NSBehaviour.instance.nsDebug; } }

    public RectTransform manualWindow;
    public Dropdown staveLinesDropdown;
    public Dropdown clefDropdown;
    public Dropdown keySignatureDropdown;
    public Dropdown timeSignatureDropdown;

    public InputField renderTestCountInputField;

    public RectTransform testContainer;

    public GameObject PREFAB_TestToggle;

    //[HideInInspector]
    public StaveEnum staveEnum = StaveEnum.Five;
    //[HideInInspector]
    public ClefEnum clefEnum = ClefEnum.Treble;
    //[HideInInspector]
    public KeySignatureEnum keySignatureEnum = KeySignatureEnum.GMaj_EMin;
    //[HideInInspector]
    public int numerator = 4;
    //[HideInInspector]
    public int denominator = 4;

    int renderTestCount = 1;

    public enum Test
    {
        Barlines,
        Beams,
        Chords,
        Clefs,
        GraceNotes,
        KeySignatures,
        LinesArpeggiato,
        LinesGlissando,
        LinesTrill,
        LinesVibrato,
        Lyrics,
        Notes,
        Percussions,
        Pitches,
        Repeats,
        Rests,
        Rhythms,
        Slurs,
        Tighs,
        TimeSignatures,
        Tuplets
    }

    bool initialized = false;

    public Dictionary<Test, bool> tests = new Dictionary<Test, bool>();

    string ppStaveLines = "TEST_STAVE_LINES";
    string ppKeySignature = "TEST_KEY_SIGNATURE";
    string ppClef = "TEST_CLEF";
    string ppTimeSignatureNumerator = "TEST_TIME_SIGNATURE_NUMERATOR";
    string ppTimeSignatureDenominator = "TEST_TIME_SIGNATURE_DENOMINATOR";

    int SetDropdownValue(string[] d, string v)
    {
        int i = 0;
        foreach (string o in d)
        {
            if (o == v)
            {
                return i;
            }
            i++;
        }
        return -1;
    }

    void Init()
    {
        if (initialized)
            return;

        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

        staveLinesDropdown.ClearOptions();
        foreach (StaveEnum se in System.Enum.GetValues(typeof(StaveEnum)))
        {
            options.Add(new Dropdown.OptionData(se.ToString()));
        }
        staveLinesDropdown.AddOptions(options);
        string staveEnumString = PlayerPrefs.GetString(ppStaveLines, staveEnum.ToString());
        staveLinesDropdown.value = SetDropdownValue(System.Enum.GetNames(typeof(StaveEnum)), staveEnumString);
        staveEnum = (StaveEnum)System.Enum.Parse(typeof(StaveEnum), staveEnumString);

        options.Clear();
        clefDropdown.ClearOptions();
        foreach (ClefEnum ce in System.Enum.GetValues(typeof(ClefEnum)))
        {
            options.Add(new Dropdown.OptionData(ce.ToString()));
        }
        clefDropdown.AddOptions(options);
        clefDropdown.value = SetDropdownValue(System.Enum.GetNames(typeof(ClefEnum)), PlayerPrefs.GetString(ppClef, clefEnum.ToString()));
        clefEnum = (ClefEnum)System.Enum.Parse(typeof(ClefEnum), PlayerPrefs.GetString(ppClef, clefEnum.ToString()));

        options.Clear();
        keySignatureDropdown.ClearOptions();
        foreach (KeySignatureEnum ks in System.Enum.GetValues(typeof(KeySignatureEnum)))
        {
            options.Add(new Dropdown.OptionData(ks.ToString()));
        }
        keySignatureDropdown.AddOptions(options);
        keySignatureDropdown.value = SetDropdownValue(System.Enum.GetNames(typeof(KeySignatureEnum)), PlayerPrefs.GetString(ppKeySignature, keySignatureEnum.ToString()));
        keySignatureEnum = (KeySignatureEnum)System.Enum.Parse(typeof(KeySignatureEnum), PlayerPrefs.GetString(ppKeySignature, keySignatureEnum.ToString()));

        options.Clear();
        timeSignatureDropdown.ClearOptions();
        options.Add(new Dropdown.OptionData("2/2"));
        options.Add(new Dropdown.OptionData("2/4"));
        options.Add(new Dropdown.OptionData("4/4"));
        options.Add(new Dropdown.OptionData("Undefined"));
        timeSignatureDropdown.AddOptions(options);
        numerator = PlayerPrefs.GetInt(ppTimeSignatureNumerator, numerator);
        denominator = PlayerPrefs.GetInt(ppTimeSignatureDenominator, denominator);
        if (numerator == 0 || denominator == 0)
        {
            timeSignatureDropdown.value = options.Count - 1;
        }
        else
        {
            string[] s = new string[options.Count];
            for (int i = 0; i < options.Count; i++)
            {
                s[i] = options[i].text;
            }
            timeSignatureDropdown.value = SetDropdownValue(s, numerator.ToString() + "/" + denominator.ToString());
        }

        foreach (Test test in System.Enum.GetValues(typeof(Test)))
        {
            tests.Add(test, (PlayerPrefs.GetInt("TEST_" + test.ToString(), 1) == 0 ? false : true));
        }

        float y = 0;
        foreach (KeyValuePair<Test, bool> kv in tests)
        {
            GameObject go = Instantiate(PREFAB_TestToggle);
            go.transform.SetParent(testContainer, false);
            Toggle toggle = go.GetComponent<Toggle>();
            Navigation n = toggle.navigation;
            n.mode = Navigation.Mode.None;
            toggle.navigation = n;
            toggle.GetComponentInChildren<Text>().text = kv.Key.ToString();
            toggle.isOn = kv.Value;
            NSTestSuiteManualToggle mToggle = go.GetComponent<NSTestSuiteManualToggle>();
            mToggle.test = kv.Key;
            mToggle.onChange += (t, b) =>
            {
                PlayerPrefs.SetInt("TEST_" + t.test.ToString(), b ? 1 : 0);
                tests[t.test] = b;
            };
            RectTransform rt = go.transform as RectTransform;
            rt.anchoredPosition = new Vector2(0, y);
            y -= 20;
        }

        testContainer.SetSize(new Vector2(testContainer.GetSize().x, Mathf.Abs(y - 20)));

        initialized = true;
    }

    IEnumerator Start()
    {
        yield return new WaitWhile(() => NSBehaviour.instance == null);
        Init();
    }

    public void OnStaveLinesDropdownChange(int i)
    {
        staveEnum = (StaveEnum)System.Enum.Parse(typeof(StaveEnum), staveLinesDropdown.options[staveLinesDropdown.value].text);
        PlayerPrefs.SetString(ppStaveLines, staveEnum.ToString());
    }

    public void OnClefDropdownChange(int i)
    {
        clefEnum = (ClefEnum)System.Enum.Parse(typeof(ClefEnum), clefDropdown.options[clefDropdown.value].text);
        PlayerPrefs.SetString(ppClef, clefEnum.ToString());
    }

    public void OnKeySignatureDropdownChange(int i)
    {
        keySignatureEnum = (KeySignatureEnum)System.Enum.Parse(typeof(KeySignatureEnum), keySignatureDropdown.options[keySignatureDropdown.value].text);
        PlayerPrefs.SetString(ppKeySignature, keySignatureEnum.ToString());
    }

    public void OnTimeSignatureDropdownChange(int i)
    {
        if (timeSignatureDropdown.options[timeSignatureDropdown.value].text.Contains("/"))
        {
            string[] values = timeSignatureDropdown.options[timeSignatureDropdown.value].text.Split('/');
            numerator = int.Parse(values[0]);
            denominator = int.Parse(values[1]);
            PlayerPrefs.SetInt(ppTimeSignatureNumerator, numerator);
            PlayerPrefs.SetInt(ppTimeSignatureDenominator, denominator);
        }
        else
        {
            PlayerPrefs.SetInt(ppTimeSignatureNumerator, 0);
            PlayerPrefs.SetInt(ppTimeSignatureDenominator, 0);
        }
    }

    public void OnRenderTestCount()
    {
        renderTestCount = int.Parse(renderTestCountInputField.text);
    }
}
