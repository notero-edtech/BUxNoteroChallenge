/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */

using UnityEngine;

[CreateAssetMenu(menuName = "NS/Settings/Specific/Rolling Top Bottom System")]
public class NSRollingTopBottomSystemSpecificSettings : NSSystemSpecificSettingsBase {
    
    [Header("Hit Zone")]
    public bool hitZoneVisible = true;
    public float hitZoneOffset = 250;
    public float hitZoneWidth = 10f;
    public Color hitZoneColor = Color.white;
    public bool hitZoneUpdatePosition = true;

    [Header("Instrument")] 
    public float instrumentTopOffset;
    public float instrumentBottomOffset;
    
    public enum Thickness { One = 1, Two = 2, Three = 3, Four = 3 }
    
    [Header("Lines")]
    public Thickness lineThickness = Thickness.One;
    
    [Header("Help Lines")]
    public bool cLine = true;
    public bool cLineThick = true;
    public bool eLine = true;
    public bool eLineThick = false;
    
    [Header("Stave")]
    public bool trebleLines = false;
    public bool bassLines = false;
    public bool ledgerLines = false;
    [Range(0, 4)] public int ledgerLinesCount = 0;
}
