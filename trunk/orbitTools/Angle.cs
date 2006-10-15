using System;
using System.Collections.Generic;
using System.Text;

namespace OrbitTools
{
    public enum AngleUnits
    {
        RADS = 0,
        DEGREES
    }

    public class Angle
    {
        private double _value;

        /// <summary>
        /// Create Default Angle with AngleUnits = RADS
        /// </summary>
        /// <param name="value"></param>
        public Angle(double value)
        {
            value = Globals.NormalizeRads(value);
            this._value = value;
        }

        public Angle(double value, AngleUnits units)
        {
            switch (units)
            {
                case AngleUnits.DEGREES:
                    this._value = Globals.Deg2Rad(value);
                    break;
                default:
                    this._value = Globals.NormalizeRads(value);
                    break;
            }
        }

        public double Rads
        {
            get { return this._value; }
            set { this._value = value; }
        }

        public double Degrees
        {
            get { return Globals.Rad2Deg(this._value); }
            set
            {
                this._value = Globals.Deg2Rad(value);
            }
        }

        public override string ToString()
        {
            string s = this._value.ToString("#.00000000") + " rads";

            return s;
        }

        public string ToString(AngleUnits units)
        {
            double val = 0.0;
            string suffix = "";
            switch (units)
            {
                case AngleUnits.DEGREES:
                    val = this.Degrees;
                    suffix = " deg";
                    break;
                default:
                    val = this.Rads;
                    suffix = " rad";
                    break;
            }

            string s = val.ToString("#.00000000") + suffix;

            return s;
        }
    }
}
