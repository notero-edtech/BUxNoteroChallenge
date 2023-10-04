using static HutongGames.PlayMaker.Actions.ForieroPlayerPrefs;
using PlayerPrefs = ForieroEngine.PlayerPrefs;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Foriero - PlayerPrefs")]
    [Tooltip("Returns true if key exists in the preferences.")]
    public class ForieroPlayerPrefsHasKey : FsmStateAction
    {
        public ScopeEnum scopeEnum;
        public PlayerManager.GamePlayer.StoreTypeEnum storeTypeEnum;

        [RequiredField]
        public FsmString key;

        [UIHint(UIHint.Variable)]
        [Title("Store Result")]
        public FsmBool variable;

        [Tooltip("Event to send if key exists.")]
        public FsmEvent trueEvent;

        [Tooltip("Event to send if key does not exist.")]
        public FsmEvent falseEvent;

        public override void Reset()
        {
            key = "";
            scopeEnum = ScopeEnum.PlayePrefs;
            storeTypeEnum = PlayerManager.GamePlayer.StoreTypeEnum.Strings;
        }

        public override void OnEnter()
        {
            Finish();

            if (!key.IsNone && !key.Value.Equals(""))
            {
                switch (scopeEnum)
                {
                    case ScopeEnum.PlayePrefs:
                        switch (storeTypeEnum)
                        {
                            case PlayerManager.GamePlayer.StoreTypeEnum.Integers:
                                variable.Value = PlayerPrefs.HasKey<int>(key.Value);
                                break;
                            case PlayerManager.GamePlayer.StoreTypeEnum.Floats:
                                variable.Value = PlayerPrefs.HasKey<float>(key.Value);
                                break;
                            case PlayerManager.GamePlayer.StoreTypeEnum.Strings:
                                variable.Value = PlayerPrefs.HasKey<string>(key.Value);
                                break;
                            case PlayerManager.GamePlayer.StoreTypeEnum.Bools:
                                variable.Value = PlayerPrefs.HasKey<bool>(key.Value);
                                break;
                        }

                        break;
                    case ScopeEnum.Player:
                        variable.Value = PlayerManager.player.HasKey(key.Value, storeTypeEnum);
                        break;
                }

            }

            Fsm.Event(variable.Value ? trueEvent : falseEvent);
        }
    }
}