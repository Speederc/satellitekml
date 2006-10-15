//
// Orbit.cs
//
// Copyright (c) 2003 Michael F. Henry
//
// mfh 12/23/2003
// 
using System;

namespace OrbitTools
{
    /// <summary>
    /// This class accepts a single satellite's NORAD two-line element
    /// set and provides information regarding the satellite's orbit 
    /// such as period, axis length, ECI coordinates/velocity, etc., using
    ///  the SGP4/SDP4 orbital models.
    /// </summary>
    public class Orbit
    {
        private Tle m_tle;
        private Julian m_jdEpoch;
        private NoradBase m_NoradModel;

        // Caching variables; note units are not necessarily the same as tle units
        private double m_secPeriod;

        // Caching variables recovered from the input TLE elements
        private double m_aeAxisSemiMinorRec;  // semi-minor axis, in AE units
        private double m_aeAxisSemiMajorRec;  // semi-major axis, in AE units
        private double m_mnMotionRec;         // radians per minute
        private double m_kmPerigeeRec;        // perigee, in km

        // ///////////////////////////////////////////////////////////////////
        public Orbit(Tle tle)
        {
            m_NoradModel = null;
            m_tle = tle;
            m_tle.Initialize();

            int epochYear = (int)m_tle.getField(Tle.eField.FLD_EPOCHYEAR);
            double epochDay = m_tle.getField(Tle.eField.FLD_EPOCHDAY);

            if (epochYear < 57)
                epochYear += 2000;
            else
                epochYear += 1900;

            m_jdEpoch = new Julian(epochYear, epochDay);

            m_secPeriod = -1.0;

            // Recover the original mean motion and semimajor axis from the
            // input elements.
            double mm = mnMotion();
            double rpmin = mm * 2 * Globals.PI / Globals.MIN_PER_DAY;   // rads per minute

            double a1 = Math.Pow(Globals.XKE / rpmin, Globals.TWOTHRD);
            double e = Eccentricity();
            double i = Inclination();
            double temp = (1.5 * Globals.CK2 * (3.0 * Globals.Sqr(Math.Cos(i)) - 1.0) /
                            Math.Pow(1.0 - e * e, 1.5));
            double delta1 = temp / (a1 * a1);
            double a0 = a1 *
                           (1.0 - delta1 *
                           ((1.0 / 3.0) + delta1 *
                           (1.0 + 134.0 / 81.0 * delta1)));

            double delta0 = temp / (a0 * a0);

            m_mnMotionRec = rpmin / (1.0 + delta0);
            m_aeAxisSemiMinorRec = a0 / (1.0 - delta0);
            m_aeAxisSemiMajorRec = m_aeAxisSemiMinorRec / Math.Sqrt(1.0 - (e * e));
            m_kmPerigeeRec = Globals.XKMPER * (m_aeAxisSemiMinorRec * (1.0 - e) - Globals.AE);

            if (2.0 * Globals.PI / m_mnMotionRec >= 225.0)
            {
                // SDP4 : period >= 225 minutes.
                m_NoradModel = new NoradSDP4(this);
            }
            else
            {
                // SGP4 : period < 225 minutes
                m_NoradModel = new NoradSGP4(this);
            }
        }

        public double Inclination() { return radGet(Tle.eField.FLD_I); }
        public double Eccentricity() { return m_tle.getField(Tle.eField.FLD_E); }
        public double RAAN() { return radGet(Tle.eField.FLD_RAAN); }
        public double ArgPerigee() { return radGet(Tle.eField.FLD_ARGPER); }
        public double BStar() { return m_tle.getField(Tle.eField.FLD_BSTAR) / Globals.AE; }
        public double Drag() { return m_tle.getField(Tle.eField.FLD_MMOTIONDT); }
        public double DragStar() { return m_tle.getField(Tle.eField.FLD_MMOTIONDT2); }
        public double mnMotion() { return m_tle.getField(Tle.eField.FLD_MMOTION); }
        public double mnAnomaly() { return radGet(Tle.eField.FLD_M); }

        public Tle GetTle { get { return m_tle; } }

        public Julian Epoch() { return m_jdEpoch; }
        public DateTime EpochTime() { return m_jdEpoch.toTime(); }

        // "Recovered" from the input elements
        public double SemiMajor() { return m_aeAxisSemiMajorRec; }
        public double SemiMinor() { return m_aeAxisSemiMinorRec; }
        public double mnMotionRec() { return m_mnMotionRec; }  // mn motion, rads/min
        public double Major() { return 2.0 * SemiMajor(); }  // major axis in AE
        public double Minor() { return 2.0 * SemiMinor(); }  // minor axis in AE
        public double Perigee() { return m_kmPerigeeRec; }  // perigee in km

        // ///////////////////////////////////////////////////////////////////////////
        // Return the period in seconds
        public double Period()
        {
            if (m_secPeriod < 0.0)
            {
                // Calculate the period using the recovered mean motion.
                if (m_mnMotionRec == 0)
                    m_secPeriod = 0.0;
                else
                    m_secPeriod = (2 * Globals.PI) / m_mnMotionRec * 60.0;
            }

            return m_secPeriod;
        }

        // //////////////////////////////////////////////////////////////////////////
        // getPosition()
        // This procedure returns the ECI position and velocity for the satellite
        // at "tsince" minutes from the (GMT) TLE epoch. The vectors returned in
        // the ECI object are kilometer-based.
        // tsince  - Time in minutes since the TLE epoch (GMT).
        public Eci getPosition(double tsince)
        {
            Eci eci = m_NoradModel.getPosition(tsince);

            eci.ae2km();

            return eci;
        }

        // ///////////////////////////////////////////////////////////////////////////
        // Return the name of the satellite.
        public string SatName()
        {
            return SatName(false);
        }

        // ///////////////////////////////////////////////////////////////////////////
        // SatName()
        // Return the name of the satellite. If requested, the NORAD number is
        // appended to the end of the name, i.e., "ISS (ZARYA) #25544".
        // The name of the satellite with the NORAD number appended is important
        // because many satellites, especially debris, have the same name and
        // would otherwise appear to be the same satellite in ouput data.
        public string SatName(bool fAppendId)
        {
            string str = m_tle.getName();

            if (fAppendId)
            {
                string strId = "";

                m_tle.getField(Tle.eField.FLD_NORADNUM,
                               Tle.eUnits.U_NATIVE,
                               ref strId,
                               true);

                str = str + " #" + strId;
            }

            return str;
        }

        // ///////////////////////////////////////////////////////////////////
        protected double radGet(Tle.eField fld)
        {
            string strTemp = null;
            return m_tle.getField(fld, Tle.eUnits.U_RAD, ref strTemp, false);
        }

        // ///////////////////////////////////////////////////////////////////
        protected double degGet(Tle.eField fld)
        {
            string strTemp = null;
            return m_tle.getField(fld, Tle.eUnits.U_DEG, ref strTemp, false);
        }
    }
}
