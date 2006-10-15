//
// Julian.cs
//
// This class encapsulates Julian dates with the epoch of 12:00 noon (12:00 UT)
// on January 1, 4713 B.C. Some epoch dates:
//    01/01/1990 00:00 UTC - 2447892.5
//    01/01/1990 12:00 UTC - 2447893.0
//    01/01/2000 00:00 UTC - 2451544.5
//    01/01/2001 00:00 UTC - 2451910.5
//
// Note the Julian day begins at noon, which allows astronomers to have the
// same date in a single observing session.
//
// References:
// "Astronomical Formulae for Calculators", Jean Meeus
// "Satellite Communications", Dennis Roddy, 2nd Edition, 1995.
//
// Copyright (c) 2003 Michael F. Henry
//
// mfh 12/24/2003
//
using System;

namespace OrbitTools
{
	/// <summary>
	/// Encapsulates a Julian date.
	/// </summary>
    public class Julian
    {
        private DateTime _construct;

        const double EPOCH_JAN1_00H_1900 = 2415019.5; // Jan 1.0 1900 = Jan 1 1900 00h UTC
        const double EPOCH_JAN1_12H_1900 = 2415020.0; // Jan 1.5 1900 = Jan 1 1900 12h UTC
        const double EPOCH_JAN1_12H_2000 = 2451545.0; // Jan 1.5 2000 = Jan 1 2000 12h UTC

        // /////////////////////////////////////////////////////////////////////
        // Create a Julian date object from a DateTime object. The time
        // contained in the DateTime object is assumed to be UTC.
        public Julian(DateTime dt)
        {
            this._construct = dt;
            if (dt.Kind != DateTimeKind.Utc)
                dt = dt.ToUniversalTime();

            double day =
               (dt.DayOfYear) + (dt.Hour / 24.0) + (dt.Minute / (60.0 * 24.0)) + (dt.Second / (60.0 * 60.0 * 24.0)) + (dt.Millisecond / (60.0 * 60.0 * 24.0 * 1000.0));
            Initialize(dt.Year, day);
        }

        // /////////////////////////////////////////////////////////////////////
        // Create a Julian date object from a year and day of year.
        // Example parameters: year = 2001, day = 1.5 (Jan 1 12h)
        public Julian(int year, double day)
        {
            Initialize(year, day);
        }

        public double FromJan1_00h_1900() { return m_Date - EPOCH_JAN1_00H_1900; }
        public double FromJan1_12h_1900() { return m_Date - EPOCH_JAN1_12H_1900; }
        public double FromJan1_12h_2000() { return m_Date - EPOCH_JAN1_12H_2000; }

        private double m_Date; // Julian date
        private int m_Year; // Year including century
        private double m_Day;  // Day of year, 0.0 = Jan 1 00h

        public double Date { get { return m_Date; } }

        // /////////////////////////////////////////////////////////////////////
        // Initialize the Julian object
        protected void Initialize(int year, double day)
        {
            // 1582 A.D.: 10 days removed from calendar
            // 3000 A.D.: Arbitrary error checking limit
            if ((year <= 1582) || (year > 3000) ||
               (day < 0.0) || (day > 366.5))
            {
                throw new Exception("Date out of range");
            }

            m_Year = year;
            m_Day = day;

            // Now calculate Julian date
            // Ref: "Astronomical Formulae for Calculators", Jean Meeus, pages 23-25

            year--;

            // Centuries are not leap years unless they divide by 400
            int A = (year / 100);
            int B = 2 - A + (A / 4);

            double NewYears = (int)(365.25 * year) +
                              (int)(30.6001 * 14) +
                              1720994.5 + B;           // 1720994 = Jan 01 year 0

            m_Date = NewYears + day;
        }

        // /////////////////////////////////////////////////////////////////////
        // toGMST()
        // Calculate Greenwich Mean Sidereal Time for the Julian date. The 
        // return value is the angle, in radians, measuring eastward from the
        // Vernal Equinox to the prime meridian. This angle is also referred
        // to as "ThetaG" (Theta GMST).
        // 
        // References:
        //    The 1992 Astronomical Almanac, page B6.
        //    Explanatory Supplement to the Astronomical Almanac, page 50.
        //    Orbital Coordinate Systems, Part III, Dr. T.S. Kelso, 
        //       Satellite Times, Nov/Dec 1995
        public double toGMST()
        {
            double UT = (m_Date + 0.5) % 1.0;
            double TU = (FromJan1_12h_2000() - UT) / 36525.0;

            double GMST = 24110.54841 + TU *
                          (8640184.812866 + TU * (0.093104 - TU * 6.2e-06));

            GMST = (GMST + Globals.SEC_PER_DAY * Globals.OMEGA_E * UT) % Globals.SEC_PER_DAY;

            if (GMST < 0.0)
                GMST += Globals.SEC_PER_DAY;  // "wrap" negative modulo value

            return (Globals.TWOPI * (GMST / Globals.SEC_PER_DAY));
        }

        // /////////////////////////////////////////////////////////////////////
        // toLMST()
        // Calculate Local Mean Sidereal Time for given longitude (for this date).
        // The longitude is assumed to be in radians measured west from Greenwich.
        // The return value is the angle, in radians, measuring eastward from the
        // Vernal Equinox to the given longitude.
        public double toLMST(double lon)
        {
            return (toGMST() + lon) % Globals.TWOPI;
        }

        // /////////////////////////////////////////////////////////////////////
        // toTime()
        // Convert to type DateTime.
        public DateTime toTime1()
        {
            DateTime dt = new DateTime(1, 1, 1);//, 0, 0, 0, DateTimeKind.Utc);
            dt = dt.AddYears(m_Year - 1);
            dt = dt.AddDays(m_Date);
            //dt = dt.AddYears(-1);

            // Jan 1
            //DateTime dt = new DateTime(m_Year, 1, 1);//, 0, 0, 0, DateTimeKind.Utc);

            //// m_Day = 1 = Jan1
            dt = dt.AddDays(m_Day - 1.0);

            return dt;
        }

        public DateTime toTime()
        {
            DateTime dt = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dt = dt.AddYears(this.m_Year - 1);

            dt = dt.AddDays(m_Day - 1.0);

            return dt;
        }

        /// <summary>
        /// Convert a System.DateTime to a Julian Date
        /// </summary>
        /// <param name="dt">a System.DateTime with Kind = DateTimeKind.Utc</param>
        /// <returns>the Julian Date</returns>
        public double DateTimeToJT(DateTime dt)
        {
            double ret = this.JulianDateOfYear(dt.Year);
            ret += this.DOY(dt.Year, dt.Month, dt.Day);
            ret += this.FractionOfDay(dt.Hour, dt.Minute, dt.Second, dt.Millisecond);

            return ret;
        }

        private double JulianDateOfYear(int year)
        {
            int yearTmp = year - 1;
            int A = yearTmp / 100;
            int B = 2 - A + (A / 4);

            double NewYears = (int)(365.25 * year) +
                              (int)(30.6001 * 14) +
                              1720994.5 + B;           // 1720994 = Jan 01 year 0

            return NewYears;
        }

        private int DOY(int year, int month, int dy)
        {
            int[] days = new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            int day = 0;
            for (int i = 1; i < month; i++)
            {
                day = day + days[i - 1];
            }
            day += dy;
            if ((year % 4 == 0 && year % 100 != 0) || (year % 400 == 0 && month > 2))
            {
                day += 1;
            }

            return day;
        }

        private double FractionOfDay(int hr, int min, int sec, int ms)
        {
            double ret = hr / 24.0;
            ret += min / 24.0 / 60.0;
            ret += sec / 24.0 / 60.0 / 60.0;
            ret += ms / 24.0 / 60.0 / 60.0 / 1000.0;

            return ret;
        }
    }
}
