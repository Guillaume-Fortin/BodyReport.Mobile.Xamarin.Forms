using System;
using SQLite.Net;
using BodyReportMobile.Core.Models;
using BodyReportMobile.Core.Framework;

namespace BodyReportMobile.Core.Crud.Module
{
	public class Crud
	{
        private static object MigrationLocker = new object();
        private static bool MigrationTableDone = false;
        /// <summary>
        /// DataBase context with transaction
        /// </summary>
        protected SQLiteConnection _dbContext = null;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dbContext">db context</param>
		public Crud(SQLiteConnection dbContext)
		{
			_dbContext = dbContext;
            MigrateTable(_dbContext);
        }

        public static void MigrateTable(SQLiteConnection dbContext)
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

		private static void CreateTables(SQLiteConnection dbContext)
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
                          ModificationDate TEXT,
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
                          ModificationDate TEXT,
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
                          ModificationDate TEXT,
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
                          ModificationDate TEXT,
						  PRIMARY KEY (UserId, Year, WeekOfYear, DayOfWeek, TrainingDayId, TrainingExerciseId, Id))");
        }

        private static void MigrateTables(SQLiteConnection dbContext)
        {
            var version = GetDatabaseVerion(dbContext);
            //new database
            if (string.IsNullOrWhiteSpace(version) || version == "0")
            {
                CreateTables(dbContext);
                version = "2";
                SetDatabaseVerion(dbContext, version);
            }
            //Migrate version 1 to version 2
            if(version == "1")
            {
                ExecuteMigrateQuery(dbContext, @"ALTER TABLE TrainingWeek ADD COLUMN ModificationDate TEXT");
                ExecuteMigrateQuery(dbContext, @"ALTER TABLE TrainingDay ADD COLUMN ModificationDate TEXT");
                ExecuteMigrateQuery(dbContext, @"ALTER TABLE TrainingExercise ADD COLUMN ModificationDate TEXT");
                ExecuteMigrateQuery(dbContext, @"ALTER TABLE TrainingExerciseSet ADD COLUMN ModificationDate TEXT");
                version = "2";
                SetDatabaseVerion(dbContext, version);
            }
        }

        private static void ExecuteMigrateQuery(SQLiteConnection dbContext, string query)
        {
            try
            {
                dbContext.Execute(query);
            }
            catch(Exception except)
            {
                ILogger.Instance.Error("Unable to execute migrate table query", except);
            }
        }

        private static string GetDatabaseVerion(SQLiteConnection dbContext)
        {
            return dbContext.ExecuteScalar<string>("PRAGMA user_version");
        }

        private static void SetDatabaseVerion(SQLiteConnection dbContext, string versionNumber)
        {
            dbContext.ExecuteScalar<string>(string.Format("PRAGMA user_version={0};", versionNumber.ToString()));
        }
    }
}

