/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSPlayback
    {
        public static string ToTimeString(this double time, string format = @"mm\:ss\.f") => TimeSpan.FromSeconds(time).ToString(format);
        public static string ToTimeString(this float time, string format = @"mm\:ss\.f") => TimeSpan.FromSeconds(time).ToString(format);
        public static string ToMeasureString(this int measure, string format = @"000") => measure.ToString(format);
        public static float ToFloat(this double d) => (float)d;
        public static float ToTimePixels(this float time) => time * NSPlayback.NSRollingPlayback.pixelsPerSecond;
        public static FeedbackEnum GetFeedbackEnum(this float diff)
        {
            if (diff < NSFeedbackSettings.instance.tooEarly) return FeedbackEnum.Missed;
            if (diff < NSFeedbackSettings.instance.early) return FeedbackEnum.TooEarly;
            if (diff < NSFeedbackSettings.instance.perfectMin) return FeedbackEnum.Early;
            if (diff >= NSFeedbackSettings.instance.perfectMin && diff <= NSFeedbackSettings.instance.perfectMax) return FeedbackEnum.Perfect;
            if (diff > NSFeedbackSettings.instance.tooLate) return FeedbackEnum.Missed;
            if (diff > NSFeedbackSettings.instance.late) return FeedbackEnum.TooLate;
            if (diff > NSFeedbackSettings.instance.perfectMax) return FeedbackEnum.Late;
            return FeedbackEnum.Undefined;
        }
    }
}
