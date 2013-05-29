using System;

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
    }
}
