/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NSTestSuiteManualToggle : MonoBehaviour
{
	public NSTestSuiteManual.Test test;

	public System.Action<NSTestSuiteManualToggle, bool> onChange;

	void Awake ()
	{
		gameObject.GetComponent<Toggle> ().onValueChanged.AddListener (OnToggleChange);
	}

	void OnToggleChange (bool b)
	{
		if (onChange != null) {
			onChange (this, b);
		}
	}
}
