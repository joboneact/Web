using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace ImageGenerator
{
    /// <summary>
    /// Console application to generate a set of sample images with various
    /// sizes, colors, gradients, and patterns for testing watermarking in WPF apps.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Entry point of the application. Creates an output directory and generates sample images.
        /// </summary>
        /// <param name="args">Command-line arguments (not used).</param>
        static void Main(string[] args)
        {
            // Path where generated images will be saved.
            string outputPath = @"C:\Users\deege\Pictures\Test WPF Watermark";
            
            // Ensure the output directory exists; create it if it doesn't.
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
                Console.WriteLine($"Created directory: {outputPath}");
            }
            
            Console.WriteLine("Generating sample images for WPF Watermark testing...");
            
            // Generate a set of 12 sample images with different properties.
            GenerateImages(outputPath);
            
            // Print a summary of what was generated.
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
        
        /// <summary>
        /// Generates a set of sample images with different characteristics and saves them to disk.
        /// </summary>
        /// <param name="outputPath">Directory where images will be saved.</param>
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
            
            // 11. Checkerboard pattern (for contrast and pattern testing)
            CreateCheckerboardImage(outputPath, 1000, 750, "11_Checkerboard_Pattern_1000x750.jpg");
            
            // 12. High contrast gradient (white to black)
            CreateGradientImage(outputPath, 1280, 720, Color.White, Color.Black, "12_High_Contrast_Gradient_1280x720.jpg", true);
        }
        
        /// <summary>
        /// Creates and saves a solid color image of the specified size.
        /// </summary>
        /// <param name="outputPath">Directory to save the image.</param>
        /// <param name="width">Image width in pixels.</param>
        /// <param name="height">Image height in pixels.</param>
        /// <param name="color">Solid color to fill the image.</param>
        /// <param name="fileName">File name for the saved image.</param>
        static void CreateSolidColorImage(string outputPath, int width, int height, Color color, string fileName)
        {
            // Create a new bitmap with the specified dimensions.
            using (var bitmap = new Bitmap(width, height))
            // Create a graphics object to draw on the bitmap.
            using (var graphics = Graphics.FromImage(bitmap))
            // Create a solid brush with the specified color.
            using (var brush = new SolidBrush(color))
            {
                // Fill the entire bitmap with the solid color.
                graphics.FillRectangle(brush, 0, 0, width, height);
                
                // Build the full file path and save the image as JPEG.
                string fullPath = Path.Combine(outputPath, fileName);
                bitmap.Save(fullPath, ImageFormat.Jpeg);
                Console.WriteLine($"Created: {fileName}");
            }
        }
        
        /// <summary>
        /// Creates and saves a gradient image (horizontal or vertical) between two colors.
        /// </summary>
        /// <param name="outputPath">Directory to save the image.</param>
        /// <param name="width">Image width in pixels.</param>
        /// <param name="height">Image height in pixels.</param>
        /// <param name="color1">Start color of the gradient.</param>
        /// <param name="color2">End color of the gradient.</param>
        /// <param name="fileName">File name for the saved image.</param>
        /// <param name="horizontal">True for horizontal gradient, false for vertical.</param>
        static void CreateGradientImage(string outputPath, int width, int height, Color color1, Color color2, string fileName, bool horizontal)
        {
            // Create a new bitmap with the specified dimensions.
            using (var bitmap = new Bitmap(width, height))
            // Create a graphics object to draw on the bitmap.
            using (var graphics = Graphics.FromImage(bitmap))
            {
                // Define the direction of the gradient.
                Point point1, point2;
                if (horizontal)
                {
                    // Horizontal: left to right.
                    point1 = new Point(0, 0);
                    point2 = new Point(width, 0);
                }
                else
                {
                    // Vertical: top to bottom.
                    point1 = new Point(0, 0);
                    point2 = new Point(0, height);
                }
                
                // Create a linear gradient brush with the specified colors and direction.
                using (var brush = new LinearGradientBrush(point1, point2, color1, color2))
                {
                    // Fill the entire bitmap with the gradient.
                    graphics.FillRectangle(brush, 0, 0, width, height);
                }
                
                // Build the full file path and save the image as JPEG.
                string fullPath = Path.Combine(outputPath, fileName);
                bitmap.Save(fullPath, ImageFormat.Jpeg);
                Console.WriteLine($"Created: {fileName}");
            }
        }
        
        /// <summary>
        /// Creates and saves a checkerboard pattern image for contrast testing.
        /// </summary>
        /// <param name="outputPath">Directory to save the image.</param>
        /// <param name="width">Image width in pixels.</param>
        /// <param name="height">Image height in pixels.</param>
        /// <param name="fileName">File name for the saved image.</param>
        static void CreateCheckerboardImage(string outputPath, int width, int height, string fileName)
        {
            // Create a new bitmap with the specified dimensions.
            using (var bitmap = new Bitmap(width, height))
            // Create a graphics object to draw on the bitmap.
            using (var graphics = Graphics.FromImage(bitmap))
            // Create brushes for light and dark squares.
            using (var lightBrush = new SolidBrush(Color.LightGray))
            using (var darkBrush = new SolidBrush(Color.DarkGray))
            {
                int squareSize = 50; // Size of each checker square in pixels.
                
                // Loop through the image and fill squares in a checkerboard pattern.
                for (int x = 0; x < width; x += squareSize)
                {
                    for (int y = 0; y < height; y += squareSize)
                    {
                        // Determine if the current square should be light or dark.
                        bool isEvenX = (x / squareSize) % 2 == 0;
                        bool isEvenY = (y / squareSize) % 2 == 0;
                        
                        // Alternate colors to create the checkerboard effect.
                        Brush brush = ((isEvenX && isEvenY) || (!isEvenX && !isEvenY)) ? lightBrush : darkBrush;
                        graphics.FillRectangle(brush, x, y, squareSize, squareSize);
                    }
                }
                
                // Build the full file path and save the image as JPEG.
                string fullPath = Path.Combine(outputPath, fileName);
                bitmap.Save(fullPath, ImageFormat.Jpeg);
                Console.WriteLine($"Created: {fileName}");
            }
        }
    }
}
