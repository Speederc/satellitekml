using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;

namespace KmlUtils
{
    public class Point : ICoordinate, IKml
    {
        AltitudeMode _altMode = AltitudeMode.clampToGround;
        double _lat, _lon, _altMeters;
        //ICoordinate _loc;

        public Point(AltitudeMode altMode, double lat, double lon, double altMeters)
        {
            this._altMode = altMode;
            this._lat = lat;
            this._lon = lon;
            this._altMeters = altMeters;
        }

        public void WriteTo(XmlTextWriter tw)
        {
            tw.WriteStartElement("Point");
            if(this._altMode != AltitudeMode.clampToGround)
                tw.WriteElementString("altitudeMode", this._altMode.ToString());
            KmlWriter.WriteCoordinate(this, tw);
            tw.WriteEndElement();//Point
        }

        //public ICoordinate Coordinate
        //{
        //    get { return _loc; }
        //    set { _loc = value; }
        //}

        public Distance GreatCircleDistance(Point pt2)
        {
            double distance = Math.Acos((Math.Sin(this.LatRad) * Math.Sin(pt2.LatRad)) + (Math.Cos(this.LatRad) * Math.Cos(pt2.LatRad) * Math.Cos(pt2.LonRad - this.LonRad)));
            distance = distance * 3963.0;  // Statute Miles
            distance = distance * 1.609344; // to Km
            Distance d = new Distance(distance, DistanceUnits.KILOMETERS);
            return d;
        }

        public double Lat
        {
            get { return this._lat; }
        }
        public double Lon
        {
            get { return this._lon; }
        }
        public double AltMeters
        {
            get { return this._altMeters; }
        }

        private double LatRad
        {
            get
            {
                return this._lat * Math.PI / 180.0;
            }
        }

        private double LonRad
        {
            get
            {
                return this._lon * Math.PI / 180.0;
            }
        }
    }
}
