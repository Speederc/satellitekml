using System;
using System.Collections.Generic;
using System.Text;

namespace KmlUtils
{
    public class ScreenOverlay : IKml
    {
        string _name = "";
        string _snippet = null;
        bool _visibility;
        string _imgHref;
        ScreenXY _sxy;
        OverlayXY _oxy;
        Size _size;
        RefreshMode _refreshMode = RefreshMode.once;
        int _refreshInterval = 300; //secs

        public ScreenOverlay(string name, string snippet, bool visibility, string imgHref, 
            ScreenXY sxy, OverlayXY oxy, Size size, RefreshMode mode, int refreshInterval)
        {
            this._name = name;
            this._snippet = snippet;
            this._visibility = visibility;
            this._imgHref = imgHref;
            this._sxy = sxy;
            this._oxy = oxy;
            this._size = size;
            this._refreshMode = mode;
            this._refreshInterval = refreshInterval;
        }

        #region IKml Members

        public void WriteTo(System.Xml.XmlTextWriter tw)
        {
            tw.WriteStartElement("ScreenOverlay");
            if(!_visibility)
                tw.WriteElementString("visibility", _visibility ? "1" : "0");
            tw.WriteElementString("name", _name);
            tw.WriteStartElement("Icon");
            tw.WriteElementString("href", _imgHref);
            if (this._refreshMode == RefreshMode.onInterval)
            {
                tw.WriteElementString("refreshMode", this._refreshMode.ToString());
                tw.WriteElementString("refreshInterval", this._refreshInterval.ToString());
            }
            tw.WriteEndElement();//Icon
            _oxy.WriteTo(tw);
            _sxy.WriteTo(tw);
            _size.WriteTo(tw);
            tw.WriteEndElement();//ScreenOverlay

        }

        #endregion
    }
}
