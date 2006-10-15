using System;
using System.Collections.Generic;
using System.Text;

namespace KmlUtils
{
    public enum ColorMode
    {
        normal = 0,
        random = 1
    }

    public enum ListItemType
    {
        checkHideChildren = 0,
        radioFolder = 1
    }

    /*
     * clampToGround - (default) Indicates to ignore an altitude specification (for example, in the <coordinates> tag).
     * relativeToGround - Sets the altitude of the element relative to the actual ground elevation of a particular location. If the ground elevation of a location is exactly at sea level and the altitude for a point is set to 9 meters, then the placemark elevation is 9 meters with this mode. However, if the same placemark is set over a location where the ground elevation is 10 meters above sea level, then the elevation of the placemark is 19 meters.
     * absolute - Sets the altitude of the element relative to sea level, regardless of the actual elevation of the terrain beneath the element. For example, if you set the altitude of a placemark to 10 meters with an absolute altitude mode, the placemark will appear to be at ground level if the terrain beneath is also 10 meters above sea level. If the terrain is 3 meters above sea level, the placemark will appear elevated above the terrain by 7 meters. A typical use of this mode is for aircraft placement. 
     */
    public enum AltitudeMode
    {
        clampToGround = 1,
        relativeToGround = 2,
        absolute = 3
    }

    public enum ScreenUnits
    {
        pixels = 0,
        fraction = 1
    }

    public enum RefreshMode
    {
        onInterval = 0,
        onExpire = 1,
        once
    }
}