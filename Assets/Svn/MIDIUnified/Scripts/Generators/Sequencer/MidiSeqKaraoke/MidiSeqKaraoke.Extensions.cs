/* Copyright © Marek Ledvina, Foriero s.r.o. */
public partial class MidiSeqKaraoke
{
    public const int MicrosecondsPerMinute = 60000000;
    public const int MicrosecondsPerSecond = 1000000;
    public const int MicrosecondsPerMillisecond = 1000;
    
    public double TimeToTicks(double t) => PPQN * 1000f * t * MicrosecondsPerMillisecond / tempoTicks;
    public double TicksToTime(double t) => t / PPQN / 1000f / MicrosecondsPerMillisecond * tempoTicks;
}

public static class MidiSeqKaraokeExtensions
{
    public static double ToTicks(this double time,int ppqn, int tempo) => ppqn * 1000f * time * MidiSeqKaraoke.MicrosecondsPerMillisecond / MidiSeqKaraoke.MicrosecondsPerMinute / tempo;
    public static double ToTime(this double ticks, int ppqn, int tempo) => ticks / ppqn / 1000f / MidiSeqKaraoke.MicrosecondsPerMillisecond * MidiSeqKaraoke.MicrosecondsPerMinute / tempo;
    
    public static double ToTicks(this float time,int ppqn, int tempo) => ppqn * 1000f * time * MidiSeqKaraoke.MicrosecondsPerMillisecond / MidiSeqKaraoke.MicrosecondsPerMinute / tempo;
    public static double ToTime(this float ticks, int ppqn, int tempo) => ticks / ppqn / 1000f / MidiSeqKaraoke.MicrosecondsPerMillisecond * MidiSeqKaraoke.MicrosecondsPerMinute / tempo;
    
    public static double ToTime(this long ticks, int ppqn, int tempo) => (double)ticks / ppqn / 1000f / MidiSeqKaraoke.MicrosecondsPerMillisecond * MidiSeqKaraoke.MicrosecondsPerMinute / tempo;
    public static double ToTime(this int ticks, int ppqn, int tempo) => (float)ticks / ppqn / 1000f / MidiSeqKaraoke.MicrosecondsPerMillisecond * MidiSeqKaraoke.MicrosecondsPerMinute / tempo;
}
