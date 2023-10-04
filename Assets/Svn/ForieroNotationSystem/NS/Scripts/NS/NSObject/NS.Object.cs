/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections.Generic;
using PathologicalGames;
using UnityEngine;
using UnityEngine.EventSystems;
using ForieroEngine.Music.NotationSystem.Extensions;
using Object = UnityEngine.Object;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    /// <summary>
    /// NSObject is base object for all pooled ojects
    /// </summary>
    public partial class NSObject
    {
        public NS ns;

        public PoolEnum pool;

        public NSPart part;
        public NSStave stave;

        public NSObject parent;

        public string name = "";
        public string tag = "";

        public int voiceNumber = 0;

        public bool draggable = false;
        public bool selectable = false;
        public bool passable = false;
        public bool hidden = false;

        public Color color = Color.black;

        NSSelectableObject selectableObject;

        public float pxDistanceX = 0f;
        public float pxDistanceY = 0f;

        public RectTransform rectTransform = null;
        public CanvasRenderer canvasRenderer = null;
        public PivotEnum pivot = PivotEnum.MiddleCenter;
        
        public bool followParentRectWidth = false;
        public bool followParentRectHeight = false;

        public readonly Margins margins = new();
        public readonly Padding padding = new();

        public Action onCreated;
        public Action onDestroyed;
        
        public void UpdateRectWidthAndHeight(bool children = true)
        {
            if (parent)
            {
                if (followParentRectWidth) rectWidth = parent.rectWidth - margins.left - margins.right;
                if (followParentRectHeight) rectHeight = parent.rectHeight - margins.bottom - margins.top;
            }

            if (!children) return;
            
            for (var i = 0; i < allObjects.Count; i++) { allObjects[i].UpdateRectWidthAndHeight(children); }
        }

        #region null operator override

        public static implicit operator bool(NSObject x) => x is object objX && x.rectTransform != null;
        public static bool operator ==(NSObject x, NSObject y)
        {
            var @objX = x as object;
            var @objY = y as object;

            if (@objX != null && @objY == null) return x.rectTransform == null;
            if (@objX == null && @objY != null) return y.rectTransform == null;
            if (@objX == null && @objY == null) return true;
            return ReferenceEquals(x, y);
        }
        
        public static bool operator !=(NSObject x, NSObject y)
        {
            var @objX = x as object;
            var @objY = y as object;

            if (@objX != null && @objY == null) return x.rectTransform != null;
            if (@objX == null && @objY != null) return y.rectTransform != null;
            if (@objX == null && @objY == null) return false;
            return !ReferenceEquals(x, y);
        }

        // public override bool Equals(System.Object obj) => this == (NSObject)obj;
        //
        // public override int GetHashCode()
        // {
        //     // Returning the hashcode of the Guid used for the reference id will be 
        //     // sufficient and would only cause a problem if RecommendationDTO objects
        //     // were stored in a non-generic hash set along side other guid instances
        //     // which is very unlikely!
        //     return false;
        // }
        
        #endregion

        public Canvas canvas
        {
            get
            {
                return pool switch
                {
                    PoolEnum.NS_FIXED => ns.fixedCanvas,
                    PoolEnum.NS_MOVABLE => ns.movableCanvas,
                    PoolEnum.NS_FIXED_OVERLAY => ns.fixedOverlayCanvas,
                    PoolEnum.NS_MOVABLE_OVERLAY => ns.movableOverlayCanvas,
                    _ => null
                };
            }
        }

        public RectTransform poolRectTransform => rectTransform.parent as RectTransform;
        public float screenHeight => poolRectTransform.GetHeight();
        public float screenWidth => poolRectTransform.GetWidth();
        public float rectWidth
        {
            get => rectTransform.GetWidth();
            set => rectTransform.SetWidth(value);
        }

        public float rectHeight
        {
            get => rectTransform.GetHeight();
            set => rectTransform.SetHeight(value);
        }

        public bool passed = false;        
        public float time = 0;
        public float pixelTime = 0f;
        public float pixelTimeZoomed =>
            NSSettingsStatic.canvasRenderMode switch
            {
                CanvasRenderMode.Screen => pixelTime * NSPlayback.Zoom,
                CanvasRenderMode.World => pixelTime,
                _ => pixelTime
            };

        public List<NSObject> allObjects = new List<NSObject>();

        public List<NSObject> objects = new List<NSObject>();
        public List<NSObjectImage> images = new List<NSObjectImage>();
        public List<NSObjectRawImage> rawImages = new List<NSObjectRawImage>();
        public List<NSObjectText> texts = new List<NSObjectText>();
        public List<NSObjectVector> vectors = new List<NSObjectVector>();
        public List<NSObjectSMuFL> smufls = new List<NSObjectSMuFL>();
        public List<NSObjectPrefab> prefabs = new List<NSObjectPrefab>();

        public static int statisticTotalObjectCount = 0;
        public static int statisticObjectsCount = 0;
        public static int statisticImagesCount = 0;
        public static int statisticRawImagesCount = 0;
        public static int statisticTextsCount = 0;
        public static int statisticVectorsCount = 0;
        public static int statisticSMuFLsCount = 0;
        public static int statisticPrefabsCount = 0;

        public static Action OnStatisticsChanged;

        public virtual void Reset() { }

        float _scale = 1f;
        public float scale { get { return _scale; } }
        /// <summary>
        /// Scale should be called after Commit
        /// </summary>
        public void SetScale(float scale, bool children = true)
        {
            _scale = scale;
            this.rectTransform.localScale = Vector3.one * scale;
            if (children)
            {
                for (int i = 0; i < allObjects.Count; i++)
                {
                    allObjects[i].SetScale(scale, children);
                }
            }
        }

        public void SetVisible(bool visible, bool children = true)
        {
            if (canvasRenderer) canvasRenderer.SetAlpha(visible ? 1f : 0f);
            if (children)
            {
                for (int i = 0; i < allObjects.Count; i++)
                {
                    allObjects[i].SetVisible(visible, children);
                }
            }
        }

        public void SetCull(bool cull, bool children = true)
        {
            if (canvasRenderer) canvasRenderer.cull = cull;
            if (!children) return;
            for (int i = 0; i < allObjects.Count; i++) { allObjects[i].SetCull(cull, children); }
        }

        public void SetAlpha(float alpha, bool children = true)
        {
            if (this.hidden) return; 
            if (canvasRenderer) canvasRenderer.SetAlpha(alpha);
            if (!children) return;
            for (var i = 0; i < allObjects.Count; i++) { allObjects[i].SetAlpha(alpha, children); }
        }

        public virtual void Commit()
        {
            if (selectable)
            {
                if (!selectableObject)
                {
                    selectableObject = rectTransform.gameObject.AddComponent<NSSelectableObject>();
                    selectableObject.nsObject = this;
                }

                if (this is IBeginDragHandler) { selectableObject.iBeginDragHandler = this as IBeginDragHandler; }
                if (this is IDragHandler) { selectableObject.iDragHandler = this as IDragHandler; }
                if (this is IEndDragHandler) { selectableObject.iEndDragHandler = this as IEndDragHandler; }
                if (this is IPointerUpHandler) { selectableObject.iPointerUpHandler = this as IPointerUpHandler; }
                if (this is IPointerDownHandler) { selectableObject.iPointerDownHandler = this as IPointerDownHandler; }
                if (this is IPointerClickHandler) { selectableObject.iPointerClickHandler = this as IPointerClickHandler; }
                if (this is IPointerEnterHandler) { selectableObject.iPointerEnterHandler = this as IPointerEnterHandler; }
                if (this is IPointerExitHandler) { selectableObject.iPointerExitHandler = this as IPointerExitHandler; }
            }
            else
            {
                if (selectableObject)
                {
                    Object.Destroy(selectableObject);
                    selectableObject = null;
                }
            }

            if (this.parent != null) { this.parent.ChildCommitCompleted(this); }
        }

        public void Colorate(Color color, bool children = true)
        {
            if (this.hidden) return;
            this.color = color;
            (this as INSColorable)?.SetColor(color);
            if (!children) return;
            for (var i = 0; i < allObjects.Count; i++) { allObjects[i].Colorate(color); }
        }
        
        public void ChildIsBeingDestroyed(NSObject o)
        {
            if (this is NSObjectImage)
            {
                images.Remove(o as NSObjectImage);
                allObjects.Remove(o);
            }
            else if (this is NSObjectRawImage)
            {
                rawImages.Remove(o as NSObjectRawImage);
                allObjects.Remove(o);
            }
            else if (this is NSObjectText)
            {
                texts.Remove(o as NSObjectText);
                allObjects.Remove(o);
            }
            else if (this is NSObjectVector)
            {
                vectors.Remove(o as NSObjectVector);
                allObjects.Remove(o);
            }
            else if (this is NSObjectSMuFL)
            {
                smufls.Remove(o as NSObjectSMuFL);
                allObjects.Remove(o);
            }
            else if (this is NSObjectPrefab)
            {
                prefabs.Remove(o as NSObjectPrefab);
                allObjects.Remove(o);
            }
            else if (this is NSObject)
            {
                objects.Remove(o);
                allObjects.Remove(o);
            }
        }

        public virtual void Destroy(bool silent = false)
        {
            if (!rectTransform)
            {
                if(!silent) Debug.LogError("Trying to destroy already destroyed NSObject");
                return;
            }

            if (this is NS)
            {
                Debug.LogError("Trying to destroy NS object. This object is handled by itself.");
                return;
            }

            statisticTotalObjectCount--;
            var typeName = "";
            if (this is NSObjectImage) { statisticImagesCount--; typeName = nameof(NSObjectImage); }
            else if (this is NSObjectRawImage) { statisticRawImagesCount--; typeName = nameof(NSObjectRawImage); }
            else if (this is NSObjectText) { statisticTextsCount--; typeName = nameof(NSObjectText); }
            else if (this is NSObjectVector) { statisticVectorsCount--; typeName = nameof(NSObjectVector); }
            else if (this is NSObjectSMuFL) { statisticSMuFLsCount--; typeName = nameof(NSObjectSMuFL); }
            else if (this is NSObjectPrefab) { statisticPrefabsCount--; typeName = nameof(NSObjectPrefab); }
            else if (this is NSObject) { statisticObjectsCount--; typeName = nameof(NSObject); }

            OnStatisticsChanged?.Invoke();

            this.rectTransform.name = typeName;
            PoolManager.Pools[pool.ToString()].Despawn(rectTransform as Transform);

            if (parent != null) { parent.ChildIsBeingDestroyed(this); }

            ns = null;
            part = null;
            stave = null;
            parent = null;
            hidden = false;
            midiData.Reset();
            margins.Reset();
            padding.Reset();

            followParentRectHeight = false;
            followParentRectWidth = false;

            if (selectableObject != null) { Object.Destroy(selectableObject); selectableObject = null; }

            rectTransform = null;

            allObjects.Clear();

            objects.Clear();
            images.Clear();
            rawImages.Clear();
            texts.Clear();
            vectors.Clear();
            smufls.Clear();
            prefabs.Clear();
            
            onDestroyed?.Invoke();
        }

        public virtual void DestroyChildren(bool firstPass = true)
        {
            var destroy = !firstPass;
            if (firstPass) firstPass = false;

            var count = 0; var i = 0;
            count = allObjects.Count;
            for (i = count - 1; i >= 0; i--) allObjects[i].DestroyChildren(firstPass);

            allObjects.Clear();

            objects.Clear();
            images.Clear();
            rawImages.Clear();
            texts.Clear();
            vectors.Clear();
            smufls.Clear();
            prefabs.Clear();

            if (destroy) this.Destroy();
        }

        public List<T> GetObjectsOfTypeWithTag<T>(string tag, bool children, List<T> foundObjects = null) where T : NSObject
        {
            foundObjects = GetObjectsOfType<T>(children);
            for (int i = foundObjects.Count - 1; i >= 0; i--) { if (foundObjects[i].tag != tag) foundObjects.RemoveAt(i); }
            return foundObjects;
        }

        public List<T> GetObjectsOfTypeWithName<T>(string name, bool children, List<T> foundObjects = null) where T : NSObject
        {
            foundObjects = GetObjectsOfType<T>(children);
            for (int i = foundObjects.Count - 1; i >= 0; i--) { if (foundObjects[i].name != name) foundObjects.RemoveAt(i); }
            return foundObjects;
        }
        
        public List<T> GetObjectsOfType<T>(bool children, List<T> foundObjects = null) where T : NSObject
        {
            foundObjects ??= new List<T>();
            for ( var i = 0; i < allObjects.Count; i++)
            {
                if (allObjects[i] is T) foundObjects.Add(allObjects[i] as T);
                if (children) allObjects[i].GetObjectsOfType(children, foundObjects);
            }
            return foundObjects;
        }

        public virtual void AddingObjectCompleted(NSObject nsObject) { return; }
        public virtual void ChildCommitCompleted(NSObject nsObject) { return; }

        public T AddObject<T>(PoolEnum pool = PoolEnum.NS_MOVABLE, PivotEnum pivot = PivotEnum.MiddleCenter, string name = "", string tag = "", GameObject prefab = null) where T : NSObject, new()
        {
            if (this.ns != null && NSSettingsStatic.addingObjectConstraints != LogEnum.None)
            {
                var check = ns.CheckAddObjectConstraints<T>(this);
                if (check == NSObjectCheckEnum.Constrained)
                {
                    var log = "Adding '" + typeof(T).Name + "' to '" + this.GetType().Name + "' is constrained.";
                    switch (NSSettingsStatic.addingObjectConstraints)
                    {
                        case LogEnum.None: break;
                        case LogEnum.Log: Debug.Log(log); break;
                        case LogEnum.LogWarning: Debug.LogWarning(log); break;
                        case LogEnum.LogError: Debug.LogError(log); break;
                    }
                }
            }

            var t = new T()
            {
                parent = this,
                voiceNumber = this.voiceNumber,
                ns = this.ns
            };

            if (t is NSPart nsPart) { nsPart.part = nsPart; }
            else { t.part = this.part; }

            if (t is NSStave nsStave) { nsStave.stave = nsStave; }
            else { t.stave = this.stave; }

            t.pxDistanceX = 0f;
            t.pxDistanceY = 0f;

            statisticTotalObjectCount++;

            string prefabName = null;
            if (t is NSObjectImage objectImage)
            {
                this.images.Add(objectImage);
                this.allObjects.Add(objectImage);
                prefabName = nameof(NSObjectImage);
                statisticImagesCount++;
            }
            else if (t is NSObjectRawImage image)
            {
                this.rawImages.Add(image);
                this.allObjects.Add(image);
                prefabName = nameof(NSObjectRawImage);
                statisticRawImagesCount++;
            }
            else if (t is NSObjectText text)
            {
                this.texts.Add(text);
                this.allObjects.Add(text);
                prefabName = nameof(NSObjectText);
                if (NSSettingsStatic.textMode == TextMode.TextMeshPRO) { prefabName += "TMP"; }
                statisticTextsCount++;
            }
            else if (t is NSObjectVector vector)
            {
                this.vectors.Add(vector);
                this.allObjects.Add(vector);
                prefabName = nameof(NSObjectVector);
                statisticVectorsCount++;
            }
            else if (t is NSObjectSMuFL fl)
            {
                this.smufls.Add(fl);
                this.allObjects.Add(fl);
                prefabName = nameof(NSObjectSMuFL);
                if (NSSettingsStatic.textMode == TextMode.TextMeshPRO)
                {
                    prefabName += "TMP";
                }
                statisticSMuFLsCount++;
            }
            else if (t is NSObjectPrefab objectPrefab)
            {
                this.prefabs.Add(objectPrefab);
                this.allObjects.Add(objectPrefab);
                prefabName = nameof(NSObjectPrefab);
                statisticPrefabsCount++;
            }
            else if (t is NSObject nsObject)
            {
                this.objects.Add(nsObject);
                this.allObjects.Add(nsObject);
                prefabName = nameof(NSObject);
                statisticObjectsCount++;
            } 
            else { Debug.LogError("Unrecognized type : " + typeof(T).Name); }

            OnStatisticsChanged?.Invoke();

            if (!string.IsNullOrEmpty(prefabName))
            {
                if (pool == PoolEnum.NS_PARENT)
                {
                    if (parent == null) { t.pool = PoolEnum.NS_MOVABLE; }
                    else { t.pool = parent.pool; }
                }
                else { t.pool = pool; }

                if (prefab) { t.rectTransform = PoolManager.Pools[t.pool.ToString()].Spawn(prefab) as RectTransform; }
                else { t.rectTransform = PoolManager.Pools[t.pool.ToString()].Spawn(prefabName) as RectTransform; }

                t.canvasRenderer = t.rectTransform.GetComponent<CanvasRenderer>();
                t.pivot = pivot;
                t.rectTransform.SetPivot(pivot.ToPivotAndAnchors());
                t.rectTransform.anchorMax = Vector2.one * 0.5f;
                t.rectTransform.anchorMin = Vector2.one * 0.5f;

                t.rectTransform.anchoredPosition = Vector2.zero;
                t.rectTransform.localScale = Vector3.one;
                t.rectTransform.rotation = Quaternion.identity;
                t.rectTransform.offsetMax = Vector2.zero;
                t.rectTransform.offsetMin = Vector2.zero;

                t.AlignToParent(false, false);

                t.name = name;
                t.tag = tag;

                t.rectTransform.gameObject.name =
                     (string.IsNullOrEmpty(t.name) ? "" : t.name + " - ") +
                     (string.IsNullOrEmpty(t.tag) ? "" : "(" + t.tag + ") - ") +
                     typeof(T).Name;

                if (t is INSColorable)
                {
                    if ((t as INSColorable).GetColor() != NSSettingsStatic.normalColor)
                    {
                        (t as INSColorable).SetColor(NSSettingsStatic.normalColor);
                    }
                }

                t.snap = new Snap
                {
                    verticalDragEnum = DragEnum.Free,
                    verticalDirection = VerticalDirectionEnum.Up,
                    verticalStep = ShiftStepEnum.Half,
                    horizontalDragEnum = DragEnum.Free,
                    horizontalDirection = HorizontalDirectionEnum.Right,
                    horizontalStep = ShiftStepEnum.Half
                };

                // reset base objects since in pool can be messed up, much efficient way would be to reset them when returning to pool //

                if (t is NSObjectImage) { }
                else if (t is NSObjectRawImage) { }
                else if (t is NSObjectText) { }
                else if (t is NSObjectVector) { }
                else if (t is NSObjectSMuFL) { }
                else if (t is NSObjectPrefab) { }
                else if (t is NSObject) { }
                else { }
            }
            else
            {
                Debug.LogError("NSObject : Null or empty prefab name!");
            }

            t.Reset();
            this.AddingObjectCompleted(t);
            this.onCreated?.Invoke();
            return t;
        }

        #region TouchEvents

        public enum DragEnum
        {
            None,
            Free,
            Snap
        }

        public struct Snap
        {
            public DragEnum verticalDragEnum;
            public VerticalDirectionEnum verticalDirection;
            public ShiftStepEnum verticalStep;

            public DragEnum horizontalDragEnum;
            public HorizontalDirectionEnum horizontalDirection;
            public ShiftStepEnum horizontalStep;
        }

        public Snap snap = new Snap
        {
            verticalDragEnum = DragEnum.Free,
            verticalDirection = VerticalDirectionEnum.Undefined,
            verticalStep = ShiftStepEnum.Half,
            horizontalDragEnum = DragEnum.Free,
            horizontalDirection = HorizontalDirectionEnum.Undefined,
            horizontalStep = ShiftStepEnum.Half
        };

        public bool dragging = false;
        public Vector2 touchBeginDragPosition = Vector2.zero;
        public Vector2 touchNewDragPosition = Vector2.zero;
        public Vector2 touchDiffDragPosition = Vector2.zero;

        public Vector2 touchObjectBeginDragPosition = Vector2.zero;

        public Vector2 TouchPosition()
        {
            if (Input.GetMouseButton(0))
            {
                return Input.mousePosition;
            }
            else if (Input.touchCount > 0)
            {
                return Input.touches[0].position;
            }
            else
            {
                return Vector2.zero;
            }
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            dragging = true;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if ((Input.touchCount == 1 || Input.GetMouseButton(0)))
            {
                touchNewDragPosition = TouchPosition();
                touchDiffDragPosition = (touchBeginDragPosition - touchNewDragPosition) / ns.backgroundCanvas.scaleFactor * (1f / NSPlayback.Zoom);

                switch (this.snap.verticalDragEnum)
                {
                    case DragEnum.Free:
                        this.PixelShiftY(-touchDiffDragPosition.y, true);
                        touchBeginDragPosition = new Vector2(touchBeginDragPosition.x, touchNewDragPosition.y);
                        break;
                    case DragEnum.Snap:
                        switch (this.snap.verticalDirection)
                        {
                            case VerticalDirectionEnum.Up:
                                this.SetPosition(touchObjectBeginDragPosition, true, true);
                                this.SnapShift(-touchDiffDragPosition.y, DirectionEnum.Up, true, ShiftStepEnum.Half);
                                break;
                            case VerticalDirectionEnum.Down:
                                this.SetPosition(touchObjectBeginDragPosition, true, true);
                                this.SnapShift(touchDiffDragPosition.y, DirectionEnum.Down, true, ShiftStepEnum.Half);
                                break;
                        }
                        break;
                }

                switch (this.snap.horizontalDragEnum)
                {
                    case DragEnum.Free:
                        this.PixelShiftX(-touchDiffDragPosition.x, true);
                        touchBeginDragPosition = new Vector2(touchNewDragPosition.x, touchBeginDragPosition.y);
                        break;
                    case DragEnum.Snap:
                        switch (this.snap.horizontalDirection)
                        {
                            case HorizontalDirectionEnum.Left:
                                this.SetPosition(touchObjectBeginDragPosition, true, true);
                                this.SnapShift(-touchDiffDragPosition.x, DirectionEnum.Left, true, ShiftStepEnum.Half);
                                break;
                            case HorizontalDirectionEnum.Right:
                                this.SetPosition(touchObjectBeginDragPosition, true, true);
                                this.SnapShift(touchDiffDragPosition.x, DirectionEnum.Right, true, ShiftStepEnum.Half);
                                break;
                            default:
                                this.PixelShiftX(touchDiffDragPosition.x, true);
                                break;
                        }
                        break;
                }
            }
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            dragging = false;
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            this.Colorate(NSSettingsStatic.normalColor);
            this.ns.selectedObject = null;

            touchBeginDragPosition = Vector2.zero;
            touchNewDragPosition = Vector2.zero;
            touchDiffDragPosition = Vector2.zero;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            this.Colorate(NSSettingsStatic.selectedColor);
            this.ns.selectedObject = this;

            touchBeginDragPosition = touchNewDragPosition = TouchPosition();
            touchObjectBeginDragPosition = this.GetPosition(true);
            touchDiffDragPosition = Vector2.zero;
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {

        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (this.ns.selectedObject == null) { this.Colorate(NSSettingsStatic.hoverColor); }
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (this.ns.selectedObject == null) { this.Colorate(NSSettingsStatic.normalColor); }
        }

        #endregion

    }
}
