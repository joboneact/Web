using System;
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
    /// <summary>
    /// Main window for the AddDateToGraphicWpf application.
    /// Handles UI logic for loading images, applying watermarks, and saving results.
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // Service for loading image files and handling file-related logic.
        private readonly ImageService _imageService;
        // Service for applying and saving watermarks to images.
        private readonly WatermarkService _watermarkService;
        // Collection of images loaded into the application, bound to the UI.
        private readonly ObservableCollection<ImageFile> _imageFiles;
        // Currently selected image in the UI.
        private ImageFile? _selectedImage;
        // Tracks if an image is currently selected (for UI state).
        private bool _hasSelectedImage;

        /// <summary>
        /// Indicates if an image is selected; used for enabling/disabling UI controls.
        /// </summary>
        public bool HasSelectedImage
        {
            get => _hasSelectedImage;
            set
            {
                _hasSelectedImage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Event for property change notifications (INotifyPropertyChanged implementation).
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Initializes the main window, services, and data bindings.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            _imageService = new ImageService();
            _watermarkService = new WatermarkService();
            _imageFiles = new ObservableCollection<ImageFile>();
            ImageListView.ItemsSource = _imageFiles;

            // Set the default date in the DatePicker to the last selected date with a valid day of week, or today.
            DateTime? lastValidDate = _imageFiles
                .Select(img => (DateTime?)img.SelectedDate)
                .Reverse()
                .FirstOrDefault(d => d.HasValue && d.Value.DayOfWeek != 0);

            DatePicker.SelectedDate = lastValidDate ?? DateTime.Now;

            // Set BoldCheckBox checked by default (also set in XAML for initial state)
            BoldCheckBox.IsChecked = true;
        }

        /// <summary>
        /// Utility method to set the default date after images are loaded.
        /// </summary>
        private void SetDefaultDateFromImages()
        {
            // Find the last image with a valid SelectedDate and a real day of week.
            var lastValidDate = _imageFiles
                .Select(img => (DateTime?)img.SelectedDate)
                .Reverse()
                .FirstOrDefault(d => d.HasValue && Enum.IsDefined(typeof(DayOfWeek), d.Value.DayOfWeek));

            DatePicker.SelectedDate = lastValidDate ?? DateTime.Now;
        }

        /// <summary>
        /// Handles the "Open Images" button click.
        /// Opens a file dialog, loads selected images, and adds them to the collection.
        /// </summary>
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
                // Load image metadata for each selected file.
                var newImages = _imageService.LoadImageFiles(openFileDialog.FileNames);
                
                // Add only images not already in the collection.
                foreach (var image in newImages)
                {
                    if (!_imageFiles.Any(img => img.FilePath == image.FilePath))
                    {
                        _imageFiles.Add(image);
                    }
                }

                // Set the default date after images are loaded.
                SetDefaultDateFromImages();

                StatusText.Text = $"Loaded {newImages.Count} images. Total: {_imageFiles.Count}";
            }
        }

        /// <summary>
        /// Handles the "Remove Selected" button click.
        /// Removes the selected image from the collection and clears preview if needed.
        /// </summary>
        private void RemoveSelected_Click(object sender, RoutedEventArgs e)
        {
            if (ImageListView.SelectedItem is ImageFile selectedImage)
            {
                _imageFiles.Remove(selectedImage);
                
                // If the removed image was selected, clear selection and preview.
                if (_selectedImage == selectedImage)
                {
                    _selectedImage = null;
                    HasSelectedImage = false;
                    PreviewImage.Source = null;
                }
                
                StatusText.Text = $"Image removed. Total: {_imageFiles.Count}";
            }
        }

        /// <summary>
        /// Handles selection changes in the image list.
        /// Updates the preview and date picker to match the selected image.
        /// </summary>
        private void ImageListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ImageListView.SelectedItem is ImageFile selectedImage)
            {
                _selectedImage = selectedImage;
                HasSelectedImage = true;
                // Set the date picker to the image's selected date.
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

        /// <summary>
        /// Handles changes to the selected date in the DatePicker.
        /// Updates the selected image's date and refreshes the preview.
        /// </summary>
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectedImage != null && DatePicker.SelectedDate.HasValue)
            {
                _selectedImage.SelectedDate = DatePicker.SelectedDate.Value;
                UpdatePreview();
            }
        }

        /// <summary>
        /// Handles the "Use Creation Date" button click.
        /// Sets the date picker and image's selected date to the file's creation date.
        /// </summary>
        private void UseCreationDate_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedImage != null)
            {
                _selectedImage.SelectedDate = _selectedImage.CreationDate;
                DatePicker.SelectedDate = _selectedImage.CreationDate;
                UpdatePreview();
            }
        }

        /// <summary>
        /// Handles changes in watermark settings (checkboxes, sliders, etc.).
        /// Triggers a preview update.
        /// </summary>
        private void Settings_Changed(object sender, RoutedEventArgs e)
        {
            UpdatePreview();
        }

        /// <summary>
        /// Handles changes in watermark settings (combo boxes).
        /// Triggers a preview update.
        /// </summary>
        private void Settings_Changed(object sender, SelectionChangedEventArgs e)
        {
            UpdatePreview();
        }

        /// <summary>
        /// Handles changes in watermark settings (sliders with double values).
        /// Triggers a preview update.
        /// </summary>
        private void Settings_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdatePreview();
        }

        /// <summary>
        /// Updates the preview image with the current watermark settings and date.
        /// </summary>
        private void UpdatePreview()
        {
            // If no image is selected or no date is set, clear the preview.
            if (_selectedImage == null || !DatePicker.SelectedDate.HasValue)
            {
                PreviewImage.Source = null;
                return;
            }

            try
            {
                // Gather current settings from UI controls.
                var settings = GetCurrentSettings();
                // Generate a preview image with watermark applied.
                var previewImage = _watermarkService.ApplyWatermark(
                    _selectedImage.FilePath, 
                    DatePicker.SelectedDate.Value, 
                    settings);
                
                PreviewImage.Source = previewImage;
            }
            catch (Exception ex)
            {
                // Show error if preview generation fails.
                MessageBox.Show($"Error updating preview: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Gathers the current watermark settings from the UI controls.
        /// </summary>
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

        /// <summary>
        /// Gets a color value from a ComboBox selection.
        /// </summary>
        /// <param name="comboBox">ComboBox containing color options.</param>
        /// <returns>Selected color, or white if not found.</returns>
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
                    "Green" => Colors.Green, // Added support for Green
                    _ => Colors.White
                };
            }
            return Colors.White;
        }

        /// <summary>
        /// Gets the watermark position from the ComboBox selection.
        /// </summary>
        /// <returns>Selected WatermarkPosition, or BottomRight if not found.</returns>
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

        /// <summary>
        /// Handles the "Save" button click to overwrite the original image with the watermark.
        /// </summary>
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

        /// <summary>
        /// Handles the "Save Copy" button click to save a watermarked copy of the image.
        /// </summary>
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

        /// <summary>
        /// Saves the watermarked image, handling file access, permissions, and user prompts.
        /// </summary>
        /// <param name="createCopy">If true, saves as a copy; otherwise, overwrites original.</param>
        private void SaveImage(bool createCopy)
        {
            try
            {
                StatusText.Text = "Saving image...";
                
                var settings = GetCurrentSettings();
                var outputPath = _imageService.GenerateOutputPath(_selectedImage!.FilePath, createCopy);
                
                // Check if the output directory is protected by Windows security features.
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
                
                // Check if the output file is read-only.
                if (File.Exists(outputPath))
                {
                    var fileInfo = new FileInfo(outputPath);
                    if (fileInfo.IsReadOnly)
                    {
                        MessageBox.Show($"Cannot save to '{outputPath}' because the file is read-only.", 
                            "File Access Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        StatusText.Text = "Save failed - file is read-only";
                        return;
                    }
                }
                
                // If overwriting the original file, confirm with the user.
                if (!createCopy && outputPath.Equals(_selectedImage.FilePath, StringComparison.OrdinalIgnoreCase))
                {
                    var result = MessageBox.Show(
                        "This will overwrite the original file. Are you sure?\n\nTip: Use 'Save Copy' to create a backup with '_watermarked' suffix.", 
                        "Overwrite Confirmation", 
                        MessageBoxButton.YesNo, 
                        MessageBoxImage.Question);
                    
                    if (result != MessageBoxResult.Yes)
                    {
                        StatusText.Text = "Save cancelled";
                        return;
                    }
                }

                // Ensure the target directory exists and is writable.
                var targetDirectory = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(targetDirectory) && !Directory.Exists(targetDirectory))
                {
                    try
                    {
                        Directory.CreateDirectory(targetDirectory);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show($"Cannot create directory '{targetDirectory}'. Please check permissions or choose a different location.", 
                            "Permission Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        StatusText.Text = "Save failed - permission denied";
                        return;
                    }
                }

                // Save the watermarked image using the service.
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
                // Handle Windows Controlled Folder Access or permission issues.
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
            catch (DirectoryNotFoundException ex)
            {
                // Handle missing directory errors.
                MessageBox.Show($"Directory not found: {ex.Message}", 
                    "Directory Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "Save failed - directory not found";
            }
            catch (IOException ex) when (ex.Message.Contains("being used by another process"))
            {
                // Handle file-in-use errors.
                MessageBox.Show($"File is in use by another program: {ex.Message}\n\nPlease close any programs that might have the file open and try again.", 
                    "File In Use", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "Save failed - file in use";
            }
            catch (Exception ex) when (ex.Message.Contains("GDI+"))
            {
                // Handle GDI+ image processing errors.
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
                // Handle all other unexpected errors.
                MessageBox.Show($"Unexpected error: {ex.Message}\n\nPlease try again or contact support.", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "Save failed - unexpected error";
            }
        }

        /// <summary>
        /// Handles drag enter event for the image list to allow image file drops.
        /// Sets the drag effect if image files are detected.
        /// </summary>
        private void ImageListView_DragEnter(object sender, DragEventArgs e)
        {
            // Check if the dragged data contains files.
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                
                // Check if any of the dropped files are image files.
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

        /// <summary>
        /// Handles drop event for the image list to add dropped image files.
        /// Only valid image files are added to the collection.
        /// </summary>
        private void ImageListView_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                
                // Filter for image files only.
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

                    // Set the default date after images are loaded.
                    SetDefaultDateFromImages();

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

        /// <summary>
        /// Checks if a file path points to a supported image file.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        /// <returns>True if the file is a supported image; otherwise, false.</returns>
        private bool IsImageFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return false;
                
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension == ".jpg" || extension == ".jpeg" || extension == ".png" || 
                   extension == ".bmp" || extension == ".tiff" || extension == ".gif";
        }

        /// <summary>
        /// Handles the "Exit" menu or button click to close the application.
        /// </summary>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handles the "Bulk Save All" button click to save all images with watermarks.
        /// </summary>
        private void BulkSaveAll_Click(object sender, RoutedEventArgs e)
        {
            if (_imageFiles.Count == 0)
            {
                MessageBox.Show("No images loaded to save.", "No Images", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Create a sub-folder in the first image's directory with current date/time and time zone
            string baseDir = Path.GetDirectoryName(_imageFiles[0].FilePath) ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string tz = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now)
                ? TimeZoneInfo.Local.DaylightName
                : TimeZoneInfo.Local.StandardName;
            string safeTz = string.Concat(tz.Split(Path.GetInvalidFileNameChars()));
            string folderName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + "_" + safeTz;
            string outputDir = Path.Combine(baseDir, folderName);

            try
            {
                Directory.CreateDirectory(outputDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create output folder:\n{outputDir}\n\n{ex.Message}", "Folder Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var settings = GetCurrentSettings();
            int saved = 0, failed = 0;
            foreach (var image in _imageFiles)
            {
                try
                {
                    string outPath = Path.Combine(outputDir, Path.GetFileName(image.FilePath));
                    _watermarkService.SaveWatermarkedImage(
                        image.FilePath,
                        outPath,
                        image.SelectedDate,
                        settings
                    );
                    saved++;
                }
                catch
                {
                    failed++;
                }
            }

            StatusText.Text = $"Bulk save complete: {saved} saved, {failed} failed. Folder: {outputDir}";
            MessageBox.Show($"Bulk save complete!\n\nSaved: {saved}\nFailed: {failed}\n\nLocation:\n{outputDir}", "Bulk Save", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Notifies the UI of property changes for data binding.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

