using UnityEngine;
using System.Collections;

public class GUIDraggableObject
{

	public bool isDirty = false;
	protected Vector2 m_Position = Vector2.zero;
	private Vector2 m_DragStart = Vector2.zero;
	private bool m_Dragging = false;

	public GUIDraggableObject (Vector2 position)
	{
		m_Position = position;
	}

	public GUIDraggableObject ()
	{

	}

	public bool Dragging
	{
		get
		{
			return m_Dragging;
		}
		set
		{
			m_Dragging = value;
		}
	}

	public Vector2 Position
	{
		get
		{
			return m_Position;
		}

		set
		{
			m_Position = value;
		}
	}

	public void Drag (Rect draggingRect)
	{
		if (Event.current.type == EventType.MouseUp)
		{
			m_Dragging = false;
		}
		else if (Event.current.type == EventType.MouseDown && draggingRect.Contains (Event.current.mousePosition))
		{
			isDirty = true;
			m_Dragging = true;
			m_DragStart = Event.current.mousePosition - m_Position;
			Event.current.Use();
		}

		if (m_Dragging)
		{
			m_Position = Event.current.mousePosition - m_DragStart;
		}
	}
}