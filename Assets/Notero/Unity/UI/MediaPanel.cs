using UnityEngine;
using UnityEngine.UI;

namespace Notero.Unity.UI
{
    public class MediaPanel : MonoBehaviour
    {
        [SerializeField]
        protected RectTransform m_MediaPanelRectTransform;

        [SerializeField]
        protected RectTransform m_MediaRendererRectTransform;

        [SerializeField]
        private RawImage m_MediaRendererImage;

        [SerializeField]
        private Mask m_MediaRendererMask;

        [SerializeField]
        private Texture m_VideoRendererTexture;

        [SerializeField]
        protected VideoPlayerPanel m_VideoPlayerPanel;

        public VideoPlayerPanel VideoPlayerPanel => m_VideoPlayerPanel;

        [Header("Settings")]
        [SerializeField]
        private RectTransform m_StarterPanel;

        [SerializeField]
        private RectTransform m_FullScreenPanel;

        protected MediaType m_MediaType;

        private void Awake()
        {
            m_VideoPlayerPanel.UpdateClipSizeHandler += UpdateVideoSize;
        }

        private void OnDestroy()
        {
            m_VideoPlayerPanel.UpdateClipSizeHandler -= UpdateVideoSize;
        }

        public void ShowImage(Texture texture)
        {
            m_MediaType = MediaType.IMAGE;

            m_VideoPlayerPanel.SetActive(false);
            SetRendererTexture(texture);
            UpdateImageSize(texture);
        }

        public void ShowVideo(string url)
        {
            m_MediaType = MediaType.VIDEO;

            m_VideoPlayerPanel.SetActive(true, url);
            SetRendererTexture(m_VideoRendererTexture);
            UpdateVideoSize(m_VideoPlayerPanel.GetVideoTexture());
        }

        public void SetVideoActive(bool isActive)
        {
            m_VideoPlayerPanel.SetActive(isActive);
        }

        public void SetVideoPlayerActive(bool isActive, string url = "")
        {
            m_MediaType = MediaType.VIDEO;

            m_VideoPlayerPanel.SetActive(isActive, url);
            SetRendererTexture(m_VideoRendererTexture);
            UpdateVideoSize(m_VideoPlayerPanel.GetVideoTexture());
        }

        public void SetFullScreenActive(bool isActive)
        {
            var parentRect = isActive ? m_FullScreenPanel : m_StarterPanel;

            m_MediaPanelRectTransform.SetParent(parentRect);
            m_MediaPanelRectTransform.SetAsFirstSibling();
            m_MediaPanelRectTransform.offsetMin = Vector2.zero;
            m_MediaPanelRectTransform.offsetMax = Vector2.zero;
            m_MediaRendererMask.enabled = !isActive;

            m_VideoPlayerPanel.SetToFullScreenPos(isActive);

            switch(m_MediaType)
            {
                case MediaType.IMAGE:
                    UpdateImageSize(m_MediaRendererImage.texture);
                    break;
                default:
                    UpdateVideoSize(m_VideoPlayerPanel.GetVideoTexture());
                    break;
            }
        }

        protected virtual void UpdateImageSize(Texture imageTexture)
        {
            var mediaPanelRect = m_MediaPanelRectTransform.rect;
            var width = mediaPanelRect.width;
            var height = mediaPanelRect.height;

            if(m_MediaRendererImage is { texture: not null })
            {
                (width, height) = MediaResizer.GetImageFitSize(mediaPanelRect, imageTexture);
            }

            m_MediaRendererRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            m_MediaRendererRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        protected virtual void UpdateVideoSize(Texture videoTexture)
        {
            var mediaPanelRect = m_MediaPanelRectTransform.rect;
            var width = mediaPanelRect.width;
            var height = mediaPanelRect.height;

            if(m_VideoPlayerPanel.IsVideoFinished())
            {
                (width, height) = MediaResizer.GetVideoFitSize(mediaPanelRect, videoTexture);
            }

            m_MediaRendererRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            m_MediaRendererRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        private void SetRendererTexture(Texture texture)
        {
            m_MediaRendererImage.texture = texture;
        }
    }
}
