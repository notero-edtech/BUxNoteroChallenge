using UnityEngine;
using ForieroEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Scene Settings")]
    [Tooltip("")]
    public class SceneSettingsLoadBy : FsmStateAction
    {
        public enum NameEnum
        {
            SceneName,
            ProxySceneName,
            Command
        }

        [RequiredField]
        [Tooltip("Scene Name")]
        public FsmString stringVariable;
        public NameEnum nameEnum;

        public override void Reset()
        {
            stringVariable = null;
            nameEnum = NameEnum.Command;
        }

        public override void OnEnter()
        {
            switch (nameEnum)
            {
                case NameEnum.SceneName:
                    SceneSettings.LoadSceneBySceneName(stringVariable.Value);
                    break;
                case NameEnum.ProxySceneName:
                    SceneSettings.LoadSceneBySceneProxyName(stringVariable.Value);
                    break;
                case NameEnum.Command:
                    SceneSettings.LoadSceneByCommand(stringVariable.Value);
                    break;
            }

            Finish();
        }
    }
}