using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SuprLogo.Controls;
using SuprLogo.ViewModel;
using System.Windows.Media;
using AnimationExtensions;
using System.Collections.Generic;

namespace SuprLogo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel vm;

        public static T FindChild<T>(DependencyObject depObj, string childName)
            where T : DependencyObject
        {
            // Confirm obj is valid.  
            if (depObj == null) return null;

            // success case 
            if (depObj is T && ((FrameworkElement)depObj).Name == childName)
                return depObj as T;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                //DFS 
                T obj = FindChild<T>(child, childName);

                if (obj != null)
                    return obj;
            }

            return null;
        }

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = vm = new MainViewModel();
            this.vm.Load();
            this.LoadAnimation();

            this.KeyUp += MainWindow_KeyUp;
            SizeChanged += MainWindow_SizeChanged;
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                var viewbox = FindChild<Viewbox>(this.mainSuprTextBlock, "viewbox");
                var child = VisualTreeHelper.GetChild(viewbox, 0) as ContainerVisual;
                var scale = child.Transform as ScaleTransform;


                var grid = FindChild<Grid>(this.mainSuprTextBlock, "grid");
                var width = scale.ScaleX * grid.RenderSize.Width;
                var height = scale.ScaleY * grid.RenderSize.Height;

                this.tbWidth.Text = "width: " + (int)width + "px";
                this.tbHeight.Text = "height: " + (int)height + "px";
            }
            catch
            {

            }
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.C &&  Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                vm.CopyLogo(this.canvas);
            }
        }

        private void LoadAnimation()
        {
            // Save current viewmodel properties
            string currentText = this.vm.TextBlockViewModel.Text;
            bool currentZoomMode = this.vm.TextBlockViewModel.IsAutoZoom;

            // Find Border controls in left and right containers
            var leftitems = this.leftcontainer.FindChilden<Border>();
            var rightitems = this.rightcontainer.FindChilden<Border>();

            // Create setup action (preconditions)
            Action setup = () =>
            {
                // Update VM with Suprlogo and zoom
                this.vm.TextBlockViewModel.Text = "Suprlogo";
                this.vm.TextBlockViewModel.IsAutoZoom = true;

                // Hide panel Border controls
                leftitems.Hide();
                rightitems.Hide();

                // Hide controls
                this.leftpanel.Hide();
                this.rightpanel.Hide();
                this.leftsplit.Hide();
                this.rightsplit.Hide();
                this.output.Hide();
                this.canvas.Hide();
            };

            // speeds
            var speed1 = 300;
            var speed2 = 500;

            // Animation for leftpanel
            var leftin = Ax.Pro()
                .Move(x: -200)
                .Fade()
             .Then()
                .Move(duration: speed2, eq: Eq.OutBack)
                .Fade(1, speed2);

            // Animation for items in leftpanel
            var leftcontent = Ax.For(leftitems, (i, b) => b
                .Wait(i * 150)
                .Move(x: -200)
                .Fade(1, speed1)
                .Move(duration: speed1, eq: Eq.OutCirc)
                );
            
            // Animation for left panel, splitter and Borders
            var animateLeft = Ax.New()
                .And(leftin, this.leftpanel) // do the panel
                .And(leftin, this.leftsplit) // do the splitter
                .AndThen(leftcontent); // do the items in the panel

            // Animation for rightpanel
            var rightin = Ax.Pro()
                .Move(x: 200)
                .Fade()
             .Then()
                .Move(duration: speed2, eq: Eq.OutBack)
                .Fade(1, speed2);

            // Animation for items in rightpanel
            var rightcontent = Ax.For(rightitems, (i, b) => b
                .Wait(i * 150)
                .Move(x: 200)
                .Fade(1, speed1)
                .Move(duration: speed1, eq: Eq.OutCirc)
                );

            // Animation for right panel, splitter and Borders
            var animateRight = Ax.New()
                .And(rightin, this.rightpanel) // do the panel
                .And(rightin, this.rightsplit) // do the splitter
                .AndThen(rightcontent); // do the items in the panel

            // Animation for main textbox (logo)
            var animateCanvas = this.canvas
                .Move(y: -600)
                .Move(duration: speed2 * 2, eq: Eq.OutElastic)
                .Fade(1, duration: speed2)
            .Then()
                .Fade(0, duration: speed1)
            .ThenDo((e)=>{
                    // Reset the VM
                    this.vm.TextBlockViewModel.Text = currentText;
                    this.vm.TextBlockViewModel.IsAutoZoom = currentZoomMode;
                })
                .Fade(1, duration: speed2);

            // Main animation
            var animate = Ax.New()
                .Do((e) => setup()) // exec setup method
                .Wait(500)
                .AndThen(animateCanvas) // show the canvas 
                .AndThen(animateLeft) // leftpanel
                .And(animateRight) // rightpanel

                .AndThen(this.output.Fade(1, speed2)); // "width px; height px" box


            // DO IT ALL! xD
            animate.Play();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.vm.Save();
        }

        private void SuprListItemTextBlock_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                var item = sender as SuprListItemTextBlock;
                if (item != null)
                    this.vm.LoadPresetCommand.Execute(item.DataContext);
            }

        }

        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
                this.vm.BrowseCommand.Execute(null);
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var tbSender = sender as TextBox;
                var binding = tbSender.GetBindingExpression(TextBox.TextProperty);
                binding.UpdateSource();
            }
        }

        #region Capture Gauge
        bool isCapture;
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(this.captureGrid);
            isCapture = true;
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.captureGrid.ReleaseMouseCapture();
            isCapture = false;
        }

        private void captureGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (isCapture)
            {
                var pos = Mouse.GetPosition(this.rotationPoint);
                var angle = Math.Atan2(pos.Y, pos.X) * (180 / Math.PI);

                vm.TextBlockViewModel.ShadowDirection = angle < 0 
                    ? 360 + angle 
                    : angle;
            }
        }
        #endregion
    }

}
