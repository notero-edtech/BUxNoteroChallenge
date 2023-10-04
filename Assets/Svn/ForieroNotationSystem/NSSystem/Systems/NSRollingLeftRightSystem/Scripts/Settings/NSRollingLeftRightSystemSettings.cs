/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */

using ForieroEngine.Music.NotationSystem.Systems;
using UnityEngine;

[CreateAssetMenu(menuName = "NS/Settings/Rolling Left Right System Settings")]
public class NSRollingLeftRightSystemSettings : NSSystemSettingsBase<NSRollingLeftRightSystemSpecificSettings>
{
    public override void Init()
    {
        if(NSB) NSB.Init(new NSRollingLeftRightSystem(NSB, this));
    }
}
