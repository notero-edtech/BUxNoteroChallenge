/* Copyright © Marek Ledvina, Foriero s.r.o. */
using System;
using System.Threading;
using ForieroEngine.Collections.NonBlocking;
using ForieroEngine.MIDIUnified;
using UnityEngine;

public partial class MidiSeqKaraokeThreadedScript : MonoBehaviour, IMidiSender
{    
    partial class ThreadedSequencer
    {
        public enum ThreadSignal
        {
            None, 
            Init,
            Play,
            PlayWithPickupBar,
            Stop,
            Pause,
            Reset,            
        }
                
        partial class SequencerThread
        {
            volatile bool _initialized = false;
            public bool initialized { get { return _initialized; } }
            public volatile bool loading = false;
            
            NonBlockingQueue<ThreadSignal> signals = new NonBlockingQueue<ThreadSignal>();
            public Action<bool> onFinished;

            readonly Thread thread;
            volatile bool terminating = false;

            public volatile bool threaded = false;

            public SequencerThread()
            {               
                thread = new Thread(ThreadRunner);
                thread.Start();                
            }

            void ThreadRunner()
            {                
                do
                {
                    if(threaded) Run();

                    Thread.Sleep(threaded ? 1 : 100);

                } while (!terminating);                
            }

            public void Run()
            {
                ThreadSignal s = ThreadSignal.None;

                while (signals.Dequeue(ref s))
                {
                    if (s == ThreadSignal.None) break;

                    switch (s)
                    {
                        case ThreadSignal.Init: try { Init(); } catch (Exception e) { Debug.LogError("SeqT | Init |" + e.Message); } break;
                        case ThreadSignal.Play: try { Play(); } catch (Exception e) { Debug.LogError("SeqT | Play |" + e.Message); } break;
                        case ThreadSignal.PlayWithPickupBar: try { PlayWithPickupBar(); } catch (Exception e) { Debug.LogError("SeqT | PlayWithPickupBar |" + e.Message); } break;
                        case ThreadSignal.Stop: try { Stop(); } catch (Exception e) { Debug.LogError("SeqT | Stop |" + e.Message); } break;
                        case ThreadSignal.Pause: try { Pause(); } catch (Exception e) { Debug.LogError("SeqT | Pause |" + e.Message); } break;
                        case ThreadSignal.Reset: try { Reset(); } catch (Exception e) { Debug.LogError("SeqT | Reset |" + e.Message); } break;
                    }

                    s = ThreadSignal.None;
                }

                switch (_state)
                {
                    case MidiState.PickUpBar: try { UpdatePickuBar(); } catch (Exception e) { Debug.LogError("SeqT | UpdatePickupBar |" + e.Message); } break;
                    case MidiState.Playing: try { Update(); } catch (Exception e) { Debug.LogError("SeqT | Update |" + e.Message); } break;
                }
            }

            public void SendSignal(ThreadSignal aSignal) => this.signals.Enqueue(aSignal);
                                    
            public void Terminate()
            {
                if (terminating) return;

                terminating = true;
                thread?.Abort();
                thread?.Join();
            }
        }        
    }
}
