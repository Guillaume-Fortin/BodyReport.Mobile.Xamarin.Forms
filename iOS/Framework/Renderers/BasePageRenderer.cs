using System;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using UIKit;
using BodyReport;
using BodyReportMobile.Presenter.Pages;

[assembly:ExportRenderer (typeof(BodyReportMobile.Presenter.Pages.BaseContentPage), typeof(BodyReport.iOS.Framework.Renderers.BasePageRenderer))]
namespace BodyReport.iOS.Framework.Renderers
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

						var viewModel = basePage.ViewModel;
						if (viewModel != null)
						{
							root.NavigationItem.SetLeftBarButtonItem (new UIBarButtonItem (backBtnTitle, UIBarButtonItemStyle.Plain, btnReturnClick), true);
							if (viewModel.DisableBackButton)
							{
								root.NavigationItem.LeftBarButtonItem.Style = UIBarButtonItemStyle.Plain;
								root.NavigationItem.LeftBarButtonItem.Enabled = false;
								root.NavigationItem.LeftBarButtonItem.Title = "";
							}
						}

						this.NavigationController.Toolbar.Translucent = false;
						this.NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
						this.NavigationController.NavigationBar.Translucent = false; // pour ne pas que la bar prenne la couleur de fond de la fenêtre


						Color color;
						if (basePage.Resources != null)
						{
							if (basePage.Resources.ContainsKey ("titleBarColor"))
							{
								color = (Color)basePage.Resources ["titleBarColor"];
								this.NavigationController.NavigationBar.BarTintColor = color.ToUIColor();
							}
							if (basePage.Resources.ContainsKey ("titleBarTextColor"))
							{
								color = (Color)basePage.Resources ["titleBarTextColor"];
								this.NavigationController.NavigationBar.TintColor = color.ToUIColor();
								this.NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes()
								{
									ForegroundColor = color.ToUIColor()
								};
							}
						}
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

