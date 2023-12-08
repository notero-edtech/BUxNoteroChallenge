using UnityEngine;

namespace Notero.Unity.UI
{
    public static class MediaResizer
    {
        private const float m_RatioDiffError = 0.1f;

        public static (float w, float h) GetImageFitSize(Rect mediaPanel, Texture image)
        {
            if (IsSmallerThanPanel(mediaPanel, image.width, image.height))
            {
                return (image.width, image.height);
            }
            else if (IsSameRatio(mediaPanel, image.width, image.height))
            {
                return (mediaPanel.width, mediaPanel.height);
            }

            return GetNewSizeInMediaRatio(mediaPanel, image.width, image.height);
        }

        public static (float w, float h) GetVideoFitSize(Rect mediaPanel, Texture video)
        {
            if (IsSameRatio(mediaPanel, video.width, video.height))
            {
                return (mediaPanel.width, mediaPanel.height);
            }

            return GetNewSizeInMediaRatio(mediaPanel, video.width, video.height);
        }

        private static (float w, float h) GetNewSizeInMediaRatio(Rect mediaPanel, float mediaWidth, float mediaHeight)
        {
            float panelWidth = mediaPanel.width;
            float panelHeight = mediaPanel.height;
            float newHeight = mediaHeight * panelWidth / mediaWidth;
            float newWidth = mediaWidth * panelHeight / mediaHeight;

            if (newHeight <= panelHeight)
            {
                return (panelWidth, newHeight);
            }

            return (newWidth, panelHeight);
        }

        private static bool IsSmallerThanPanel(Rect mediaPanel, float mediaWidth, float mediaHeight)
        {
            return (mediaWidth < mediaPanel.width && mediaHeight < mediaPanel.height);
        }

        private static bool IsSameRatio(Rect mediaPanel, float mediaWidth, float mediaHeight)
        {
            float mediaPanelRatio = mediaPanel.width / mediaPanel.height;
            float imageRatio = mediaWidth / mediaHeight;
            float diff = Mathf.Abs(mediaPanelRatio - imageRatio);

            return diff < m_RatioDiffError;
        }
    }
}