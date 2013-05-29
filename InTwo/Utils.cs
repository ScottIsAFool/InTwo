using System;
using GalaSoft.MvvmLight.Ioc;
using Nokia.Music;
using Nokia.Music.Types;

namespace InTwo
{
    public static class Utils
    {
        private static readonly Random _random = new Random();

        internal static int GetRandomNumber(int minValue, int maxValue)
        {
            var number = (_random.NextDouble() * maxValue) + minValue;

            return (int)Math.Floor(number);
        }

        public static Uri GetSampleUri(this Product product)
        {
            var client = SimpleIoc.Default.GetInstance<MusicClient>();
            return client.GetTrackSampleUri(product.Id);
        }
    }
}
