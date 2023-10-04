using DG.Tweening;
using ForieroEngine.Extensions;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class VoiceOverBackgroundDownloader : MonoBehaviour
{

	private bool _isIn = false;

	public static VoiceOverBackgroundDownloader singleton;
	public bool showProgress = true;
	public bool checkFiles = true;

	[FormerlySerializedAs("downloadPannel")] public RectTransform downloadPanel;
	public RectTransform downloadBackground;
	public Text downloadText;
	public RectTransform downloadSlider;
	public RectTransform downloadSliderBackground;

	public VoiceOverDownloader.DownloadLanguageClass downloadLanguage = null;

	private void OnDestroy ()
	{
		//singleton = null;
		_tween?.Kill();
	}

	private void Awake ()
	{
		if (singleton) { Destroy (this.gameObject); return; }
		singleton = this;
		DontDestroyOnLoad (this.gameObject);
	}

	private void Start ()
	{
		if (this != singleton) { return; }
		Lang.OnLanguageChange += () => { DownloadLanguage (); };
		if (checkFiles) { DownloadLanguage (); }
		downloadPanel.gameObject.SetActive (false);
	}

	private bool _canDownloadLanguage = false;

	public void DownloadLanguage ()
	{
		_canDownloadLanguage = true;
		DoDownloadLanguage ();
	}

	private void DoDownloadLanguage ()
	{
		if (!_canDownloadLanguage) {
			if (Lang.debug) {
				Debug.LogWarning ("Voice Over background downloader blocked since 'DownloadLanguage' was not called!");
			}
			return;
		}

		//if (downloadLanguage != null) {
		//	downloadLanguage.Cancel ();
		//}

		downloadLanguage = new VoiceOverDownloader.DownloadLanguageClass (Lang.selectedLanguage);
		DoIn ();
		downloadLanguage.Download (
			(p)=> _percentage = p,
			(e)=> { Debug.Log(e); },
			() => {
				downloadLanguage = null;
				DoOut ();
			});
	}

	private Tween _tween;

	public void DoIn ()
	{
		if (!showProgress) return;
		downloadPanel.gameObject.SetActive (true);
		_isIn = true;
		_tween?.Kill();
		_tween = downloadPanel.DOAnchorPos (new Vector2 (0, 5), 0.3f).SetEase (Ease.InOutSine);
	}

	public void DoOut ()
	{
		if (!showProgress) return;
		_isIn = false;
		_tween?.Kill();		
		_tween = downloadPanel.DOAnchorPos (new Vector2 (0, -55), 0.3f).SetEase (Ease.InOutSine).OnComplete (() => {
			downloadPanel.gameObject.SetActive (false);
		});
	}

	private float _percentage = 0f;
	// Update is called once per frame
	private void Update ()
	{
		if (!showProgress) return;
			
		if(downloadSlider) downloadSlider.SetSize (new Vector2 (downloadSliderBackground.GetSize ().x * _percentage, downloadSlider.GetSize ().y));
		if(downloadText) downloadText.text = $"{(int)(_percentage * 100)}%";		
	}
}
