//
// Tle.cs
// 
// 12/23/2003
//
// Copyright (c) 2003 Michael F. Henry
//
using System;
using System.Collections;

// ////////////////////////////////////////////////////////////////////////
//
// NASA Two-Line Element Data format
//
// [Reference: T.S. Kelso / www.celestrak.com]
//
// Two line element data consists of three lines in the following format:
//
//  AAAAAAAAAAAAAAAAAAAAAA
//  1 NNNNNU NNNNNAAA NNNNN.NNNNNNNN +.NNNNNNNN +NNNNN-N +NNNNN-N N NNNNN
//  2 NNNNN NNN.NNNN NNN.NNNN NNNNNNN NNN.NNNN NNN.NNNN NN.NNNNNNNNNNNNNN
//  
//  Line 0 is a twenty-two-character name.
// 
//   Lines 1 and 2 are the standard Two-Line Orbital Element Set Format identical
//   to that used by NORAD and NASA.  The format description is:
//      
//     Line 1
//     Column    Description
//     01-01     Line Number of Element Data
//     03-07     Satellite Number
//     10-11     International Designator (Last two digits of launch year)
//     12-14     International Designator (Launch number of the year)
//     15-17     International Designator (Piece of launch)
//     19-20     Epoch Year (Last two digits of year)
//     21-32     Epoch (Julian Day and fractional portion of the day)
//     34-43     First Time Derivative of the Mean Motion
//               or Ballistic Coefficient (Depending on ephemeris type)
//     45-52     Second Time Derivative of Mean Motion (decimal point assumed;
//               blank if N/A)
//     54-61     BSTAR drag term if GP4 general perturbation theory was used.
//               Otherwise, radiation pressure coefficient.  (Decimal point assumed)
//     63-63     Ephemeris type
//     65-68     Element number
//     69-69     Check Sum (Modulo 10)
//               (Letters, blanks, periods, plus signs = 0; minus signs = 1)
//
//     Line 2
//     Column    Description
//     01-01     Line Number of Element Data
//     03-07     Satellite Number
//     09-16     Inclination [Degrees]
//     18-25     Right Ascension of the Ascending Node [Degrees]
//     27-33     Eccentricity (decimal point assumed)
//     35-42     Argument of Perigee [Degrees]
//     44-51     Mean Anomaly [Degrees]
//     53-63     Mean Motion [Revs per day]
//     64-68     Revolution number at epoch [Revs]
//     69-69     Check Sum (Modulo 10)
//        
//     All other columns are blank or fixed.
//          
// Example:
//      
// NOAA 6
// 1 11416U          86 50.28438588 0.00000140           67960-4 0  5293
// 2 11416  98.5105  69.3305 0012788  63.2828 296.9658 14.24899292346978


namespace OrbitTools
{
    /// <summary>
    /// This class encapsulates a single set of standard NORAD two-line elements.
    /// </summary>
    public class Tle
    {
        // Note: The column offsets are ZERO based.

        // Name
        public const int TLE_LEN_LINE_DATA = 69; public const int TLE_LEN_LINE_NAME = 22;

        // Line 1
        public const int TLE1_COL_SATNUM = 2; public const int TLE1_LEN_SATNUM = 5;
        public const int TLE1_COL_INTLDESC_A = 9; public const int TLE1_LEN_INTLDESC_A = 2;
        public const int TLE1_COL_INTLDESC_B = 11; public const int TLE1_LEN_INTLDESC_B = 3;
        public const int TLE1_COL_INTLDESC_C = 14; public const int TLE1_LEN_INTLDESC_C = 3;
        public const int TLE1_COL_EPOCH_A = 18; public const int TLE1_LEN_EPOCH_A = 2;
        public const int TLE1_COL_EPOCH_B = 20; public const int TLE1_LEN_EPOCH_B = 12;
        public const int TLE1_COL_MEANMOTIONDT = 33; public const int TLE1_LEN_MEANMOTIONDT = 10;
        public const int TLE1_COL_MEANMOTIONDT2 = 44; public const int TLE1_LEN_MEANMOTIONDT2 = 8;
        public const int TLE1_COL_BSTAR = 53; public const int TLE1_LEN_BSTAR = 8;
        public const int TLE1_COL_EPHEMTYPE = 62; public const int TLE1_LEN_EPHEMTYPE = 1;
        public const int TLE1_COL_ELNUM = 64; public const int TLE1_LEN_ELNUM = 4;

        // Line 2
        public const int TLE2_COL_SATNUM = 2; public const int TLE2_LEN_SATNUM = 5;
        public const int TLE2_COL_INCLINATION = 8; public const int TLE2_LEN_INCLINATION = 8;
        public const int TLE2_COL_RAASCENDNODE = 17; public const int TLE2_LEN_RAASCENDNODE = 8;
        public const int TLE2_COL_ECCENTRICITY = 26; public const int TLE2_LEN_ECCENTRICITY = 7;
        public const int TLE2_COL_ARGPERIGEE = 34; public const int TLE2_LEN_ARGPERIGEE = 8;
        public const int TLE2_COL_MEANANOMALY = 43; public const int TLE2_LEN_MEANANOMALY = 8;
        public const int TLE2_COL_MEANMOTION = 52; public const int TLE2_LEN_MEANMOTION = 11;
        public const int TLE2_COL_REVATEPOCH = 63; public const int TLE2_LEN_REVATEPOCH = 5;

        public enum eTleLine
        {
            LINE_ZERO = 0,
            LINE_ONE,
            LINE_TWO
        };

        public enum eField
        {
            FLD_FIRST,
            FLD_NORADNUM = eField.FLD_FIRST,
            FLD_INTLDESC,
            FLD_SET,       // TLE set number
            FLD_EPOCHYEAR, // Epoch: Last two digits of year
            FLD_EPOCHDAY,  // Epoch: Fractional Julian Day of year
            FLD_ORBITNUM,  // Orbit at epoch
            FLD_I,         // Inclination
            FLD_RAAN,      // R.A. ascending node
            FLD_E,         // Eccentricity
            FLD_ARGPER,    // Argument of perigee
            FLD_M,         // Mean anomaly
            FLD_MMOTION,   // Mean motion
            FLD_MMOTIONDT, // First time derivative of mean motion
            FLD_MMOTIONDT2,// Second time derivative of mean motion
            FLD_BSTAR,     // BSTAR Drag
            FLD_LAST       // MUST be last
        }

        public enum eUnits
        {
            U_FIRST,
            U_RAD = U_FIRST,  // radians
            U_DEG,            // degrees
            U_NATIVE,         // TLE format native units (no conversion)
            U_LAST            // MUST be last
        }

        // Satellite name and two data lines
        private string m_strName;
        private string m_strLine1;
        private string m_strLine2;

        // Converted fields, in Double.Parse()-able form
        private Hashtable m_Field;

        // Cache of field values in "double" format. 
        // Key   - integer
        // Value - cached value
        private Hashtable m_Cache;

        // Generates key for cache
        private int Key(eUnits u, eField f)
        {
            return ((int)u * 100) + (int)f;
        }

        // //////////////////////////////////////////////////////////////////////////
        public Tle(string strName, string strLine1, string strLine2)
        {
            m_strName = strName;
            m_strLine1 = strLine1;
            m_strLine2 = strLine2;

            Initialize();
        }

        // //////////////////////////////////////////////////////////////////////////
        public Tle(Tle tle)
        {
            m_strName = tle.m_strName;
            m_strLine1 = tle.m_strLine1;
            m_strLine2 = tle.m_strLine2;

            Initialize();
        }

        public string getName() { return m_strName; }
        public string getLine1() { return m_strLine1; }
        public string getLine2() { return m_strLine2; }

        /// <summary>
        /// Returns the requested TLE data field.
        /// </summary>
        /// <param name="fld">The field to return</param>
        /// <returns>The requested field, in native format</returns>
        public double getField(eField fld)
        {
            string strTemp = null;

            return getField(fld, eUnits.U_NATIVE, ref strTemp, false);
        }

        // /////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns the requested TLE data field as a double or 
        /// as a text string in the units requested. 
        /// </summary>
        /// <remarks>
        /// The numeric return values are cached; requesting the same field 
        /// repeatedly incurs minimal overhead.
        /// </remarks>
        /// <param name="fld">The TLE field to retrieve</param>
        /// <param name="units">Specifies the units desired</param>
        /// <param name="str">If non-null, the field is returned as
        /// a string.</param>
        /// <param name="AppendUnits">
        /// If true, units are appended to text string.
        /// </param>
        /// <returns>
        /// The requested field, converted to the requested units if necessary.
        /// </returns>
        public double getField(eField fld,
                               eUnits units,
                               ref string str,
                               bool AppendUnits)
        {
            if (str != null)
            {
                // Return requested field in string form.
                str = m_Field[fld].ToString();

                if (AppendUnits)
                    str += getUnits(fld);

                return 0.0;
            }
            else
            {
                // Return requested field in floating-point form.
                // Return cache contents if it exists, else populate cache.
                int key = Key(units, fld);

                if (m_Cache.ContainsKey(key))
                {
                    // return cached value
                    return (double)m_Cache[key];
                }
                else
                {
                    // Value not in cache; add it
                    double valNative = Double.Parse(m_Field[fld].ToString());
                    double valConv = ConvertUnits(valNative, fld, units);
                    m_Cache[key] = valConv;

                    return valConv;
                }
            }
        }

        // ///////////////////////////////////////////////////////////////////////////
        // Convert the given field into the requested units. It is assumed that
        // the value being converted is in the TLE format's "native" form.
        protected double ConvertUnits(double valNative, // value to convert
                                      eField fld,       // what field the value is
                                      eUnits units)     // what units to convert to
        {
            if (fld == eField.FLD_I ||
                fld == eField.FLD_RAAN ||
                fld == eField.FLD_ARGPER ||
                fld == eField.FLD_M)
            {
                // The native TLE format is DEGREES
                if (units == eUnits.U_RAD)
                    return valNative * Globals.RADS_PER_DEG;
            }

            return valNative; // return value in unconverted native format
        }

        // ///////////////////////////////////////////////////////////////////////////
        protected string getUnits(eField fld)
        {
            const string strDegrees = " degrees";
            const string strRevsPerDay = " revs / day";

            switch (fld)
            {
                case eField.FLD_I:
                case eField.FLD_RAAN:
                case eField.FLD_ARGPER:
                case eField.FLD_M:
                    return strDegrees;

                case eField.FLD_MMOTION:
                    return strRevsPerDay;

                default:
                    return string.Empty;
            }
        }

        // //////////////////////////////////////////////////////////////////////////
        // ExpToDecimal()
        // Converts TLE-style exponential notation of the form [ |-]00000[+|-]0 to
        // decimal notation. Assumes implied decimal point to the left of the first
        // number in the string, i.e., 
        //       " 12345-3" =  0.00012345
        //       "-23429-5" = -0.0000023429   
        //       " 40436+1" =  4.0436
        static string ExpToDecimal(string str)
        {
            const int COL_MANTISSA = 0;
            const int LEN_MANTISSA = 6;

            const int COL_EXPONENT = 6;
            const int LEN_EXPONENT = 2;

            // Mantissa
            double dblMan = Double.Parse(str.Substring(COL_MANTISSA, LEN_MANTISSA));
            bool bNeg = (dblMan < 0.0);

            if (bNeg)
            {
                // Make value positive for now
                dblMan *= -1;
            }

            // Move decimal place to left of first digit
            while (dblMan >= 1.0)
                dblMan /= 10.0;

            if (bNeg)
            {
                // Reapply negative sign
                dblMan *= -1;
            }

            // Exponent
            int nExp = Int32.Parse(str.Substring(COL_EXPONENT, LEN_EXPONENT));
            double dblVal = dblMan * Math.Pow(10, nExp);

            return dblVal.ToString("f9");
        }

        // //////////////////////////////////////////////////////////////////////////
        // Initialize()
        // Initialize the TLE object.
        public void Initialize()
        {
            // Have we already been initialized?
            if (m_Field != null)
                return;

            m_Field = new Hashtable();
            m_Cache = new Hashtable();

            m_Field[eField.FLD_NORADNUM] = m_strLine1.Substring(TLE1_COL_SATNUM, TLE1_LEN_SATNUM);
            m_Field[eField.FLD_INTLDESC] = m_strLine1.Substring(TLE1_COL_INTLDESC_A,
                                                                TLE1_LEN_INTLDESC_A +
                                                                TLE1_LEN_INTLDESC_B +
                                                                TLE1_LEN_INTLDESC_C);
            m_Field[eField.FLD_EPOCHYEAR] =
               m_strLine1.Substring(TLE1_COL_EPOCH_A, TLE1_LEN_EPOCH_A);

            m_Field[eField.FLD_EPOCHDAY] =
               m_strLine1.Substring(TLE1_COL_EPOCH_B, TLE1_LEN_EPOCH_B);

            if (m_strLine1[TLE1_COL_MEANMOTIONDT] == '-')
            {
                // value is negative
                m_Field[eField.FLD_MMOTIONDT] = "-0";
            }
            else
                m_Field[eField.FLD_MMOTIONDT] = "0";

            m_Field[eField.FLD_MMOTIONDT] += m_strLine1.Substring(TLE1_COL_MEANMOTIONDT + 1,
               TLE1_LEN_MEANMOTIONDT);

            // decimal point assumed; exponential notation
            m_Field[eField.FLD_MMOTIONDT2] =
               ExpToDecimal(m_strLine1.Substring(TLE1_COL_MEANMOTIONDT2,
                                                 TLE1_LEN_MEANMOTIONDT2));

            // decimal point assumed; exponential notation
            m_Field[eField.FLD_BSTAR] =
               ExpToDecimal(m_strLine1.Substring(TLE1_COL_BSTAR, TLE1_LEN_BSTAR));
            //TLE1_COL_EPHEMTYPE      
            //TLE1_LEN_EPHEMTYPE   

            m_Field[eField.FLD_SET] =
               m_strLine1.Substring(TLE1_COL_ELNUM, TLE1_LEN_ELNUM).TrimStart();

            // TLE2_COL_SATNUM         
            // TLE2_LEN_SATNUM         

            m_Field[eField.FLD_I] =
               m_strLine2.Substring(TLE2_COL_INCLINATION, TLE2_LEN_INCLINATION).TrimStart();

            m_Field[eField.FLD_RAAN] =
               m_strLine2.Substring(TLE2_COL_RAASCENDNODE, TLE2_LEN_RAASCENDNODE).TrimStart();

            // Eccentricity: decimal point is assumed
            m_Field[eField.FLD_E] = "0." + m_strLine2.Substring(TLE2_COL_ECCENTRICITY,
                                                                TLE2_LEN_ECCENTRICITY);

            m_Field[eField.FLD_ARGPER] =
               m_strLine2.Substring(TLE2_COL_ARGPERIGEE, TLE2_LEN_ARGPERIGEE).TrimStart();

            m_Field[eField.FLD_M] =
               m_strLine2.Substring(TLE2_COL_MEANANOMALY, TLE2_LEN_MEANANOMALY).TrimStart();

            m_Field[eField.FLD_MMOTION] =
               m_strLine2.Substring(TLE2_COL_MEANMOTION, TLE2_LEN_MEANMOTION).TrimStart();

            m_Field[eField.FLD_ORBITNUM] =
               m_strLine2.Substring(TLE2_COL_REVATEPOCH, TLE2_LEN_REVATEPOCH).TrimStart();
        }

        // //////////////////////////////////////////////////////////////////////////
        // IsTleFormat()
        // Returns true if "str" is a valid data line of a two-line element set,
        //   else false.
        //
        // To be valid a line must:
        //      Have as the first character the line number
        //      Have as the second character a blank
        //      Be TLE_LEN_LINE_DATA characters long
        //      Have a valid checksum (note: no longer required as of 12/96)
        //      
        static bool IsValidLine(string str, eTleLine line)
        {
            str.TrimStart();
            str.TrimEnd();

            int nLen = str.Length;

            if (nLen != TLE_LEN_LINE_DATA)
                return false;

            // First character in string must be a line number
            if ((str[0] - '0') != (int)line)
                return false;

            // Second char in string must be blank
            if (str[1] != ' ')
                return false;

            return true;
        }

        // //////////////////////////////////////////////////////////////////////////
        // CheckSum()
        // Calculate the check sum for a given line of TLE data, the last character
        // of which is the current checksum. (Although there is no check here,
        // the current checksum should match the one calculated.)
        // The checksum algorithm: 
        //    Each number in the data line is summed, modulo 10.
        //    Non-numeric characters are zero, except minus signs, which are 1.
        //
        static int CheckSum(string str)
        {
            // The length is "- 1" because we don't include the current (existing)
            // checksum character in the checksum calculation.
            int len = str.Length - 1;
            int xsum = 0;

            for (int i = 0; i < len; i++)
            {
                char ch = str[i];

                if (Char.IsDigit(ch))
                    xsum += (ch - '0');
                else if (ch == '-')
                    xsum++;
            }

            return (xsum % 10);

        } // CheckSum()
    }
}
