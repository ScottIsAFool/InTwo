using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

namespace InTwo.Controls
{
    public partial class Tile : UserControl
    {
        public Tile(string score, string genre, TileTypes tileTypes)
        {
            InitializeComponent();

            DataContext = this;

            if (ViewModelBase.IsInDesignModeStatic)
            {
                BestScoreText = "3368";
                TileType = TileTypes.Normal;
            }
            else
            {
                BestScoreText = score;
                GenreText = genre;
                TileType = tileTypes;
            }
        }

        public static readonly DependencyProperty BestScoreTextProperty =
            DependencyProperty.Register("BestScoreText", typeof(string), typeof(Tile), new PropertyMetadata(default(string)));

        public string BestScoreText
        {
            get { return (string)GetValue(BestScoreTextProperty); }
            set { SetValue(BestScoreTextProperty, value); }
        }

        public static readonly DependencyProperty GenreTextProperty =
            DependencyProperty.Register("GenreText", typeof(string), typeof(Tile), new PropertyMetadata(default(string)));

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
            return await Utils.SaveTile(this, 336, 691, Constants.Tiles.WideTileFile);
        }

        internal async Task<bool> SaveNormalTile()
        {
            return await Utils.SaveTile(this, 336, 336, Constants.Tiles.NormalTileFile);
        }
        
        public enum TileTypes
        {
            Wide,
            Normal
        }
    }
}
