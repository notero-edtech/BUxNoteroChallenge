using static HutongGames.PlayMaker.Actions.ForieroPlayerPrefs;
using PlayerPrefs = ForieroEngine.PlayerPrefs;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Foriero - PlayerPrefs")]
    [Tooltip("Removes all keys and values from the preferences. Use with caution.")]
    public class ForieroPlayerPrefsDeleteAll : FsmStateAction
    {
        public ScopeEnum scopeEnum;

        public override void Reset()
        {
            scopeEnum = ScopeEnum.PlayePrefs;
        }

        public override void OnEnter()
        {
            switch (scopeEnum)
            {
                case ScopeEnum.PlayePrefs:
                    PlayerPrefs.DeleteAll();
                    break;
                case ScopeEnum.Player:
                    PlayerManager.player.DeleteAll();
                    break;
            }

            Finish();
        }
    }
}