using System;
using MvvmCross.Core.ViewModels;
using BodyReportMobile.Core.Services;
using System.Windows.Input;
using BodyReportMobile.Core;
using System.Threading.Tasks;
using MvvmCross.Plugins.Messenger;
using System.Threading;

namespace BodyReportMobile.Core.ViewModels
{
	public class TipViewModel : BaseViewModel
	{
		public TipViewModel (IMvxMessenger messenger) : base(messenger)
		{
		}

		public ICommand DisplaySecondPageCommand
		{
			get
			{
				return new MvxCommand(Test);
			}
		}

		/*async Task*/ void Test()
		{
			//await ShowModalViewModel<SecondViewModel>(this);
			//await ShowModalViewModel<ThirdViewModel>(this);
		}
		/*
		async Task<bool> displayPage()
		{
			var tcs = new TaskCompletionSource<bool>();

			var myPage = new MyPage ();
			myPage.Disappearing += (sender, e) => tcs.SetResult(true);

			await this.MainPage.Navigation.PushAsync (myPage);

			return await tcs.Task;
		}
		*/

		/*	private readonly ICalculation _calculation;
		public TipViewModel(ICalculation calculation)
		{
			_calculation = calculation;
		}

		public override void Start()
		{
			_subTotal = 100;
			_generosity = 10;
			Recalculate();
			base.Start();
		}

		private double _subTotal;

		public double SubTotal
		{
			get { return _subTotal; }
			set { _subTotal = value; RaisePropertyChanged(() => SubTotal); Recalculate(); }
		}

		private int _generosity;

		public int Generosity
		{
			get { return _generosity; }
			set 
			{
				if (value != _generosity) {
					_generosity = value;

					RaisePropertyChanged (() => Generosity);
					Recalculate ();
				}
			}
		}

		private double _tip;

		public double Tip
		{
			get { return _tip; }
			private set
			{
				_tip = value;
				RaisePropertyChanged(() => Tip);
			}
		}

		private void Recalculate()
		{
			Tip = _calculation.TipAmount(SubTotal, Generosity);
		}*/
	}
}

