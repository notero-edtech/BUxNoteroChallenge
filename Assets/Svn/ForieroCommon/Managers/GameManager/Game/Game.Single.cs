using System;

public static partial class Game
{
    public static class Single
    {
        public static SingleData single = new SingleData();
    }
    
    [Serializable]
    public class SingleData : AbstractGameData<SingleData>
    {
        public string name = "";
        
        public override void Init(){
        
        }
    }
}
