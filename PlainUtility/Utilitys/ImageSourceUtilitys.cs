using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// <a href="http://blog.shibayan.jp/entry/20071013/1192204667">WPF に用意されている 7 種類のビットマップ</a>>
/// </remarks>
namespace Utilitys
{
    public class ImageSourceUtility
    {
        public static BitmapSource DrawStrokes(BitmapSource imageSource, ICollection<Stroke> strokes, Size strokeCanvasSize)
        {
            if (imageSource != null && strokes != null && strokeCanvasSize.Width > 0 && strokeCanvasSize.Height > 0)
            {
                DrawingVisual drawingVisual = new DrawingVisual();
                using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                {
                    // DrawingContext.DrawImageがdpi96を想定しているため補正してから描画する
                    double imageSourceWidth = imageSource.PixelWidth * 96.0 / imageSource.DpiX;
                    double imageSourceHeight = imageSource.PixelHeight * 96.0 / imageSource.DpiY;

                    drawingContext.DrawImage(imageSource, new Rect(new Size(imageSourceWidth, imageSourceHeight)));

                    var matrix = new Matrix(imageSourceWidth / strokeCanvasSize.Width, 0, 0, imageSourceHeight / strokeCanvasSize.Height, 0, 0);

                    foreach (var stroke in strokes)
                    {
                        var val = stroke.Clone();
                        val.Transform(matrix, true);
                        val.Draw(drawingContext);
                    }
                }
                RenderTargetBitmap bitmap = new RenderTargetBitmap(imageSource.PixelWidth, imageSource.PixelHeight, imageSource.DpiX, imageSource.DpiY, PixelFormats.Pbgra32);
                bitmap?.Render(drawingVisual);
                return bitmap as BitmapSource;
            }
            else
                return imageSource;
        }

        public static BitmapSource RedrawBitmapSourceToNewDpiBpp(BitmapSource bitmapSource, double dpi, int bpp)
        {
            PixelFormat pixelFormat;
            switch (bpp)
            {
                case 2:
                    pixelFormat = PixelFormats.Gray2;
                    break;
                case 8:
                    pixelFormat = PixelFormats.Gray8;
                    break;
                case 16:
                    pixelFormat = PixelFormats.Gray16;
                    break;
                case 32:
                    pixelFormat = PixelFormats.Pbgra32;
                    break;
                default:
                case 58:
                    pixelFormat = PixelFormats.Rgb24;
                    break;
            }

            BitmapSource source;

            var bf = BitmapFrame.Create(bitmapSource);

            var convertedBitmap = new FormatConvertedBitmap(bf, pixelFormat, null, 0);
            int w = convertedBitmap.PixelWidth;
            int h = convertedBitmap.PixelHeight;
            int stride = (w * pixelFormat.BitsPerPixel + 7) / 8;
            byte[] pixels = new byte[h * stride];
            convertedBitmap.CopyPixels(pixels, stride, 0);

            source = BitmapSource.Create(w, h, dpi, dpi, convertedBitmap.Format, convertedBitmap.Palette, pixels, stride);

            return source;
        }

        public static void BitmapSourceToFile(string filePath, BitmapSource bmpSrc, FileMode fileMode = FileMode.OpenOrCreate, FileAccess fileAccess = FileAccess.Write)
        {
            if (bmpSrc == null)
            {
                return;
            }

            string ext = System.IO.Path.GetExtension(filePath).ToLower();
            BitmapEncoder encoder = null;
            switch (ext)
            {
                default:
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
                using (FileStream fs = new FileStream(filePath, fileMode, fileAccess))
                {
                    encoder.Save(fs);
                }
            }
        }
    }
}
