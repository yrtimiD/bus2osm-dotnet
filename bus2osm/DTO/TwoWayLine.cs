
using System;
using System.Collections.Generic;

namespace bus2osm.DTO
{


	public class TwoWayLine
	{
		public string CompanyName { get; set; }
		public string CompanyID { get; set; }

		public string Number { get; set; }
		public string ForwardName { get; set; }
		public string BackwardName { get; set; }
		
		public SortedList<int, Stop> ForwardStops { get; set; }
		public SortedList<int, Stop> BackwardStops { get; set; }
		
		public string LineCode { get; set; }
		
		public TwoWayLine ()
		{
			ForwardStops = new SortedList<int, Stop> ();
			BackwardStops = new SortedList<int, Stop> ();
		}
	}
}
