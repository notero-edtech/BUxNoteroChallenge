/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;
using PathologicalGames;
using System.Collections.Generic;
using System.Linq;

namespace ForieroEngine.Music.NotationSystem
{
    public abstract partial class NS : NSObject, IDisposable
    {
        public static bool debug { get { return NSDebugSettings.instance.debug; } }

        public static bool logNotImplementedXmlNode = false;

        public static RoundingEnum lineWidthCalculation = RoundingEnum.Undefined;
        public static RoundingEnum stemWidthCalculation = RoundingEnum.Undefined;
        public static RoundingEnum barLineWidthCalculation = RoundingEnum.Undefined;

        public NSBehaviour nsBehaviour;
        public Canvas backgroundCanvas;
        public Canvas fixedCanvas;
        public RectTransform fixedCanvasRT;
        public Canvas fixedOverlayCanvas;
        public Canvas movableCanvas;
        public RectTransform movableCanvasRT;
        public Canvas movableOverlayCanvas;
        
        public readonly NSSystemSettings nsSystemSettings;
        public readonly NSSystemSpecificSettingsBase nsSystemSpecificSettings;

        public NSObject selectedObject;

        public class Parsing
        {
            public Dictionary<string, NSPart> parts = new ();
        }

        public Parsing parsing = new ();

        public float MinimumDurationWidth => NSSettingsStatic.smuflFontSize;
        public float Pixels => LineSize / 10f;
        public float LineSize => NSSettingsStatic.smuflFontSize / 4f;
        public float LineHalfSize => NSSettingsStatic.smuflFontSize / 8f;
        public float LineQuarterSize => NSSettingsStatic.smuflFontSize / 16f;
        public float LineEightSize => NSSettingsStatic.smuflFontSize / 32f;

        //			<line-width type="stem">0.957</line-width>
        //			<line-width type="beam">5</line-width>
        //			<line-width type="staff">1.25</line-width>
        //			<line-width type="light barline">1.4583</line-width>
        //			<line-width type="heavy barline">5</line-width>
        //			<line-width type="leger">1.875</line-width>
        //			<line-width type="ending">1.4583</line-width>
        //			<line-width type="wedge">0.9375</line-width>
        //			<line-width type="enclosure">1.4583</line-width>
        //			<line-width type="tuplet bracket">1.4583</line-width>
        //			<note-size type="grace">60</note-size>
        //			<note-size type="cue">60</note-size>
        //			<distance type="hyphen">60</distance>
        //			<distance type="beam">8</distance>

        public float LineWidth => Mathf.Clamp((NSSettingsStatic.smuflFontSize / LineHalfSize / 8f).Round(lineWidthCalculation), 1f, 100f);
        public float LedgerLineLength => NSSettingsStatic.smuflFontSize / 2f;
        public float StemWidth =>  Mathf.Clamp((NSSettingsStatic.smuflFontSize / LineHalfSize / 6f).Round(stemWidthCalculation), 1f, 100f);
        public float StemBlur => StemWidth / 2f;
        public float BarLineWidth => Mathf.Clamp((NSSettingsStatic.smuflFontSize / LineHalfSize / 4f).Round(barLineWidthCalculation), 1f, 100f);
        public float BarLineBlur => BarLineWidth / 2f;
        public float BeamWidth => ns.LineHalfSize;
        public float SlurThickness => ns.LineEightSize;
        public float SlurEndsThickness => ns.LineEightSize / 10f;
        public float TieThickness => ns.LineEightSize;
        public float TieEndsThickness => ns.LineEightSize / 10f;
        public float SystemBracketWidth => ns.LineQuarterSize;
        public float BeamVerticalDistance => LineHalfSize + ns.LineHalfSize / 2f;
        public int StaveLineDistance => 6;
        public int PartLineDistance => 11;

        protected NS(NSBehaviour nsBehaviour, NSSystemSettings systemSettings, NSSystemSpecificSettingsBase systemSpecificSettings)
        {
            this.ns = this;
            this.nsSystemSettings = systemSettings;
            this.nsSystemSpecificSettings = systemSpecificSettings;

            this.nsBehaviour = nsBehaviour;
            this.backgroundCanvas = nsBehaviour.backgroundCanvas;
            this.fixedCanvas = nsBehaviour.fixedCanvas;
            this.fixedCanvasRT = nsBehaviour.fixedCanvasRT;
            
            this.movableCanvas = nsBehaviour.movableCanvas;
            this.movableCanvasRT = nsBehaviour.movableCanvasRT;
            //this.overlayCanvas = nsBehaviour.overlayCanvas;

            this.rectTransform = PoolManager.Pools[PoolEnum.NS_FIXED.ToString()].Spawn(nameof(NSObject)) as RectTransform;
            this.rectTransform.name = "NS";
            this.pool = PoolEnum.NS_FIXED;
        }

        public void Dispose()
        {
            NS.instance = null;
            this.DestroyChildren();
            this.rectTransform.name = nameof(NSObject);
            PoolManager.Pools[PoolEnum.NS_FIXED.ToString()].Despawn(ns.rectTransform);
            this.rectTransform = null;
            this.ns = null;
        }

        public virtual void Init()
        {
            NS.instance = this;
        }
        public abstract void Update();
        public abstract void ZoomChanged(float zoom);

        public List<T> GetMidiObjectsSortedByTime<T>() where T : NSObject
        {
            var list = GetObjectsOfType<T>(true);
            return list.Where(item => item.midiData.messages.Any()).OrderBy(item => item.time).Select(item => item).ToList();
        }

        public List<T> GetMidiObjectsSortedByPixelTime<T>() where T : NSObject
        {
            var list = GetObjectsOfType<T>(true);
            return list.Where(item => item.midiData.messages.Any()).OrderBy(item => item.pixelTime).Select(item => item).ToList();
        }

        public List<T> GetPassableObjectsSortedByTime<T>() where T : NSObject
        {
            var list = GetObjectsOfType<T>(true);
            return list.Where(item => item.passable).OrderBy(item => item.time).Select(item => item).ToList();
        }

        public List<T> GetPassableObjectsSortedByPixelTime<T>() where T : NSObject
        {
            var list = GetObjectsOfType<T>(true);
            return list.Where(item => item.passable).OrderBy(item => item.pixelTime).Select(item => item).ToList();
        }

        public void SetStaveAlpha(int staveNumber, float alpha)
        {
            GetObjectsOfType<NSPart>(false)
                .ForEach(
                    p => p.GetObjectsOfType<NSStave>(false)
                    .ForEach(
                        s => { if(s.options.index == staveNumber) s.SetAlpha(alpha, true); })
                );
        } 
    }
}
