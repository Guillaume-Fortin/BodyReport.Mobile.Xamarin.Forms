using BodyReportMobile.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.Framework
{
    public interface IPresenterManager
    {
        void AddViewDependency<TViewModel, TView>() where TViewModel : class where TView : class;
        Task<bool> TryDisplayViewAsync(BaseViewModel viewModel, BaseViewModel parentViewModel);
    }
}
