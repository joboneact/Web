# Windows Defender and Antivirus Instructions

## Windows Defender Issues

If Windows Defender flags this application, it's likely due to:

1. **Unsigned executable** - The application isn't digitally signed
2. **Image processing** - Antivirus software sometimes flags image manipulation tools
3. **New executable** - Windows Defender may be cautious about newly compiled programs

## Solutions

### Option 1: Add Exclusion
1. Open Windows Security (Windows Defender)
2. Go to **Virus & threat protection**
3. Click **Manage settings** under "Virus & threat protection settings"
4. Click **Add or remove exclusions**
5. Add the entire project folder as an exclusion

### Option 2: Temporary Disable (Not Recommended)
1. Temporarily disable real-time protection
2. Run the application
3. Re-enable protection immediately after

### Option 3: Build with Release Configuration
```powershell
dotnet build -c Release
```

### Option 4: Run from Source
Instead of the compiled executable, run directly from source:
```powershell
dotnet run
```

## GDI+ Error Solutions

The "A generic error occurred in GDI+" error has been fixed in the code with:

1. **Memory-based image loading** - Avoids file locks
2. **Proper encoder selection** - Uses original image format
3. **High-quality encoding** - 95% quality for JPEGs
4. **Better error handling** - More specific error messages
5. **Permission checking** - Validates write access before attempting save

## Common Fixes Applied

- ✅ Load images into memory to prevent file locks
- ✅ Use proper image encoders for each format
- ✅ Check file permissions before saving
- ✅ Create output directories if they don't exist
- ✅ Handle read-only files gracefully
- ✅ Provide specific error messages for different failure types

## If You Still Get Errors

1. **Run as Administrator** - Right-click executable → "Run as administrator"
2. **Check file permissions** - Ensure the target folder is writable
3. **Close other programs** - Make sure no other app has the image file open
4. **Try different location** - Save to Desktop or Documents folder
5. **Use Save Copy** - Instead of overwriting, create a new file with "_watermarked" suffix
