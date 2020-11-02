using LightStream.Implementations;
using LightStream.Interfaces;

namespace LightStream
{
  public static class LightStreamFactory
    {
        public static ILightStream<T> Factory<T>()
        {
            return new LightStream<T>();
        }

        public static ILightStream<T> Factory<T>(T value)
        {
            return new LightStream<T>(value);
        }
    }
}
