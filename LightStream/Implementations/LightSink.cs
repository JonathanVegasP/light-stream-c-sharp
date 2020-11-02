using LightStream.Exceptions;
using LightStream.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace LightStream.Implementations
{
    class LightSink<T> : ILightSink<T>
    {
        public ILightListener<T> listener = new LightListener<T>();

        public LightSink() { }

        public LightSink(T value)
        {
            Value = value;
        }

        public T Value { get; private set; }

        [NotNull]
        public int Length => listener?.Length ?? 0;

        [NotNull]
        public bool HasListeners => listener?.HasListeners == true;

        [NotNull]
        public bool IsClosed => listener == null;

        private void IsAlreadyClosed(bool closed = true)
        {
            if (IsClosed)
            {
                throw new LightStreamException($"You cannot {(closed ? "close" : "add events to")} a closed LightStream");
            }
        }

        public void Add([AllowNull] T data)
        {
            IsAlreadyClosed();
            Value = data;
            listener.NotifyEvent(data);
        }

        public void AddError([NotNull] object error)
        {
            IsAlreadyClosed();
            listener.NotifyError(error);
        }

        public async Task Close()
        {
            IsAlreadyClosed(closed: true);
            await listener.NotifyDone();
            await listener.Close();
            listener = null;
            Value = default;
        }
    }

}
