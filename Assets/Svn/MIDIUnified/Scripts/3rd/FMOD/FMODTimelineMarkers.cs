using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class FMODTimelineMarkers
{
        public string Id { get; set; }
        public List<Marker> Markers { get; set; }
    
        public class TriggerCondition
        {
            public int ParameterIndex { get; set; }
            public int Minimum { get; set; }
            public int Maximum { get; set; }
        }
    
        public class Marker
        {
            public enum MTypeEnum
            {
                Region,
                TransitionRegion,
                TempoMarker,
                DestinationMarker,
                TransitionMarker
            }
        
            [JsonConverter(typeof(StringEnumConverter))]
            public MTypeEnum MType { get; set; }
            public double Length { get; set; }
            public int Looping { get; set; }
            // in seconds //
            public double? Position { get; set; }
            public string PositionString => Position == null ? "00:00:00.00" : TimeSpan.FromSeconds((double)Position).ToString(@"mm\:ss\.fff");
            public int? DestinationIndex { get; set; }
            public int? QuantizationInterval { get; set; }
            public List<TriggerCondition> TriggerConditions { get; set; }
            public int? Tempo { get; set; }
            public string Name { get; set; }
            public override string ToString() => MType + "/" + PositionString + "|" +  Name + "|" + Tempo;
        }

        public static FMODTimelineMarkers FromJSON(string json)
        {
            FMODTimelineMarkers r;
            try { r = JsonConvert.DeserializeObject<FMODTimelineMarkers>(json); }
            catch { r = new FMODTimelineMarkers(); }
            return r;
        }
}
