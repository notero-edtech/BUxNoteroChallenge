/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */

using ForieroEngine.Music.NotationSystem;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "NS/Settings/Specific/Rolling Left Right System")]
public class NSRollingLeftRightSystemSpecificSettings : NSSystemSpecificSettingsBase {
    [Header("Hit Zone")]
    public bool hitZoneVisible = true;
    public float hitZoneOffset = 250;
    public float hitZoneWidth = 10f;
    public Color hitZoneColor = Color.white;
    public bool hitZoneUpdatePosition = true;
    
    [Header("Width")]
    public Stretch stretch = Stretch.Auto;
    public float autoMargin = 0;
    public float width = 700;
    
    [Header("Offsets")]
    [FormerlySerializedAs("staveNotationOffset")] 
    public float notationOffset = 0;
}
