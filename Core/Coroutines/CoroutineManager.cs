using UnityEngine;
using System.Collections;

namespace Hubris.Core.Coroutines
{
    /// <summary>
    /// The purpose of the <see cref="CoroutineManager"/> is to provide a more easily consumable api for dealing with coroutines.
    /// It is also to provide a way for non-monobehaviour scripts to use coroutines.
    /// </summary>
    public class CoroutineManager
    {
        static MonoBehaviour _instance;

        /// <summary> 
        /// Gets instance of CoroutineHandler, creates one if it doesn't exist.
        /// For use in scripts that aren't monobehaviour, but still need to run coroutines.
        /// </summary>
        static MonoBehaviour Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("CoroutineManager");

                    _instance = go.AddComponent<CoroutineDummy>();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Starts co-routine using the CoroutineManager GameObject. If it doesn't exist, it will be created automatically.
        /// <para> </para>
        /// Returns: A reference to the coroutine (to use with <see cref="StopCoroutine(Coroutine)"/>).
        /// </summary>
        /// <param name="coroutine">The coroutine to start.</param>
        /// <returns>A reference to the coroutine (to use with <see cref="StopCoroutine(Coroutine)"/>).</returns>
        public static Coroutine StartCoroutine(IEnumerator coroutine)
        {
            return Instance.StartCoroutine(coroutine);
        }

        /// <summary>
        /// Stops a co-routine using the CoroutineManager GameObject.
        /// </summary>
        /// <param name="coroutine">The coroutine to stop.</param>
        public static void StopCoroutine(Coroutine coroutine)
        {
            Instance.StopCoroutine(coroutine);
        }

        /// <summary>
        /// Stops a co-routine using the CoroutineManager GameObject.
        /// </summary>
        /// <param name="coroutine"></param>
        public static void StopCoroutine(IEnumerator coroutine)
        {
            Instance.StopCoroutine(coroutine);
        }

    }
}
