using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace KmlUtils
{
    public abstract class ASubStyle : IKml
    {
        protected string _color = "ffffffff";
        protected ColorMode _colorMode = ColorMode.normal;
        public abstract void WriteTo(XmlTextWriter tw);
    }
}
