using Notero.RaindropGameplay.Core.Scoring;
using UnityEngine;

namespace Notero.RaindropGameplay.UI
{
    public interface IFeedbackDisplayable
    {
        public void SetPosition(Vector2 position);
        public void SetScale(Vector3 scale);
        public void SetRotation(Quaternion rotation);
        public void SetActive(NoteTimingScore score);
    }
}
