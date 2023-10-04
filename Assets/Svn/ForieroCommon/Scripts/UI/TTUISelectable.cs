using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class TTUISelectable : MonoBehaviour
{
    public TTUISelectableStyle style;

    private void Awake()
    {
        var s = this.gameObject.GetComponent<Selectable>();
        if (!style) return;
        s.transition = style.transition;
        switch (style.transition)
        {
            case Selectable.Transition.None: break;
            case Selectable.Transition.ColorTint: s.colors = style.colorBlock; break;
            case Selectable.Transition.SpriteSwap: s.spriteState = style.spriteState; break;
            case Selectable.Transition.Animation: s.animationTriggers = style.animationTriggers; break;
        }
    }
}
