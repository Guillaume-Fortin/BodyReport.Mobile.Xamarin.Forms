using System;
using Xamarin.Forms;
using BodyReport.iOS.Framework.Renderers;

[assembly: ExportRenderer(typeof(Label), typeof(LabelRenderer))]
namespace BodyReport.iOS.Framework.Renderers
{
	public class LabelRenderer : Xamarin.Forms.Platform.iOS.LabelRenderer
	{
		/// <summary>
		/// Raises the element property changed event.
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="e">The event arguments</param>
		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (Control != null)
			{
				Control.AdjustsFontSizeToFitWidth = true;
				Control.MinimumScaleFactor = 0.5f;
				Control.MinimumFontSize = 8;
			}
		}
	}
}

