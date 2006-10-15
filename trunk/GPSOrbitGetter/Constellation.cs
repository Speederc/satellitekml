using System;
using System.Collections;
using System.Reflection;

using OrbitTools;

namespace GPSOrbitGetter
{
	/// <summary>
	/// Summary description for Constellation.
	/// </summary>
    //public class Constellation
    //{
    //    //private DJTSatLib.ISatellites _sats;
    //    private ArrayList _satellites;

    //    public Constellation(string keplerPath)
    //    {
    //        // Create new COM Object and set source path
    //        //this._sats = new DJTSatLib.SatellitesClass();

    //        // Get Correct Path
    //        int length = Assembly.GetExecutingAssembly().Location.Length;
    //        string path = Assembly.GetExecutingAssembly().Location.Trim();
    //        path = path.Remove(path.LastIndexOf("\\"), path.Length - path.LastIndexOf("\\"));//Replace(".exe", "");//Remove(path.Length-4,4);
    //        //this._sats.KeplerPath = path + "\\" + keplerPath;

    //        // Get List of Satellite Names
    //        this._satellites = new ArrayList();
    //        //string [] split = this._sats.SatNames.Split(new Char [] {'"' , ',' });//separate the satellites
    //        foreach (string s in split)
    //        {
    //            if (s.Trim() != "")
    //                 this._satellites.Add(s);
    //        }
    //    }

    //    public Satellite MakeOrbit(Satellite s)
    //    {
    //        // Make an orbit for this satellite
    //        DateTime now = DateTime.UtcNow;
    //        for(int i=0;i<144;i++)
    //        {
    //            DateTime time = now + new TimeSpan(0,0,i*5,0,0);
    //            Point p = this.GetAtTime(s.Name, time).InitLocation;
    //            s.AddOrbitPoint(p);
    //        }

    //        return s;
    //    }

    //    public Satellite GetAtTime(string satName, DateTime utcTime)
    //    {
    //        Satellite s = null;
    //        double jt, az, elv, phase, lon, lat;
    //        double range, range_rate, alt;
    //        bool south;
    //        double x, y, z, xdot, ydot, zdot, vdot;

    //        // Get Satellite params at a certain time
    //        jt = this._sats.DateTimeToJT(utcTime.ToOADate());
    //        this._sats.IsVisible(satName, jt, 2, out south, out az, out elv,
    //            out lon, out lat, out range, out range_rate, out alt, out phase );
		
    //        this._sats.SatState(out x,out y,out z,out xdot,out ydot,out zdot);

    //        vdot = Math.Sqrt(Math.Pow(xdot,2.0) + Math.Pow(ydot, 2.0) + Math.Pow(zdot, 2.0));
    //        s = new Satellite(satName, jt, az, elv, phase, lon, lat, range, range_rate, alt, south, x, y, z, xdot, ydot, zdot, vdot);

    //        return s;
    //    }

    //    public void SetLocation(double latDeg, double lonDeg, double altMeters)
    //    {
    //        this._sats.ObsLat = latDeg;
    //        this._sats.ObsLon = lonDeg;
    //        this._sats.ObsHeight = altMeters;
    //    }
    //    public string[] Names
    //    {
    //        get
    //        {
    //            return (string[])this._satellites.ToArray(Type.GetType("System.String"));
    //        }
    //    }
    //}
}
