using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;

namespace KmlUtils
{
    public class LineString : IKml
    {
        AltitudeMode _altMode = AltitudeMode.clampToGround;
        List<ICoordinate> _coords;
        bool _extrude;
        bool _tessellate;

        public LineString(AltitudeMode altMode, List<ICoordinate> coords, bool extrude, bool tessellate)
        {
            this._altMode = altMode;
            this._coords = coords;
            this._extrude = extrude;
            this._tessellate = tessellate;
        }

        public void WriteTo(XmlTextWriter tw)
        {
            tw.WriteStartElement("LineString");
            if(!this._tessellate && this._altMode != AltitudeMode.clampToGround)
                tw.WriteElementString("altitudeMode", this._altMode.ToString());
            if (this._tessellate)
                tw.WriteElementString("tessellate", "1");
            if (this._extrude)
                tw.WriteElementString("extrude", "1");
            KmlWriter.WriteCoordinates(this._coords, tw);
            tw.WriteEndElement();//LineString
        }

        //public ICoordinate Coordinate
        //{
        //    get { return _loc; }
        //    set { _loc = value; }
        //}

       
    }
}
