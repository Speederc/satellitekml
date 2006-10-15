using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;

namespace KmlUtils
{
    public interface IKml
    {
        void WriteTo(XmlTextWriter tw);
    }
}
