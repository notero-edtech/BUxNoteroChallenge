/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */

using ForieroEngine.Music.NotationSystem.Systems;
using UnityEngine;

[CreateAssetMenu(menuName = "NS/Settings/Page Layout Ticker System Settings")]
public class NSPageLayoutTickerSystemSettings : NSSystemSettingsBase<NSPageLayoutTickerSystemSpecificSettings>
{
    public override void Init()
    {
       if(NSB) NSB.Init(new NSPageLayoutTickerSystem(NSB, this));
    }
}
