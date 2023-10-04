/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem.Extensions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/**
 * Draws a graph of lines by means of OnGUI 
 */ 
public class LineGenerator : MonoBehaviour, ISingleton
{
	public TextMeshProUGUI fpsText;
	[SerializeField] Texture2D _texture;
	public Color fpsGraphColor = Color.green;
	[Tooltip ("High value is considered to be any value grater 60fps")]
	public Color fpsHighLineIndicatorColor = Color.yellow;
	[Tooltip ("Normal value is considered to be 60fps")]
	public Color fpsNormalLineIndicatorColor = Color.red;

	private const float TOP_NORMAL_MARGIN_SCALE = 0.3f;
	private const float TOP_MAX_MARGIN_SCALE = 0;
	private const float FPS_NORMAL = 60f;
	private const int MIN_SIZE = 100;

	private int _amount = 100;
	private float _deltaTime = 0.0f;
	private string _fpsString = "";
	private float _fullHeight = 0f;
	private float _maxHeight = 0f;
	private bool _canStart = false;
	private Queue<float> _heightsQueue;
	private Vector2 _bottomLeft = new Vector2 (0f, 0f);

	private RectTransform rt;
	private Rect _guiRect = new Rect ();

	private IEnumerator Start ()
	{
		rt = GetComponent<RectTransform> ();
		yield return Initialize();
	}

	private IEnumerator Initialize ()
	{
		// Debug.Log("Routine started, we are going to skip a frame and then come back");
		yield return null;
		_guiRect = rt.GetGUIRect();
		_bottomLeft = _guiRect.min;
        _amount = (int)_guiRect.width;
		_fullHeight = _guiRect.height - (_guiRect.height * TOP_NORMAL_MARGIN_SCALE);
		_maxHeight = _guiRect.height - (_guiRect.height * TOP_MAX_MARGIN_SCALE);
		InitHeightsQueue ();
		_canStart = true;
	}

	/**
     * Creates a queue of zeroes with a size of LineRenderer image width
     */ 
	private void InitHeightsQueue ()
	{
		_heightsQueue = new Queue<float> ();
		for (var i = 0; i < _amount; i++) { _heightsQueue.Enqueue (0f); }
	}

	public void Update ()
	{
		if (!_canStart) return;
		
		_guiRect = rt.GetGUIRect();
		_bottomLeft = _guiRect.min;
		//Calculate FPS
		_deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
		var msec = _deltaTime * 1000.0f;
		var fps = 1.0f / _deltaTime;
		_fpsString = $"{msec:0.0} ms ({fps:0.} fps)";

		//Display in Text field in Canvas
		if (fpsText) { fpsText.text = _fpsString; }

		//Remove first line in order and add a new line to the end
		_heightsQueue.Dequeue ();
		var lineHeight = _fullHeight * fps / FPS_NORMAL;  // where fps / FPS_NORMAL is a coefficient of current fps value to normal 60 fps
		if (lineHeight > _maxHeight) { lineHeight = _maxHeight; }
		_heightsQueue.Enqueue (lineHeight);
	}

	/**
     * Draws texture with a size 2x2 px in a rectangle with a size 1xlineHeight px 
     */
	public void OnGUI ()
	{
		if (_canStart) {
			
			//GUI.Box(_guiRect, "");
			
			float counter = 0;
			foreach (var lineHeight in _heightsQueue) {
				// Position of GUI layer is top left corner of the screen
				var posX = counter + _guiRect.x;
				var posY = _bottomLeft.y + _guiRect.height;
				GUI.color = fpsGraphColor;
				GUI.DrawTexture (new Rect (new Vector2 (posX, posY), new Vector2 (1, -lineHeight)), _texture);
				counter++;
			}
		}

		DrawNormalFpsLineIndicator ();
	}

	/**
     * Draws a normal fps line indicator with width = rectangle width, height=1px
     * and a high fps line indicator above it.
     * Both lines must be drawn above the fps graph rectangular area.
    */
	private void DrawNormalFpsLineIndicator ()
	{
		GUI.color = fpsNormalLineIndicatorColor;
		GUI.DrawTexture (new Rect (new Vector2 (_bottomLeft.x, _bottomLeft.y + _guiRect.height - _fullHeight), new Vector2 ((float)_amount, 1)), _texture);

		GUI.color = fpsHighLineIndicatorColor;
		GUI.DrawTexture (new Rect (new Vector2 (_bottomLeft.x, _bottomLeft.y + _guiRect.height - _maxHeight), new Vector2 ((float)_amount, 1)), _texture);
	}
}
