/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */

using ForieroEngine.Music.NotationSystem.Classes;
using TMPro;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem
{
	public static class NSSettingsStatic
	{
		public static bool midiOut = true;
		public static float zoomMin = 0.1f;
		public static float zoomMax = 2f;
		public static CanvasRenderMode canvasRenderMode = CanvasRenderMode.Screen;
		public static TextMode textMode = TextMode.Text;
		public static int smuflFontSize = 70;
		public static int textFontSize = 25;
		public static bool fixedCameraHDragMove = false;
		public static bool fixedCameraVDragMove = true;
		public static bool movableCameraHDragMove = true;
		public static bool movableCameraVDragMove = true;	
		public static float movableCameraLerpSpeed = 10;
		public static float notationOffset = 0;
		
		public static Color normalColor = Color.black;
		public static Color selectedColor = Color.green;
		public static Color hoverColor = Color.cyan;
		public static bool checkPassed = false;
		public static int barlinesPixelShiftX = 0;
		public static int barlinesPixelShiftY = 0;
		public static float timeLerpSpeed = 10;
		public static bool colorPassedObjects = false;
		public static Color passedColor = Color.red;
		public static LogEnum addingObjectConstraints = LogEnum.LogError;
		public static ToneNamesEnum noteNamesEnum = ToneNamesEnum.Undefined;
		public static ToneNamesEnum keysNamesEnum = ToneNamesEnum.Undefined;       
#if UNITY_EDITOR || DEVELOPMENT_BUILD
		public static float hiddenObjectsAlpha = NSDebugSettings.instance.hiddenObjects ? 0.3f : 0f;
#else
        public static float hiddenObjectsAlpha = 0;
#endif

        public static void Apply(NSSystemSettings systemSettings)
        {			
			NSPlayback.Zoom = systemSettings.zoom;
			zoomMin = systemSettings.zoomMin;
			zoomMax = systemSettings.zoomMax;
			canvasRenderMode = systemSettings.canvasRenderMode;
			textMode = systemSettings.textMode;
			smuflFontSize = systemSettings.smuflFontSize;
			textFontSize = systemSettings.textFontSize;
			fixedCameraHDragMove = systemSettings.fixedCameraHDragMove;
			fixedCameraVDragMove = systemSettings.fixedCameraVDragMove;
			movableCameraHDragMove = systemSettings.movableCameraHDragMove;
			movableCameraVDragMove = systemSettings.movableCameraVDragMove;
			movableCameraLerpSpeed = systemSettings.movableCameraLerpSpeed;
			normalColor = systemSettings.normalColor;
			selectedColor = systemSettings.selectedColor;
			hoverColor = systemSettings.hoverColor;
			checkPassed = systemSettings.checkPassed;
			barlinesPixelShiftX = systemSettings.barlinesPixelShiftX;
			barlinesPixelShiftY = systemSettings.barlinesPixelShiftY;
			timeLerpSpeed = systemSettings.timeLerpSpeed;
			colorPassedObjects = systemSettings.colorPassedObjects;
			passedColor = systemSettings.passedColor;
			addingObjectConstraints = systemSettings.addingObjectConstraints;			
		}
	}
}
