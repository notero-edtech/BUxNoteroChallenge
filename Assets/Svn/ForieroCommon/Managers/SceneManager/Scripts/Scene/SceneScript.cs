using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Scene = ForieroEngine.Scene;

public class SceneScript : MonoBehaviour
{
    private static SceneScript singleton;

    public static bool fade = false;
    public static Color fadeColor = Color.black;
    public static float loadingThreshold = 0.8f;
    public static Scene.SceneEnum sceneEnum = Scene.SceneEnum.Undefined;

    private static Texture2D fadeTexture;
    
    private Rect _rect = Rect.zero;
    private Canvas _canvas;
    private Image _image;
    private RectTransform _imageRT;

    private CanvasGroup _canvasGroupLogo;
    private CanvasGroup _canvasGroupLoading;
    private CanvasGroup _canvasGroupSceneLoading;
    
    private void OnDestroy () { singleton = null; }
    private void Awake ()
    {
        if (singleton) { DestroyImmediate (this.gameObject); return; }
        singleton = this;
        DontDestroyOnLoad (this.gameObject);
        fadeTexture = new Texture2D (1, 1);
        fadeTexture.SetPixel (0, 0, Color.white);
        fadeTexture.Apply ();
        
        _canvas = this.gameObject.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 10000;
        
        _image = new GameObject("Image").AddComponent<Image>();
        _image.transform.parent = _canvas.transform;
        _image.gameObject.AddComponent<RectTransform>();
        _image.gameObject.AddComponent<TTUIIgnoreRaycast>();
        
        if (SceneSettings.instance.logoUIPrefab)
        {
            _canvasGroupLogo = Instantiate(SceneSettings.instance.logoUIPrefab, this.transform).GetOrAddComponent<CanvasGroup>();
            _canvasGroupLogo.alpha = 0;
            _canvasGroupLogo.gameObject.SetActive(false);
        }

        if (SceneSettings.instance.loadingUIPrefab)
        {
            _canvasGroupLoading = Instantiate(SceneSettings.instance.loadingUIPrefab, this.transform).GetOrAddComponent<CanvasGroup>();
            _canvasGroupLoading.alpha = 0;
            _canvasGroupLoading.gameObject.SetActive(false);
        }

        if (SceneSettings.instance.sceneLoadingUIPrefab)
        {
            _canvasGroupSceneLoading = Instantiate(SceneSettings.instance.sceneLoadingUIPrefab, this.transform).GetOrAddComponent<CanvasGroup>();
            _canvasGroupSceneLoading.alpha = 0;
            _canvasGroupSceneLoading.gameObject.SetActive(false);
        }
        
        _image.gameObject.SetActive(false);
    }

    private void Start()
    {
        _imageRT = _image.transform as RectTransform;
        _imageRT.anchoredPosition = Vector2.zero;
        _imageRT.anchorMin = Vector2.zero;
        _imageRT.anchorMax = Vector2.one;
        _imageRT.pivot = Vector2.one * 0.5f;
    }

    private void Update ()
    {
        if (fade)
        {
            if(!_image.gameObject.activeSelf) _image.gameObject.SetActive(true);
            switch (sceneEnum)
            {
                case Scene.SceneEnum.Loading:
                    if(!_canvasGroupLoading) break;
                    if(!_canvasGroupLoading.gameObject.activeSelf) _canvasGroupLoading.gameObject.SetActive(true);
                    _canvasGroupLoading.alpha = fadeColor.a;
                    break;
                case Scene.SceneEnum.Logo:
                    if(!_canvasGroupLogo) break;
                    if(!_canvasGroupLogo.gameObject.activeSelf) _canvasGroupLogo.gameObject.SetActive(true);
                    _canvasGroupLogo.alpha = fadeColor.a;
                    break;
                case Scene.SceneEnum.SceneLoading:
                    if(!_canvasGroupSceneLoading) break;
                    if(!_canvasGroupSceneLoading.gameObject.activeSelf) _canvasGroupSceneLoading.gameObject.SetActive(true);
                    _canvasGroupSceneLoading.alpha = fadeColor.a;
                    break;
            }
            _image.color = fadeColor;
        }
        else
        {
            switch (sceneEnum)
            {
                case Scene.SceneEnum.Loading:
                    if(!_canvasGroupLoading) break;
                    if(_canvasGroupLoading.gameObject.activeSelf) _canvasGroupLoading.gameObject.SetActive(false);
                    _canvasGroupLoading.alpha = 0;
                    break;
                case Scene.SceneEnum.Logo:
                    if(!_canvasGroupLogo) break;
                    if(_canvasGroupLogo.gameObject.activeSelf) _canvasGroupLogo.gameObject.SetActive(false);
                    _canvasGroupLogo.alpha = 0;
                    break;
                case Scene.SceneEnum.SceneLoading:
                    if(!_canvasGroupSceneLoading) break;
                    if(_canvasGroupSceneLoading.gameObject.activeSelf) _canvasGroupSceneLoading.gameObject.SetActive(false);
                    _canvasGroupSceneLoading.alpha = 0;
                    break;
            }
            if(_image.gameObject.activeSelf) _image.gameObject.SetActive(false); 
        }
    }
}
