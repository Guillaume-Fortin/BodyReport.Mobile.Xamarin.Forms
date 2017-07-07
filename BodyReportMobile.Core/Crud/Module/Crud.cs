using System;
using BodyReportMobile.Core.Models;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BodyReportMobile.Core.Crud.Module
{
    public class Crud
    {
        private static object MigrationLocker = new object();
        private static bool MigrationTableDone = false;
        /// <summary>
        /// DataBase context with transaction
        /// </summary>
        protected ApplicationDbContext _dbContext = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">db context</param>
        public Crud(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            MigrateTable(_dbContext);
        }

        public static void MigrateTable(ApplicationDbContext dbContext)
        {
            lock (MigrationLocker)
            {
                if (!MigrationTableDone)
                {
                    MigrateTables(dbContext);
                    MigrationTableDone = true;
                }
            }
        }
        
        public static void EmptyUserTables(ApplicationDbContext dbContext)
        {
           /* dbContext.DeleteAll<UserInfoRow>();
            dbContext.DeleteAll<TrainingWeekRow>();
            dbContext.DeleteAll<TrainingDayRow>();
            dbContext.DeleteAll<TrainingExerciseRow>();
            dbContext.DeleteAll<TrainingExerciseSetRow>();*/
        }
        /*
        private static void CreateTables(ApplicationDbContext dbContext)
        {
            dbContext.CreateTable<UserInfoRow>();
            dbContext.CreateTable<BodyExerciseRow>();
            dbContext.CreateTable<MuscleRow>();
            dbContext.CreateTable<MuscularGroupRow>();
            //Translation table
            dbContext.Execute(@"CREATE TABLE IF NOT EXISTS Translation (CultureId INTEGER NOT NULL,
						  Key VARCHAR(256) NOT NULL,
						  Value VARCHAR(2000),
						  PRIMARY KEY (CultureId, Key))");
            //TrainingWeek table
            dbContext.Execute(@"CREATE TABLE IF NOT EXISTS TrainingWeek (
						  UserId VARCHAR(450) NOT NULL,
						  Year INTEGER NOT NULL,
						  WeekOfYear INTEGER NOT NULL,
						  UserHeight REAL,
						  UserWeight REAL,
						  Unit INTEGER,
                          ModificationDate NUMERIC,
						  PRIMARY KEY (UserId, Year, WeekOfYear))");
            //TrainingDay table
            dbContext.Execute(@"CREATE TABLE IF NOT EXISTS TrainingDay (
						  UserId VARCHAR(450) NOT NULL,
						  Year INTEGER NOT NULL,
						  WeekOfYear INTEGER NOT NULL,
						  DayOfWeek INTEGER NOT NULL,
						  TrainingDayId INTEGER NOT NULL,
						  BeginHour NUMERIC,
						  EndHour NUMERIC,
                          ModificationDate NUMERIC,
						  PRIMARY KEY (UserId, Year, WeekOfYear, DayOfWeek, TrainingDayId))");
            //TrainingExercise table
            dbContext.Execute(@"CREATE TABLE IF NOT EXISTS TrainingExercise (
						  UserId VARCHAR(450) NOT NULL,
						  Year INTEGER NOT NULL,
						  WeekOfYear INTEGER NOT NULL,
						  DayOfWeek INTEGER NOT NULL,
						  TrainingDayId INTEGER NOT NULL,
						  Id INTEGER NOT NULL,
						  BodyExerciseId INTEGER,
						  RestTime INTEGER,
                          EccentricContractionTempo INTEGER DEFAULT NULL,
                          StretchPositionTempo INTEGER DEFAULT NULL,
                          ConcentricContractionTempo INTEGER DEFAULT NULL,
                          ContractedPositionTempo INTEGER DEFAULT NULL,
                          ModificationDate NUMERIC,
                          ExerciseUnitType INTEGER DEFAULT NULL,
						  PRIMARY KEY (UserId, Year, WeekOfYear, DayOfWeek, TrainingDayId, Id))");
            //TrainingExerciseSet table
            dbContext.Execute(@"CREATE TABLE IF NOT EXISTS TrainingExerciseSet (
						  UserId VARCHAR(450) NOT NULL,
						  Year INTEGER NOT NULL,
						  WeekOfYear INTEGER NOT NULL,
						  DayOfWeek INTEGER NOT NULL,
						  TrainingDayId INTEGER NOT NULL,
						  TrainingExerciseId INTEGER NOT NULL,
						  Id INTEGER NOT NULL,
						  NumberOfSets INTEGER,
						  NumberOfReps INTEGER,
						  Weight INTEGER,
						  Unit INTEGER,
                          ModificationDate NUMERIC,
                          ExecutionTime INTEGER DEFAULT NULL,
						  PRIMARY KEY (UserId, Year, WeekOfYear, DayOfWeek, TrainingDayId, TrainingExerciseId, Id))");
            //Country table
            dbContext.CreateTable<CountryRow>();
        }*/

        private static bool MigrateTables(ApplicationDbContext dbContext)
        {
            try
            {
                dbContext.Database.Migrate();
                return true;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            return false;

            /*var version = GetDatabaseVerion(dbContext);
            //new database
            if (string.IsNullOrWhiteSpace(version) || version == "0")
            {
                CreateTables(dbContext);
                version = "4";
                SetDatabaseVerion(dbContext, version);
            }
            //Migrate version 1 to version 2
            if (version == "1")
            {
                dbContext.CreateTable<CountryRow>();
                ExecuteMigrateQuery(dbContext, @"ALTER TABLE TrainingWeek ADD COLUMN ModificationDate TEXT");
                ExecuteMigrateQuery(dbContext, @"ALTER TABLE TrainingDay ADD COLUMN ModificationDate TEXT");
                ExecuteMigrateQuery(dbContext, @"ALTER TABLE TrainingExercise ADD COLUMN ModificationDate TEXT");
                ExecuteMigrateQuery(dbContext, @"ALTER TABLE TrainingExerciseSet ADD COLUMN ModificationDate TEXT");
                version = "2";
                SetDatabaseVerion(dbContext, version);
            }

            if (version == "2")
            {
                dbContext.CreateTable<CountryRow>();
                ExecuteMigrateQuery(dbContext, @"ALTER TABLE TrainingExercise ADD COLUMN EccentricContractionTempo INTEGER DEFAULT NULL");
                ExecuteMigrateQuery(dbContext, @"ALTER TABLE TrainingExercise ADD COLUMN StretchPositionTempo INTEGER DEFAULT NULL");
                ExecuteMigrateQuery(dbContext, @"ALTER TABLE TrainingExercise ADD COLUMN ConcentricContractionTempo INTEGER DEFAULT NULL");
                ExecuteMigrateQuery(dbContext, @"ALTER TABLE TrainingExercise ADD COLUMN ContractedPositionTempo INTEGER DEFAULT NULL");
                version = "3";
                SetDatabaseVerion(dbContext, version);
            }

            if (version == "3")
            {
                // Need delete record for resynchronize data with server modification
                ExecuteMigrateQuery(dbContext, @"DELETE FROM BodyExercise");
                ExecuteMigrateQuery(dbContext, @"DELETE FROM TrainingWeek");
                ExecuteMigrateQuery(dbContext, @"DELETE FROM TrainingDay");
                ExecuteMigrateQuery(dbContext, @"DELETE FROM TrainingExercise");
                ExecuteMigrateQuery(dbContext, @"DELETE FROM TrainingExerciseSet");
                //Modify database schema
                ExecuteMigrateQuery(dbContext, @"ALTER TABLE BodyExercise ADD COLUMN ExerciseCategoryType INTEGER DEFAULT NULL");
                ExecuteMigrateQuery(dbContext, @"ALTER TABLE BodyExercise ADD COLUMN ExerciseUnitType INTEGER DEFAULT NULL");
                ExecuteMigrateQuery(dbContext, @"ALTER TABLE TrainingExercise ADD COLUMN ExerciseUnitType INTEGER DEFAULT NULL");
                ExecuteMigrateQuery(dbContext, @"ALTER TABLE TrainingExerciseSet ADD COLUMN ExecutionTime INTEGER DEFAULT NULL");
                version = "4";
                SetDatabaseVerion(dbContext, version);
            }

            if (version == "4")
            {
                // For test
                ExecuteMigrateQuery(dbContext, @"DELETE FROM BodyExercise");
                ExecuteMigrateQuery(dbContext, @"DELETE FROM TrainingWeek");
                ExecuteMigrateQuery(dbContext, @"DELETE FROM TrainingDay");
                ExecuteMigrateQuery(dbContext, @"DELETE FROM TrainingExercise");
                ExecuteMigrateQuery(dbContext, @"DELETE FROM TrainingExerciseSet");
            }*/
        }
        /*
        private static void ExecuteMigrateQuery(ApplicationDbContext dbContext, string query)
        {
            try
            {
                dbContext.Execute(query);
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to execute migrate table query", except);
            }
        }*/
        /*
        private static string GetDatabaseVerion(ApplicationDbContext dbContext)
        {
            return dbContext.ExecuteScalar<string>("PRAGMA user_version");
        }

        private static void SetDatabaseVerion(ApplicationDbContext dbContext, string versionNumber)
        {
            dbContext.ExecuteScalar<string>(string.Format("PRAGMA user_version={0};", versionNumber.ToString()));
        }*/
        /*
        public static void TestRework(ApplicationDbContext dbContext)
        {
            dbContext.DeleteAll<BodyExerciseRow>();
            dbContext.DeleteAll<MuscleRow>();
            dbContext.DeleteAll<MuscularGroupRow>();
            dbContext.DeleteAll<CountryRow>();
            dbContext.DeleteAll<TrainingWeekRow>();
            dbContext.DeleteAll<TrainingDayRow>();
            dbContext.DeleteAll<TrainingExerciseRow>();
            dbContext.DeleteAll<TrainingExerciseSetRow>();
        }*/
    }
}

