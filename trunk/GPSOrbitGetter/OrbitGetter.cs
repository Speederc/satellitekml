using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Net;

using OrbitTools;

namespace GPSOrbitGetter
{
	/// <summary>
	/// Summary description for OrbitGetter.
	/// </summary>
	public class OrbitGetter
	{
		private Thread _tleThread;
		private int _updateInterval = 60 * (1000*60); // ms
        private Dictionary<string, Constellation2> _constellations;

        /// <summary>
        /// Create an OrbitGetter object with its update interval set to <i>updateIntervalMinutes</i>
        /// </summary>
        /// <param name="updateIntervalMinutes">the frequency, in minutes, to update TLE files.</param>
		public OrbitGetter(int updateIntervalMinutes)
		{
            this._updateInterval = updateIntervalMinutes * 60 * 1000; // ms
            this._constellations = new Dictionary<string, Constellation2>();
			this._tleThread = new Thread(new ThreadStart(this.UpdateConstellations));
			this._tleThread.Start();
		}

        /// <summary>
        /// Adds a Satellite Constellation to the OrbitGetter
        /// </summary>
        /// <param name="uri">The URI of the TLE file to parse the constellation from.</param>
        public void AddConstellation(string name, string uri)
        {
            string twoLineElements = this.GetDocFromUri(uri);
            Constellation2 c2 = new Constellation2(twoLineElements, uri);
            lock (this._constellations)
            {
                this._constellations[name] = c2; // will replace
            }
        }

        // ShutDown the TLE Updater Thread.
        public void ShutDown()
        {
            this._tleThread.Abort();
        }

        public void ClearConstellations()
        {
            lock (this._constellations)
            {
                this._constellations.Clear();
            }
        }

        public void RemoveConstellation(string name)
        {
            lock (this._constellations)
            {
                if (this._constellations.ContainsKey(name))
                    this._constellations.Remove(name);
            }
        }

        /// <summary>
        /// Gets a constellation from the Constellation collection.
        /// </summary>
        /// <param name="constellationName">the name of th constellation to get.</param>
        /// <returns>the constellation with key <i>name</i>, or null if it doesn't exist.</returns>
        public Constellation2 GetConstellation(string constellationName)
        {
            Constellation2 c2 = null;
            lock (this._constellations)
            {
                c2 = this._constellations[constellationName];
            }
            return c2;
        }

        public bool SetTimeForConstellation(string constellationName, DateTime utc)
        {
            utc = Globals.InsureUTC(utc);
            lock (this._constellations)
            {
                if(this._constellations.ContainsKey(constellationName))
                {
                    this._constellations[constellationName].SetTime(utc);
                    return true;
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Gets or Sets the update interval for the thread that retrieves TLE files in minutes.
        /// </summary>
        public int UpdateInterval
        {
            get { return this._updateInterval / 60 / 1000; }
            set { this._updateInterval = value * 60 * 1000;}
        }

        private string GetDocFromUri(string uri)
        {

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            // execute the request
            HttpWebResponse response = (HttpWebResponse)
                request.GetResponse();

            // we will read data via the response stream
            Stream resStream = response.GetResponseStream();

            string tempString = null;
            int count = 0;
            byte[] buf = new byte[4096];
            StringBuilder sb = new StringBuilder();

            do
            {
                // fill the buffer with data
                count = resStream.Read(buf, 0, buf.Length);

                // make sure we read some data
                if (count != 0)
                {
                    // translate from bytes to ASCII text
                    tempString = Encoding.ASCII.GetString(buf, 0, count);

                    // continue building the string
                    sb.Append(tempString);
                }
            }
            while (count > 0); // any more data to read?

            string ret = sb.ToString();
            return ret;
        }
        private void UpdateConstellations()
        {
            while (true)
            {
                try
                {
                    Dictionary<string, Constellation2> newCons = new Dictionary<string, Constellation2>();
                    lock (this._constellations)
                    {
                        foreach (string name in this._constellations.Keys)
                        {
                            string uri = this._constellations[name].URI;
                            string newDoc = this.GetDocFromUri(uri);
                            Constellation2 c2 = new Constellation2(newDoc, uri);
                            newCons[name] = c2;
                        }
                        this._constellations = newCons;
                    }

                    Thread.Sleep(this._updateInterval);
                }
                catch (ThreadInterruptedException)
                {
                    continue;
                }
                catch (ThreadAbortException)
                {
                    return;
                }
            }
        }
	}
}
