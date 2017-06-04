﻿using BodyReportMobile.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BodyReportMobile.Presenter.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfidentialityRulesPage : BaseContentPage
    {
        public ConfidentialityRulesPage(ConfidentialityRulesViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
