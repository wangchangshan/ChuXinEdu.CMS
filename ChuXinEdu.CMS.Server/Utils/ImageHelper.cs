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
        public static void SaveThumbnailImage(Bitmap originalImage, string folderPath, string name, int width, int heigth)
        {
            string imagePath = folderPath;
            string imageName = name + "_" + width.ToString() + "X" + heigth.ToString() + ".png";
            imagePath = System.IO.Path.Combine(imagePath, imageName);

            Image thumbnailImage = new Bitmap(width, heigth);
            Graphics graphics = Graphics.FromImage(thumbnailImage);
            graphics.InterpolationMode = InterpolationMode.High;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.Clear(System.Drawing.Color.Transparent);
            graphics.DrawImage(originalImage, new Rectangle(0, 0, width, heigth), new Rectangle(0, 0, originalImage.Width, originalImage.Height), GraphicsUnit.Pixel);
            thumbnailImage.Save(imagePath);
            graphics.Dispose();
        }
    }
}