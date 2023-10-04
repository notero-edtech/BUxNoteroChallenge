using UnityEngine;
using UnityEditor;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMakerEditor;
using HutongGames.PlayMaker;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

[CustomActionEditor (typeof(KaraokeAction))]
public class KaraokeActionEditor : CustomActionEditor
{
	KaraokeAction action;

	public override void OnEnable ()
	{
		action = target as KaraokeAction;
	}

	public override bool OnGUI ()
	{
		bool isDirty = false;

		EditField ("command");

		switch (action.command) {
		case KaraokeAction.CommandEnum.none:
			break;
		case KaraokeAction.CommandEnum.cont:
			DoContinue ();
			break;
		case KaraokeAction.CommandEnum.play:
			DoPlay ();
			break;
		case KaraokeAction.CommandEnum.stop:
			DoStop ();
			break;
		case KaraokeAction.CommandEnum.pause:
			DoPause ();
			break;
		case KaraokeAction.CommandEnum.set_file:
			DoSetFile ();
			break;
		case KaraokeAction.CommandEnum.set_volume:
			DoSetVolume ();
			break;
		case KaraokeAction.CommandEnum.set_speed:
			DoSpeed ();
			break;
		case KaraokeAction.CommandEnum.events:
			DoEvents ();
			break;
		case KaraokeAction.CommandEnum.settings:
			DoSettings ();
			break;
		}

		return isDirty || GUI.changed;
	}

	void DoSpeed ()
	{
		EditField ("floatValue");
	}

	void DoEvents ()
	{
		EditField ("karaokeEvent");
		EditField (action.karaokeEvent.ToString ());
		switch (action.karaokeEvent) {
		case KaraokeAction.EventEnum.OnWord:
			EditField ("commands");
			break;
		}
	}

	void DoContinue ()
	{
		
	}

	void DoPlay ()
	{
		EditField ("pickupBar");
	}

	void DoStop ()
	{

	}

	void DoPause ()
	{

	}

	void DoSetFile ()
	{
		EditField ("midiFile");
		EditField ("music");
		EditField ("vocals");
	}

	void DoSetVolume ()
	{
		EditField ("musicVolume");
		EditField ("vocalsVolume");
	}

	void DoSettings ()
	{
		EditField ("playMusic");
		EditField ("playVocals");
	}
}
