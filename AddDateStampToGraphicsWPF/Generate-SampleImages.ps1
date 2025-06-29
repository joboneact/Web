# PowerShell Script to Generate Sample Images for WPF Watermark Testing
# This script creates 12 sample JPEG images with various colors, gradients, patterns, and sizes.
# The images are useful for testing watermark visibility, contrast, and positioning in WPF applications.

param(
    # Output directory for generated images. Change as needed.
    [string]$OutputPath = "C:\Users\deege\Pictures\Test WPF Watermark"
)

# Ensure the output directory exists; create it if it doesn't.
if (-not (Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force
    Write-Host "Created directory: $OutputPath" -ForegroundColor Green
}

Write-Host "Generating sample images for WPF Watermark testing..." -ForegroundColor Cyan

# Load the System.Drawing assembly for image creation and manipulation.
Add-Type -AssemblyName System.Drawing

# ------------------------------------------------------------------------------
# Function: New-SolidColorImage
# Purpose : Creates a JPEG image filled with a single solid color.
# Params  : $Width, $Height - dimensions of the image
#           $Color          - System.Drawing.Color object for fill
#           $FileName       - Output file name (saved in $OutputPath)
# ------------------------------------------------------------------------------
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

# ------------------------------------------------------------------------------
# Function: New-GradientImage
# Purpose : Creates a JPEG image with a linear gradient fill.
# Params  : $Width, $Height - dimensions of the image
#           $Color1, $Color2 - Start and end System.Drawing.Color objects
#           $FileName        - Output file name (saved in $OutputPath)
#           $Direction       - "Horizontal" or "Vertical" gradient
# ------------------------------------------------------------------------------
function New-GradientImage {
    param($Width, $Height, $Color1, $Color2, $FileName, $Direction = "Horizontal")
    
    $bitmap = New-Object System.Drawing.Bitmap($Width, $Height)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    
    # Choose gradient direction
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

# ------------------------------------------------------------------------------
# Function: New-PatternImage
# Purpose : Creates a JPEG image with a checkerboard pattern.
# Params  : $Width, $Height - dimensions of the image
#           $FileName       - Output file name (saved in $OutputPath)
# ------------------------------------------------------------------------------
function New-PatternImage {
    param($Width, $Height, $FileName)
    
    $bitmap = New-Object System.Drawing.Bitmap($Width, $Height)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    
    # Checkerboard pattern settings
    $squareSize = 50
    $lightBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::LightGray)
    $darkBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::DarkGray)
    
    # Draw alternating squares
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

# ------------------------------------------------------------------------------
# Generate 12 sample images with different properties for comprehensive testing.
# ------------------------------------------------------------------------------

# 1. Small red image (good for testing small watermarks)
New-SolidColorImage -Width 400 -Height 300 -Color ([System.Drawing.Color]::Red) -FileName "01_Small_Red_400x300.jpg"

# 2. Medium blue image
New-SolidColorImage -Width 800 -Height 600 -Color ([System.Drawing.Color]::Blue) -FileName "02_Medium_Blue_800x600.jpg"

# 3. Large green image
New-SolidColorImage -Width 1920 -Height 1080 -Color ([System.Drawing.Color]::Green) -FileName "03_Large_Green_1920x1080.jpg"

# 4. Dark background (good for light watermarks)
New-SolidColorImage -Width 1024 -Height 768 -Color ([System.Drawing.Color]::FromArgb(30, 30, 30)) -FileName "04_Dark_Background_1024x768.jpg"

# 5. Light background (good for dark watermarks)
New-SolidColorImage -Width 1024 -Height 768 -Color ([System.Drawing.Color]::FromArgb(240, 240, 240)) -FileName "05_Light_Background_1024x768.jpg"

# 6. Horizontal gradient (blue to white)
New-GradientImage -Width 1200 -Height 800 -Color1 ([System.Drawing.Color]::Blue) -Color2 ([System.Drawing.Color]::White) -FileName "06_Gradient_Blue_White_1200x800.jpg" -Direction "Horizontal"

# 7. Vertical gradient (black to yellow)
New-GradientImage -Width 900 -Height 600 -Color1 ([System.Drawing.Color]::Black) -Color2 ([System.Drawing.Color]::Yellow) -FileName "07_Gradient_Black_Yellow_900x600.jpg" -Direction "Vertical"

# 8. Portrait orientation (tall image)
New-SolidColorImage -Width 600 -Height 900 -Color ([System.Drawing.Color]::Purple) -FileName "08_Portrait_Purple_600x900.jpg"

# 9. Square image
New-SolidColorImage -Width 800 -Height 800 -Color ([System.Drawing.Color]::Orange) -FileName "09_Square_Orange_800x800.jpg"

# 10. Very wide image (panoramic)
New-GradientImage -Width 1600 -Height 400 -Color1 ([System.Drawing.Color]::Cyan) -Color2 ([System.Drawing.Color]::Magenta) -FileName "10_Wide_Gradient_1600x400.jpg" -Direction "Horizontal"

# 11. Checkerboard pattern
New-PatternImage -Width 1000 -Height 750 -FileName "11_Checkerboard_Pattern_1000x750.jpg"

# 12. High contrast gradient (white to black)
New-GradientImage -Width 1280 -Height 720 -Color1 ([System.Drawing.Color]::White) -Color2 ([System.Drawing.Color]::Black) -FileName "12_High_Contrast_Gradient_1280x720.jpg" -Direction "Horizontal"

# ------------------------------------------------------------------------------
# Print summary and usage notes for the generated images.
# ------------------------------------------------------------------------------
Write-Host "`nGenerated 12 sample images in: $OutputPath" -ForegroundColor Green
Write-Host "`nImage summary:" -ForegroundColor Cyan
Write-Host "- Various sizes: 400x300 to 1920x1080" -ForegroundColor White
Write-Host "- Different orientations: landscape, portrait, square, wide" -ForegroundColor White
Write-Host "- Color variety: solid colors, gradients, patterns" -ForegroundColor White
Write-Host "- Contrast tests: dark, light, high contrast backgrounds" -ForegroundColor White
Write-Host "`nPerfect for testing watermark visibility and positioning!" -ForegroundColor Green
