//
// Eci.cs
//
// 12/22/2003
//
// Copyright (c) 2003 Michael F. Henry
//
using System;

namespace OrbitTools
{
   /// <summary>
   /// Encapsulates an Earth-Centered Inertial coordinate position/velocity.
   /// </summary>
   public class Eci
   {
      public Eci() 
      { 
         m_VecUnits = VecUnits.UNITS_NONE;
      }

      public Eci(Vector pos, Vector vel, Julian date, bool IsAeUnits)
      {
         m_pos      = pos;
         m_vel      = vel;
         m_date     = date;
         m_VecUnits = (IsAeUnits ? VecUnits.UNITS_AE : VecUnits.UNITS_NONE);
      }

      protected enum VecUnits
      {
         UNITS_NONE, // not initialized
         UNITS_AE,
         UNITS_KM,
      };

      private Vector   m_pos;
      private Vector   m_vel;
      private Julian   m_date;
      private VecUnits m_VecUnits;

      // /////////////////////////////////////////////////////////////////////
      public Vector getPos() { return m_pos;  }
      public Vector getVel() { return m_vel;  }
      public Julian getDate(){ return m_date; }

      public void setUnitsAe() { m_VecUnits = VecUnits.UNITS_AE; }
      public void setUnitsKm() { m_VecUnits = VecUnits.UNITS_KM; }
      public bool UnitsAreAe() { return m_VecUnits == VecUnits.UNITS_AE; }
      public bool UnitsAreKm() { return m_VecUnits == VecUnits.UNITS_KM; }

      // ///////////////////////////////////////////////////////////////////
      // Calculate the ECI coordinates of the location "geo" at time "date".
      // Assumes geo coordinates are km-based.
      // Assumes the earth is an oblate spheroid as defined in WGS '72.
      // Reference: The 1992 Astronomical Almanac, page K11
      // Reference: www.celestrak.com (Dr. TS Kelso)
      public Eci(CoordGeo geo, Julian date)
      {
         m_VecUnits = VecUnits.UNITS_KM;

         double mfactor = Globals.TWOPI * (Globals.OMEGA_E / Globals.SEC_PER_DAY);
         double lat = geo.m_Lat;
         double lon = geo.m_Lon;
         double alt = geo.Altitude.Kilometers;

         // Calculate Local Mean Sidereal Time (theta)
         double theta = date.toLMST(lon);
         double c = 1.0 / Math.Sqrt(1.0 + Globals.F * (Globals.F - 2.0) * Globals.Sqr(Math.Sin(lat)));
         double s = Globals.Sqr(1.0 - Globals.F) * c;
         double achcp = (Globals.XKMPER * c + alt) * Math.Cos(lat);

         m_date = date;

         m_pos = new Vector();

         m_pos.X = achcp * Math.Cos(theta);                    // km
         m_pos.Y = achcp * Math.Sin(theta);                    // km
         m_pos.Z = (Globals.XKMPER * s + alt) * Math.Sin(lat); // km
         m_pos.W = Math.Sqrt(Globals.Sqr(m_pos.X) + 
                   Globals.Sqr(m_pos.Y) + 
                   Globals.Sqr(m_pos.Z));            // range, km

         m_vel = new Vector();

         m_vel.X = -mfactor * m_pos.Y;               // km / sec
         m_vel.Y =  mfactor * m_pos.X;
         m_vel.Z = 0.0;
         m_vel.W = Math.Sqrt(Globals.Sqr(m_vel.X) +  // range rate km/sec^2
                   Globals.Sqr(m_vel.Y));
      }

      // ///////////////////////////////////////////////////////////////////////////
      // Return the corresponding geodetic position (based on the current ECI
      // coordinates/Julian date).
      // Assumes the earth is an oblate spheroid as defined in WGS '72.
      // Side effects: Converts the position and velocity vectors to km-based units.
      // Reference: The 1992 Astronomical Almanac, page K12. 
      // Reference: www.celestrak.com (Dr. TS Kelso)
      public CoordGeo toGeo()
      {
         ae2km(); // Vectors must be in kilometer-based units

         double theta = Globals.AcTan(m_pos.Y, m_pos.X);
         double lon   = (theta - m_date.toGMST()) % Globals.TWOPI;
   
         if (lon < 0.0) 
            lon += Globals.TWOPI;  // "wrap" negative modulo

         double r   = Math.Sqrt(Globals.Sqr(m_pos.X) + Globals.Sqr(m_pos.Y));
         double e2  = Globals.F * (2.0 - Globals.F);
         double lat = Globals.AcTan(m_pos.Z, r);

         const double DELTA = 1.0e-07;
         double phi;
         double c;

         do   
         {
            phi = lat;
            c   = 1.0 / Math.Sqrt(1.0 - e2 * Globals.Sqr(Math.Sin(phi)));
            lat = Globals.AcTan(m_pos.Z + Globals.XKMPER * c * e2 * Math.Sin(phi), r);
         }
         while (Math.Abs(lat - phi) > DELTA);
   
         double alt = r / Math.Cos(lat) - Globals.XKMPER * c;

         return new CoordGeo(lat, lon, alt); // radians, radians, kilometers
      }

      // ///////////////////////////////////////////////////////////////////////////
      // Convert the position and velocity vector units from Globals.AE-based units
      // to kilometer based units.
      public void ae2km()
      {
         if (UnitsAreAe())
         {  
            MulPos(Globals.XKMPER / Globals.AE);                                    // km
            MulVel((Globals.XKMPER / Globals.AE) * (Globals.MIN_PER_DAY / 86400));  // km/sec
            m_VecUnits = VecUnits.UNITS_KM;
         }
      }

      protected void MulPos(double factor) 
      {
         m_pos.Mul(factor); 
      }

      protected void MulVel(double factor) 
      { 
         m_vel.Mul(factor); 
      }
   }
}
