using System;
using System.Collections.Generic;
using System.Text;

namespace KmlUtils
{
    public interface ICoordinate : IKml
    {
        double Lat { get;}
        double Lon { get;}
        double AltMeters { get;}
    }
}
