using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;

namespace InTwo.Controls
{
    public partial class Tile : UserControl
    {
        public Tile()
        {
            InitializeComponent();

            DataContext = this;

            if (ViewModelBase.IsInDesignModeStatic)
            {
                BestScoreText = "3368";
                TileType = TileTypes.Normal;
            }
        }

        public static readonly DependencyProperty BestScoreTextProperty =
            DependencyProperty.Register("BestScoreText", typeof(string), typeof(Tile), new PropertyMetadata(default(string), OnBestScoreTextChanged));

        private static void OnBestScoreTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var tile = sender as Tile;

            if (tile == null)
            {
                return;
            }

            var bestScoreText = string.Format("Best score: {0}", e.NewValue);
            tile.BestScoreNormal.Text = bestScoreText;
            tile.BestScoreWide.Text = bestScoreText;
        }

        public string BestScoreText
        {
            get { return (string)GetValue(BestScoreTextProperty); }
            set { SetValue(BestScoreTextProperty, value); }
        }

        public static readonly DependencyProperty GenreTextProperty =
            DependencyProperty.Register("GenreText", typeof(string), typeof(Tile), new PropertyMetadata(default(string), OnGenreTextChanged));

        private static void OnGenreTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var tile = sender as Tile;
            if (tile == null)
            {
                return;
            }

            tile.GenreTextNormal.Text = (string) e.NewValue;
            tile.GenreTextWide.Text = string.Format("Genre: {0}", e.NewValue);
        }

        public string GenreText
        {
            get { return (string)GetValue(GenreTextProperty); }
            set { SetValue(GenreTextProperty, value); }
        }

        public static readonly DependencyProperty TileTypeProperty =
            DependencyProperty.Register("TileType", typeof(TileTypes), typeof(Tile), new PropertyMetadata(default(TileTypes), TileTypeChanged));

        private static void TileTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var tile = (Tile)sender;

            switch ((TileTypes)e.NewValue)
            {
                case TileTypes.Normal:
                    tile.Height = tile.Width = 336;
                    tile.WideStack.Visibility = Visibility.Collapsed;
                    tile.NormalStack.Visibility = Visibility.Visible;
                    break;
                case TileTypes.Wide:
                    tile.Height = 336;
                    tile.Width = 691;
                    tile.WideStack.Visibility = Visibility.Visible;
                    tile.NormalStack.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        public TileTypes TileType
        {
            get { return (TileTypes)GetValue(TileTypeProperty); }
            set { SetValue(TileTypeProperty, value); }
        }

        internal async Task<bool> SaveWideTile()
        {
            InvalidateMeasure();
            return await Utils.SaveTile(this, 336, 691, string.Format(Constants.Tiles.WideTileBackFile, App.CurrentPlayer.Username));
        }

        internal async Task<bool> SaveNormalTile()
        {
            InvalidateMeasure();
            return await Utils.SaveTile(this, 336, 336, string.Format(Constants.Tiles.NormalTileBackFile, App.CurrentPlayer.Username));
        }
        
        public enum TileTypes
        {
            Wide,
            Normal
        }
    }
}
