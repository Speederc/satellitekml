using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace KmlUtils
{
    public class Placemark : IKml
    {
        string _name = "";
        string _description = null;
        string _styleUrl = null;
        string _snippet = null;
        bool _visibility;
        IKml _subPlacemark;
        //AltitudeMode _altMode = AltitudeMode.clampToGround;

        public Placemark(string name, string desc, string styleUrl, string snippet, bool visibility, IKml subPlacemark)
        {
            this._name = name;
            this._description = desc;
            this._styleUrl = styleUrl;
            this._snippet = snippet;
            this._visibility = visibility;
            this._subPlacemark = subPlacemark;
        }

        public void WriteTo(XmlTextWriter tw)
        {
            tw.WriteStartElement("Placemark");
            tw.WriteElementString("name", this._name);
            if(!_visibility)
                tw.WriteElementString("visibility", this._visibility ? "1" : "0");
            if (this._snippet != null)
                tw.WriteElementString("Snippet", this._snippet);
            if(this._description != null)
                KmlWriter.WriteDescription(this._description, tw);
            if (this._styleUrl != null)
                KmlWriter.WriteStyleUrl(this._styleUrl, tw);

            this._subPlacemark.WriteTo(tw);

            tw.WriteEndElement();//Placemark
            
        }
    }
}
