using LightStream.Delegates;
using System.Diagnostics.CodeAnalysis;

namespace LightStream.Interfaces
{
    public interface ILightSubscription<T>
    {
        void OnEvent([AllowNull] OnEvent<T> handleEvent);
        void OnError([AllowNull] OnError handleError);
        void OnDone([AllowNull] OnDone handleDone);
        [NotNull]
        bool IsPaused { get; }
        void Resume();
        void Pause();
        void Cancel();
    }
}
