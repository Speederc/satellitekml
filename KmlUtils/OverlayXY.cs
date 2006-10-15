using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;

namespace KmlUtils
{
    public class OverlayXY : ScreenXY
    {
        public OverlayXY(ScreenUnits units, double x, double y) : base(units, x, y)
        {
        }

        #region IKml Members

        public new void WriteTo(XmlTextWriter tw)
        {
            tw.WriteStartElement("overlayXY");
            tw.WriteAttributeString("x", _x.ToString());
            tw.WriteAttributeString("y", _y.ToString());
            tw.WriteAttributeString("xunits", _units.ToString());
            tw.WriteAttributeString("yunits", _units.ToString());
            tw.WriteEndElement();//screenXY
        }

        #endregion
    }
}