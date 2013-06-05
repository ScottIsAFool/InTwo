using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using ImageTools;
using ImageTools.IO.Png;
using Nokia.Music;
using Nokia.Music.Types;

namespace InTwo
{
    public static class Utils
    {
        private static readonly Random Random = new Random();

        internal static int GetRandomNumber(int minValue, int maxValue)
        {
            var number = (Random.NextDouble() * maxValue) + minValue;

            return (int)Math.Floor(number);
        }

        internal static Uri GetSampleUri(this Product product)
        {
            var client = SimpleIoc.Default.GetInstance<IMusicClient>();
            return client.GetTrackSampleUri(product.Id);
        }

        private static readonly string[] IgnoredWords = {"the", "and", "a", "an"};

        internal static void SeeIfWeHaveAWinner(Product gameTrack, string artistGuess, string songGuess, out bool artistGuessCorrect, out bool songGuessCorrect)
        {
            artistGuessCorrect = songGuessCorrect = false;

            if (!string.IsNullOrEmpty(artistGuess))
            {
                foreach (var artist in gameTrack.Performers)
                {
                    if (CheckWords(artist, artistGuess, ref artistGuessCorrect))
                    {
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(songGuess))
            {
                CheckWords(gameTrack, songGuess, ref artistGuessCorrect);
            }
        }

        private static bool CheckWords(MusicItem gameInfo, string guess, ref bool guessCorrect)
        {
            var splitWords = gameInfo.Name.ToLower().Split(new[] { ' ' }).ToList();
            splitWords.ForEach(word =>
            {
                if (IgnoredWords.Contains(word))
                {
                    splitWords.Remove(word);
                }
            });

            var guessSplitWords = guess.ToLower().Split(new[] { ' ' }).ToList();
            guessSplitWords.ForEach(word =>
            {
                if (IgnoredWords.Contains(word))
                {
                    guessSplitWords.Remove(word);
                }
            });

            var numberOfCorrectWords = 0;
            guessSplitWords.ForEach(word =>
            {
                if (splitWords.Contains(word))
                {
                    numberOfCorrectWords++;
                }
            });

            if (numberOfCorrectWords > (splitWords.Count * 0.9))
            {
                guessCorrect = true;
                return true;
            }
            return false;
        }

        internal static CheckBox CreateDontShowCheckBox(string propertyName)
        {
            var cb = new CheckBox { Content = "Don't show this message again" };

            var binding = new Binding
            {
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath(propertyName),
                Source = App.SettingsWrapper.AppSettings
            };

            cb.SetBinding(CheckBox.IsCheckedProperty, binding);

            return cb;
        }

        public static async Task<bool> SaveTile(UIElement tile, int height, int width, string file)
        {
            try
            {
                var asyncService = SimpleIoc.Default.GetInstance<IAsyncStorageService>();

                if (await asyncService.FileExistsAsync(file))
                {
                    await asyncService.DeleteFileAsync(file);
                }

                var tileBitmap = new WriteableBitmap(width, height);
                tileBitmap.Render(tile, null);
                tileBitmap.Invalidate();

                using (var fileStream = await asyncService.CreateFileAsync(file))
                {
                    var encoder = new PngEncoder();
                    var image = tileBitmap.ToImage();
                    encoder.Encode(image, fileStream);
                }

                return true;
            }
            catch
            {

            }
            return false;
        }
    }
}
