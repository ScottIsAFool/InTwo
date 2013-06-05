using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using ImageTools;
using ImageTools.IO.Png;

namespace InTwo.Controls
{
    public partial class WideTile : UserControl
    {
        public WideTile()
        {
            InitializeComponent();
            
            DataContext = this;
            
            if (ViewModelBase.IsInDesignModeStatic)
            {
                BestScoreText = "3368";
            }
        }

        public static readonly DependencyProperty BestScoreTextProperty =
            DependencyProperty.Register("BestScoreText", typeof (string), typeof (WideTile), new PropertyMetadata(default(string)));

        public string BestScoreText
        {
            get { return (string) GetValue(BestScoreTextProperty); }
            set { SetValue(BestScoreTextProperty, value); }
        }

        public static readonly DependencyProperty GenreTextProperty =
            DependencyProperty.Register("GenreText", typeof (string), typeof (WideTile), new PropertyMetadata(default(string)));

        public string GenreText
        {
            get { return (string) GetValue(GenreTextProperty); }
            set { SetValue(GenreTextProperty, value); }
        }

        public async Task<bool> SaveWideTile()
        {
            try
            {
                var asyncService = SimpleIoc.Default.GetInstance<IAsyncStorageService>();

                if (await asyncService.FileExistsAsync(Constants.Tiles.WideTileFile))
                {
                    await asyncService.DeleteFileAsync(Constants.Tiles.WideTileFile);
                }

                var tileBitmap = new WriteableBitmap(691, 336);
                tileBitmap.Render(this, null);
                tileBitmap.Invalidate();

                using (var file = await asyncService.CreateFileAsync(Constants.Tiles.WideTileFile))
                {
                    var encoder = new PngEncoder();
                    var image = tileBitmap.ToImage();
                    encoder.Encode(image, file);
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
