using System;
using System.Collections.Generic;
public static partial class Game
{
    public static class Slots
    {
        public static SlotData player = new SlotData(); 
        public static List<SlotData> players = new List<SlotData>();
    }
    [Serializable]
    public class SlotData : AbstractGameData<SlotData>
    {
        public string name = "";
        
        public override void Init(){
        
        }
    }
}
