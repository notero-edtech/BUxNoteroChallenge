using static HutongGames.PlayMaker.Actions.ForieroPlayerPrefs;
using PlayerPrefs = ForieroEngine.PlayerPrefs;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Foriero - PlayerPrefs")]
    [Tooltip("Sets the value of the preference identified by key.")]
    public class ForieroPlayerPrefsSetString : FsmStateAction
    {
        public ScopeEnum scopeEnum;

        [CompoundArray("Count", "Key", "Value")]
        [Tooltip("Case sensitive key.")]
        public FsmString[] keys;
        public FsmString[] values;

        public override void Reset()
        {
            keys = new FsmString[1];
            values = new FsmString[1];
            scopeEnum = ScopeEnum.PlayePrefs;
        }

        public override void OnEnter()
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (!keys[i].IsNone || !keys[i].Value.Equals(""))
                {
                    switch (scopeEnum)
                    {
                        case ScopeEnum.PlayePrefs:
                            PlayerPrefs.SetString(keys[i].Value, values[i].IsNone ? "" : values[i].Value);
                            break;
                        case ScopeEnum.Player:
                            PlayerManager.player.SetString(keys[i].Value, values[i].IsNone ? "" : values[i].Value);
                            break;
                    }

                }
            }
            Finish();
        }

    }
}