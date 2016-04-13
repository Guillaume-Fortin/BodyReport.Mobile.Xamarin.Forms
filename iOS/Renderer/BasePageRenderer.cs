using System;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using UIKit;
using BodyReport;
using BodyReport.iOS;
using BodyReportMobile.Presenter.Pages;

[assembly:ExportRenderer (typeof(BaseContentPage), typeof(BasePageRenderer))]
namespace BodyReport.iOS
{
	public class BasePageRenderer : PageRenderer
	{
		public BasePageRenderer ()
		{
		}

		protected override void OnElementChanged (VisualElementChangedEventArgs e)
		{
			base.OnElementChanged (e);

			var page = e.NewElement as BaseContentPage;

			if (page != null) {
			}
		}

		private void btnReturnClick(object sender, EventArgs e)
		{
			var basePage = Element as BaseContentPage;
			if(basePage != null){
				basePage.SendBackButtonPressed ();
			}	
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear (animated);

			if (this.NavigationController != null) {
				var root = this.NavigationController.TopViewController;
				if (root != null) {
					var basePage = Element as BaseContentPage;
					if (basePage != null) {
						string backBtnTitle = basePage.BackButtonTitle;
						if (!basePage.DisableBackButton && !string.IsNullOrEmpty (backBtnTitle)) {
							root.NavigationItem.SetLeftBarButtonItem (new UIBarButtonItem (backBtnTitle, UIBarButtonItemStyle.Plain, btnReturnClick), true);
							if (basePage.DisableBackButton) {
								root.NavigationItem.LeftBarButtonItem.Style = UIBarButtonItemStyle.Plain;
								root.NavigationItem.LeftBarButtonItem.Enabled = false;
								root.NavigationItem.LeftBarButtonItem.Title = "";
							}
						}

						this.NavigationController.Toolbar.Translucent = false;
						this.NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
						this.NavigationController.NavigationBar.Translucent = false; // pour ne pas que la bar prenne la couleur de fond de la fenêtre


						this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(0.012f, 0.663f, 0.957f); //#03A9F4
						this.NavigationController.NavigationBar.TintColor = UIColor.White;
					}
				}
			}
		}


		//public override void ViewDidAppear (bool animated)
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();
			//this.View.Frame = this.View.Superview.Bounds;
		}
	}
}

