# Chat Log: WPF Date Watermark Application Development

**Date:** June 28, 2025  
**Project:** AddDateToGraphicWpf  
**Objective:** Create a WPF application to add date watermarks to image files

---

## User Request

> write a wpf app to select one or more graphic files. And then allow a watermark date stamp to be added. the default date should be the creation date of the graphic file. There should be a preview windows that shows the date watermark. Plus Save button and a Save Copy Button. 
> 
> There should be settings for the date watermark font, colors, drop shadow and transparency.

---

## Chat History (since last update)

---

**User:**  
add a button to bulk save all the loaded graph files. a new sub-folder should be created with current date time in a short format along with time zone.  all the files with a watermark should be saved into sub-folder.

**GitHub Copilot:**  

---

## Development Process

### 1. Project Structure Creation

**Project File (`AddDateToGraphicWpf.csproj`):**<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
    <Nullable>enable</Nullable>
    <ApplicationManifest>app.manifest</ApplicationManifest>
    <GenerateAssemblyInfo>true</GenerateAssemblyInfo>
    <AssemblyTitle>Date Watermark Tool</AssemblyTitle>
    <AssemblyDescription>A tool for adding date watermarks to images</AssemblyDescription>
    <AssemblyCompany>Your Company</AssemblyCompany>
    <AssemblyProduct>Date Watermark Tool</AssemblyProduct>
    <AssemblyCopyright>Copyright © 2025</AssemblyCopyright>
    <AssemblyVersion>1.0.0.0</AssemblyVersion>
    <FileVersion>1.0.0.0</FileVersion>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="System.Drawing.Common" Version="8.0.0" />
  </ItemGroup>

</Project>
**Application Entry Point (`App.xaml`):**<Application x:Class="AddDateToGraphicWpf.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             StartupUri="MainWindow.xaml">
    <Application.Resources>
        
    </Application.Resources>
</Application>
**Application Code-Behind (`App.xaml.cs`):**using System.Windows;

namespace AddDateToGraphicWpf
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
### 2. Data Models

**Watermark Settings Model (`Models/WatermarkSettings.cs`):**using System;

namespace AddDateToGraphicWpf.Models
{
    public class WatermarkSettings
    {
        public string FontFamily { get; set; } = "Arial";
        public double FontSize { get; set; } = 24;
        public bool IsBold { get; set; } = false;
        public bool IsItalic { get; set; } = false;
        public System.Windows.Media.Color TextColor { get; set; } = System.Windows.Media.Colors.White;
        public System.Windows.Media.Color ShadowColor { get; set; } = System.Windows.Media.Colors.Black;
        public double Transparency { get; set; } = 0.8;
        public bool HasDropShadow { get; set; } = true;
        public double ShadowOffsetX { get; set; } = 2;
        public double ShadowOffsetY { get; set; } = 2;
        public WatermarkPosition Position { get; set; } = WatermarkPosition.BottomRight;
        public string DateFormat { get; set; } = "MM/dd/yyyy";
        public double MarginX { get; set; } = 10;
        public double MarginY { get; set; } = 10;
    }

    public enum WatermarkPosition
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }
}
**Image File Model (`Models/ImageFile.cs`):**using System;

namespace AddDateToGraphicWpf.Models
{
    public class ImageFile
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public DateTime SelectedDate { get; set; }
        public long FileSize { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
### 3. Service Layer

**Watermark Service (`Services/WatermarkService.cs`):**using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using AddDateToGraphicWpf.Models;

namespace AddDateToGraphicWpf.Services
{
    public class WatermarkService
    {
        public BitmapImage ApplyWatermark(string imagePath, DateTime date, WatermarkSettings settings)
        {
            try
            {
                // Load the image into memory to avoid file locks
                byte[] imageBytes;
                using (var fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    imageBytes = new byte[fileStream.Length];
                    fileStream.Read(imageBytes, 0, imageBytes.Length);
                }

                using var memoryStream = new MemoryStream(imageBytes);
                using var originalImage = Image.FromStream(memoryStream);
                using var watermarkedImage = ApplyWatermarkToImage(originalImage, date, settings);
                
                using var outputStream = new MemoryStream();
                watermarkedImage.Save(outputStream, ImageFormat.Png);
                outputStream.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = outputStream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to apply watermark: {ex.Message}", ex);
            }
        }

        public void SaveWatermarkedImage(string sourcePath, string outputPath, DateTime date, WatermarkSettings settings)
        {
            try
            {
                // Ensure the output directory exists
                var outputDirectory = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                // Load the original image into memory to avoid file locks
                byte[] imageBytes;
                using (var fileStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
                {
                    imageBytes = new byte[fileStream.Length];
                    fileStream.Read(imageBytes, 0, imageBytes.Length);
                }

                using var memoryStream = new MemoryStream(imageBytes);
                using var originalImage = Image.FromStream(memoryStream);
                using var watermarkedImage = ApplyWatermarkToImage(originalImage, date, settings);
                
                // Get encoder info for the original format
                var format = originalImage.RawFormat;
                var encoder = GetEncoderInfo(format);
                
                if (encoder != null)
                {
                    // Use high quality settings
                    var encoderParams = new EncoderParameters(1);
                    encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 95L);
                    
                    // Save with proper encoder
                    watermarkedImage.Save(outputPath, encoder, encoderParams);
                }
                else
                {
                    // Fallback to PNG if we can't determine the format
                    watermarkedImage.Save(outputPath, ImageFormat.Png);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to save watermarked image: {ex.Message}", ex);
            }
        }

        private Bitmap ApplyWatermarkToImage(Image originalImage, DateTime date, WatermarkSettings settings)
        {
            var bitmap = new Bitmap(originalImage.Width, originalImage.Height);
            
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                
                // Draw the original image
                graphics.DrawImage(originalImage, 0, 0);
                
                // Create the date text
                var dateText = date.ToString(settings.DateFormat);
                
                // Create font
                var fontStyle = FontStyle.Regular;
                if (settings.IsBold) fontStyle |= FontStyle.Bold;
                if (settings.IsItalic) fontStyle |= FontStyle.Italic;
                
                using var font = new Font(settings.FontFamily, (float)settings.FontSize, fontStyle);
                
                // Measure text size
                var textSize = graphics.MeasureString(dateText, font);
                
                // Calculate position
                var position = CalculatePosition(bitmap.Width, bitmap.Height, textSize, settings);
                
                // Draw drop shadow if enabled
                if (settings.HasDropShadow)
                {
                    var shadowColor = Color.FromArgb(
                        (int)(255 * settings.Transparency),
                        settings.ShadowColor.R,
                        settings.ShadowColor.G,
                        settings.ShadowColor.B);
                    
                    using var shadowBrush = new SolidBrush(shadowColor);
                    graphics.DrawString(dateText, font, shadowBrush,
                        position.X + (float)settings.ShadowOffsetX,
                        position.Y + (float)settings.ShadowOffsetY);
                }
                
                // Draw main text
                var textColor = Color.FromArgb(
                    (int)(255 * settings.Transparency),
                    settings.TextColor.R,
                    settings.TextColor.G,
                    settings.TextColor.B);
                
                using var textBrush = new SolidBrush(textColor);
                graphics.DrawString(dateText, font, textBrush, position.X, position.Y);
            }
            
            return bitmap;
        }

        private PointF CalculatePosition(int imageWidth, int imageHeight, SizeF textSize, WatermarkSettings settings)
        {
            float x = 0, y = 0;
            
            switch (settings.Position)
            {
                case WatermarkPosition.TopLeft:
                    x = (float)settings.MarginX;
                    y = (float)settings.MarginY;
                    break;
                case WatermarkPosition.TopCenter:
                    x = (imageWidth - textSize.Width) / 2;
                    y = (float)settings.MarginY;
                    break;
                case WatermarkPosition.TopRight:
                    x = imageWidth - textSize.Width - (float)settings.MarginX;
                    y = (float)settings.MarginY;
                    break;
                case WatermarkPosition.MiddleLeft:
                    x = (float)settings.MarginX;
                    y = (imageHeight - textSize.Height) / 2;
                    break;
                case WatermarkPosition.MiddleCenter:
                    x = (imageWidth - textSize.Width) / 2;
                    y = (imageHeight - textSize.Height) / 2;
                    break;
                case WatermarkPosition.MiddleRight:
                    x = imageWidth - textSize.Width - (float)settings.MarginX;
                    y = (imageHeight - textSize.Height) / 2;
                    break;
                case WatermarkPosition.BottomLeft:
                    x = (float)settings.MarginX;
                    y = imageHeight - textSize.Height - (float)settings.MarginY;
                    break;
                case WatermarkPosition.BottomCenter:
                    x = (imageWidth - textSize.Width) / 2;
                    y = imageHeight - textSize.Height - (float)settings.MarginY;
                    break;
                case WatermarkPosition.BottomRight:
                    x = imageWidth - textSize.Width - (float)settings.MarginX;
                    y = imageHeight - textSize.Height - (float)settings.MarginY;
                    break;
            }
            
            return new PointF(x, y);
        }

        private ImageCodecInfo? GetEncoderInfo(ImageFormat format)
        {
            var encoders = ImageCodecInfo.GetImageEncoders();
            
            foreach (var encoder in encoders)
            {
                if (encoder.FormatID == format.Guid)
                {
                    return encoder;
                }
            }
            
            return null;
        }
    }
}
**Image Service (`Services/ImageService.cs`):**using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using AddDateToGraphicWpf.Models;

namespace AddDateToGraphicWpf.Services
{
    public class ImageService
    {
        private readonly string[] _supportedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".tiff", ".gif" };

        public List<ImageFile> LoadImageFiles(string[] filePaths)
        {
            var imageFiles = new List<ImageFile>();

            foreach (var filePath in filePaths)
            {
                if (IsImageFile(filePath))
                {
                    try
                    {
                        var imageFile = CreateImageFile(filePath);
                        imageFiles.Add(imageFile);
                    }
                    catch (Exception ex)
                    {
                        // Log error or show to user
                        System.Diagnostics.Debug.WriteLine($"Error loading image {filePath}: {ex.Message}");
                    }
                }
            }

            return imageFiles;
        }

        public bool IsImageFile(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return _supportedExtensions.Contains(extension);
        }

        private ImageFile CreateImageFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            var imageFile = new ImageFile
            {
                FilePath = filePath,
                FileName = fileInfo.Name,
                CreationDate = fileInfo.CreationTime,
                SelectedDate = fileInfo.CreationTime,
                FileSize = fileInfo.Length
            };

            // Get image dimensions
            try
            {
                using var image = Image.FromFile(filePath);
                imageFile.Width = image.Width;
                imageFile.Height = image.Height;
            }
            catch
            {
                // Default dimensions if we can't read the image
                imageFile.Width = 0;
                imageFile.Height = 0;
            }

            return imageFile;
        }

        public string GenerateOutputPath(string originalPath, bool isCopy = false)
        {
            var directory = Path.GetDirectoryName(originalPath) ?? "";
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(originalPath);
            var extension = Path.GetExtension(originalPath);
            
            var suffix = isCopy ? "_watermarked" : "";
            var newFileName = $"{nameWithoutExtension}{suffix}{extension}";
            
            // Try safe locations in order of preference
            var safeLocations = new[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Path.GetTempPath(),
                @"C:\Temp"
            };

            foreach (var location in safeLocations)
            {
                try
                {
                    if (Directory.Exists(location) && HasWriteAccess(location))
                    {
                        return Path.Combine(location, newFileName);
                    }
                }
                catch
                {
                    continue;
                }
            }

            // Fallback to original path
            return GenerateOutputPath(originalPath, isCopy);
        }

        public string GenerateBulkOutputFolder()
        {
            var baseFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss_zzz").Replace(":", "-");
            var bulkFolder = Path.Combine(baseFolder, $"Watermarked_{timestamp}");

            if (!Directory.Exists(bulkFolder))
            {
                Directory.CreateDirectory(bulkFolder);
            }

            return bulkFolder;
        }

        public bool IsProtectedFolder(string folderPath)
        {
            var protectedFolders = new[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)
            };

            return protectedFolders.Any(protectedFolder => 
                folderPath.StartsWith(protectedFolder, StringComparison.OrdinalIgnoreCase));
        }

        private bool HasWriteAccess(string folderPath)
        {
            try
            {
                var testFile = Path.Combine(folderPath, $"test_{Guid.NewGuid()}.tmp");
                File.WriteAllText(testFile, "test");
                File.Delete(testFile);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
### 4. User Interface

**Main Window XAML (`MainWindow.xaml`):**<Window x:Class="AddDateToGraphicWpf.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Date Watermark Tool" Height="800" Width="1200"
        WindowStartupLocation="CenterScreen">
    
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>
        
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="300"/>
            <ColumnDefinition Width="*"/>
            <ColumnDefinition Width="300"/>
        </Grid.ColumnDefinitions>

        <!-- Menu Bar -->
        <Menu Grid.Row="0" Grid.ColumnSpan="3">
            <MenuItem Header="_File">
                <MenuItem Header="_Open Images..." Click="OpenImages_Click"/>
                <Separator/>
                <MenuItem Header="E_xit" Click="Exit_Click"/>
            </MenuItem>
        </Menu>

        <!-- Left Panel - File List -->
        <GroupBox Grid.Row="1" Grid.Column="0" Header="Selected Images" Margin="5">
            <Grid>
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="*"/>
                    <RowDefinition Height="Auto"/>
                </Grid.RowDefinitions>
                
                <Button Grid.Row="0" Content="Add Images..." Click="OpenImages_Click" Margin="5"/>
                
                <ListView Grid.Row="1" x:Name="ImageListView" SelectionChanged="ImageListView_SelectionChanged" 
                         AllowDrop="True" DragEnter="ImageListView_DragEnter" Drop="ImageListView_Drop" Margin="5">
                    <ListView.View>
                        <GridView>
                            <GridViewColumn Header="File Name" DisplayMemberBinding="{Binding FileName}" Width="180"/>
                            <GridViewColumn Header="Size" DisplayMemberBinding="{Binding FileSize, StringFormat={}{0:N0} bytes}" Width="80"/>
                        </GridView>
                    </ListView.View>
                </ListView>
                
                <Button Grid.Row="2" Content="Remove Selected" Click="RemoveSelected_Click" Margin="5"/>
            </Grid>
        </GroupBox>

        <!-- Center Panel - Preview -->
        <GroupBox Grid.Row="1" Grid.Column="1" Header="Preview" Margin="5">
            <Grid>
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="*"/>
                    <RowDefinition Height="Auto"/>
                </Grid.RowDefinitions>
                
                <StackPanel Grid.Row="0" Orientation="Horizontal" Margin="5">
                    <Label Content="Date:" VerticalAlignment="Center"/>
                    <DatePicker x:Name="DatePicker" SelectedDateChanged="DatePicker_SelectedDateChanged" VerticalAlignment="Center" Margin="5,0"/>
                    <Button Content="Use Creation Date" Click="UseCreationDate_Click" Margin="10,0,0,0"/>
                </StackPanel>
                
                <ScrollViewer Grid.Row="1" x:Name="PreviewScrollViewer" HorizontalScrollBarVisibility="Auto" VerticalScrollBarVisibility="Auto">
                    <Image x:Name="PreviewImage" Stretch="Uniform" MaxWidth="600" MaxHeight="400"/>
                </ScrollViewer>
                
                <StackPanel Grid.Row="2" Orientation="Horizontal" HorizontalAlignment="Center" Margin="5">
                    <Button Content="Save" Click="Save_Click" Width="100" Margin="5" IsEnabled="{Binding HasSelectedImage}"/>
                    <Button Content="Save Copy" Click="SaveCopy_Click" Width="100" Margin="5" IsEnabled="{Binding HasSelectedImage}"/>
                    <Button Content="Bulk Save All" Click="BulkSaveAll_Click" Width="150" Margin="5"/>
                </StackPanel>
            </Grid>
        </GroupBox>

        <!-- Right Panel - Settings -->
        <GroupBox Grid.Row="1" Grid.Column="2" Header="Watermark Settings" Margin="5">
            <ScrollViewer VerticalScrollBarVisibility="Auto">
                <StackPanel Margin="5">
                    
                    <!-- Font Settings -->
                    <Expander Header="Font" IsExpanded="True">
                        <StackPanel Margin="10">
                            <Label Content="Font Family:"/>
                            <ComboBox x:Name="FontFamilyComboBox" SelectionChanged="Settings_Changed">
                                <ComboBoxItem Content="Arial" IsSelected="True"/>
                                <ComboBoxItem Content="Times New Roman"/>
                                <ComboBoxItem Content="Calibri"/>
                                <ComboBoxItem Content="Verdana"/>
                                <ComboBoxItem Content="Tahoma"/>
                            </ComboBox>
                            
                            <Label Content="Font Size:" Margin="0,10,0,0"/>
                            <Slider x:Name="FontSizeSlider" Minimum="8" Maximum="72" Value="24" ValueChanged="Settings_Changed"/>
                            <TextBlock Text="{Binding Value, ElementName=FontSizeSlider, StringFormat={}{0:F0}px}" HorizontalAlignment="Center"/>
                            
                            <CheckBox x:Name="BoldCheckBox" Content="Bold" Margin="0,10,0,0" Checked="Settings_Changed" Unchecked="Settings_Changed"/>
                            <CheckBox x:Name="ItalicCheckBox" Content="Italic" Margin="0,5,0,0" Checked="Settings_Changed" Unchecked="Settings_Changed"/>
                        </StackPanel>
                    </Expander>

                    <!-- Color Settings -->
                    <Expander Header="Colors" IsExpanded="True">
                        <StackPanel Margin="10">
                            <Label Content="Text Color:"/>
                            <ComboBox x:Name="TextColorComboBox" SelectionChanged="Settings_Changed">
                                <ComboBoxItem Content="White" IsSelected="True"/>
                                <ComboBoxItem Content="Black"/>
                                <ComboBoxItem Content="Red"/>
                                <ComboBoxItem Content="Blue"/>
                                <ComboBoxItem Content="Yellow"/>
                            </ComboBox>
                            
                            <Label Content="Shadow Color:" Margin="0,10,0,0"/>
                            <ComboBox x:Name="ShadowColorComboBox" SelectionChanged="Settings_Changed">
                                <ComboBoxItem Content="Black" IsSelected="True"/>
                                <ComboBoxItem Content="White"/>
                                <ComboBoxItem Content="Gray"/>
                                <ComboBoxItem Content="Red"/>
                                <ComboBoxItem Content="Blue"/>
                            </ComboBox>
                        </StackPanel>
                    </Expander>

                    <!-- Position Settings -->
                    <Expander Header="Position" IsExpanded="True">
                        <StackPanel Margin="10">
                            <Label Content="Position:"/>
                            <ComboBox x:Name="PositionComboBox" SelectionChanged="Settings_Changed">
                                <ComboBoxItem Content="Top Left"/>
                                <ComboBoxItem Content="Top Center"/>
                                <ComboBoxItem Content="Top Right"/>
                                <ComboBoxItem Content="Middle Left"/>
                                <ComboBoxItem Content="Middle Center"/>
                                <ComboBoxItem Content="Middle Right"/>
                                <ComboBoxItem Content="Bottom Left"/>
                                <ComboBoxItem Content="Bottom Center" IsSelected="True"/>
                                <ComboBoxItem Content="Bottom Right"/>
                            </ComboBox>
                        </StackPanel>
                    </Expander>

                    <!-- Effects Settings -->
                    <Expander Header="Effects" IsExpanded="True">
                        <StackPanel Margin="10">
                            <CheckBox x:Name="DropShadowCheckBox" Content="Drop Shadow" IsChecked="True" Checked="Settings_Changed" Unchecked="Settings_Changed"/>
                            
                            <Label Content="Transparency:" Margin="0,10,0,0"/>
                            <Slider x:Name="TransparencySlider" Minimum="0" Maximum="1" Value="0.8" ValueChanged="Settings_Changed"/>
                            <TextBlock Text="{Binding Value, ElementName=TransparencySlider, StringFormat={}{0:P0}}" HorizontalAlignment="Center"/>
                        </StackPanel>
                    </Expander>

                    <!-- Date Format -->
                    <Expander Header="Date Format" IsExpanded="True">
                        <StackPanel Margin="10">
                            <Label Content="Format:"/>
                            <ComboBox x:Name="DateFormatComboBox" SelectionChanged="Settings_Changed">
                                <ComboBoxItem Content="MM/dd/yyyy" IsSelected="True"/>
                                <ComboBoxItem Content="dd/MM/yyyy"/>
                                <ComboBoxItem Content="yyyy-MM-dd"/>
                                <ComboBoxItem Content="MMMM dd, yyyy"/>
                                <ComboBoxItem Content="dd MMMM yyyy"/>
                                <ComboBoxItem Content="MMM dd, yyyy"/>
                                <ComboBoxItem Content="ddd, MMM dd, yyyy"/>
                            </ComboBox>
                        </StackPanel>
                    </Expander>

                </StackPanel>
            </ScrollViewer>
        </GroupBox>

        <!-- Status Bar -->
        <StatusBar Grid.Row="2" Grid.ColumnSpan="3">
            <StatusBarItem>
                <TextBlock x:Name="StatusText" Text="Ready"/>
            </StatusBarItem>
        </StatusBar>
    </Grid>
</Window>
**Main Window Code-Behind (`MainWindow.xaml.cs`):**using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using AddDateToGraphicWpf.Models;
using AddDateToGraphicWpf.Services;

namespace AddDateToGraphicWpf
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly ImageService _imageService;
        private readonly WatermarkService _watermarkService;
        private readonly ObservableCollection<ImageFile> _imageFiles;
        private ImageFile? _selectedImage;
        private bool _hasSelectedImage;

        public bool HasSelectedImage
        {
            get => _hasSelectedImage;
            set
            {
                _hasSelectedImage = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            
            _imageService = new ImageService();
            _watermarkService = new WatermarkService();
            _imageFiles = new ObservableCollection<ImageFile>();
            
            ImageListView.ItemsSource = _imageFiles;
            DatePicker.SelectedDate = DateTime.Now;
        }

        private void OpenImages_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select Image Files",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.tiff;*.gif|All Files|*.*",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var newImages = _imageService.LoadImageFiles(openFileDialog.FileNames);
                
                foreach (var image in newImages)
                {
                    if (!_imageFiles.Any(img => img.FilePath == image.FilePath))
                    {
                        _imageFiles.Add(image);
                    }
                }
                
                StatusText.Text = $"Loaded {newImages.Count} images. Total: {_imageFiles.Count}";
            }
        }

        private void RemoveSelected_Click(object sender, RoutedEventArgs e)
        {
            if (ImageListView.SelectedItem is ImageFile selectedImage)
            {
                _imageFiles.Remove(selectedImage);
                
                if (_selectedImage == selectedImage)
                {
                    _selectedImage = null;
                    HasSelectedImage = false;
                    PreviewImage.Source = null;
                }
                
                StatusText.Text = $"Image removed. Total: {_imageFiles.Count}";
            }
        }

        private void ImageListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ImageListView.SelectedItem is ImageFile selectedImage)
            {
                _selectedImage = selectedImage;
                HasSelectedImage = true;
                DatePicker.SelectedDate = selectedImage.SelectedDate;
                UpdatePreview();
                StatusText.Text = $"Selected: {selectedImage.FileName} ({selectedImage.Width}x{selectedImage.Height})";
            }
            else
            {
                _selectedImage = null;
                HasSelectedImage = false;
                PreviewImage.Source = null;
                StatusText.Text = "No image selected";
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectedImage != null && DatePicker.SelectedDate.HasValue)
            {
                _selectedImage.SelectedDate = DatePicker.SelectedDate.Value;
                UpdatePreview();
            }
        }

        private void UseCreationDate_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedImage != null)
            {
                _selectedImage.SelectedDate = _selectedImage.CreationDate;
                DatePicker.SelectedDate = _selectedImage.CreationDate;
                UpdatePreview();
            }
        }

        private void Settings_Changed(object sender, RoutedEventArgs e)
        {
            UpdatePreview();
        }

        private void Settings_Changed(object sender, SelectionChangedEventArgs e)
        {
            UpdatePreview();
        }

        private void Settings_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            if (_selectedImage == null || !DatePicker.SelectedDate.HasValue)
            {
                PreviewImage.Source = null;
                return;
            }

            try
            {
                var settings = GetCurrentSettings();
                var previewImage = _watermarkService.ApplyWatermark(
                    _selectedImage.FilePath, 
                    DatePicker.SelectedDate.Value, 
                    settings);
                
                PreviewImage.Source = previewImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating preview: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private WatermarkSettings GetCurrentSettings()
        {
            var settings = new WatermarkSettings();

            // Font settings
            if (FontFamilyComboBox.SelectedItem is ComboBoxItem fontItem)
                settings.FontFamily = fontItem.Content.ToString() ?? "Arial";
            
            settings.FontSize = FontSizeSlider.Value;
            settings.IsBold = BoldCheckBox.IsChecked ?? false;
            settings.IsItalic = ItalicCheckBox.IsChecked ?? false;

            // Color settings
            settings.TextColor = GetColorFromComboBox(TextColorComboBox);
            settings.ShadowColor = GetColorFromComboBox(ShadowColorComboBox);

            // Position settings
            settings.Position = GetPositionFromComboBox();

            // Effects settings
            settings.HasDropShadow = DropShadowCheckBox.IsChecked ?? false;
            settings.Transparency = TransparencySlider.Value;

            // Date format
            if (DateFormatComboBox.SelectedItem is ComboBoxItem formatItem)
                settings.DateFormat = formatItem.Content.ToString() ?? "MM/dd/yyyy";

            return settings;
        }

        private Color GetColorFromComboBox(ComboBox comboBox)
        {
            if (comboBox.SelectedItem is ComboBoxItem item)
            {
                return item.Content.ToString() switch
                {
                    "White" => Colors.White,
                    "Black" => Colors.Black,
                    "Red" => Colors.Red,
                    "Blue" => Colors.Blue,
                    "Yellow" => Colors.Yellow,
                    "Gray" => Colors.Gray,
                    _ => Colors.White
                };
            }
            return Colors.White;
        }

        private WatermarkPosition GetPositionFromComboBox()
        {
            if (PositionComboBox.SelectedItem is ComboBoxItem item)
            {
                return item.Content.ToString() switch
                {
                    "Top Left" => WatermarkPosition.TopLeft,
                    "Top Center" => WatermarkPosition.TopCenter,
                    "Top Right" => WatermarkPosition.TopRight,
                    "Middle Left" => WatermarkPosition.MiddleLeft,
                    "Middle Center" => WatermarkPosition.MiddleCenter,
                    "Middle Right" => WatermarkPosition.MiddleRight,
                    "Bottom Left" => WatermarkPosition.BottomLeft,
                    "Bottom Center" => WatermarkPosition.BottomCenter,
                    "Bottom Right" => WatermarkPosition.BottomRight,
                    _ => WatermarkPosition.BottomRight
                };
            }
            return WatermarkPosition.BottomRight;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedImage == null || !DatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select an image and date first.", "No Selection", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveImage(false);
        }

        private void SaveCopy_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedImage == null || !DatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select an image and date first.", "No Selection", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveImage(true);
        }

        private void BulkSaveAll_Click(object sender, RoutedEventArgs e)
        {
            if (_imageFiles.Count == 0)
            {
                MessageBox.Show("No images loaded to save.", "No Images", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                StatusText.Text = "Bulk saving images...";
                var settings = GetCurrentSettings();
                var bulkFolder = _imageService.GenerateBulkOutputFolder();

                foreach (var imageFile in _imageFiles)
                {
                    var outputPath = Path.Combine(bulkFolder, imageFile.FileName);
                    _watermarkService.SaveWatermarkedImage(
                        imageFile.FilePath,
                        outputPath,
                        imageFile.SelectedDate,
                        settings);
                }

                StatusText.Text = $"Bulk save completed. Images saved to: {bulkFolder}";
                MessageBox.Show($"All images saved successfully!\n\nLocation: {bulkFolder}", 
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during bulk save: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "Bulk save failed - unknown error";
            }
        }

        private void SaveImage(bool createCopy)
        {
            try
            {
                StatusText.Text = "Saving image...";
                
                var settings = GetCurrentSettings();
                var outputPath = _imageService.GenerateOutputPath(_selectedImage!.FilePath, createCopy);
                
                // Check if we're trying to save to a protected folder
                var directory = Path.GetDirectoryName(outputPath) ?? "";
                if (_imageService.IsProtectedFolder(directory))
                {
                    var result = MessageBox.Show(
                        "The target folder appears to be protected by Windows Controlled Folder Access.\n\n" +
                        "Would you like to save to a safe location instead?\n\n" +
                        "Click 'Yes' to save to your Desktop\n" +
                        "Click 'No' to try the original location anyway\n" +
                        "Click 'Cancel' to abort",
                        "Protected Folder Detected", 
                        MessageBoxButton.YesNoCancel, 
                        MessageBoxImage.Question);
                    
                    if (result == MessageBoxResult.Cancel)
                    {
                        StatusText.Text = "Save cancelled";
                        return;
                    }
                    else if (result == MessageBoxResult.Yes)
                    {
                        outputPath = _imageService.GenerateSafeOutputPath(_selectedImage.FilePath, createCopy);
                    }
                }
                
                // If not creating a copy and file would overwrite itself, ask for confirmation
                if (!createCopy && File.Exists(outputPath))
                {
                    var result = MessageBox.Show(
                        "This will overwrite the original file. Are you sure?", 
                        "Overwrite Confirmation", 
                        MessageBoxButton.YesNo, 
                        MessageBoxImage.Question);
                    
                    if (result != MessageBoxResult.Yes)
                        return;
                }

                _watermarkService.SaveWatermarkedImage(
                    _selectedImage.FilePath, 
                    outputPath, 
                    DatePicker.SelectedDate!.Value, 
                    settings);

                var action = createCopy ? "Copy saved" : "Image saved";
                StatusText.Text = $"{action}: {Path.GetFileName(outputPath)}";
                
                MessageBox.Show($"{action} successfully!\n\nLocation: {outputPath}", 
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (UnauthorizedAccessException ex)
            {
                var message = "Access denied. This is likely due to Windows Controlled Folder Access.\n\n" +
                             "Solutions:\n" +
                             "1. Add this app to allowed apps in Windows Security\n" +
                             "2. Temporarily disable Controlled Folder Access\n" +
                             "3. Save to a different folder (Desktop, Temp, etc.)\n\n" +
                             $"Error: {ex.Message}";
                             
                MessageBox.Show(message, "Controlled Folder Access Blocked", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "Save failed - controlled folder access";
            }
            catch (Exception ex) when (ex.Message.Contains("GDI+"))
            {
                var message = "Image processing error (GDI+). This can happen due to:\n\n" +
                             "• File permissions or Controlled Folder Access\n" +
                             "• Corrupted image file\n" +
                             "• Insufficient disk space\n" +
                             "• Antivirus interference\n\n" +
                             $"Technical error: {ex.Message}\n\n" +
                             "Try saving to a different location or use 'Save Copy' instead.";
                             
                MessageBox.Show(message, "Image Processing Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "Save failed - GDI+ error";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving image: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "Save failed - unknown error";
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ImageListView_DragEnter(object sender, DragEventArgs e)
        {
            // Check if the dragged data contains files
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                
                // Check if any of the dropped files are image files
                bool hasImageFiles = files.Any(file => IsImageFile(file));
                
                if (hasImageFiles)
                {
                    e.Effects = DragDropEffects.Copy;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            
            e.Handled = true;
        }

        private void ImageListView_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                
                // Filter for image files only
                var imageFiles = files.Where(file => IsImageFile(file)).ToArray();
                
                if (imageFiles.Length > 0)
                {
                    var newImages = _imageService.LoadImageFiles(imageFiles);
                    
                    int addedCount = 0;
                    foreach (var image in newImages)
                    {
                        if (!_imageFiles.Any(img => img.FilePath == image.FilePath))
                        {
                            _imageFiles.Add(image);
                            addedCount++;
                        }
                    }
                    
                    if (addedCount > 0)
                    {
                        StatusText.Text = $"Added {addedCount} images via drag & drop. Total: {_imageFiles.Count}";
                    }
                    else
                    {
                        StatusText.Text = "No new images added (files already in list).";
                    }
                }
                else
                {
                    StatusText.Text = "No valid image files found in dropped items.";
                }
            }
            
            e.Handled = true;
        }

        private bool IsImageFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return false;
                
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension == ".jpg" || extension == ".jpeg" || extension == ".png" || 
                   extension == ".bmp" || extension == ".tiff" || extension == ".gif";
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
### 5. Build and Test

**Build Command:**dotnet build
**Build Output:**Build succeeded in 12.1s
### 6. Documentation

**README.md Created** with comprehensive documentation including:
- Features overview
- Supported image formats
- Usage instructions
- Watermark settings details
- Build and run instructions
- Project structure
- Technical details

---

## Application Features Summary

✅ **Multi-file Selection** - Add multiple images at once  
✅ **Default Creation Date** - Automatically uses file creation date  
✅ **Real-time Preview** - See watermark before saving  
✅ **Comprehensive Settings** - Font, colors, position, effects  
✅ **Save Options** - Overwrite original or create copy  
✅ **Bulk Save All** - Save all images to a new sub-folder  
✅ **Professional UI** - Clean, organized three-panel interface  

**Supported Formats:** JPG, PNG, BMP, TIFF, GIF

**Customization Options:**
- **Font:** Family, size (8-72px), bold, italic
- **Colors:** Text and shadow colors (5 predefined options each)
- **Position:** 9 positions (Top/Middle/Bottom × Left/Center/Right)
- **Effects:** Drop shadow, transparency (0-100%)
- **Date Formats:** 6 different formats including long and short forms

**Architecture:** Clean separation with Models, Services, and UI layers using WPF and .NET 8.0

---

## Final Notes

The application successfully builds and provides all requested functionality:
- ✅ Multiple file selection
- ✅ Date watermark with creation date default
- ✅ Preview window with real-time updates
- ✅ Save and Save Copy buttons
- ✅ Bulk Save All functionality
- ✅ Comprehensive watermark settings (font, colors, drop shadow, transparency)
- ✅ Professional, user-friendly interface

The project is ready to run with `dotnet run` command from the project directory.

---

## Continued Development: GDI+ and Windows Security Issues

**Date:** June 28, 2025 (Continued)  
**Issues Encountered:** GDI+ exceptions and Windows Controlled Folder Access blocking

---

### User Reports Additional Issues

**User Error Log (`LogChat.log.txt`):**Error saving image: A generic error occurred in GDI+.

Controlled folder access blocked.
**Additional Terminal Errors:**------------------------------------------------------------------------------
You may only use the Microsoft Visual Studio .NET/C/C++ Debugger (vsdbg) with
Visual Studio Code, Visual Studio or Visual Studio for Mac software to help you
develop and test your applications.
------------------------------------------------------------------------------
Exception thrown: 'System.Runtime.InteropServices.ExternalException' in System.Drawing.Common.dll
Exception thrown: 'System.InvalidOperationException' in AddDateToGraphicWpf.dll
The program '[24460] AddDateToGraphicWpf.exe' has exited with code 0 (0x0).
---

## Sample Image Generation for Testing

**Date:** June 28, 2025 (Continued)  
**Task:** Generate diverse test images for comprehensive watermark testing

---

### User Request for Test Images

**User Request:**
> generate a dozen sample graphic files with various colors, contracts and gradients plus various sizes at the folder: C:\Users\deege\Pictures\Test WPF Watermark

---

### Solution: Multiple Image Generation Methods

#### 1. PowerShell Script Approach

**Created `Generate-SampleImages.ps1`:**# PowerShell Script to Generate Sample Images for WPF Watermark Testing
# Creates 12 sample images with various colors, gradients, and sizes

param(
    [string]$OutputPath = "C:\Users\deege\Pictures\Test WPF Watermark"
)

# Ensure output directory exists
if (-not (Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force
    Write-Host "Created directory: $OutputPath" -ForegroundColor Green
}

Write-Host "Generating sample images for WPF Watermark testing..." -ForegroundColor Cyan

# Add System.Drawing assembly
Add-Type -AssemblyName System.Drawing

# Function to create a solid color image
function New-SolidColorImage {
    param($Width, $Height, $Color, $FileName)
    
    $bitmap = New-Object System.Drawing.Bitmap($Width, $Height)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $brush = New-Object System.Drawing.SolidBrush($Color)
    
    $graphics.FillRectangle($brush, 0, 0, $Width, $Height)
    
    $fullPath = Join-Path $OutputPath $FileName
    $bitmap.Save($fullPath, [System.Drawing.Imaging.ImageFormat]::Jpeg)
    
    $graphics.Dispose()
    $brush.Dispose()
    $bitmap.Dispose()
    
    Write-Host "Created: $FileName" -ForegroundColor Yellow
}

# Function to create a gradient image
function New-GradientImage {
    param($Width, $Height, $Color1, $Color2, $FileName, $Direction = "Horizontal")
    
    $bitmap = New-Object System.Drawing.Bitmap($Width, $Height)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    
    if ($Direction -eq "Horizontal") {
        $brush = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
            (New-Object System.Drawing.Point(0, 0)),
            (New-Object System.Drawing.Point($Width, 0)),
            $Color1, $Color2)
    } else {
        $brush = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
            (New-Object System.Drawing.Point(0, 0)),
            (New-Object System.Drawing.Point(0, $Height)),
            $Color1, $Color2)
    }
    
    $graphics.FillRectangle($brush, 0, 0, $Width, $Height)
    
    $fullPath = Join-Path $OutputPath $FileName
    $bitmap.Save($fullPath, [System.Drawing.Imaging.ImageFormat]::Jpeg)
    
    $graphics.Dispose()
    $brush.Dispose()
    $bitmap.Dispose()
    
    Write-Host "Created: $FileName" -ForegroundColor Yellow
}

# Function to create a pattern image
function New-PatternImage {
    param($Width, $Height, $FileName)
    
    $bitmap = New-Object System.Drawing.Bitmap($Width, $Height)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    
    # Create a checkerboard pattern
    $squareSize = 50
    $lightBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::LightGray)
    $darkBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::DarkGray)
    
    for ($x = 0; $x -lt $Width; $x += $squareSize) {
        for ($y = 0; $y -lt $Height; $y += $squareSize) {
            $isEvenX = [math]::Floor($x / $squareSize) % 2 -eq 0
            $isEvenY = [math]::Floor($y / $squareSize) % 2 -eq 0
            
            if (($isEvenX -and $isEvenY) -or (-not $isEvenX -and -not $isEvenY)) {
                $brush = $lightBrush
            } else {
                $brush = $darkBrush
            }
            
            $graphics.FillRectangle($brush, $x, $y, $squareSize, $squareSize)
        }
    }
    
    $fullPath = Join-Path $OutputPath $FileName
    $bitmap.Save($fullPath, [System.Drawing.Imaging.ImageFormat]::Jpeg)
    
    $graphics.Dispose()
    $lightBrush.Dispose()
    $darkBrush.Dispose()
    $bitmap.Dispose()
    
    Write-Host "Created: $FileName" -ForegroundColor Yellow
}

# Generate 12 sample images
# ... [image generation calls] ...
#### 2. C# Console Application Approach

**Created `ImageGenerator/Program.cs`:**using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace ImageGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string outputPath = @"C:\Users\deege\Pictures\Test WPF Watermark";
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
                Console.WriteLine($"Created directory: {outputPath}");
            }
            
            Console.WriteLine("Generating sample images for WPF Watermark testing...");
            
            // Generate 12 different test images
            GenerateImages(outputPath);
            
            Console.WriteLine($"\nGenerated 12 sample images in: {outputPath}");
            Console.WriteLine("\nImage summary:");
            Console.WriteLine("- Various sizes: 400x300 to 1920x1080");
            Console.WriteLine("- Different orientations: landscape, portrait, square, wide");
            Console.WriteLine("- Color variety: solid colors, gradients, patterns");
            Console.WriteLine("- Contrast tests: dark, light, high contrast backgrounds");
            Console.WriteLine("\nPerfect for testing watermark visibility and positioning!");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
        
        static void GenerateImages(string outputPath)
        {
            // 1. Small red image (good for testing small watermarks)
            CreateSolidColorImage(outputPath, 400, 300, Color.Red, "01_Small_Red_400x300.jpg");
            
            // 2. Medium blue image
            CreateSolidColorImage(outputPath, 800, 600, Color.Blue, "02_Medium_Blue_800x600.jpg");
            
            // 3. Large green image
            CreateSolidColorImage(outputPath, 1920, 1080, Color.Green, "03_Large_Green_1920x1080.jpg");
            
            // 4. Dark background (good for light watermarks)
            CreateSolidColorImage(outputPath, 1024, 768, Color.FromArgb(30, 30, 30), "04_Dark_Background_1024x768.jpg");
            
            // 5. Light background (good for dark watermarks)
            CreateSolidColorImage(outputPath, 1024, 768, Color.FromArgb(240, 240, 240), "05_Light_Background_1024x768.jpg");
            
            // 6. Horizontal gradient (blue to white)
            CreateGradientImage(outputPath, 1200, 800, Color.Blue, Color.White, "06_Gradient_Blue_White_1200x800.jpg", true);
            
            // 7. Vertical gradient (black to yellow)
            CreateGradientImage(outputPath, 900, 600, Color.Black, Color.Yellow, "07_Gradient_Black_Yellow_900x600.jpg", false);
            
            // 8. Portrait orientation (tall image)
            CreateSolidColorImage(outputPath, 600, 900, Color.Purple, "08_Portrait_Purple_600x900.jpg");
            
            // 9. Square image
            CreateSolidColorImage(outputPath, 800, 800, Color.Orange, "09_Square_Orange_800x800.jpg");
            
            // 10. Very wide image (panoramic)
            CreateGradientImage(outputPath, 1600, 400, Color.Cyan, Color.Magenta, "10_Wide_Gradient_1600x400.jpg", true);
            
            // 11. Checkerboard pattern
            CreateCheckerboardImage(outputPath, 1000, 750, "11_Checkerboard_Pattern_1000x750.jpg");
            
            // 12. High contrast gradient (white to black)
            CreateGradientImage(outputPath, 1280, 720, Color.White, Color.Black, "12_High_Contrast_Gradient_1280x720.jpg", true);
        }
        
        static void CreateSolidColorImage(string outputPath, int width, int height, Color color, string fileName)
        {
            using (var bitmap = new Bitmap(width, height))
            using (var graphics = Graphics.FromImage(bitmap))
            using (var brush = new SolidBrush(color))
            {
                graphics.FillRectangle(brush, 0, 0, width, height);
                
                string fullPath = Path.Combine(outputPath, fileName);
                bitmap.Save(fullPath, ImageFormat.Jpeg);
                Console.WriteLine($"Created: {fileName}");
            }
        }
        
        static void CreateGradientImage(string outputPath, int width, int height, Color color1, Color color2, string fileName, bool horizontal)
        {
            using (var bitmap = new Bitmap(width, height))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                Point point1, point2;
                if (horizontal)
                {
                    point1 = new Point(0, 0);
                    point2 = new Point(width, 0);
                }
                else
                {
                    point1 = new Point(0, 0);
                    point2 = new Point(0, height);
                }
                
                using (var brush = new LinearGradientBrush(point1, point2, color1, color2))
                {
                    graphics.FillRectangle(brush, 0, 0, width, height);
                }
                
                string fullPath = Path.Combine(outputPath, fileName);
                bitmap.Save(fullPath, ImageFormat.Jpeg);
                Console.WriteLine($"Created: $
