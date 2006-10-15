using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace KmlUtils
{
    public class KmlWriter
    {
        //public static void WriteIconStyle(string id, string href, string color, double scale, int x, int y, int w, int h, XmlTextWriter tw)
        //{
        //    tw.WriteStartElement("Style");
        //    tw.WriteAttributeString("id", id);
        //    tw.WriteStartElement("IconStyle");
        //    tw.WriteElementString("scale", scale.ToString());
        //    if (color != null && color.Length == 6)
        //    {
        //        tw.WriteElementString("color", RGBtoABGR(color));
        //    }
        //    tw.WriteStartElement("Icon");
        //    tw.WriteElementString("href", href);
        //    if (w != 0 && h != 0)
        //    {
        //        tw.WriteElementString("x", x.ToString());
        //        tw.WriteElementString("y", y.ToString());
        //        tw.WriteElementString("w", w.ToString());
        //        tw.WriteElementString("h", h.ToString());
        //    }
        //    tw.WriteEndElement();//Icon
        //    tw.WriteEndElement(); // IconStyle
        //    tw.WriteEndElement(); // Style
        //}

        //public static void WriteLineStyle(string id, string color, XmlTextWriter w)
        //{
        //    w.WriteStartElement("Style");
        //    w.WriteAttributeString("id", id);
        //    w.WriteStartElement("LineStyle");
        //    w.WriteElementString("color", RGBtoABGR(color));
        //    w.WriteElementString("width", "2");
        //    w.WriteEndElement(); // LineStyle
        //    w.WriteEndElement(); // Style
        //}

        //public static void WriteBuoyStyle(string id, string iconHref, string iconColor, int x, int y, int w, int h, double iconScale, string labelColor, double labelScale, XmlTextWriter tw)
        //{
        //    tw.WriteStartElement("Style");
        //    tw.WriteAttributeString("id", id);

        //    tw.WriteStartElement("IconStyle");
        //    tw.WriteElementString("color", RGBtoABGR(iconColor));
        //    tw.WriteElementString("scale", iconScale.ToString());
        //    tw.WriteStartElement("Icon");
        //    tw.WriteElementString("href", iconHref);
        //    if (w != 0 && h != 0)
        //    {
        //        tw.WriteElementString("x", x.ToString());
        //        tw.WriteElementString("y", y.ToString());
        //        tw.WriteElementString("w", w.ToString());
        //        tw.WriteElementString("h", h.ToString());
        //    }
        //    tw.WriteEndElement();//Icon
        //    tw.WriteEndElement(); // IconStyle

        //    tw.WriteStartElement("LabelStyle");
        //    tw.WriteElementString("color", RGBtoABGR(labelColor));
        //    tw.WriteElementString("scale", labelScale.ToString());
        //    tw.WriteEndElement(); // LabelStyle

        //    tw.WriteEndElement(); // Style
        //}

        public static string RGBAtoABGR(string rgba)
        {
            string ret = "";
            // RRGGBBAA
            // 01234567
            ret += rgba.Substring(6, 2);
            ret += rgba.Substring(4, 2);
            ret += rgba.Substring(2, 2);
            ret += rgba.Substring(0, 2);
            return ret;
        }

        public static void WriteLabelStyle(string id, string rrggbbss, double scale, XmlTextWriter tw)
        {
            tw.WriteStartElement("Style");
            tw.WriteAttributeString("id", id);
            tw.WriteStartElement("LabelStyle");
            tw.WriteElementString("color", RGBAtoABGR(rrggbbss));
            tw.WriteElementString("scale", scale.ToString());
            tw.WriteEndElement(); // LabelStyle
            tw.WriteEndElement(); // Style
        }

        public static void WriteCoordinate(ICoordinate p, XmlTextWriter tw)
        {
            tw.WriteStartElement("coordinates");
            tw.WriteString(p.Lon.ToString() + "," + p.Lat.ToString() + "," + p.AltMeters.ToString() + " ");
            tw.WriteEndElement();
        }

        public static void WriteCoordinates(List<ICoordinate> pts, XmlTextWriter tw)
        {
            tw.WriteStartElement("coordinates");
            foreach (ICoordinate p in pts)
            {
                if(p != null)
                    tw.WriteString(p.Lon.ToString() + "," + p.Lat.ToString() + "," + p.AltMeters.ToString() + " ");
            }
            tw.WriteEndElement();
        }

        public static void WriteEmpyFolderEntry(string name, bool visibility, XmlTextWriter tw)
        {
            tw.WriteStartElement("Folder");
            tw.WriteElementString("name", name);
            if (!visibility)
                tw.WriteElementString("visibility", "0");
            LabelStyle lab = new LabelStyle("00000000", ColorMode.normal, 0.0);
            IconStyle ic = new IconStyle("00000000", ColorMode.normal, 0.0, new Icon(null, RefreshMode.once, 3600, 0, 0, 0, 0));
            List<ASubStyle> styles = new List<ASubStyle>();
            styles.Add(lab);
            styles.Add(ic);
            Style s = new Style(null, styles);

            tw.WriteStartElement("Placemark");
            tw.WriteElementString("name", "");
            tw.WriteElementString("Snippet", "");
            s.WriteTo(tw);
            new Point(AltitudeMode.clampToGround, 0.0, 0.0, 0.0).WriteTo(tw);

            tw.WriteEndElement();//Placemark
            tw.WriteEndElement();//Folder
        }

        public static void WriteStyleUrl(string styleUrl, XmlTextWriter tw)
        {
            tw.WriteElementString("styleUrl", styleUrl);
        }

        //public static void WriteStorm(Storm s, List<Buoy> buoys, List<Radar> radars, string buoyHref, XmlTextWriter tw)
        //{
        //    string desc = "";
        //    string tmp = "";
        //    tw.WriteStartElement("Folder");//storm
        //    tw.WriteElementString("name", s.Name + " " + s.ID);
        //    int y = 0;
        //    DateTime dt = DateTime.UtcNow;
        //    TimeSpan ts = TimeSpan.Zero;

        //    #region Initial
        //    tw.WriteStartElement("Placemark");
        //    tw.WriteElementString("name", s.ID + " " + s.Name);
        //    tw.WriteElementString("visibility", "1");
        //    tw.WriteElementString("Snippet", "");
        //    tmp = s.DateTime.ToString() + " UTC\n";
        //    tmp += s.DateTime.ToLocalTime().ToString() + " EDT\n";
        //    tmp += "\n" + s.Description;
        //    tw.WriteStartElement("description");
        //    WriteDescription(tmp, tw);
        //    tw.WriteEndElement();//description
        //    tw.WriteElementString("styleUrl", "#Initial");
        //    tw.WriteStartElement("Point");
        //    WriteCoordinates(s.Initial.Location, tw);
        //    tw.WriteEndElement();//Point
        //    tw.WriteEndElement();//Placemark
        //    #endregion

        //    #region TrackHistory
        //    if (s.TrackHistory != null && s.TrackHistory.Count > 0)
        //    {
        //        tw.WriteStartElement("Folder");//model
        //        tw.WriteElementString("name", "TrackHistory");

        //        #region Line
        //        tw.WriteStartElement("Placemark");
        //        tw.WriteElementString("name", "TrackHistoryLine");
        //        tw.WriteElementString("visibility", "1");
        //        tw.WriteElementString("styleUrl", "#lHIST");
        //        tw.WriteStartElement("LineString");
        //        List<IPoint> hist = s.TrackHistory;
        //        hist.Add(s.Initial.Location);
        //        WriteCoordinates(hist, tw);
        //        tw.WriteEndElement();//LineString
        //        tw.WriteEndElement();//Placemark
        //        #endregion

        //        #region Placemarks
        //        foreach (IPoint o in s.TrackHistory)
        //        {
        //            if (o != null)
        //            {
        //                dt += ts;
        //                tw.WriteStartElement("Placemark");
        //                tw.WriteElementString("name", "");
        //                tw.WriteElementString("visibility", "1");

        //                desc = "";
        //                tw.WriteElementString("styleUrl", "#HIST");
        //                tw.WriteStartElement("Point");
        //                WriteCoordinates(o, tw);
        //                tw.WriteEndElement();//Point
        //                tw.WriteEndElement();//Placemark
        //            }
        //            else
        //                Console.WriteLine("point: o was null");
        //        }

        //        #endregion

        //        tw.WriteEndElement();//trackHistory folder
        //    }
        //    #endregion

        //    #region BAMD
        //    if (s.BAMD.Count > 0)
        //    {
        //        tw.WriteStartElement("Folder");//model
        //        tw.WriteElementString("name", "BAMD");

        //        #region Line
        //        tw.WriteStartElement("Placemark");
        //        tw.WriteElementString("name", "TrackLine");
        //        tw.WriteElementString("visibility", "1");
        //        tw.WriteElementString("styleUrl", "#lBAMD");
        //        tw.WriteStartElement("LineString");
        //        tw.WriteStartElement("coordinates");
        //        foreach (Observation o in s.BAMD)
        //        {
        //            if (o.Lat != 0)
        //                tw.WriteString(o.Lon.ToString() + "," + o.Lat.ToString() + ",15000 ");
        //        }
        //        tw.WriteEndElement();//coordinates
        //        tw.WriteEndElement();//LineString
        //        tw.WriteEndElement();//Placemark
        //        #endregion

        //        #region Placemarks
        //        y = 0;
        //        dt = s.DateTime;
        //        ts = new TimeSpan(12, 0, 0);
        //        foreach (Observation o in s.BAMD)
        //        {
        //            if (y != 0 && o.Lat != 0)
        //            {
        //                dt += ts;
        //                tw.WriteStartElement("Placemark");
        //                tw.WriteElementString("name", o.Hour + "hrs");
        //                tw.WriteElementString("visibility", "1");
        //                tw.WriteElementString("Snippet", "");

        //                tw.WriteStartElement("description");
        //                desc = "BAMD\n\n";
        //                desc += dt.ToString() + " UTC\n";
        //                desc += dt.ToLocalTime().ToString() + " EDT\n\n";
        //                desc += "Lat: " + o.Lat.ToString() + " Lon: " + o.Lon.ToString() + "\n";
        //                desc += "SHIP: " + o.SHIP.ToString() + "kts\n";
        //                desc += "DSHP: " + o.DSHP.ToString() + "kts\n";
        //                WriteDescription(desc, tw);
        //                tw.WriteEndElement();//description
        //                desc = "";
        //                tw.WriteElementString("styleUrl", "#BAMD");
        //                tw.WriteStartElement("Point");
        //                WriteCoordinates(o.Location, tw);
        //                tw.WriteEndElement();//Point
        //                tw.WriteEndElement();//Placemark
        //            }
        //            y++;
        //        }

        //        #endregion

        //        tw.WriteEndElement();//model folder
        //    }
        //    #endregion

        //    #region BAMM
        //    if (s.BAMM.Count > 0)
        //    {
        //        tw.WriteStartElement("Folder");//model
        //        tw.WriteElementString("name", "BAMM");

        //        #region Line
        //        tw.WriteStartElement("Placemark");
        //        tw.WriteElementString("name", "TrackLine");
        //        tw.WriteElementString("visibility", "1");
        //        tw.WriteElementString("styleUrl", "#lBAMM");
        //        tw.WriteStartElement("LineString");
        //        tw.WriteStartElement("coordinates");
        //        foreach (Observation o in s.BAMM)
        //        {
        //            if (o.Lat != 0)
        //                tw.WriteString(o.Lon.ToString() + "," + o.Lat.ToString() + ",15000 ");
        //        }
        //        tw.WriteEndElement();//coordinates
        //        tw.WriteEndElement();//LineString
        //        tw.WriteEndElement();//Placemark
        //        #endregion

        //        #region Placemarks
        //        y = 0;
        //        dt = s.DateTime;
        //        foreach (Observation o in s.BAMM)
        //        {
        //            if (y != 0 && o.Lat != 0)
        //            {
        //                dt += ts;
        //                tw.WriteStartElement("Placemark");
        //                tw.WriteElementString("name", o.Hour + "hrs");
        //                tw.WriteElementString("visibility", "1");
        //                tw.WriteElementString("Snippet", "");

        //                tw.WriteStartElement("description");
        //                desc = "BAMM\n\n";
        //                desc += dt.ToString() + " UTC\n";
        //                desc += dt.ToLocalTime().ToString() + " EDT\n\n";
        //                desc += "Lat: " + o.Lat.ToString() + " Lon: " + o.Lon.ToString() + "\n";
        //                desc += "SHIP: " + o.SHIP.ToString() + "kts\n";
        //                desc += "DSHP: " + o.DSHP.ToString() + "kts\n";
        //                WriteDescription(desc, tw);
        //                tw.WriteEndElement();//description
        //                desc = "";
        //                tw.WriteElementString("styleUrl", "#BAMM");
        //                tw.WriteStartElement("Point");
        //                WriteCoordinates(o.Location, tw);
        //                tw.WriteEndElement();//Point
        //                tw.WriteEndElement();//Placemark
        //            }
        //            y++;
        //        }

        //        #endregion

        //        tw.WriteEndElement();//model folder
        //    }
        //    #endregion

        //    #region LBAR
        //    if (s.LBAR.Count > 0)
        //    {
        //        tw.WriteStartElement("Folder");//model
        //        tw.WriteElementString("name", "LBAR");
        //        tw.WriteElementString("Snippet", "");

        //        #region Line
        //        tw.WriteStartElement("Placemark");
        //        tw.WriteElementString("name", "TrackLine");
        //        tw.WriteElementString("visibility", "1");
        //        tw.WriteElementString("styleUrl", "#lLBAR");
        //        tw.WriteStartElement("LineString");
        //        tw.WriteStartElement("coordinates");
        //        foreach (Observation o in s.LBAR)
        //        {
        //            if (o.Lat != 0)
        //                tw.WriteString(o.Lon.ToString() + "," + o.Lat.ToString() + ",15000 ");
        //        }
        //        tw.WriteEndElement();//coordinates
        //        tw.WriteEndElement();//LineString
        //        tw.WriteEndElement();//Placemark
        //        #endregion

        //        #region Placemarks
        //        y = 0;
        //        dt = s.DateTime;
        //        foreach (Observation o in s.LBAR)
        //        {
        //            if (y != 0 && o.Lat != 0)
        //            {
        //                dt += ts;
        //                tw.WriteStartElement("Placemark");
        //                tw.WriteElementString("name", o.Hour + "hrs");
        //                tw.WriteElementString("visibility", "1");
        //                tw.WriteElementString("Snippet", "");

        //                tw.WriteStartElement("description");
        //                desc = "LBAR\n\n";
        //                desc += dt.ToString() + " UTC\n";
        //                desc += dt.ToLocalTime().ToString() + " EDT\n\n";
        //                desc += "Lat: " + o.Lat.ToString() + " Lon: " + o.Lon.ToString() + "\n";
        //                desc += "SHIP: " + o.SHIP.ToString() + "kts\n";
        //                desc += "DSHP: " + o.DSHP.ToString() + "kts\n";
        //                WriteDescription(desc, tw);
        //                tw.WriteEndElement();//description
        //                desc = "";
        //                tw.WriteElementString("styleUrl", "#LBAR");
        //                tw.WriteStartElement("Point");
        //                WriteCoordinates(o.Location, tw);
        //                tw.WriteEndElement();//Point
        //                tw.WriteEndElement();//Placemark
        //            }
        //            y++;
        //        }

        //        #endregion

        //        tw.WriteEndElement();//model folder
        //    }
        //    #endregion

        //    #region A98E
        //    if (s.A98E.Count > 0)
        //    {
        //        tw.WriteStartElement("Folder");//model
        //        tw.WriteElementString("name", "A98E");
        //        tw.WriteElementString("Snippet", "");

        //        #region Line
        //        tw.WriteStartElement("Placemark");
        //        tw.WriteElementString("name", "TrackLine");
        //        tw.WriteElementString("visibility", "1");
        //        tw.WriteElementString("styleUrl", "#lA98E");
        //        tw.WriteStartElement("LineString");
        //        tw.WriteStartElement("coordinates");
        //        foreach (Observation o in s.A98E)
        //        {
        //            if (o.Lat != 0)
        //                tw.WriteString(o.Lon.ToString() + "," + o.Lat.ToString() + ",15000 ");
        //        }
        //        tw.WriteEndElement();//coordinates
        //        tw.WriteEndElement();//LineString
        //        tw.WriteEndElement();//Placemark
        //        #endregion

        //        #region Placemarks
        //        y = 0;
        //        dt = s.DateTime;
        //        foreach (Observation o in s.A98E)
        //        {
        //            if (y != 0 && o.Lat != 0)
        //            {
        //                dt += ts;
        //                tw.WriteStartElement("Placemark");
        //                tw.WriteElementString("name", o.Hour + "hrs");
        //                tw.WriteElementString("visibility", "1");
        //                tw.WriteElementString("Snippet", "");

        //                tw.WriteStartElement("description");
        //                desc = "A98E\n\n";
        //                desc += dt.ToString() + " UTC\n";
        //                desc += dt.ToLocalTime().ToString() + " EDT\n\n";
        //                desc += "Lat: " + o.Lat.ToString() + " Lon: " + o.Lon.ToString() + "\n";
        //                desc += "SHIP: " + o.SHIP.ToString() + "kts\n";
        //                desc += "DSHP: " + o.DSHP.ToString() + "kts\n";
        //                WriteDescription(desc, tw);
        //                tw.WriteEndElement();//description
        //                desc = "";
        //                tw.WriteElementString("styleUrl", "#A98E");
        //                tw.WriteStartElement("Point");
        //                WriteCoordinates(o.Location, tw);
        //                tw.WriteEndElement();//Point
        //                tw.WriteEndElement();//Placemark
        //            }
        //            y++;
        //        }

        //        #endregion

        //        tw.WriteEndElement();//model folder
        //    }
        //    #endregion

        //    #region Nearby Buoys

        //    tw.WriteStartElement("Folder");//model
        //    tw.WriteElementString("visibility", "1");
        //    if (buoys.Count > 0)
        //    {
        //        tw.WriteElementString("name", "Nearby Buoys");
        //    }
        //    else
        //    {
        //        tw.WriteElementString("name", "No Nearby Buoys");
        //    }

        //    foreach (Buoy b in buoys)
        //    {
        //        WriteBuoy(b, buoyHref, tw);
        //    }

        //    tw.WriteEndElement();//buoys folder

        //    #endregion

        //    #region Nearby Radars
        //    tw.WriteStartElement("Folder");
        //    tw.WriteElementString("name", "Nearby Radars");
        //    tw.WriteElementString("open", "0");
        //    tw.WriteElementString("visibility", "0");
        //    KmlUtils.WriteListStyle(KmlUtils.LISTSTYLERADIO, tw);

        //    List<Radar> tmpRad = new List<Radar>();
        //    #region Long Range
        //    foreach (Radar r in radars)
        //    {
        //        if (r.RadType == Radar.RadarType.LongReflect)
        //            tmpRad.Add(r);
        //    }
        //    tw.WriteStartElement("Folder");
        //    if (tmpRad.Count > 0)
        //    {
        //        tw.WriteElementString("name", "Long Range Radar");
        //    }
        //    else
        //    {
        //        tw.WriteElementString("name", "No Long Range Radar");
        //    }
        //    foreach (Radar r in radars)
        //    {
        //        WriteRadar(r, tw);
        //    }
        //    tw.WriteEndElement();//Long range
        //    #endregion

        //    #region Composite
        //    tmpRad.Clear();
        //    foreach (Radar r in radars)
        //    {
        //        if (r.RadType == Radar.RadarType.CompositeReflect)
        //            tmpRad.Add(r);
        //    }
        //    tw.WriteStartElement("Folder");
        //    if (tmpRad.Count > 0)
        //    {
        //        tw.WriteElementString("name", "Composite Radar");
        //    }
        //    else
        //    {
        //        tw.WriteElementString("name", "No Composite Radar");
        //    }
        //    foreach (Radar r in radars)
        //    {
        //        WriteRadar(r, tw);
        //    }
        //    tw.WriteEndElement();//Long range
        //    #endregion

        //    tw.WriteEndElement();//Nearby Radars


        //    #endregion

        //    tw.WriteEndElement();//Folder
        //}

        //public static void WriteWarning(XmlTextWriter tw)
        //{
        //    tw.WriteStartElement("Placemark");
        //    tw.WriteElementString("name", "Tropical Data Kml Warning (Click Me)");
        //    tw.WriteElementString("visibility", "1");
        //    string tmp = "";
        //    tmp += "You are currently using the Uncompressed Kml version \n";
        //    tmp += "of this Network Link.  Due to bandwidth considerations \n";
        //    tmp += "I ask that you please switch over to the identical Kmz \n";
        //    tmp += "(compressed) version of this Network Link.  Please use the \n";
        //    tmp += "following link instead. \n";
        //    tmp += "<br>";
        //    tmp += "<a href=\"http://bbs.keyhole.com/ubb/showflat.php/Cat/0/Number/51227/an/0/page/0#51227\">Live Tropical Data and NHC Storm Forecast Models (KMZ)</a> \n";
        //    tmp += "<br>";
        //    tmp += "Paul Seabury 2005";
        //    tw.WriteStartElement("description");
        //    WriteDescription(tmp, tw);
        //    tw.WriteEndElement();//description
        //    //tw.WriteElementString("styleUrl", "#Initial");
        //    tw.WriteStartElement("LookAt");
        //    tw.WriteElementString("longitude", "-90.0");
        //    tw.WriteElementString("latitude", "25.0");
        //    tw.WriteElementString("range", "2400000");
        //    tw.WriteElementString("tilt", "0");
        //    tw.WriteElementString("heading", "0");
        //    tw.WriteEndElement(); // LookAt
        //    tw.WriteStartElement("Point");
        //    tw.WriteElementString("altitudeMode", "absolute");
        //    tw.WriteElementString("coordinates", "-90.00,25.00,15000");
        //    tw.WriteEndElement();//Point
        //    tw.WriteEndElement();//Placemark
        //}

        //public static void WriteRadar(Radar r, XmlTextWriter tw)
        //{
            //tw.WriteStartElement("GroundOverlay");
            //tw.WriteElementString("visibility", "0");
            //tw.WriteElementString("name", r.Description);
            //tw.WriteStartElement("Icon");
            //tw.WriteElementString("href", r.ImgLink);
            //tw.WriteElementString("refreshMode", "onInterval");
            //tw.WriteElementString("refreshInterval", "300");
            //tw.WriteElementString("viewBoundScale", "0.75");
            //tw.WriteEndElement(); // Icon
            //tw.WriteStartElement("LatLonBox");
            //tw.WriteElementString("north", r.North.ToString());
            //tw.WriteElementString("south", r.South.ToString());
            //tw.WriteElementString("east", r.East.ToString());
            //tw.WriteElementString("west", r.West.ToString());
            //tw.WriteEndElement(); // LatLonBox
            //tw.WriteEndElement(); // Ground Overlay
        //}

        //public static void WriteBuoy(Buoy b, string href, XmlTextWriter tw)
        //{
            //tw.WriteStartElement("Placemark");
            //tw.WriteElementString("visibility", "0");
            //tw.WriteElementString("name", b.Id);
            //tw.WriteElementString("styleUrl", "#Buoy");
            //tw.WriteElementString("Snippet", "");

            //string desc = "";

            //desc += "<img src=\"";
            //desc += href + "/" + b.Id + ".png\"";
            //desc += "alt =\"Image for " + b.Id + " temporarily unavailable.\"";
            //desc += ">\n";

            //desc += "<a href=\"";
            //desc += b.InfoUrl;
            //desc += "\">More Info for " + b.Id + "</a>";

            //tw.WriteStartElement("description");
            //WriteDescription(desc, tw);
            //tw.WriteEndElement();//description

            //tw.WriteStartElement("Point");
            //WriteCoordinates(b.Location, tw);
            //tw.WriteEndElement();//Point
            //tw.WriteEndElement();//Placemark
        //}

        //public static string LISTSTYLERADIO = "radioFolder";
        //public static string LISTSTYLECHECKED = "checkHideChildren";
        public static void WriteListStyle(ListItemType type, XmlTextWriter tw)
        {
            tw.WriteStartElement("Style");
            tw.WriteStartElement("ListStyle");
            tw.WriteElementString("listItemType", type.ToString());
            tw.WriteEndElement();
            tw.WriteEndElement();
        }

        public static void WriteDescription(string desc, XmlTextWriter w)
        {
            if (desc.Length > 0)
            {
                w.WriteStartElement("description");
                w.WriteCData(desc);
                w.WriteEndElement();
            }
        }

        public static void WriteScreenOverlay(string name, string desc, bool vis, string iconHref, bool overlayIsFraction,
            double ox, double oy, double sx, double sy, double rotation, double sizex, double sizey,
            XmlTextWriter tw)
        {
            tw.WriteStartElement("ScreenOverlay");
            tw.WriteElementString("name", name);
            tw.WriteElementString("desciption", desc);
            if(!vis)
                tw.WriteElementString("visibility", vis ? "1" : "0");
            tw.WriteStartElement("Icon");
            tw.WriteElementString("href", iconHref);
            tw.WriteEndElement();//Icon
            tw.WriteStartElement("overlayXY");
            tw.WriteAttributeString("x", overlayIsFraction ? ox.ToString() : Math.Floor(ox).ToString());
            tw.WriteAttributeString("y", overlayIsFraction ? oy.ToString() : Math.Floor(oy).ToString());
            if (overlayIsFraction)
            {
                tw.WriteAttributeString("xunits", "fraction");
                tw.WriteAttributeString("yunits", "fraction");
            }
            tw.WriteEndElement();//overlayXY
            tw.WriteStartElement("screenXY");
            tw.WriteAttributeString("x", overlayIsFraction ? sx.ToString() : Math.Floor(sx).ToString());
            tw.WriteAttributeString("y", overlayIsFraction ? sy.ToString() : Math.Floor(sy).ToString());
            if (overlayIsFraction)
            {
                tw.WriteAttributeString("xunits", "fraction");
                tw.WriteAttributeString("yunits", "fraction");
            }
            tw.WriteEndElement();//screenXY
            tw.WriteStartElement("size");
            tw.WriteAttributeString("x", overlayIsFraction ? sizex.ToString() : Math.Floor(sizex).ToString());
            tw.WriteAttributeString("y", overlayIsFraction ? sizey.ToString() : Math.Floor(sizey).ToString());
            if (overlayIsFraction)
            {
                tw.WriteAttributeString("xunits", "fraction");
                tw.WriteAttributeString("yunits", "fraction");
            }
            tw.WriteEndElement();//size
            tw.WriteElementString("rotation", rotation.ToString());
            tw.WriteEndElement();//ScreenOverlay
        }


    }
}