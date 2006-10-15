using System;
using System.Collections.Generic;
using System.Text;

namespace KmlUtils
{
    public class LatLonBox : IKml
    {
        double _north;
        double _south;
        double _east;
        double _west;
        double _rotation;
    
        public LatLonBox(double n, double s, double e, double w, double rotation)
        {
            this._north = n;
            this._south = s;
            this._east = e;
            this._west = w;
            this._rotation = rotation;
        }

        #region Public Properties
        public double North
        {
            get { return _north; }
            set { _north = value; }
        }
        public double South
        {
            get { return _south; }
            set { _south = value; }
        }
        public double East
        {
            get { return _east; }
            set { _east = value; }
        }
        public double West
        {
            get { return _west; }
            set { _west = value; }
        }
        public double Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }
        #endregion

        #region IKml Members

        public void WriteTo(System.Xml.XmlTextWriter tw)
        {
            tw.WriteStartElement("LatLonBox");
            tw.WriteElementString("north", _north.ToString());
            tw.WriteElementString("south", _south.ToString());
            tw.WriteElementString("east", _east.ToString());
            tw.WriteElementString("west", _west.ToString());
            if(_rotation != 0)
                tw.WriteElementString("rotation", _rotation.ToString());
            tw.WriteEndElement();//LatLonBox
        }

        #endregion
    }
}
