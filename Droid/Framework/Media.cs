using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using static Android.Graphics.Bitmap;
using System.IO;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Presenter.Framework.Controls;
using Android.Print;
using Xamarin.Forms;

namespace BodyReport.Droid.Framework
{
    public class Media : IMedia
    {
        public override void CompressImageAsPng(string imagePath, string compressedImagePath, int? maxSize)
        {
            BitmapFactory.Options options = new BitmapFactory.Options();
            if (maxSize.HasValue)
            {
                // First we get the the dimensions of the file on disk
                options.InPurgeable = true;
                options.InJustDecodeBounds = true;
                BitmapFactory.DecodeFile(imagePath, options);

                // The actual width of the image.
                int srcWidth = options.OutWidth;
                // The actual height of the image.
                int srcHeight = options.OutHeight;
                // Only scale if the source is bigger than the width/height of the destination view.
                if (maxSize > Math.Max(srcWidth, srcHeight))
                    maxSize = Math.Max(srcWidth, srcHeight);

                // Calculate the correct inSampleSize/scale value. This helps reduce memory use. It should be a power of 2.
                int inSampleSize = 1;
                while (srcWidth / 2 > maxSize || srcHeight / 2 > maxSize)
                {
                    srcWidth /= 2;
                    srcHeight /= 2;
                    inSampleSize *= 2;
                }

                float desiredScale = (float)maxSize / srcWidth;

                // Decode with inSampleSize
                options.InJustDecodeBounds = false;
                options.InDither = false;
                options.InSampleSize = inSampleSize;
                options.InScaled = false;
                // Ensures the image stays as a 32-bit ARGB_8888 image.
                // This preserves image quality.
                options.InPreferredConfig = Bitmap.Config.Argb8888;

                Bitmap srcBitmap = BitmapFactory.DecodeFile(imagePath, options);

                // Resize
                Matrix matrix = new Matrix();
                matrix.PostScale(desiredScale, desiredScale);
                Bitmap scaledBitmap = Bitmap.CreateBitmap(srcBitmap, 0, 0, srcBitmap.Width, srcBitmap.Height, matrix, true);
                srcBitmap = null;

                if (File.Exists(compressedImagePath))
                    File.Delete(compressedImagePath);

                using (Stream fileStream = File.OpenWrite(compressedImagePath))
                {
                    scaledBitmap.Compress(CompressFormat.Png, 100, fileStream);
                    fileStream.Flush();
                }
            }
            else
            {

                // Decode with inSampleSize
                options.InPurgeable = true;
                options.InJustDecodeBounds = false;
                options.InDither = false;
                options.InSampleSize = 1;
                options.InScaled = false;
                // Ensures the image stays as a 32-bit ARGB_8888 image.
                // This preserves image quality.
                options.InPreferredConfig = Bitmap.Config.Argb8888;

                Bitmap srcBitmap = BitmapFactory.DecodeFile(imagePath, options);

                if (File.Exists(compressedImagePath))
                    File.Delete(compressedImagePath);

                using (Stream fileStream = File.OpenWrite(compressedImagePath))
                {
                    srcBitmap.Compress(CompressFormat.Png, 100, fileStream);
                    fileStream.Flush();
                }
            }
        }

		public override bool PrintDocumentFromWebView (object webView)
		{
			try {
				PrintManager printManager = (PrintManager)Forms.Context.GetSystemService (Context.PrintService);

				Android.Webkit.WebView platformWebView = (Android.Webkit.WebView)(webView as CustomWebView).PlatformControl;

				printManager.Print ("BodyReportJob", platformWebView.CreatePrintDocumentAdapter ("BodyReportJob"), null);

				return true;
			} catch (Exception) {
				return false;
			}
		}
    }
}