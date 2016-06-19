using BodyReport.Message;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.Services;
using BodyReportMobile.Core.WebServices;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLabs.Ioc;

namespace BodyReportMobile.Core.Data
{
    public class DataSync
    {
        public static async Task SynchronizeUserImageAsync()
        {
            try
            {
                // download user image
                string localUserImagePath = UserData.Instance.UserInfo == null ? null : AppTools.Instance.GetUserImageLocalPath(UserData.Instance.UserInfo.UserId);

                if (!string.IsNullOrWhiteSpace(localUserImagePath))
                {
                    string urlImage = await UserProfileWebService.GetUserProfileImageRelativeUrlAsync(UserData.Instance.UserInfo.UserId);
                    if (string.IsNullOrEmpty(urlImage))
                    {
                        IFileManager fileManager = Resolver.Resolve<IFileManager>();
                        if (fileManager.FileExist(localUserImagePath))
                            fileManager.DeleteFile(localUserImagePath);
                    }
                    else
                        await HttpConnector.Instance.DownloadFileAsync(urlImage, localUserImagePath);
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to synchronize user image", except);
            }
        }

        public static async Task SynchronizeCountriesAsync()
        {
            try
            {
                //Synchronize Countries
                var countryList = await CountryWebService.FindCountriesAsync();
                if (countryList != null)
                {
                    var dbContext = Resolver.Resolve<ISQLite>().GetConnection();
                    var countryService = new CountryService(dbContext);
                    countryService.UpdateCountryList(countryList);
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to synchronize countries", except);
            }
        }

        public static async Task SynchronizeMusclesAsync(SQLiteConnection dbContext)
        {
            try
            {
                //Synchronize Muscles
                var muscleList = await MuscleWebService.FindMusclesAsync();
                if (muscleList != null)
                {
                    var muscleService = new MuscleService(dbContext);
                    muscleService.UpdateMuscleList(muscleList);
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to synchronize muscles", except);
            }
        }

        public static async Task SynchronizeMuscularGroupAsync(SQLiteConnection dbContext)
        {
            try
            {
                //Synchronize Muscular groups
                var muscularGroupList = await MuscularGroupWebService.FindMuscularGroupsAsync();
                if (muscularGroupList != null)
                {
                    var muscularGroupService = new MuscularGroupService(dbContext);
                    muscularGroupService.UpdateMuscularGroupList(muscularGroupList);
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to synchronize mucular groups", except);
            }
        }

        public static async Task SynchronizeBodyExercisesAsync(SQLiteConnection dbContext)
        {
            try
            {
                //Synchronize body exercises
                var bodyExerciseList = await BodyExerciseWebService.FindBodyExercisesAsync();
                if (bodyExerciseList != null)
                {
                    var bodyExerciseService = new BodyExerciseService(dbContext);
                    bodyExerciseService.UpdateBodyExerciseList(bodyExerciseList);

                    //Synchronize body exercises images
                    List<Task> imagesSyncTaskList = new List<Task>();
                    string urlImage, localImagePath;
                    string urlImages = HttpConnector.Instance.BaseUrl + "images/bodyexercises/{0}";
                    foreach (var bodyExercise in bodyExerciseList)
                    {
                        if (string.IsNullOrWhiteSpace(bodyExercise.ImageName))
                            bodyExercise.ImageName = bodyExercise.Id.ToString() + ".png";
                        urlImage = string.Format(urlImages, bodyExercise.ImageName);
                        localImagePath = Path.Combine(AppTools.BodyExercisesImagesDirectory, bodyExercise.ImageName);
                        var t = AppTools.Instance.CachingImageAsync(bodyExercise, urlImage, localImagePath, null);
                        if (t != null)
                            imagesSyncTaskList.Add(t);
                    }
                    foreach(var imageTask in imagesSyncTaskList)
                    {
                        await imageTask;
                    }
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to synchronize body exercises", except);
            }
        }

        public static async Task SynchronizeTranslationsAsync(SQLiteConnection dbContext)
        {
            try
            {
                //Synchronize Translations
                var translationList = await TranslationWebService.FindTranslationsAsync();
                if (translationList != null)
                {
                    var translationService = new TranslationService(dbContext);
                    translationService.UpdateTranslationList(translationList);
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to synchronize translations", except);
            }
        }

        public static async Task<bool> SynchronizeTrainingWeeksAsync(SQLiteConnection dbContext)
        {
            bool result = false;
            try
            {
                //Synchronize TrainingWeek with server (with trainingday and exercise)
                var criteria = new TrainingWeekCriteria();
                criteria.UserId = new StringCriteria() { Equal = UserData.Instance.UserInfo.UserId };
                TrainingWeekScenario trainingWeekScenario = new TrainingWeekScenario() { ManageTrainingDay = false };
                var trainingWeekService = new TrainingWeekService(dbContext);
                //retreive local data
                var localTrainingWeekList = trainingWeekService.FindTrainingWeek(criteria, trainingWeekScenario);
                //retreive online data
                var criteriaList = new CriteriaList<TrainingWeekCriteria>() { criteria };
                var onlineTrainingWeekList = await TrainingWeekWebService.FindTrainingWeeksAsync(criteriaList, trainingWeekScenario);
                bool found;
                //Delete local data if not found on server
                if (localTrainingWeekList != null)
                {
                    var deletedTrainingWeekList = new List<TrainingWeek>();
                    foreach (var localTrainingWeek in localTrainingWeekList)
                    {
                        found = false;
                        if (onlineTrainingWeekList != null)
                        {
                            foreach (var onlineTrainingWeek in onlineTrainingWeekList)
                            {
                                if (TrainingWeek.IsEqualByKey(onlineTrainingWeek, localTrainingWeek))
                                {
                                    found = true;
                                    break;
                                }
                            }
                        }
                        if (!found)
                            deletedTrainingWeekList.Add(localTrainingWeek);
                    }
                    if (deletedTrainingWeekList.Count > 0)
                    {
                        //Delete in local database
                        trainingWeekService.DeleteTrainingWeekList(deletedTrainingWeekList);
                        foreach (var deleteTrainingWeek in deletedTrainingWeekList)
                            localTrainingWeekList.Remove(deleteTrainingWeek);
                    }
                }
                //if modification date online != local, get full trainingWeek online data and save them on local database
                var synchronizeTrainingWeekList = new List<TrainingWeek>();
                if (onlineTrainingWeekList != null)
                {
                    foreach (var onlineTrainingWeek in onlineTrainingWeekList)
                    {
                        found = false;
                        if (localTrainingWeekList != null)
                        {
                            foreach (var localTrainingWeek in localTrainingWeekList)
                            {
                                //Same trainingWeek
                                if (TrainingWeek.IsEqualByKey(onlineTrainingWeek, localTrainingWeek))
                                {
                                    if (onlineTrainingWeek.ModificationDate.ToUniversalTime() != localTrainingWeek.ModificationDate.ToUniversalTime()) //ToUniversalTime for security...
                                        synchronizeTrainingWeekList.Add(onlineTrainingWeek);
                                    found = true;
                                    break;
                                }
                            }
                        }
                        if (!found)
                            synchronizeTrainingWeekList.Add(onlineTrainingWeek);
                    }
                }

                //Synchronize all trainingWeek data
                trainingWeekScenario = new TrainingWeekScenario()
                {
                    ManageTrainingDay = true,
                    TrainingDayScenario = new TrainingDayScenario() { ManageExercise = true }
                };
                if (synchronizeTrainingWeekList.Count > 0)
                {
                    criteriaList.Clear();
                    foreach (var trainingWeek in synchronizeTrainingWeekList)
                    {
                        criteria = new TrainingWeekCriteria();
                        criteria.UserId = new StringCriteria() { Equal = trainingWeek.UserId };
                        criteria.Year = new IntegerCriteria() { Equal = trainingWeek.Year };
                        criteria.WeekOfYear = new IntegerCriteria() { Equal = trainingWeek.WeekOfYear };
                        criteriaList.Add(criteria);
                    }
                    onlineTrainingWeekList = await TrainingWeekWebService.FindTrainingWeeksAsync(criteriaList, trainingWeekScenario);
                    if (onlineTrainingWeekList != null && onlineTrainingWeekList.Count > 0)
                    {
                        trainingWeekService.UpdateTrainingWeekList(onlineTrainingWeekList, trainingWeekScenario);
                    }
                }
                result = true;
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to synchronize training weeks", except);
            }
            return result;
        }
    }
}
