using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public partial class LangVoice2Word : EditorWindow
{
    private List<DragObject> m_Data = new List<DragObject>();
    private bool doRepaint = false;
    //private Rect dropTargetRect = new Rect(10.0f, 10.0f, 30.0f, 30.0f);

    public class DragObject : GUIDraggableObject
    // This class just has the capability of being dragged in GUI - it could be any type of generic data class
    {
        public bool positionInitialized = false;
        public string text;
        public int wordIndex;
        public int rowIndex;
        public Vector2 wordSize;
        public Vector2 timeSize;
        public float time = 0f;
        public float totalTime = 0f;
        public int totalRows = 0;
        private string timeString = "00:00.00";
        public Vector2 linePosition = Vector2.zero;

        public int lineHeight = 150;
        public int lineWidth = 2;
        public Color lineColor = Color.red;
        public bool highlighted = false;
        public Color highlightColor = Color.yellow;


        public float xMIN = 0;
        public float xMAX = 100;

        public Action<DragObject> playEvent;

        GUIStyle boxStyle;
        GUIStyle buttonStyle;
        //GUIStyle labelStyle;

        public DragObject(string text, int wordIndex, Vector2 position) : base(position)
        {
            this.text = text;
            this.wordIndex = wordIndex;
            buttonStyle = new GUIStyle(GUI.skin.GetStyle("Button"));
            boxStyle = new GUIStyle(GUI.skin.GetStyle("Box"));
            boxStyle.normal.textColor = Color.white;
            //labelStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            timeSize = buttonStyle.CalcSize(new GUIContent(timeString));
            wordSize = buttonStyle.CalcSize(new GUIContent(text));
        }

        public DragObject(string text, int wordIndex, float time)
        {
            this.text = text;
            this.wordIndex = wordIndex;
            this.time = time;
            buttonStyle = new GUIStyle(GUI.skin.GetStyle("Button"));
            boxStyle = new GUIStyle(GUI.skin.GetStyle("Box"));
            boxStyle.normal.textColor = Color.white;
            //labelStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            timeSize = buttonStyle.CalcSize(new GUIContent(timeString));
            wordSize = buttonStyle.CalcSize(new GUIContent(text));
        }

        public void OnGUI()
        {
            Rect drawRect = new Rect(m_Position.x, m_Position.y, timeSize.x, timeSize.y), dragRect;

            timeString = "00:00.00";

            if (time > 0) timeString = DateTime.MinValue.AddSeconds(time).ToString("mm:ss.ff");

            wordSize = buttonStyle.CalcSize(new GUIContent(text));
            timeSize = buttonStyle.CalcSize(new GUIContent(timeString));

            linePosition = new Vector2(m_Position.x + timeSize.x / 2f, m_Position.y);

            Drawing.DrawNodeCurve(linePosition, new Vector2(linePosition.x, linePosition.y - lineHeight), lineColor, 3, 0, false);

            if (highlighted)
            {
                Color background = GUI.backgroundColor;
                Color content = GUI.contentColor;
                Color normal = GUI.color;

                GUI.backgroundColor = highlightColor;
                GUI.contentColor = highlightColor; ;
                GUI.color = highlightColor;

                GUI.Box(new Rect(linePosition.x - wordSize.x / 2f - 10, m_Position.y - lineHeight - timeSize.y - 10, wordSize.x + 20, wordSize.y + 20), text, buttonStyle);

                GUI.backgroundColor = background;
                GUI.contentColor = content; ;
                GUI.color = normal;
            }
            else
            {
                if (GUI.Button(new Rect(linePosition.x - wordSize.x / 2f, m_Position.y - lineHeight - timeSize.y, wordSize.x, wordSize.y), text))
                {
                    if (playEvent != null) playEvent(this);
                }
            }

            GUILayout.BeginArea(drawRect);
            GUILayout.Box(timeString, buttonStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            dragRect = GUILayoutUtility.GetLastRect();
            dragRect = new Rect(dragRect.x + m_Position.x, dragRect.y + m_Position.y, dragRect.width, dragRect.height);
            GUILayout.EndArea();

            Drag(dragRect);

            if (positionInitialized)
            {
                if (m_Position.x + timeSize.x / 2f < xMIN)
                {
                    if (rowIndex > 0)
                    {
                        rowIndex--;
                        positionInitialized = false;
                        Dragging = false;
                    }
                    else
                    {

                        //Dragging = false;
                    }
                }
                else if (m_Position.x + timeSize.x / 2f > xMAX)
                {
                    if (rowIndex < totalRows - 1)
                    {
                        rowIndex++;
                        positionInitialized = false;
                        Dragging = false;
                    }
                    else
                    {
                        //Dragging = false;
                    }
                }
            }

            m_Position = new Vector2(Mathf.Clamp(m_Position.x, xMIN - timeSize.x / 2f, xMAX - timeSize.x / 2f), m_Position.y);
        }
    }

    void OnTimeBar()
    {
        if (audioTime > 0)
        {
            int waveformIndex = GetWaveformIndexFormTime(audioTime);
            float x = TimeToWidth(audioTime, waveformIndex);
            float y = (waveformTop + waveformHeight + waveformBottom + box.border.top + box.border.bottom) * (waveformIndex) + waveformTop + box.border.top + waveformHeight;
            Vector2 position = new Vector2(x, y);
            Drawing.DrawNodeCurve(position, new Vector2(position.x, position.y - waveformHeight), timeColor, 3, 0, false);
        }
    }

    void OnGUIWords()
    {
        if (m_Data == null) return;
        if (waveforms.Count == 0) return;

        //DragObject dropDead;
        DragObject toFront;
        bool previousState, flipRepaint;
        Color color;

        //dropDead = null;
        toFront = null;
        doRepaint = false;
        flipRepaint = false;


        foreach (DragObject data in m_Data)
        {
            if (data == null) continue;

            if (data.rowIndex > waveforms.Count - 1) continue;

            if (waveforms[data.rowIndex] == null) continue;

            float y = (waveformTop + waveformHeight + waveformBottom + box.border.top + box.border.bottom) * (data.rowIndex) + waveformTop + box.border.top + waveformHeight;
            float width = waveforms[data.rowIndex].width;

            float timeMIN = data.rowIndex * WidthToTime(renderWidth - waveformBorder);
            if (timeMIN < 0) timeMIN = 0f;
            //float timeMAX = timeMIN + WidthToTime(width);

            data.xMIN = waveformBorder / 2f;
            data.xMAX = waveformBorder / 2f + width;

            if (!data.positionInitialized)
            {
                data.Position = new Vector2(waveformBorder / 2f + TimeToWidth(data.time - timeMIN) - data.timeSize.x / 2f, y);
                data.positionInitialized = true;
            }
            else
            {

                data.Position = new Vector2(data.Position.x, y);
                if (data.isDirty)
                {
                    data.time = timeMIN + WidthToTime(data.linePosition.x - waveformBorder / 2f);
                    if (!data.Dragging) data.isDirty = false;
                }
            }

            previousState = data.Dragging;

            color = GUI.color;
            //
            //if (previousState)
            //{
            //	GUI.color = dropTargetRect.Contains (Event.current.mousePosition) ? Color.red : color;
            //}


            data.OnGUI();

            GUI.color = color;

            if (!data.positionInitialized)
            {
                y = (waveformTop + waveformHeight + waveformBottom + box.border.top + box.border.bottom) * (data.rowIndex) + waveformTop + box.border.top + waveformHeight;
                width = waveforms[data.rowIndex].width;

                timeMIN = data.rowIndex * WidthToTime(renderWidth - waveformBorder);
                if (timeMIN < 0) timeMIN = 0f;
                //timeMAX = timeMIN + WidthToTime(width);

                data.xMIN = waveformBorder / 2f;
                data.xMAX = waveformBorder / 2f + width;

                data.Position = new Vector2(waveformBorder / 2f + Mathf.Clamp(TimeToWidth(data.time - timeMIN), 1, width - 1) - data.timeSize.x / 2f, y);
                data.linePosition = new Vector2(data.Position.x + data.timeSize.x / 2f, data.Position.y);
                data.positionInitialized = true;
                data.time = timeMIN + WidthToTime(data.linePosition.x - waveformBorder / 2f);
            }

            if (data.Dragging)
            {
                doRepaint = true;

                if (m_Data.IndexOf(data) != m_Data.Count - 1)
                {
                    toFront = data;
                }
            }
            else if (previousState)
            {
                flipRepaint = true;

                //if (dropTargetRect.Contains (Event.current.mousePosition))
                //{
                //	dropDead = data;
                //}
            }
        }

        if (toFront != null)
        // Move an object to front if needed
        {
            m_Data.Remove(toFront);
            m_Data.Add(toFront);
        }

        //if (dropDead != null)
        //	// Destroy an object if needed
        //{
        //	m_Data.Remove (dropDead);
        //}

        if (flipRepaint)
        // If some object just stopped being dragged, we should repaing for the state change
        {
            Repaint();
        }
    }
}