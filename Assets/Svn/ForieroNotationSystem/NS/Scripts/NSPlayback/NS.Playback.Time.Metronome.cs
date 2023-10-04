/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSPlayback
    {                               
        public static partial class Time
        {           
            public static class Metronome
            {
                public static float StartTime = 0f;
                private static volatile float _offset = 0;
                public static float Offset
                {
                    get => _offset;
                    set
                    {
                        _offset = value;
                        Beats.Cancel();                        
                        Beats.Schedule();
                    }
                }
            }    
        }              
    }
}
