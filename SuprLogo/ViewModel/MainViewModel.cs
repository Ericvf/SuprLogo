using SuprLogo.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Net.Http;

namespace SuprLogo.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<SuprTextBlockViewModel> Presets
        {
            get
            {
                return this.presets;
            }
            set
            {
                this.presets = value;
                this.RaisePropertyChanged("Presets");
            }
        }
        private ObservableCollection<SuprTextBlockViewModel> presets = new ObservableCollection<SuprTextBlockViewModel>();

        public SuprTextBlockViewModel TextBlockViewModel
        {
            get
            {
                return this.textBlockViewModel;
            }
            set
            {
                this.textBlockViewModel = value;
                this.RaisePropertyChanged("TextBlockViewModel");
            }
        }
        private SuprTextBlockViewModel textBlockViewModel;

        private string defaultDirectory;
        public string DefaultDirectory
        {
            get
            {
                return this.defaultDirectory;
            }
            set
            {
                this.defaultDirectory = value;
                this.RaisePropertyChanged("DefaultDirectory");
            }
        }

        private string batchLines;
        public string BatchLines
        {
            get
            {
                return this.batchLines;
            }
            set
            {
                this.batchLines = value;
                this.RaisePropertyChanged("BatchLines");
            }
        }


        private EncodingType encodingType;
        public EncodingType EncodingType
        {
            get
            {
                return this.encodingType;
            }
            set
            {
                this.encodingType = value;
                this.RaisePropertyChanged("EncodingType");
            }
        }

        [XmlIgnore]
        public string Extension
        {
            get
            {
                return this.GetExtension(this.EncodingType);
            }
        }

        #region Commands
        [XmlIgnore]
        public ICommand BrowseCommand { get; private set; }

        [XmlIgnore]
        public ICommand SaveCommand { get; private set; }

        [XmlIgnore]
        public ICommand SaveBatchCommand { get; private set; }

        [XmlIgnore]
        public ICommand SavePresetCommand { get; private set; }

        [XmlIgnore]
        public ICommand LoadPresetCommand { get; private set; }

        [XmlIgnore]
        public ICommand DeletePresetCommand { get; private set; }
        #endregion

        public MainViewModel()
        {
            this.TextBlockViewModel = new SuprTextBlockViewModel()
            {
                Text = "Sample Text",
                SelectedFont = new FontFamily("Mail Ray Stuff"),
                FontSize = 60,
                StrokeSize = 6,
                IsAutoZoom = true,
                BackgroundColor = Colors.Transparent,
                ForegroundStart = (Color)ColorConverter.ConvertFromString("#FFCC00"),
                ForegroundStop = (Color)ColorConverter.ConvertFromString("#FF6600"),
                StrokeStart = Colors.White,
                StrokeStop = Colors.White,
                StrokeOpacity = 1,
                ForegroundOpacity = 1,
                ShadowOpacity = 0.75,
                ShadowDepth = 5,
                ShadowSoftness = 0.5,
                ShadowDirection = 320
            };

            this.BrowseCommand = new ActionCommand(this.OnBrowseExecuted);
            this.SaveCommand = new ActionCommand(this.OnSaveExecuted);
            this.SavePresetCommand = new ActionCommand(this.OnSavePresetExecuted);
            this.LoadPresetCommand = new ActionCommand(this.OnLoadPresetExecuted);
            this.DeletePresetCommand = new ActionCommand(this.OnDeletePresetExecuted);
            this.SaveBatchCommand = new ActionCommand(this.OnSaveBatchExecuted);

            this.EncodingType = Framework.EncodingType.Png;
            this.DefaultDirectory = Directory.GetCurrentDirectory();
        }

        private void OnBrowseExecuted(object parameter)
        {
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = this.DefaultDirectory;

            var result = dialog.ShowDialog();

            if (result == DialogResult.OK)
                this.DefaultDirectory = dialog.SelectedPath;
        }

        private void OnSaveExecuted(object parameter)
        {
            var canvas = (parameter as FrameworkElement);
            var filename = this.GetFileName(this.DefaultDirectory, this.TextBlockViewModel.Text, this.Extension);
            SaveLogo(canvas, filename);
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public async void CopyLogo(FrameworkElement element)
        {
            var width = element.ActualWidth;
            var height = element.ActualHeight;

            RenderTargetBitmap bitmap = new RenderTargetBitmap(
                (int)Math.Ceiling(width), (int)Math.Ceiling(height),
                96, 96, PixelFormats.Pbgra32);

            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext dc = visual.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(element);
                dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
            }

            bitmap.Render(visual);

            ImageSource imageSource = ImageHelpers.AutoCropBitmap(bitmap, this.TextBlockViewModel.BackgroundColor);
            BitmapEncoder encoder = new BmpBitmapEncoder();

            switch (this.EncodingType)
            {
                case EncodingType.Png:
                    encoder = new PngBitmapEncoder();
                    break;
                case EncodingType.Jpg:
                    var jpgEncoder = new JpegBitmapEncoder();
                    jpgEncoder.QualityLevel = 100;
                    encoder = jpgEncoder;

                    break;
                case EncodingType.Bmp:
                    encoder = new BmpBitmapEncoder();
                    break;
                default:
                    break;
            }

            try
            {
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imageSource));
                //using (Stream stm = File.Create(filename))
                //{
                //    encoder.Save(stm);
                //}
                using (var ms = new MemoryStream())
                {
                    encoder.Save(ms);

                    var content = new ByteArrayContent(ms.ToArray());
                    HttpClient httpClient = new HttpClient();
                    await httpClient.PostAsync("http://localhost:81/", content);
                }
            }
            catch (Exception ex)
            {
                Microsoft.Windows.Controls.MessageBox.Show("Error during save:" + ex.Message.ToString());
            }
        }

        private void SaveLogo(FrameworkElement element, string filename)
        {
            var width = element.ActualWidth;
            var height = element.ActualHeight;

            RenderTargetBitmap bitmap = new RenderTargetBitmap(
                (int)Math.Ceiling(width), (int)Math.Ceiling(height),
                96, 96, PixelFormats.Pbgra32);

            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext dc = visual.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(element);
                dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
            }

            bitmap.Render(visual);

            ImageSource imageSource = ImageHelpers.AutoCropBitmap(bitmap, this.TextBlockViewModel.BackgroundColor);
            BitmapEncoder encoder = new BmpBitmapEncoder();

            switch (this.EncodingType)
            {
                case EncodingType.Png:
                    encoder = new PngBitmapEncoder();
                    break;
                case EncodingType.Jpg:
                    var jpgEncoder = new JpegBitmapEncoder();
                    jpgEncoder.QualityLevel = 100;
                    encoder = jpgEncoder;

                    break;
                case EncodingType.Bmp:
                    encoder = new BmpBitmapEncoder();
                    break;
                default:
                    break;
            }

            try
            {
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imageSource));
                using (Stream stm = File.Create(filename))
                {
                    encoder.Save(stm);
                }
            }
            catch (Exception ex)
            {
                Microsoft.Windows.Controls.MessageBox.Show("Error during save:" + ex.Message.ToString());
            }
        }

        private void OnSavePresetExecuted(object parameter)
        {
            this.Presets.Add(new SuprTextBlockViewModel(this.TextBlockViewModel));
            var orderedPresets = this.Presets.OrderBy(p => p.Text).ToList();
            this.Presets = new ObservableCollection<SuprTextBlockViewModel>(orderedPresets);
        }

        private void OnLoadPresetExecuted(object parameter)
        {
            var item = parameter as SuprTextBlockViewModel;
            if (item != null)
                this.TextBlockViewModel = new SuprTextBlockViewModel(item);
        }

        private void OnDeletePresetExecuted(object parameter)
        {
            var item = parameter as SuprTextBlockViewModel;
            if (item != null && this.presets.Contains(item))
                presets.Remove(item);

        }

        private void OnSaveBatchExecuted(object parameter)
        {
            var canvas = (parameter as FrameworkElement);
            if (canvas != null && this.BatchLines != null)
            {
                var defaultText = this.TextBlockViewModel.Text;
                var batchLines = this.BatchLines.Split('\n');
                //var i = 1;
                foreach (var line in batchLines)
                {
                    var text = line.Trim();

                    if (!string.IsNullOrEmpty(text))
                    {
                        //var namepart = string.Format("{0:000}_{1}", i++, text);
                        var namepart = text;

                        var filename = this.GetFileName(this.DefaultDirectory, namepart, this.Extension);
                        this.TextBlockViewModel.Text = text;

                        Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
                        {
                            SaveLogo(canvas, filename);
                        }), DispatcherPriority.ContextIdle, null);
                    }
                }

                this.TextBlockViewModel.Text = defaultText;
            }
        }

        public void Load()
        {
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                MainViewModel data = default(MainViewModel);

                if (myIsolatedStorage.FileExists("MainViewModel.xml"))
                {
                    //IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile("MainViewModel.xml", FileMode.Open, FileAccess.Read);
                    //using (StreamReader reader = new StreamReader(fileStream))
                    //{
                    //    var string1 = reader.ReadToEnd();
                    //}

                    using (IsolatedStorageFileStream stream = myIsolatedStorage.OpenFile("MainViewModel.xml", FileMode.Open))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(MainViewModel));
                        data = (MainViewModel)serializer.Deserialize(stream);

                    }
                }
                else
                {
                    var x = Assembly.GetExecutingAssembly().GetManifestResourceStream("SuprLogo.MainViewModel.xml");
                    XmlSerializer serializer = new XmlSerializer(typeof(MainViewModel));
                    data = (MainViewModel)serializer.Deserialize(x);
                }

                this.TextBlockViewModel = data.TextBlockViewModel;
                this.Presets = data.Presets;
                this.BatchLines = data.BatchLines;
                this.DefaultDirectory = data.DefaultDirectory;
                this.EncodingType = data.EncodingType;
            }
        }

        public void Save()
        {
            //try
            //{
            //    using (var fs = File.Open("Settings.xml", FileMode.Create))
            //    {
            //        var xmlWriterSettings = new XmlWriterSettings();
            //        xmlWriterSettings.Indent = true;
            //        XmlSerializer serializer = new XmlSerializer(typeof(MainViewModel));
            //        using (XmlWriter xmlWriter = XmlWriter.Create(fs, xmlWriterSettings))
            //        {
            //            serializer.Serialize(xmlWriter, this);
            //        }
            //    }
            //}
            //catch
            //{

            //}
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                var xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Indent = true;

                using (IsolatedStorageFileStream stream = myIsolatedStorage.OpenFile("MainViewModel.xml", FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(MainViewModel));
                    using (XmlWriter xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
                    {
                        serializer.Serialize(xmlWriter, this);
                    }
                }
            }
        }

        private string GetFileName(string directory, string filename, string extension)
        {
            filename = Regex.Replace(filename, @"[^\w\.@-]", "_");
            string fullname = Path.Combine(directory, filename + extension);
            int i = 0;

            while (File.Exists(fullname))
                fullname = Path.Combine(directory, string.Format("{0}_{1:000}{2}", filename, ++i, extension));

            return fullname;
        }

        private string GetExtension(EncodingType encodingType)
        {
            string returnValue = default(string);
            switch (this.EncodingType)
            {
                case EncodingType.Png:
                    returnValue = ".png";
                    break;
                case EncodingType.Jpg:
                    returnValue = ".jpg";
                    break;
                case EncodingType.Bmp:
                    returnValue = ".bmp";
                    break;
                default:
                    break;
            }
            return returnValue;
        }

        #region PropertyChanged
        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
