using System;
using System.Data;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using OfficeOpenXml;
using System.Drawing.Drawing2D;

namespace ChuXinEdu.CMS.Server.Utils
{
    public class ImageHelper
    {
        public static void SaveThumbnailImage(Bitmap originalImage, string originalPath, int width, int heigth)
        {
            string newPath = originalPath + "_" + width.ToString() + "X" + heigth.ToString() + ".png";

            if (!File.Exists(newPath))
            {
                Image thumbnailImage = new Bitmap(width, heigth);
                Graphics graphics = Graphics.FromImage(thumbnailImage);
                graphics.InterpolationMode = InterpolationMode.High;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.Clear(System.Drawing.Color.Transparent);
                graphics.DrawImage(originalImage, new Rectangle(0, 0, width, heigth), new Rectangle(0, 0, originalImage.Width, originalImage.Height), GraphicsUnit.Pixel);
                thumbnailImage.Save(newPath);
                graphics.Dispose();
            }
        }

        public static void ChangeImageSize(string path, int width, int heigth)
        {
            if (File.Exists(path))
            {
                Byte[] bytes = File.ReadAllBytes(path);

                using (Stream sm = new MemoryStream(bytes))
                {
                    Bitmap oriBitmap = new Bitmap(Bitmap.FromStream(sm));

                    Image thumbnailImage = new Bitmap(width, heigth);
                    Graphics graphics = Graphics.FromImage(thumbnailImage);
                    graphics.InterpolationMode = InterpolationMode.High;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.Clear(System.Drawing.Color.Transparent);
                    graphics.DrawImage(oriBitmap, new Rectangle(0, 0, width, heigth), new Rectangle(0, 0, oriBitmap.Width, oriBitmap.Height), GraphicsUnit.Pixel);
                    thumbnailImage.Save(path);
                    graphics.Dispose();
                }

            }
        }
    }
}