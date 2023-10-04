using static HutongGames.PlayMaker.Actions.ForieroPlayerPrefs;
using PlayerPrefs = ForieroEngine.PlayerPrefs;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Foriero - PlayerPrefs")]
    [Tooltip("Sets the value of the preference identified by key.")]
    public class ForieroPlayerPrefsSetBool : FsmStateAction
    {
        public ScopeEnum scopeEnum;

        [CompoundArray("Count", "Key", "Value")]
        [Tooltip("Case sensitive key.")]
        public FsmString[] keys;
        public FsmBool[] values;

        public override void Reset()
        {
            keys = new FsmString[1];
            values = new FsmBool[1];
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
                            PlayerPrefs.SetBool(keys[i].Value, values[i].IsNone ? false : values[i].Value);
                            break;
                        case ScopeEnum.Player:
                            PlayerManager.player.SetBool(keys[i].Value, values[i].IsNone ? false : values[i].Value);
                            break;
                    }

                }
            }
            Finish();
        }

    }
}