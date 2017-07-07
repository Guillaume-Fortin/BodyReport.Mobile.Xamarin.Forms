using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using BodyReportMobile.Core.Data;

namespace BodyReportMobile.Core.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20170706212402_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("BodyReportMobile.Core.Models.BodyExerciseRow", b =>
                {
                    b.Property<int>("Id");

                    b.Property<int?>("ExerciseCategoryType");

                    b.Property<int?>("ExerciseUnitType");

                    b.Property<int>("MuscleId");

                    b.HasKey("Id");

                    b.ToTable("BodyExercise");
                });

            modelBuilder.Entity("BodyReportMobile.Core.Models.CountryRow", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Name")
                        .HasMaxLength(400);

                    b.Property<string>("ShortName")
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.ToTable("Country");
                });

            modelBuilder.Entity("BodyReportMobile.Core.Models.MuscleRow", b =>
                {
                    b.Property<int>("Id");

                    b.Property<int>("MuscularGroupId");

                    b.HasKey("Id");

                    b.ToTable("Muscle");
                });

            modelBuilder.Entity("BodyReportMobile.Core.Models.MuscularGroupRow", b =>
                {
                    b.Property<int>("Id");

                    b.HasKey("Id");

                    b.ToTable("MuscularGroup");
                });

            modelBuilder.Entity("BodyReportMobile.Core.Models.TrainingDayRow", b =>
                {
                    b.Property<string>("UserId")
                        .HasMaxLength(450);

                    b.Property<int>("Year");

                    b.Property<int>("WeekOfYear");

                    b.Property<int>("DayOfWeek");

                    b.Property<int>("TrainingDayId");

                    b.Property<DateTime>("BeginHour")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<DateTime>("EndHour")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<DateTime>("ModificationDate")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<int?>("Unit");

                    b.HasKey("UserId", "Year", "WeekOfYear", "DayOfWeek", "TrainingDayId");

                    b.ToTable("TrainingDay");
                });

            modelBuilder.Entity("BodyReportMobile.Core.Models.TrainingExerciseRow", b =>
                {
                    b.Property<string>("UserId")
                        .HasMaxLength(450);

                    b.Property<int>("Year");

                    b.Property<int>("WeekOfYear");

                    b.Property<int>("DayOfWeek");

                    b.Property<int>("TrainingDayId");

                    b.Property<int>("Id");

                    b.Property<int>("BodyExerciseId");

                    b.Property<int?>("ConcentricContractionTempo");

                    b.Property<int?>("ContractedPositionTempo");

                    b.Property<int?>("EccentricContractionTempo");

                    b.Property<int?>("ExerciseUnitType");

                    b.Property<DateTime>("ModificationDate")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<int>("RestTime");

                    b.Property<int?>("StretchPositionTempo");

                    b.HasKey("UserId", "Year", "WeekOfYear", "DayOfWeek", "TrainingDayId", "Id");

                    b.ToTable("TrainingExercise");
                });

            modelBuilder.Entity("BodyReportMobile.Core.Models.TrainingExerciseSetRow", b =>
                {
                    b.Property<string>("UserId")
                        .HasMaxLength(450);

                    b.Property<int>("Year");

                    b.Property<int>("WeekOfYear");

                    b.Property<int>("DayOfWeek");

                    b.Property<int>("TrainingDayId");

                    b.Property<int>("TrainingExerciseId");

                    b.Property<int>("Id");

                    b.Property<int?>("ExecutionTime");

                    b.Property<DateTime>("ModificationDate")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<int>("NumberOfReps");

                    b.Property<int>("NumberOfSets");

                    b.Property<double>("Weight");

                    b.HasKey("UserId", "Year", "WeekOfYear", "DayOfWeek", "TrainingDayId", "TrainingExerciseId", "Id");

                    b.ToTable("TrainingExerciseSet");
                });

            modelBuilder.Entity("BodyReportMobile.Core.Models.TrainingWeekRow", b =>
                {
                    b.Property<string>("UserId")
                        .HasMaxLength(450);

                    b.Property<int>("Year");

                    b.Property<int>("WeekOfYear");

                    b.Property<DateTime>("ModificationDate")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<int>("Unit");

                    b.Property<double>("UserHeight");

                    b.Property<double>("UserWeight");

                    b.HasKey("UserId", "Year", "WeekOfYear");

                    b.ToTable("TrainingWeek");
                });

            modelBuilder.Entity("BodyReportMobile.Core.Models.TranslationRow", b =>
                {
                    b.Property<int>("CultureId");

                    b.Property<string>("Key")
                        .HasMaxLength(256);

                    b.Property<string>("Value")
                        .HasMaxLength(2000);

                    b.HasKey("CultureId", "Key");

                    b.ToTable("Translation");
                });

            modelBuilder.Entity("BodyReportMobile.Core.Models.UserInfoRow", b =>
                {
                    b.Property<string>("UserId")
                        .HasMaxLength(450);

                    b.Property<int>("CountryId");

                    b.Property<double>("Height");

                    b.Property<int>("Sex");

                    b.Property<string>("TimeZoneName")
                        .HasMaxLength(100);

                    b.Property<int>("Unit");

                    b.Property<double>("Weight");

                    b.Property<string>("ZipCode")
                        .HasMaxLength(80);

                    b.HasKey("UserId");

                    b.ToTable("UserInfo");
                });
        }
    }
}
