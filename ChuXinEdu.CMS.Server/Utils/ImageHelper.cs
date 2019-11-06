using System;
using System.Data;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using OfficeOpenXml;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ChuXinEdu.CMS.Server.Utils
{
    public class ImageHelper
    {
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        /// <summary>
        /// 图片压缩(降低质量以减小文件的大小)
        /// </summary>
        /// <param name="srcBitmap">传入的Bitmap对象</param>
        /// <param name="destStream">压缩后的Stream对象</param>
        /// <param name="level">压缩等级，0到100，0 最差质量，100 最佳</param>
        public static void Compress(Bitmap srcBitmap, string destFile, long level)
        {
            ImageCodecInfo myImageCodecInfo;
            Encoder myEncoder;
            EncoderParameter myEncoderParameter;
            EncoderParameters myEncoderParameters;

            // Get an ImageCodecInfo object that represents the JPEG codec.
            myImageCodecInfo = GetEncoderInfo("image/jpeg");

            // Create an Encoder object based on the GUID

            // for the Quality parameter category.
            myEncoder = Encoder.Quality;

            // Create an EncoderParameters object.
            // An EncoderParameters object has an array of EncoderParameter
            // objects. In this case, there is only one

            // EncoderParameter object in the array.
            myEncoderParameters = new EncoderParameters(1);

            // Save the bitmap as a JPEG file with 给定的 quality level
            myEncoderParameter = new EncoderParameter(myEncoder, level);
            myEncoderParameters.Param[0] = myEncoderParameter;


            srcBitmap.Save(destFile, myImageCodecInfo, myEncoderParameters);
        }

        /// <summary>
        /// 图片压缩(降低质量以减小文件的大小)
        /// </summary>
        /// <param name="srcBitmap">传入的Bitmap对象</param>
        /// <param name="destStream">压缩后的Stream对象</param>
        /// <param name="level">压缩等级，0到100，0 最差质量，100 最佳</param>
        public static void Compress(Bitmap srcBitmap, Stream destStream, long level)
        {
            ImageCodecInfo myImageCodecInfo;
            Encoder myEncoder;
            EncoderParameter myEncoderParameter;
            EncoderParameters myEncoderParameters;

            // Get an ImageCodecInfo object that represents the JPEG codec.
            myImageCodecInfo = GetEncoderInfo("image/jpeg");

            // Create an Encoder object based on the GUID

            // for the Quality parameter category.
            myEncoder = Encoder.Quality;

            // Create an EncoderParameters object.
            // An EncoderParameters object has an array of EncoderParameter
            // objects. In this case, there is only one

            // EncoderParameter object in the array.
            myEncoderParameters = new EncoderParameters(1);

            // Save the bitmap as a JPEG file with 给定的 quality level
            myEncoderParameter = new EncoderParameter(myEncoder, level);
            myEncoderParameters.Param[0] = myEncoderParameter;


            srcBitmap.Save(destStream, myImageCodecInfo, myEncoderParameters);
        }

        /// <summary>
        /// 保存缩略图
        /// </summary>
        /// <param name="isCompress">是否压缩</param>
        public static void SaveThumbnailImage(Bitmap originalImage, string originalPath, int width, int heigth, bool isCompress, string ext)
        {
            string newPath = originalPath + "_" + width.ToString() + ext;

            if (!File.Exists(newPath))
            {
                Bitmap thumbnailImage = new Bitmap(width, heigth);
                Graphics graphics = Graphics.FromImage(thumbnailImage);
                graphics.InterpolationMode = InterpolationMode.High;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.Clear(System.Drawing.Color.Transparent);
                graphics.DrawImage(originalImage, new Rectangle(0, 0, width, heigth), new Rectangle(0, 0, originalImage.Width, originalImage.Height), GraphicsUnit.Pixel);

                if (isCompress)
                {
                    int imageCompressLevel = 50;
                    string strImageCompressLevel = CustomConfig.GetSetting("ImageCompressLevel");
                    if (!String.IsNullOrEmpty(strImageCompressLevel))
                    {
                        imageCompressLevel = Convert.ToInt32(strImageCompressLevel);
                    }
                    Compress(thumbnailImage, newPath, imageCompressLevel);
                }
                else
                {
                    thumbnailImage.Save(newPath);
                }
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