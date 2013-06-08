using System;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using InTwo.Controls;
using Microsoft.Phone.Controls;

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
            var score = App.SettingsWrapper.AppSettings.PlayerWrapper.BestScore;
            var genre = App.SettingsWrapper.AppSettings.PlayerWrapper.BestScoreGenre;

            // Create wide tile image
            //var wideTileCreated = await new Tile(score, genre, Tile.TileTypes.Wide).SaveWideTile();

            //// Create normal tile image
            //var normalTileCreated = await new Tile(score, genre, Tile.TileTypes.Normal).SaveNormalTile();

            //var shellData = await CreateTileData(wideTileCreated, normalTileCreated, true);

            var shellData = await CreateTileData(false, false, true);

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
                BackBackgroundImage = normalTileCreated ? new Uri(Constants.Tiles.NormalTileIsoUri, UriKind.RelativeOrAbsolute) : null,
                WideBackBackgroundImage = wideTileCreated ? new Uri(Constants.Tiles.WideTileIsoUri, UriKind.RelativeOrAbsolute) : null,
                BackTitle = showUsername ? App.CurrentPlayer.username : string.Empty,
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
                        return new Uri(string.Format(Constants.Tiles.UserProfileWideIsoUriFormat, App.CurrentPlayer.username), UriKind.RelativeOrAbsolute);
                    case TileSize.Medium:
                        return new Uri(string.Format(Constants.Tiles.UserProfileIsoUriFormat, App.CurrentPlayer.username), UriKind.RelativeOrAbsolute);
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
                && await asyncService.FileExistsAsync(string.Format(Constants.Tiles.UserProfileFileFormat, App.CurrentPlayer.username));
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

        
    }
}
