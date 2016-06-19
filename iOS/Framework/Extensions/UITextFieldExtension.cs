using System;
using UIKit;
using CoreAnimation;
using CoreGraphics;
using System.Linq;

namespace BodyReport.iOS
{
	public static class UITextFieldExtension
	{
		public static void DrawCustomBorder(this UITextField control, UIColor borderColor, float borderWidth)
		{
			if (control == null || control.Layer == null)
				return;
			
			var borderName = "customBorder";
			var borderRect = new CGRect (0, control.Frame.Height - borderWidth, control.Frame.Width, borderWidth);

			CALayer oldLayer = null;
			if(control.Layer.Sublayers != null)
				oldLayer = control.Layer.Sublayers.Where (l => l.Name == borderName).FirstOrDefault ();
			if (oldLayer != null)
			{
				oldLayer.Frame = borderRect;
			}
			else
			{
				var borderLayer = new CALayer ();
				borderLayer.MasksToBounds = true;
				borderLayer.Frame = borderRect;
				borderLayer.BorderColor = borderColor.CGColor;
				borderLayer.BorderWidth = borderWidth;
				borderLayer.Name = borderName;
				control.Layer.AddSublayer (borderLayer);
			}
		}
	}
}

