//
// Coord.cs
//
// 12/22/2003
//
// Copyright (c) 2003 Michael F. Henry
//
using System;

namespace OrbitTools
{
   /// <summary>
   /// Geocentric coordinates
   /// </summary>
    public class CoordGeo
    {
        public CoordGeo()
        {
            m_Lat = 0.0;
            m_Lon = 0.0;
            m_Alt = new Distance(0.0);
        }

        public CoordGeo(double lat, double lon, double alt)
        {
            m_Lat = lat;
            m_Lon = lon;
            m_Alt = new Distance(alt, DistanceUnits.KILOMETERS);
        }
        public double LatRad
        {
            get { return this.m_Lat; }
        }
        public double LonRad
        {
            get { return this.m_Lon; }
        }
        public double LatDeg
        {
            get
            {
                return Globals.Rad2Deg(Globals.NormalizeLat(this.m_Lat));
            }
        }

        public double LonDeg
        {
            get
            {
                double ret = 0.0;
                ret = Globals.Rad2Deg(Globals.NormalizeLon(this.m_Lon));

                return ret;
            }
        }

        public Distance Altitude
        {
            get { return this.m_Alt; }
        }

        public static CoordGeo CalculatePointOnRhumbLine(CoordGeo point, double azimuth, double dist)
        {
            double lat1 = point.LatRad;// (double)com.bbn.openmap.geo.Geo.radians(point.getLatitude());
            double lon1 = point.LonRad;// (double)com.bbn.openmap.geo.Geo.radians(point.getLongitude());
            double d = dist / 1855.3 * Math.PI / 10800.0;
            double lat = 0.0;
            double lon = 0.0;
            lat = lat1 + d * Math.Cos(azimuth);
            double dphi = Math.Log((1 + Math.Sin(lat)) / Math.Cos(lat)) - Math.Log((1 + Math.Sin(lat1)) / Math.Cos(lat1));
            double dlon = 0.0;
            if (Math.Abs(Math.Cos(azimuth)) > Math.Sqrt(0.00000000000001))
            {
                dlon = dphi * Math.Tan(azimuth);
            }
            else
            { // along parallel
                dlon = Math.Sin(azimuth) * d / Math.Cos(lat1);
            }
            lon = Mod(lon1 - dlon + Math.PI, 2 * Math.PI) - Math.PI;
            //System.out.println("calculatePointOnRhumbLine:  lat1 = "+lat1+"+  lon1 = "+lon1 + "    lat = "+lat+"+  lon = "+lon);
            return new CoordGeo(lat, lon, 0);// LatLonPoint((float)lat, (float)lon, true);
        }

        /** Metoda pozwala znormowaæ wartoœæ do podanego zakresu
         * @param y normaowana wartoœæ
         * @param x zakres
         * @return normowana wartoœæ
         */
        private static double Mod(double y, double x)
        {
            double ret;
            if (y >= 0)
            {
                ret = y - x * (int)(y / x);
            }
            else
            {
                ret = y + x * ((int)(-y / x) + 1);
            }
            return ret;
        }


        public double m_Lat;   // Latitude,  radians (negative south)
        public double m_Lon;   // Longitude, radians (negative west)
        public Distance m_Alt;   // Altitude      (above mean sea level)


    }

   /// <summary>
   /// Topocentric-Horizon coordinates.
   /// </summary>
    public class CoordTopo
    {
        public CoordTopo()
        {
            m_Az = 0.0;
            m_El = 0.0;
            m_Range = 0.0;
            m_RangeRate = 0.0;
        }

        public CoordTopo(double az, double el, double rng, double rate)
        {
            m_Az = az;
            m_El = el;
            m_Range = rng;
            m_RangeRate = rate;
        }

        private double m_Az;         // Azimuth, radians
        private double m_El;         // Elevation, radians
        private double m_Range;      // Range, kilometers
        private double m_RangeRate;  // Range rate of change, km/sec
                                    // Negative value means "towards observer"
        public double Azimuth
        {
            get { return m_Az; }
            set { m_Az = value; }
        }
        public double Elevation
        {
            get { return m_El; }
            set { m_El = value; }
        }
        public double Range
        {
            get { return m_Range; }
            set { m_Range = value; }
        }
        public double RangeRate
        {
            get { return m_RangeRate; }
            set { m_RangeRate = value; }
        }
    }
}
