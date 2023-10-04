using static HutongGames.PlayMaker.Actions.ForieroPlayerPrefs;
using PlayerPrefs = ForieroEngine.PlayerPrefs;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Foriero - PlayerPrefs")]
    [Tooltip("Returns the value corresponding to key in the preference file if it exists.")]
    public class ForieroPlayerPrefsGetFloat : FsmStateAction
    {
        public ScopeEnum scopeEnum;

        [CompoundArray("Count", "Key", "Variable")]
        [Tooltip("Case sensitive key.")]
        public FsmString[] keys;
        [UIHint(UIHint.Variable)]
        public FsmFloat[] variables;

        public override void Reset()
        {
            keys = new FsmString[1];
            variables = new FsmFloat[1];
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
                            variables[i].Value = PlayerPrefs.GetFloat(keys[i].Value, variables[i].IsNone ? 0f : variables[i].Value);
                            break;
                        case ScopeEnum.Player:
                            variables[i].Value = PlayerManager.player.GetFloat(keys[i].Value, variables[i].IsNone ? 0f : variables[i].Value);
                            break;
                    }
                }
            }
            Finish();
        }

    }
}