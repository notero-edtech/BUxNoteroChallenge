using static HutongGames.PlayMaker.Actions.ForieroPlayerPrefs;
using PlayerPrefs = ForieroEngine.PlayerPrefs;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Foriero - PlayerPrefs")]
    [Tooltip("Removes key and its corresponding value from the preferences.")]
    public class ForieroPlayerPrefsDeleteKey : FsmStateAction
    {
        public ScopeEnum scopeEnum;
        public PlayerManager.GamePlayer.StoreTypeEnum storeTypeEnum;

        public FsmString key;

        public override void Reset()
        {
            key = "";
            scopeEnum = ScopeEnum.PlayePrefs;
            storeTypeEnum = PlayerManager.GamePlayer.StoreTypeEnum.Strings;
        }

        public override void OnEnter()
        {
            if (!key.IsNone && !key.Value.Equals(""))
            {
                switch (scopeEnum)
                {
                    case ScopeEnum.PlayePrefs:
                        switch (storeTypeEnum)
                        {
                            case PlayerManager.GamePlayer.StoreTypeEnum.Integers:
                                PlayerPrefs.DeleteKey<int>(key.Value);
                                break;
                            case PlayerManager.GamePlayer.StoreTypeEnum.Floats:
                                PlayerPrefs.DeleteKey<float>(key.Value);
                                break;
                            case PlayerManager.GamePlayer.StoreTypeEnum.Strings:
                                PlayerPrefs.DeleteKey<string>(key.Value);
                                break;
                            case PlayerManager.GamePlayer.StoreTypeEnum.Bools:
                                PlayerPrefs.DeleteKey<bool>(key.Value);
                                break;
                        }
                        break;
                    case ScopeEnum.Player:
                        PlayerManager.player.DeleteKey(key.Value, storeTypeEnum);
                        break;
                }
            }

            Finish();
        }
    }
}