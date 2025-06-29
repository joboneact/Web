using System;

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
