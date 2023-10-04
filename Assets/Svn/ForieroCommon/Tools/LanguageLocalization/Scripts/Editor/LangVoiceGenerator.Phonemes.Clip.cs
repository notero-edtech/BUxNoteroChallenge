using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ForieroEditor.Extensions;
using UnityEditor;
using UnityEngine;
using static ForieroEditor.Extensions.ForieroEditorExtensions;
using Debug = UnityEngine.Debug;

public partial class LangVoiceGenerator : EditorWindow
{
    public class PhonemeClip
    {
        enum State
        {
            None,
            Playing,
            Paused
        }

        State state = State.None;

        AudioClip _clip;
        public AudioClip clip
        {
            set
            {
                if (_clip != value)
                {
                    _clip = value;
                    InitAudio();
                }
            }
            get
            {
                return _clip;
            }
        }

        TextAsset _phonemes;
        public TextAsset phonemes
        {
            set
            {
                if (_phonemes != value)
                {
                    _phonemes = value;
                    InitPhonemes();
                }
            }
            get
            {
                return _phonemes;
            }
        }

        public bool isDragging
        {
            get
            {
                for (int i = 0; i < phonemeList.Count; i++)
                {
                    if (phonemeList[i].Dragging) return true;
                }
                return false;
            }
        }

        bool wasPlaying = false;
        bool wasDragging = false;

        public void EveryFrameUpdate()
        {
            if (isDragging)
            {
                wasDragging = true;
                window.Repaint();
                return;
            }
            else if (wasDragging)
            {
                wasDragging = false;
                window.Repaint();
                return;
            }

            if (clip && clip.IsClipPlaying())
            {
                wasPlaying = true;
                window.Repaint();
                return;
            }
            else if (wasPlaying)
            {
                wasPlaying = false;
                window.Repaint();
                return;
            }

            if (rightMouseDown)
            {
                window.Repaint();
            }
        }

        public float[] minMaxData = null;
        public Rect clipRect;

        class Phoneme : GUIDraggableObject
        {
            public float time;
            public Lang.PhonemeEnum phoneme;

            float draggingTime = 0;
            static Phoneme selectedPhoneme = null;
            //            float clipWidth = 0;

            public bool remove = false;

            public Texture GetMouthTexture()
            {
                switch (phoneme)
                {
                    case Lang.PhonemeEnum.RhubarbA: return rhubarb_a;
                    case Lang.PhonemeEnum.RhubarbB: return rhubarb_b;
                    case Lang.PhonemeEnum.RhubarbC: return rhubarb_c;
                    case Lang.PhonemeEnum.RhubarbD: return rhubarb_d;
                    case Lang.PhonemeEnum.RhubarbE: return rhubarb_e;
                    case Lang.PhonemeEnum.RhubarbF: return rhubarb_f;
                    case Lang.PhonemeEnum.RhubarbG: return rhubarb_g;
                    case Lang.PhonemeEnum.RhubarbH: return rhubarb_h;
                    case Lang.PhonemeEnum.RhubarbX: return rhubarb_x;
                }

                return rhubarb_a;
            }

            public void Draw(Rect clipRect, float totalTime)
            {
                var timeX = clipRect.x + Mathf.Clamp(time + draggingTime, 0, totalTime) / totalTime * clipRect.width;

                Drawing.DrawLine(new Vector2(timeX, clipRect.y), new Vector2(timeX, clipRect.y + clipRect.height + 3 + 10), Color.white, 2);

                var rect = new Rect(timeX - 10, clipRect.y + 3 + clipRect.height, 20, 20);

                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && rect.Contains(Event.current.mousePosition))
                {
                    selectedPhoneme = this;
                }

                var c = GUI.backgroundColor;

                if (selectedPhoneme == this) GUI.backgroundColor = Color.green;

                GUI.Box(rect, phoneme.ToString().Replace("Rhubarb", ""), GUI.skin.GetStyle("Button"));

                GUI.backgroundColor = c;

                remove = Event.current.type == EventType.MouseDown && Event.current.button == 1 && rect.Contains(Event.current.mousePosition);



                this.Drag(rect);

                if (this.Dragging)
                {
                    if (!Mathf.Approximately(clipRect.width, 1f))
                    {
                        draggingTime = this.Position.x / clipRect.width * totalTime;
                        //Debug.Log("Position : " + this.Position.x);
                        //Debug.Log("Width : " + clipRect.width);
                        //Debug.Log("Total Time : " + totalTime);
                        //Debug.Log("Dragtime : " + draggingTime);
                    }
                }
                else if (this.isDirty)
                {
                    this.isDirty = false;
                    time += draggingTime;
                    this.Position = Vector2.zero;
                    draggingTime = 0;
                }
            }
        }

        List<Phoneme> phonemeList = new List<Phoneme>();

        float? Time => clip?.GetClipPosition();
        float _time = 0;
        public float time => Time == null ? 0 : (clip.IsClipPlaying() ? (float)Time : _time);
        string TimeString()
        {
            TimeSpan ts = TimeSpan.FromSeconds(time);
            return string.Format("{0:00}:{1:00}.{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds);
        }

        int? ChannelCount => clip?.GetChannelCount();
        string ChannelString()
        {
            int c = ChannelCount ?? 0;
            return c == 1 ? "Mono" : c == 2 ? "Stereo" : (c - 1).ToString() + ".1";
        }

        int? Frequency => clip?.GetFrequency();
        string FrequencyString()
        {
            return Frequency == null ? "0 Hz" : (Frequency.ToString() + " Hz");
        }

        double? Duration => clip?.GetDuration();
        public float duration = 0;
        string DurationString()
        {
            TimeSpan ts = TimeSpan.FromSeconds(duration);
            return string.Format("{0:00}:{1:00}.{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds);
        }

        string StartTimeString()
        {
            TimeSpan ts = TimeSpan.FromSeconds(startTime);
            return string.Format("{0:00}:{1:00}.{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds);
        }

        string EndTimeString()
        {
            TimeSpan ts = TimeSpan.FromSeconds(endTime);
            return string.Format("{0:00}:{1:00}.{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds);
        }

        string TimeDisplayString()
        {
            return TimeString() + " / " + DurationString();
        }

        string TimeSelectionDisplayString()
        {
            return StartTimeString() + " / " + EndTimeString();
        }

        public bool rightMouseDown = false;
        bool? leftMouseDown = null;

        float timeX = 0;
        float mouseDownX = 0;
        float regionWidth = 0f;
        float startTime = 0;
        float endTime = 0;

        void InitAudio()
        {
            minMaxData = clip?.GetMinMaxData();
            Reset();
        }

        void InitPhonemes()
        {
            if (!phonemes) return;

            save = false;

            phonemeList.Clear();

            string[] lines = Regex.Split(phonemes.text, "\r\n|\r|\n");
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line.Trim())) continue;

                string[] tabs = line.Split('\t');
                var p = new Phoneme();
                p.time = float.Parse(tabs[0].Trim());
                p.phoneme = Lang.PhonemeEnum.RhubarbX;

                if(Enum.TryParse<Lang.PhonemeEnum>("Rhubarb" + tabs[1].Trim(), out var r)){
                    p.phoneme = r;
                } else
                {
                    Debug.LogError("RHUBARB Could not parse : " + tabs[1].Trim());
                }

                phonemeList.Add(p);
            }
        }

        void Reset()
        {
            duration = Duration == null ? 0 : (float)Duration / 1000f;

            mouseDownX = timeX = startTime = _time = 0;
            endTime = duration;
            rightMouseDown = false;
            regionWidth = 0f;
        }

        public void Play()
        {
            if (clip && clip.IsClipPlaying()) return;
            clip?.PlayClip(startTime);
            if (clip) state = State.Playing;
        }

        public void Pause()
        {
            clip?.PauseClip();
            if (clip) state = clip.IsClipPlaying() ? State.Paused : State.None;
        }

        public void Resume()
        {
            clip?.ResumeClip();
            if (clip) state = clip.IsClipPlaying() ? State.Playing : state;
        }

        public void Stop()
        {
            clip?.StopClip();
            if (clip) state = State.None;
            Reset();
        }

        public void Save()
        {
            save = false;

            if (!phonemes) return;

            string s = "";
            foreach (var p in phonemeList)
            {
                s += p.time.ToString("N3") + "\t" + p.phoneme.ToString().Replace("Rhubarb", "") + System.Environment.NewLine;
            }

            File.WriteAllText(AssetDatabase.GetAssetPath(phonemes), s);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(phonemes));
        }

        bool save = false;
        Phoneme removePhoneme = null;
        Vector2 waveView = Vector2.zero;
        Vector2 waveViewVelocity = Vector2.zero;

        float zoom = 1f;
                
        public void OnGUI()
        {
            DrawSeparator();
               
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(TimeDisplayString());
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(TimeSelectionDisplayString());
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(state == State.None ? "" : state.ToString());
            GUILayout.FlexibleSpace();
            zoom = EditorGUILayout.Slider(zoom, 1, 10);
            GUILayout.FlexibleSpace();

            var color = GUI.backgroundColor;
            GUI.backgroundColor = save ? Color.red : color;

            if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                Save();
            }

            GUI.backgroundColor = color;

            EditorGUILayout.EndHorizontal();

            DrawSeparator();

            if (timeX > (window.position.width / 2f + waveView.x) && state == State.Playing)
            //if (zoom > 1 && state == State.Playing)
            {
                waveView = Vector2.SmoothDamp(waveView, new Vector2(timeX - window.position.width / 2f, 0), ref waveViewVelocity, 0.2f);
            }
            else
            {
                waveViewVelocity = Vector2.zero;
            }

            waveView = GUILayout.BeginScrollView(waveView, GUILayout.Height(160));
            
            clipRect = GUILayoutUtility.GetRect(window.position.width * zoom, 100);

            if (clip == null)
            {
                duration = 0;
                GUILayout.EndScrollView();
                return;
            }

            if (time >= endTime)
            {
                clip?.StopClip();
            }

            GUILayout.Label("", GUILayout.Height(20));
            DrawSeparator();

            var curveColor = new Color(255 / 255f, 168 / 255f, 7 / 255f);
            int numChannels = clip.channels;
            int numSamples = minMaxData.Length / (2 * numChannels);
            float h = (float)clipRect.height / numChannels;
            for (int channel = 0; channel < numChannels; channel++)
            {
                var channelRect = new Rect(clipRect.x, clipRect.y + channel * h, clipRect.width, h);
                AudioCurveRendering.DrawMinMaxFilledCurve(
                    channelRect,
                    delegate (float x, out Color col, out float minValue, out float maxValue)
                    {
                        col = curveColor;
                        float p = Mathf.Clamp(x * (numSamples - 2), 0.0f, numSamples - 2);
                        int i = (int)Mathf.Floor(p);
                        int offset1 = (i * numChannels + channel) * 2;
                        int offset2 = offset1 + numChannels * 2;
                        minValue = Mathf.Min(minMaxData[offset1 + 1], minMaxData[offset2 + 1]);
                        maxValue = Mathf.Max(minMaxData[offset1 + 0], minMaxData[offset2 + 0]);
                        if (minValue > maxValue) { float tmp = minValue; minValue = maxValue; maxValue = tmp; }
                    }
                );
            }

            if (!rightMouseDown) timeX = time / duration * clipRect.width;

            if (Event.current.type == EventType.MouseDown && clipRect.Contains(Event.current.mousePosition))
            {
                clip?.StopClip();
                state = State.None;
                _time = startTime = (Event.current.mousePosition.x - clipRect.x) / clipRect.width * duration;
                timeX = time / duration * clipRect.width;
                clip?.SetClipPosition(_time);
                mouseDownX = timeX;
                regionWidth = 0;

                if (Event.current.button == 0)
                {
                    rightMouseDown = true;
                    Event.current.Use();
                }
                else if (Event.current.button == 1)
                {
                    leftMouseDown = true;
                    Event.current.Use();
                }
            }
            else if (Event.current.type == EventType.MouseDrag && rightMouseDown)
            {
                regionWidth = mouseDownX - Event.current.mousePosition.x - clipRect.x;

                if (regionWidth > 0)
                {
                    regionWidth = Mathf.Clamp(regionWidth, 0, mouseDownX);
                    timeX = Mathf.Clamp(mouseDownX - regionWidth, 0, clipRect.width);

                }
                else
                {
                    regionWidth = Mathf.Clamp(regionWidth, -(clipRect.width - mouseDownX), 0);
                    timeX = time / duration * clipRect.width;
                }
                Event.current.Use();
            }
            else if (Event.current.type == EventType.MouseUp && rightMouseDown)
            {
                if (Mathf.Approximately(regionWidth, 0))
                {
                    endTime = duration;
                }
                else
                {
                    if (regionWidth > 0)
                    {
                        timeX = mouseDownX - regionWidth;
                        _time = timeX / clipRect.width * duration;
                        clip?.SetClipPosition(_time);
                    }
                    else
                    {
                        timeX = time / duration * clipRect.width;
                    }

                    startTime = _time;
                    endTime = _time + Mathf.Abs(regionWidth) / clipRect.width * duration;
                }

                rightMouseDown = false;
                Event.current.Use();
            }

            removePhoneme = null;

            for (int i = 0; i < phonemeList.Count; i++)
            {
                phonemeList[i].Draw(clipRect, duration);
                if (phonemeList[i].remove) removePhoneme = phonemeList[i];
            }

            if (removePhoneme != null)
            {
                phonemeList.Remove(removePhoneme);
                save = true;
            }

            if (isDragging) save = true;

            if (leftMouseDown != null && leftMouseDown == true)
            {
                leftMouseDown = false;

            }
            else if (leftMouseDown == false)
            {
                GenericMenu menu = new GenericMenu();

                void MenuExecute(object o)
                {
                    string p = o as string;
                    Phoneme phoneme = new Phoneme();
                    phoneme.time = time;
                    phoneme.phoneme = (Lang.PhonemeEnum)System.Enum.Parse(typeof(Lang.PhonemeEnum), "Rhubarb" + p);
                    phonemeList.Add(phoneme);
                    phonemeList.Sort((a, b) => a.time.CompareTo(b.time));
                    save = true;
                }

                menu.AddItem(new GUIContent("A"), false, MenuExecute, "A");
                menu.AddItem(new GUIContent("B"), false, MenuExecute, "B");
                menu.AddItem(new GUIContent("C"), false, MenuExecute, "C");
                menu.AddItem(new GUIContent("D"), false, MenuExecute, "D");
                menu.AddItem(new GUIContent("E"), false, MenuExecute, "E");
                menu.AddItem(new GUIContent("F"), false, MenuExecute, "F");
                menu.AddItem(new GUIContent("G"), false, MenuExecute, "G");
                menu.AddItem(new GUIContent("H"), false, MenuExecute, "H");
                menu.AddItem(new GUIContent("X"), false, MenuExecute, "X");
                menu.ShowAsContext();
                leftMouseDown = null;
            }

            if (!Mathf.Approximately(regionWidth, 0))
            {
                Drawing.DrawLine(new Vector2(mouseDownX - regionWidth / 2f, clipRect.y), new Vector2(mouseDownX - regionWidth / 2f, clipRect.y + clipRect.height), Color.blue.A(0.3f), regionWidth);
            }

            Drawing.DrawLine(new Vector2(timeX, clipRect.y), new Vector2(timeX, clipRect.y + clipRect.height), Color.yellow, 3);
                                                
            GUILayout.EndScrollView();
                                                
            DrawSeparator();

            var r = GUILayoutUtility.GetLastRect();

            EditorGUI.DrawTextureTransparent(new Rect(r.width / 2f - 100 + r.x, r.y + 50, 200, 200), GetMouthTexture());

            if (clip && !clip.IsClipPlaying() && state == State.Playing) state = State.None;

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();

                var s = "";

                switch (state)
                {
                    case State.None:
                        s = "Play";
                        break;
                    case State.Playing:
                        s = "Pause";
                        break;
                    case State.Paused:
                        s = "Resume";
                        break;
                }

                if (GUILayout.Button(s, EditorStyles.toolbarButton, GUILayout.Width(80)))
                {
                    switch (state)
                    {
                        case State.None:
                            Play();
                            break;
                        case State.Playing:
                            Pause();
                            break;
                        case State.Paused:
                            Resume();
                            break;
                    }
                }
                if (GUILayout.Button("Stop", EditorStyles.toolbarButton, GUILayout.Width(80))) { Stop(); }

                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            DrawSeparator();            
        }

        Texture GetMouthTexture()
        {
            for (int i = phonemeList.Count - 1; i >= 0; i--)
            {
                if (phonemeList[i].time > time) continue;
                return phonemeList[i].GetMouthTexture();
            }


            return rhubarb_x;
        }
    }
}