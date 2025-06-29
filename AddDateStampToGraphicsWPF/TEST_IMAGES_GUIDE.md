# Sample Test Images for WPF Watermark Application

## Generated Images Overview

The image generator creates 12 diverse test images to thoroughly test watermark functionality across different scenarios:

### 📁 Location
```
C:\Users\deege\Pictures\Test WPF Watermark\
```

## 🖼️ Image Collection

### **Size Variations**
1. **01_Small_Red_400x300.jpg** - Small format (400×300)
2. **02_Medium_Blue_800x600.jpg** - Standard format (800×600)  
3. **03_Large_Green_1920x1080.jpg** - HD format (1920×1080)
4. **10_Wide_Gradient_1600x400.jpg** - Panoramic format (1600×400)

### **Orientation Tests**
5. **08_Portrait_Purple_600x900.jpg** - Portrait orientation (tall)
6. **09_Square_Orange_800x800.jpg** - Square format (1:1 ratio)

### **Background Contrast**
7. **04_Dark_Background_1024x768.jpg** - Very dark (RGB: 30,30,30) - tests light watermarks
8. **05_Light_Background_1024x768.jpg** - Very light (RGB: 240,240,240) - tests dark watermarks
9. **12_High_Contrast_Gradient_1280x720.jpg** - White to black gradient

### **Color Variety**
10. **06_Gradient_Blue_White_1200x800.jpg** - Horizontal blue to white gradient
11. **07_Gradient_Black_Yellow_900x600.jpg** - Vertical black to yellow gradient

### **Pattern Complexity**
12. **11_Checkerboard_Pattern_1000x750.jpg** - Alternating gray checkerboard pattern

## 🧪 Testing Scenarios

### **Watermark Position Testing**
- **Small images**: Test how watermarks scale on limited space
- **Large images**: Test positioning accuracy on high resolution
- **Wide images**: Test horizontal positioning in panoramic formats
- **Portrait images**: Test vertical positioning in tall formats

### **Contrast & Visibility**
- **Dark backgrounds**: Test white/light colored watermarks with drop shadows
- **Light backgrounds**: Test dark watermarks without shadows
- **Gradients**: Test watermark visibility across color transitions
- **Patterns**: Test watermark readability over complex backgrounds

### **Font & Size Testing**
- **Various image sizes**: Test font scaling across different resolutions
- **Different orientations**: Ensure consistent appearance in all orientations

## 🚀 How to Generate

### Method 1: Batch File (Easiest)
```batch
GenerateTestImages.bat
```

### Method 2: C# Console App
```powershell
cd ImageGenerator
dotnet run
```

### Method 3: PowerShell Script
```powershell
powershell -ExecutionPolicy Bypass -File "Generate-SampleImages.ps1"
```

## 📊 Testing Checklist

Use these images to test:

### ✅ **Positioning (9 positions × 12 images = 108 tests)**
- [ ] Top Left, Top Center, Top Right
- [ ] Middle Left, Middle Center, Middle Right  
- [ ] Bottom Left, Bottom Center, Bottom Right

### ✅ **Appearance Settings**
- [ ] Font sizes (8px to 72px)
- [ ] Font families (Arial, Times New Roman, etc.)
- [ ] Bold and italic styles
- [ ] Text colors (White, Black, Red, Blue, Yellow)
- [ ] Shadow colors and transparency

### ✅ **Special Cases**
- [ ] Very small images (readability limits)
- [ ] Very large images (performance)
- [ ] Different aspect ratios
- [ ] High contrast scenarios
- [ ] Pattern interference

### ✅ **File Operations**
- [ ] Save (overwrite original)
- [ ] Save Copy (create _watermarked version)
- [ ] Different source folders
- [ ] Various file formats

## 🎯 Perfect Testing Environment

These test images provide comprehensive coverage for:
- **All common image sizes and orientations**
- **Full spectrum of background colors and contrasts** 
- **Pattern complexity variations**
- **Real-world usage scenarios**

**Result:** Confidence that your watermark will look great on any image! 🌟
