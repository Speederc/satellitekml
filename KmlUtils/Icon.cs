using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;

namespace KmlUtils
{
    public class Icon : IKml
    {
        string _href;
        RefreshMode _refreshMode = RefreshMode.once;
        int _refreshInterval;
        int _x, _y, _w, _h;

        public Icon(string href, RefreshMode refreshMode, int refreshInterval, int x, int y, int w, int h)
        {
            this._href = href;
            this._refreshMode = refreshMode;
            this._refreshInterval = refreshInterval;
            this._x = x;
            this._y = y;
            this._w = w;
            this._h = h;
        }

        #region IKml Members

        public void WriteTo(XmlTextWriter tw)
        {
            tw.WriteStartElement("Icon");
            if (_href != null)
            {
                tw.WriteElementString("href", _href);

                if (_w != 0 && _h != 0)
                {
                    tw.WriteElementString("x", _x.ToString());
                    tw.WriteElementString("y", _y.ToString());
                    tw.WriteElementString("w", _w.ToString());
                    tw.WriteElementString("h", _h.ToString());
                }
                if (_refreshMode != null && _refreshMode == RefreshMode.onInterval)
                {
                    tw.WriteElementString("refreshMode", _refreshMode.ToString());
                    tw.WriteElementString("refreshInterval", _refreshInterval.ToString());
                }
            }
            tw.WriteEndElement();//Icon
        }

        #endregion
    }
}
