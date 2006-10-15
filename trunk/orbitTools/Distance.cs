using System;
using System.Collections.Generic;
using System.Text;

namespace OrbitTools
{
    public enum DistanceUnits
    {
        METERS = 0,
        KILOMETERS,
        AE,
        AU
    }

    public class Distance
    {
        public static double METERS_TO_KILOMETERS = 1.0 / 1000.0;
        public static double METERS_TO_AU = 1.0 / 149597900000.0;
        public static double METERS_TO_AE = 1000.0 * Globals.XKMPER / 1.0;
        public static double KILOMETERS_TO_METERS = 1000;
        public static double KILOMETERS_TO_AU = 1000.0 / 149597900000.0;
        public static double KILOMETERS_TO_AE = Globals.XKMPER / 1.0;
        public static double AU_TO_METERS = 149597900000.0;
        public static double AU_TO_KILOMETERS = 149597900.0;
        public static double AU_TO_AE = 149597900.0 / Globals.XKMPER;
        public static double AE_TO_METERS = Globals.XKMPER * 1000.0;
        public static double AE_TO_KILOMETERS = Globals.XKMPER * 1.0;
        public static double AE_TO_AU = Globals.XKMPER * 1000.0 / 149597900000.0;

        private double _value;

        /// <summary>
        /// Create a Default Distance with DistanceUnits = METERS
        /// </summary>
        /// <param name="value">the value to set this distance to</param>
        public Distance(double value)
        {
            this._value = value;
        }

        public Distance(double value, DistanceUnits type)
        {
            switch (type)
            {
                case DistanceUnits.KILOMETERS:
                    this._value = value * Distance.KILOMETERS_TO_METERS;
                    break;
                case DistanceUnits.AU:
                    this._value = value * Distance.AU_TO_METERS;
                    break;
                case DistanceUnits.AE:
                    this._value = value * Distance.AE_TO_METERS;
                    break;
                default:
                    this._value = value;
                    break;
            }
        }

        public double Meters
        {
            get
            {
                return this._value;
            }
        }

        public double Kilometers
        {
            get
            {
                return this._value * Distance.METERS_TO_KILOMETERS;
            }
        }

        public double AU
        {
            get
            {
                return this._value * Distance.METERS_TO_AU;
            }
        }

        public double AE
        {
            get
            {
                return this._value * Distance.METERS_TO_AE;
            }
        }

        public string ToString(DistanceUnits units)
        {
            double val = 0.0;
            string suffix = "";
            switch (units)
            {
                case DistanceUnits.AE:
                    val = this.AE;
                    suffix = " AE";
                    break;
                case DistanceUnits.AU:
                    val = this.AU;
                    suffix = " AU";
                    break;
                case DistanceUnits.KILOMETERS:
                    val = this.Kilometers;
                    suffix = " Km";
                    break;
                default:
                    val = this.Meters;
                    suffix = " m";
                    break;
            }

            string s = val.ToString("#.00000000") + suffix;
            
            return s;
        }

        public override string ToString()
        {
            string s = this._value.ToString("#.00000000") + " m";

            return s;
        }


    }
}
