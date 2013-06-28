using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Cimbalino.Phone.Toolkit.Extensions;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using InTwo.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace InTwo
{
    public class TileService
    {
        private static readonly IShellTileService ShellTileService = new ShellTileWithCreateService();

        private static TileService _current;
        public static TileService Current
        {
            get { return _current ?? (_current = new TileService()); }
        }

        public async void UpdatePrimaryTile()
        {
            ShellTileServiceFlipTileData shellData;
            if (App.CurrentPlayer != null && App.SettingsWrapper.AppSettings.PlayerWrapper.BestScore > 0)
            {
                var score = App.SettingsWrapper.AppSettings.PlayerWrapper.BestScore;
                var genre = App.SettingsWrapper.AppSettings.PlayerWrapper.BestScoreGenre;

                // Create wide tile image
                var wideTile = CreateBackImage(score.ToStringInvariantCulture(), genre, true);
                var wideTileCreated = await Utils.SaveTile(wideTile, 336, 691, string.Format(Constants.Tiles.WideTileBackFile, App.CurrentPlayer.Username));

                // Create normal tile image
                var normalTile = CreateBackImage(score.ToStringInvariantCulture(), genre, false);
                var normalTileCreated = await Utils.SaveTile(normalTile, 336, 336, string.Format(Constants.Tiles.NormalTileBackFile, App.CurrentPlayer.Username));

                shellData = await CreateTileData(wideTileCreated, normalTileCreated, true);
            }
            else
            {
                shellData = await CreateTileData(false, false, true);
            }

            UpdateTile(shellData);
        }

        private static void UpdateTile(IShellTileServiceTileData shellData)
        {
            var primaryTile = ShellTileService.ActiveTiles.First();

            primaryTile.Update(shellData);
        }

        public async void ClearTile()
        {
            var shellData = await CreateTileData(false, false, false);

            UpdateTile(shellData);

            DeleteTileImages();
        }

        private static async Task<ShellTileServiceFlipTileData> CreateTileData(bool wideTileCreated, bool normalTileCreated, bool showUsername)
        {
            var useProfilePicture = await ShouldUseUserProfilePicture();

            var shell = new ShellTileServiceFlipTileData
            {
                BackBackgroundImage = normalTileCreated ? new Uri(string.Format(Constants.Tiles.NormalTileBackIsoUri, App.CurrentPlayer.Username), UriKind.RelativeOrAbsolute) : null,
                WideBackBackgroundImage = wideTileCreated ? new Uri(string.Format(Constants.Tiles.WideTileBackIsoUri, App.CurrentPlayer.Username), UriKind.RelativeOrAbsolute) : null,
                BackTitle = (showUsername && App.CurrentPlayer != null) ? App.CurrentPlayer.Username : string.Empty,
                BackgroundImage = CreateFrontTileImageUrl(useProfilePicture, TileSize.Medium),
                WideBackgroundImage = CreateFrontTileImageUrl(useProfilePicture, TileSize.Large),
                SmallBackgroundImage = CreateFrontTileImageUrl(false, TileSize.Small),
                Title = useProfilePicture ? "In Two" : string.Empty
            };

            return shell;
        }

        private static Uri CreateFrontTileImageUrl(bool useProfilePicture, TileSize tileSize)
        {
            if (useProfilePicture)
            {
                switch (tileSize)
                {
                    case TileSize.Large:
                        return new Uri(string.Format(Constants.Tiles.UserProfileWideIsoUriFormat, App.CurrentPlayer.Username), UriKind.RelativeOrAbsolute);
                    case TileSize.Medium:
                        return new Uri(string.Format(Constants.Tiles.UserProfileIsoUriFormat, App.CurrentPlayer.Username), UriKind.RelativeOrAbsolute);
                    default:
                        return null;
                }
            }
            
            var transparentText = App.SettingsWrapper.AppSettings.UseTransparentTileBackground ? "Transparent" : string.Empty;
                
            return new Uri(string.Format(Constants.Tiles.AppTileFormat, transparentText, tileSize), UriKind.Relative);
        }

        private static async Task<bool> ShouldUseUserProfilePicture()
        {
            var asyncService = SimpleIoc.Default.GetInstance<IAsyncStorageService>();
            return App.CurrentPlayer != null 
                && App.SettingsWrapper.AppSettings.UseProfilePictureInTile 
                && await asyncService.FileExistsAsync(string.Format(Constants.Tiles.UserProfileFileFormat, App.CurrentPlayer.Username));
        }

        private static async void DeleteTileImages()
        {
            var asyncService = SimpleIoc.Default.GetInstance<IAsyncStorageService>();

            if (await asyncService.FileExistsAsync(Constants.Tiles.NormalTileFile))
            {
                await asyncService.DeleteFileAsync(Constants.Tiles.NormalTileFile);
            }

            if (await asyncService.FileExistsAsync(Constants.Tiles.WideTileFile))
            {
                await asyncService.DeleteFileAsync(Constants.Tiles.WideTileFile);
            }
        }

        private static UIElement CreateBackImage(string score, string genre, bool isWideTile)
        {
            var grid = new Grid
            {
                Height = 336,
                Width = isWideTile ? 691 : 336
            };

            var stackpanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };

            if (isWideTile)
            {
                var scoreText = string.Format("Best score: {0}", score);
                var scoreBlock = CreateTextBlock(scoreText, 95);
                stackpanel.Children.Add(scoreBlock);

                var genreText = string.Format("Genre: {0}", genre);
                var genreBlock = CreateTextBlock(genreText, 95, new Thickness(12, 100, 0, 0));
                stackpanel.Children.Add(genreBlock);

                grid.Children.Add(stackpanel);
            }
            else
            {
                var scoreText = string.Format("Best score: {0}", score);
                var scoreBlock = CreateTextBlock(scoreText);
                stackpanel.Children.Add(scoreBlock);

                var genreLabel = CreateTextBlock("Genre:", margin: new Thickness(12, 100, 0, 0));
                stackpanel.Children.Add(genreLabel);

                var genreBlock = CreateTextBlock(genre, margin: new Thickness(12, 150, 0, 0));
                stackpanel.Children.Add(genreBlock);

                grid.Children.Add(stackpanel);
            }

            return grid;
        }

        private static TextBlock CreateTextBlock(string text, int? fontHeight = null, Thickness? margin = null)
        {
            var textBlock = new TextBlock
            {
                Text = text,
                Style = (Style) Application.Current.Resources["PhoneTextExtraLargeStyle"]
            };

            if (fontHeight.HasValue)
            {
                textBlock.FontSize = fontHeight.Value;
            }

            if (margin.HasValue)
            {
                textBlock.Margin = margin.Value;
            }

            return textBlock;
        }
    }
}
