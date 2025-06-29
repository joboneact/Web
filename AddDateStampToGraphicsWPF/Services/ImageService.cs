using System;
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
            
            return Path.Combine(directory, newFileName);
        }

        public string GenerateSafeOutputPath(string originalPath, bool isCopy = false)
        {
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
