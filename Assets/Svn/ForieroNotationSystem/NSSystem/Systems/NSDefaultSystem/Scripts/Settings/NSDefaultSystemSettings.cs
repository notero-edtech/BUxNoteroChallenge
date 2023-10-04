/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */

using ForieroEngine.Music.NotationSystem.Systems;
using UnityEngine;

[CreateAssetMenu(menuName = "NS/Settings/Default System Settings")]
public class NSDefaultSystemSettings : NSSystemSettingsBase<NSDefaultSystemSpecificSettings>
{
    public override void Init()
    {
        if(NSB) NSB.Init(new NSDefaultSystem(NSB, this));
    }
}
