/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;

[CreateAssetMenu(menuName = "NS/Settings/Specific/Page Layout Ticker System")]
public class NSPageLayoutTickerSystemSpecificSettings : NSSystemSpecificSettingsBase {
    public enum PageStack { Horizontal, Vertical, Undefined = int.MaxValue }

    public PageStack pageStack = PageStack.Undefined;
    public int numberOfPages = 2;
    public Sprite pageSprite;
}
