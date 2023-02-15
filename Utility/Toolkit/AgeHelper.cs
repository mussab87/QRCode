using System;
using Utility.Core.Logging;

namespace Utility.Toolkit
{
    /**
     * Usage example:
     * ==============
     * DateTime bday = new DateTime(1987, 11, 27);
     * DateTime cday = DateTime.Today;
     * AgeHelper age = new AgeHelper(bday, cday);
     * Console.WriteLine("It's been {0} years, {1} months, and {2} days since your birthday", age.Year, age.Month, age.Day);
    */
    public class AgeHelper
    {
        public int Years;
        public int Months;
        public int Days;

        public AgeHelper(DateTime Bday)
        {
            this.Count(Bday);
        }

        public AgeHelper(DateTime Bday, DateTime Cday)
        {
            this.Count(Bday, Cday);
        }

        public AgeHelper Count(DateTime Bday)
        {
            return this.Count(Bday, DateTime.Today);
        }

        public AgeHelper Count(DateTime Bday, DateTime Cday)
        {
            if ((Cday.Year - Bday.Year) > 0 ||
                (((Cday.Year - Bday.Year) == 0) && ((Bday.Month < Cday.Month) ||
                  ((Bday.Month == Cday.Month) && (Bday.Day <= Cday.Day)))))
            {
                int DaysInBdayMonth = DateTime.DaysInMonth(Bday.Year, Bday.Month);
                int DaysRemain = Cday.Day + (DaysInBdayMonth - Bday.Day);

                if (Cday.Month > Bday.Month)
                {
                    this.Years = Cday.Year - Bday.Year;
                    this.Months = Cday.Month - (Bday.Month + 1) + Math.Abs(DaysRemain / DaysInBdayMonth);
                    this.Days = (DaysRemain % DaysInBdayMonth + DaysInBdayMonth) % DaysInBdayMonth;
                }
                else if (Cday.Month == Bday.Month)
                {
                    if (Cday.Day >= Bday.Day)
                    {
                        this.Years = Cday.Year - Bday.Year;
                        this.Months = 0;
                        this.Days = Cday.Day - Bday.Day;
                    }
                    else
                    {
                        this.Years = (Cday.Year - 1) - Bday.Year;
                        this.Months = 11;
                        this.Days = DateTime.DaysInMonth(Bday.Year, Bday.Month) - (Bday.Day - Cday.Day);
                    }
                }
                else
                {
                    this.Years = (Cday.Year - 1) - Bday.Year;
                    this.Months = Cday.Month + (11 - Bday.Month) + Math.Abs(DaysRemain / DaysInBdayMonth);
                    this.Days = (DaysRemain % DaysInBdayMonth + DaysInBdayMonth) % DaysInBdayMonth;
                }
            }
            else
            {
                throw new ArgumentException("Birthday date must be earlier than current date");
            }
            return this;
        }

        public static PersonAge CalculateAge(DateTime birthDate)
        {
            FileTrace.WriteMemberEntry();

            PersonAge personAge = null;
            try
            {
                var ageVal = 0;
                var ageUnitType = string.Empty;

                var age = new AgeHelper(birthDate);
                if (age.Years == 0)
                {
                    // Person is less than a year old
                    // or Person is weeks old.
                    ageVal = age.Days;
                    ageUnitType = PersonAge.C_AGE_UNIT_DAYS;
                }
                else
                {
                    ageVal = age.Years;
                    ageUnitType = PersonAge.C_AGE_UNIT_YEARS;
                }

                personAge = new PersonAge()
                {
                    Age = ageVal,
                    AgeUnit = ageUnitType,
                };
            }
            catch (Exception ex)
            {
                FileTrace.WriteException(ex);
                throw;
            }
            FileTrace.WriteMemberExit();
            return personAge;
        }
    }

    public class PersonAge
    {
        public static readonly string C_AGE_UNIT_YEARS = "year";
        public static readonly string C_AGE_UNIT_DAYS = "day";

        public int? Age { get; set; }
        public string AgeUnit { get; set; }
    }
}