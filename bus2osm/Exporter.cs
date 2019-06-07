using System;
using System.Collections.Generic;
using System.Text;
using bus2osm.DTO;
using System.Xml.Serialization;

namespace bus2osm
{
	public static class Exporter
	{
		private static readonly string defaultLanguage = "he";
		
		private static bool Validate (OneWayLine lineFW, OneWayLine lineBW)
		{
			return Validate (lineFW) && Validate(lineBW);
		}
		
		private static bool Validate(OneWayLine line)
		{
			foreach (var stop in line.Stops.Values)
			{
				if (!stop.HasLatLon) return false;
			}

			return true;
		}

		public static void CreateOsm (OneWayLine lineFW, OneWayLine lineBW, string fileName)
		{
			if (false == Validate (lineFW, lineBW))
				throw new ArgumentException ("Lines isn't valid");
			
			int id = -1;

			osm osm = new osm();
			osm.generator = "bus2osm";
			osm.version = 0.6M;

			Dictionary<string, int> assignedIDs = new Dictionary<string, int>();
			List<osmNode> nodes = new List<osmNode>();
			List<osmRelationMember> fw = new List<osmRelationMember>();
			List<osmRelationMember> bw = new List<osmRelationMember>();

			foreach (var stop in lineFW.Stops.Values)
			{
				if (!assignedIDs.ContainsKey(stop.PlaceID))
				{
					assignedIDs[stop.PlaceID] = id--;
					
					nodes.Add( CreateNode(assignedIDs, stop) );
				}

				fw.Add(new osmRelationMember() { @ref = assignedIDs[stop.PlaceID].ToString(), role = osmRelationMemberRole.stop, type = "node" });
			}

			foreach (var stop in lineBW.Stops.Values)
			{
				if (!assignedIDs.ContainsKey(stop.PlaceID))
				{
					assignedIDs[stop.PlaceID] = id--;
					nodes.Add(CreateNode(assignedIDs, stop));
				}

				bw.Add(new osmRelationMember() { @ref = assignedIDs[stop.PlaceID].ToString(), role = osmRelationMemberRole.stop, type = "node" });
			}

			osm.node = nodes.ToArray();
			
			List<osmTag> tags;
			#region fw relation
			tags = new List<osmTag>();
			tags.Add(new osmTag(){k="type", v="route"});
			tags.Add(new osmTag(){k="route", v="bus"});
			tags.Add(new osmTag(){k="ref", v=lineFW.Number});
			tags.AddRange(GetAllLangTags (lineFW.Name, "name"));
			tags.AddRange(GetAllLangTags (lineFW.CompanyName, "operator"));
			tags.AddRange(GetAllLangTags (lineFW.From, "from"));
			tags.AddRange(GetAllLangTags (lineFW.To, "to"));
			
			osmRelation fwRel = new osmRelation()
			{
				timestamp = DateTime.Now,
				tag = tags.ToArray(),
				visible = true,
				member = fw.ToArray(),
				id = id--
			};
			#endregion
			
			#region bw relation
			tags = new List<osmTag>();
			tags.Add(new osmTag(){k="type", v="route"});
			tags.Add(new osmTag(){k="route", v="bus"});
			tags.Add(new osmTag(){k="ref", v=lineBW.Number});
			tags.AddRange(GetAllLangTags (lineBW.Name, "name"));
			tags.AddRange(GetAllLangTags (lineBW.CompanyName, "operator"));
			tags.AddRange(GetAllLangTags (lineBW.From, "from"));
			tags.AddRange(GetAllLangTags (lineBW.To, "to"));

			osmRelation bwRel = new osmRelation()
			{
				timestamp = DateTime.Now,
				tag = tags.ToArray(),
				visible = true,
				member = bw.ToArray(),
				id = id--
			};
			#endregion
			
			osm.relation = new osmRelation[] { fwRel, bwRel };

			XmlSerializer ser = new XmlSerializer(typeof(osm));
			ser.Serialize(System.IO.File.OpenWrite(fileName), osm);
		}
		
		private static IEnumerable<osmTag> GetAllLangTags (Dictionary<string, string> langToValue, string tagName)
		{
			List<osmTag> tags = new List<osmTag> ();
			foreach (var lang in langToValue) 
			{
				tags.Add (new osmTag(){k=tagName+":"+lang.Key, v=lang.Value});
				if (lang.Key == defaultLanguage) tags.Add(new osmTag(){k=tagName, v=lang.Value});
			}
			
			return tags;
		}

		private static osmNode CreateNode(Dictionary<string, int> assignedIDs, Stop stop)
		{
			osmNode node = new osmNode() { id = assignedIDs[stop.PlaceID], lat = stop.Lat, lon = stop.Lon, visible = true, timestamp = DateTime.Now };
			List<osmTag> tags = new List<osmTag>();
			tags.AddRange(GetAllLangTags(stop.Name, "name"));
			tags.Add(new osmTag() { k = "highway", v = "bus_stop" });
			if (stop.IsApproximated)
			{
				tags.Add(new osmTag() { k = "fixme", v = "Find exact node position" });
			}
			node.tag = tags.ToArray();
			return node;
		}
	}
}
