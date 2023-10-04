using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ForieroEditor.Extensions;

public partial class LangVoiceGenerator : EditorWindow
{
    partial class WordsClip
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

        TextAsset _words;
        public TextAsset words
        {
            set
            {
                if (_words != value)
                {
                    _words = value;
                    InitWords();
                }
            }
            get
            {
                return _words;
            }
        }

        public string text = "";
        
        string audioDescription;
        float audioTime = 0;
        Color timeColor = Color.white;

        List<Texture2D> waveforms = new List<Texture2D>();

        Vector2 waveformScroll = Vector2.zero;
        int waveformPixelsPerSecond = 300;
        float waveformTop = 20f;
        float waveformBottom = 20f;
        float waveformHeight = 150;
        int waveformBorder = 10;

        float renderWidth = 0;

        bool focused = false;

        float WidthToTime(float width)
        {
            return width / waveformPixelsPerSecond;
        }

        float TimeToWidth(float time)
        {
            return Mathf.Floor(time * waveformPixelsPerSecond);
        }

        float TimeToWidth(float time, int waveformIndex)
        {
            return Mathf.Floor(time * waveformPixelsPerSecond) - (waveformIndex * (renderWidth - waveformBorder));
        }

        void PlayWord(DragObject aDragObject)
        {
            float closestTime = clip.length;
            foreach (DragObject d in m_Data)
            {
                if (d.time > aDragObject.time && closestTime > d.time) closestTime = d.time;
            }
            playWord = true;
            playWordFinishTime = closestTime;
            Debug.Log(aDragObject.time);
            clip.PlayClip(aDragObject.time);
        }

        int GetWaveformIndexFormTime(float time)
        {
            int result = -1;
            do
            {
                result++;
                time -= WidthToTime(renderWidth - waveformBorder);

            } while (time > 0);
            return result;
        }

        public void Play()
        {
            if (clip && clip.IsClipPlaying()) return;
            clip?.PlayClip(0);
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
        }

        bool playWord = false;
        float playWordFinishTime = 0f;

        GUIStyle box;

        public void OnGUI()
        {
            box = GUI.skin.GetStyle("Box");
            if (waveforms.Count > 0)
            {

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
                
                waveformPixelsPerSecond = EditorGUILayout.IntField("Pixels/Second", waveformPixelsPerSecond);

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Refresh", GUILayout.Width(80)))
                {
                   // Load(selectedDictionary, Lang.selectedLanguage, recordIds[recordIndex]);
                }

                if (GUILayout.Button("Save", GUILayout.Width(80)))
                {
                    Save();
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Time : ", audioTime.ToString());

                EditorGUILayout.EndHorizontal();

                waveformScroll = GUILayout.BeginScrollView(waveformScroll, GUIStyle.none, GUIStyle.none);

                int waveformIndex = 0;



                foreach (Texture2D waveform in waveforms)
                {
                    if (waveform == null) continue;
                    Rect waveformRect = new Rect(
                        waveformBorder / 2f,
                        (waveformTop + waveformHeight + waveformBottom + box.border.top + box.border.bottom) * (waveformIndex) + waveformTop + box.border.top,
                        waveform.width,
                        waveformHeight);

                    EditorGUI.DrawTextureAlpha(waveformRect, waveform as Texture);

                    GUILayout.Box("", GUILayout.Width(waveformRect.width + box.border.right), GUILayout.Height(waveformTop + waveformHeight + waveformBottom));

                    waveformIndex++;
                }

                OnTimeBar();

                OnGUIWords();

                GUILayout.EndScrollView();
            }
        }

        public void EveryFrameUpdate()
        {
            if (!focused) return;

            if (clip)
            {

                audioTime = clip.GetClipPosition();
                if (playWord && audioTime >= playWordFinishTime)
                {
                    clip?.StopClip();
                    playWord = false;
                }

                if (m_Data.Count > 0)
                {
                    DragObject closestDragObject = m_Data[0];
                    foreach (DragObject d in m_Data)
                    {
                        d.highlighted = false;
                        if (d.time < audioTime && d.time > closestDragObject.time)
                        {
                            closestDragObject = d;
                        }
                    }
                    closestDragObject.highlighted = true;
                }

                window?.Repaint();

                if (!clip.IsClipPlaying())
                {
                    foreach (DragObject d in m_Data)
                    {
                        d.highlighted = false;
                    }
                }
            }
        }
    }         
}