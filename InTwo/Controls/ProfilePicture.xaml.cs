using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InTwo.Controls
{
    public partial class ProfilePicture : UserControl
    {
        public ProfilePicture()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IsClippedProperty = DependencyProperty.Register(
            "IsClipped", 
            typeof (bool), 
            typeof (ProfilePicture), 
            new PropertyMetadata(default(bool), OnIsClippedChanged));

        private static void OnIsClippedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender == null) return;

            var pp = sender as ProfilePicture;

            if (pp == null) return;

            pp.BorderToClip.Clip = pp.IsClipped
                                       ? new EllipseGeometry
                                             {
                                                 Center = new Point(85, 85),
                                                 RadiusX = 85,
                                                 RadiusY = 85
                                             }
                                       : null;
        }

        public bool IsClipped
        {
            get { return (bool) GetValue(IsClippedProperty); }
            set { SetValue(IsClippedProperty, value); }
        }
    }
}
