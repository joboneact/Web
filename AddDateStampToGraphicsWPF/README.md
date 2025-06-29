# Date Watermark Tool

A WPF application for adding date watermarks to image files with customizable settings and real-time preview.

## Features

- **Multi-file Support**: Select and process multiple image files at once
- **Date Selection**: Use the file's creation date or specify a custom date
- **Real-time Preview**: See how the watermark will look before saving
- **Customizable Watermark**:
  - Font family, size, bold, and italic options
  - Text and shadow colors
  - Position (9 different locations)
  - Drop shadow with transparency
  - Multiple date formats
- **Save Options**: Save over original or create a copy with "_watermarked" suffix

## Supported Image Formats

- JPEG (.jpg, .jpeg)
- PNG (.png)
- BMP (.bmp)
- TIFF (.tiff)
- GIF (.gif)

## How to Use

1. **Add Images**: Click "Add Images..." or use File → Open Images to select image files
2. **Select Image**: Click on an image in the list to preview it
3. **Set Date**: The creation date is used by default, or choose a custom date
4. **Customize Watermark**: Adjust font, colors, position, and effects in the settings panel
5. **Preview**: See the watermark applied in real-time in the preview window
6. **Save**: 
   - Click "Save" to overwrite the original file
   - Click "Save Copy" to create a new file with "_watermarked" suffix

## Watermark Settings

### Font
- **Font Family**: Arial, Times New Roman, Calibri, Verdana, Tahoma
- **Font Size**: 8-72 pixels (adjustable with slider)
- **Style**: Bold and/or Italic options

### Colors
- **Text Color**: White, Black, Red, Blue, Yellow
- **Shadow Color**: Black, White, Gray, Red, Blue

### Position
- Top: Left, Center, Right
- Middle: Left, Center, Right  
- Bottom: Left, Center, Right

### Effects
- **Drop Shadow**: Enable/disable with customizable offset
- **Transparency**: 0-100% (adjustable with slider)

### Date Formats
- MM/dd/yyyy (12/25/2023)
- dd/MM/yyyy (25/12/2023)
- yyyy-MM-dd (2023-12-25)
- MMMM dd, yyyy (December 25, 2023)
- dd MMMM yyyy (25 December 2023)
- MMM dd, yyyy (Dec 25, 2023)

## Building and Running

### Prerequisites
- .NET 8.0 SDK
- Windows OS (WPF application)

### Build
```powershell
dotnet build
```

### Run
```powershell
dotnet run
```

## Project Structure

```
AddDateToGraphicWpf/
├── Models/
│   ├── ImageFile.cs           # Image file data model
│   └── WatermarkSettings.cs   # Watermark configuration model
├── Services/
│   ├── ImageService.cs        # Image file operations
│   └── WatermarkService.cs    # Watermark application logic
├── MainWindow.xaml            # Main UI layout
├── MainWindow.xaml.cs         # Main UI logic
├── App.xaml                   # Application definition
├── App.xaml.cs                # Application startup
└── AddDateToGraphicWpf.csproj # Project file
```

## Technical Details

- Built with WPF (.NET 8.0)
- Uses System.Drawing for image processing
- MVVM-like architecture with services
- Real-time preview using BitmapImage conversion
- Preserves original image format when saving
