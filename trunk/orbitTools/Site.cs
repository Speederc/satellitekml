//
// Site.cs
//
// Copyright (c) 2003 Michael F. Henry
//
// mfh 12/24/2003
//
using System;

namespace OrbitTools
{
   /// <summary>
   /// The Site class encapsulates a location on earth.
   /// </summary>
    public class Site
    {
        public static Site WASH_DC = new Site(38.8919, -77.0338, .200);
        public static Site AUST_TX = new Site(30.266944, -97.742778 , .200);
        public static Site HOUS_TX = new Site(29.763056, -95.363056, .200);
        

        private CoordGeo m_geo;  // lat, lon, alt of earth site

        // //////////////////////////////////////////////////////////////////////////
        public Site(CoordGeo geo)
        {
            m_geo = geo;
        }

        // ///////////////////////////////////////////////////////////////////////////
        // c'tor accepting:
        //    Latitude  in degress (negative south)
        //    Longitude in degress (negative west)
        //    Altitude  in km
        public Site(double degLat, double degLon, double kmAlt)
        {
            m_geo =
               new CoordGeo(Globals.Deg2Rad(degLat), Globals.Deg2Rad(degLon), kmAlt);
        }

        public CoordGeo Position
        {
            get { return m_geo; }
            set { m_geo = value; }
        }

        public double Lat { get { return m_geo.m_Lat; } }
        public double Lon { get { return m_geo.m_Lon; } }
        public double Alt { get { return m_geo.Altitude.Kilometers; } }

        // ///////////////////////////////////////////////////////////////////////////
        // getPosition()
        // Return the ECI coordinate of the site at the given time.
        public Eci getPosition(Julian date)
        {
            return new Eci(m_geo, date);
        }

        // ///////////////////////////////////////////////////////////////////////////
        // getLookAngle()
        // Return the topocentric (azimuth, elevation, etc.) coordinates for a target
        // object described by the input ECI coordinates.
        public CoordTopo getLookAngle(Eci eci)
        {
            // Calculate the ECI coordinates for this Site object at the time
            // of interest.
            Julian date = eci.getDate();
            Eci eciSite = new Eci(m_geo, date);

            // The Site ECI units are km-based; ensure target ECI units are same
            if (!eci.UnitsAreKm())
                throw new Exception("ECI units must be kilometer-based");

            Vector vecRgRate = new Vector(eci.getVel().X - eciSite.getVel().X,
                                          eci.getVel().Y - eciSite.getVel().Y,
                                          eci.getVel().Z - eciSite.getVel().Z);

            double x = eci.getPos().X - eciSite.getPos().X;
            double y = eci.getPos().Y - eciSite.getPos().Y;
            double z = eci.getPos().Z - eciSite.getPos().Z;
            double w = Math.Sqrt(Globals.Sqr(x) + Globals.Sqr(y) + Globals.Sqr(z));

            Vector vecRange = new Vector(x, y, z, w);

            // The site's Local Mean Sidereal Time at the time of interest.
            double theta = date.toLMST(Lon);

            double sin_lat = Math.Sin(Lat);
            double cos_lat = Math.Cos(Lat);
            double sin_theta = Math.Sin(theta);
            double cos_theta = Math.Cos(theta);

            double top_s = sin_lat * cos_theta * vecRange.X +
                           sin_lat * sin_theta * vecRange.Y -
                           cos_lat * vecRange.Z;
            double top_e = -sin_theta * vecRange.X +
                            cos_theta * vecRange.Y;
            double top_z = cos_lat * cos_theta * vecRange.X +
                           cos_lat * sin_theta * vecRange.Y +
                           sin_lat * vecRange.Z;
            double az = Math.Atan(-top_e / top_s);

            if (top_s > 0.0)
                az += Globals.PI;

            if (az < 0.0)
                az += 2.0 * Globals.PI;

            double el = Math.Asin(top_z / vecRange.W);
            double rate = (vecRange.X * vecRgRate.X +
                           vecRange.Y * vecRgRate.Y +
                           vecRange.Z * vecRgRate.Z) / vecRange.W;

            CoordTopo topo = new CoordTopo(az,         // azimuth, radians
                                           el,         // elevation, radians
                                           vecRange.W, // range, km
                                           rate);      // rate, km / sec
#if WANT_ATMOSPHERIC_CORRECTION
      // Elevation correction for atmospheric refraction.
      // Reference:  Astronomical Algorithms by Jean Meeus, pp. 101-104
      // Note:  Correction is meaningless when apparent elevation is below horizon
      topo.m_El += Globals.Deg2Rad((1.02 / 
                                    Math.Tan(Globals.Deg2Rad(Globals.Rad2Deg(el) + 10.3 / 
                                    (Globals.Rad2Deg(el) + 5.11)))) / 60.0);
      if (topo.m_El < 0.0)
         topo.m_El = el;    // Reset to true elevation

      if (topo.m_El > (Globals.PI / 2))
         topo.m_El = (Globals.PI / 2);
#endif

            return topo;
        }

        // ///////////////////////////////////////////////////////////////////////////
        public string toString()
        {
            bool LatNorth = true;
            bool LonEast = true;

            if (m_geo.m_Lat < 0.0)
            {
                LatNorth = false;
            }

            if (m_geo.m_Lon < 0.0)
            {
                LonEast = false;
            }

            string str = Math.Abs(Globals.Rad2Deg(m_geo.m_Lat)).ToString("{0,6:f3} ");

            str += (LatNorth ? 'N' : 'S');

            str += Math.Abs(Globals.Rad2Deg(m_geo.m_Lon)).ToString("{0,6:f3}");
            str += (LonEast ? 'E' : 'W');

            str += (m_geo.Altitude.Kilometers * 1000.0).ToString();
            str += "m"; // meters

            return str;
        }

    }
}
