using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// <a href="http://blog.shibayan.jp/entry/20071013/1192204667">WPF に用意されている 7 種類のビットマップ</a>>
/// </remarks>
namespace Utilitys
{
    public class ImageSourceToFile
    {
        public static void BitmapSourceToFile(string filePath, BitmapSource bmpSrc)
        {
            if (bmpSrc == null)
            {
                return;
            }

            string ext = System.IO.Path.GetExtension(filePath).ToLower();
            BitmapEncoder encoder = null;
            switch (ext)
            {
                case ".bmp":
                    encoder = new BmpBitmapEncoder();
                    break;
                case ".gif":
                    encoder = new GifBitmapEncoder();
                    break;
                case ".jpg":
                    encoder = new JpegBitmapEncoder();
                    break;
                case ".png":
                    encoder = new PngBitmapEncoder();
                    break;
                case ".tif":
                    encoder = new TiffBitmapEncoder();
                    break;
            }

            if (encoder != null)
            {
                encoder.Frames.Add(BitmapFrame.Create(bmpSrc));
                using (FileStream fs = new FileStream(filePath, System.IO.FileMode.Create))
                {
                    encoder.Save(fs);
                }
            }
        }
    }
}
