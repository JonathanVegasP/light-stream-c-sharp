using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace LightStream.Interfaces
{
    public interface ILightSink<T>
    {
        void Add([AllowNull] T data);
        void AddError([NotNull] object error);
        [NotNull]
        int Length { get; }
        [NotNull]
        bool HasListeners { get; }
        [NotNull]
        bool IsClosed { get; }
        Task Close();
    }
}
