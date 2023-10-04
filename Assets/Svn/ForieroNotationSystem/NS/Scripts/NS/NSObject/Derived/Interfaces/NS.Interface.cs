/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem
{
    public interface INSText
    {
        Color GetColor();

        void SetColor(Color color);

        string GetText();

        void SetText(string text);

        TextAnchor GetAlignment();

        void SetAlignment(TextAnchor alignment);

        float GetPreferredWidth();

        float GetPreferredHeight();

        bool GetRaycastTarget();

        void SetRaycastTarget(bool raycastTarget);

        int GetFontSize();

        void SetFontSize(int fontSize);
    }

    public interface INSColorable
    {
        void SetColor(Color color);
        void SetAlpha(float alpha);
        Color GetColor();
    }
}
