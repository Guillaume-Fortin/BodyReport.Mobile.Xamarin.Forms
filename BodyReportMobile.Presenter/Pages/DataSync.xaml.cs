﻿using BodyReportMobile.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BodyReportMobile.Presenter.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DataSync : BaseContentPage
    {
        public DataSync(DataSyncViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
