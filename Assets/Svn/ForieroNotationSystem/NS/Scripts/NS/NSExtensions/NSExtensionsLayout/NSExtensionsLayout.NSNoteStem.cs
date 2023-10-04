/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using System.Linq;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;
using UnityEngine.Assertions;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSExtensionsLayout
    {
        public static float GetDefaultStemSize(this NSNoteStem noteStem, float lineMultiplier = 3.5f)
        {
            float result = noteStem.ns.LineSize * lineMultiplier;

            switch (noteStem.options.noteEnum)
            {
                case NoteEnum.Item32nd:
                    result += 1f * noteStem.ns.BeamVerticalDistance;
                    break;
                case NoteEnum.Item64th:
                    result += 2f * noteStem.ns.BeamVerticalDistance;
                    break;
                case NoteEnum.Item128th:
                    result += 3f * noteStem.ns.BeamVerticalDistance;
                    break;
                case NoteEnum.Item256th:
                    result += 4f * noteStem.ns.BeamVerticalDistance;
                    break;
                case NoteEnum.Item512th:
                    result += 5f * noteStem.ns.BeamVerticalDistance;
                    break;
                case NoteEnum.Item1024th:
                    result += 6f * noteStem.ns.BeamVerticalDistance;
                    break;
            }

            return result;
        }

        public static float GetDefaultStaveRelatedSize(this NSNoteStem noteStem, float lineMultiplier = 3.5f)
        {
            float result = noteStem.GetDefaultStemSize(lineMultiplier);

            if (!noteStem.stave) return result;

            var distance = noteStem.GetPositionY(false).Distance(noteStem.stave.GetPositionY(false));

            if (Mathf.Approximately(noteStem.GetPositionY(false), noteStem.stave.GetPositionY(false)))
            {
                return result;
            }
            else if (noteStem.GetPositionY(false) > noteStem.stave.GetPositionY(false))
            {
                switch (noteStem.options.stemEnum)
                {
                    case StemEnum.Up:
                        break;
                    case StemEnum.Down:
                        result = Mathf.Clamp(distance, result, float.MaxValue);
                        break;
                    case StemEnum.Undefined:
                        break;
                }
            }
            else if (noteStem.GetPositionY(false) < noteStem.stave.GetPositionY(false))
            {
                switch (noteStem.options.stemEnum)
                {
                    case StemEnum.Up:
                        result = Mathf.Clamp(distance, result, float.MaxValue);
                        break;
                    case StemEnum.Down:
                        break;
                    case StemEnum.Undefined:
                        break;
                }
            }

            return result;
        }
    }
}
