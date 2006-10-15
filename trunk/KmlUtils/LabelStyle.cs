using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace KmlUtils
{
    public class LabelStyle : ASubStyle, IKml
    {
        //string _color = "ffffffff";
        //ColorMode _colorMode = ColorMode.normal;
        double _scale = 1.0;

        public LabelStyle(string rrggbbaa, ColorMode mode, double scale)
        {
            this._color = rrggbbaa;
            this._colorMode = mode;
            this._scale = scale;
        }

        public override void WriteTo(XmlTextWriter tw)
        {
            tw.WriteStartElement("LabelStyle");
            tw.WriteElementString("scale", _scale.ToString());
            if (_color != null && _color.Length == 8)
            {
                tw.WriteElementString("color", KmlWriter.RGBAtoABGR(_color));
            }
            tw.WriteElementString("colorMode", this._colorMode.ToString());
            tw.WriteEndElement();//LabelStyle
        }
    }
}
