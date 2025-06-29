using System;

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
        public WatermarkPosition Position { get; set; } = WatermarkPosition.BottomCenter;
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
