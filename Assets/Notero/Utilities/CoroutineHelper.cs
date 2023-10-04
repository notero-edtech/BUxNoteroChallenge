using System.Collections;
using UnityEngine;


namespace Notero.Utilities
{
    public class CoroutineHelper : MonoSingleton<CoroutineHelper>
    {
        /// <summary>
        /// Play coroutine
        /// </summary>
        /// <param name="coroutine">IEnumerator function</param>
        /// <returns>Played Coroutine</returns>
        public Coroutine Play(IEnumerator coroutine)
        {
            return StartCoroutine(coroutine);
        }

        /// <summary>
        /// Stop coroutine
        /// </summary>
        /// <param name="coroutine">IEnumerator function</param>
        public void Stop(IEnumerator coroutine)
        {
            if (this != null && coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        /// <summary>
        /// Stop coroutine
        /// </summary>
        /// <param name="coroutine">Played coroutine</param>
        public void Stop(Coroutine coroutine)
        {
            if (this != null && coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        /// <summary>
        /// Stop played coroutine and play new coroutine
        /// Note: Use to stop previous coroutine before play new one
        /// </summary>
        /// <param name="stopCoroutine">Coroutine that want to stop(previous coroutine)</param>
        /// <param name="playCoroutine">Coroutine that want to play(new coroutine)</param>
        /// <returns></returns>
        public Coroutine Restart(Coroutine stopCoroutine, IEnumerator playCoroutine)
        {
            Stop(stopCoroutine);
            return Play(playCoroutine);
        }
    }
}