using LightStream.Delegates;
using LightStream.Exceptions;
using LightStream.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace LightStream.Implementations
{
    class LightStream<T> : ILightStream<T>
    {
        private readonly LightSink<T> _sink;

        public LightStream()
        {
            _sink = new LightSink<T>();
        }

        public LightStream(T value)
        {
            _sink = new LightSink<T>(value);
        }

        private void IsAlreadyClosed()
        {
            if (IsClosed)
            {
                throw new LightStreamException("You cannot add events to a closed LightStream");
            }
        }

        [AllowNull]
        public T Value => _sink.Value;

        [NotNull]
        public ILightSink<T> Sink => _sink;

        [NotNull]
        public int Length => _sink.Length;

        [NotNull]
        public bool HasListeners => _sink.HasListeners;

        [NotNull]
        public bool IsClosed => _sink.IsClosed;

        public void Add([AllowNull] T data)
        {
            _sink.Add(data);
        }

        public void AddError([NotNull] object error)
        {
            _sink.AddError(error);
        }

        public Task Close()
        {
            return _sink.Close();
        }

        public ILightSubscription<T> Listen([AllowNull] OnEvent<T> onEvent, [AllowNull] OnError onError = null, [AllowNull] OnDone onDone = null)
        {
            IsAlreadyClosed();
            ILightSubscription<T> subscription = new LightSubscription<T>(_sink.listener, onEvent, onError, onDone);
            _sink.listener.AddSubscription(subscription);
            return subscription;
        }
    }
}
