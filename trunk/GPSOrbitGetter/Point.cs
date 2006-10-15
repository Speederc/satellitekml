using System;

namespace GPSOrbitGetter
{
	/// <summary>
	/// Summary description for APoint.
	/// </summary>
	public class Point
	{
		private double _lat;
		private double _lon;
		private double _alt;

		public Point(double latRads, double lonRads, double altKm)
		{
			this._lat = latRads;
			this._lon = lonRads;
			this._alt = altKm;
		}

		public double LatRad
		{
			get{return this._lat;}
			set{this._lat = value;}
		}

		public double LatDeg
		{
			get{return this.RadToDeg(this._lat);}
		}

		public double LonRad
		{
			get{return this._lon;}
			set{this._lon = value;}
		}

		public double LonDeg
		{
			get
			{
				double ret = this.RadToDeg(this._lon);
				if(ret > 180)
				{
					double delta = ret - 180.0;
					ret = (-180.0 + delta);
				}

				return ret;
			}
		}

		public double AltKm
		{
			get{return this._alt;}
		}
		private double RadToDeg(double rads)
		{
			if(rads > (Math.PI * 2.0))
				rads -= (Math.PI * 2.0);

			double ret = rads * 180.0 / Math.PI;

			return ret;
		}
	}
}
