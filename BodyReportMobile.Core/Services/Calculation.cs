﻿using System;

namespace BodyReportMobile.Core.Services
{
	public class Calculation : ICalculation
	{
		public double TipAmount(double subTotal, int generosity)
		{
			return subTotal * ((double)generosity)/100.0;
		}
	}
}

