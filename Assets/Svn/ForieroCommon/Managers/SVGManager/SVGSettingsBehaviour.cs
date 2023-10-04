using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ForieroEngine.SVG;

public class SVGSettingsBehaviour : MonoBehaviour
{
	public TextMeshProUGUI text;
	public float delay = 0.3f;

	private void Awake()
	{
		SVGAtlas.OnWatchStartMessage += OnWatchStartMessage;
	}

	int packCount = 0;

	IEnumerator Start()
	{
		yield return new WaitForSeconds(delay);
		packCount = SVGSettings.instance.atlases.Count;
		foreach (SVGAtlas a in SVGSettings.instance.atlases)
		{
			a.Pack(a.loadOnOnset, () =>
			{
				packCount--;
				if (packCount == 0)
				{
					SceneSettings.LoadSceneByCommand("NEXT");
				}
			});
		}
	}

	private void OnDestroy()
	{
		SVGAtlas.OnWatchStartMessage -= OnWatchStartMessage;
	}

	void OnWatchStartMessage(string message)
	{
		if (text)
		{
			text.text = message;
		}
	}
}
