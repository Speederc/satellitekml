using System;
using System.Collections.Generic;
using System.Text;

namespace KmlUtils
{
    public class GroundOverlay : IKml
    {
        Icon _icon;//required
        LatLonBox _latLonBox;//required
        bool _visibility;//optional
        string _name;//optional
        string _color;//optional
        int _drawOrder;//optional
        LookAt _lookAt;//optional

        /// <summary>
        /// Simplest constructor with only required elements
        /// </summary>
        /// <param name="icon">the Icon object that defines the image of the overlay.</param>
        /// <param name="box">the LatLonBox that defines the bounds of the overlay.</param>
        public GroundOverlay(Icon icon, LatLonBox box)
        {
            this._icon = icon;
            this._latLonBox = box;
        }

        public GroundOverlay(Icon icon, LatLonBox box, LookAt lookat, string name, string rrggbbaa, int drawOrder, bool visibility)
            : this(icon, box)
        {
            this._lookAt = lookat;
            this._name = name;
            this._color = rrggbbaa;
            this._drawOrder = drawOrder;
            this._visibility = visibility;
        }

        #region IKml Members

        public void WriteTo(System.Xml.XmlTextWriter tw)
        {
            tw.WriteStartElement("GroundOverlay");
            if(this._name != null)
                tw.WriteElementString("name", _name);
            if (this._lookAt != null)
                _lookAt.WriteTo(tw);
            if (!_visibility) // default is on
                tw.WriteElementString("visibility", _visibility ? "1" : "0");
            if(_color != null)
                tw.WriteElementString("color", KmlWriter.RGBAtoABGR(_color));
            if(_drawOrder != 0)
                tw.WriteElementString("drawOrder", _drawOrder.ToString());
            _icon.WriteTo(tw);
            _latLonBox.WriteTo(tw);
            tw.WriteEndElement();//GroundOverlay
        }

        #endregion
    }
}
