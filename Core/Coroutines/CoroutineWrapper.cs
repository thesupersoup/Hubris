using UnityEngine;
using System.Collections;
using System;

namespace Hubris.Core.Coroutines
{
    public class CoroutineWrapper
    {
        public bool IsRunning { get; private set; }

        public Action OnStartCallback;
        public Action OnFinishCallback;

        readonly IEnumerable _enumerable;

        Coroutine _runRoutine;
        Coroutine _wrappedRoutine;

        /// <summary>
        /// Create a coroutine wrapper from an <see cref="IEnumerable"/>.
        /// The reason this is not an <see cref="IEnumerator"/> is that the <see cref="IEnumerable"/> can produce new <see cref="IEnumerator"/>s.
        /// This can be used to control when the coroutine starts, or even restart it.
        /// </summary>
        /// <param name="coroutine">IEnumerable to run.</param>
        public CoroutineWrapper(IEnumerable coroutine)
        {
            this._enumerable = coroutine;
        }

        private IEnumerator Run()
        {
            //cache wrapped routine
            _wrappedRoutine = CoroutineManager.StartCoroutine(_enumerable.GetEnumerator());

            // yield the wrapped routine so that it can run its course without this method continueing
            yield return _wrappedRoutine;

            //running the wrapped routine should now be done
            IsRunning = false;

            OnFinishCallback?.Invoke();
        }

        /// <summary>
        /// Start the coroutine.
        /// Does nothing if already running.
        /// </summary>
        /// <returns>Reference to self.</returns>
        public CoroutineWrapper Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;

                OnStartCallback?.Invoke();

                _runRoutine = CoroutineManager.StartCoroutine(Run());
            }

            return this;
        }


        /// <summary>
        /// Stops the coroutine.
        /// Does nothing if not running.
        /// </summary>
        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;

                // Stop the run routine that is wrapping the wrapped routing, so that stopping that won't trigger OnFinishCallback
                CoroutineManager.StopCoroutine(_runRoutine);

                // safely stop the wrapped coroutine
                CoroutineManager.StopCoroutine(_wrappedRoutine);

                OnFinishCallback?.Invoke();
            }
        }

        /// <summary>
        /// Stops coroutine and restarts it.
        /// If the coroutine is not running it will be started.
        /// </summary>
        public void Restart()
        {
            Stop();
            Start();
        }
    }

    public static class CoroutineWrapperExt
    {
        /// <summary>
        /// Create a CoroutineWrapper.
        /// </summary>
        /// <param name="inRoutine">CoRoutine to create wrapper from.</param>
        /// <returns></returns>
        public static CoroutineWrapper CreateWrapper(this IEnumerable inRoutine)
        {
            return new CoroutineWrapper(inRoutine);
        }
    }
}
