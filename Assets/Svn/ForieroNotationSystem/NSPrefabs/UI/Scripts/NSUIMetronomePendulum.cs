/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem;
using UnityEngine;

public class NSUIMetronomePendulum : MonoBehaviour
{
   public RectTransform pendulumRT;
   private void Update() { pendulumRT.localEulerAngles = new Vector3(0, 0, NSPlayback.Metronome.PendulumAngle); }
}
