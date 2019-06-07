
using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Diagnostics;
using bus2osm.DTO;

namespace bus2osm
{
	public static class BusCoIlParser
	{
		public static readonly string lineBothDirsTemplate = @"http://www.bus.co.il/otobusim/Front2007/LinePlaces.asp?CompanyID={0}&LineCode={1}&LanguageID={2}";
		
		public static readonly string Company = @"http://www.bus.co.il/otobusim/Front2007/Company.asp?ID={0}&LanguageID={1}";
		public static readonly string AllCompanyLines = @"http://www.bus.co.il/otobusim/Front2007/Lines.asp?OperatingCompanyID={0}&LanguageID={1}";
		public static readonly string LineStops = @"http://www.bus.co.il/otobusim/Front2007/LinePlaces.asp?CompanyID={0}&LineCode={1}&LanguageID={2}";
		public static readonly string LineForwardMap = @"http://www.bus.co.il/otobusim/Front2007/PlacesMap.asp?LineCompanyID={0}&LineCode={1}&LineAlternateCode=0&LineDirection=1&LanguageID={2}";
		public static readonly string LineBackwardMap = @"http://www.bus.co.il/otobusim/Front2007/PlacesMap.asp?LineCompanyID={0}&LineCode={1}&LineAlternateCode=0&LineDirection=2&LanguageID={2}";

		public static readonly Uri baseUri = new Uri(@"http://www.bus.co.il/otobusim/Front2007/");

		public static Dictionary<string, string> langs = new Dictionary<string, string>(){{"he","10"}, {"en","20"}};
		
		public static decimal DefaultLat { get; set; }
		public static decimal DefaultLon { get; set; }

		/// <summary>
		/// accepts http://www.bus.co.il/otobusim/Front2007/LinePlaces.asp?CompanyID=61&LineCode=115&LanguageID=10
		/// </summary>
		public static void GetLineInfo (string CompanyID, string LineCode, OneWayLine lineFW, OneWayLine lineBW)
		{
			lineFW.CompanyID = lineBW.CompanyID = CompanyID;
			lineFW.LineCode = lineBW.LineCode = LineCode;

			foreach (var lang in langs) 
			{
				string langName = lang.Key;
				string langCode = lang.Value;
				
				Uri companyUri = new Uri (String.Format (BusCoIlParser.Company, lineFW.CompanyID, langCode));
				lineFW.CompanyName[langName] = lineBW.CompanyName[langName] = GetSingleValue (companyUri, "//h1[@class='FormTitle']", null);

				Uri sourceLink = new Uri (String.Format (lineBothDirsTemplate, CompanyID, LineCode, langCode));
				lineFW.Number = lineBW.Number = GetSingleValue (sourceLink, "//td[@class='FormTitle']", null);
				GetOrderedStops (sourceLink, langName, lineFW, lineBW);
			}

			GetStopsCoords (lineFW, lineBW, langs["he"]);

			foreach (var lang in langs.Keys) 
			{
				CleanAllNames (lineFW.Stops.Values, lang);
				CleanAllNames (lineBW.Stops.Values, lang);
			}
			
			if (! ApproximateMissed (lineFW.Stops.Values))
			{
				SetDefaultCoords (lineFW.Stops.Values);
			}
			if (! ApproximateMissed (lineBW.Stops.Values))
			{
				SetDefaultCoords(lineBW.Stops.Values);
			}

		}

		/// <summary>
		/// Gets ordered list of stops in both directions
		/// accepts http://www.bus.co.il/otobusim/Front2007/LinePlaces.asp?CompanyID=61&LineCode=115&LanguageID=10
		/// </summary>
		private static bool GetOrderedStops (Uri sourceLink, string langName, OneWayLine lineFW, OneWayLine lineBW)
		{
			try {
				
				TextReader reader = Importer.Import (sourceLink);
				
				HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument ();
				doc.Load (reader);
				
				HtmlAgilityPack.HtmlNodeCollection routes = doc.DocumentNode.SelectNodes ("//table[@id='Table1']");
				
				ParseStopsTable (lineFW, routes[0], langName);
				
				ParseStopsTable (lineBW, routes[1], langName);
				
				return true;
			} 
			catch (Exception ex) 
			{
				Console.WriteLine (ex.Message);
				return false;
			}
		}

		private static void ParseStopsTable(OneWayLine line, HtmlAgilityPack.HtmlNode routeNode, string langName)
		{
			int stopCounter = 0;

			HtmlAgilityPack.HtmlNode titleNode = routeNode.SelectNodes("tr[1]/td[1]/table[1]/tr[1]/td[1]")[0];

			line.From[langName] = titleNode.ChildNodes[0].InnerText.Trim();
			line.To[langName] = titleNode.ChildNodes[4].InnerText.Trim();
			line.Name[langName] = String.Format("{0} - {1}", line.From[langName], line.To[langName]);

			HtmlAgilityPack.HtmlNodeCollection rows = routeNode.SelectNodes("tr[1]/td[1]/table[1]/tr[@class='SmallTableRow ']");

			if (rows == null)
			{
				Console.WriteLine(routeNode.InnerHtml);
				Console.WriteLine("null collection");
			}
			else
			{
				foreach (HtmlAgilityPack.HtmlNode stopRowNode in rows)
				{
					//Console.WriteLine (stopNode.InnerHtml);
					string stopName = stopRowNode.ChildNodes[1].InnerHtml.Trim();
					//Console.WriteLine("Name: " + stopName);

					HtmlAgilityPack.HtmlNode linkNode = stopRowNode.ChildNodes[7].ChildNodes[1];
					string link = linkNode.Attributes["href"].Value;
					//Console.WriteLine("Link: " + link);

					Uri linkUri = null;
					Uri.TryCreate(BusCoIlParser.baseUri, link, out linkUri);
					NameValueCollection col = System.Web.HttpUtility.ParseQueryString(linkUri.Query);
					
					if ( line.Stops.ContainsKey(stopCounter) )
					{
						line.Stops[stopCounter].Name[langName] = stopName;
						stopCounter++;
					}
					else 
					{
						Stop s = new Stop() { PlaceID = col["PlaceID"]??col["PlaceID1"] };
						s.Name[langName] = stopName;

						if (s.PlaceID == null) throw new ApplicationException();

						line.Stops.Add(stopCounter++, s);
					}

					//Console.WriteLine("----");
				}
			}
		}

		public static void GetStopsCoords (OneWayLine lineFW, OneWayLine lineBW, string langCode)
		{
			#region forward direction
			{
				Uri forwMapUri = new Uri(String.Format(LineForwardMap, lineFW.CompanyID, lineFW.LineCode, langCode));
				TextReader reader = Importer.Import(forwMapUri);
				HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
				doc.Load(reader);

				Regex coordsRegex = new Regex(@"\s*([\d\.]+)\s*,\s*([\d\.]+)\s*,\s*([\d\.]+)\s*\);\s*$"); //x,y,zoom
				foreach (var stop in lineFW.Stops.Values)
				{
					HtmlAgilityPack.HtmlNode node = doc.DocumentNode.SelectSingleNode(".//tr[@id='PlaceRow" + stop.PlaceID + "']");
					if (node != null)
					{
						HtmlAgilityPack.HtmlNode linkNode = node.SelectSingleNode(".//a");
						if (linkNode != null)
						{
							string posLink = linkNode.Attributes["href"].Value;
							if (posLink.StartsWith("javascript: SelectPlace"))
							{
								Match m = coordsRegex.Match(posLink);
								if (m.Success)
								{
									stop.Lon = decimal.Parse(m.Groups[1].Value);
									stop.Lat = decimal.Parse(m.Groups[2].Value);

								}
							}
						}
					}
					else
					{
						Trace.WriteLine("No place node for stop: " + stop.PlaceID);
					}
				}
			}
			#endregion
			
			#region backward direction
			{
				Uri backwMapUri = new Uri(String.Format(LineBackwardMap, lineBW.CompanyID, lineBW.LineCode, langCode));
				TextReader reader = Importer.Import(backwMapUri);
				HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
				doc.Load(reader);

				Regex coordsRegex = new Regex(@"\s*([\d\.]+)\s*,\s*([\d\.]+)\s*,\s*([\d\.]+)\s*\);\s*$"); //x,y,zoom
				foreach (var stop in lineBW.Stops.Values)
				{
					HtmlAgilityPack.HtmlNode node = doc.DocumentNode.SelectSingleNode(".//tr[@id='PlaceRow" + stop.PlaceID + "']");
					if (node != null)
					{
						HtmlAgilityPack.HtmlNode linkNode = node.SelectSingleNode(".//a");
						if (linkNode != null)
						{
							string posLink = linkNode.Attributes["href"].Value;
							if (posLink.StartsWith("javascript: SelectPlace"))
							{
								Match m = coordsRegex.Match(posLink);
								if (m.Success)
								{
									stop.Lon = decimal.Parse(m.Groups[1].Value);
									stop.Lat = decimal.Parse(m.Groups[2].Value);

								}
							}
						}
					}
					else
					{
						Trace.WriteLine("No place node for stop: " + stop.PlaceID);
					}
				}

			}
			#endregion
		}

		private static void CleanAllNames(IList<Stop> stops, string lang)
		{
			if (stops.Count == 0) return;
			#region find common prefix
			string prefix = "";
			bool allHave = true;
			while (allHave)
			{
				prefix = stops[0].Name[lang].Substring(0, prefix.Length + 1);

				foreach (var stop in stops)
				{
					if (!stop.Name[lang].StartsWith(prefix)) { allHave = false; break; }
				}
			}
			#endregion

			if (prefix.Length > 0)
			{
				prefix = prefix.Remove(prefix.Length - 1, 1);

				foreach (var stop in stops)
				{
					stop.Name[lang] = CleanName(prefix, stop.Name[lang]);
				}
			}
		}

		private static string CleanName(string junk, string name)
		{
			return name.Replace(junk, "").Trim();
		}

		private static bool ApproximateMissed(IList<Stop> stops)
		{
			if (stops.Count == 0) return true;

			int ai = -1;
			int bi = -1;

			int i = 0;
			for (; i < stops.Count; i++)
			{
				if (stops[i].HasLatLon)
				{
					ai = i;
					for (int t = 0; t < i; t++)
					{
						stops[t].IsApproximated = true;
						stops[t].Lat = stops[ai].Lat;
						stops[t].Lon = stops[ai].Lon;
					}
					break;
				}
			}
			
			if (ai == -1) return false;

			ai = i;
			bi = -1;
			while(i<stops.Count-1)
			{
				i++;
				if (stops[i].HasLatLon)
				{
					ai = i;
				}
				else 
				{
					ai = i - 1;
					i++;
					do
					{
						if (stops[i].HasLatLon) { bi = i; break; }
						i++;
					} while (i < stops.Count);
					if (bi != -1)
					{
						Interpolate(stops, ai, bi);
						ai = bi;
						bi = -1;
					}

				}
			}

			return true;
		}

		public static void Interpolate(IList<Stop> stops, int ai, int bi)
		{
			for (int i = ai+1; i < bi; i++)
			{
				stops[i].IsApproximated = true;
				stops[i].Lat = stops[ai].Lat + (stops[bi].Lat - stops[ai].Lat) / (bi - ai) * (i - ai);
				stops[i].Lon = stops[ai].Lon + (stops[bi].Lon - stops[ai].Lon) / (bi - ai) * (i - ai);
			}
		}

		private static void SetDefaultCoords(IList<Stop> stops)
		{
			foreach (var stop in stops)
			{
				if (!stop.HasLatLon)
				{
					stop.IsApproximated = true;
					stop.Lat = DefaultLat;
					stop.Lon = DefaultLon;
				}
			}
		}
		

		#region tools

		private static void PrintNodes (HtmlAgilityPack.HtmlNode node)
		{
			foreach (var child in node.ChildNodes) 
			{
				
				Console.WriteLine ("{0}, {1} -> {2}", node.ChildNodes.IndexOf(child), child.Name, child.InnerHtml);
			}
		}

		private static string GetSingleValue(Uri uri, string xpath, string regex)
		{
			TextReader reader = Importer.Import(uri);

			if (!String.IsNullOrEmpty(xpath))
			{
				HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
				doc.Load(reader);

				HtmlAgilityPack.HtmlNode node = doc.DocumentNode.SelectSingleNode(xpath);
				if (node != null)
					return node.InnerText.Trim();
			}
			else if (!String.IsNullOrEmpty(regex))
			{
				String wholeText = reader.ReadToEnd();

				Match m = Regex.Match(wholeText, regex, RegexOptions.IgnoreCase | RegexOptions.Multiline);
				if (m != null && m.Success)
					return m.Groups[1].Value;
			}

			return null;
		}
		#endregion
	}
}
