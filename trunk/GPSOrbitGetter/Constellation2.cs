using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using OrbitTools;

namespace GPSOrbitGetter
{
    public class Constellation2
    {
        private Dictionary<string, Satellite2> _satellites;
        private string _uri;

        public Constellation2(string fullDoc, string uri)
        {
            this._satellites = this.Parse(fullDoc);
            this._uri = uri;
        }

        public List<Satellite2> Satellites
        {
            get {
                List<Satellite2> ret = new List<Satellite2>();
                lock (this._satellites)
                {
                    foreach (Satellite2 s2 in this._satellites.Values)
                        ret.Add(s2);
                }
                return ret;
            }
        }

        public Dictionary<string, Satellite2> NameMap
        {
            get
            {
                lock (this._satellites)
                {
                    return new Dictionary<string, Satellite2>(this._satellites);
                }
            }
        }

        public Satellite2 GetSatellite(string satName)
        {
            Satellite2 ret = new Satellite2();
            lock (this._satellites)
            {
                if(this._satellites.ContainsKey(satName))
                    ret = this._satellites[satName];
            }
            return ret;
        }

        public void SetTime(DateTime utc)
        {
            utc = Globals.InsureUTC(utc);
            List<Satellite2> toRemove = new List<Satellite2>();
            lock(this._satellites)
            {               
                foreach (Satellite2 sat in this._satellites.Values)
                {
                    bool success = sat.SetTime(utc);
                    if (!success)
                    {
                        toRemove.Add(sat);
                    }
                }
            }

            foreach (Satellite2 sat in toRemove)
                this.RemoveSatellite(sat.NameNumber);
        }
        public string URI
        {
            get { return this._uri; }
        }

        public void RemoveSatellite(string name)
        {
            lock (this._satellites)
            {
                if (this._satellites.ContainsKey(name))
                    this._satellites.Remove(name);
            }
        }
        private Dictionary<string, Satellite2> Parse(string tles)
        {
            Dictionary<string, Satellite2> ret = new Dictionary<string, Satellite2>();

            StringReader sr = new StringReader(tles);
            try
            {
                while (true)
                {
                    string s1 = sr.ReadLine();
                    if (s1 == null)
                        break;
                    string s2 = sr.ReadLine();
                    if (s2 == null)
                        break;
                    string s3 = sr.ReadLine();
                    if (s3 == null)
                        break;

                    Tle tle = new Tle(s1, s2, s3);

                    //if (tle.getName().Contains("BIIA-22"))
                    //{
                        Satellite2 st = new Satellite2(tle);
                        ret.Add(st.NameNumber, st);
                    //}

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //return new Dictionary<string, Satellite2>();
            }

            return ret;
        }
    }
}
