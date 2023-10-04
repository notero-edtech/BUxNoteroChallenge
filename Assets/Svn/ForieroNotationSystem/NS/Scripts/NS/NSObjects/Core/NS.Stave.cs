/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem.Extensions;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public partial class NSStave : NSObject
    {
        #region options

        public class Options : INSObjectOptions<Options>
        {
            public string id = "1";
            public int index = 0;
            public StaveEnum staveEnum = StaveEnum.Undefined;
            public float width = 100;
            public bool background = false;
            public Color backgroundColor = Color.white;
            public float lineDistance = 6;
            public bool systemBracket = false;
            
            public void Reset()
            {
                id = "1";
                index = 0;
                staveEnum = StaveEnum.Undefined;
                width = 100;
                background = false;
                backgroundColor = Color.white;
                lineDistance = 6;
                systemBracket = false;
            }

            public void CopyValuesFrom(Options o)
            {
                id = o.id;
                index = o.index;
                staveEnum = o.staveEnum;
                width = o.width;
                background = o.background;
                backgroundColor = o.backgroundColor;
                lineDistance = o.lineDistance;
                systemBracket = o.systemBracket;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        #endregion

        public NSStaveLines staveLines;
        public NSObjectImage backgroundImage = null;

        public NSBracket leftSystemBracket;
        public NSBracket rightSystemBracket;
        public NSBarLine leftBarLine;
        public NSBarLine rightBarLine;

        public NSMetronomeMark metronomeMark;

        public NSClef clef;
        public NSKeySignature keySignature;
        public NSTimeSignature timeSignature;

        public class Parsing
        {
            public readonly NSClef.Options clef = new () { clefEnum = ClefEnum.Treble };
            public readonly NSTimeSignature.Options timeSignature = new () { timeSignatureEnum = TimeSignatureEnum.Common };
            public readonly NSKeySignature.Options keySignature = new () { keySignatureEnum = KeySignatureEnum.CMaj_AMin, keyModeEnum = KeyModeEnum.Major };
        }

        public Parsing parsing = new Parsing();

        /// <summary>
        /// We need global dictionary of ledgerlines in order to not draw ledgerlines twice on the same x position
        /// </summary>
        public readonly SortedDictionary<float, NSLedgerLines> ledgerLines = new ();

        public readonly List<NSObject> collorables = new ();

        /// <summary>
        /// Bottom edge of the stave.
        /// </summary>
        /// <value>The bottom edge.</value>
        public float bottomEdge => -topEdge;

        /// <summary>
        /// Top edge of the stave.
        /// </summary>
        /// <value>The top edge.</value>
        public float topEdge => options.staveEnum == StaveEnum.Undefined ? 0 : ((float)options.staveEnum - 1) * ns.LineHalfSize;
                
        public override void Commit()
        {
            DestroyChildren();

            base.Commit();

            if (options.staveEnum == StaveEnum.Undefined) { return; }

            rectWidth = options.width;
            rectHeight = ns.LineSize * ((float)options.staveEnum - 1f);

            if (options.background)
            {
                backgroundImage = this.AddObject<NSObjectImage>(pool);
                backgroundImage.image.color = options.backgroundColor;

                backgroundImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);

                backgroundImage.rectTransform.anchoredPosition += new Vector2(0, ns.LineSize * 2f);
                backgroundImage.SendVisuallyBack();

                backgroundImage.followParentRectWidth = true;
                backgroundImage.followParentRectHeight = true;
                backgroundImage.Commit();

                // (ns.stemWidth).Round(RoundingEnum.Floor) //

                backgroundImage.Shift(DirectionEnum.Down, true, 2);
            }

            if (options.staveEnum != StaveEnum.Undefined)
            {
                staveLines = this.AddObject<NSStaveLines>( pool);
                staveLines.options.staveEnum = options.staveEnum;
                staveLines.followParentRectWidth = true;
                staveLines.followParentRectHeight = false;
                staveLines.AlignToParent(true, true);
                staveLines.Commit();
                if (backgroundImage) staveLines.SendVisuallyFront(backgroundImage, true);

                staveLines.PixelShiftX(-rectWidth / 2f, true);
            }

            if (options.systemBracket)
            {
                NSBracket._options.Reset();
                SetLeftSystemBracket(NSBracket._options);
                NSBracket._options.Reset();
                SetRightSystemBracket(NSBracket._options);
            }

            UpdateRectWidthAndHeight(true);
        }

        public override void Reset()
        {
            base.Reset();
            options.Reset();

            staveLines = null;

            clef = null;
            keySignature = null;
            timeSignature = null;

            leftSystemBracket = null;
            
            leftBarLine = null;
            rightBarLine = null;
        }

        public override void AddingObjectCompleted(NSObject nsObject)
        {
            base.AddingObjectCompleted(nsObject);
        }

        public override void ChildCommitCompleted(NSObject nsObject)
        {
            base.ChildCommitCompleted(nsObject);
            // add objects to voices here //
        }

        public NSBracket SetLeftSystemBracket(NSBracket.Options o)
        {
            if (leftSystemBracket == null) { leftSystemBracket = this.AddObject<NSBracket>( PoolEnum.NS_PARENT); }
            
            o.direction = HorizontalDirectionEnum.Right;
            if (Mathf.Approximately(0, o.height)) { o.height = topEdge - bottomEdge; }
            
            leftSystemBracket.options.CopyValuesFrom(o);
            leftSystemBracket.Commit();

            if (staveLines != null && staveLines.lines.Count > 0) { leftSystemBracket.PixelShiftX(-rectWidth / 2f, true); }
            return leftSystemBracket;
        }

        public NSBracket SetRightSystemBracket(NSBracket.Options o)
        {
            if (rightSystemBracket == null) { rightSystemBracket = this.AddObject<NSBracket>( PoolEnum.NS_PARENT); }

            o.direction = HorizontalDirectionEnum.Left;
            if (Mathf.Approximately(0, o.height)) { o.height = topEdge - bottomEdge; }

            rightSystemBracket.options.CopyValuesFrom(o);
            rightSystemBracket.Commit();

            rightSystemBracket.PixelShiftX(rectWidth / 2f, true);

            return rightSystemBracket;
        }

        public NSBarLine SetLeftBarLine(NSBarLine.Options o)
        {
            if (leftBarLine == null) { leftBarLine = this.AddObject<NSBarLine>(PoolEnum.NS_PARENT); }

            leftBarLine.options.CopyValuesFrom(o);
            leftBarLine.Commit();

            leftBarLine.PixelShiftX(-rectWidth / 2f, true);

            return leftBarLine;
        }

        public NSBarLine SetRightBarLine(NSBarLine.Options o)
        {
            if (rightBarLine == null) { rightBarLine = this.AddObject<NSBarLine>( PoolEnum.NS_PARENT); }

            rightBarLine.options.CopyValuesFrom(o);
            rightBarLine.Commit();

            if (staveLines != null && staveLines.lines.Count > 0) { rightBarLine.PixelShiftX(rectWidth / 2f, true); }

            return leftBarLine;
        }

        public NSClef SetClef(NSClef.Options o)
        {
            if (clef == null) { clef = this.AddObject<NSClef>(PoolEnum.NS_PARENT, PivotEnum.MiddleLeft); }

            clef.options.CopyValuesFrom(o);
            clef.Commit();

            return clef;
        }

        public NSMetronomeMark SetMetronomeMark(NSMetronomeMark.Options o)
        {
            if (metronomeMark == null) { metronomeMark = this.AddObject<NSMetronomeMark>(PoolEnum.NS_PARENT); }

            metronomeMark.options.CopyValuesFrom(o);
            metronomeMark.Commit();

            return metronomeMark;
        }

        public NSKeySignature SetKeySignature(NSKeySignature.Options o)
        {
            if (keySignature == null) { keySignature = this.AddObject<NSKeySignature>(PoolEnum.NS_PARENT); }

            keySignature.options.CopyValuesFrom(o);
            keySignature.Commit();

            return keySignature;
        }

        public NSTimeSignature SetTimeSignature(NSTimeSignature.Options o)
        {
            if (timeSignature == null) { timeSignature = this.AddObject<NSTimeSignature>(PoolEnum.NS_PARENT); }

            timeSignature.options.CopyValuesFrom(o);
            timeSignature.Commit();

            return timeSignature;
        }
    }
}
