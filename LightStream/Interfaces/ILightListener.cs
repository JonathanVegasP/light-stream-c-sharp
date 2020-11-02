using System.Threading.Tasks;

namespace LightStream.Interfaces
{
    interface ILightListener<T>
    {
        void AddSubscription(ILightSubscription<T> subscription);
        void RemoveSubscription(ILightSubscription<T> subscription);
        void NotifyEvent(T data);
        void NotifyError(object error);
        Task NotifyDone();
        int Length { get; }
        bool HasListeners { get; }
        Task Close();
    }
}
