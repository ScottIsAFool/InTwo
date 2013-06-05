using System;
using System.Threading.Tasks;
using Cimbalino.Phone.Toolkit.Services;
using InTwo.Controls;

namespace InTwo
{
    public class TileService
    {
        private static IShellTileService _shellTileService = new ShellTileWithAddService();
        private static TileService _current;
        public static TileService Current
        {
            get { return _current ?? (_current = new TileService()); }
        }

        public async Task UpdatePrimaryTile()
        {
            var score = App.CurrentPlayer.best_score;
            var genre = App.CurrentPlayer.last_level;

            // Create wide tile image
            var wideTileCreated = await new Tile(score, genre, Tile.TileTypes.Wide).SaveWideTile();

            // Create normal tile image
            var normalTileCreated = await new Tile(score, genre, Tile.TileTypes.Normal).SaveNormalTile();

            var shellData = CreateTileData(wideTileCreated, normalTileCreated, true);
        }

        public void ClearTile()
        {
            var shellData = CreateTileData(false, false, false);
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
    }
}
