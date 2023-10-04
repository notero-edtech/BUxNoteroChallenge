/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Systems;
using UnityEngine;

[CreateAssetMenu(menuName = "NS/Settings/Rolling Top Bottom System Settings")]
public class NSRollingTopBottomSystemSettings : NSSystemSettingsBase<NSRollingTopBottomSystemSpecificSettings>
{
    public override void Init()
    {
        if(NSB) NSB.Init(new NSRollingTopBottomSystem(NSB, this));
    }
}
