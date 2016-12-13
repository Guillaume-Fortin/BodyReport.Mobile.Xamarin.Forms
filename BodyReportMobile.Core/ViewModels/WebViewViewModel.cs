using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using BodyReport.Message;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.ViewModels;
using XLabs.Ioc;

namespace BodyReportMobile.Core
{
	public class WebViewViewModel : BaseViewModel
	{
		public string Url {
			get;
			set;
		}

		private string _printLabel;
		public string PrintLabel {
			get { return _printLabel; }
			set {
				_printLabel = value;
				OnPropertyChanged ();
			}
		}

		public WebViewViewModel () : base()
		{
			PrintLabel = Translation.Get (TRS.PRINT); //necessary for ios Toolbaritem binding failed
		}

		public static async Task<bool> ShowAsync (string url, BaseViewModel parent = null)
		{
			var viewModel = new WebViewViewModel ();
			viewModel.Url = url;
			return await ShowModalViewModelAsync (viewModel, parent);
		}

		protected override void InitTranslation ()
		{
			base.InitTranslation ();
			TitleLabel = Translation.Get ("Preview");
			PrintLabel = Translation.Get (TRS.PRINT); //necessary for ios Toolbaritem binding failed
		}

		private async Task PrintAsync (object webView)
		{
			try {
				var media = Resolver.Resolve<IMedia> ();
				var result = media.PrintDocumentFromWebView (webView);
				if (!result) {
					var userDialog = Resolver.Resolve<IUserDialogs> ();
					await userDialog.AlertAsync ("Unable to print", "error", "ok");
				}
			} catch (Exception except) {
				ILogger.Instance.Error ("Unable to print trainingDay", except);
			}
		}

		#region Command

		private ICommand _printCommand = null;
		public ICommand PrintCommand {
			get {
				if (_printCommand == null) {
					_printCommand = new ViewModelCommandAsync (this, async (object webView) => {
						await PrintAsync (webView);
					});
				}
				return _printCommand;
			}
		}

		#endregion
	}
}
