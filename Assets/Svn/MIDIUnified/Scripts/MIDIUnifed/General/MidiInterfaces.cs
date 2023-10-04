public enum MidiSeqStates { None, PickUpBar, Playing, Pausing, Finished }

public interface IMidiSeqControl
{
	MidiSeqStates State { get; }
	void Play();
	void Continue();
	void Pause();
	void Stop();
}
	
public interface IFrameFeed {
	int SampleRate();
	int DeltaFrames();
		
	void PlayFeed();
	void StopFeed();
	void PauseFeed();
}

