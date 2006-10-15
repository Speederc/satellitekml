using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace KmlUtils
{
    public class LineStyle : ASubStyle, IKml
    {
        //string _color = "ffffffff";
        //ColorMode _colorMode = ColorMode.normal;
        int _width = 1;

        public LineStyle(string rrggbbaa, ColorMode mode, int width)
        {
            this._color = rrggbbaa;
            this._colorMode = mode;
            this._width = width;
        }

        public override void WriteTo(XmlTextWriter tw)
        {
            tw.WriteStartElement("LineStyle");
            tw.WriteElementString("width", _width.ToString());
            if (_color != null && _color.Length == 8)
            {
                tw.WriteElementString("color", KmlWriter.RGBAtoABGR(_color));
            }
            tw.WriteElementString("colorMode", this._colorMode.ToString());
            tw.WriteEndElement();//LineStyle
        }
    }
}
