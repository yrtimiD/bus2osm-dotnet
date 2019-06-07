
using System;
using System.Collections.Generic;

namespace bus2osm.DTO
{


	public class OneWayLine
	{
		public Dictionary<string, string> CompanyName { get; set; }
		public string CompanyID { get; set; }

		public string Number { get; set; }
		public Dictionary<string, string> Name { get; set; }
		public Dictionary<string, string> From { get; set; }
		public Dictionary<string, string> To { get; set; }
		
		public SortedList<int, Stop> Stops { get; set; }
				
		public string LineCode { get; set; }
		
		public OneWayLine ()
		{
			CompanyName = new Dictionary<string, string> ();
			Name = new Dictionary<string, string> ();
			From = new Dictionary<string, string> ();
			To = new Dictionary<string, string> ();
			Stops = new SortedList<int, Stop> ();
		}
	}
}
