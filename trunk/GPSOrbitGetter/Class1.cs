using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

using OrbitTools;

namespace GPSOrbitGetter
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			OrbitGetter og = new OrbitGetter(60);
            og.AddConstellation("GPS", "http://celestrak.com/NORAD/elements/gps-ops.txt");
            //ArrayList satellites = new ArrayList();
            //Constellation c = new Constellation("GPS");
            //c.SetLocation(38.8919, -77.0338, 0);

			Thread.Sleep(1500);

			while(true)
			{
				//WriteKML(og.Satellites);
				DateTime dt = DateTime.UtcNow;
                og.SetTimeForConstellation("GPS", dt);
                WriteKML(og.GetConstellation("GPS").Satellites);
                foreach (Satellite2 sat in og.GetConstellation("GPS").Satellites)
                {
                    if (sat.Name.IndexOf("BIIR-10") >= 0)
                        Console.WriteLine(sat.ToString());
                }
				
				
				Thread.Sleep(1000);
			}
		}

		private static void WriteFootPrint(Satellite2 s, XmlTextWriter tw)
		{
			//FileStream fs = new FileStream(s.Name + "_footprint.kml", FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 256);
			//XmlTextWriter tw = new XmlTextWriter(fs, Encoding.UTF8);
			//tw.Formatting = Formatting.Indented;
			//tw.WriteStartDocument();
			//tw.WriteStartElement("kml", "http://earth.google.com/kml/2.0");
			//tw.WriteStartElement("Document");
			
			//WriteLineStyle("GPS", "0000ff", tw);

			#region FootPrint
			tw.WriteStartElement("Folder");//FootPrint
			tw.WriteElementString("name", "Footprint" );
			tw.WriteStartElement("Placemark");
			tw.WriteElementString("name", "Circle");
			tw.WriteElementString("visibility", "0");
			tw.WriteElementString("styleUrl", "#Footprint");
			tw.WriteStartElement("Style");
			tw.WriteStartElement("PolyStyle");
			tw.WriteElementString("fill", "0");
			tw.WriteEndElement(); // PolyStyle
			tw.WriteEndElement(); // Style
			//<Polygon>
			//	<tessellate>1</tessellate>
			//      <outerBoundaryIs>
			//        <LinearRing>
			//          <coordinates>
			//-93.27468332017597,43.31409521513579,999.9999999999999 -102.226768103357,40.4670784747462,999.9999999999999 -105.8163585721755,30.80281111876798,999.9999999999999 -104.358672003192,11.28951605279321,999.9999999999999 -89.29206720375167,1.493575267472583,999.9999999999999 -75.06237971503623,4.668087714474587,999.9999999999999 -73.26460063907996,20.71931469426008,999.9999999999999 -78.38887676947647,35.92964913359087,999.9999999999999 -93.27468332017597,43.31409521513579,999.9999999999999 
			//          </coordinates>
			//        </LinearRing>
			//      </outerBoundaryIs>
			//</Polygon>
			tw.WriteStartElement("Polygon");
			//tw.WriteElementString("tesselate", "1");
			tw.WriteStartElement("outerBoundaryIs");
			tw.WriteStartElement("LinearRing");
			tw.WriteStartElement("coordinates");
			foreach (CoordGeo o in s.FootPrint)
			{
				tw.WriteString(o.LonDeg.ToString() + "," + o.LatDeg.ToString() + ",0" + " ");
			}
			tw.WriteEndElement();//coordinates
			tw.WriteEndElement();//LinearRing
			tw.WriteEndElement();//outerBoundaryIs
			tw.WriteEndElement();//Polygon
			tw.WriteEndElement();//Placemark

						
			#endregion

			#region SubSatellite Point
//			tw.WriteStartElement("Placemark");
//			tw.WriteElementString("name", "SubSatellite Point");
//			//tw.WriteElementString("visibility", "0");
//			tw.WriteElementString("styleUrl", "#GPS");
//			tw.WriteStartElement("Point");
//			tw.WriteElementString("altitudeMode", "absolute");
//			tw.WriteElementString("coordinates", s.LongitudeDeg.ToString() + "," + s.LatitudeDeg.ToString() + ",0 ");
//			tw.WriteEndElement();//Point
//			tw.WriteEndElement();//Placemark
			#endregion

			#region Cone
			tw.WriteStartElement("Placemark");
			tw.WriteElementString("name", "Cone");
			//tw.WriteElementString("visibility", "0");
			tw.WriteElementString("styleUrl", "#Pyramid");
			tw.WriteStartElement("MultiGeometry");
			foreach(CoordGeo p in s.FootPrint)
			{
				tw.WriteStartElement("LineString");
				tw.WriteElementString("altitudeMode", "absolute");
				//tw.WriteElementString("tesselate", "0");
				tw.WriteStartElement("coordinates");
				double meters = s.Position.Altitude.Meters;
				tw.WriteString(p.LonDeg.ToString() + "," + p.LatDeg.ToString() + "," + p.Altitude.Meters.ToString() + " ");
				tw.WriteString(s.Position.LonDeg.ToString() + "," + s.Position.LatDeg.ToString() + "," + meters.ToString() + " ");
				tw.WriteEndElement();//coordinates
				tw.WriteEndElement();//LineString
			}
			tw.WriteEndElement();//MultiGeometry
			tw.WriteEndElement();//PlaceMark
			#endregion

			#region Shadow
			tw.WriteStartElement("Placemark");
			tw.WriteElementString("name", "Shadow");
			//tw.WriteElementString("visibility", "0");
			tw.WriteElementString("styleUrl", "#Pyramid");
			tw.WriteStartElement("MultiGeometry");
			List<CoordGeo> pts = s.FootPrint;
			int last = 0;
			int count = 1;
			for(int i = 4;i<=34;i+=2)//17 is center of hatch
			{

                CoordGeo start = (CoordGeo)pts[i];
				#region Find End Point
				// Will only work for 2PI/72 pts.
				last = 73 - (count*2);
				count++;
				if(last < 0)
				{
					last = 73 + last;
				}
                CoordGeo end = (CoordGeo)pts[last];
				#endregion
				tw.WriteStartElement("LineString");
				//tw.WriteElementString("altitudeMode", "clampedToGround");//This is default behaviour
				tw.WriteElementString("tessellate", "1");
				tw.WriteStartElement("coordinates");
				tw.WriteString(start.LonDeg.ToString() + "," + start.LatDeg.ToString() + ",0 ");
				tw.WriteString(end.LonDeg.ToString() + "," + end.LatDeg.ToString() + ",0");
				tw.WriteEndElement();//coordinates
				tw.WriteEndElement();//LineString
			}
			tw.WriteEndElement();//MultiGeometry
			tw.WriteEndElement();//PlaceMark
			#endregion

			tw.WriteEndElement();//Folder
		}

		private static void WriteSatellite(Satellite2 s, XmlTextWriter tw)
		{
			#region Write Kml File
//			FileStream fs = new FileStream(s.Name+".kml", FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 256);
//			XmlTextWriter tw = new XmlTextWriter(fs, Encoding.UTF8);
//			tw.Formatting = Formatting.Indented;
//			tw.WriteStartDocument();
//			tw.WriteStartElement("kml", "http://earth.google.com/kml/2.0");
//			tw.WriteStartElement("Document");
			
			//WriteLineStyle("GPS", "0000ff", tw);
			tw.WriteStartElement("Folder");//satellite
			tw.WriteElementString("name", s.Name );
			tw.WriteStartElement("Placemark");
			tw.WriteElementString("name", s.Name);
			tw.WriteElementString("visibility", "1");
			tw.WriteElementString("styleUrl", "#SubSatellite");
			tw.WriteStartElement("Point");
			tw.WriteElementString("extrude", "1");
			tw.WriteElementString("altitudeMode", "absolute");
			
			CoordGeo p = s.Position;
			tw.WriteElementString("coordinates", p.LonDeg.ToString() + "," + p.LatDeg.ToString() + "," + (p.Altitude.Meters).ToString());
			tw.WriteEndElement();//Point
			tw.WriteEndElement();//Placemark

			WriteFootPrint(s, tw);
			tw.WriteEndElement();//Folder
			#endregion

//			tw.WriteEndElement();//Document
//			tw.WriteEndElement();//kml
//			tw.Flush();
//			tw.Close();
//			fs.Close();
		}

		private static void WriteKML(List<Satellite2> satellites)
		{
			#region Write Kml File
			FileStream fs = new FileStream("singleOrbit_allSats_2k.kml", FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 256);
			XmlTextWriter tw = new XmlTextWriter(fs, Encoding.UTF8);
			tw.Formatting = Formatting.Indented;
			tw.WriteStartDocument();
			tw.WriteStartElement("kml", "http://earth.google.com/kml/2.0");
			tw.WriteStartElement("Document");
			
			//WritePolyStyle("FootPrint", "ccffff77", false, true, tw);
			//http://paulseabury.com/genericSat.png
			WriteLineStyle("Footprint", "ccffff77", 1, tw);
			WriteLineStyle("Pyramid", "ccffff33", 1, tw);
			//WriteLineStyle("SubSatellite", "cccccc99", 4, tw);
			WriteSatelliteStyle("SubSatellite", "cccccc99", 4, "http://paulseabury.com/genericSat.png", tw);
			tw.WriteStartElement("Folder");//satellite
			tw.WriteElementString("name", "GPS Constellation");
			foreach(Satellite2 s in satellites)
			{
				WriteSatellite(s, tw);
//				tw.WriteStartElement("Folder");//satellite
//				tw.WriteElementString("name", s.Name);
//				#region Path
//				
//				tw.WriteStartElement("Placemark");
//				tw.WriteElementString("name", "TrackLine");
//				tw.WriteElementString("visibility", "0");
//				tw.WriteElementString("styleUrl", "#GPS");
//				tw.WriteStartElement("LineString");
//				tw.WriteElementString("altitudeMode", "absolute");
//				tw.WriteStartElement("coordinates");
//				foreach (CoordGeoDeg o in s.Path)
//				{
//					tw.WriteString(o.Lon.ToString() + "," + o.Lat.ToString() + "," + (o.Alt*100.0).ToString() + " ");
//				}
//				tw.WriteEndElement();//coordinates
//				tw.WriteEndElement();//LineString
//				tw.WriteEndElement();//Placemark
//				
//				#endregion
//				#region Location
//				tw.WriteStartElement("Placemark");
//				tw.WriteElementString("name", s.TLE.getName());
//				tw.WriteElementString("visibility", "1");
//				tw.WriteStartElement("Point");
//				tw.WriteElementString("altitudeMode", "absolute");
//				#region Get Time Differenced Location
//				DateTime now = DateTime.UtcNow;
//				TimeSpan diff = now - s.Initial;
//				double mins = diff.TotalMinutes;
//				CoordGeoDeg deg = new CoordGeoDeg(s.Orbit.getPosition(diff).toGeo());
//				#endregion
//				tw.WriteElementString("coordinates", deg.Lon.ToString() + "," + deg.Lat.ToString() + "," + (deg.Alt * 100.0).ToString());
//				tw.WriteEndElement();//Point
//				tw.WriteEndElement();//Placemark
//				#endregion
//				tw.WriteEndElement();//Folder

			}
			tw.WriteEndElement();//Folder
			tw.WriteEndElement();//Document
			tw.WriteEndElement();//kml
			tw.Flush();
			tw.Close();
			fs.Close();
			#endregion
		}

		static void WriteSatelliteStyle(string id, string lineColor, int lineWidth, string iconURL, XmlTextWriter w)
		{
			w.WriteStartElement("Style");
			w.WriteAttributeString("id", id);
			w.WriteStartElement("IconStyle");
			w.WriteElementString("color", RGBAtoABGR(lineColor));
			w.WriteElementString("scale", "4.0");
			w.WriteStartElement("Icon");
			w.WriteElementString("href", iconURL);
			w.WriteEndElement(); // Icon
			w.WriteEndElement(); // IconStyle
			w.WriteStartElement("LineStyle");
			w.WriteElementString("color", RGBAtoABGR(lineColor));
			w.WriteElementString("width", lineWidth.ToString());
			w.WriteEndElement(); // LineStyle
			w.WriteEndElement(); // Style
		}

		static void WriteLineStyle(string id, string color, int width, XmlTextWriter w)
		{
			w.WriteStartElement("Style");
			w.WriteAttributeString("id", id);
			w.WriteStartElement("LineStyle");
			w.WriteElementString("color", RGBAtoABGR(color));
			w.WriteElementString("width", width.ToString());
			w.WriteEndElement(); // LineStyle
			w.WriteEndElement(); // Style
		}

		static void WriteIconStyle(string id, string colorRGBA, string colorMode, double scale, string href, XmlTextWriter w)
		{
			w.WriteStartElement("Style");
			w.WriteAttributeString("id", id);
			w.WriteStartElement("IconStyle");
			w.WriteElementString("color", RGBAtoABGR(colorRGBA));
			w.WriteElementString("colorMode", colorMode);
			w.WriteElementString("scale", "4.0");
			w.WriteStartElement("Icon");
			w.WriteElementString("href", href);
			w.WriteEndElement(); // Icon
			w.WriteEndElement(); // IconStyle
			w.WriteEndElement(); // Style
		}

		static void WriteIconStyle(string id, string colorRGBA, double scale, string href, XmlTextWriter w)
		{
			WriteIconStyle(id, colorRGBA, "normal", scale, href, w);
		}

		static void WritePolyStyle(string id, string colorRGBA, string colorMode, bool fill, bool outline, XmlTextWriter w)
		{
			w.WriteStartElement("Style");
			w.WriteAttributeString("id", id);
			w.WriteStartElement("PolyStyle");
			w.WriteElementString("color", RGBAtoABGR(colorRGBA));
			if(!colorMode.Equals("normal"))
				w.WriteElementString("colorMode", colorMode);
			if(!fill)
				w.WriteElementString("fill", fill ? "1" : "0" );
			if(!outline)
				w.WriteElementString("outline", outline ? "1" : "0");
			w.WriteEndElement(); // PolyStyle
			w.WriteEndElement(); // Style
		}

		static void WritePolyStyle(string id, string colorRGBA, bool fill, bool outline, XmlTextWriter w)
		{
			WritePolyStyle(id, colorRGBA, "normal", fill, outline, w);
		}
		static string RGBAtoABGR(string rgb)
		{
			string ret = "";
			// RRGGBBAA
			// 01234567
			ret += rgb.Substring(6, 2);
			ret += rgb.Substring(4, 2);
			ret += rgb.Substring(2, 2);
			ret += rgb.Substring(0, 2);
			return ret;
		}
	}
}
