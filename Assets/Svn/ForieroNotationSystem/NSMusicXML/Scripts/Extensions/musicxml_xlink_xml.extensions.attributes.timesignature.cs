/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem;

namespace ForieroEngine.Music.MusicXML.Xsd
{
    public static partial class MusicXMLExtensions
    {
        public static time[] SelfOrDefault(this time[] times) => times ??= new time[1] { ((time)null).SelfOrDefault() };
      
        public static time SelfOrDefault(this time time)
        {
            if (time != null) return time;
            time = new time();
            time.ItemsElementName = time.ItemsElementName.Add(ItemsChoiceType11.beats);
            time.Items = time.Items.Add<string>("4");
            time.ItemsElementName = time.ItemsElementName.Add(ItemsChoiceType11.beattype);
            time.Items = time.Items.Add<string>("4");
            time.symbolSpecified = true;
            time.symbol = timesymbol.common;
            return time;
        }

        public static TimeSignatureStruct GetTimeSignature(this time time)
        {
            var beats = int.Parse(time.ItemsElementName.ValueOf<string>(ItemsChoiceType11.beats, time.Items));
            var beatsType = int.Parse(time.ItemsElementName.ValueOf<string>(ItemsChoiceType11.beattype, time.Items));
            return new TimeSignatureStruct(beats, beatsType);
        }

        public static int GetStaveNumber(this time time, int defaultValue = -1) => time == null || !int.TryParse(time.number, out var temp) ? defaultValue : temp - 1;

        public static TimeSignatureEnum GetTimeSignatureEnum(this time time, TimeSignatureEnum defaultValue = TimeSignatureEnum.Normal)
        {
            if (time is not { symbolSpecified: true }) return defaultValue;
            return time.symbol switch
            {
                timesymbol.common => TimeSignatureEnum.Common,
                timesymbol.cut => TimeSignatureEnum.Cut,
                timesymbol.normal => TimeSignatureEnum.Normal,
                timesymbol.dottednote => TimeSignatureEnum.Normal,
                timesymbol.note => TimeSignatureEnum.Normal,
                timesymbol.singlenumber => TimeSignatureEnum.SingleNumber,
                _ => defaultValue
            };
        }
    }
}
