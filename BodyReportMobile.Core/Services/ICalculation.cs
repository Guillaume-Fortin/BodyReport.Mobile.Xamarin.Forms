﻿using System;

namespace BodyReportMobile.Core.Services
{
	public interface ICalculation
	{
		double TipAmount(double subTotal, int generosity);
	}
}

