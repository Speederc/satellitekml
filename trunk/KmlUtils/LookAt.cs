using System;
using System.Collections.Generic;
using System.Text;

namespace KmlUtils
{
    public class LookAt : IKml
    {
        private double _lat;//required
        private double _lon;//required
        private double _range;//optional
        private double _tilt;//optional
        private double _heading;//optional
        private bool _rangeSpecified = false;


        public LookAt(double lat, double lon)
        {
            this._lat = lat;
            this._lon = lon;
        }

        public LookAt(double lat, double lon, double range, double tilt, double heading)
            : this(lat, lon)
        {
            this._rangeSpecified = true;
            this._range = range;
            this._tilt = tilt;
            this._heading = heading;
        }
        
        
        #region IKml Members

        public void WriteTo(System.Xml.XmlTextWriter tw)
        {
            tw.WriteStartElement("LookAt");
            tw.WriteElementString("longitude", this._lon.ToString());
            tw.WriteElementString("latitude", this._lat.ToString());
            if (_rangeSpecified)
                tw.WriteElementString("range", _range.ToString());
            if (_tilt != 0)
                tw.WriteElementString("tilt", _tilt.ToString());
            if (_heading != 0)
                tw.WriteElementString("heading", _heading.ToString());
            tw.WriteEndElement();
        }

        #endregion
    }
}
