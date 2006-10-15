using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace KmlUtils
{
    public class IconStyle : ASubStyle, IKml
    {
        //string _color = "ffffffff";
        //ColorMode _colorMode = ColorMode.normal;
        double _scale = 1.0;
        Icon _icon;
        //string _iconHref = "";
        //int _x, _y, _w, _h;

        public IconStyle(string rrggbbaa, ColorMode mode, double scale, Icon icon)
        {
            this._color = rrggbbaa;
            if(this._color != null)
                KmlWriter.RGBAtoABGR(_color);
            this._colorMode = mode;
            this._scale = scale;
            this._icon = icon;
            //this._iconHref = iconHref;
            //this._x = x;
            //this._y = y;
            //this._w = w;
            //this._h = h;
        }

        public override void WriteTo(XmlTextWriter tw)
        {
            tw.WriteStartElement("IconStyle");
            tw.WriteElementString("scale", _scale.ToString());
            if(_color != null)
                tw.WriteElementString("color", _color);
            tw.WriteElementString("colorMode", this._colorMode.ToString());
            _icon.WriteTo(tw);
            //tw.WriteStartElement("Icon");
            //tw.WriteElementString("href", _iconHref);
            //if (_w != 0 && _h != 0)
            //{
            //    tw.WriteElementString("x", _x.ToString());
            //    tw.WriteElementString("y", _y.ToString());
            //    tw.WriteElementString("w", _w.ToString());
            //    tw.WriteElementString("h", _h.ToString());
            //}
            //tw.WriteEndElement();//Icon
            tw.WriteEndElement(); // IconStyle
        }
    }
}
