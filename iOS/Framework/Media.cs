using BodyReportMobile.Core.Framework;
using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UIKit;
using BodyReportMobile.Presenter.Framework.Controls;
using Xamarin.Forms;

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

		public override bool PrintDocumentFromWebView (object webView)
		{
			try {
				UIWebView platformWebView = (UIWebView)(webView as CustomWebView).PlatformControl;

				UIPrintInteractionController printer = UIPrintInteractionController.SharedPrintController;

				printer.ShowsPageRange = true;

				printer.PrintInfo = UIPrintInfo.PrintInfo;
				printer.PrintInfo.OutputType = UIPrintInfoOutputType.General;
				printer.PrintInfo.JobName = "BodyReportJob";

				printer.PrintPageRenderer = new UIPrintPageRenderer () {
					HeaderHeight = 40,
					FooterHeight = 40
				};
				printer.PrintPageRenderer.AddPrintFormatter (platformWebView.ViewPrintFormatter, 0);

				if (Device.Idiom == TargetIdiom.Phone) {
					printer.PresentAsync (true);
				} else if (Device.Idiom == TargetIdiom.Tablet) {
					printer.PresentFromRectInViewAsync (new CGRect (200, 200, 0, 0), platformWebView, true);
				}

				return true;
			} catch (Exception) {
				return false;
			}
		}
    }
}
