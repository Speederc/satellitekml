using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace GPSOrbitGetter
{
    class HttpRequestParser
    {
        public static string GetHttpResource(string uri)
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

            string resp = sb.ToString();

            return resp;
            //lock (this._satellites)
            //{
            //    this.Parse(twoLineElements);
            //}

        }
    }
}
