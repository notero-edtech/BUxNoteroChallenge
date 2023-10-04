using UnityEngine;
using UnityEngine.Serialization;

namespace ForieroEditor.Tools.MultiProjectBuilder
{
    [System.Serializable]
    public class MultiProjectBuilderDummy : ScriptableObject
    {
        [FormerlySerializedAs("buildVersions")]
        [SerializeField]
        public MultiProjectBuilder mpb;
    }
}
