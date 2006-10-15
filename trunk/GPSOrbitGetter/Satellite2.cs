using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using OrbitTools;

namespace GPSOrbitGetter
{
	/// <summary>
	/// Summary description for Satellite2.
	/// </summary>
	public class Satellite2
	{
		#region Member Variables
        // Orbit Params
        private Orbit _orbit;
        private Distance _footDiam = new Distance((2.0 * Globals.PI * Globals.XKMPER) / 2.0, DistanceUnits.KILOMETERS);

        // Position Params
        private bool _isVisible = false;
        private Site _obsSite = Site.WASH_DC; // default to Wash DC
		private List<CoordGeo> _footPrint;
        private DateTime _pt = DateTime.UtcNow;//	Position Time
		private Angle _az;//	    azimuth from _obsSite
        private Angle _el;//	    elevation from _obsSite
        private Angle _ph;//	    phase (-pi, -pi/2, 0, pi/2, +pi 	=> new, first quarter, full, last quarter, old) from _obsSite
        private Distance _rg;//     range from _obsSite
        private double _rr;//       range rate Km/s from _obsSite
		private Angle _lon;//	    longitude
		private Angle _lat;//	    Latitude
        private Distance _alt;//    Altitude
        private double _velX;//     velocity in X direction Km/s
        private double _velY;//     velocity in X direction Km/s
        private double _velZ;//     velocity in X direction Km/s
        private double _vel;//      velocity magnitude
 		#endregion
		public Satellite2()
		{
            this._footPrint = new List<CoordGeo>();

		}
		public Satellite2(Tle tle) : this()
		{
			this._orbit = new Orbit(tle);
		}
		public Satellite2(Orbit orbit) : this()
		{
			this._orbit = orbit;
		}
		public string ToString()
		{
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Name          : \t {0} \n", this.Name);
            sb.AppendFormat("Epoch         : \t {0} \n", this.EpochUTC.ToString());
            sb.AppendFormat("Latitude      : \t {0} \n", this._lat.Degrees.ToString("#.00000000"));
            sb.AppendFormat("Longitude     : \t {0} \n", this._lon.Degrees.ToString("#.00000000"));
            sb.AppendFormat("Altitude      : \t {0} \n", this._alt.ToString(DistanceUnits.KILOMETERS));
            //sb.AppendFormat("Inclination   : \t {0} \n", this.Inclination.ToString(AngleUnits.DEGREES));
            //sb.AppendFormat("RA of AscNode : \t {0} \n", this.RightAscencionAscendingNode.ToString(AngleUnits.DEGREES));
            //sb.AppendFormat("Eccentricity  : \t {0} \n", this.Eccentricity.ToString("#.00000000"));
            //sb.AppendFormat("Arg. Of Per.  : \t {0} \n", this.ArgumentOfPerigee.ToString(AngleUnits.DEGREES));
            sb.AppendFormat("Revs\\day      : \t {0} \n", this.Motion.ToString("#.00000000"));
            //sb.AppendFormat("Period        : \t {0} \n", this.Period.ToString());
            //sb.AppendFormat("SemiMajor axis: \t {0} \n", this.SemiMajor.ToString(DistanceUnits.KILOMETERS));
            //sb.AppendFormat("Perigee       : \t {0} \n", this.Perigee.ToString(DistanceUnits.KILOMETERS));
            //sb.AppendFormat("BStar         : \t {0} \n", this.BStar.ToString("#.00000000"));
            sb.AppendFormat("Mean Anmly    : \t {0} \n", this.MeanAnomaly.ToString(AngleUnits.DEGREES));
            sb.AppendFormat("Velocity      : \t {0} \n", this._vel.ToString("#.00000000") + " Km/s");
            sb.AppendFormat("Range         : \t {0} \n", this._rg.ToString(DistanceUnits.KILOMETERS));
            //sb.AppendFormat("RangeRate     : \t {0} \n", this._rr.ToString("#.00000000") + " Km/s");
            sb.AppendFormat("Elevation     : \t {0} \n", this.Elevation.Degrees.ToString("#.00000000"));
            sb.AppendFormat("Azimuth       : \t {0} \n", this._az.ToString(AngleUnits.DEGREES));
            //sb.AppendFormat("Visible       : \t {0} \n", this.IsVisible.ToString());
            //sb.AppendFormat("Azimuth     : \t {0} \n", this.AzimuthDeg.ToString("#.00000000"));
            //sb.AppendFormat("Elevation   : \t {0} \n", this.ElevationDeg.ToString("#.00000000"));
            //sb.AppendFormat("Range       : \t {0} \n", this.Range.ToString("#.00000000"));
            //sb.AppendFormat("RangeRate   : \t {0} \n", this.RangeRate.ToString("#.00000000"));
            //sb.Append("\n");

            return sb.ToString();
		}
        public CoordGeo Position
        {
            get { return new CoordGeo(this._lat.Rads, this._lon.Rads, this._alt.Kilometers); }
        }
        public bool IsVisible
        {
            get
            {
                return this.Elevation.Degrees > 0;
                double d = Math.Abs(Globals.GreatCircleDistance(this.Position, this._obsSite.Position).Kilometers);
                return d < (this._footDiam.Kilometers / 2.0); // radius
            }
        }
        public Angle Azimuth
        {
            get { return this._az; }
        }
		public string Name
		{
			get{return this._orbit.SatName();}
		}
		public string NameNumber
		{
			get{return this._orbit.SatName(true);}
		}
        public Site Site
        {
            get { return this._obsSite; }
            set { this._obsSite = value; }
        }
        public DateTime TimeUTC
        {
            get { return this._pt; }
            set
            {
                this._pt = value;
                this.InitFromTime(this._pt);
            }
        }
		public Angle ArgumentOfPerigee
		{
			get
            {
                double rads = this._orbit.ArgPerigee();
                return new Angle(rads);
            }
		}
		public double BStar
		{
			get{return this._orbit.BStar();}
		}
        /// <summary>
        /// Revolutions per day.
        /// </summary>
		public double Motion
		{
			get{return this._orbit.mnMotion();}
		}
		public double MotionD1
		{
			get{return this._orbit.Drag();}
		}
		public double MotionD2
		{
			get{return this._orbit.DragStar();}
		}
		public double Eccentricity
		{
			get{return this._orbit.Eccentricity();}
		}
        public OrbitTools.Julian EpochJulian
        {
            get { return this._orbit.Epoch(); }
        }
        public DateTime EpochUTC
        {
            get
            {
                DateTime dt = this._orbit.EpochTime();
                dt = Globals.InsureUTC(dt);
                return dt;
            }
        }
        public DateTime EpochLocal
        {
            get
            {
                DateTime dt = this._orbit.EpochTime();
                if (dt.Kind == DateTimeKind.Utc)
                    dt = dt.ToLocalTime();
                return dt;
            }
        }
        public OrbitTools.Eci GetPositionECI(double minutesSinceEpoch)
        {
            return this._orbit.getPosition(minutesSinceEpoch);
        }
        public OrbitTools.Eci GetPositionEci(DateTime positionAt)
        {
            if (positionAt.Kind == DateTimeKind.Local || positionAt.Kind == DateTimeKind.Unspecified)
                positionAt = positionAt.ToUniversalTime();

            TimeSpan ts = positionAt - this.EpochUTC;

            double minutes = ts.TotalMinutes;
            return this._orbit.getPosition(minutes);
        }
        public OrbitTools.CoordGeo GetPositionGeo(DateTime positionAt)
        {
            if (positionAt.Kind == DateTimeKind.Local || positionAt.Kind == DateTimeKind.Unspecified)
                positionAt = positionAt.ToUniversalTime();

            TimeSpan ts = positionAt - this.EpochUTC;
            double minutes = ts.TotalMinutes;
            Eci e = this._orbit.getPosition(minutes);
            return e.toGeo();
        }
        public OrbitTools.CoordGeo GetPositionGeo(double minutesSinceEpoch)
        {
            Eci e = this._orbit.getPosition(minutesSinceEpoch);
            return e.toGeo();
        }
        public bool SetTime(DateTime utc)
        {
            return this.InitFromTime(utc);
        }
        public Angle Inclination
        {
            get 
            {
                double rads = this._orbit.Inclination();
                return new Angle(rads); 
            }
        }
        public double MajorAxisAE
        {
            get { return this._orbit.Major(); }
        }
        public double MinorAxisAE
        {
            get { return this._orbit.Minor(); }
        }
        public Angle MeanAnomaly
        {
            get 
            {
                double rads = this._orbit.mnAnomaly();
                return new Angle(rads); 
            }
        }
        /// <summary>
        /// Mean Motion Revs/Day
        /// </summary>
        public double MeanMotion
        {
            get { return this._orbit.mnMotion(); }
        }
        /// <summary>
        /// Mean Motion Rads/Min
        /// </summary>
        public double MeanMotionRec
        {
            get { return this._orbit.mnMotionRec(); }
        }
        public Distance Perigee
        {
            get 
            {
                Distance d = new Distance(this._orbit.Perigee(), DistanceUnits.KILOMETERS);
                return d;
            }
        }
        /// <summary>
        /// Timespan representing Satellite orbital Period
        /// </summary>
        public TimeSpan Period
        {
            get 
            {
                double secs = this._orbit.Period(); 
                double fracPart = secs - Math.Floor(secs);
                double millis = fracPart * 1000.0;
                TimeSpan per = new TimeSpan(0, 0, 0, (int)secs, (int)millis);
                return per;
            }
        }
        public Angle RightAscencionAscendingNode
        {
            get 
            {
                double rads = this._orbit.RAAN();
                return new Angle(rads); 
            }
        }
        public Distance SemiMajor
        {
            get
            {
                Distance d = new Distance(this._orbit.SemiMajor(), DistanceUnits.AE);
                return d;
            }
        }
        public double SemiMinorAE
        {
            get { return this._orbit.SemiMinor(); }
        }
        // Km / sec
        public double VelocityAt(DateTime dt)
        {
            dt = Globals.InsureUTC(dt);
            TimeSpan ts = dt - this.EpochUTC;
            double minutes = ts.TotalMinutes;
            Eci e = this._orbit.getPosition(minutes);
            Vector v = e.getVel();

            double sum = 0.0;
            sum += Math.Pow(v.X, 2.0);
            sum += Math.Pow(v.Y, 2.0);
            sum += Math.Pow(v.Z, 2.0);

            double vel = Math.Sqrt(sum);

            return vel;
        }
        public Angle Elevation
        {
            get
            {
                return Globals.NormalizeElevation(this._el);
            }
        }
        public Tle TLE
        {
            get { return this._orbit.GetTle; }
        }

        public List<CoordGeo> FootPrint
        {
            get
            {
                this.MakeFootprint(this._pt);
                return new List<CoordGeo>(this._footPrint);
            }
        }
        private void MakeFootprint(DateTime atTime)
        {
            this._footPrint.Clear();

            // Constants
            double Re = 6378.137;
            double TWOPI = Math.PI * 2.0;

            double Rs = Math.Sqrt((Math.Pow(this.GetPositionECI(0).getPos().X, 2.0) + Math.Pow(this.GetPositionECI(0).getPos().Y, 2.0) + Math.Pow(this.GetPositionECI(0).getPos().Z, 2.0)));
            double srad = Math.Acos(Re / Rs);
            double latTemp = this.GetPositionGeo(atTime).m_Lat;
            double lonTemp = this.GetPositionGeo(atTime).m_Lon;
            double cla = Math.Cos(latTemp);
            double sla = Math.Sin(latTemp);
            double clo = Math.Cos(lonTemp);
            double slo = Math.Sin(lonTemp);
            double sra = Math.Sin(srad);
            double cra = Math.Cos(srad);

            double angle = 0.0;
            double X, Y, Z, x, y, z, lat, lon, lon1, lon2;
            for (int i = 0; i < 73; i++)
            {
                angle = i * (TWOPI / 72.0);
                X = cra;
                Y = sra * Math.Sin(angle);
                Z = sra * Math.Cos(angle);

                x = (X * cla) - (Z * sla);
                y = Y;
                z = (X * sla) + (Z * cla);

                X = (x * clo) - (y * slo);
                Y = (x * slo) + (y * clo);
                Z = z;

                lon = Math.Atan2(Y, X);
                lon1 = this.FN_Atan(Y, X);
                lon2 = this.FN_Atan2(Y, X);
                lat = Math.Asin(Z);
                CoordGeo cg = new CoordGeo(lat, lon, 0);
                this._footPrint.Add(cg);
                if (i == 0)
                {
                    this._footDiam = new Distance(Globals.GreatCircleDistance(this.Position, cg).Kilometers * 2.0, DistanceUnits.KILOMETERS);
                }
            }
        }
        private double FN_Atan(double y, double x)
        {
            double coeff_1 = Math.PI / 4;
            double coeff_2 = 3 * coeff_1;
            double abs_y = Math.Abs(y);
            if (abs_y == 0)
                abs_y += +.00000000001;      // kludge to prevent 0/0 condition
            double r, angle;
            if (x >= 0)
            {
                r = (x - abs_y) / (x + abs_y);
                angle = coeff_1 - coeff_1 * r;
            }
            else
            {
                r = (x + abs_y) / (abs_y - x);
                angle = coeff_2 - coeff_1 * r;
            }
            if (y < 0)
                return (-angle);     // negate if in quad III or IV
            else
                return (angle);
        }
        private double FN_Atan2(double y, double x)
        {
            double ret = 0.0;

            if (x != 0)
                ret = Math.Atan(y / x);
            else
                ret = (Math.PI / 2.0) * this.SGN(y);

            ret = this.MakeZeroToTWOPI(ret);

            return ret;
        }
        private double SGN(double x)
        {
            if (x > 0)
                return 1;
            else if (x < 0)
                return -1;

            return 0;
        }
        private double MakeZeroToTWOPI(double x)
        {
            double ret = x;
            if (ret < 0)
            {
                while (ret < 0)
                    ret += Math.PI;
            }

            if (ret > (2.0 * Math.PI))
            {
                while (ret > (2.0 * Math.PI))
                    ret -= Math.PI;
            }

            return ret;
        }
        private bool InitFromTime(DateTime utc)
        {
            utc = Globals.InsureUTC(utc);
            TimeSpan ts = utc - this.EpochUTC;
            double minutes = ts.TotalMinutes;
            Eci position = new Eci();
            try
            {
               position = this._orbit.getPosition(minutes);
            }
            catch (Exception)
            {
                return false;
            }

            this._lat = new Angle(position.toGeo().LatDeg, AngleUnits.DEGREES);
            this._lon = new Angle(position.toGeo().LonDeg, AngleUnits.DEGREES);
            this._alt = position.toGeo().Altitude;

            Vector velVec = position.getVel();
            this._velX = velVec.X;
            this._velY = velVec.Y;
            this._velZ = velVec.Z;
            this._vel = Math.Sqrt(Math.Pow(this._velX, 2.0) + Math.Pow(this._velY, 2.0) + Math.Pow(this._velZ, 2.0));

            CoordTopo ct = this.Site.getLookAngle(position);
            this._az = new Angle(ct.Azimuth);
            this._el = new Angle(ct.Elevation);
            this._rg = new Distance(ct.Range, DistanceUnits.KILOMETERS);
            this._rr = ct.RangeRate;

            this.MakeFootprint(utc);

            return true;
        }
	}
}
