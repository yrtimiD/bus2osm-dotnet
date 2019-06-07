
using System;
using System.IO;
using System.Net;

namespace bus2osm
{


	public static class Importer
	{

		public static TextReader Import (Uri uri)
		{
			WebRequest request = HttpWebRequest.Create (uri);
			WebResponse response = request.GetResponse ();
			
			string s = new StreamReader (response.GetResponseStream ()).ReadToEnd ();
			return new StringReader(s);
		}
		
	}
}
