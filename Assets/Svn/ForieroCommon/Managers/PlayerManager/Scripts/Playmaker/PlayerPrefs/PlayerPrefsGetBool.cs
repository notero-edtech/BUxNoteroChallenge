using static HutongGames.PlayMaker.Actions.ForieroPlayerPrefs;
using PlayerPrefs = ForieroEngine.PlayerPrefs;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Foriero - PlayerPrefs")]
    [Tooltip("Returns the value corresponding to key in the preference file if it exists.")]
    public class ForieroPlayerPrefsGetBool : FsmStateAction
    {
        public ScopeEnum scopeEnum;

        [CompoundArray("Count", "Key", "Variable")]
        [Tooltip("Case sensitive key.")]
        public FsmString[] keys;
        [UIHint(UIHint.Variable)]
        public FsmBool[] variables;

        public override void Reset()
        {
            keys = new FsmString[1];
            variables = new FsmBool[1];
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
                            variables[i].Value = PlayerPrefs.GetBool(keys[i].Value, variables[i].IsNone ? false : variables[i].Value);
                            break;
                        case ScopeEnum.Player:
                            variables[i].Value = PlayerManager.player.GetBool(keys[i].Value, variables[i].IsNone ? false : variables[i].Value);
                            break;
                    }
                }

            }
            Finish();
        }

    }
}