﻿using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BodyReportMobile.Core;
using BodyReportMobile.Core.Models;

namespace BodyReportMobile.Core.Crud.Transformer
{
    public static class MuscleTransformer
    {
        public static string GetTranslationKey(int muscleId)
        {
            return string.Format("MUSCLE-{0}", muscleId);
        }

        public static void ToRow(Muscle bean, MuscleRow row)
        {
            if (bean == null)
                return;

            row.Id = bean.Id;
            row.MuscularGroupId = bean.MuscularGroupId;
        }

        internal static Muscle ToBean(MuscleRow row)
        {
            if (row == null)
                return null;

            var bean = new Muscle();
            bean.Id = row.Id;
            bean.MuscularGroupId = row.MuscularGroupId;
            //bean.Name = Resources.Translation.GetInDB(GetTranslationKey(row.Id));
            return bean;
        }
    }
}
