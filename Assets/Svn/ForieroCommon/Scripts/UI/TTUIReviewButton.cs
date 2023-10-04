using ForieroEngine.Purchasing;
using UnityEngine.UI;

public class TTUIReviewButton : Button
{
    TTUIReviewButton()
    {
        onClick.AddListener(OnReviewClick);
    }
    
    void OnReviewClick()
    {
        ParentalLock.Show(4, (passed) =>
        {
            if (passed)
            {
#if (UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_WSA) && !UNITY_EDITOR
               NativeReviewRequest.RequestReview();
#else
               Store.OpenStore();
#endif
            }
        });
    }
}