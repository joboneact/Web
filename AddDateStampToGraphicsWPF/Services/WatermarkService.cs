using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using AddDateToGraphicWpf.Models;

namespace AddDateToGraphicWpf.Services
{
    public class WatermarkService : IDisposable
    {
        private bool _disposed = false;

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Cleanup managed resources if any
                }
                _disposed = true;
            }
        }

        ~WatermarkService()
        {
            Dispose(false);
        }
    }
}
