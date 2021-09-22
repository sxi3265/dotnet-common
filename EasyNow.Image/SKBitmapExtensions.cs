using System;
using SkiaSharp;

namespace EasyNow.Image
{
    public static class SKBitmapExtensions
    {
        /// <summary>
        /// 二值化
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="reverse"></param>
        /// <returns></returns>
        public static SKBitmap ToBinaryzation(SKBitmap bitmap,bool reverse=false)
        {
            var total = 0;
            for (var i = 0; i < bitmap.Width; i++)
            {
                for (var j = 0; j < bitmap.Height; j++)
                {
                    var color = bitmap.GetPixel(i, j);
                    total += color.Blue;
                }
            }

            var whiteColor = new SKColor(255, 255, 255);
            var blackColor = new SKColor(0, 0, 0);

            var avg = total / (bitmap.Width * bitmap.Height);
            for (var i = 0; i < bitmap.Width; i++)
            {
                for (var j = 0; j < bitmap.Height; j++)
                {
                    var color = bitmap.GetPixel(i, j);
                    bitmap.SetPixel(i,j,(reverse?color.Blue>=avg:color.Blue<avg)?whiteColor:blackColor);
                }
            }

            return bitmap;
        }

        /// <summary>
        /// 灰度
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static SKBitmap ToGray(SKBitmap bitmap)
        {
            for (var i = 0; i < bitmap.Width; i++)
            {
                for (var j = 0; j < bitmap.Height; j++)
                {
                    var color = bitmap.GetPixel(i, j);
                    var gray = (byte)(0.299 * color.Red + 0.578 * color.Green + 0.114 * color.Blue);
                    var newColor = new SKColor(gray, gray, gray);
                    bitmap.SetPixel(i,j,newColor);
                }
            }

            return bitmap;
        }
    }
}
