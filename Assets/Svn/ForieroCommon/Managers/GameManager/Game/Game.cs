public static partial class Game
{
    public enum GameEnum
    {
        Single,
        Slots,
        Players
    }
    
    public static GameEnum gameEnum => GameSettings.instance.gameEnum;
    
    public abstract class AbstractGameData<T> where T : class
    {
        public void Save()
        {
            
        }
        public void Load()
        {
            
        }

        public abstract void Init();
    }
}
