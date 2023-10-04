/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Systems;
using UnityEngine;

[CreateAssetMenu(menuName = "NS/Settings/Rolling Ticker System Settings")]
public class NSRollingTickerSystemSettings : NSSystemSettingsBase<NSRollingTickerSystemSpecificSettings>
{
    public override void Init()
    {
        if(NSB) NSB.Init(new NSRollingTickerSystem(NSB, this));
    }
}
