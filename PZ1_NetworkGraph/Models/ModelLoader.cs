using System;
using System.Collections.Generic;
using System.Xml;

namespace Models
{
	public class ModelLoader
	{
		private XmlDocument doc;

		private LanLonConverter converter;

		private CoordinateScaleConverter scaleConverter;

		public List<double> listaX = new List<double>();

		public List<double> listaY = new List<double>();

		public ModelLoader()
		{
			doc = new XmlDocument();
			converter = new LanLonConverter();
			scaleConverter = new CoordinateScaleConverter();
			Lines = new List<LineEntity>();
			Entities = new List<PowerEntity>();
		}

		public List<LineEntity> Lines { get; set; }

		public List<PowerEntity> Entities { get; set; }

		public void LoadModelFromFile(string path = "Geographic.xml")
		{
			doc.Load(path);
			
			LoadPowerEntities();
			LoadLines();
			countConnections();
			ScaleModel();
		}

		private void LoadLines()
		{
			XmlNodeList nodeList = doc.DocumentElement.SelectNodes("/NetworkModel/Lines/LineEntity");

			foreach (XmlNode node in nodeList)
			{
				LineEntity lineEntity = new LineEntity();

				lineEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
				lineEntity.Name = node.SelectSingleNode("Name").InnerText;
				lineEntity.FirstEnd = long.Parse(node.SelectSingleNode("FirstEnd").InnerText);
				lineEntity.SecondEnd = long.Parse(node.SelectSingleNode("SecondEnd").InnerText);
				lineEntity.IsUnderground = node.SelectSingleNode("IsUnderground").InnerText.Equals("true") ? true : false;
				lineEntity.ConductorMaterial = node.SelectSingleNode("ConductorMaterial").InnerText;
				lineEntity.LineType = node.SelectSingleNode("LineType").InnerText;
				lineEntity.ThermalConstantHeat = long.Parse(node.SelectSingleNode("ThermalConstantHeat").InnerText);
				lineEntity.R = float.Parse(node.SelectSingleNode("R").InnerText);
				Lines.Add(lineEntity);
			}
		}

		private void LoadPowerEntities()
		{
			LoadSubstationEntities();
			LoadSwitchEntities();
			LoadNodeEntities();
		}

		private void LoadSubstationEntities()
		{
			XmlNodeList nodeList;
			nodeList = doc.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity");

			foreach (XmlNode node in nodeList)
			{
				SubstationEntity substationEntity = new SubstationEntity();

				substationEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
				substationEntity.Name = node.SelectSingleNode("Name").InnerText;
				substationEntity.X = double.Parse(node.SelectSingleNode("X").InnerText);
				substationEntity.Y = double.Parse(node.SelectSingleNode("Y").InnerText);

				Entities.Add(substationEntity);
			}
		}

		private void LoadSwitchEntities()
		{
			XmlNodeList nodeList;
			nodeList = doc.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");

			foreach (XmlNode node in nodeList)
			{
				SwitchEntity switchEntity = new SwitchEntity();

				switchEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
				switchEntity.Name = node.SelectSingleNode("Name").InnerText;
				switchEntity.X = double.Parse(node.SelectSingleNode("X").InnerText);
				switchEntity.Y = double.Parse(node.SelectSingleNode("Y").InnerText);
				switchEntity.Status= node.SelectSingleNode("Status").InnerText;
				Entities.Add(switchEntity);
			}
		}

		private void LoadNodeEntities()
		{
			XmlNodeList nodeList;
			nodeList = doc.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");

			foreach (XmlNode node in nodeList)
			{
				NodeEntity nodeEntity = new NodeEntity();

				nodeEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
				nodeEntity.Name = node.SelectSingleNode("Name").InnerText;
				nodeEntity.X = double.Parse(node.SelectSingleNode("X").InnerText);
				nodeEntity.Y = double.Parse(node.SelectSingleNode("Y").InnerText);

				Entities.Add(nodeEntity);
			}
		}

		public void CalculateLatLonPointsAndScaleFactorForEntities()
		{
			foreach(var entity in Entities)
			{
				converter.ConvertPointToLanLon(entity.Point);
				scaleConverter.CalculateMinMax(entity.Point);
			}
		}

		private void ScaleModel()
		{
			CalculateLatLonPointsAndScaleFactorForEntities();
			//2040- !2000!  1280- !1240!
			scaleConverter.CalculateScaleFactorAndProportion(500, 300);

			foreach (var entity in Entities)
			{
				scaleConverter.DoScalePoint(entity.Point);
				int index = Entities.IndexOf(entity);

				for (int i = 0; i < index; i++)
				{
					if (entity.Point.IsTooClose(Entities[i].Point, 1) && entity.Id != Entities[i].Id)
					{
						double xDiff = Math.Abs(entity.Point.X - Entities[i].Point.X);
						double YDiff = Math.Abs(entity.Point.Y - Entities[i].Point.Y);

						if (xDiff > YDiff)
						{
							entity.Point.X += 1 - xDiff;
							i = -1;
						}
						else
						{
							entity.Point.Y += 1 - YDiff;
							i = -1;
						}
					}
				}
				//listaX.Add(entity.Point.X);
				//listaY.Add(entity.Point.Y);
			}
		}

		private void countConnections()
        {
			foreach (var e in Entities)
            {
				int count = 0;
				foreach(var l in Lines)
                {
					if(l.FirstEnd== e.Id || l.SecondEnd == e.Id)
                    {
						count++;
                    }
                }
				e.ConnectionsCount = count;
            }
        }
	}
}
