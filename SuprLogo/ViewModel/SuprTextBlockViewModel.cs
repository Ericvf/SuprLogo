using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace SuprLogo.ViewModel
{
    [Serializable]
    public class SuprTextBlockViewModel : INotifyPropertyChanged
    {
        private string text = "Sample Text";
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
                this.RaisePropertyChanged("Text");
            }
        }

        private bool isAutoZoom = false;
        public bool IsAutoZoom
        {
            get
            {
                return this.isAutoZoom;
            }
            set
            {
                this.isAutoZoom = value;
                this.RaisePropertyChanged("IsAutoZoom");
            }
        }

        private Brush backgroundBrush;
        [XmlIgnore]
        public Brush BackgroundBrush
        {
            get
            {
                return this.backgroundBrush;
            }
            set
            {
                this.backgroundBrush = value;
                this.RaisePropertyChanged("BackgroundBrush");
            }
        }

        private Color backgroundColor = Colors.CornflowerBlue;
        public Color BackgroundColor
        {
            get
            {
                return this.backgroundColor;
            }
            set
            {
                this.backgroundColor = value;
                this.CreateBackgroundBrush();
                this.RaisePropertyChanged("BackgroundColor");
            }
        }

        private bool bold;
        public bool Bold
        {
            get
            {
                return this.bold;
            }
            set
            {
                this.bold = value;
                this.RaisePropertyChanged("Bold");
            }
        }

        private bool italic;
        public bool Italic
        {
            get
            {
                return this.italic;
            }
            set
            {
                this.italic = value;
                this.RaisePropertyChanged("Italic");
            }
        }

        private Color foregroundStop = Colors.Red;
        public Color ForegroundStop
        {
            get
            {
                return this.foregroundStop;
            }
            set
            {
                this.foregroundStop = value;
                this.RaisePropertyChanged("ForegroundStop");
                CreateForegroundBrush();
            }
        }

        private Color foregroundStart = Colors.Black;
        public Color ForegroundStart
        {
            get
            {
                return this.foregroundStart;
            }
            set
            {
                this.foregroundStart = value;
                this.RaisePropertyChanged("ForegroundStart");
                CreateForegroundBrush();
            }
        }

        private Color strokeStop = Colors.White;
        public Color StrokeStop
        {
            get
            {
                return this.strokeStop;
            }
            set
            {
                this.strokeStop = value;
                this.RaisePropertyChanged("StrokeStop");
                CreateStrokeBrush();
            }
        }

        private Color strokeStart = Colors.Gray;
        public Color StrokeStart
        {
            get
            {
                return this.strokeStart;
            }
            set
            {
                this.strokeStart = value;
                this.RaisePropertyChanged("StrokeStart");
                CreateStrokeBrush();
            }
        }

        private double fontSize = 32;
        public double FontSize
        {
            get
            {
                return this.fontSize;
            }
            set
            {
                this.fontSize = value;
                this.RaisePropertyChanged("FontSize");
            }
        }

        private double strokeSize = 5;
        public double StrokeSize
        {
            get
            {
                return this.strokeSize;
            }
            set
            {
                this.strokeSize = value;
                this.RaisePropertyChanged("StrokeSize");
            }
        }

        private Brush foregroundBrush;
        [XmlIgnore]
        public Brush ForegroundBrush
        {
            get
            {
                return this.foregroundBrush;
            }
            set
            {
                this.foregroundBrush = value;
                this.RaisePropertyChanged("ForegroundBrush");
            }
        }

        private Brush strokeBrush;
        [XmlIgnore]
        public Brush StrokeBrush
        {
            get
            {
                return this.strokeBrush;
            }
            set
            {
                this.strokeBrush = value;
                this.RaisePropertyChanged("StrokeBrush");
            }
        }

        private double strokeOpacity = 1;
        public double StrokeOpacity
        {
            get
            {
                return this.strokeOpacity;
            }
            set
            {
                this.strokeOpacity = value;
                this.RaisePropertyChanged("StrokeOpacity");
            }
        }

        private double shadowDepth = 2;
        public double ShadowDepth
        {
            get
            {
                return this.shadowDepth;
            }
            set
            {
                this.shadowDepth = value;
                this.RaisePropertyChanged("ShadowDepth");
            }
        }

        private double shadowSoftness = 1;
        public double ShadowSoftness
        {
            get
            {
                return this.shadowSoftness;
            }
            set
            {
                this.shadowSoftness = value;
                this.RaisePropertyChanged("ShadowSoftness");
            }
        }

        private double shadowOpacity = 1;
        public double ShadowOpacity
        {
            get
            {
                return this.shadowOpacity;
            }
            set
            {
                this.shadowOpacity = value;
                this.RaisePropertyChanged("ShadowOpacity");
            }
        }

        private double shadowDirection = 0;
        public double ShadowDirection
        {
            get
            {
                return this.shadowDirection;
            }
            set
            {
                this.shadowDirection = value;
                this.RaisePropertyChanged("ShadowDirection");
            }
        }

        private double foregroundOpacity = 1;
        public double ForegroundOpacity
        {
            get
            {
                return this.foregroundOpacity;
            }
            set
            {
                this.foregroundOpacity = value;
                this.RaisePropertyChanged("ForegroundOpacity");
            }
        }

        private FontFamily selectedFont;
        [XmlIgnore]
        public FontFamily SelectedFont
        {
            get
            {
                return this.selectedFont;
            }
            set
            {
                this.selectedFont = value;
                this.RaisePropertyChanged("SelectedFont");
            }
        }
        public string Font
        {
            get
            {
                return this.SelectedFont.ToString();
            }
            set
            {
                this.SelectedFont = new FontFamily(value);
            }
        }        

        public SuprTextBlockViewModel()
        {
            this.CreateBackgroundBrush();
            this.CreateForegroundBrush();
            this.CreateStrokeBrush();
        }

        public SuprTextBlockViewModel(SuprTextBlockViewModel source)
        {
            this.Text = source.Text;

            this.Font = source.Font;
            this.FontSize = source.FontSize;

            this.BackgroundColor = source.BackgroundColor;

            this.ForegroundStart = source.ForegroundStart;
            this.ForegroundStop= source.ForegroundStop;
            
            this.StrokeStart = source.StrokeStart;
            this.StrokeStop = source.StrokeStop;
            this.StrokeSize = source.StrokeSize;
            this.StrokeOpacity = source.StrokeOpacity;
            
            this.ShadowDepth = source.ShadowDepth;
            this.ShadowDirection = source.ShadowDirection;
            this.ShadowSoftness = source.ShadowSoftness;
            this.ShadowOpacity = source.ShadowOpacity;

            this.IsAutoZoom = source.IsAutoZoom;
            this.Bold = source.Bold;
            this.Italic = source.Italic;

            this.CreateBackgroundBrush();
            this.CreateForegroundBrush();
            this.CreateStrokeBrush();
        }

        void CreateBackgroundBrush()
        {
            this.BackgroundBrush = new SolidColorBrush(this.backgroundColor);
        }

        void CreateForegroundBrush()
        {
            var foregroundBrush = new LinearGradientBrush();
            foregroundBrush.StartPoint = new Point(0.5, 0);
            foregroundBrush.EndPoint = new Point(0.5, 1);
            foregroundBrush.GradientStops.Add(new GradientStop(this.ForegroundStart, 0));
            foregroundBrush.GradientStops.Add(new GradientStop(this.ForegroundStop, 1));
            this.ForegroundBrush = foregroundBrush;
        }

        void CreateStrokeBrush()
        {
            var foregroundBrush = new LinearGradientBrush();
            foregroundBrush.StartPoint = new Point(0.5, 0);
            foregroundBrush.EndPoint = new Point(0.5, 1);
            foregroundBrush.GradientStops.Add(new GradientStop(this.StrokeStart, 0));
            foregroundBrush.GradientStops.Add(new GradientStop(this.StrokeStop, 1));
            this.StrokeBrush = foregroundBrush;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
