using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;

namespace KmlUtils
{
    public class ScreenXY : IKml
    {
        protected ScreenUnits _units = ScreenUnits.pixels;
        protected double _x;
        protected double _y;

        public ScreenXY(ScreenUnits units, double x, double y)
        {
            this._units = units;
            this._x = x;
            this._y = y;
        }

        #region IKml Members

        public void WriteTo(XmlTextWriter tw)
        {
            tw.WriteStartElement("screenXY");
            tw.WriteAttributeString("x", _x.ToString());
            tw.WriteAttributeString("y", _y.ToString());
            tw.WriteAttributeString("xunits", _units.ToString());
            tw.WriteAttributeString("yunits", _units.ToString());
            tw.WriteEndElement();//screenXY
        }

        #endregion
    }
}
