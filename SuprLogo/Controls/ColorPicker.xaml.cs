using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SuprLogo.Controls
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        public Color ColorValue
        {
            get
            {
                return (Color)GetValue(ColorValueProperty);
            }
            set
            {
                SetValue(ColorValueProperty, value);
            }
        }

        public static readonly DependencyProperty ColorValueProperty =
            DependencyProperty.Register("ColorValue", typeof(Color), typeof(ColorPicker), new UIPropertyMetadata(Colors.Transparent));

        public ColorPicker()
        {
            InitializeComponent();
        }
    }
}
