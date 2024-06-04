using LSLib.LS;
using LSLib.LS.Enums;
using LZ4;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Formats.Asn1;
using System.IO;
using System.Media;
using System.Reflection.Emit;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using static bg3dialogreader.Form1;
using static bg3dialogreader.Json;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace bg3dialogreader
{
    public partial class Form1 : Form
    {
        private Resource _resource;
        SqliteConnection connection = new("Data Source=bg3.db");
        SqliteCommand sqliteCommand = new();
        List<string> fdialogs = [];
        List<string> adialogs = [];
        List<Task> converttasks = [];
        Dictionary<string, string> testdictfound = new()
        {
            {"e0d1ff71-04a8-4340-ae64-9684d846eb83", "Player"}
        };
        Dictionary<string, string> testdictnotfound = [];
        string bg3path = "";
        List<string> exdialogs = [];
        Dictionary<string, string> adgg = [];
        Dictionary<string, byte[]> testloadall = [];
        int dfc = 0;
        int dfc2 = 0;
        public Form1()
        {
            InitializeComponent();
            if (!File.Exists("vgmstream-cli.exe"))
            {
                linkLabelNotFoundVGM.Visible = true;
                //exportToolStripMenuItem.Enabled = false;
            }
            if (File.Exists("bg3.db"))
            {
                DateTime fd = File.GetCreationTime("bg3.db");
                labelDBInfo.Text = "(Database created: " + fd.ToString() + ")";
                labelDBInfo.Visible = true;
            }
            else
            {
                labelDBInfo.Text = "(Database not exists)";
                labelDBInfo.Visible = true;
            }
        }

        private void parsetranslation()
        {
            string locPak = "";
            string locLang = "";
            this.Invoke(new MethodInvoker(delegate () { locLang = comboBoxLanguageSelect.Text; }));

            if (locLang == "English")
                locPak = bg3path + "\\Localization\\English.pak";
            else
                locPak = bg3path + "\\Localization\\" + locLang + "\\" + locLang + ".pak"; //"\\Localization\\English.pak";

            var pakReader = new PackageReader();
            using var package = pakReader.Read(locPak);
            List<PackagedFileInfo> files = package.Files;
            foreach (var file in files)
            {
                if (file.Name.Contains("/" + locLang + "/" + locLang.ToLower() + ".loca") || file.Name.Contains("/" + locLang + "/" + locLang.ToLower() + ".xml"))
                {
                    using var fileStream = file.CreateContentReader();
                    var fileExt = LocaUtils.ExtensionToFileFormat(file.Name);
                    var locReader = LocaUtils.Load(fileStream, fileExt);

                    sqliteCommand.CommandText = "INSERT or REPLACE INTO tagsflags VALUES (@handle,@text,null)";
                    foreach (var entry in locReader.Entries)
                    {
                        sqliteCommand.Parameters.Add(new SqliteParameter("@handle", entry.Key));
                        sqliteCommand.Parameters.Add(new SqliteParameter("@text", entry.Text));
                        sqliteCommand.ExecuteNonQuery();
                        sqliteCommand.Parameters.Clear();
                    }
                }
            }
        }

        private static void rConvert(Resource resource, MemoryStream outputPath, ResourceFormat format, ResourceConversionParameters conversionParams)
        {
            var writer = new LSXWriter(outputPath);
            writer.Version = conversionParams.LSX;
            writer.PrettyPrint = conversionParams.PrettyPrint;
            writer.Write(resource);
        }

        private void dialogs(PackagedFileInfo file)
        {
            using var fileStream = file.CreateContentReader();
            using var memoryStream = new MemoryStream();
            fileStream.CopyTo(memoryStream);

            convertd2(memoryStream.ToArray(), file.Name);

            dfc++;
            this.Invoke(new MethodInvoker(delegate () { labelExportedFilesCounter.Text = dfc.ToString(); }));

            if (checkBoxExportLSJ.Checked)
            {
                var path = file.Name.Split(new string[] { "/Story/Dialogs/" }, StringSplitOptions.None);

                Directory.CreateDirectory(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory + "\\Dialogs_lsj\\" + path[1]));
                File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + "\\Dialogs_lsj\\" + path[1], memoryStream.ToArray());
            }
        }
        
        private void finaliz()
        {
            foreach (var entry in testdictnotfound)
            {
                if (testdictfound.ContainsKey(entry.Value))
                {
                    testdictfound[entry.Key] = testdictfound[entry.Value];
                }
                else
                {
                    if (testdictnotfound.ContainsKey(entry.Value))
                    {
                        if (testdictfound.ContainsKey(testdictnotfound[entry.Value]))
                        {
                            testdictfound[entry.Key] = testdictfound[testdictnotfound[entry.Value]];
                        }
                        else
                        {
                            if (testdictnotfound.ContainsKey(testdictnotfound[entry.Value]))
                            {
                                if (testdictfound.ContainsKey(testdictnotfound[testdictnotfound[entry.Value]]))
                                {
                                    testdictfound[entry.Key] = testdictfound[testdictnotfound[testdictnotfound[entry.Value]]];
                                }
                            }
                        }
                    }
                }
            }

            foreach (var entry in testdictfound)
            {
                sqliteCommand.CommandText = "INSERT or REPLACE INTO tagsflags VALUES (@uuid,@text,@desc)";

                sqliteCommand.Parameters.Add(new SqliteParameter("@uuid", entry.Key));
                sqliteCommand.Parameters.Add(new SqliteParameter("@text", entry.Value));
                sqliteCommand.Parameters.Add(new SqliteParameter("@desc", DBNull.Value));

                sqliteCommand.ExecuteNonQuery();
                sqliteCommand.Parameters.Clear();
            }
        }

        private void namesmerged(PackagedFileInfo file)
        {

            using var fileStream = file.CreateContentReader();
            using var asd = new MemoryStream();
            if (file.Name.Contains(".lsf"))
            {
                using var reader = new LSFReader(fileStream);

                _resource = reader.Read();
                ResourceFormat format = ResourceFormat.LSX;
                var conversionParams = ResourceConversionParameters.FromGameVersion(Game.BaldursGate3);
                rConvert(_resource, asd, format, conversionParams);
                //string result = Encoding.UTF8.GetString(asd.ToArray());//.Replace("\uFEFF", "");
            }
            else if (file.Name.Contains(".lsx"))
            {
                fileStream.CopyTo(asd);
            }

            asd.Position = 0;
            XmlReader xmreader = XmlReader.Create(asd);
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(xmreader);

            XmlNode node = xDoc.DocumentElement.SelectSingleNode("/save/region/node");

            if (node.LastChild != null)
            {
                XmlNode node1 = xDoc.DocumentElement.SelectSingleNode("/save/region/node/children");
                foreach (XmlNode item in node1.ChildNodes)
                {
                    string MapKey = "";
                    string DisplayName = "";
                    string TemplateName = "";
                    string ParentTemplateId = "";
                    string dbname = "";
                    foreach (XmlNode item1 in item.ChildNodes)
                    {
                        if (item1.Name == "attribute" && item1.Attributes.GetNamedItem("id").Value == "MapKey")
                            MapKey = item1.Attributes.GetNamedItem("value").Value;
                        if (item1.Name == "attribute" && item1.Attributes.GetNamedItem("id").Value == "DisplayName")
                            DisplayName = item1.Attributes.GetNamedItem("handle").Value;
                        if (item1.Name == "attribute" && item1.Attributes.GetNamedItem("id").Value == "TemplateName")
                            TemplateName = item1.Attributes.GetNamedItem("value").Value;
                        if (item1.Name == "attribute" && item1.Attributes.GetNamedItem("id").Value == "ParentTemplateId")
                            ParentTemplateId = item1.Attributes.GetNamedItem("value").Value;
                    }

                    if (DisplayName.StartsWith('h'))
                    {
                        sqliteCommand.CommandText = "SELECT * FROM tagsflags WHERE uuid=@uuid";
                        sqliteCommand.Parameters.Add(new SqliteParameter("@uuid", DisplayName));

                        using (SqliteDataReader readers = sqliteCommand.ExecuteReader())
                        {
                            if (readers.HasRows)
                            {
                                while (readers.Read())
                                {
                                    dbname = readers.GetValue(1).ToString();
                                    testdictfound[MapKey] = dbname;
                                }
                            }
                        }
                        sqliteCommand.Parameters.Clear();
                    }
                    else if (DisplayName == "")
                    {
                        if (TemplateName != "")
                            testdictnotfound[MapKey] = TemplateName;
                        else
                            testdictnotfound[MapKey] = ParentTemplateId;
                    }
                }
            }
        }
        private void namesorigins(PackagedFileInfo file)
        {
            using var fileStream = file.CreateContentReader();
            using XmlReader xmreader = XmlReader.Create(fileStream);
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(xmreader);

            XmlNode node = xDoc.DocumentElement.SelectSingleNode("/save/region/node/children");

            foreach (XmlNode item in node.ChildNodes)
            {
                string uuid = "";
                string GlobalTemplate = "";
                foreach (XmlNode item1 in item.ChildNodes)
                {
                    if (item1.Name == "attribute" && item1.Attributes.GetNamedItem("id").Value == "UUID")
                        uuid = item1.Attributes.GetNamedItem("value").Value;
                    if (item1.Name == "attribute" && item1.Attributes.GetNamedItem("id").Value == "GlobalTemplate")
                        GlobalTemplate = item1.Attributes.GetNamedItem("value").Value;
                }
                testdictnotfound[uuid] = GlobalTemplate;
            }
        }
        private void namesspeakergroup(PackagedFileInfo file)
        {
            using var fileStream = file.CreateContentReader();
            using var reader = new LSFReader(fileStream);

            var asd = new MemoryStream();
            _resource = reader.Read();
            ResourceFormat format = ResourceFormat.LSX;
            var conversionParams = ResourceConversionParameters.FromGameVersion(Game.BaldursGate3);
            rConvert(_resource, asd, format, conversionParams);
            //string result = Encoding.UTF8.GetString(asd.ToArray());//.Replace("\uFEFF", "");


            asd.Position = 0;
            XmlReader xmreader = XmlReader.Create(asd);
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(xmreader);


            XmlNode node = xDoc.DocumentElement.SelectSingleNode("/save/region/node/children");

            foreach (XmlNode item in node.ChildNodes)
            {
                string uuid = "";
                string stringname = "";
                string desc = "";
                foreach (XmlNode item1 in item.ChildNodes)
                {
                    if (item1.Name == "attribute" && item1.Attributes.GetNamedItem("id").Value == "UUID")
                        uuid = item1.Attributes.GetNamedItem("value").Value;
                    if (item1.Name == "attribute" && item1.Attributes.GetNamedItem("id").Value == "Name")
                        stringname = item1.Attributes.GetNamedItem("value").Value;
                    if (item1.Name == "attribute" && item1.Attributes.GetNamedItem("id").Value == "Description")
                        desc = item1.Attributes.GetNamedItem("value").Value;
                }
                if (stringname == "GROUP_Players")
                    continue;


                sqliteCommand.CommandText = "INSERT or REPLACE INTO tagsflags VALUES (@uuid,@text,@desc)";

                sqliteCommand.Parameters.Add(new SqliteParameter("@uuid", uuid));
                sqliteCommand.Parameters.Add(new SqliteParameter("@text", stringname));
                sqliteCommand.Parameters.Add(new SqliteParameter("@desc", desc));

                sqliteCommand.ExecuteNonQuery();
                sqliteCommand.Parameters.Clear();
            }
        }
        private void Reactions(PackagedFileInfo file)
        {
            using var fileStream = file.CreateContentReader();

            var npc = new Dictionary<string, string>()
            {
                {"2bb39cf2-4649-4238-8d0c-44f62b5a3dfd", "Shadowheart"},
                {"35c3caad-5543-4593-be75-e7deba30f062", "Gale"},
                {"3780c689-d903-41c2-bf64-1e6ec6a8e1e5", "Astarion"},
                {"efc9d114-0296-4a30-b701-365fc07d44fb", "Wyll"},
                {"fb3bc4c3-49eb-4944-b714-d0cb357bb635", "Lae\'zel"},
                {"b8b4a974-b045-45f6-9516-b457b8773abd", "Karlach"},
                {"c1f137c7-a17c-47b0-826a-12e44a8ec45c", "Jaheira"},
                {"eae09670-869d-4b70-b605-33af4ee80b34", "Minthara"},
                {"e1b629bc-7340-4fe6-81a4-834a838ff5c5", "Minsc"},
                {"a36281c5-adcd-4d6e-8e5a-b5650b8f17eb", "Halsin"},
                {"38357c93-b437-4f03-88d0-a67bd4c0e3e9", "Alfira"},
                {"5af0f42c-9b32-4c3c-b108-46c44196081b", "The Dark Urge"},
                {"a4b56492-d5ac-4a84-8e45-5437cd9da7f3", "Custom"}
            };

            using XmlReader xmreader = XmlReader.Create(fileStream);
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(xmreader);


            string react = "[";

            XmlNode node = xDoc.DocumentElement.SelectSingleNode("/save/region/node/children/node");

            if (node.LastChild.Name == "children")
            {
                XmlNode node1 = xDoc.DocumentElement.SelectSingleNode("/save/region/node/children/node/children/node/children");

                foreach (XmlNode item in node1.ChildNodes)
                {
                    string uuid = "";
                    string stringname = "";
                    foreach (XmlNode item1 in item.ChildNodes)
                    {
                        if (item1.Name == "attribute" && item1.Attributes.GetNamedItem("id").Value == "id")
                            uuid = item1.Attributes.GetNamedItem("value").Value;
                        if (item1.Name == "attribute" && item1.Attributes.GetNamedItem("id").Value == "value")
                            stringname = item1.Attributes.GetNamedItem("value").Value;

                    }

                    if (stringname != "0")
                        react += "'" + npc[uuid] + " " + stringname + "', ";
                }
                sqliteCommand.CommandText = "INSERT or REPLACE INTO tagsflags VALUES (@uuid,@text,@desc) ON CONFLICT(uuid) DO UPDATE SET uuid=excluded.uuid";
                sqliteCommand.Parameters.Add(new SqliteParameter("@uuid", Path.GetFileNameWithoutExtension(file.Name)));

                if (react == "[")
                    sqliteCommand.Parameters.Add(new SqliteParameter("@text", DBNull.Value));
                else
                {
                    react = react.Remove(react.Length - 2);
                    react += "]";
                    sqliteCommand.Parameters.Add(new SqliteParameter("@text", react));
                }

                sqliteCommand.Parameters.Add(new SqliteParameter("@desc", DBNull.Value));
                sqliteCommand.ExecuteNonQuery();
                sqliteCommand.Parameters.Clear();
            }
        }
        private void difficulties(PackagedFileInfo file)
        {
            using var fileStream = file.CreateContentReader();
            using XmlReader xmreader = XmlReader.Create(fileStream);
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(xmreader);

            XmlNode node = xDoc.DocumentElement.SelectSingleNode("/save/region/node/children");

            foreach (XmlNode item in node.ChildNodes)
            {
                string uuid = "";
                string stringname = "";
                string diff = "";
                foreach (XmlNode item1 in item.ChildNodes)
                {
                    if (item1.Name == "attribute" && item1.Attributes.GetNamedItem("id").Value == "UUID")
                        uuid = item1.Attributes.GetNamedItem("value").Value;
                    if (item1.Name == "attribute" && item1.Attributes.GetNamedItem("id").Value == "Name")
                        stringname = item1.Attributes.GetNamedItem("value").Value;
                    if (item1.Name == "attribute" && item1.Attributes.GetNamedItem("id").Value == "Difficulties")
                        diff = item1.Attributes.GetNamedItem("value").Value;
                }

                sqliteCommand.CommandText = "INSERT or REPLACE INTO tagsflags VALUES (@uuid,@text,@desc)";
                sqliteCommand.Parameters.Add(new SqliteParameter("@uuid", uuid));
                sqliteCommand.Parameters.Add(new SqliteParameter("@text", diff));
                sqliteCommand.Parameters.Add(new SqliteParameter("@desc", stringname));

                sqliteCommand.ExecuteNonQuery();
                sqliteCommand.Parameters.Clear();

            }
        }
        private void questflags(PackagedFileInfo file)
        {
            using var fileStream = file.CreateContentReader();
            using XmlReader xmreader = XmlReader.Create(fileStream);
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(xmreader);

            XmlNode node = xDoc.DocumentElement.SelectSingleNode("/save/region/node/children");

            foreach (XmlNode item in node.ChildNodes)
            {
                if (item.Attributes.GetNamedItem("id").Value == "Quest")
                {
                    foreach (XmlNode item1 in item.LastChild)
                    {
                        if (item1.Attributes.GetNamedItem("id").Value == "QuestStep")
                        {
                            string uuid = "";
                            string stringname = "";
                            string desc = "";
                            string desc2 = "";
                            foreach (XmlNode item2 in item1.ChildNodes)
                            {
                                if (item2.Name == "attribute" && item2.Attributes.GetNamedItem("id").Value == "DialogFlagGUID")
                                    uuid = item2.Attributes.GetNamedItem("value").Value;
                                if (item2.Name == "attribute" && item2.Attributes.GetNamedItem("id").Value == "ID")
                                    stringname = item2.Attributes.GetNamedItem("value").Value;
                                if (item2.Name == "attribute" && item2.Attributes.GetNamedItem("id").Value == "Description")
                                    desc = item2.Attributes.GetNamedItem("handle").Value;
                            }


                            sqliteCommand.CommandText = "SELECT * FROM tagsflags WHERE uuid=@uuid";
                            sqliteCommand.Parameters.Add(new SqliteParameter("@uuid", desc));

                            using (SqliteDataReader reader = sqliteCommand.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        desc2 = reader.GetValue(1).ToString();
                                    }
                                }
                            }
                            sqliteCommand.Parameters.Clear();


                            sqliteCommand.CommandText = "INSERT or REPLACE INTO tagsflags VALUES (@uuid,@text,@desc)";
                            sqliteCommand.Parameters.Add(new SqliteParameter("@uuid", uuid));
                            sqliteCommand.Parameters.Add(new SqliteParameter("@text", stringname));
                            sqliteCommand.Parameters.Add(new SqliteParameter("@desc", desc2));
                            sqliteCommand.ExecuteNonQuery();
                            sqliteCommand.Parameters.Clear();
                        }
                    }
                }
            }
        }
        private void tagsflags(PackagedFileInfo file)
        {
            using var fileStream = file.CreateContentReader();
            using var asd = new MemoryStream();
            if (file.Name.Contains(".lsf"))
            {
                using var reader = new LSFReader(fileStream);
                _resource = reader.Read();
                ResourceFormat format = ResourceFormat.LSX;
                var conversionParams = ResourceConversionParameters.FromGameVersion(Game.BaldursGate3);
                rConvert(_resource, asd, format, conversionParams);
            }
            else if (file.Name.Contains(".lsx"))
            {
                fileStream.CopyTo(asd);
            }
           
            //string result = Encoding.UTF8.GetString(asd.ToArray());//.Replace("\uFEFF", "");

            asd.Position = 0;
            XmlReader xmreader = XmlReader.Create(asd);
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(xmreader);


            string stringname = "";
            string desc = "";

            XmlNode node = xDoc.DocumentElement.SelectSingleNode("/save/region/node");

            //node.ChildNodes[0].Attributes.GetNamedItem("value");


            foreach (XmlNode item in node.ChildNodes)
            {

                if (item.Name == "attribute" && item.Attributes.GetNamedItem("id").Value == "Name")
                    stringname = item.Attributes.GetNamedItem("value").Value;
                if (item.Name == "attribute" && item.Attributes.GetNamedItem("id").Value == "Description")
                    desc = item.Attributes.GetNamedItem("value").Value;
            }

            sqliteCommand.CommandText = "INSERT or REPLACE INTO tagsflags VALUES (@handle,@text,@desc) ON CONFLICT(uuid) DO UPDATE SET uuid=excluded.uuid";
            sqliteCommand.Parameters.Add(new SqliteParameter("@handle", Path.GetFileNameWithoutExtension(file.Name)));
            if (stringname != "")
                sqliteCommand.Parameters.Add(new SqliteParameter("@text", stringname));
            else
                sqliteCommand.Parameters.Add(new SqliteParameter("@text", DBNull.Value));

            if (desc != "")
                sqliteCommand.Parameters.Add(new SqliteParameter("@desc", desc));
            else
                sqliteCommand.Parameters.Add(new SqliteParameter("@desc", DBNull.Value));
            sqliteCommand.ExecuteNonQuery();
            sqliteCommand.Parameters.Clear();
        }

        private void readfiletable(string pakFile)
        {
            string pakFilePath = bg3path + pakFile;
            var pakReader = new PackageReader();
            using var package = pakReader.Read(pakFilePath);
            List<PackagedFileInfo> files = package.Files;
            foreach (var file in files)
            {
                if ((file.Name.Contains("/Tags/") || file.Name.Contains("/Flags/")) && file.Name.Contains(".ls"))
                    tagsflags(file);
                if (file.Name.Contains("/Story/Journal/quest_prototypes.lsx"))
                    questflags(file);
                if (file.Name.Contains("/ApprovalRatings/Reactions/"))
                    Reactions(file);
                if (file.Name.Contains("/DifficultyClasses/DifficultyClasses.lsx"))
                    difficulties(file);
                if ((file.Name.Contains("/Items/_merged.lsf") || file.Name.Contains("/Characters/_merged.lsf") || file.Name.Contains("/RootTemplates/")) && !file.Name.Contains("/Content/"))
                    namesmerged(file);
                if (file.Name.Contains("/Origins/Origins.lsx"))
                    namesorigins(file);
                if (file.Name.Contains("/Voice/SpeakerGroups.lsf"))
                    namesspeakergroup(file);
                if (file.Name.Contains("/Localization/English/Soundbanks/") && file.Name.Contains(".lsf"))
                    aud(file);
            }

        }

        private void aud(PackagedFileInfo file)
        {
            using var fileStream = file.CreateContentReader();
            using var reader = new LSFReader(fileStream);

            var asd = new MemoryStream();
            _resource = reader.Read();
            ResourceFormat format = ResourceFormat.LSX;
            var conversionParams = ResourceConversionParameters.FromGameVersion(Game.BaldursGate3);
            rConvert(_resource, asd, format, conversionParams);
            //string result = Encoding.UTF8.GetString(asd.ToArray());//.Replace("\uFEFF", "");

            asd.Position = 0;
            XmlReader xmreader = XmlReader.Create(asd);
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(xmreader);

            XmlNode node = xDoc.DocumentElement.SelectSingleNode("/save/region/node/children/node/children/node/children");

            foreach (XmlNode item in node.ChildNodes)
            {
                if (item.Attributes.GetNamedItem("id").Value == "VoiceTextMetaData")
                {
                    string huuid = "";
                    string hsource = "";

                    if (item.FirstChild.Attributes.GetNamedItem("id").Value == "MapKey")
                    {
                        huuid = item.FirstChild.Attributes.GetNamedItem("value").Value;
                    }
                    if (item.LastChild.LastChild.LastChild.Attributes.GetNamedItem("id").Value == "Source")
                    {
                        hsource = item.LastChild.LastChild.LastChild.Attributes.GetNamedItem("value").Value;
                    }

                    sqliteCommand.CommandText = "UPDATE tagsflags SET description=@desc WHERE uuid=@uuid";
                    sqliteCommand.Parameters.Add(new SqliteParameter("@uuid", huuid));
                    sqliteCommand.Parameters.Add(new SqliteParameter("@desc", hsource));
                    sqliteCommand.ExecuteNonQuery();
                    sqliteCommand.Parameters.Clear();
                }
            }
        }

        private void readfiletable2(string pakFile)
        {
            string pakFilePath = bg3path + pakFile;
            var pakReader = new PackageReader();
            using var package = pakReader.Read(pakFilePath);
            List<PackagedFileInfo> files = package.Files;
            foreach (var file in files)
            {
                if (file.Name.Contains("/Story/Dialogs/") && file.Name.Contains(".lsj"))
                    if (!exdialogs.Contains(file.Name))
                    {
                        dialogs(file);
                        exdialogs.Add(file.Name);
                    }
            }
        }

        public static TreeNode MakeTreeFromPaths(List<string> paths, string rootNodeName = "Dialogs", char separator = '/')
        {
            var rootNode = new TreeNode(rootNodeName);
            foreach (var path in paths.Where(x => !string.IsNullOrEmpty(x.Trim())))
            {
                var currentNode = rootNode;
                var rpath = path.Split(';');

                var pathItems = rpath[0].Split(separator);
                foreach (var item in pathItems)
                {
                    var tmp = currentNode.Nodes.Cast<TreeNode>().Where(x => x.Text.Equals(item));
                    currentNode = tmp.Count() > 0 ? tmp.Single() : currentNode.Nodes.Add(item);
                    if (item.Contains(".lsj"))
                    {
                        currentNode.Tag = rpath[1] + ";" + rpath[2] + ";" + rpath[3] + ";" + rpath[4];
                    }


                }
            }
            return rootNode;
        }
        public void convertd2(byte[] aasdasd, string pfi)
        {
            List<string> final = new List<string>();
            Rootobject root = JsonConvert.DeserializeObject<Rootobject>(Encoding.UTF8.GetString(aasdasd));
            int index = 0;
            Dictionary<string, int> uuidToNodeId = new Dictionary<string, int>();
            Dictionary<int, List<int>> childDict = new Dictionary<int, List<int>>();
            Dictionary<int, List<string>> textlines = new Dictionary<int, List<string>>();
            Dictionary<int, string> aliases = new Dictionary<int, string>();
            Dictionary<int, string> jumps = new Dictionary<int, string>();
            

            foreach (var node in root.save.regions.dialog.nodes[0].node)
            {
                uuidToNodeId.Add(node.UUID.value, index);
                index++;
            }
            index = 0;
            foreach (var node in root.save.regions.dialog.nodes[0].node)
            {
                List<int> childList1 = new List<int>();
                if (node.children?[0].child?[0].UUID?.value != null)
                {
                    foreach (var child in node.children[0].child)
                    {
                        childList1.Add(uuidToNodeId[child.UUID.value]);
                    }
                }
                childDict.Add(index, childList1);
                index++;
            }

            List<int> rootnodes = new List<int>();
            Dictionary<string, string> speakersdict = new Dictionary<string, string>();

            string synopsis = root.save.regions.editorData.synopsis.value;
            string howtotrigger = "";
            if (root.save.regions.editorData.HowToTrigger != null)
            {
                howtotrigger = root.save.regions.editorData.HowToTrigger.value;
            }


            if (root.save.regions.dialog.speakerlist?[0].speaker?[0].list?.value != null)
            {
                foreach (var item in root.save.regions.dialog.speakerlist[0].speaker)
                {
                    if (item.list != null)
                    {
                        speakersdict.Add(item.index.value, item.list.value);
                    }
                }
            }
            if (root.save.regions.dialog.nodes?[0].RootNodes?[0].RootNodes?.value != null)
            {
                foreach (var item in root.save.regions.dialog.nodes[0].RootNodes)
                {

                    rootnodes.Add(uuidToNodeId[item.RootNodes.value]);
                }
            }
            else
            {
                rootnodes.Add(0);//
            }
            Dictionary<int, List<string>> strings = new Dictionary<int, List<string>>();

            List<List<string>> listoflists = new List<List<string>>();
            List<string> uuidlistcheck = new List<string>();

            index = 0;
            foreach (var node in root.save.regions.dialog.nodes[0].node)
            {
                string uuid = "";
                string constructor = "";
                List<int> childList = new List<int>();
                List<string> text = new List<string>();
                
                List<List<string>> textsss = new List<List<string>>();
                string speaker = "><div><span class='nodeid'>" + index.ToString() + ". </span><a class='anchor' id='n" + index.ToString() + "'></a>";
                int nodespeaker;
                string alias = "";
                List<string> tags = new List<string>();
                List<string> checkflag = new List<string>();
                List<string> setflag = new List<string>();
                //List<string> setflag = new List<string>();
                string approval = "";
                string approval2 = "";
                string poplevel = "";
                string ability = "";
                string skill = "";
                string difficulty = "";
                int advantage = 0;
                string rollres = "";
                string jump = "";
                string endnode = "";
                string context = "";
                bool ischild = false;
                int childid = 0;
                int ischildid = 0;
                int jumptar = 0;
                string jumptargetpoint = "";
                string CinematicNodeContext = "";
                string InternalNodeContext = "";
                string NodeContext = "";
                int aliastar = 0;

                string jjjump = "";
                string aaalias = "";

                List<string> links = new List<string>();
                List<string> test = new List<string>();
                List<string> uuidlist = new List<string>();
                test.Add("[" + index.ToString() + "]");
                //uuid
                uuid = node.UUID.value;
                //text and rulletags
                if (node.TaggedTexts?[0].TaggedText?[0].TagTexts?[0].TagText?[0].TagText?.handle != null)
                {
                    foreach (var texts in node.TaggedTexts[0].TaggedText)
                    {
                        List<string> textags = new List<string>();
                        List<string> textsrules = new List<string>();
                        //text.Add(texts.TagTexts?[0].TagText?[0].TagText?.handle);
                        foreach (var itemtagtext in texts.TagTexts[0].TagText)
                        {
                            if (adgg.ContainsKey(itemtagtext.TagText.handle))
                                textags.Add("<span class='dialog'>" + adgg[itemtagtext.TagText.handle].Split(new[] { "&----&" }, StringSplitOptions.None)[0].Replace("<br>", "&lt;br&gt;") + "</span>");
                            else
                                textags.Add("<span class='dialog'>" + itemtagtext.TagText.handle + "</span>");

                            
                        }


                        if (texts.RuleGroup?[0].Rules?[0].Rulee?[0].Tags?[0].Tag?[0].Object?.value != null)
                        {
                            foreach (var rules in texts.RuleGroup[0].Rules[0].Rulee)
                            {
                                foreach (var rule in rules.Tags[0].Tag)
                                {
                                    //text.Add(rule.Object.value);
                                    if (adgg.ContainsKey(rule.Object.value))
                                        textsrules.Add("<span class='ruletag' title='" + adgg[rule.Object.value].Split(new[] { "&----&" }, StringSplitOptions.None)[1].ToString().Replace("'", "&#39") + "'>" + adgg[rule.Object.value].Split(new[] { "&----&" }, StringSplitOptions.None)[0].ToString() + "</span>");
                                    else
                                        textsrules.Add("<span class='ruletag' title='None'>" + rule.Object.value + "</span>");
                                }
                            }

                        }

                        foreach (var item in textags)
                        {
                            List<string> listnew = [item, .. textsrules];
                            textsss.Add(listnew);
                        }

                        //textss.Reverse(1, textss.Count - 1);
                        
                    }
                }
                //textlines.Add(index, text);
                //childnodes
                if (node.children?[0].child?[0].UUID?.value != null)
                {
                    foreach (var item in childDict)
                    {
                        if (item.Value.Contains(index))
                        {
                            ischild = true;
                        }
                    }
                    foreach (var child in node.children[0].child)
                    {
                        if (rootnodes.Contains(index) || ischild)
                        {
                            if (!uuidlistcheck.Contains(child.UUID.value))
                            {
                                uuidlistcheck.Add(child.UUID.value);
                                uuidlist.Add(uuidToNodeId[child.UUID.value].ToString());
                            }
                            else
                            {
                                links.Add("[+++" + uuidToNodeId[child.UUID.value].ToString() + "+++]");
                            }
                        }
                        //childList.Add(uuidToNodeId[child.UUID.value]);
                    }
                }
                //childDict.Add(index, childList);
                //setflag
                if (node.setflags?[0].flaggroup != null)
                {
                    foreach (var flagsetgroup in node.setflags[0].flaggroup)
                    {
                        foreach (var flagsetgroupitem in flagsetgroup.flag)
                        {
                            if (adgg.ContainsKey(flagsetgroupitem.UUID.value))
                                if (flagsetgroupitem.value.value)
                                    setflag.Add("<span class='setflag' title='" + adgg[flagsetgroupitem.UUID.value].Split(new[] { "&----&" }, StringSplitOptions.None)[1].Replace("'", "&#39") + "'>" + adgg[flagsetgroupitem.UUID.value].Split(new[] { "&----&" }, StringSplitOptions.None)[0].ToString() + "</span>");
                                else
                                    setflag.Add("<span class='setflag' title='" + adgg[flagsetgroupitem.UUID.value].Split(new[] { "&----&" }, StringSplitOptions.None)[1].Replace("'", "&#39") + "'>" + adgg[flagsetgroupitem.UUID.value].Split(new[] { "&----&" }, StringSplitOptions.None)[0].ToString() + "</span>" + " = " + flagsetgroupitem.value.value);
                            else
                                if (flagsetgroupitem.value.value)
                                    setflag.Add("<span class='setflag' title='None'>" + flagsetgroupitem.UUID.value + "</span>");
                                else
                                    setflag.Add("<span class='setflag' title='None'>" + flagsetgroupitem.UUID.value + "</span>" + " = " + flagsetgroupitem.value.value);
                            //setflag.Add(flagsetgroupitem.UUID.value, flagsetgroupitem.value.value);
                        }
                    }
                }
                //checkflag
                if (node.checkflags?[0].flaggroup != null)
                {
                    foreach (var flagcheckgroup in node.checkflags[0].flaggroup)
                    {
                        foreach (var flagcheckgroupitem in flagcheckgroup.flag)
                        {
                            if (adgg.ContainsKey(flagcheckgroupitem.UUID.value))
                                if (flagcheckgroupitem.value.value)
                                    checkflag.Add("<span class='checkflag' title='" + adgg[flagcheckgroupitem.UUID.value].Split(new[] { "&----&" }, StringSplitOptions.None)[1].ToString().Replace("'", "&#39") + "'>" + adgg[flagcheckgroupitem.UUID.value].Split(new[] { "&----&" }, StringSplitOptions.None)[0].ToString() + "</span>");
                                else
                                    checkflag.Add("<span class='checkflag' title='" + adgg[flagcheckgroupitem.UUID.value].Split(new[] { "&----&" }, StringSplitOptions.None)[1].ToString().Replace("'", "&#39") + "'>" + adgg[flagcheckgroupitem.UUID.value].Split(new[] { "&----&" }, StringSplitOptions.None)[0].ToString() + "</span>" + " = " + flagcheckgroupitem.value.value);
                            else
                                if (flagcheckgroupitem.value.value)
                                    checkflag.Add("<span class='checkflag' title='None'>" + flagcheckgroupitem.UUID.value + "</span>");
                                else
                                    checkflag.Add("<span class='checkflag' title='None'>" + flagcheckgroupitem.UUID.value + "</span>" + " = " + flagcheckgroupitem.value.value);
                            //checkflag.Add(flagcheckgroupitem.UUID.value, flagcheckgroupitem.value.value);
                        }
                    }
                }
                //tags
                if (node.Tags?[0].Tagg?[0].Tag != null)
                {
                    foreach (var taggs in node.Tags[0].Tagg)
                    {
                        if (adgg.ContainsKey(taggs.Tag.value))
                            tags.Add("<span class='tags' title='" + adgg[taggs.Tag.value].Split(new[] { "&----&" }, StringSplitOptions.None)[1].ToString().Replace("'", "&#39") + "'>" + adgg[taggs.Tag.value].Split(new[] { "&----&" }, StringSplitOptions.None)[0].ToString() + "</span>");
                        else
                            tags.Add("<span class='tags' title='None'>" + taggs.Tag.value + "</span>");
                        //tags.Add(taggs.Tag.value);
                    }
                }
                
                //alias
                if (node.SourceNode?.value != null)
                {
                    //aliastar = index;
                    test.Add("[===" + uuidToNodeId[node.SourceNode.value].ToString() + "===]");
                    //aliases.Add(index, node.SourceNode.value);
                }
                //jump
                if (node.jumptarget?.value != null)
                {

                    if (node.jumptargetpoint?.value != null)
                    {
                        //jumps.Add(index, uuidToNodeId[node.jumptarget.value] + ":" + node.jumptargetpoint.value);
                        jump = "<span class='goto' data-id='" + uuidToNodeId[node.jumptarget.value].ToString() + "'> Jump to Node " + uuidToNodeId[node.jumptarget.value].ToString() +" (" + node.jumptargetpoint.value + ")</span>";
                        test.Add("[&&&" + uuidToNodeId[node.jumptarget.value].ToString() + "&&&]");
                    }
                    //else
                    //{
                        //jumps.Add(index, uuidToNodeId[node.jumptarget.value].ToString());
                    //}
                }
                constructor = node.constructor.value;
                if (node.endnode?.value != null)
                {
                    if (node.endnode.value)
                    {
                        endnode = "<span class='end'>End</span>";
                    }
                    //endnode = node.endnode.value;
                }
                if (node.PopLevel?.value != null)
                {
                    poplevel = node.PopLevel.value.ToString();
                }
                //speakerid
                if (node.speaker?.value != null)
                {
                    if (node.speaker.value == -666)
                        speaker = "><div><span class='nodeid'>" + index.ToString() + ". </span><a class='anchor' id='n" + index.ToString() + "'></a><div style='display:inline-block;'><div class='npc' style='display: inline-block;'>Narrator</div></a></div><span>: </span>";
                    else if (node.speaker.value < 0)
                        speaker = "><div><span class='nodeid'>" + index.ToString() + ". </span><a class='anchor' id='n" + index.ToString() + "'></a>";
                    else
                    {
                        List<string> speakerlistidlist = new List<string>();
                        List<string> speakerlistid = new List<string>(speakersdict[node.speaker.value.ToString()].Split(';'));
                        if (speakerlistid.Count > 1)
                        {
                            foreach (var speakerid in speakerlistid)
                            {
                                if (adgg.ContainsKey(speakerid))
                                {
                                    if (adgg[speakerid].Split(new[] { "&----&" }, StringSplitOptions.None)[0].ToString() == "Player")
                                    {
                                        speakerlistidlist.Add("Player");
                                    }
                                    else
                                        speakerlistidlist.Add(adgg[speakerid].Split(new[] { "&----&" }, StringSplitOptions.None)[0].ToString());
                                }
                                else
                                {
                                    speakerlistidlist.Add(speakerid);
                                }
                            }
                            speaker = "><div><span class='nodeid'>" + index.ToString() + ". </span><a class='anchor' id='n" + index.ToString() + "'></a><div style='display:inline-block;'><div class='npc' style='display: inline-block;'>" + string.Join(", ", speakerlistidlist.ToArray()) + "</div></a></div><span>: </span>";
                        }
                        else
                        {
                            string speakerid = speakersdict[node.speaker.value.ToString()];

                            if (adgg.ContainsKey(speakerid))
                            {
                                if (adgg[speakerid].Split(new[] { "&----&" }, StringSplitOptions.None)[0].ToString() == "Player")
                                {
                                    speaker = "><div><span class='nodeid'>" + index.ToString() + ". </span><a class='anchor' id='n" + index.ToString() + "'></a><span class='npcplayer'>Player</span><span>: </span>";
                                }
                                else
                                {
                                    if (adgg[speakerid].Split(new[] { "&----&" }, StringSplitOptions.None)[1].ToString() != "")
                                    {
                                        speaker = "><div><span class='nodeid'>" + index.ToString() + ". </span><a class='anchor' id='n" + index.ToString() + "'></a><div style='display:inline-block;'><div class='npcgroup' title='" + adgg[speakerid].Split(new[] { "&----&" }, StringSplitOptions.None)[1].ToString().Replace("'", "&#39") + "' style='display: inline-block;'>" + adgg[speakerid].Split(new[] { "&----&" }, StringSplitOptions.None)[0].ToString() + "</div></a></div><span>: </span>";
                                    }
                                    else
                                        speaker = "><div><span class='nodeid'>" + index.ToString() + ". </span><a class='anchor' id='n" + index.ToString() + "'></a><div style='display:inline-block;'><div class='npc' style='display: inline-block;'>" + adgg[speakerid].Split(new[] { "&----&" }, StringSplitOptions.None)[0].ToString() + "</div></a></div><span>: </span>";
                                }
                            }
                            else
                            {
                                speaker = "><div><span class='nodeid'>" + index.ToString() + ". </span><a class='anchor' id='n" + index.ToString() + "'></a><div style='display:inline-block;'><div class='npc' style='display: inline-block;'>" + speakerid + "</div></a></div><span>: </span>";
                            }

                        }
                    }


                    //nodespeaker = node.speaker.value;
                }

                if (node.editorData?[0].data?[0].key?.value != null)
                {
                    foreach (var data in node.editorData[0].data)
                    {
                        if (data.key.value == "CinematicNodeContext")
                        {
                            CinematicNodeContext = data.val.value;
                            if (CinematicNodeContext != "" && CinematicNodeContext != "<placeholder>")
                                context += " | CinematicNodeContext: " + CinematicNodeContext.Replace("'", "&#39") + "&#013;";
                        }
                        if (data.key.value == "InternalNodeContext")
                        {
                            InternalNodeContext = data.val.value;
                            if(InternalNodeContext != "")
                                context += " | InternalNodeContext: " + InternalNodeContext.Replace("'", "&#39") + "&#013;";
                        }
                        if (data.key.value == "NodeContext")
                        {
                            NodeContext = data.val.value;
                            if (NodeContext != "")
                                context += " | NodeContext: " + NodeContext.Replace("'", "&#39") + "&#013;";
                        }
                    }
                }


                //rolls
                if (node.ApprovalRatingID?.value != null)
                {
                    if (adgg.ContainsKey(node.ApprovalRatingID.value))
                        approval = "<span class='approval'>" + adgg[node.ApprovalRatingID.value].Split(new[] { "&----&" }, StringSplitOptions.None)[0].ToString() + "</span>";
                    else
                        approval = "<span class='approval'>" + node.ApprovalRatingID.value + "</span>";

                    //approval = node.ApprovalRatingID.value;
                }
                if (node.Success?.value != null)
                {
                    rollres = rollres + node.Success.value.ToString();
                }
                if (node.Ability?.value != null)
                {
                    ability = node.Ability.value;
                }
                if (node.DifficultyClassID?.value != null)
                {
                    if (adgg.ContainsKey(node.DifficultyClassID.value))
                        difficulty = adgg[node.DifficultyClassID.value].Split(new[] { "&----&" }, StringSplitOptions.None)[0].ToString();
                    else
                        difficulty = node.DifficultyClassID.value;
                    //difficulty = node.DifficultyClassID.value;
                }
                if (node.Skill?.value != null)
                {
                    skill = node.Skill.value;
                }
                if (node.Advantage?.value != null)
                {
                    advantage = node.Advantage.value;
                }

                foreach (var asd in uuidlist)
                {
                    test.Add("(" + asd.ToString() + ")");
                }
                foreach (var fff in links)
                {
                    test.Add("(" + fff.ToString() + ")");
                }
                listoflists.Add(test);



                string rolls = "";
                if (constructor.Contains("eRoll"))
                    rolls = "<span class='rolls'>" + "Roll " + "(" + ability + ", " + skill + ") vs " + difficulty + " (" + advantage + ")" + "</span>";

                if (context != "")
                    context = "<span class='context' title='" + context + "'><sup>devnote</sup></span>";
                    //context = "<span class='context'>" + context + "'</span>";

                if (textsss.Count > 0)
                {
                    List<string> xfh44ew = new List<string>();
                    foreach (var af in textsss) //string.Join(", ", setflag2.ToArray())
                    {
                        xfh44ew.Add(speaker + string.Join(", ", af.ToArray()) + context + string.Join(", ", tags.ToArray()) + "<span class='checkflag'>" + string.Join(", ", checkflag.ToArray()) + "</span>" + "<span class='setflag'>" + string.Join(", ", setflag.ToArray()) + "</span>" + rolls + approval + "<br>" + endnode);
                    }
                    strings[index] = xfh44ew;
                }
                else
                {
                    strings[index] = new List<string>() { speaker + "[" + constructor + "] " + poplevel + jump + context + rollres + string.Join(", ", tags.ToArray()) + "<span class='checkflag'>" + string.Join(", ", checkflag.ToArray()) + "</span>" + "<span class='setflag'>" + string.Join(", ", setflag.ToArray()) + "</span>" + rolls + approval + "<br>" + endnode };
                }



                index++;
            }

            rootnodes.Sort();
            int hhh = 0;
            foreach (var rrroot in rootnodes)
            {
                textt(listoflists, "(" + rrroot + ")", hhh, strings, final);
            }

            writef(synopsis, pfi, howtotrigger, final);

        }


        public void writef(string synopsis, string pfi, string howtotrigger, List<string> final)
        {
            List<string> dd = new List<string>();

            //var ddd = pfi.Split(new string[] { "\\" }, StringSplitOptions.None);
            // path = "\\Dialogs11111\\Tutorial\\TUT_LowerDeck_AfterImpFight.html";
            var ddd = pfi.Split(new string[] { "/Story/" }, StringSplitOptions.None);
            var ddd2 = ddd[1].Split(new string[] { "/" }, StringSplitOptions.None);
            int level = ddd2.Length - 1;
            string header = "<!DOCTYPE html><html><head><script src=\"" + "../".Repeat(level) + "styles/" + "jquery-3.7.1.min.js\"></script> <link rel=\"stylesheet\" href=\"" + "../".Repeat(level) + "styles/" + "styles.css\" /> </head><body><script type='text/javascript' src='" + "../".Repeat(level) + "styles/" + "CollapsibleLists.compressed.js'></script> <script src='" + "../".Repeat(level) + "styles/" + "scripts.js'></script> <div class='dialog'> <ul class='dialogList collapsibleList'> <span class='ecall' onclick='ecall()'>Expand all</span> | <span class='shflags' onclick='shflags()'>Hide all flags</span> | <span class='shcontext' onclick='shcontext()'>Hide context</span> | <span class='shtags' onclick='shtags()'>Hide tags</span> | <span class='shcheckflag' onclick='shcheckflag()'>Hide checkflag</span> | <span class='shsetflag' onclick='shsetflag()'>Hide setflag</span> | <span class='shroll' onclick='shroll()'>Hide roll</span> | <span class='shapprov' onclick='shapprov()'>Hide approv</span> | <span class='shrules' onclick='shrules()'>Hide ruletag</span> | <span class='shid' onclick='shid()'>Hide node id</span><br>";
            dd.Add(header);
            if (synopsis != "")
            {
                dd.Add("<h4>Synopsis:</h4><span class='synopsis'>" + synopsis.Replace("\r\n", "<br>") + "</span><hr>");
            }

            if (howtotrigger != "")
            {
                dd.Add("<h4>How to trigger:</h4><span class='howtotrigger'>" + howtotrigger.Replace("\r\n", "<br>") + "</span><hr><br><br>");
            }
            else
                dd.Add("<br><br>");

            int depth = 0;
            foreach (string line in final)
            {
                string trimmedLine = line.TrimEnd();
                int newDepth = line.TakeWhile(c => c == '\t').Count();
                if (newDepth > depth)
                {
                    dd.Add("<ul>".Repeat(newDepth - depth));
                }
                else if (depth > newDepth)
                {
                    dd.Add("</ul>".Repeat(depth - newDepth));
                }
                dd.Add("<li" + trimmedLine);
                depth = newDepth;
            }



            dd.Add("</body></html>");
            string path = ddd[1].Replace(".lsj", ".html");
            Directory.CreateDirectory(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory + path));
            File.WriteAllLines(AppDomain.CurrentDomain.BaseDirectory + path, dd);//AppDomain.CurrentDomain.BaseDirectory +
            //MessageBox.Show("Test");
            //dfc2++;
            //converttasks.RemoveAll(x => x.IsCompleted);
            //this.Invoke(new MethodInvoker(delegate () { labelExportedFilesCounter.Text = dfc2.ToString(); }));
        }
        public void textt(List<List<string>> listoflists, string rootindx, int hhh, Dictionary<int, List<string>> strings, List<string> final)
        {
            foreach (var asd in listoflists)
            {
                List<string> matching = new List<string>();
                if (rootindx.Replace("(", "[").Replace(")", "]") == asd[0])
                {
                    //var asdasdasd = Int32.Parse(asd[0].Substring(1, asd[0].Length - 2));
                    foreach (var fg in strings[Int32.Parse(asd[0].Substring(1, asd[0].Length - 2))])
                    {
                        final.Add("\t".Repeat(hhh) + fg + "\n");
                    }
                    foreach (var ffff in asd)
                    {
                        if (ffff.Contains("+++"))
                        {
                            final.Add("\t".Repeat(hhh + 1) + " class='goto'><span class='goto' data-id='" + ffff.Substring(5, ffff.Length - 10) + "'> Link to Node " + ffff.Substring(5, ffff.Length - 10) + "</span></li>" + "\n");
                        }
                        else if (ffff.Contains("==="))
                        {
                            string repepepeepp = "";
                            if (strings[Int32.Parse(ffff.Substring(4, ffff.Length - 8))][0].Contains("<span class='dialog'>"))
                            {
                                if (strings[Int32.Parse(ffff.Substring(4, ffff.Length - 8))][0].Contains("class='npcplayer'>Player</span><"))
                                    repepepeepp += "Player: ";
                                else
                                {
                                    if (strings[Int32.Parse(ffff.Substring(4, ffff.Length - 8))][0].Contains("npcgroup"))//new[] { "<span class='dialog'>" }, StringSplitOptions.None
                                        repepepeepp += strings[Int32.Parse(ffff.Substring(4, ffff.Length - 8))][0].Split(new[] { "style='display: inline-block;'>" }, StringSplitOptions.None)[1].Split(new[] { "</div></a>" }, StringSplitOptions.None)[0] + ": ";
                                    else
                                        repepepeepp += strings[Int32.Parse(ffff.Substring(4, ffff.Length - 8))][0].Split(new[] { "<div class='npc' style='display: inline-block;'>" }, StringSplitOptions.None)[1].Split(new[] { "</div></a>" }, StringSplitOptions.None)[0] + ": ";
                                }
                                repepepeepp += strings[Int32.Parse(ffff.Substring(4, ffff.Length - 8))][0].Split(new[] { "<span class='dialog'>" }, StringSplitOptions.None)[1].Split(new[] { "</span>" }, StringSplitOptions.None)[0];
                                final.Add("\t".Repeat(hhh + 1) + " class='goto' title='" + repepepeepp.Replace("<", "&lt;").Replace(">", "&gt;").Replace("'", "&#39") + "'><span class='goto' data-id='" + ffff.Substring(4, ffff.Length - 8) + "'>Alias to Node " + ffff.Substring(4, ffff.Length - 8) + "</span></li>" + "\n");
                            }
                            else
                                final.Add("\t".Repeat(hhh + 1) + " class='goto' title='" + strings[Int32.Parse(ffff.Substring(4, ffff.Length - 8))][0].Replace("<", "&lt;").Replace(">", "&gt;").Replace("'", "&#39") + "'><span class='goto' data-id='" + ffff.Substring(4, ffff.Length - 8) + "'>Alias to Node " + ffff.Substring(4, ffff.Length - 8) + "</span></li>" + "\n");
                        }
                        else if (ffff.Contains("("))
                            matching.Add(ffff);
                    }
                    hhh += 1;
                    foreach (var l in matching)
                    {
                        textt(listoflists, l, hhh, strings, final);
                    }

                }
            }
        }
        public void lis(List<string> final)
        {
            int depth = 0;
            foreach (string line in final)
            {
                string trimmedLine = line.TrimEnd();
                int newDepth = line.TakeWhile(c => c == '\t').Count();
                if (newDepth > depth)
                {
                    Console.WriteLine("<ul>".Repeat(newDepth - depth));
                }
                else if (depth > newDepth)
                {
                    Console.WriteLine("</ul>".Repeat(depth - newDepth));
                }
                Console.WriteLine("<li>{0}", trimmedLine);
                depth = newDepth;
            }
        }
        public bool getparentorsomething(string uuid, Dictionary<string, string> nodes)
        {

            foreach (var item in nodes)
            {
                if (item.Key.Contains(".children") && item.Key.Contains(".UUID.value"))
                {
                    if (item.Key.Contains(".children") && item.Key.Contains(".UUID.value"))
                    {
                        if (item.Value == uuid)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public int getuuididx(string uuid, JToken nodelist)
        {
            for (int i = 0; i < nodelist.Count(); i++)
            {
                if (nodelist[i]["UUID"]["value"].ToString() == uuid)
                    return i;
            }
            return 0;
        }

        private void readfiletable3(string pakFile)
        {
            string pakFilePath = bg3path + pakFile;
            var pakReader = new PackageReader();
            using var package = pakReader.Read(pakFilePath);
            List<PackagedFileInfo> files = package.Files;
            foreach (var file in files)
            {
                if (file.Name.Contains("/Story/Dialogs/") && file.Name.Contains(".lsj"))
                    if (!exdialogs.Contains(file.Name))
                    {
                        dialogs3(file);
                        exdialogs.Add(file.Name);
                    }
            }

        }

        private void dialogs3(PackagedFileInfo file)
        {
            using var fileStream = file.CreateContentReader();
            using var memoryStream = new MemoryStream();
            fileStream.CopyTo(memoryStream);

            dos2(memoryStream.ToArray(), file.Name);
        }

        public void dos2(byte[] testuncompressedList, string filename)
        {
            var lsj = Encoding.Default.GetString(testuncompressedList).Split('\n');
            //var lsja = File.ReadAllLines(Encoding.Default.GetString(testuncompressedList));

            List<string> flagslist = new List<string>() { "Local", "Tag", "Script", "Global", "Character", "User", "Party" };

            List<string> tags = new List<string>();
            Dictionary<string, string> constructortype = new Dictionary<string, string>
                {
                    { "Alias", "TagAnswer" },
                    { "TagAnswer", "TagAnswer" },
                    { "TagQuestion", "TagQuestion" },
                    { "Jump", "Jump" },
                    { "Visual State", "TagAnswer" },
                    { "RollResult", "PersuasionResult" },
                    { "ActiveRoll", "Persuasion" },
                    { "PassiveRoll", "Persuasion" },
                    { "TagCinematic", "TagAnswer" },
                    { "TagGreeting", "TagGreeting" },
                    { "Pop", "Pop" },
                    { "FallibleQuestionResult", "TagAnswer" }
                };

            bool flag = false;
            bool tag = false;
            bool rollres = false;
            bool visual = false;
            for (int i = 0; i < lsj.Length; i++)
            {
                if (lsj[i].Contains("constructor") && lsj[i].Contains("Visual"))
                {
                    lsj[i] = lsj[i].Replace("\"constructor\" : {\"type\" : \"FixedString\", \"value\" : \"Visual State\"},", "\"constructor\" : {\"type\" : \"FixedString\", \"value\" : \"TagAnswer\"},");
                    visual = true;
                }

                if (visual)
                {
                    var asd = lsj[i];
                    if (lsj[i].Contains("\"setflags\" : [ {} ]"))
                    {
                        lsj[i] = lsj[i].Replace("\"setflags\" : [ {} ]", "\"setflags\" : [ {} ], \"speaker\" : {\"type\" : 4, \"value\" : -1}, \"waittime\" : {\"type\" : 6, \"value\" : -1.0}");
                        visual = false;
                    }

                }

                if (lsj[i].Contains("constructor") && lsj[i].Contains("eRoll"))
                {
                    lsj[i - 1] = lsj[i - 1].Replace("],", "],\"DifficultyMod\" : {\"type\" : 27, \"value\" : 0},\"LevelOverride\" : {\"type\" : 4, \"value\" : 0},\"PersuasionTargetSpeakerIndex\" : {\"type\" : 4, \"value\" : 0},\"ShowOnce\" : {\"type\" : 19, \"value\" : 1},\"StatName\" : {\"type\" : 22, \"value\" : \"\"},\"StatsAttribute\" : {\"type\" : 22, \"value\" : \"None\"},");
                    lsj[i] = lsj[i].Replace("ActiveRoll", "Persuasion").Replace("PassiveRoll", "Persuasion");
                }
                if (lsj[i].Contains("constructor") && lsj[i].Contains("RollResult"))
                {
                    rollres = true;
                    lsj[i] = lsj[i].Replace("RollResult", "PersuasionResult");

                }

                if (rollres)
                {
                    if (lsj[i].Contains("\"setflags\" : [ {} ]"))
                    {
                        lsj[i] = lsj[i].Replace("\"setflags\" : [ {} ]", "\"setflags\" : [ {} ], \"speaker\" : {\"type\" : 4, \"value\" : -1}, \"waittime\" : {\"type\" : 6, \"value\" : -1.0}");
                        rollres = false;
                    }
                }
                if (lsj[i].Contains("flag"))
                {
                    flag = true;
                    tag = false;
                }
                else if (lsj[i].Contains("Tags"))
                {
                    flag = false;
                    tag = true;
                }
                else if (lsj[i].Contains("child"))
                {
                    flag = false;
                    tag = false;
                }
                if (lsj[i].Contains("\"setflags\" : [ {} ]\r"))
                {
                    lsj[i] = lsj[i].Replace("\"setflags\" : [ {} ]\r", "\"setflags\" : [ {} ], \"speaker\" : {\"type\" : 4, \"value\" : -1}, \"waittime\" : {\"type\" : 6, \"value\" : -1.0}");
                }

                if (flag)
                {
                    if (lsj[i].Contains("\"UUID\" : {\"type\" : \"FixedString\","))
                    {
                        var result = lsj[i].Split('"').Where((ss, aa) => aa % 2 == 1).ToList()[4];
                        sqliteCommand.CommandText = "SELECT * FROM tagsflags WHERE uuid=@uuid";
                        sqliteCommand.Parameters.Add(new SqliteParameter("@uuid", result));
                        using (SqliteDataReader reader = sqliteCommand.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    lsj[i] = lsj[i].Replace(result, reader.GetValue(1).ToString());
                                }
                            }
                        }
                        sqliteCommand.Parameters.Clear();
                    }

                    lsj[i] = lsj[i].Replace("\"UUID\" : {\"type\" : \"FixedString\",", "\"name\" : {\"type\" : \"FixedString\",");


                    if (lsj[i].Contains("],\"type\" : {\"type\" : \"FixedString\", \"value\" : \""))
                    {
                        var result = lsj[i].Split('"').Where((ss, aa) => aa % 2 == 1).ToList()[4];
                        if (!flagslist.Contains(result))
                        {
                            lsj[i] = lsj[i].Replace(result, "Character");
                        }
                    }
                }
                if (tag)
                {
                    if (lsj[i].Contains("\"Object\" : {\"type\" : \"guid\", \"value\""))
                    {
                        var result = lsj[i].Split('"').Where((ss, aa) => aa % 2 == 1).ToList()[4];
                        sqliteCommand.CommandText = "SELECT * FROM tagsflags WHERE uuid=@uuid";
                        sqliteCommand.Parameters.Add(new SqliteParameter("@uuid", result));
                        using (SqliteDataReader reader = sqliteCommand.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    lsj[i] = lsj[i].Replace(result, reader.GetValue(1).ToString());
                                }
                            }
                        }
                        sqliteCommand.Parameters.Clear();
                    }
                }

                if (lsj[i].Contains("\"Tag\" : {\"type\" : \"guid\", \"value\""))
                {
                    var result = lsj[i].Split('"').Where((ss, aa) => aa % 2 == 1).ToList()[4];
                    sqliteCommand.CommandText = "SELECT * FROM tagsflags WHERE uuid=@uuid";
                    sqliteCommand.Parameters.Add(new SqliteParameter("@uuid", result));
                    using (SqliteDataReader reader = sqliteCommand.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                lsj[i] = lsj[i].Replace(result, reader.GetValue(1).ToString());
                            }
                        }
                    }
                    sqliteCommand.Parameters.Clear();
                }

                


                lsj[i] = lsj[i].Replace("\"value\" : true", "\"value\" : 1").Replace("\"value\" : false", "\"value\" : 0");
                lsj[i] = lsj[i].Replace("\"constructor\" : {\"type\" : \"FixedString\", \"value\" : \"Alias\"},", "\"constructor\" : {\"type\" : \"FixedString\", \"value\" : \"TagAnswer\"},");
                lsj[i] = lsj[i].Replace("\"constructor\" : {\"type\" : \"FixedString\", \"value\" : \"TagCinematic\"},", "\"constructor\" : {\"type\" : \"FixedString\", \"value\" : \"TagAnswer\"},");
                lsj[i] = lsj[i].Replace("\"constructor\" : {\"type\" : \"FixedString\", \"value\" : \"Trade\"},", "\"constructor\" : {\"type\" : \"FixedString\", \"value\" : \"TagAnswer\"},");
                lsj[i] = lsj[i].Replace("\"constructor\" : {\"type\" : \"FixedString\", \"value\" : \"FallibleQuestionResult\"},", "\"constructor\" : {\"type\" : \"FixedString\", \"value\" : \"PersuasionResult\"},");
                lsj[i] = lsj[i].Replace("\"constructor\" : {\"type\" : \"FixedString\", \"value\" : \"Nested Dialog\"},", "\"constructor\" : {\"type\" : \"FixedString\", \"value\" : \"TagAnswer\"},");

                lsj[i] = lsj[i].Replace("\"type\" : \"uint8\"", "\"type\" : 1")
                    .Replace("\"type\" : \"int32\"", "\"type\" : 4")
                    .Replace("\"type\" : \"float\"", "\"type\" : 6")
                    .Replace("\"type\" : \"bool\"", "\"type\" : 19")
                    .Replace("\"type\" : \"FixedString\"", "\"type\" : 22")
                    .Replace("\"type\" : \"LSString\"", "\"type\" : 23")
                    .Replace("\"type\" : \"guid\"", "\"type\" : 22")
                    .Replace("\"type\" : \"TranslatedString\"", "\"type\" : 28");
                lsj[i] = lsj[i].Replace("\"transitionmode\" : {\"type\" : 1, \"value\" : 2}", "\"transitionmode\" : {\"type\" : 1, \"value\" : 0}");

                if (lsj[i].Contains("handle"))
                {
                    var value = lsj[i].Split('"');

                    sqliteCommand.CommandText = "SELECT * FROM tagsflags WHERE uuid=@uuid";
                    sqliteCommand.Parameters.Add(new SqliteParameter("@uuid", value[3]));
                    using (SqliteDataReader reader = sqliteCommand.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                lsj[i + 1] = lsj[i + 1].Replace("\"type\" : \"TranslatedString\",", "\"type\" : 28, \"value\" : \"" + reader.GetValue(1).ToString() + "\",");
                            }
                        }
                    }
                    sqliteCommand.Parameters.Clear();
                }
            }



            var ddd = filename.Split(new string[] { "/Story/Dialogs/" }, StringSplitOptions.None);
            Directory.CreateDirectory(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory + "\\Dialogs_DOS2DefEd\\" + ddd[1]));
            File.WriteAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\Dialogs_DOS2DefEd\\" + ddd[1], lsj);
            dfc2++;
            this.Invoke(new MethodInvoker(delegate () { labelExportedFilesCounter.Text = dfc2.ToString(); }));
        }

        private void readfiletable4(string file)
        {
            string pathfile = bg3path + file;
            using (FileStream fileStream = new FileStream(pathfile, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader binaryReader1 = new BinaryReader(fileStream))
                {
                    long testoffset = 0;
                    int testsize = 0;
                    int testzsize = 0;

                    binaryReader1.ReadBytes(0x08);
                    long offset = binaryReader1.ReadInt64();
                    binaryReader1.BaseStream.Position = offset;
                    int numFiles = binaryReader1.ReadInt32();
                    int compressedSize = binaryReader1.ReadInt32();
                    byte[] compressedFileList = binaryReader1.ReadBytes(compressedSize);

                    int fileBufferSize = numFiles * 272;
                    var uncompressedList = new byte[fileBufferSize];
                    int uncompressedSize = LZ4Codec.Decode(compressedFileList, 0, compressedFileList.Length, uncompressedList, 0, fileBufferSize, true);
                    //File.WriteAllBytes(Path.GetFileName(pathfile) + ".asd", uncompressedList);
                    using (MemoryStream mem = new MemoryStream(uncompressedList))
                    {
                        using (BinaryReader brmem = new BinaryReader(mem))
                        {
                            for (int i = 0; i < numFiles; i++)
                            {
                                var data = brmem.ReadBytes(0x100);
                                var offs = brmem.ReadBytes(6);
                                Array.Resize(ref offs, 8);
                                var dialogoffset = BitConverter.ToInt64(offs, 0);
                                brmem.ReadBytes(2);
                                var zsize = brmem.ReadUInt32();
                                var size = brmem.ReadUInt32();
                                string x = Encoding.UTF8.GetString(data.TakeWhile(a => a != 0).ToArray());

                                testoffset = dialogoffset;
                                testsize = (int)size;
                                testzsize = (int)zsize;

                                if (testzsize == 0)
                                    continue;

                                if (x.Contains("/Story/Dialogs/") && x.Contains(".lsj") && testzsize != 0)
                                    if (!fdialogs.Contains(x))
                                    {
                                        fdialogs.Add(x + ";" + dialogoffset + ";" + zsize + ";" + size + ";" + file);
                                    }
                            }
                        }
                    }
                }
            }
        }

        public void loadaudiotable()
        {
            string voicepak = bg3path + "\\Localization\\Voice.pak";

            using (FileStream fileStream = new FileStream(voicepak, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader binaryReader1 = new BinaryReader(fileStream))
                {
                    binaryReader1.ReadBytes(0x08);
                    long offset = binaryReader1.ReadInt64();
                    binaryReader1.BaseStream.Position = offset;
                    int numFiles = binaryReader1.ReadInt32();
                    int compressedSize = binaryReader1.ReadInt32();
                    byte[] compressedFileList = binaryReader1.ReadBytes(compressedSize);

                    int fileBufferSize = numFiles * 272;
                    var uncompressedList = new byte[fileBufferSize];
                    int uncompressedSize = LZ4Codec.Decode(compressedFileList, 0, compressedFileList.Length, uncompressedList, 0, fileBufferSize, true);


                    using (MemoryStream mem = new MemoryStream(uncompressedList))
                    {
                        using (BinaryReader brmem = new BinaryReader(mem))
                        {
                            for (int i = 0; i < numFiles; i++)
                            {
                                var data = brmem.ReadBytes(0x100);
                                var offs = brmem.ReadBytes(6);
                                Array.Resize(ref offs, 8);
                                var dialogoffset = BitConverter.ToInt64(offs, 0);
                                brmem.ReadBytes(2);
                                var zsize = brmem.ReadUInt32();
                                var size = brmem.ReadUInt32();
                                string x = Encoding.UTF8.GetString(data.TakeWhile(a => a != 0).ToArray());
                                if (x.Contains(".wem"))
                                    adialogs.Add(x + ";" + dialogoffset + ";" + zsize + ";" + size);
                            }
                        }
                    }
                }
            }
        }
        public void bplay()
        {
            string audiof = "";
            if (listViewDialog.SelectedItems[0].SubItems[2].Text != "")
            {
                audiof = listViewDialog.SelectedItems[0].SubItems[2].Text;

                foreach (var item in adialogs)
                {
                    var itemp = item.Split(';');
                    if (itemp[0].Contains(audiof))
                    {
                        play(itemp[1], itemp[2], itemp[3]);
                        break;
                    }
                }
            }
        }
        public void play(string off, string zsize, string size)
        {
            string voicepak = bg3path + "\\Localization\\Voice.pak";
            long x = 0;
            int y = 0;
            Int64.TryParse(off, out x);
            Int32.TryParse(zsize, out y);
            using (FileStream fileStream = new FileStream(voicepak, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader binaryReader1 = new BinaryReader(fileStream))
                {
                    binaryReader1.BaseStream.Position = x;
                    int compressedSize = y;
                    var compressedFileList = binaryReader1.ReadBytes(compressedSize);

                    File.WriteAllBytes("temp.wem", compressedFileList);
                    var process = Process.Start(AppDomain.CurrentDomain.BaseDirectory + "\\vgmstream-cli.exe", "-o temp.wav temp.wem");
                    process.WaitForExit();
                    SoundPlayer simpleSound = new SoundPlayer(AppDomain.CurrentDomain.BaseDirectory + "\\temp.wav");
                    simpleSound.Play();
                }
            }
        }

        public void extr(string afpath, string off, string zsize, string size)
        {
            string voicepak = bg3path + "\\Localization\\Voice.pak";
            long x = 0;
            int y = 0;
            Int64.TryParse(off, out x);
            Int32.TryParse(zsize, out y);
            var afname = Path.GetFileName(afpath);
            using (FileStream fileStream = new FileStream(voicepak, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader binaryReader1 = new BinaryReader(fileStream))
                {
                    binaryReader1.BaseStream.Position = x;
                    int compressedSize = y;
                    var compressedFileList = binaryReader1.ReadBytes(compressedSize);

                    /*
                    using (SaveFileDialog saveFileDialog1 = new SaveFileDialog())
                    {
                        saveFileDialog1.FileName = afname;
                        saveFileDialog1.Filter = "wem (*.wem)| *.wem";
                        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            File.WriteAllBytes(saveFileDialog1.FileName, compressedFileList);
                            var process = Process.Start(AppDomain.CurrentDomain.BaseDirectory + "\\vgmstream-cli.exe", "-o " + saveFileDialog1.FileName + ".wav " + saveFileDialog1.FileName);
                            process.WaitForExit();
                            File.Delete(saveFileDialog1.FileName);
                        }
                    }
                    */

                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\Extracted_audio\\");
                    File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + "\\Extracted_audio\\" + afname + ".wem", compressedFileList);
                    var process = Process.Start(AppDomain.CurrentDomain.BaseDirectory + "\\vgmstream-cli.exe", "-o " + AppDomain.CurrentDomain.BaseDirectory + "\\Extracted_audio\\" + afname + ".wav " + AppDomain.CurrentDomain.BaseDirectory + "\\Extracted_audio\\" + afname + ".wem");
                    process.WaitForExit();
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\Extracted_audio\\" + afname + ".wem");
                }
            }

        }

        public void LoadDB()
        {
            sqliteCommand.CommandText = "SELECT * FROM tagsflags";
            using (SqliteDataReader reader = sqliteCommand.ExecuteReader())
            {
                if (reader.HasRows)
                {

                    while (reader.Read())
                    {
                        var asd1 = "";
                        var asd2 = "";
                        asd1 = reader.GetValue(0).ToString();
                        asd2 = reader.GetValue(1).ToString() + "&----&" + reader.GetValue(2).ToString();
                        adgg.Add(asd1, asd2);
                    }

                }

            }
            sqliteCommand.Parameters.Clear();
        }
        private async void buttonCreateDB_Click(object sender, EventArgs e)
        {
            checkBoxExportLSJ.Enabled = false;
            buttonCreateDB.Enabled = false;
            buttonExtractHTML.Enabled = false;
            buttonOpen.Enabled = false;
            buttonExtractDE2.Enabled = false;
            buttonLoadTree.Enabled = false;
            comboBoxLanguageSelect.Enabled = false;
            labelDBInfo.Text = "(Creating database)";

            File.Delete("bg3.db");

            var watch = Stopwatch.StartNew();


            connection.Open();
            sqliteCommand.Connection = connection;
            sqliteCommand.CommandText = "CREATE TABLE tagsflags (uuid text, name text, description text)";
            sqliteCommand.ExecuteNonQuery();
            sqliteCommand.CommandText = "CREATE UNIQUE INDEX indx ON tagsflags(uuid)";
            sqliteCommand.ExecuteNonQuery();

            sqliteCommand.CommandText = "begin";
            sqliteCommand.ExecuteNonQuery();






            richTextBoxLog.AppendText("Parsing: " + comboBoxLanguageSelect.Text + ".pak ");

            await Task.Run(() => parsetranslation());
            richTextBoxLog.AppendText("- Done\n");

            richTextBoxLog.AppendText("Parsing: Gustav.pak ");
            await Task.Run(() => readfiletable("\\Gustav.pak"));
            richTextBoxLog.AppendText("- Done\n");

            richTextBoxLog.AppendText("Parsing: Shared.pak ");
            await Task.Run(() => readfiletable("\\Shared.pak"));
            richTextBoxLog.AppendText("- Done\n");

            foreach (var file in Directory.GetFiles(bg3path))
            {
                if (Path.GetFileName(file).StartsWith("Patch"))
                {
                    richTextBoxLog.AppendText("Parsing: " + Path.GetFileName(file) + " ");
                    await Task.Run(() => readfiletable("\\" + Path.GetFileName(file)));
                    richTextBoxLog.AppendText("- Done\n");
                }
            }

            if (File.Exists(bg3path + "\\Localization\\VoiceMeta.pak"))
            {
                richTextBoxLog.AppendText("Parsing: VoiceMeta.pak ");
                await Task.Run(() => readfiletable("\\Localization\\VoiceMeta.pak"));
                richTextBoxLog.AppendText("- Done\n");
            }

            richTextBoxLog.AppendText("Finalizing ");

            await Task.Run(() => finaliz());

            richTextBoxLog.AppendText("- Done\n");

            sqliteCommand.CommandText = "end";
            sqliteCommand.ExecuteNonQuery();

            connection.Close();

            watch.Stop();

            DateTime fd = File.GetCreationTime("bg3.db");
            labelDBInfo.Text = "(Database created: " + fd.ToString() + ")";

            MessageBox.Show("Time: " + watch.Elapsed.TotalSeconds.ToString() + "sec", "Done");


            checkBoxExportLSJ.Enabled = true;
            buttonCreateDB.Enabled = true;
            buttonExtractHTML.Enabled = true;
            buttonOpen.Enabled = true;
            buttonExtractDE2.Enabled = true;
            buttonLoadTree.Enabled = true;
            comboBoxLanguageSelect.Enabled = true;
        }

        private async void buttonExtractHTML_Click(object sender, EventArgs e)
        {
            dfc = 0;
            dfc2 = 0;
            

            exdialogs.Clear();
            richTextBoxLog.Clear();
            labelExportedFiles.Visible = true;
            labelExportedFilesCounter.Visible = true;
            comboBoxLanguageSelect.Enabled = false;
            checkBoxExportLSJ.Enabled = false;
            buttonCreateDB.Enabled = false;
            buttonExtractHTML.Enabled = false;
            buttonOpen.Enabled = false;
            buttonLoadTree.Enabled = false;
            buttonExtractDE2.Enabled = false;
            var watch = Stopwatch.StartNew();

            connection.Open();
            sqliteCommand.Connection = connection;
            sqliteCommand.CommandText = "begin";
            sqliteCommand.ExecuteNonQuery();

            adgg.Clear();

            LoadDB();

            foreach (var file in Directory.GetFiles(bg3path).Reverse())
            {
                if (Path.GetFileName(file).StartsWith("Patch"))
                {
                    richTextBoxLog.AppendText("Load: " + Path.GetFileName(file) + " ");
                    await Task.Run(() => readfiletable2("\\" + Path.GetFileName(file)));
                    richTextBoxLog.AppendText("- Done\n");
                }
            }

            richTextBoxLog.AppendText("Load: Gustav.pak ");

            await Task.Run(() => readfiletable2("\\Gustav.pak"));

            richTextBoxLog.AppendText("- Done\n");
            richTextBoxLog.AppendText("Load: Shared.pak ");

            await Task.Run(() => readfiletable2("\\Shared.pak"));



            richTextBoxLog.AppendText("- Done\n");

            richTextBoxLog.AppendText("Waiting for completion\n");

            adgg.Clear();


            richTextBoxLog.AppendText("Done\n");

            checkBoxExportLSJ.Enabled = true;
            buttonCreateDB.Enabled = true;
            buttonExtractHTML.Enabled = true;
            buttonOpen.Enabled = true;
            buttonLoadTree.Enabled = true;
            comboBoxLanguageSelect.Enabled = true;
            buttonExtractDE2.Enabled = true;
            sqliteCommand.CommandText = "end";
            sqliteCommand.ExecuteNonQuery();
            watch.Stop();
            MessageBox.Show("Time: " + watch.Elapsed.TotalSeconds.ToString() + "sec", "Done");
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                Filter = "Any *.pak in Data | *.pak"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                comboBoxLanguageSelect.Items.Clear();
                bg3path = Path.GetDirectoryName(dialog.FileName);
                if (File.Exists(bg3path + "\\Localization\\English.pak"))
                    comboBoxLanguageSelect.Items.Add("English");
                var locdirs = Directory.GetDirectories(bg3path + "\\Localization\\");
                for (int l = 0; l < locdirs.Length; l++)
                {
                    if (File.Exists(bg3path + "\\Localization\\" + Path.GetFileNameWithoutExtension(locdirs[l]) + "\\" + Path.GetFileNameWithoutExtension(locdirs[l]) + ".pak"))
                        comboBoxLanguageSelect.Items.Add(Path.GetFileNameWithoutExtension(locdirs[l]));
                }
                comboBoxLanguageSelect.SelectedItem = comboBoxLanguageSelect.Items[0];
                buttonCreateDB.Enabled = true;
                buttonLoadTree.Enabled = true;
                comboBoxLanguageSelect.Enabled = true;
                if (File.Exists("bg3.db"))
                {
                    buttonExtractHTML.Enabled = true;
                    buttonExtractDE2.Enabled = true;
                    checkBoxExportLSJ.Enabled = true;
                }
            }
        }

        private async void buttonExtractDE2_Click(object sender, EventArgs e)
        {
            dfc = 0;
            exdialogs.Clear();
            richTextBoxLog.Clear();
            labelExportedFiles.Visible = true;
            comboBoxLanguageSelect.Enabled = false;
            checkBoxExportLSJ.Enabled = false;
            buttonCreateDB.Enabled = false;
            buttonExtractHTML.Enabled = false;
            buttonOpen.Enabled = false;
            buttonExtractDE2.Enabled = false;
            buttonLoadTree.Enabled = false;
            var watch = Stopwatch.StartNew();

            connection.Open();
            sqliteCommand.Connection = connection;
            sqliteCommand.CommandText = "begin";
            sqliteCommand.ExecuteNonQuery();


            foreach (var file in Directory.GetFiles(bg3path).Reverse())
            {
                if (Path.GetFileName(file).StartsWith("Patch"))
                {
                    richTextBoxLog.AppendText("Export from: " + Path.GetFileName(file) + " ");
                    await Task.Run(() => readfiletable3("\\" + Path.GetFileName(file)));
                    richTextBoxLog.AppendText("- Done\n");
                }
            }

            richTextBoxLog.AppendText("Export from: Gustav.pak ");

            await Task.Run(() => readfiletable3("\\Gustav.pak"));

            richTextBoxLog.AppendText("- Done\n");
            richTextBoxLog.AppendText("Export from: Shared.pak ");

            await Task.Run(() => readfiletable3("\\Shared.pak"));

            richTextBoxLog.AppendText("- Done\n");

            checkBoxExportLSJ.Enabled = true;
            buttonCreateDB.Enabled = true;
            buttonExtractHTML.Enabled = true;
            buttonOpen.Enabled = true;
            buttonExtractDE2.Enabled = true;
            buttonLoadTree.Enabled = true;
            comboBoxLanguageSelect.Enabled = true;
            sqliteCommand.CommandText = "end";
            sqliteCommand.ExecuteNonQuery();
            watch.Stop();
            MessageBox.Show("Time: " + watch.Elapsed.TotalSeconds.ToString() + "sec", "Done");
        }

        private async void buttonLoadTree_Click(object sender, EventArgs e)
        {
            fdialogs.Clear();

            buttonLoadTree.Enabled = false;
            buttonCreateDB.Enabled = false;
            buttonExtractHTML.Enabled = false;
            buttonExtractDE2.Enabled = false;
            treeViewDialog.Nodes.Clear();
            connection.Open();
            sqliteCommand.Connection = connection;

            adgg.Clear();
            LoadDB();


            foreach (var file in Directory.GetFiles(bg3path))
            {
                if (Path.GetFileName(file).StartsWith("Patch"))
                {
                    await Task.Run(() => readfiletable4("\\" + Path.GetFileName(file)));
                }
            }
            await Task.Run(() => readfiletable4("\\Gustav.pak"));
            await Task.Run(() => readfiletable4("\\Shared.pak"));
            treeViewDialog.Nodes.Add(MakeTreeFromPaths(fdialogs));
            treeViewDialog.Sort();
            if (adialogs.Count == 0)
            {
                loadaudiotable();
            }
            connection.Close();
            buttonLoadTree.Enabled = true;
            buttonCreateDB.Enabled = true;
            buttonExtractHTML.Enabled = true;
            buttonExtractDE2.Enabled = true;
        }

        private void exportHTMLDialogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            connection.Open();
            sqliteCommand.Connection = connection;
            if (treeViewDialog.SelectedNode != null && treeViewDialog.SelectedNode.Tag != null)
            {
                var tags = treeViewDialog.SelectedNode.Tag.ToString().Split(';');
                string pathfile = bg3path + tags[3];

                using (FileStream fileStream = new FileStream(pathfile, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader binaryReader1 = new BinaryReader(fileStream))
                    {
                        binaryReader1.BaseStream.Position = Convert.ToInt64(tags[0]);
                        int compressedSize = Convert.ToInt32(tags[1]);
                        byte[] compressedFileList = binaryReader1.ReadBytes(compressedSize);

                        int fileBufferSize = Convert.ToInt32(tags[2]);
                        var uncompressedList = new byte[fileBufferSize];
                        int uncompressedSize = LZ4Codec.Decode(compressedFileList, 0, compressedFileList.Length, uncompressedList, 0, fileBufferSize, true);

                        convertd2(uncompressedList, treeViewDialog.SelectedNode.FullPath.Replace("\\", "/"));
                    }
                }
                MessageBox.Show("Done");
            }
            connection.Close();
        }

        private void exportDOS2DialogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            connection.Open();
            sqliteCommand.Connection = connection;
            if (treeViewDialog.SelectedNode != null && treeViewDialog.SelectedNode.Tag != null)
            {
                var tags = treeViewDialog.SelectedNode.Tag.ToString().Split(';');
                string pathfile = bg3path + tags[3];

                using (FileStream fileStream = new FileStream(pathfile, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader binaryReader1 = new BinaryReader(fileStream))
                    {
                        binaryReader1.BaseStream.Position = Convert.ToInt64(tags[0]);
                        int compressedSize = Convert.ToInt32(tags[1]);
                        byte[] compressedFileList = binaryReader1.ReadBytes(compressedSize);

                        int fileBufferSize = Convert.ToInt32(tags[2]);
                        var uncompressedList = new byte[fileBufferSize];
                        int uncompressedSize = LZ4Codec.Decode(compressedFileList, 0, compressedFileList.Length, uncompressedList, 0, fileBufferSize, true);

                        dos2(uncompressedList, treeViewDialog.SelectedNode.FullPath.Replace("\\", "/"));
                    }
                }
                MessageBox.Show("Done");
            }
            connection.Close();
        }

        private void exportLSJDialogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewDialog.SelectedNode != null && treeViewDialog.SelectedNode.Tag != null)
            {
                var tags = treeViewDialog.SelectedNode.Tag.ToString().Split(';');
                string pathfile = bg3path + tags[3];

                using (FileStream fileStream = new FileStream(pathfile, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader binaryReader1 = new BinaryReader(fileStream))
                    {
                        binaryReader1.BaseStream.Position = Convert.ToInt64(tags[0]);
                        int compressedSize = Convert.ToInt32(tags[1]);
                        byte[] compressedFileList = binaryReader1.ReadBytes(compressedSize);

                        int fileBufferSize = Convert.ToInt32(tags[2]);
                        var uncompressedList = new byte[fileBufferSize];
                        int uncompressedSize = LZ4Codec.Decode(compressedFileList, 0, compressedFileList.Length, uncompressedList, 0, fileBufferSize, true);

                        //convertd(uncompressedList, treeView1.SelectedNode.FullPath.Replace("\\", "/"));
                        var asd = treeViewDialog.SelectedNode.FullPath.Split(new string[] { "\\Story\\Dialogs\\" }, StringSplitOptions.None);

                        Directory.CreateDirectory(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory + "\\Dialogs_lsj\\" + asd[1]));
                        File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + "\\Dialogs_lsj\\" + asd[1], uncompressedList);
                    }
                }
                MessageBox.Show("Done");
            }
        }

        private void exportAudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists("vgmstream-cli.exe"))
            {
                if (adialogs.Count == 0)
                {
                    loadaudiotable();
                }
                List<string> dsel = new List<string>();
                var ddd = "";
                if (listViewDialog.SelectedItems.Count == 1)
                {
                    if (listViewDialog.SelectedItems[0].SubItems[2].Text == "")
                    {
                        MessageBox.Show("Selected line do not contain audio files");
                    }
                    else
                    {
                        ddd = listViewDialog.SelectedItems[0].SubItems[2].Text;
                        foreach (var item in adialogs)
                        {
                            var itemp = item.Split(';');
                            if (itemp[0].Contains(ddd))
                            {
                                extr(itemp[0], itemp[1], itemp[2], itemp[3]);
                                break;
                            }
                        }
                    }
                }
                else if (listViewDialog.SelectedItems.Count > 1)
                {
                    for (int i = 0; i < listViewDialog.SelectedItems.Count; i++)
                    {
                        if (listViewDialog.SelectedItems[i].SubItems[2].Text == "")
                        {
                            continue;
                        }
                        else
                        {
                            dsel.Add(listViewDialog.SelectedItems[i].SubItems[2].Text);

                        }
                    }
                    if (dsel.Count == 0)
                    {
                        MessageBox.Show("Selected lines do not contain audio files");
                    }
                    foreach (var itemd in dsel)
                    {
                        foreach (var item in adialogs)
                        {
                            var itemp = item.Split(';');
                            if (itemp[0].Contains(itemd))
                            {
                                extr(itemp[0], itemp[1], itemp[2], itemp[3]);
                                break;
                            }

                        }

                    }

                }
                else
                    MessageBox.Show("Nothing selected");

                MessageBox.Show("Done");
            }
            else
                MessageBox.Show("vgmstream-cli.exe not found");


        }

        private void copyHandleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var h = "";
            List<string> dsel = new List<string>();
            if (listViewDialog.SelectedItems.Count == 1)
            {
                h = "";
                h = listViewDialog.SelectedItems[0].SubItems[0].Text;
                Clipboard.SetText(h);
            }
            else if (listViewDialog.SelectedItems.Count > 1)
            {
                h = "";
                for (int i = 0; i < listViewDialog.SelectedItems.Count; i++)
                {
                    if (listViewDialog.SelectedItems[i].SubItems[0].Text == "")
                    {
                        continue;
                    }
                    else
                    {
                        dsel.Add(listViewDialog.SelectedItems[i].SubItems[0].Text);
                    }
                }
                foreach (var item in dsel)
                {
                    h += item + Environment.NewLine;
                }
                Clipboard.SetText(h);
            }
        }

        private void copyDialogLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var h = "";
            List<string> dsel = new List<string>();
            if (listViewDialog.SelectedItems.Count == 1)
            {
                h = "";
                h = listViewDialog.SelectedItems[0].SubItems[1].Text;
                Clipboard.SetText(h);
            }
            else if (listViewDialog.SelectedItems.Count > 1)
            {
                h = "";
                for (int i = 0; i < listViewDialog.SelectedItems.Count; i++)
                {
                    if (listViewDialog.SelectedItems[i].SubItems[1].Text == "")
                    {
                        continue;
                    }
                    else
                    {
                        dsel.Add(listViewDialog.SelectedItems[i].SubItems[1].Text);
                    }
                }
                foreach (var item in dsel)
                {
                    h += item + Environment.NewLine;
                }
                Clipboard.SetText(h);
            }
        }

        private void copyAudioFilenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var h = "";
            List<string> dsel = new List<string>();
            if (listViewDialog.SelectedItems.Count == 1)
            {
                h = "";
                h = listViewDialog.SelectedItems[0].SubItems[2].Text;
                if (h != "")
                {
                    Clipboard.SetText(h);
                }

            }
            else if (listViewDialog.SelectedItems.Count > 1)
            {
                h = "";
                for (int i = 0; i < listViewDialog.SelectedItems.Count; i++)
                {
                    if (listViewDialog.SelectedItems[i].SubItems[2].Text == "")
                    {
                        continue;
                    }
                    else
                    {
                        dsel.Add(listViewDialog.SelectedItems[i].SubItems[2].Text);
                    }
                }
                foreach (var item in dsel)
                {
                    h += item + Environment.NewLine;
                }
                if (h != "")
                {
                    Clipboard.SetText(h);
                }
            }
        }

        private void linkLabelNotFoundVGM_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/vgmstream/vgmstream/releases") { UseShellExecute = true });
        }

        private void treeViewDialog_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeViewDialog.SelectedNode = e.Node;
                var focusedItem = treeViewDialog.SelectedNode;
                if (focusedItem != null && focusedItem.Bounds.Contains(e.Location))
                {
                    contextMenuStripDialogTree.Enabled = true;
                    contextMenuStripDialogTree.Show(Cursor.Position);
                }
            }
        }

        private void listViewDialog_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var focusedItem = listViewDialog.FocusedItem;
                if (focusedItem != null && focusedItem.Bounds.Contains(e.Location))
                {
                    contextMenuStripDialogList.Enabled = true;
                    contextMenuStripDialogList.Show(Cursor.Position);
                }
            }
        }

        private void listViewDialog_DoubleClick(object sender, EventArgs e)
        {
            if (File.Exists("vgmstream-cli.exe"))
            {
                if (adialogs.Count == 0)
                {
                    loadaudiotable();
                }
                bplay();
            }
            else
                MessageBox.Show("vgmstream-cli.exe not found");
        }

        private void treeViewDialog_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            connection.Open();
            sqliteCommand.Connection = connection;
            if (treeViewDialog.SelectedNode != null && treeViewDialog.SelectedNode.Tag != null)
            {
                labelCurentFilePath.Text = treeViewDialog.SelectedNode.FullPath;
                listViewDialog.Items.Clear();

                var tags = treeViewDialog.SelectedNode.Tag.ToString().Split(';');
                string pathfile = bg3path + tags[3];

                using (FileStream fileStream = new FileStream(pathfile, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader binaryReader1 = new BinaryReader(fileStream))
                    {
                        binaryReader1.BaseStream.Position = Convert.ToInt64(tags[0]);
                        int compressedSize = Convert.ToInt32(tags[1]);
                        byte[] compressedFileList = binaryReader1.ReadBytes(compressedSize);

                        int fileBufferSize = Convert.ToInt32(tags[2]);
                        var uncompressedList = new byte[fileBufferSize];
                        int uncompressedSize = LZ4Codec.Decode(compressedFileList, 0, compressedFileList.Length, uncompressedList, 0, fileBufferSize, true);

                        //richTextBox1.Text = Encoding.UTF8.GetString(uncompressedList);
                        var dialog = JObject.Parse(Encoding.UTF8.GetString(uncompressedList));
                        var values = dialog
                            .SelectTokens("$..*")
                            .Where(t => !t.HasValues)
                            .ToDictionary(t => t.Path, t => t.ToString());



                        int nodes = dialog["save"]["regions"]["dialog"]["nodes"][0]["node"].Count();
                        foreach (var item in values)
                        {
                            if (item.Key.Contains("TagText.handle"))
                            {
                                ListViewItem lvi = listViewDialog.Items.Add(item.Value);
                                sqliteCommand.CommandText = "SELECT * FROM tagsflags WHERE uuid=@uuid";
                                sqliteCommand.Parameters.Add(new SqliteParameter("@uuid", item.Value));
                                using (SqliteDataReader reader = sqliteCommand.ExecuteReader())
                                {
                                    if (reader.HasRows)
                                    {
                                        while (reader.Read())
                                        {
                                            lvi.SubItems.Add(reader.GetValue(1).ToString());
                                            lvi.SubItems.Add(reader.GetValue(2).ToString());
                                        }
                                    }
                                }
                                sqliteCommand.Parameters.Clear();
                            }
                        }
                    }
                }
            }
            connection.Close();
        }

        private void treeViewDialog_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (treeViewDialog.HitTest(e.Location).Node == null)
                {
                    contextMenuStripDialogTree.Enabled = false;
                }

            }
        }

        private void listViewDialog_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listViewDialog.HitTest(e.Location).Item == null)
                {
                    contextMenuStripDialogList.Enabled = false;
                }

            }
        }
    }

    public static class StringExtensions
    {
        public static string Repeat(this string s, int n)
            => new StringBuilder(s.Length * n).Insert(0, s, n).ToString();
    }
}
