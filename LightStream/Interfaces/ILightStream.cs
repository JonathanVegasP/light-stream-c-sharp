using LightStream.Delegates;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace LightStream.Interfaces
{
    public interface ILightStream<T>
    {

        [AllowNull]
        T Value { get; }
        void Add([AllowNull] T data);
        void AddError([NotNull] object error);
        [NotNull]
        ILightSink<T> Sink { get; }
        ILightSubscription<T> Listen([AllowNull] OnEvent<T> onEvent, [AllowNull] OnError onError = null, [AllowNull] OnDone onDone = null);
        [NotNull]
        int Length { get; }
        [NotNull]
        bool HasListeners { get; }
        [NotNull]
        bool IsClosed { get; }
        Task Close();
    }
}
