using LightStream.Interfaces;
using static LightStream.LightStreamFactory;

namespace LightStream.Extensions
{
    public static class LightStreamExtensions
    {
        public static ILightStream<T> LS<T>(this T source)
        {
            return Factory(source);
        }
    }
}
