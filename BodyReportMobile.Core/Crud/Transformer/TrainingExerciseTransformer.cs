using BodyReportMobile.Core.Models;
using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BodyReport.Framework;

namespace BodyReportMobile.Core.Crud.Transformer
{
    public class TrainingExerciseTransformer
    {
        public static void ToRow(TrainingExercise bean, TrainingExerciseRow row)
        {
            if (bean == null)
                return;

            row.UserId = bean.UserId;
            row.Year = bean.Year;
            row.WeekOfYear = bean.WeekOfYear;
            row.DayOfWeek = bean.DayOfWeek;
            row.TrainingDayId = bean.TrainingDayId;
            row.Id = bean.Id;
            row.BodyExerciseId = bean.BodyExerciseId;
            row.RestTime = bean.RestTime;
            row.ModificationDate = DbUtils.DbDateToUtc(bean.ModificationDate);
            row.EccentricContractionTempo = bean.EccentricContractionTempo;
            row.StretchPositionTempo = bean.StretchPositionTempo;
            row.ConcentricContractionTempo = bean.ConcentricContractionTempo;
            row.ContractedPositionTempo = bean.ContractedPositionTempo;
            row.ExerciseUnitType = (int)bean.ExerciseUnitType;
        }

        internal static TrainingExercise ToBean(TrainingExerciseRow row)
        {
            if (row == null)
                return null;

            var bean = new TrainingExercise();
            bean.UserId = row.UserId;
            bean.Year = row.Year;
            bean.WeekOfYear = row.WeekOfYear;
            bean.DayOfWeek = row.DayOfWeek;
            bean.TrainingDayId = row.TrainingDayId;
            bean.Id = row.Id;
            bean.BodyExerciseId = row.BodyExerciseId;
            bean.RestTime = row.RestTime;
            bean.ModificationDate = DbUtils.DbDateToUtc(row.ModificationDate);
            bean.EccentricContractionTempo = row.EccentricContractionTempo.HasValue ? row.EccentricContractionTempo.Value : 0;
            bean.StretchPositionTempo = row.StretchPositionTempo.HasValue ? row.StretchPositionTempo.Value : 0;
            bean.ConcentricContractionTempo = row.ConcentricContractionTempo.HasValue ? row.ConcentricContractionTempo.Value : 0;
            bean.ContractedPositionTempo = row.ContractedPositionTempo.HasValue ? row.ContractedPositionTempo.Value : 0;
            bean.ExerciseUnitType = Utils.IntToEnum<TExerciseUnitType>(row.ExerciseUnitType ?? (int)TExerciseUnitType.RepetitionNumber);
            return bean;
        }
    }
}
