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
            if (File.Exists(compressedImagePath))
                File.Delete(compressedImagePath);

			NSData convertedImageData;
            UIImage image = UIImage.FromFile(imagePath);
			if (!maxSize.HasValue)
			{
				convertedImageData = image.AsPNG ();
				convertedImageData.Save (compressedImagePath, true);
				return;
			}

			CGSize newSize = new CGSize ((nfloat)maxSize, (nfloat)maxSize);
			if (image.Size.Width < newSize.Width && image.Size.Height < newSize.Height)
			{
				convertedImageData = image.AsPNG ();
				convertedImageData.Save (compressedImagePath, true);
				return;
			}

			//Determine the scale factors
			nfloat widthScale = newSize.Width/image.Size.Width;
			nfloat heightScale = newSize.Height/image.Size.Height;

			nfloat scaleFactor;

			//The larger scale factor will scale less (0 < scaleFactor < 1) leaving the other dimension hanging outside the newSize rect
			if (widthScale > heightScale)
				scaleFactor = widthScale;
			else 
				scaleFactor = heightScale;
			CGSize scaledSize = new CGSize(image.Size.Width * scaleFactor, image.Size.Height * scaleFactor);

			UIImage convertedImage = UIImage.FromImage (image.CGImage).Scale (scaledSize);
			convertedImageData = convertedImage.AsPNG();
			convertedImageData.Save(compressedImagePath, true);
        }
    }
}
