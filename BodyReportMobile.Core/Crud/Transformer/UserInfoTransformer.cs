﻿using BodyReportMobile.Core.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.Crud.Transformer
{
    public class UserInfoTransformer
    {
        public static void ToRow(UserInfo bean, UserInfoRow row)
        {
            if (bean == null)
                return;

            row.UserId = bean.UserId;
            row.Unit = (int)bean.Unit;
            row.Height = bean.Height;
            row.Weight = bean.Weight;
            row.Sex = (int)bean.Sex;
            row.ZipCode = bean.ZipCode;
            row.CountryId = bean.CountryId;
            row.TimeZoneName = bean.TimeZoneName;
        }

        internal static UserInfo ToBean(UserInfoRow row)
        {
            if (row == null)
                return null;

            var bean = new UserInfo();
            bean.UserId = row.UserId;
            bean.Unit = (TUnitType)row.Unit;
            bean.Height = row.Height;
            bean.Weight = row.Weight;
            bean.Sex = (TSexType)row.Sex;
            bean.ZipCode = row.ZipCode;
            bean.CountryId = row.CountryId;
            bean.TimeZoneName = row.TimeZoneName;
            return bean;
        }
    }
}
