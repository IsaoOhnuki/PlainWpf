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
