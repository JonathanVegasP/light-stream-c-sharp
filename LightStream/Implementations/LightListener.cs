using LightStream.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LightStream.Implementations
{
    class LightListener<T> : ILightListener<T>
    {
        private HashSet<LightSubscription<T>> _subscriptions = new HashSet<LightSubscription<T>>();

        public int Length => _subscriptions?.Count ?? 0;

        public bool HasListeners => Length > 0;

        public void AddSubscription(ILightSubscription<T> subscription)
        {
            _subscriptions.Add((LightSubscription<T>)subscription);
        }

        public Task Close()
        {
            return Task.Run(() =>
            {
                _subscriptions.Clear();
                _subscriptions = null;
            });
        }

        public Task NotifyDone()
        {
            return Task.Run(() =>
            {
                foreach (var subscription in _subscriptions)
                {
                    if (subscription.IsPaused)
                    {
                        continue;
                    }

                    subscription.onDone?.Invoke();
                }
            });
        }

        public void NotifyError(object error)
        {
            Task.Run(() =>
            {
                foreach (var subscription in _subscriptions)
                {
                    if (subscription.IsPaused)
                    {
                        continue;
                    }

                    subscription.onError?.Invoke(error);
                }
            });
        }

        public void NotifyEvent(T data)
        {
            Task.Run(() =>
            {
                foreach (var subscription in _subscriptions)
                {
                    if (subscription.IsPaused)
                    {
                        continue;
                    }

                    try
                    {
                        subscription.onEvent?.Invoke(data);
                    }
                    catch (Exception e)
                    {
                        subscription.onError?.Invoke(e);
                    }
                }
            });
        }

        public void RemoveSubscription(ILightSubscription<T> subscription)
        {
            _subscriptions.Remove((LightSubscription<T>)subscription);
        }
    }
}
