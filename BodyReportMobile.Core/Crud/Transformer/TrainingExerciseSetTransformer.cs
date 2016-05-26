﻿using BodyReportMobile.Core.Models;
using Framework;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.Crud.Transformer
{
    public class TrainingExerciseSetTransformer
    {
        public static void ToRow(TrainingExerciseSet bean, TrainingExerciseSetRow row)
        {
            if (bean == null)
                return;

            row.UserId = bean.UserId;
            row.Year = bean.Year;
            row.WeekOfYear = bean.WeekOfYear;
            row.DayOfWeek = bean.DayOfWeek;
            row.TrainingDayId = bean.TrainingDayId;
            row.TrainingExerciseId = bean.TrainingExerciseId;
            row.Id = bean.Id;
            row.NumberOfSets = bean.NumberOfSets;
            row.NumberOfReps = bean.NumberOfReps;
            row.Weight = bean.Weight;
            row.Unit = (int)bean.Unit;
            row.ModificationDate = bean.ModificationDate;
        }

        internal static TrainingExerciseSet ToBean(TrainingExerciseSetRow row)
        {
            if (row == null)
                return null;

            var bean = new TrainingExerciseSet();
            bean.UserId = row.UserId;
            bean.Year = row.Year;
            bean.WeekOfYear = row.WeekOfYear;
            bean.DayOfWeek = row.DayOfWeek;
            bean.TrainingDayId = row.TrainingDayId;
            bean.TrainingExerciseId = row.TrainingExerciseId;
            bean.Id = row.Id;
            bean.NumberOfSets = row.NumberOfSets;
            bean.NumberOfReps = row.NumberOfReps;
            bean.Weight = row.Weight;
            bean.Unit = Utils.IntToEnum<TUnitType>(row.Unit);
            bean.ModificationDate = row.ModificationDate;
            return bean;
        }
    }
}
