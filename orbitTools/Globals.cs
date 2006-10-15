//
// Globals.cs
//
// 12/22/2003
//
// Copyright (c) 2003 Michael F. Henry
//
using System;

namespace OrbitTools
{
    /// <summary>
    /// Summary description for Globals.
    /// </summary>
    abstract public class Globals
    {
        public const double PI = Math.PI;
        public const double TWOPI = 2.0 * Globals.PI;
        public const double RADS_PER_DEG = Globals.PI / 180.0;
        public const double DEG_PER_RAD = 180.0 / Globals.PI;

        public const double GM = 398601.2;   // Earth gravitational constant, km^3/sec^2
        public const double GEOSYNC_ALT = 42241.892;  // km
        public const double EARTH_RAD = 6370.0;     // km
        public const double EARTH_DIA = 12800.0;    // km
        public const double DAY_SIDERAL = (23 * 3600) + (56 * 60) + 4.09;  // sec
        public const double DAY_24HR = (24 * 3600);   // sec

        public const double AE = 1.0;
        public const double AU = 149597870.0;  // Astronomical unit (km) (IAU 76)
        public const double SR = 696000.0;     // Solar radius (km)      (IAU 76)
        public const double TWOTHRD = 2.0 / 3.0;
        public const double XKMPER = 6378.135;     // Earth equatorial radius - kilometers (WGS '72)
        public const double F = 1.0 / 298.26; // Earth flattening (WGS '72)
        public const double GE = 398600.8;     // Earth gravitational constant (WGS '72)
        public const double J2 = 1.0826158E-3; // J2 harmonic (WGS '72)
        public const double J3 = -2.53881E-6;  // J3 harmonic (WGS '72)
        public const double J4 = -1.65597E-6;  // J4 harmonic (WGS '72)
        public const double CK2 = J2 / 2.0;
        public const double CK4 = -3.0 * J4 / 8.0;
        public const double XJ3 = J3;
        public const double E6A = 1.0e-06;
        public const double QO = Globals.AE + 120.0 / Globals.XKMPER;
        public const double S = Globals.AE + 78.0 / Globals.XKMPER;
        public const double MIN_PER_DAY = 1440.0;        // Minutes per day (solar)
        public const double SEC_PER_DAY = 86400.0;       // seconds per day (solar)
        public const double OMEGA_E = 1.00273790934; // earth rotation per sideral day
        public static double XKE = Math.Sqrt(3600.0 * GE /
                                             (Globals.XKMPER * Globals.XKMPER * Globals.XKMPER)); //Math.Sqrt(ge) ER^3/min^2
        public static double QOMS2T = Math.Pow((QO - Globals.S), 4); //(QO - Globals.S)^4 ER^4

        // ///////////////////////////////////////////////////////////////////////////
        public static double Sqr(double x)
        {
            return (x * x);
        }

        // ///////////////////////////////////////////////////////////////////////////
        public static double Fmod2p(double arg)
        {
            double modu = (arg % TWOPI);

            if (modu < 0.0)
                modu += TWOPI;

            return modu;
        }

        // Get Lon Negative for West, Positive for East.  Input should be 0->2PI
        public static double NormalizeLon(double rads)
        {
            // East
            if (rads < Math.PI)
                return rads;

            // West
            double tmp = 0.0;
            if (rads > Math.PI)
            {
                tmp = rads - Math.PI;
                tmp = (-Math.PI) + tmp;
                return tmp;
            }

            return rads;
        }

        public static double NormalizeLat(double rads)
        {
            // must be -pi/2 -> pi/2
            while (rads < (-Globals.PI / 2.0))
            {
                rads += Globals.PI / 2.0;
            }

            while (rads > (Globals.PI / 2.0))
            {
                rads -= Globals.PI / 2.0;
            }

            return rads;
        }

        // ///////////////////////////////////////////////////////////////////////////
        // Globals.AcTan()
        // ArcTangent of sin(x) / cos(x). The advantage of this function over arctan()
        // is that it returns the correct quadrant of the angle.
        public static double AcTan(double sinx, double cosx)
        {
            double ret;

            if (cosx == 0.0)
            {
                if (sinx > 0.0)
                    ret = PI / 2.0;
                else
                    ret = 3.0 * PI / 2.0;
            }
            else
            {
                if (cosx > 0.0)
                    ret = Math.Atan(sinx / cosx);
                else
                    ret = PI + Math.Atan(sinx / cosx);
            }

            return ret;
        }

        // ///////////////////////////////////////////////////////////////////////////
        public static double Rad2Deg(double r)
        {
            //Globals.NormalizeRads(r);

            double degs = Globals.DEG_PER_RAD * r;
                       
            return degs;
        }

        // ////////////////////////////////////////////////////////////////////////////
        public static double Deg2Rad(double d)
        {
            double rads = Globals.RADS_PER_DEG * d;

            //rads = Globals.NormalizeRads(rads);

            return rads;
        }

        public static double NormalizeRads(double rads)
        {
            // 0 -> 2PI
            while (rads < 0)
            {
                rads += Globals.TWOPI;
            }

            while (rads > Globals.TWOPI)
            {
                rads -= Globals.TWOPI;
            }

            return rads;
        }

        public static Angle NormalizeElevation(Angle el)
        {
            double rads = el.Rads;

            rads = Globals.NormalizeRads(rads);

            if (rads > 3 * Globals.PI / 2.0)
            {
                rads = Globals.PI / 2.0 - (rads - 3 * Globals.PI / 2.0);
                rads = -rads;
            }
            else if (rads > Globals.PI)
            {
                rads -= Globals.PI;
                rads = -rads;
            }
            else if (rads > Globals.PI / 2.0)
            {
                rads = Globals.PI / 2.0 - (rads - Globals.PI / 2.0);
            }

            el.Rads = rads;

            return el;
        }

        public static DateTime InsureUTC(DateTime dt)
        {
            if (dt.Kind != DateTimeKind.Utc)
            {
                // Attempt to Fix the supplied DateTime to UTC...assuming it was local to start with.
                DateTime tmp = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, DateTimeKind.Local);
                tmp = tmp.ToUniversalTime();
                return tmp;
            }

            return dt;
        }

        public static Distance GreatCircleDistance(CoordGeo pt1, CoordGeo pt2)
        {
            double distance = Math.Acos((Math.Sin(pt1.LatRad) * Math.Sin(pt2.LatRad)) + (Math.Cos(pt1.LatRad) * Math.Cos(pt2.LatRad) * Math.Cos(pt2.LonRad - pt1.LonRad)));
            distance = distance * 3963.0;  // Statute Miles
            distance = distance * 1.609344; // to Km
            Distance d = new Distance(distance, DistanceUnits.KILOMETERS);
            return d;
        }
    }
}
