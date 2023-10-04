using System.Collections;
using AudioSynthesis.Synthesis;
using ForieroEngine.Audio.Recording;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Synthesizer;
using UnityEngine;
using MidiMessage = AudioSynthesis.Synthesis.MidiMessage;

public partial class CSharpSynth : MonoBehaviour, ISynthRecorder
{
    public static CSharpSynth Instance { get; private set; }

    #region general private variables
    
    private AudioSource _audioSource = null;
    private int _channels = 0;
    private int _bufferSize = 0;
    private int _numBuffers = 0;
    private int _outputSampleRate = 44100;
    private readonly string _soundBank = "soundfont.sf2";
    
    #endregion
    
    #region thread private variables

    private int _currentBufferIndex = 0;
    private float[] _currentBuffer = new float[0];
    private float[] _tempBuffer = new float[0];

    private MidiMessage _midiMessage;
    private Synthesizer _synth;
    private int _sampleRateDivider = 1;
    
    private int _length = 0;
    private int _dataIndex = 0;
    
    #endregion

    private void Awake()
    {
        if (Instance) { DestroyImmediate(this); return; }
        Instance = this; DontDestroyOnLoad(this.gameObject);
        AudioSettings.GetDSPBufferSize(out _bufferSize, out _numBuffers);
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.outputAudioMixerGroup = MIDIMixerSettings.instance.instrument;
        _outputSampleRate = AudioSettings.outputSampleRate;
        _audioSource.bypassEffects = false;
        _audioSource.bypassListenerEffects = true;
        _audioSource.bypassReverbZones = true;
        _audioSource.priority = 0;
    }

    private void OnApplicationQuit() => CleanUp();
    private void OnDestroy() => CleanUp();
   
    private void CleanUp() { initialized = false; Instance = null; }
    private void OnDisable() { isEnabled = false; }
    private void OnEnable() { isEnabled = true; }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => MIDI.initialized);
        InitSynth();
    }

    private bool InitSynth()
    {
        if (initialized) return false;
        if (!_audioSource) return false;
        if (_synth != null) return false;

        // Get number of channels
        _channels = AudioSettings.driverCapabilities.ToString() == "Mono" ? 1 : 2;

        // Create synth //
        var sampleRate = MIDISynthSettings.GetPlatformSettings().sampleRate;
        _sampleRateDivider = (int)MIDISynthSettings.GetPlatformSettings().outputSampleRateDivider; 
        _synth = new Synthesizer(sampleRate, _channels, _bufferSize / _numBuffers, _numBuffers, 64);

        // Load bank data //
        if(!MIDI.soundFontAsset) { Debug.LogError("MU | CSharpSynth | MIDI.soundFontAsset is empty!!!"); return false; }
        var bankData = MIDI.soundFontAsset ? MIDI.soundFontAsset.bytes : null;
        if (bankData == null) return false;
        
        _synth.LoadBank(new MFile(bankData, _soundBank));
        // Need to do this for bank to load properly, don't know why //
        _synth.ResetSynthControls();
        
        initialized = true;
        return true;
    }

    // Called when audio filter needs more sound data
    private void OnAudioFilterRead(float[] data, int channels)
    {
        // Do nothing if play not enabled
        // This flag is raised/lowered when user starts/stops play
        // Helps avoids thread finding synth in state of shutting down
        if (!initialized || !active || !isEnabled) return;
         
        if (allSoundsOff)
        {
            _synth?.NoteOffAll(true);
            _synth?.ResetSynthControls();
            _synth?.ResetPrograms();

            allSoundsOff = false;
        }

        while (queue.Dequeue(ref _midiMessage)) { _synth?.ProcessMidiMessage(_midiMessage.channel, _midiMessage.command, _midiMessage.data1, _midiMessage.data2); }

        _dataIndex = 0;
        while (_dataIndex < data.Length)
        {
            if (_currentBuffer == null || _currentBufferIndex >= _currentBuffer.Length)
            {
                _synth?.GetNext();

                if (_sampleRateDivider == 1)
                {
                    _currentBuffer = _synth?.WorkingBuffer;

                    for (var i = 0; i < _currentBuffer.Length; i++)
                    {
                        _currentBuffer[i] *= volume * 10f;
                    }
                }
                else
                {
                    _tempBuffer = _synth?.WorkingBuffer;

                    if(_currentBuffer.Length != _tempBuffer.Length) _currentBuffer = new float[_tempBuffer.Length * _sampleRateDivider];

                    for (var i = 0; i < _tempBuffer.Length; i += channels)
                    {
                        for (var ch = 0; ch < channels; ch++)
                        {
                            for (var d = 0; d < _sampleRateDivider; d++)
                            {
                                _currentBuffer[i * _sampleRateDivider + ch + d] = _tempBuffer[i + ch] * volume * 10f;
                            }
                        }
                    }
                }

                _currentBufferIndex = 0;
            }

            _length = Mathf.Min(_currentBuffer.Length - _currentBufferIndex, data.Length - _dataIndex);
            System.Array.Copy(_currentBuffer, _currentBufferIndex, data, _dataIndex, _length);
            _currentBufferIndex += _length;
            _dataIndex += _length;
        }

        if (record) fileStream?.ConvertAndWrite(data);
    }
}
