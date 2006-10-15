using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace KmlUtils
{
    public class Style
    {
        string _id = "";
        List<ASubStyle> _styles = new List<ASubStyle>();

        public Style(string id, List<ASubStyle> styles)
        {
            this._id = id;
            this._styles = styles;
        }

        public void WriteTo(XmlTextWriter tw)
        {
            tw.WriteStartElement("Style");
            if(_id != null)
                tw.WriteAttributeString("id", this._id);

            foreach (ASubStyle style in this._styles)
            {
                style.WriteTo(tw);
            }
            tw.WriteEndElement();//Style
        }
    }
}
