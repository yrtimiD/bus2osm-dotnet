using System;

namespace bus2osm
{
	class MainClass
	{
//		static string lineBothDirs = @"http://www.bus.co.il/otobusim/Front2007/LinePlaces.asp?CompanyID=61&LineCode=115&Design=2007&LanguageID=10";
//		static string lineBothDirsTemplate = @"http://www.bus.co.il/otobusim/Front2007/LinePlaces.asp?CompanyID={0}&LineCode={1}&Design=2007&LanguageID={2}";
//		static string lineWithMap = @"http://www.bus.co.il/otobusim/Front2007/PlacesMap.asp?LineCompanyID=61&LineCode=115&LineAlternateCode=0&LineDirection=1&PlaceID1=84415&LanguageID=10&Design=2007";
//		static string allLines = @"http://www.bus.co.il/otobusim/Front2007/Lines.asp?OperatingCompanyID=61&LanguageID=&Design=";
		
		public static void Main (string[] args)
		{
			//args = new string[] { "61", "102", "10" };
			
			if (args.Length == 0 || args.Length != 2) 
			{
				Console.WriteLine ("Using:");
				Console.WriteLine ("bus2osm [CompanyID] [LineCode]");
				Console.WriteLine ();
			}
			else 
			{
				string CompanyID = args[0];
				string LineCode = args[1];
				try
				{
					BusCoIlParser.DefaultLon = 34.797315M;
					BusCoIlParser.DefaultLat = 31.24277M;
		
					DTO.OneWayLine lineFW = new DTO.OneWayLine ();
					DTO.OneWayLine lineBW = new DTO.OneWayLine ();
		
					BusCoIlParser.GetLineInfo (CompanyID, LineCode, lineFW, lineBW);
		
					Console.WriteLine ("FW: "+lineFW.Stops.Count);
					Console.WriteLine ("BW: "+lineBW.Stops.Count);
		
					
					Exporter.CreateOsm(lineFW, lineBW, lineFW.Number+".osm");
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
	
			}
			//Console.WriteLine("Done");
			//Console.ReadKey();
		}


	}
}
