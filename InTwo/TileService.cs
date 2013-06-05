using System;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using InTwo.Controls;

namespace InTwo
{
    public class TileService
    {
        private static readonly IShellTileService ShellTileService = new ShellTileWithAddService();

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
            var wideTileCreated = await new Tile(score, genre, Tile.TileTypes.Wide).SaveWideTile();

            // Create normal tile image
            var normalTileCreated = await new Tile(score, genre, Tile.TileTypes.Normal).SaveNormalTile();

            var shellData = CreateTileData(wideTileCreated, normalTileCreated, true);

            UpdateTile(shellData);
        }

        private static void UpdateTile(IShellTileServiceTileData shellData)
        {
            var primaryTile = ShellTileService.ActiveTiles.First();

            primaryTile.Update(shellData);
        }

        public void ClearTile()
        {
            var shellData = CreateTileData(false, false, false);

            UpdateTile(shellData);

            DeleteTileImages();
        }

        private static ShellTileServiceFlipTileData CreateTileData(bool wideTileCreated, bool normalTileCreated, bool showUsername)
        {
            var shell = new ShellTileServiceFlipTileData
            {
                BackBackgroundImage = normalTileCreated ? new Uri(Constants.Tiles.NormalTileIsoUri, UriKind.Relative) : null,
                WideBackBackgroundImage = wideTileCreated ? new Uri(Constants.Tiles.WideTileIsoUri) : null,
                BackTitle = showUsername ? App.CurrentPlayer.username : string.Empty
            };

            return shell;
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
