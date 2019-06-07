
using System;
using System.Collections.Generic;

namespace bus2osm.DTO
{


	public class Stop
	{
		public decimal Lat { get; set; }
		public decimal Lon { get; set; }
		
		public Dictionary<string, string> Name { get; set; }
		
		public string PlaceID { get; set; }

		public bool IsApproximated { get; set; }

		public bool HasLatLon
		{
			get 
			{
				return Lat != 0 && Lon != 0;
			}
		}
		
		public Stop ()
		{
			Name = new Dictionary<string, string> ();
		}
		
		public override string ToString()
		{
			return String.Format("{0}: {1}/{2}", Name, Lat, Lon);
		}
	}
}