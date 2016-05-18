using BodyReportMobile.Core.Framework;
using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UIKit;

namespace BodyReport.iOS.Framework
{
    public class Media : IMedia
    {
        public override void CompressImageAsPng(string imagePath, string compressedImagePath, int? maxSize)
        {
            //TODO scale image
            if (File.Exists(compressedImagePath))
                File.Delete(compressedImagePath);

            CGImage cgImage = UIImage.FromFile(imagePath).CGImage;
            nint originWidth = cgImage.Width;
            nint originHeight = cgImage.Height;
            var bitsPerComponent = cgImage.BitsPerComponent;
            var bytesPerRow = cgImage.BitsPerPixel;
            var colorSpace = cgImage.ColorSpace;
            var bitmapInfo = cgImage.BitmapInfo;
            using (var context = new CGBitmapContext(null, originWidth, originHeight, bitsPerComponent, bytesPerRow, colorSpace, bitmapInfo))
            {
                context.InterpolationQuality = CGInterpolationQuality.High;
                context.DrawImage(new CGRect(CGPoint.Empty, new CGSize(originWidth, originHeight)), cgImage);
                using(var imageRef = context.ToImage())
                {
                    UIImage convertedImage = new UIImage(imageRef);
                    NSData convertedImageData = convertedImage.AsPNG();
                    convertedImageData.Save(compressedImagePath, true);
                }
            }
        }
    }
}
