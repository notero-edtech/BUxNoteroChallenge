using System.Collections.Generic;
using ForieroEngine;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using PlayerPrefs = ForieroEngine.PlayerPrefs;
using TMPro;

public partial class PlayerManager : MonoBehaviour
{
    public static bool autoSave = false;
    static bool loaded = false;

    public GameObject PREFAB_user;

    public Button addButton;
    public Button removeButton;
    public Button playButton;
    public TMP_InputField newUserInputField;
    public RectTransform userContainer;
    public RectTransform confirmPanel;
    public RectTransform referralsPanel;
    public Text confirmText;
    public ScrollRect scrollRect;

    public GameObject PREFAB_TV_OFF;

    static GamePlayer _player = new GamePlayer("Guest");
    public bool levels = true;

    static string _prefix;
    static string _suffix;
    static string _prefsKey;
    static GamePlayer.StoreTypeEnum _prefsType = GamePlayer.StoreTypeEnum.Integers;

    public string prefix = "";
    public string prefsKey = "";
    public GamePlayer.StoreTypeEnum prefsType = GamePlayer.StoreTypeEnum.Integers;
    public string suffix = "";
        
    public static GamePlayer player
    {
        private set { _player = value; }
        get
        {
            if (!loaded) Load();                        
            return _player;
        }
    }

    public static void Reset()
    {
        Players.players = new List<GamePlayer>();
        _player = new GamePlayer("Guest");
        selected = -1;
        PlayerPrefs.DeleteKey<string>("PLAYERS");
    }

    static string[] selection = new string[0];
    static int selected = -1;

    void SelectPlayer(int idx)
    {
        for (int k = 0; k < Players.players.Count; k++)
        {
            Players.players[k].selected = false;
        }
        Players.players[idx].selected = true;
        selected = idx;
    }

    void OnEnable()
    {
        _suffix = suffix;
        _prefix = prefix;
        _prefsKey = prefsKey;
        _prefsType = prefsType;

        Load();

        PrepareSelection();
    }

    void Awake()
    {
        newUserInputField.onSubmit.AddListener((user) => { 
            OnAddClick();    
        });
    }
    
    void Start()
    {
        InitPlayerButtons();

        EventSystem.current.SetSelectedGameObject(playButton.gameObject, null);
    }

    void OnDestroy()
    {
        if (!autoSave) Save();
    }

    public void OnQuitClick()
    {
        Instantiate(PREFAB_TV_OFF);

        SM.StopAllMusic(0.2f);
    }

    static void PrepareSelection()
    {
        Players.players.Sort((GamePlayer p1, GamePlayer p2) =>
        {
            return p1.name.CompareTo(p2.name);
        });

        selection = new string[Players.players.Count];

        for (int i = 0; i < selection.Length; i++)
        {
            string append = _prefix;
            switch (_prefsType)
            {
                case GamePlayer.StoreTypeEnum.Integers:
                    append += Players.players[i].GetInt(_prefsKey);
                    break;
                case GamePlayer.StoreTypeEnum.Floats:
                    append += Players.players[i].GetFloat(_prefsKey);
                    break;
                case GamePlayer.StoreTypeEnum.Strings:
                    append += Players.players[i].GetString(_prefsKey);
                    break;
            }
            append += _suffix;
            selection[i] += append;
            if (Players.players[i].selected)
            {
                selected = i;
            }
        }

    }

    public static void Save()
    {
        Players.Save();
    }

    static void AutoSave()
    {
        if (autoSave) Save();
    }

    static void Load()
    {
        Players.Load();

        foreach (GamePlayer pl in Players.players)
        {
            if (pl.selected)
            {
                _player = pl;
                break;
            }
        }
    }

    public static bool PlayerNameExists(string aName)
    {
        foreach (GamePlayer pl in Players.players)
        {
            if (pl.name.Equals(aName))
                return true;
        }
        return false;
    }

    public static bool SelectPlayer(string aName)
    {
        bool result = false;
        foreach (GamePlayer pl in Players.players)
        {
            if (pl.name.Equals(aName))
            {
                _player = pl;
                result = true;
            }
        }

        foreach (GamePlayer pl in Players.players)
        {
            if (pl == player)
            {
                pl.selected = true;
            }
            else
            {
                pl.selected = false;
            }
        }

        AutoSave();

        return result;
    }

    public static bool SelectNextPlayer()
    {
        bool next = false;
        bool result = false;
        for (int i = 0; i < Players.players.Count; i++)
        {
            if (next)
            {
                _player = Players.players[i];
                result = true;
                break;
            }
            if (player == Players.players[i])
                next = true;
        }

        foreach (GamePlayer pl in Players.players)
        {
            if (pl == player)
            {
                pl.selected = true;
            }
            else
            {
                pl.selected = false;
            }
        }

        AutoSave();

        return result;
    }

    public static bool SelectPrevPlayer()
    {
        bool next = false;
        bool result = false;
        for (int i = Players.players.Count - 1; i >= 0; i--)
        {
            if (next)
            {
                _player = Players.players[i];
                result = true;
                break;
            }
            if (player == Players.players[i])
                next = true;
        }

        foreach (GamePlayer pl in Players.players)
        {
            if (pl == player)
            {
                pl.selected = true;
            }
            else
            {
                pl.selected = false;
            }
        }

        AutoSave();

        return result;
    }

    public static int PlayerIndex(string aName)
    {
        for (int i = 0; i < Players.players.Count; i++)
        {
            if (aName.Equals(Players.players[i].name))
                return i;
        }
        return -1;
    }

    public static bool RemovePlayer(string aName)
    {
        GamePlayer p = null;
        foreach (GamePlayer pl in Players.players)
        {
            if (pl.name.Equals(aName)) p = pl;
        }

        if (p != null)
        {
            if (player == p) _player = new GamePlayer("Guest");
            
            Players.players.Remove(p);

            AutoSave();

            return true;
        }

        return false;
    }

    public static void AddPlayer(string aName)
    {
        foreach (GamePlayer pl in Players.players)
        {
            pl.selected = false;
        }

        _player = new GamePlayer(aName) { selected = true };

        Players.players.Add(_player);

        PrepareSelection();

        selected = Players.players.FindIndex((GamePlayer pl) => pl.guid == _player.guid);
        
        AutoSave();
    }

    public void RemovePlayer()
    {
        Players.players.RemoveAt(selected);
        selected = -1;
        AutoSave();
        PrepareSelection();
    }

    public class Players
    {
        public static List<GamePlayer> players = new List<GamePlayer>();
        public List<GamePlayer> playersData = new List<GamePlayer>();

        public static GamePlayer selectedPlayer
        {
            set
            {
                foreach (GamePlayer p in players)
                {
                    p.selected = p == value;
                }
            }
            get
            {
                foreach (GamePlayer p in players)
                {
                    if (p.selected) return p;
                }

                return null;
            }
        }

        public static GamePlayer GetPlayer(string name)
        {
            foreach (GamePlayer p in players)
            {
                if (p.name == name) return p;
            }

            return null;
        }

        static int saveCount = 0;

        public static void Save()
        {            
            saveCount++;
            Players p = new Players();
            p.playersData = new List<GamePlayer>(Players.players);
            string s = JsonConvert.SerializeObject(p);

            if (Foriero.debug) Debug.Log("Saving players : " + s);

            PlayerPrefs.SetString("PLAYERS", s);
        }

        public static void Load()
        {            
            string s = PlayerPrefs.GetString("PLAYERS", "");

            if (Foriero.debug) Debug.Log("Loading players : " + s);

            Players p = s.TryParse<Players>();

            if (p == null) { Debug.LogError("PLAYERS ARE EMPTY OR DAMAGED!!!"); }
            else { Players.players = new List<GamePlayer>(p.playersData); }

            loaded = true;
        }
    }

    public class GamePlayer
    {
        public enum StoreTypeEnum
        {
            Integers,
            Floats,
            Strings,
            Bools
        }

        public GamePlayer(string name) => this.name = name;
        
        public string guid = System.Guid.NewGuid().ToString();
        public string name = "";
        public Dictionary<string, int> ints = new Dictionary<string, int>();
        public Dictionary<string, float> floats = new Dictionary<string, float>();
        public Dictionary<string, string> strings = new Dictionary<string, string>();
        public Dictionary<string, bool> bools = new Dictionary<string, bool>();
        public bool selected = false;

        public bool HasKey(string aKey, StoreTypeEnum aStoreType)
        {
            bool result = false;
            switch (aStoreType)
            {
                case StoreTypeEnum.Floats:
                    result = floats.ContainsKey(aKey);
                    break;
                case StoreTypeEnum.Integers:
                    result = ints.ContainsKey(aKey);
                    break;
                case StoreTypeEnum.Strings:
                    result = strings.ContainsKey(aKey);
                    break;
                case StoreTypeEnum.Bools:
                    result = bools.ContainsKey(aKey);
                    break;
            }
            return result;
        }

        public void DeleteAll()
        {
            ints = new Dictionary<string, int>();
            floats = new Dictionary<string, float>();
            strings = new Dictionary<string, string>();
            bools = new Dictionary<string, bool>();

            AutoSave();
        }

        public void DeleteKey(string aKey, StoreTypeEnum aStoreType)
        {
            switch (aStoreType)
            {
                case StoreTypeEnum.Floats:
                    if (floats.ContainsKey(aKey))
                    {
                        floats.Remove(aKey);
                        AutoSave();
                    }
                    break;
                case StoreTypeEnum.Integers:
                    if (ints.ContainsKey(aKey))
                    {
                        ints.Remove(aKey);
                        AutoSave();
                    }
                    break;
                case StoreTypeEnum.Strings:
                    if (strings.ContainsKey(aKey))
                    {
                        strings.Remove(aKey);
                        AutoSave();
                    }
                    break;
                case StoreTypeEnum.Bools:
                    if (bools.ContainsKey(aKey))
                    {
                        bools.Remove(aKey);
                        AutoSave();
                    }
                    break;
            }
        }

        public void SetInt(string aKey, int aValue)
        {
            if (ints.ContainsKey(aKey))
            {
                ints[aKey] = aValue;
            }
            else
            {
                ints.Add(aKey, aValue);
            }

            AutoSave();
        }

        public void SetFloat(string aKey, float aValue)
        {
            if (floats.ContainsKey(aKey))
            {
                floats[aKey] = aValue;
            }
            else
            {
                floats.Add(aKey, aValue);
            }

            AutoSave();
        }

        public void SetString(string aKey, string aValue)
        {
            if (strings.ContainsKey(aKey))
            {
                strings[aKey] = aValue;
            }
            else
            {
                strings.Add(aKey, aValue);
            }

            AutoSave();
        }

        public void SetBool(string aKey, bool aValue)
        {
            if (bools.ContainsKey(aKey))
            {
                bools[aKey] = aValue;
            }
            else
            {
                bools.Add(aKey, aValue);
            }

            AutoSave();
        }

        public int GetInt(string aKey, int aDefault = 0)
        {
            if (ints != null)
            {
                if (string.IsNullOrEmpty(aKey)) return aDefault;

                if (ints.ContainsKey(aKey))
                {
                    return ints[aKey];
                }
                else
                {
                    return aDefault;
                }
            }
            else
            {
                return aDefault;
            }
        }

        public float GetFloat(string aKey, float aDefault = 0f)
        {
            if (floats != null)
            {
                if (string.IsNullOrEmpty(aKey)) return aDefault;

                if (floats.ContainsKey(aKey))
                {
                    return floats[aKey];
                }
                else
                {
                    return aDefault;
                }
            }
            else
            {
                return aDefault;
            }
        }

        public string GetString(string aKey, string aDefault = "")
        {
            if (strings != null)
            {
                if (string.IsNullOrEmpty(aKey)) return aDefault;

                if (strings.ContainsKey(aKey))
                {
                    return strings[aKey];
                }
                else
                {
                    return aDefault;
                }
            }
            else
            {
                return aDefault;
            }
        }

        public bool GetBool(string aKey, bool aDefault = false)
        {
            if (bools != null)
            {
                if (string.IsNullOrEmpty(aKey)) return aDefault;

                if (bools.ContainsKey(aKey))
                {
                    return bools[aKey];
                }
                else
                {
                    return aDefault;
                }
            }
            else
            {
                return aDefault;
            }
        }
    }

}
