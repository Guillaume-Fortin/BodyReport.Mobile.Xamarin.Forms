using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BodyReportMobile.Core.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BodyExercise",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    ExerciseCategoryType = table.Column<int>(nullable: true),
                    ExerciseUnitType = table.Column<int>(nullable: true),
                    MuscleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyExercise", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 400, nullable: true),
                    ShortName = table.Column<string>(maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Muscle",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    MuscularGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Muscle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MuscularGroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MuscularGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrainingDay",
                columns: table => new
                {
                    UserId = table.Column<string>(maxLength: 450, nullable: false),
                    Year = table.Column<int>(nullable: false),
                    WeekOfYear = table.Column<int>(nullable: false),
                    DayOfWeek = table.Column<int>(nullable: false),
                    TrainingDayId = table.Column<int>(nullable: false),
                    BeginHour = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)),
                    EndHour = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)),
                    ModificationDate = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)),
                    Unit = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingDay", x => new { x.UserId, x.Year, x.WeekOfYear, x.DayOfWeek, x.TrainingDayId });
                });

            migrationBuilder.CreateTable(
                name: "TrainingExercise",
                columns: table => new
                {
                    UserId = table.Column<string>(maxLength: 450, nullable: false),
                    Year = table.Column<int>(nullable: false),
                    WeekOfYear = table.Column<int>(nullable: false),
                    DayOfWeek = table.Column<int>(nullable: false),
                    TrainingDayId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    BodyExerciseId = table.Column<int>(nullable: false),
                    ConcentricContractionTempo = table.Column<int>(nullable: true),
                    ContractedPositionTempo = table.Column<int>(nullable: true),
                    EccentricContractionTempo = table.Column<int>(nullable: true),
                    ExerciseUnitType = table.Column<int>(nullable: true),
                    ModificationDate = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)),
                    RestTime = table.Column<int>(nullable: false),
                    StretchPositionTempo = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingExercise", x => new { x.UserId, x.Year, x.WeekOfYear, x.DayOfWeek, x.TrainingDayId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "TrainingExerciseSet",
                columns: table => new
                {
                    UserId = table.Column<string>(maxLength: 450, nullable: false),
                    Year = table.Column<int>(nullable: false),
                    WeekOfYear = table.Column<int>(nullable: false),
                    DayOfWeek = table.Column<int>(nullable: false),
                    TrainingDayId = table.Column<int>(nullable: false),
                    TrainingExerciseId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    ExecutionTime = table.Column<int>(nullable: true),
                    ModificationDate = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)),
                    NumberOfReps = table.Column<int>(nullable: false),
                    NumberOfSets = table.Column<int>(nullable: false),
                    Weight = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingExerciseSet", x => new { x.UserId, x.Year, x.WeekOfYear, x.DayOfWeek, x.TrainingDayId, x.TrainingExerciseId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "TrainingWeek",
                columns: table => new
                {
                    UserId = table.Column<string>(maxLength: 450, nullable: false),
                    Year = table.Column<int>(nullable: false),
                    WeekOfYear = table.Column<int>(nullable: false),
                    ModificationDate = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)),
                    Unit = table.Column<int>(nullable: false),
                    UserHeight = table.Column<double>(nullable: false),
                    UserWeight = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingWeek", x => new { x.UserId, x.Year, x.WeekOfYear });
                });

            migrationBuilder.CreateTable(
                name: "Translation",
                columns: table => new
                {
                    CultureId = table.Column<int>(nullable: false),
                    Key = table.Column<string>(maxLength: 256, nullable: false),
                    Value = table.Column<string>(maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translation", x => new { x.CultureId, x.Key });
                });

            migrationBuilder.CreateTable(
                name: "UserInfo",
                columns: table => new
                {
                    UserId = table.Column<string>(maxLength: 450, nullable: false),
                    CountryId = table.Column<int>(nullable: false),
                    Height = table.Column<double>(nullable: false),
                    Sex = table.Column<int>(nullable: false),
                    TimeZoneName = table.Column<string>(maxLength: 100, nullable: true),
                    Unit = table.Column<int>(nullable: false),
                    Weight = table.Column<double>(nullable: false),
                    ZipCode = table.Column<string>(maxLength: 80, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfo", x => x.UserId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodyExercise");

            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropTable(
                name: "Muscle");

            migrationBuilder.DropTable(
                name: "MuscularGroup");

            migrationBuilder.DropTable(
                name: "TrainingDay");

            migrationBuilder.DropTable(
                name: "TrainingExercise");

            migrationBuilder.DropTable(
                name: "TrainingExerciseSet");

            migrationBuilder.DropTable(
                name: "TrainingWeek");

            migrationBuilder.DropTable(
                name: "Translation");

            migrationBuilder.DropTable(
                name: "UserInfo");
        }
    }
}
