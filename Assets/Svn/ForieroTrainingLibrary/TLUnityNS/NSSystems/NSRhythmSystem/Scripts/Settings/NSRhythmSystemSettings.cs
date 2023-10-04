/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */

using ForieroEngine.Music.NotationSystem.Systems;
using UnityEngine;

[CreateAssetMenu(menuName = "NS/Settings/Rhythm System SO")]
public class NSRhythmSystemSettings : NSSystemSettingsBase<NSRhythmSystemSpecificSettings>
{
    public override void Init()
    {
        if(NSB) NSB.Init(new NSRhythmSystem(NSB, this));
    }
}
