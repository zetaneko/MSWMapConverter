using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace MSWMapConverterLib
{
    public class MapConverter
    {
        /// <summary>
        /// Returns a List of region Ids
        /// </summary>
        /// <param name="wzExtractPath"></param>
        /// <returns></returns>
        public List<int> GetListsOfRegionIds(string wzExtractPath)
        {
            var regions = new List<int>();
            string[] subDirectories = Directory.GetDirectories(Path.Combine(wzExtractPath, "Map.wz", "Map"), "*", SearchOption.TopDirectoryOnly);
            foreach (var subDir in subDirectories)
            {
                regions.Add(Int32.Parse(subDir.Split(@"\").Last().ToLower().Replace("map", "")));
            }

            return regions;
        }

        /// <summary>
        /// Returns a List of maps for a given region
        /// </summary>
        /// <param name="wzExtractPath"></param>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public List<string> GetListsOfMaps(string wzExtractPath, int regionId)
        {
            var maps = new List<string>();
            string[] subDirectories = Directory.GetDirectories(Path.Combine(wzExtractPath, "Map.wz", "Map", $"Map{regionId}"), "*", SearchOption.TopDirectoryOnly);
            foreach (var subDir in subDirectories)
            {
                maps.Add(subDir.Split(@"\").Last().Replace(".img", ""));
            }

            return maps;
        }

        /// <summary>
        /// Converts the map from the MS Client into the MSW format
        /// </summary>
        /// <param name="regionId"></param>
        /// <param name="mapId"></param>
        /// <param name="wzExtractPath"></param>
        /// <param name="mswExtractPath"></param>
        public void Convert(int regionId, string mapId, string wzExtractPath, string mswExtractPath)
        {
            // Load CSV file (this example uses LINQ to parse CSV, but you may want to use a CSV library like CsvHelper)
            var resourceTileMappings = File.ReadLines(@"ResourceTileMappings.csv")
                                           .Skip(1)
                                           .Select(line => line.Split(','))
                                           .Select(values => new { Ruid = values[0].Replace("\"", ""), Name = values[1].Replace("\"", "") })
                                           .ToList();

            // Load CSV file (this example uses LINQ to parse CSV, but you may want to use a CSV library like CsvHelper)
            var musicMappings = File.ReadLines(@"MusicMappings.csv")
                                           .Skip(1)
                                           .Select(line => line.Split(','))
                                           .Select(values => new { Ruid = values[0].Replace("\"", ""), Name = values[1].Replace("\"", "") })
                                           .ToList();

            // Load JSON template
            var templateTileObjectTxt = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"TemplateTileObject.json"));

            // Load JSON template
            var templateFootholdObjectTxt = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"TemplateFootholdObject.json"));

            var templateRopeLadderObjectTxt = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"TemplateRopeLadderObject.json"));

            var templateMapObjectTxt = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"TemplateMapObject.json"));

            string mswMapFilePath = $@"{mswExtractPath}\map\map-{regionId}-{mapId}.json";

            var mapIdentifier = $"Map-{regionId}-{mapId}";

            // Load and parse XML
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load($@"{wzExtractPath}\Map.wz\Map\Map{regionId}\{mapId}.img.xml");

            var bgmRuid = "";

            foreach (XmlNode infoRoot in xmlDocument.SelectNodes("//imgdir[@name='info']"))
            {
                foreach (XmlNode section in infoRoot.ChildNodes)
                {
                    if (section.Attributes["name"].Value == "bgm")
                    {
                        bgmRuid = musicMappings.FirstOrDefault(rt => rt.Name == section.Attributes["value"].Value.ToLower().Replace("/", "_")).Ruid;
                    }
                }

                break; // Break after the first foothold node
            }

            templateMapObjectTxt = templateMapObjectTxt.Replace("|guid|", Guid.NewGuid().ToString())
                                                                   .Replace("|uniqueid|", mapIdentifier)
                                                                   .Replace("|bgmruid|", bgmRuid)
                                                                   .Replace("|name|", mapIdentifier);

            File.WriteAllText(mswMapFilePath, templateMapObjectTxt);

            XmlDocument connectInfo = new XmlDocument();
            connectInfo.Load($@"{wzExtractPath}\Map.wz\Obj\connect.img.xml");

            // Load MSW Map file
            var mswMapFile = File.ReadAllText(mswMapFilePath);

            JToken mswMapJson = JsonConvert.DeserializeObject<JToken>(mswMapFile);
            JArray mswMapContent = (JArray)mswMapJson["contents"];

            mswMapJson["contents"] = new JArray(mswMapContent.Where(contentItem => !((string)contentItem["entryPath"]).Contains("/MigratedMap/") && !((string)contentItem["entryPath"]).Contains("/MigratedFootholds/") && !((string)contentItem["entryPath"]).Contains("/MigratedRopeLadders/")));

            // Ensure that on each map, all of the entry IDs are unique to avoid clashes when replicating from the template map
            foreach (var item in mswMapJson["contents"])
            {
                item["entryId"] = Guid.NewGuid().ToString();
            }

            foreach (XmlNode layer in xmlDocument.ChildNodes[1].ChildNodes)
            {
                var layerName = layer.Attributes["name"].Value;

                if (layerName == "0" || layerName == "1" || layerName == "2" || layerName == "3" || layerName == "4" || layerName == "5" || layerName == "6" || layerName == "7" || layerName == "8" || layerName == "9" || layerName == "10")
                {
                    var objectsToSort = new List<ObjectToSort>();

                    foreach (XmlNode obj in layer.SelectNodes("imgdir[3]/imgdir"))
                    {
                        var objType = obj.ChildNodes[0].Attributes["value"].Value;
                        if (obj.HasChildNodes && objType != "connect")
                        {

                            double x = 0, y = 0, dO = 0;
                            string l0 = "", l1 = "", l2 = "";

                            foreach (XmlNode node in obj.ChildNodes)
                            {
                                switch (node.Attributes["name"].Value)
                                {
                                    case "x":
                                        x = double.Parse(node.Attributes["value"].Value);
                                        break;
                                    case "y":
                                        y = double.Parse(node.Attributes["value"].Value);
                                        break;
                                    case "l0":
                                        l0 = node.Attributes["value"].Value;
                                        break;
                                    case "l1":
                                        l1 = node.Attributes["value"].Value;
                                        break;
                                    case "l2":
                                        l2 = node.Attributes["value"].Value;
                                        break;
                                    case "z":
                                        dO = double.Parse(node.Attributes["value"].Value);
                                        break;
                                }
                            }

                            XmlDocument objInfo = new XmlDocument();
                            objInfo.Load($@"{wzExtractPath}\Map.wz\Obj\{objType}.img.xml");

                            foreach (XmlNode l0Type in objInfo.SelectNodes("//imgdir[@name='" + l0 + "']"))
                            {
                                if (l0Type.Attributes["name"].Value == l0)
                                {
                                    foreach (XmlNode l1Type in l0Type.ChildNodes)
                                    {
                                        if (l1Type.Attributes["name"].Value == l1)
                                        {
                                            foreach (XmlNode l2Type in l1Type.ChildNodes)
                                            {
                                                if (l2Type.Attributes["name"].Value == l2)
                                                {
                                                    foreach (XmlNode canvas in l2Type.SelectNodes("canvas"))
                                                    {
                                                        double canvasX = double.Parse(canvas["vector"].Attributes["x"].Value);
                                                        double canvasY = double.Parse(canvas["vector"].Attributes["y"].Value);

                                                        x -= canvasX;
                                                        y -= canvasY;

                                                        y += double.Parse(canvas.Attributes["height"].Value) / 2;
                                                        x += double.Parse(canvas.Attributes["width"].Value) / 2;

                                                        //double vectorX = double.Parse(vector.Attributes["x"].Value);
                                                        //x -= vectorX;

                                                        break;
                                                    }
                                                    break;
                                                }
                                            }
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }

                            string recName = $"Obj_{objType}.img_{l0}_{l1}_{l2}_0";
                            var resourceTile = resourceTileMappings.FirstOrDefault(rt => rt.Name == recName);

                            if (resourceTile != null)
                            {
                                var ruid = resourceTile.Ruid;
                                var mswX = x / 100;
                                var mswY = -y / 100;
                                var guid = Guid.NewGuid().ToString();
                                var thisTileJson = templateTileObjectTxt.Replace("|guid|", guid)
                                                                       .Replace("|uniqueName|", $"obj{layerName}-{obj.Attributes["name"].Value}")
                                                                       .Replace("|ruid|", ruid)
                                                                       .Replace("|mswX|", mswX.ToString())
                                                                       .Replace("|mswY|", mswY.ToString())
                                                                       .Replace("|mapIdentifier|", mapIdentifier)
                                                                       .Replace("|layer|", layerName)
                                                                       .Replace("|do|", dO.ToString());

                                var thisTileObj = JsonConvert.DeserializeObject<JToken>(thisTileJson);

                                objectsToSort.Add(new ObjectToSort { DisplayOrder = dO, Object = thisTileObj });
                            }
                        }
                    }

                    foreach (var objToSort in objectsToSort.OrderBy(o => o.DisplayOrder))
                    {
                        ((JArray)mswMapJson["contents"]).Add(objToSort.Object);
                    }

                    // Assuming first imgdir child directly under the layer node:
                    var info = layer.FirstChild;

                    var tileSetInfo = new XmlDocument();
                    string tileType = null;

                    foreach (XmlNode child in info.ChildNodes)
                    {
                        if (child.Attributes["name"].Value == "tS")
                        {
                            tileType = child.Attributes["value"].Value;
                            tileSetInfo.Load($@"{wzExtractPath}\Map.wz\Tile\{tileType}.img.xml");
                            break;
                        }
                    }

                    var sortedTiles = new List<ObjectToSort>();

                    foreach (XmlNode tile in layer.SelectNodes("imgdir[2]/imgdir"))
                    {
                        double x = 0, y = 0, no = 0, zM = 0;
                        string u = "";

                        int dO = int.Parse(tile.Attributes["name"].Value);

                        foreach (XmlNode node in tile.ChildNodes)
                        {
                            switch (node.Attributes["name"].Value)
                            {
                                case "x":
                                    x = double.Parse(node.Attributes["value"].Value);
                                    break;
                                case "y":
                                    y = double.Parse(node.Attributes["value"].Value);
                                    break;
                                case "u":
                                    u = node.Attributes["value"].Value;
                                    break;
                                case "no":
                                    no = double.Parse(node.Attributes["value"].Value);
                                    break;
                                case "zM":
                                    zM = double.Parse(node.Attributes["value"].Value);
                                    break;
                            }
                        }

                        foreach (XmlNode uType in tileSetInfo.SelectNodes("//imgdir[@name='" + u + "']"))
                        {
                            foreach (XmlNode canvas in uType.SelectNodes("canvas"))
                            {
                                if (no.ToString() == canvas.Attributes["name"].Value)
                                {
                                    double canvasX = double.Parse(canvas["vector"].Attributes["x"].Value);
                                    double canvasY = double.Parse(canvas["vector"].Attributes["y"].Value);

                                    x -= canvasX;
                                    y -= canvasY;

                                    y += double.Parse(canvas.Attributes["height"].Value) / 2;
                                    x += double.Parse(canvas.Attributes["width"].Value) / 2;

                                    break;
                                }
                            }
                        }

                        string recName = $"Tile_{tileType}.img_{u}_{no}";
                        var resourceTile = resourceTileMappings.FirstOrDefault(rt => rt.Name == recName);

                        if (resourceTile != null)
                        {
                            var ruid = resourceTile.Ruid;
                            var mswX = x / 100;
                            var mswY = -y / 100;
                            var guid = Guid.NewGuid().ToString();
                            var thisTileJson = templateTileObjectTxt.Replace("|guid|", guid)
                                                                   .Replace("|uniqueName|", $"Tile{layerName}-{tile.Attributes["name"].Value}")
                                                                   .Replace("|ruid|", ruid)
                                                                   .Replace("|mswX|", mswX.ToString())
                                                                   .Replace("|mswY|", mswY.ToString())
                                                                   .Replace("|mapIdentifier|", mapIdentifier)
                                                                   .Replace("|layer|", layerName)
                                                                   .Replace("|do|", dO.ToString());

                            JToken thisTileObj = JsonConvert.DeserializeObject<JToken>(thisTileJson);

                            sortedTiles.Add(new ObjectToSort { DisplayOrder = zM, Object = thisTileObj });
                        }
                    }

                    foreach (var item in sortedTiles.OrderBy(i => i.DisplayOrder))
                    {
                        ((JArray)mswMapJson["contents"]).Add(item.Object);
                    }

                    foreach (XmlNode connect in layer.SelectNodes("imgdir[3]/imgdir"))
                    {
                        if (connect.HasChildNodes && connect.ChildNodes[0].Attributes["value"].Value == "connect")
                        {

                            double x = 0, y = 0, l1 = 0, l2 = 0, dO = 0;
                            string l0 = "";

                            dO = double.Parse(connect.Attributes["name"].Value);

                            foreach (XmlNode node in connect.ChildNodes)
                            {
                                switch (node.Attributes["name"].Value)
                                {
                                    case "x":
                                        x = double.Parse(node.Attributes["value"].Value);
                                        break;
                                    case "y":
                                        y = double.Parse(node.Attributes["value"].Value);
                                        break;
                                    case "l0":
                                        l0 = node.Attributes["value"].Value;
                                        break;
                                    case "l1":
                                        l1 = double.Parse(node.Attributes["value"].Value);
                                        break;
                                    case "l2":
                                        l2 = double.Parse(node.Attributes["value"].Value);
                                        break;
                                }
                            }


                            foreach (XmlNode l0Type in connectInfo.SelectNodes("//imgdir[@name='" + l0 + "']"))
                            {
                                if (l0Type.Attributes["name"].Value == l0)
                                {
                                    foreach (XmlNode l1Type in l0Type.ChildNodes)
                                    {
                                        if (l1Type.Attributes["name"].Value == l1.ToString())
                                        {
                                            foreach (XmlNode l2Type in l1Type.ChildNodes)
                                            {
                                                if (l2Type.Attributes["name"].Value == l2.ToString())
                                                {
                                                    foreach (XmlNode canvas in l2Type.SelectNodes("canvas"))
                                                    {
                                                        //double canvasX = double.Parse(canvas["vector"].Attributes["x"].Value);
                                                        //double canvasY = double.Parse(canvas["vector"].Attributes["y"].Value);

                                                        //x -= canvasX;
                                                        //y -= canvasY;

                                                        y += double.Parse(canvas.Attributes["height"].Value) / 2;
                                                        x += double.Parse(canvas.Attributes["width"].Value) / 2;

                                                        if (canvas["extended"] != null)
                                                        {
                                                            XmlNode vector = canvas["extended"].FirstChild;
                                                            double vectorY = double.Parse(vector.Attributes["y"].Value);
                                                            y += vectorY;
                                                        }

                                                        //double vectorX = double.Parse(vector.Attributes["x"].Value);
                                                        //x -= vectorX;

                                                        break;
                                                    }
                                                    break;
                                                }
                                            }
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }

                            string recName = $"{l0}_{l1}_{l2}_0";
                            var resourceTile = resourceTileMappings.FirstOrDefault(rt => rt.Name == recName);

                            if (resourceTile != null)
                            {
                                var ruid = resourceTile.Ruid;
                                var mswX = x / 100;
                                var mswY = -y / 100;
                                var guid = Guid.NewGuid().ToString();
                                var thisTileJson = templateTileObjectTxt.Replace("|guid|", guid)
                                                                       .Replace("|uniqueName|", $"Connect{layerName}-{connect.Attributes["name"].Value}")
                                                                       .Replace("|ruid|", ruid)
                                                                       .Replace("|mswX|", mswX.ToString())
                                                                       .Replace("|mswY|", mswY.ToString())
                                                                       .Replace("|mapIdentifier|", mapIdentifier)
                                                                       .Replace("|layer|", layerName)
                                                                       .Replace("|do|", dO.ToString());

                                var thisTileObj = JsonConvert.DeserializeObject(thisTileJson);

                                ((JArray)mswMapJson["contents"]).Add(thisTileObj);
                            }
                        }
                    }
                }
                else if (layerName == "ladderRope")
                {
                    foreach (XmlNode ladderRope in layer.ChildNodes)
                    {
                        double x = 0, y1 = 0, y2 = 0;

                        double id = double.Parse(ladderRope.Attributes["name"].Value);

                        bool isLadder = false;

                        foreach (XmlNode node in ladderRope.ChildNodes)
                        {
                            switch (node.Attributes["name"].Value)
                            {
                                case "x":
                                    x = double.Parse(node.Attributes["value"].Value) / 100;
                                    break;
                                case "y1":
                                    y1 = 0 - double.Parse(node.Attributes["value"].Value) / 100;
                                    break;
                                case "y2":
                                    y2 = 0 - double.Parse(node.Attributes["value"].Value) / 100;
                                    break;
                                case "l":
                                    if (node.Attributes["value"].Value == "1")
                                    {
                                        isLadder = true;
                                    }
                                    break;
                            }
                        }

                        // Calculate scale of rope or ladder
                        var yScale = 0 - ((y2 - y1) * 0.9);

                        var xScale = 0.12;
                        if (isLadder)
                        {
                            xScale = 0.61;
                        }

                        // Center the object
                        if (isLadder)
                        {
                            x = x - (xScale / 2) + 0.1;
                        }
                        else
                        {
                            x = x - (xScale / 2) - 0.41;
                        }

                        // Center the object
                        y1 = y1 - (yScale / 2) - 3.3;

                        var guid1 = Guid.NewGuid().ToString();
                        var guid2 = Guid.NewGuid().ToString();
                        var guid3 = Guid.NewGuid().ToString();

                        var ca = "0";
                        if (isLadder)
                        {
                            ca = "1";
                        }

                        var thisTileJson = templateRopeLadderObjectTxt.Replace("|guid1|", guid1)
                                                                .Replace("|guid2|", guid2)
                                                                .Replace("|guid3|", guid3)
                                                                .Replace("|ca|", ca)
                                                                .Replace("|uniqueName|", $"ConnectPath{layerName}-{ladderRope.Attributes["name"].Value}")
                                                                .Replace("|mswX|", x.ToString())
                                                                .Replace("|mswY|", y1.ToString())
                                                                .Replace("|mapIdentifier|", mapIdentifier)
                                                                .Replace("|mswSX|", xScale.ToString())
                        .Replace("|mswSY|", yScale.ToString());
                        JArray thisTileObj = JsonConvert.DeserializeObject<JArray>(thisTileJson);

                        foreach (var connectPath in thisTileObj)
                        {
                            ((JArray)mswMapJson["contents"]).Add(connectPath);
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Skipping {layerName}");
                }
            }



            // Process foothold section
            foreach (XmlNode footholdRoot in xmlDocument.SelectNodes("//imgdir[@name='foothold']"))
            {
                foreach (XmlNode section in footholdRoot.SelectNodes("imgdir"))
                {
                    foreach (XmlNode link in section.SelectNodes("imgdir"))
                    {
                        List<List<Dictionary<string, double>>> edgeLists = new List<List<Dictionary<string, double>>>();
                        List<XmlNode> points = new List<XmlNode>();
                        foreach (XmlNode point in link.SelectNodes("imgdir"))
                        {
                            points.Add(point);
                        }

                        var sortedPoints = points.OrderBy(p => p.ChildNodes[4].Attributes["value"].Value);

                        foreach (XmlNode point in sortedPoints)
                        {
                            List<Dictionary<string, double>> sectionEdges = new List<Dictionary<string, double>>();
                            double x1 = double.Parse(point.ChildNodes[0].Attributes["value"].Value) / 100;
                            double y1 = -double.Parse(point.ChildNodes[1].Attributes["value"].Value) / 100;
                            double x2 = double.Parse(point.ChildNodes[2].Attributes["value"].Value) / 100;
                            double y2 = -double.Parse(point.ChildNodes[3].Attributes["value"].Value) / 100;

                            sectionEdges.Add(new Dictionary<string, double> { { "x", x1 }, { "y", y1 } });
                            sectionEdges.Add(new Dictionary<string, double> { { "x", x2 }, { "y", y2 } });
                            edgeLists.Add(sectionEdges);
                        }

                        var edgeListJson = JsonConvert.SerializeObject(edgeLists, Newtonsoft.Json.Formatting.Indented);

                        var guid = Guid.NewGuid().ToString();
                        var thisFootholdJson = templateFootholdObjectTxt.Replace("|guid|", guid)
                                                               .Replace("|uniqueName|", $"Foothold{section.Attributes["name"].Value}-{link.Attributes["name"].Value}")
                                                               .Replace("|mapIdentifier|", mapIdentifier)
                                                               .Replace("|edgelists|", edgeListJson);

                        var thisFootholdObj = JsonConvert.DeserializeObject(thisFootholdJson);

                        ((JArray)mswMapJson["contents"]).Add(thisFootholdObj);
                    }
                }

                break; // Break after the first foothold node
            }

            var mswMapNewFile = JsonConvert.SerializeObject(mswMapJson, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(mswMapFilePath, mswMapNewFile);
        }
    }

    public class ObjectToSort
    {
        public double DisplayOrder { get; set; }
        public JToken Object { get; set; }
    }

    public class XMLObjectToSort
    {
        public double DisplayOrder { get; set; }
        public XmlNode Object { get; set; }
    }
}
