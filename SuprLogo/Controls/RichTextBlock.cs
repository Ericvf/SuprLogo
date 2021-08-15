using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Globalization;

namespace SuprLogo.Controls
{

    public class RichTextBlock : FrameworkElement
    {
        VisualCollection visuals;
        Geometry geometry;
        Pen strokePen;

        #region DependencyProperties

        public int FontSize
        {
            get { return (int)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(int), typeof(RichTextBlock), new UIPropertyMetadata(24, PropertyChanged));


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(RichTextBlock), new UIPropertyMetadata("RichTextBlock", PropertyChanged));


        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Foreground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(RichTextBlock), new UIPropertyMetadata(Brushes.Black, PropertyChanged));

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); this.InvalidateVisual(); }
        }

        // Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(RichTextBlock), new UIPropertyMetadata(Brushes.Transparent, PropertyChanged));

        public bool Bold
        {
            get { return (bool)GetValue(BoldProperty); }
            set { SetValue(BoldProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Bold.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BoldProperty =
            DependencyProperty.Register("Bold", typeof(bool), typeof(RichTextBlock), new UIPropertyMetadata(false, PropertyChanged));

        public bool Italic
        {
            get { return (bool)GetValue(ItalicProperty); }
            set { SetValue(ItalicProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Italic.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItalicProperty =
            DependencyProperty.Register("Italic", typeof(bool), typeof(RichTextBlock), new UIPropertyMetadata(false, PropertyChanged));

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(RichTextBlock), new UIPropertyMetadata(0D, PropertyChanged));



        public FontFamily Font
        {
            get { return (FontFamily)GetValue(FontProperty); }
            set { SetValue(FontProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Font.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontProperty =
            DependencyProperty.Register("Font", typeof(FontFamily), typeof(RichTextBlock), new UIPropertyMetadata(new FontFamily("Verdana"), PropertyChanged));


        #endregion

        static void PropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var richTextBlock = dependencyObject as RichTextBlock;
            if (richTextBlock != null)
                richTextBlock.InvalidateGeometry();
        }

        public RichTextBlock()
        {
            visuals = new VisualCollection(this);
            Loaded += new RoutedEventHandler(RichTextBlock_Loaded);
        }

        void RichTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            this.InvalidateGeometry();
        }

        /// <summary>
        /// Create the outline geometry based on the formatted text.
        /// </summary>
        internal void InvalidateGeometry()
        {
            if (this.Font == null)
                return;


            FontStyle fontStyle = FontStyles.Normal;
            FontWeight fontWeight = FontWeights.Medium;

            if (this.Bold) fontWeight = FontWeights.Bold;
            if (this.Italic) fontStyle = FontStyles.Italic;

            var text = this.Text.Replace(@"\n", Environment.NewLine);
            FormattedText formattedText = new FormattedText(
                text,
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface(
                    this.Font,
                    fontStyle,
                    fontWeight,
                    FontStretches.Normal),
                this.FontSize,
                Brushes.Black
                );


            formattedText.Trimming = TextTrimming.None;
            this.strokePen = new Pen(this.Stroke, this.StrokeThickness);

            // Build the geometry object that represents the text.
            geometry = formattedText.BuildGeometry(new System.Windows.Point(0, 0));

            var bounds = geometry.GetRenderBounds(this.strokePen);
            geometry = formattedText.BuildGeometry(new System.Windows.Point(-bounds.Left, -bounds.Top));

            this.InvalidateMeasure();
            this.InvalidateVisual();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size desiredSize = new Size();

            if (this.geometry != null)
            {
                var bounds = this.geometry.GetRenderBounds(this.strokePen);
                desiredSize = new Size(bounds.Width + bounds.Left,
                                        bounds.Height + bounds.Top);
            }

            return desiredSize;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawGeometry(this.Foreground, this.strokePen, geometry);
            base.OnRender(drawingContext);
        }

        protected override Visual GetVisualChild(int index)
        {
            return visuals[index];
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return visuals.Count;
            }
        }
    }
}
