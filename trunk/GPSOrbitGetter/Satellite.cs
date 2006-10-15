using System;
using System.Text;
using System.Collections;

using OrbitTools;

namespace GPSOrbitGetter
{
	/// <summary>
	/// Summary description for Satellite.
	/// </summary>
	public class Satellite
	{
		private ArrayList _pyramid;
		private ArrayList _orbit;
		private ArrayList _footPrint;
		private string _name;
		private double _jt;//	Julian Time
		private double _az;//	azimuth rads
		private double _el;//	elevation rads
		private double _ph;//	(-pi, -pi/2, 0, pi/2, +pi 	=> new, first quarter, full, last quarter, old)
		private double _lon;//	rads
		private double _lat;//	rads
		private double _rg;//	range km
		private double _rr;//	rangeRate km/s
		private double _alt;//	km
		private bool _desc;//	bool (descending?)
		private double _x;//	ECIx km
		private double _y;//	ECIy km
		private double _z;//	ECIz km
		private double _xDOT;// km/s
		private double _yDOT;// km/s
		private double _zDOT;// km/s
		private double _vDOT;// km/s

		public Satellite(string name, double jt, double az, double el, double ph, double lon, double lat,
			double rg, double rr, double alt, bool desc, double x, double y, double z,
			double xDOT, double yDOT, double zDOT, double vDOT)
		{
			this._pyramid = new ArrayList();
			this._footPrint = new ArrayList();
			this._orbit = new ArrayList();
			this._name = name;
			this._jt = jt;
			this._az = az;
			this._el = el;
			this._ph = ph;
			this._lon = lon;
			this._lat = lat;
			this._rg = rg;
			this._rr = rr;
			this._alt = alt;
			this._desc = desc;
			this._x = x;
			this._y = y;
			this._z = z;
			this._xDOT = xDOT;
			this._yDOT = yDOT;
			this._zDOT = zDOT;
			this._vDOT = vDOT;
		}

		public Satellite(OrbitTools.Tle tle)
		{
			Orbit o = new Orbit(tle);

		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("Name        : \t {0} \n", this.Name);
			sb.AppendFormat("Julian Date : \t {0} \n", this._jt.ToString("#.00000000"));
			sb.AppendFormat("Latitude    : \t {0} \n", this.LatitudeDeg.ToString("#.00000000"));
			sb.AppendFormat("Longitude   : \t {0} \n", this.LongitudeDeg.ToString("#.00000000"));
			sb.AppendFormat("Altitude    : \t {0} \n", this.Altitude.ToString("#.00000000"));
			sb.AppendFormat("Velocity    : \t {0} \n", this.Velocity.ToString("#.00000000"));
			sb.AppendFormat("Azimuth     : \t {0} \n", this.AzimuthDeg.ToString("#.00000000"));
			sb.AppendFormat("Elevation   : \t {0} \n", this.ElevationDeg.ToString("#.00000000"));
			sb.AppendFormat("Range       : \t {0} \n", this.Range.ToString("#.00000000"));
			sb.AppendFormat("RangeRate   : \t {0} \n", this.RangeRate.ToString("#.00000000"));
			sb.Append("\n");

			return sb.ToString();
		}

		public string Name
		{
			get{return this._name;}
		}
		public double AzimuthDeg
		{
			get
			{
				return this.RadToDeg(this._az);
			}
		}

		public double ElevationDeg
		{
			get
			{
				return this.RadToDeg(this._el);
			}
		}

		public double LatitudeDeg
		{
			get
			{
				return this.RadToDeg(this._lat);
			}
		}

		public double LongitudeDeg
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

		public double Range
		{
			get{return this._rg;}
		}
		public double RangeRate
		{
			get{return this._rr;}
		}
		public double Altitude
		{
			get{return this._alt;}
		}
		public bool Descending
		{
			get{return this._desc;}
		}
		public double ECIx
		{
			get{return this._x;}
		}
		public double ECIy
		{
			get{return this._y;}
		}
		public double ECIz
		{
			get{return this._z;}
		}
		public double ECIx_DOT
		{
			get{return this._xDOT;}
		}
		public double ECIy_DOT
		{
			get{return this._yDOT;}
		}
		public double ECIz_DOT
		{
			get{return this._zDOT;}
		}
		public double Velocity
		{
			get{return this._vDOT;}
		}

		public Point InitLocation
		{
			get{return new Point(this._lat, this._lon, this._alt);}
		}
		public ArrayList PyramidPoints
		{
			get{return new ArrayList(this._pyramid);}
		}

		public ArrayList FootPrintPts
		{
			get
			{
				this.MakeFootprint();
				return new ArrayList(this._footPrint);
			}
		}
		public ArrayList OrbitPts
		{
			get{return this._orbit;}
			set{this._orbit = value;}
		}
		public void AddOrbitPoint(Point p)
		{
			this._orbit.Add(p);
		}
		public double RadToDeg(double rads)
		{
			double ret = rads * 180.0 / Math.PI;

			return ret;
		}

		private void MakeFootprint()
		{
			this._footPrint.Clear();

			// Constants
			double Re = 6378.137;
			double TWOPI = Math.PI * 2.0;

			double Rs = Math.Sqrt((Math.Pow(this._x, 2.0) + Math.Pow(this._y, 2.0) + Math.Pow(this._z, 2.0)));
			double srad = Math.Acos(Re/Rs);
			double cla = Math.Cos(this._lat);
			double sla = Math.Sin(this._lat);
			double clo = Math.Cos(this._lon);
			double slo = Math.Sin(this._lon);
			double sra = Math.Sin(srad);
			double cra = Math.Cos(srad);

			double angle = 0.0;
			double X, Y, Z, x, y, z, lat, lon, lon1, lon2;
			for(int i=0;i<73;i++)
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
				this._footPrint.Add(new Point(lat, lon, 0));
				
			}
		}

		private void MakeFootprint2()
		{
			// Constants
			double Re = 6378.137;
			double Rc = Math.PI / 2.425;
			double HalfPI = Math.PI / 2.0;
			double Lon_s = this._lon;
			double Lat_s = this._lat;
			bool oppHem = false;
			
			// Local vars
			double angle = 0.0;
			double newLat, newLon;
			Point p = null;

			// Clear former List
			this._footPrint.Clear();

			// Rotate the Circle (every 2 degrees)
			for(int i=0;i<180;i++)
			{
				// Compute Angle
				angle = 2.0 * ((double)i) * Math.PI/180.0;
				
				// Compute new Lon
				newLon = Lon_s + (Rc * Math.Sin(angle));//Lon_s - angle;
				newLon = this.NormalizeRads(newLon);
				newLon = this.NormalizeLon(newLon);

				// Compute new Lat
				newLat = Lat_s + (Rc * Math.Cos(angle));
				//newLat = this.NormalizeLat(newLat, out oppHem);

				// Swap Lon if Necessary
				if(oppHem)
				{
					newLon = this.SwapHemisphere(newLon);
				}

				// Create Point
				p = new Point(newLat, newLon, 0);
				
				// Add Point to footprint
				this._footPrint.Add(p);

				// Add Pyramid Points
				if(i==0 || i==45 || i==90 || i== 135)
					this._pyramid.Add(p);
			}
		}
		
		// Get -2PI < rads < 2PI
		private double NormalizeRads(double rads)
		{
			double ret = rads;
			if(rads > (2.0 * Math.PI))
				ret =  (rads - (2.0 * Math.PI));

			if(rads < (-2.0 * Math.PI))
				ret =  (rads + (2.0 * Math.PI));

			
			return ret;
		}


		// Get Lon Negative for West, Positive for East.  Input should be 0->2PI
		private double NormalizeLon(double rads)
		{
			// East
			if(rads < Math.PI)
				return rads;

			// West
			double tmp = 0.0;
			if(rads > Math.PI)
			{
				tmp = rads - Math.PI;
				tmp = (-Math.PI) + tmp;
				return tmp;
			}

			return rads;
		}

		
		// If Lat crosses Pole, fix it in other hemisphere.
		private double NormalizeLat(double lat, out bool oppHem)
		{
			double diff = 0.0;
			double ret = lat;
			oppHem = false;

			if(lat > (Math.PI/2.0))
			{
				diff = lat - (Math.PI/2.0);
				ret = (Math.PI/2.0) - diff;
				oppHem = true;
			}

			if(lat < (-Math.PI/2.0))
			{
				diff = Math.Abs(lat + (Math.PI/2.0));
				ret = (-Math.PI/2.0) + diff;
				oppHem = true;
			}

			return ret;
		}


		// Swap an angle from E to W hemi
		private double SwapHemisphere(double rads)
		{
            double ret = 0.0;

			ret = rads + Math.PI;
			ret = this.NormalizeRads(ret);
			ret = this.NormalizeLon(ret);

			return ret;
		}

		
		private bool NormalizePhi(double phi, out double newPhi)
		{
			double tmp = 0.0;
			if(phi > (Math.PI/2.0))
			{
				tmp = phi - (Math.PI/2.0);
				//newPhi = (Math.PI/2.0) - tmp;
				newPhi = tmp;
				return true;
			}

			if(phi < (-Math.PI/2.0))
			{
				tmp = Math.Abs(phi + (Math.PI/2.0));
				//newPhi = (-Math.PI/2.0) + tmp;
				newPhi = (-Math.PI / 2.0) + tmp;
				return true;
			}
			
			newPhi = phi;

			return false;
		}

		private double Deg2Rad(double deg)
		{
			return deg * Math.PI / 180.0;
		}
		private double ASN(double rad)
		{
			return this.FN_Atan(rad, Math.Sqrt(1 - (rad*rad)));
		}
		private double SGN(double x)
		{
			if(x>0)
				return 1;
			else if(x<0)
				return -1;

			return 0;
		}
		private double FN_Atan2(double y, double x)
		{
			double ret = 0.0;

			if(x != 0)
				ret = Math.Atan(y/x);
			else
				ret = (Math.PI/2.0)*this.SGN(y);

			ret = this.MakeZeroToTWOPI(ret);
				
			return ret;
		}

		private double MakeZeroToTWOPI(double x)
		{
			double ret = x;
			if(ret<0)
			{
				while(ret < 0)
					ret += Math.PI;
			}

			if(ret > (2.0 * Math.PI))
			{
				while(ret > (2.0 * Math.PI))
					ret -= Math.PI;
			}

			return ret;
		}
		private double FN_Atan(double y, double x)
		{
			double coeff_1 = Math.PI/4;
			double coeff_2 = 3*coeff_1;
			double abs_y = Math.Abs(y);
			if(abs_y == 0)
				abs_y += + .00000000001;      // kludge to prevent 0/0 condition
			double r, angle;
			if (x>=0)
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
				return(-angle);     // negate if in quad III or IV
			else
				return(angle);
		}

		private void MakeFootprint1()
		{
			this._footPrint.Clear();

			double Re = 6378.137;
			double satRadius = Re + this._alt;
			double phiDelta = Math.PI / 2.425;
			double lonDelta, latDelta, theta;
			double newLat, newLon, newTheta, newPhi, normPhi;
			double startPhi, startTheta;
			bool oppositeHem = false, startSouth = false;

			// Figure Initial surface position of satellite
			if(this._lat < 0)
				startSouth = true;

			startPhi = this._lat;
			//			startPhi = ((Math.PI/2.0) - this._lat);
			
			startTheta = this._lon;//Math.Atan2(this._y, this._x);// * (Re * Math.Sin(startPhi));

			//double startX = Re * Math.Cos(startTheta) * Math.Sin(startPhi);
			//double startY = Re * Math.Sin(startTheta) * Math.Sin(startPhi);
			//double startZ = Re * Math.Cos(startPhi);
			double thetaD = this.RadToDeg(startTheta);
			double phiD = this.RadToDeg(startPhi);

			Point p = null;


			double surfaceRadius = Math.PI / 2.425;
			
			//double lonDelta, latDelta, theta;
			//double newLat, newLon;
			//Point p = null;

			for(int i=0; i<180; i++)
			{
				newPhi = startPhi + (surfaceRadius * Math.Cos(((double)i) * (Math.PI * 2.0 / 180)));
				
				oppositeHem = this.NormalizePhi(newPhi, out normPhi);

				newTheta = startTheta - this.NormalizeRads((((double)i) * (Math.PI * 2.0 / 180)));
				newTheta = this.NormalizeRads(newTheta);
				//newTheta = this.NormalizeLat(newTheta);
				//				if(oppositeHem)
				//				{
				//					//Abs diff from 180 and flip sign.
				//					if(newTheta > 0)
				//					{
				//						newTheta = Math.PI - newTheta;
				//						newTheta = -newTheta;
				//					}
				//					else
				//					{
				//						newTheta = Math.PI - Math.Abs(newTheta);
				//					}
				//
				////					newTheta += Math.PI;
				////					newTheta = this.NormalizeRads(newTheta);
				//				}

				#region olderCrap
				//				lonDelta = radius * Math.Sin(theta);
				//				latDelta = radius * Math.Cos(theta);
				//
				//				newLat = this._lat + latDelta;
				//				newLon = this._lon + lonDelta;
				////				double ld = newLat;
				////				if(newLat > Math.PI / 2.0)
				////					ld = (Math.PI / 2.0) - (newLat - (Math.PI / 2.0));
				////				double scale = Math.Cos(ld);
				////				newLon = this._lon + (lonDelta * (1/scale));
				//
				//				// Correct for > 2PI
				//				if(newLon > (Math.PI * 2.0))
				//					newLon -= (Math.PI * 2.0);
				//
				//				// Don't go past the poles
				//				if(newLat >= (Math.PI / 2.0))
				//				{
				//					if(newLon > Math.PI)
				//						newLon -= Math.PI;
				//					else
				//						newLon += Math.PI;
				//					//newLon = -newLon;
				//					newLat = (Math.PI / 2.0) - (newLat - Math.PI / 2.0);
				//					//newLat = ((Math.PI / 2.0) - .00001);
				//				}
				//
				//				if(newLat <= (-Math.PI / 2.0))
				//				{
				//					if(newLon > Math.PI)
				//						newLon -= Math.PI;
				//					else
				//						newLon += Math.PI;
				//					//newLon = -newLon;
				//					newLat = (-Math.PI / 2.0) + (-Math.PI / 2.0 - newLat);
				//					//newLat = -((Math.PI / 2.0) + .00001);
				//				}
				#endregion

				newLon = newTheta;// Final 

				//newPhi = (Math.PI/2.0) - normPhi;
				newPhi = normPhi;


				//				if(south)
				//					newPhi = -newPhi;

				thetaD = this.RadToDeg(newTheta);
				phiD = this.RadToDeg(newPhi);

				p = new Point(newTheta, newPhi, 0);
				newLat = p.LatDeg;
				newLon = p.LonDeg;

				this._footPrint.Add(p);

				if(i==0 || i==45 || i==90 || i== 135)
					this._pyramid.Add(p);
			}

			Point first = this._footPrint[0] as Point;
			this._footPrint.Add(new Point(first.LatRad,first.LonRad, 0));

			#region old crap
			//			double srad = Math.Acos(6378.0/this._alt);
			//			double cla = Math.Cos(this._lat);
			//			double sla = Math.Sin(this._lat);
			//			double clo = Math.Cos(this._lon);
			//			double slo = Math.Sin(this._lon);
			//			double cra = Math.Cos(this._alt);
			//			double sra = Math.Sin(this._alt);
			//
			//			double x, y, z, xx, yy, zz;
			//			for(int i=0;i<36;i++)
			//			{
			//				double angle = 2 * Math.PI *(((double)i)/36.0);
			//				x = cra;
			//				y = sra * Math.Sin(angle);
			//				z = sra * Math.Cos(angle);
			//
			//				xx = (x*cla) - (z*sla);
			//				yy = y;
			//				zz = (x*sla) + (z*cla);
			//
			//				x = (x*clo) - (y*slo);
			//				y = (x*slo) + (y*clo);
			//				z = zz;
			//
			//				// Now Convert from ECI to Lat/Lon
			//				//double lon = this.FN_Atan(y,x);
			//				double max = this.Deg2Rad(89.9);
			//				double min = this.Deg2Rad(-89.9);
			//
			//				double lon = Math.Atan2(y,x);
			//				//double lat = this.ASN(z);
			//				double lat = Math.Asin(z);
			//				if(lat >= max)
			//					lat = max;
			//				else if(lat <= min)
			//					lat = min;
			//
			//				Point p = new Point(lat, lon, 1000);
			//				this._footPrint.Add(p);
			//			}
			#endregion

			#region Original Procedure
			//			DEF PROCfoot(RS, slat, slon)
			//			REM Take satellite distance, sub-satellite lat/lon and compute unit vectors'
			//			REM x,y,z of N points of footprint on Earth's surface in Geocentric
			//			REM Coordinates.  Also terrestrial latitude and longitude of points. 
			//			:
			//			srad = ACS(RE/RS):                 REM Radius of footprint circle
			//			cla = COS(slat): sla = SIN(slat):  REM Sin/Cos these to save time
			//			clo = COS(slon): slo = SIN(slon)
			//			sra = SIN(srad): cra = COS(srad) 
			//			FOR I = 0 TO N:      REM N points to the circle
			//			A = 2*PI*I/N:        REM Angle around the circle
			//			:
			//			X = cra:             REM Circle of points centred on Lat=0, Lon=0
			//			Y = sra*SIN(A):      REM assuming Earth's radius = 1
			//			Z = sra*COS(A)       REM [ However, use a table for SIN(.)  COS(.) ]
			//			:
			//			x = X*cla - Z*sla:   REM Rotate point "up" by latitude  slat
			//			y = Y
			//			z = X*sla + Z*cla
			//			:
			//			X = x*clo - y*slo:   REM Rotate point "around" through longitude  slon
			//			Y = x*slo + y*clo
			//			Z = z
			//			:
			//			LON(I) = FNatn(Y,X): REM Convert point to Lat/Lon (or as required by map
			//			LAT(I) = ASN(Z):     REM projection and display system). [ASN(X) arcsin FNatn(X,SQR(1-X*X))]
			//			NEXT I
			//			ENDPROC
			#endregion
		}

	
	}
}
