using LightStream.Delegates;
using LightStream.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace LightStream.Implementations
{
    class LightSubscription<T> : ILightSubscription<T>
    {
        private readonly ILightListener<T> _listener;

        public OnEvent<T> onEvent;

        public OnError onError;

        public OnDone onDone;

        public LightSubscription(ILightListener<T> listener, OnEvent<T> onEvent, OnError onError, OnDone onDone)
        {
            _listener = listener;
            this.onEvent = onEvent;
            this.onError = onError;
            this.onDone = onDone;
        }

        [NotNull]
        public bool IsPaused { get; private set; } = false;

        public void Cancel()
        {
            _listener.RemoveSubscription(this);
            onEvent = null;
            onError = null;
            onDone = null;
        }

        public void OnDone([AllowNull] OnDone handleDone)
        {
            onDone = handleDone;
        }

        public void OnError([AllowNull] OnError handleError)
        {
            onError = handleError;
        }

        public void OnEvent([AllowNull] OnEvent<T> handleEvent)
        {
            onEvent = handleEvent;
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void Resume()
        {
            IsPaused = false;
        }
    }
}
