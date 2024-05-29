using bg3dialogreader.Properties;
using LSLib.Granny;
using LSLib.Granny.Model;
using LSLib.LS;
using LSLib.LS.Enums;
using LSLib.LS.Story;
using LZ4;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace bg3dialogreader
{
    public partial class Form1 : Form
    {
        private Resource _resource;
        SqliteConnection connection = new SqliteConnection("Data Source=bg3.db");
        SqliteCommand sqlitecommand = new SqliteCommand();
        Dictionary<string, string> testdictfound = new Dictionary<string, string>()
        {
            {"e0d1ff71-04a8-4340-ae64-9684d846eb83", "Player"}
        };
        Dictionary<string, string> testdictnotfound = new Dictionary<string, string>();
        string bg3path = "";
        List<string> exdialogs = new List<string>();
        public Form1()
        {
            InitializeComponent();
        }

        private void parsetranslation()
        {
            string engpak = bg3path + "\\Localization\\English.pak";
            using (FileStream fileStream = new FileStream(engpak, FileMode.Open, FileAccess.Read))
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
                    //File.WriteAllBytes("asd.asd", uncompressedList);
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

                                if (x.Contains("/English/english.loca"))
                                {
                                    binaryReader1.BaseStream.Position = testoffset;
                                    byte[] testcompress = binaryReader1.ReadBytes(testzsize);
                                    //var uncompressedtest = new byte[size];
                                    var testuncompressedList = new byte[testsize];
                                    int testuncompressedSize = LZ4Codec.Decode(testcompress, 0, testcompress.Length, testuncompressedList, 0, testsize, true);

                                    using (MemoryStream fff = new MemoryStream(testuncompressedList))
                                    {
                                        using (var reader = new LocaReader(fff))
                                        {
                                            //reader.Read();
                                            var asd = new MemoryStream();
                                            var resource = reader.Read();


                                            sqlitecommand.CommandText = "INSERT or REPLACE INTO tagsflags VALUES (@handle,@text,null)";
                                            foreach (var item in resource.Entries)
                                            {

                                                sqlitecommand.Parameters.Add(new SqliteParameter("@handle", item.Key));
                                                sqlitecommand.Parameters.Add(new SqliteParameter("@text", item.Text));
                                                sqlitecommand.ExecuteNonQuery();
                                                sqlitecommand.Parameters.Clear();

                                            }



                                        }
                                    }


                                }
                            }
                        }
                    }
                }
            }

        }

        private static void Convert(Resource resource, MemoryStream outputPath, ResourceFormat format, ResourceConversionParameters conversionParams)
        {
            var writer = new LSXWriter(outputPath);
            writer.Version = conversionParams.LSX;
            writer.PrettyPrint = conversionParams.PrettyPrint;
            writer.Write(resource);
        }
        private void dialogs(string filename, BinaryReader br, long offset, int size, int zsize)
        {
            br.BaseStream.Position = offset;
            byte[] testcompress = br.ReadBytes(zsize);
            //var uncompressedtest = new byte[size];
            var testuncompressedList = new byte[size];
            int testuncompressedSize = LZ4Codec.Decode(testcompress, 0, testcompress.Length, testuncompressedList, 0, size, true);
            if (size == 0)
            {
                testuncompressedList = testcompress;
            }
            
            var asd = filename.Split(new string[] { "/Story/Dialogs/" }, StringSplitOptions.None);

            Directory.CreateDirectory(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory + "\\Dialogs_lsj\\" + asd[1]));
            File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + "\\Dialogs_lsj\\" + asd[1], testuncompressedList);
        }
        private void dialogs2(string filename, BinaryReader br, long offset, int size, int zsize)
        {
            br.BaseStream.Position = offset;
            byte[] testcompress = br.ReadBytes(zsize);
            //var uncompressedtest = new byte[size];
            var testuncompressedList = new byte[size];
            int testuncompressedSize = LZ4Codec.Decode(testcompress, 0, testcompress.Length, testuncompressedList, 0, size, true);
            if (size == 0)
            {
                testuncompressedList = testcompress;
            }
            convert(testuncompressedList, filename);

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
                sqlitecommand.CommandText = "INSERT or REPLACE INTO tagsflags VALUES (@uuid,@text,@desc)";

                sqlitecommand.Parameters.Add(new SqliteParameter("@uuid", entry.Key));
                sqlitecommand.Parameters.Add(new SqliteParameter("@text", entry.Value));
                sqlitecommand.Parameters.Add(new SqliteParameter("@desc", DBNull.Value));

                sqlitecommand.ExecuteNonQuery();
                sqlitecommand.Parameters.Clear();
            }

        }

        private void namesmerged(BinaryReader br, long offset, int size, int zsize)
        {
            br.BaseStream.Position = offset;
            byte[] testcompress = br.ReadBytes(zsize);
            //var uncompressedtest = new byte[size];
            var testuncompressedList = new byte[size];
            int testuncompressedSize = LZ4Codec.Decode(testcompress, 0, testcompress.Length, testuncompressedList, 0, size, true);
            if (size == 0)
                testuncompressedList = testcompress;
            using (MemoryStream test = new MemoryStream(testuncompressedList))
            {
                using (var reader = new LSFReader(test))
                {
                    //reader.Read();
                    var asd = new MemoryStream();
                    _resource = reader.Read();
                    ResourceFormat format = ResourceFormat.LSX;
                    var conversionParams = ResourceConversionParameters.FromGameVersion(Game.BaldursGate3);
                    Convert(_resource, asd, format, conversionParams);
                    //string result = Encoding.UTF8.GetString(asd.ToArray());//.Replace("\uFEFF", "");


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

                            if (DisplayName.StartsWith("h"))
                            {
                                sqlitecommand.CommandText = "SELECT * FROM tagsflags WHERE uuid=@uuid";
                                sqlitecommand.Parameters.Add(new SqliteParameter("@uuid", DisplayName));

                                using (SqliteDataReader readers = sqlitecommand.ExecuteReader())
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
                                sqlitecommand.Parameters.Clear();
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
            }
        }
        private void namesorigins(BinaryReader br, long offset, int size, int zsize)
        {
            br.BaseStream.Position = offset;
            byte[] testcompress = br.ReadBytes(zsize);
            //var uncompressedtest = new byte[size];
            var testuncompressedList = new byte[size];
            int testuncompressedSize = LZ4Codec.Decode(testcompress, 0, testcompress.Length, testuncompressedList, 0, size, true);
            if (size == 0)
                testuncompressedList = testcompress;
            using (MemoryStream test = new MemoryStream(testuncompressedList))
            {

                XmlReader xmreader = XmlReader.Create(test);
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
        }
        private void namesspeakergroup(BinaryReader br, long offset, int size, int zsize)
        {
            br.BaseStream.Position = offset;
            byte[] testcompress = br.ReadBytes(zsize);
            //var uncompressedtest = new byte[size];
            var testuncompressedList = new byte[size];
            int testuncompressedSize = LZ4Codec.Decode(testcompress, 0, testcompress.Length, testuncompressedList, 0, size, true);
            if (size == 0)
                testuncompressedList = testcompress;
            using (MemoryStream test = new MemoryStream(testuncompressedList))
            {
                using (var reader = new LSFReader(test))
                {
                    //reader.Read();
                    var asd = new MemoryStream();
                    _resource = reader.Read();
                    ResourceFormat format = ResourceFormat.LSX;
                    var conversionParams = ResourceConversionParameters.FromGameVersion(Game.BaldursGate3);
                    Convert(_resource, asd, format, conversionParams);
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


                        sqlitecommand.CommandText = "INSERT or REPLACE INTO tagsflags VALUES (@uuid,@text,@desc)";

                        sqlitecommand.Parameters.Add(new SqliteParameter("@uuid", uuid));
                        sqlitecommand.Parameters.Add(new SqliteParameter("@text", stringname));
                        sqlitecommand.Parameters.Add(new SqliteParameter("@desc", desc));

                        sqlitecommand.ExecuteNonQuery();
                        sqlitecommand.Parameters.Clear();
                    }

                }
            }
        }
        private void reactions(string filename, BinaryReader br, long offset, int size, int zsize)
        {
            br.BaseStream.Position = offset;
            byte[] testcompress = br.ReadBytes(zsize);
            //var uncompressedtest = new byte[size];
            var testuncompressedList = new byte[size];
            int testuncompressedSize = LZ4Codec.Decode(testcompress, 0, testcompress.Length, testuncompressedList, 0, size, true);
            if (size == 0)
                testuncompressedList = testcompress;
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

            using (MemoryStream test = new MemoryStream(testuncompressedList))
            {

                XmlReader xmreader = XmlReader.Create(test);
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
                    sqlitecommand.CommandText = "INSERT or REPLACE INTO tagsflags VALUES (@uuid,@text,@desc) ON CONFLICT(uuid) DO UPDATE SET uuid=excluded.uuid";
                    sqlitecommand.Parameters.Add(new SqliteParameter("@uuid", Path.GetFileNameWithoutExtension(filename)));

                    if (react == "[")
                        sqlitecommand.Parameters.Add(new SqliteParameter("@text", DBNull.Value));
                    else
                    {
                        react = react.Remove(react.Length - 2);
                        react += "]";
                        sqlitecommand.Parameters.Add(new SqliteParameter("@text", react));
                    }
                    
                    sqlitecommand.Parameters.Add(new SqliteParameter("@desc", DBNull.Value));

                    sqlitecommand.ExecuteNonQuery();
                    sqlitecommand.Parameters.Clear();

                }

            }
        }
        private void difficulties(BinaryReader br, long offset, int size, int zsize)
        {
            br.BaseStream.Position = offset;
            byte[] testcompress = br.ReadBytes(zsize);
            //var uncompressedtest = new byte[size];
            var testuncompressedList = new byte[size];
            int testuncompressedSize = LZ4Codec.Decode(testcompress, 0, testcompress.Length, testuncompressedList, 0, size, true);
            if (size == 0)
                testuncompressedList = testcompress;
            using (MemoryStream test = new MemoryStream(testuncompressedList))
            {

                XmlReader xmreader = XmlReader.Create(test);
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

                    sqlitecommand.CommandText = "INSERT or REPLACE INTO tagsflags VALUES (@uuid,@text,@desc)";
                    sqlitecommand.Parameters.Add(new SqliteParameter("@uuid", uuid));
                    sqlitecommand.Parameters.Add(new SqliteParameter("@text", diff));
                    sqlitecommand.Parameters.Add(new SqliteParameter("@desc", stringname));

                    sqlitecommand.ExecuteNonQuery();
                    sqlitecommand.Parameters.Clear();

                }

            }
        }
        private void questflags(BinaryReader br, long offset, int size, int zsize)
        {
            br.BaseStream.Position = offset;
            byte[] testcompress = br.ReadBytes(zsize);
            //var uncompressedtest = new byte[size];
            var testuncompressedList = new byte[size];
            int testuncompressedSize = LZ4Codec.Decode(testcompress, 0, testcompress.Length, testuncompressedList, 0, size, true);
            if (size == 0)
                testuncompressedList = testcompress;
            using (MemoryStream test = new MemoryStream(testuncompressedList))
            {

                XmlReader xmreader = XmlReader.Create(test);
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


                                sqlitecommand.CommandText = "SELECT * FROM tagsflags WHERE uuid=@uuid";
                                sqlitecommand.Parameters.Add(new SqliteParameter("@uuid", desc));

                                using (SqliteDataReader reader = sqlitecommand.ExecuteReader())
                                {
                                    if (reader.HasRows) 
                                    {
                                        while (reader.Read()) 
                                        {
                                            desc2 = reader.GetValue(1).ToString();
                                        }
                                    }



                                }
                                sqlitecommand.Parameters.Clear();


                                sqlitecommand.CommandText = "INSERT or REPLACE INTO tagsflags VALUES (@uuid,@text,@desc)";
                                sqlitecommand.Parameters.Add(new SqliteParameter("@uuid", uuid));
                                sqlitecommand.Parameters.Add(new SqliteParameter("@text", stringname));
                                sqlitecommand.Parameters.Add(new SqliteParameter("@desc", desc2));
                                sqlitecommand.ExecuteNonQuery();
                                sqlitecommand.Parameters.Clear();
                            }
                        }
                    }

                }

            }
        }
        private void tagsflags(string filename, BinaryReader br, long offset, int size, int zsize)
        {
            br.BaseStream.Position = offset;
            byte[] testcompress = br.ReadBytes(zsize);
            //var uncompressedtest = new byte[size];
            var testuncompressedList = new byte[size];
            int testuncompressedSize = LZ4Codec.Decode(testcompress, 0, testcompress.Length, testuncompressedList, 0, size, true);
            if (size == 0)
                testuncompressedList = testcompress;
            using (MemoryStream test = new MemoryStream(testuncompressedList))
            {
                using (var reader = new LSFReader(test))
                {
                    //reader.Read();
                    var asd = new MemoryStream();
                    _resource = reader.Read();
                    ResourceFormat format = ResourceFormat.LSX;
                    var conversionParams = ResourceConversionParameters.FromGameVersion(Game.BaldursGate3);
                    Convert(_resource, asd, format, conversionParams);
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

                    

                    sqlitecommand.CommandText = "INSERT or REPLACE INTO tagsflags VALUES (@handle,@text,@desc) ON CONFLICT(uuid) DO UPDATE SET uuid=excluded.uuid";
                    sqlitecommand.Parameters.Add(new SqliteParameter("@handle", Path.GetFileNameWithoutExtension(filename)));
                    if (stringname != "")
                        sqlitecommand.Parameters.Add(new SqliteParameter("@text", stringname));
                    else
                        sqlitecommand.Parameters.Add(new SqliteParameter("@text", DBNull.Value));

                    if (desc != "")
                        sqlitecommand.Parameters.Add(new SqliteParameter("@desc", desc));
                    else
                        sqlitecommand.Parameters.Add(new SqliteParameter("@desc", DBNull.Value));
                    sqlitecommand.ExecuteNonQuery();
                    sqlitecommand.Parameters.Clear();



                }
            }
        }

        private void readfiletable(string file)
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
                    //File.WriteAllBytes("asd2.asd", uncompressedList);
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


                                if ((x.Contains("/Tags/") || x.Contains("/Flags/")) && x.Contains(".lsf"))
                                    tagsflags(x, binaryReader1, testoffset, testsize, testzsize);//+
                                if (x.Contains("/Story/Journal/quest_prototypes.lsx"))
                                    questflags(binaryReader1, testoffset, testsize, testzsize);//+
                                if (x.Contains("/ApprovalRatings/Reactions/"))//*.lsx
                                    reactions(x, binaryReader1, testoffset, testsize, testzsize);//+
                                if (x.Contains("/DifficultyClasses/DifficultyClasses.lsx"))
                                    difficulties(binaryReader1, testoffset, testsize, testzsize);//+
                                if ((x.Contains("/Items/_merged.lsf") || x.Contains("/Characters/_merged.lsf") || x.Contains("/RootTemplates/")) && !x.Contains("/Content/"))
                                    namesmerged(binaryReader1, testoffset, testsize, testzsize);
                                if (x.Contains("/Origins/Origins.lsx"))
                                    namesorigins(binaryReader1, testoffset, testsize, testzsize);
                                if (x.Contains("/Voice/SpeakerGroups.lsf"))
                                    namesspeakergroup(binaryReader1, testoffset, testsize, testzsize);



                                if (checkBox1.Checked)
                                {
                                    if (x.Contains("/Story/Dialogs/") && x.Contains(".lsj"))
                                        dialogs(x, binaryReader1, testoffset, testsize, testzsize);
                                }
                                if (checkBox2.Checked && file.StartsWith("\\Patch"))
                                {
                                    if (x.Contains("/Story/Dialogs/") && x.Contains(".lsj"))
                                        dialogs(x, binaryReader1, testoffset, testsize, testzsize);
                                }




                            }


                        }
                    }

                }

            }
        }

        private void readfiletable2(string file)
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
                    //File.WriteAllBytes("asd2.asd", uncompressedList);
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


                                if (x.Contains("/Story/Dialogs/") && x.Contains(".lsj") && testzsize != 0)
                                    if (!exdialogs.Contains(x))
                                    {
                                        dialogs2(x, binaryReader1, testoffset, testsize, testzsize);
                                        exdialogs.Add(x);
                                    }

                            }


                        }
                    }

                }

            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Any *.pak in Data | *.pak";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                bg3path = Path.GetDirectoryName(dialog.FileName);

                checkBox1.Enabled = false;
                checkBox2.Enabled = false;
                button1.Enabled = false;
                button2.Enabled = false;
                File.Delete("bg3.db");

                var watch = System.Diagnostics.Stopwatch.StartNew();





                connection.Open();
                sqlitecommand.Connection = connection;
                sqlitecommand.CommandText = "CREATE TABLE tagsflags (uuid text, name text, description text)";
                sqlitecommand.ExecuteNonQuery();
                sqlitecommand.CommandText = "CREATE UNIQUE INDEX indx ON tagsflags(uuid)";
                sqlitecommand.ExecuteNonQuery();

                sqlitecommand.CommandText = "begin";
                sqlitecommand.ExecuteNonQuery();






                richTextBox1.AppendText("Parsing: English.pak ");

                await Task.Run(() => parsetranslation());

                richTextBox1.AppendText("- Done\n");

                richTextBox1.AppendText("Parsing: Gustav.pak ");

                await Task.Run(() => readfiletable("\\Gustav.pak"));

                richTextBox1.AppendText("- Done\n");
                richTextBox1.AppendText("Parsing: Shared.pak ");

                await Task.Run(() => readfiletable("\\Shared.pak"));

                richTextBox1.AppendText("- Done\n");


                foreach (var file in Directory.GetFiles(bg3path))
                {
                    if (Path.GetFileName(file).StartsWith("Patch"))
                    {
                        richTextBox1.AppendText("Parsing: " + Path.GetFileName(file) + " ");
                        await Task.Run(() => readfiletable("\\" + Path.GetFileName(file)));
                        richTextBox1.AppendText("- Done\n");
                    }
                }




                richTextBox1.AppendText("Finalizing ");

                await Task.Run(() => finaliz());

                richTextBox1.AppendText("- Done\n");

                sqlitecommand.CommandText = "end";
                sqlitecommand.ExecuteNonQuery();

                connection.Close();


                watch.Stop();
                MessageBox.Show("Time: " + watch.Elapsed.TotalSeconds.ToString() + "sec", "Done");

                checkBox1.Enabled = true;
                checkBox2.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
            }

        }
        private async void button2_Click(object sender, EventArgs e)
        {
            exdialogs.Clear();
            richTextBox1.Clear();
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Any *.pak in Data | *.pak";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                bg3path = Path.GetDirectoryName(dialog.FileName);

                checkBox1.Enabled = false;
                checkBox2.Enabled = false;
                button1.Enabled = false;
                button2.Enabled = false;
                var watch = System.Diagnostics.Stopwatch.StartNew();

                connection.Open();
                sqlitecommand.Connection = connection;
                sqlitecommand.CommandText = "begin";
                sqlitecommand.ExecuteNonQuery();


                foreach (var file in Directory.GetFiles(bg3path).Reverse())
                {
                    if (Path.GetFileName(file).StartsWith("Patch"))
                    {
                        richTextBox1.AppendText("Export from: " + Path.GetFileName(file) + " ");
                        await Task.Run(() => readfiletable2("\\" + Path.GetFileName(file)));
                        richTextBox1.AppendText("- Done\n");
                    }
                }

                richTextBox1.AppendText("Export from: Gustav.pak ");

                await Task.Run(() => readfiletable2("\\Gustav.pak"));

                richTextBox1.AppendText("- Done\n");
                richTextBox1.AppendText("Export from: Shared.pak ");

                await Task.Run(() => readfiletable2("\\Shared.pak"));

                richTextBox1.AppendText("- Done\n");

                checkBox1.Enabled = true;
                checkBox2.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                sqlitecommand.CommandText = "end";
                sqlitecommand.ExecuteNonQuery();
                watch.Stop();
                MessageBox.Show("Time: " + watch.Elapsed.TotalSeconds.ToString() + "sec", "Done");
            }
        }
        public void convert(byte[] aasdasd, string pfi)
        {
            connection.Open();
            sqlitecommand.Connection = connection;

            

            //var ghhahre = Directory.GetFiles("Dialogs\\", "*", SearchOption.AllDirectories);

            //var dialog = JObject.Parse(File.ReadAllText("Dialogs\\Act3\\EndGame\\END_BrainBattle_CombatOver.lsj"));
            final.Clear();

            string dddddddddd = Encoding.UTF8.GetString(aasdasd.TakeWhile(a => a != 0).ToArray());

            var dialog = JObject.Parse(dddddddddd);

            List<string> speakers = new List<string>();
            List<string> root = new List<string>();
            Dictionary<string, string> speakersdict = new Dictionary<string, string>();

            JToken speakerlist = null;
            JToken rootnodelist = null;

            var nodelist = dialog["save"]["regions"]["dialog"]["nodes"][0]["node"];


            speakerlist = dialog["save"]["regions"]["dialog"]["speakerlist"][0]["speaker"];
                
            rootnodelist = dialog["save"]["regions"]["dialog"]["nodes"][0]["RootNodes"];
            string synopsis = dialog["save"]["regions"]["editorData"]["synopsis"]["value"].ToString();

            if (speakerlist != null)
            {
                foreach (var item in speakerlist)
                {
                    if (item["list"] != null)
                    {
                        speakers.Add(item["list"]["value"].ToString());
                        speakersdict[item["index"]["value"].ToString()] = item["list"]["value"].ToString();
                    }
                }
            }
            if (rootnodelist != null)
            {
                foreach (var x in rootnodelist)
                {
                    var r = getuuididx(x["RootNodes"]["value"].ToString(), nodelist);
                    root.Add("(" + r.ToString() + ")");
                }
            }
            else
                root.Add("(" + 0.ToString() + ")");

            var nodes = nodelist
                .SelectTokens("$..*")
                .Where(t => !t.HasValues)
                .ToDictionary(t => t.Path, t => t.ToString());

            Dictionary<int, List<string>> strings = new Dictionary<int, List<string>>();

            List<List<string>> listoflists = new List<List<string>>();
            List<string> uuidlistcheck = new List<string>();



            // int nodescount = ( as JObject).Count;
            for (int nodeidx = 0; nodeidx < nodelist.Count(); nodeidx++)
            {

                string ttt = "save.regions.dialog.nodes[0].node[" + nodeidx.ToString() + "]";
                string uuid = "";
                string constructor = "";
                List<string> childslist = new List<string>();
                List<string> text = new List<string>();
                string speaker = "><div><span class='nodeid'>" + nodeidx.ToString() + ". </span><a class='anchor' id='n" + nodeidx.ToString() + "'></a>";
                string alias = "";
                List<string> tags = new List<string>();
                List<string> checkflag = new List<string>();
                List<string> setflag = new List<string>();
                string approval = "";
                string approval2 = "";
                string poplevel = "";
                string ability = "";
                string skill = "";
                string difficulty = "";
                string advantage = "";
                string rollres = "";
                string jump = "";
                string endnode = "";
                string context = "";
                int childid = 0;
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
                test.Add("[" + nodeidx.ToString() + "]");
                //for (int x = 0; x < nodes.Count; x++)
                foreach (var item in nodes)
                {

                    if (item.Key.StartsWith(ttt))
                    {
                        if (item.Key.Contains("TagText.handle"))
                        {
                            sqlitecommand.CommandText = "SELECT * FROM tagsflags WHERE uuid=@uuid";
                            sqlitecommand.Parameters.Add(new SqliteParameter("@uuid", item.Value));
                            using (SqliteDataReader reader = sqlitecommand.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        text.Add("<span class='dialog'>" + reader.GetValue(1).ToString().Replace("<br>", "&lt;br&gt;") + "</span>");
                                    }
                                }
                                else
                                    text.Add("<span class='dialog'>" + item.Value + "</span>");
                            }
                            sqlitecommand.Parameters.Clear();
                        }
                        if (item.Key.Contains(".children") && item.Key.Contains(".UUID.value"))
                        {
                            childid = getuuididx(item.Value, nodelist);
                            if (!uuidlistcheck.Contains(item.Value))
                            {
                                uuidlistcheck.Add(item.Value);
                                uuidlist.Add(childid.ToString());
                            }
                            else
                            {
                                links.Add("[+++" + childid.ToString() + "+++]");
                            }
                        }
                        if (item.Key.Contains(".setflags") && item.Key.Contains(".UUID.value"))
                        {
                            sqlitecommand.CommandText = "SELECT * FROM tagsflags WHERE uuid=@uuid";
                            sqlitecommand.Parameters.Add(new SqliteParameter("@uuid", item.Value));
                            using (SqliteDataReader reader = sqlitecommand.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        setflag.Add("<span class='setflag' title='" + reader.GetValue(2).ToString().Replace("'", "&#39") + "'>" + reader.GetValue(1).ToString() + "</span>");
                                    }
                                }
                                else
                                    setflag.Add("<span class='setflag' title='None'>" + item.Value + "</span>");
                            }
                            sqlitecommand.Parameters.Clear();
                        }
                        if (item.Key.Contains(".setflags") && item.Key.Contains(".value.value"))
                            setflag.Add(item.Value);

                        if (item.Key.Contains(".checkflag") && item.Key.Contains(".UUID.value"))
                        {
                            sqlitecommand.CommandText = "SELECT * FROM tagsflags WHERE uuid=@uuid";
                            sqlitecommand.Parameters.Add(new SqliteParameter("@uuid", item.Value));
                            using (SqliteDataReader reader = sqlitecommand.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        checkflag.Add("<span class='checkflag' title='" + reader.GetValue(2).ToString().Replace("'", "&#39") + "'>" + reader.GetValue(1).ToString() + "</span>");
                                    }
                                }
                                else
                                    checkflag.Add("<span class='checkflag' title='None'>" + item.Value + "</span>");
                            }
                            sqlitecommand.Parameters.Clear();
                        }
                        if (item.Key.Contains(".checkflag") && item.Key.Contains(".value.value"))
                            checkflag.Add(item.Value);

                        if (item.Key.Contains("Rules") && item.Key.Contains(".Object.value"))
                        {
                            sqlitecommand.CommandText = "SELECT * FROM tagsflags WHERE uuid=@uuid";
                            sqlitecommand.Parameters.Add(new SqliteParameter("@uuid", item.Value));
                            using (SqliteDataReader reader = sqlitecommand.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        text.Add("<span class='ruletag' title='" + reader.GetValue(2).ToString().Replace("'", "&#39") + "'>" + reader.GetValue(1).ToString() + "</span>");
                                    }
                                }
                                else
                                    text.Add("<span class='ruletag' title='None'>" + item.Value + "</span>");
                            }
                            sqlitecommand.Parameters.Clear();
                        }

                        if (item.Key.Contains("Tags") && item.Key.Contains(".Tag.value"))
                        {
                            sqlitecommand.CommandText = "SELECT * FROM tagsflags WHERE uuid=@uuid";
                            sqlitecommand.Parameters.Add(new SqliteParameter("@uuid", item.Value));
                            using (SqliteDataReader reader = sqlitecommand.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        tags.Add("<span class='tags' title='" + reader.GetValue(2).ToString().Replace("'", "&#39") + "'>" + reader.GetValue(1).ToString() + "</span>");
                                    }
                                }
                                else
                                    tags.Add("<span class='tags' title='None'>" + item.Value + "</span>");
                            }
                            sqlitecommand.Parameters.Clear();
                        }
                        if (item.Key.Contains("SourceNode.value"))
                        {
                            aliastar = getuuididx(item.Value, nodelist);
                            test.Add("[===" + aliastar.ToString() + "===]");
                        }
                        if (item.Key.Contains("jumptarget.value"))
                        {
                            jumptar = getuuididx(item.Value, nodelist);
                            jump = "<span class='goto' data-id='" + jumptar.ToString() + "'> Jump to Node " + jumptar.ToString();
                            test.Add("[&&&" + jumptar.ToString() + "&&&]");
                        }
                        if (item.Key.Contains("jumptargetpoint.value"))
                        {
                            jumptargetpoint = item.Value;
                            jump += " (" + jumptargetpoint + ")</span>";
                        }
                        if (item.Key.Contains("constructor.value"))
                            constructor = item.Value;
                        if (item.Key.Contains("PopLevel"))
                            poplevel = item.Value;
                        if (item.Key.Contains("endnode.value"))
                        {
                            if (item.Value == "True")
                            {
                                endnode = "<span class='end'>End</span>";
                            }
                        }
                        if (item.Value == "CinematicNodeContext")
                        {
                            CinematicNodeContext = nodes[item.Key.Replace("key.value", "val.value")];
                            if (CinematicNodeContext != "" && CinematicNodeContext != "<placeholder>")
                                context += "CinematicNodeContext: " + CinematicNodeContext.Replace("'", "&#39") + "&#013;";
                        }
                        if (item.Value == "InternalNodeContext")
                        {
                            InternalNodeContext = nodes[item.Key.Replace("key.value", "val.value")];
                            if (InternalNodeContext != "")
                                context += "InternalNodeContext: " + InternalNodeContext.Replace("'", "&#39") + "&#013;";
                        }
                        if (item.Value == "NodeContext")
                        {
                            NodeContext = nodes[item.Key.Replace("key.value", "val.value")];
                            if (NodeContext != "")
                                context += "NodeContext: " + NodeContext.Replace("'", "&#39") + "&#013;";
                        }

                        if (!item.Key.Contains("Rules") && item.Key.Contains(".speaker.value"))
                        {
                            if (item.Value == "-666")
                                speaker = "><div><span class='nodeid'>" + nodeidx.ToString() + ". </span><a class='anchor' id='n" + nodeidx.ToString() + "'></a><div style='display:inline-block;'><div class='npc' style='display: inline-block;'>Narrator</div></a></div><span>: </span>";
                            else if (Int32.Parse(item.Value) < 0)
                                speaker = "><div><span class='nodeid'>" + nodeidx.ToString() + ". </span><a class='anchor' id='n" + nodeidx.ToString() + "'></a>";
                            else
                            {
                                List<string> speakerlistidlist = new List<string>();
                                List<string> speakerlistid = new List<string>(speakersdict[item.Value].Split(';'));
                                if (speakerlistid.Count > 1)
                                {
                                    foreach (var speakerid in speakerlistid)
                                    {
                                        sqlitecommand.CommandText = "SELECT * FROM tagsflags WHERE uuid=@uuid";
                                        sqlitecommand.Parameters.Add(new SqliteParameter("@uuid", speakerid));
                                        using (SqliteDataReader reader = sqlitecommand.ExecuteReader())
                                        {
                                            if (reader.HasRows)
                                            {
                                                while (reader.Read())
                                                {
                                                    if (reader.GetValue(1).ToString() == "Player")
                                                    {
                                                        speakerlistidlist.Add("Player");
                                                    }
                                                    else
                                                        speakerlistidlist.Add(reader.GetValue(1).ToString());
                                                }
                                            }
                                            else
                                                speakerlistidlist.Add(speakerid);
                                        }
                                        sqlitecommand.Parameters.Clear();
                                    }
                                    speaker = "><div><span class='nodeid'>" + nodeidx.ToString() + ". </span><a class='anchor' id='n" + nodeidx.ToString() + "'></a><div style='display:inline-block;'><div class='npc' style='display: inline-block;'>" + string.Join(", ", speakerlistidlist.ToArray()) + "</div></a></div><span>: </span>";
                                }
                                else
                                {
                                    string speakerid = speakersdict[item.Value];
                                    sqlitecommand.CommandText = "SELECT * FROM tagsflags WHERE uuid=@uuid";
                                    sqlitecommand.Parameters.Add(new SqliteParameter("@uuid", speakerid));
                                    using (SqliteDataReader reader = sqlitecommand.ExecuteReader())
                                    {
                                        if (reader.HasRows)
                                        {
                                            while (reader.Read())
                                            {
                                                if (reader.GetValue(1).ToString() == "Player")
                                                {
                                                    speaker = "><div><span class='nodeid'>" + nodeidx.ToString() + ". </span><a class='anchor' id='n" + nodeidx.ToString() + "'></a><span class='npcplayer'>Player</span><span>: </span>";
                                                }
                                                else
                                                {
                                                    if (reader.GetValue(2).ToString() != "")
                                                    {
                                                        speaker = "><div><span class='nodeid'>" + nodeidx.ToString() + ". </span><a class='anchor' id='n" + nodeidx.ToString() + "'></a><div style='display:inline-block;'><div class='npcgroup' title='" + reader.GetValue(2).ToString().Replace("'", "&#39") + "' style='display: inline-block;'>" + reader.GetValue(1).ToString() + "</div></a></div><span>: </span>";
                                                    }
                                                    else
                                                        speaker = "><div><span class='nodeid'>" + nodeidx.ToString() + ". </span><a class='anchor' id='n" + nodeidx.ToString() + "'></a><div style='display:inline-block;'><div class='npc' style='display: inline-block;'>" + reader.GetValue(1).ToString() + "</div></a></div><span>: </span>";
                                                }
                                            }
                                        }
                                        else
                                            speaker = "><div><span class='nodeid'>" + nodeidx.ToString() + ". </span><a class='anchor' id='n" + nodeidx.ToString() + "'></a><div style='display:inline-block;'><div class='npc' style='display: inline-block;'>" + speakerid + "</div></a></div><span>: </span>";
                                    }
                                    sqlitecommand.Parameters.Clear();
                                }
                            }
                        }
                        if (item.Key.Contains(".ApprovalRatingID.value"))
                        {
                            sqlitecommand.CommandText = "SELECT * FROM tagsflags WHERE uuid=@uuid";
                            sqlitecommand.Parameters.Add(new SqliteParameter("@uuid", item.Value));
                            using (SqliteDataReader reader = sqlitecommand.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        approval = "<span class='approval'>" + reader.GetValue(1).ToString() + "</span>";
                                    }
                                }
                                else
                                    approval = "<span class='approval'>" + item.Value + "</span>";
                            }
                            sqlitecommand.Parameters.Clear();
                        }
                        if (item.Key.Contains("Success.value"))
                            rollres = rollres + item.Value;
                        if (item.Key.Contains("Ability.value"))
                            ability = item.Value;
                        if (item.Key.Contains("DifficultyClassID.value"))
                        {
                            sqlitecommand.CommandText = "SELECT * FROM tagsflags WHERE uuid=@uuid";
                            sqlitecommand.Parameters.Add(new SqliteParameter("@uuid", item.Value));
                            using (SqliteDataReader reader = sqlitecommand.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        difficulty = reader.GetValue(1).ToString();
                                    }
                                }
                                else
                                    difficulty = item.Value;
                            }
                            sqlitecommand.Parameters.Clear();
                        }
                        if (item.Key.Contains("Skill.value"))
                            skill = item.Value;
                        if (item.Key.Contains("Advantage.value"))
                            advantage = item.Value;


                    }

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


                List<string> setflag2 = new List<string>();
                for (int idx = 0; idx < setflag.Count; idx++)
                {
                    if (setflag[idx] == "True")
                    {
                        setflag2.Add(setflag[idx - 1]);
                    }
                    else if (setflag[idx] == "False")
                    {
                        setflag2.Add(setflag[idx - 1] + " = False");
                    }
                }

                List<string> checkflag2 = new List<string>();
                for (int idx = 0; idx < checkflag.Count; idx++)
                {
                    if (checkflag[idx] == "True")
                    {
                        checkflag2.Add(checkflag[idx - 1]);
                    }
                    else if (checkflag[idx] == "False")
                    {
                        checkflag2.Add(checkflag[idx - 1] + " = False");
                    }
                }

                if (text.Count > 0)
                {
                    text.Reverse();
                    List<int> aa = new List<int>();
                    for (int i = 0; i < text.Count; i++)
                    {
                        if (text[i].StartsWith("<span class='dialog"))
                        {
                            aa.Add(i);
                        }
                    }
                    List<int> y = new List<int>();
                    for (int i = 1; i < aa.Count; i++)
                    {
                        y.Add(aa[i]);
                    }
                    y.Add(text.Count);
                    List<List<string>> z = new List<List<string>>();
                    for (int i = 0; i < aa.Count; i++)
                    {
                        List<string> sublist = new List<string>();
                        for (int j = aa[i]; j < y[i]; j++)
                        {
                            sublist.Add(text[j]);
                        }
                        z.Add(sublist);
                    }


                    List<string> xfh44ew = new List<string>();
                    z.Reverse();
                    foreach (var af in z) //string.Join(", ", setflag2.ToArray())
                    {
                        xfh44ew.Add(speaker + string.Join(", ", af.ToArray()) + context + string.Join(", ", tags.ToArray()) + "<span class='checkflag'>" + string.Join(", ", checkflag2.ToArray()) + "</span>" + "<span class='setflag'>" + string.Join(", ", setflag2.ToArray()) + "</span>" + rolls + approval + "<br>" + endnode);
                    }
                    strings[nodeidx] = xfh44ew;
                }
                else
                {
                    strings[nodeidx] = new List<string>() { speaker + "[" + constructor + "] " + poplevel + jump + context + rollres + string.Join(", ", tags.ToArray()) + "<span class='checkflag'>" + string.Join(", ", checkflag2.ToArray()) + "</span>" + "<span class='setflag'>" + string.Join(", ", setflag2.ToArray()) + "</span>" + rolls + approval + "<br>" + endnode };
                }







            }
            List<int> lst = root.Select(ee => int.Parse(ee.Substring(1, ee.Length - 2))).ToList();
            lst.Sort();
            root = lst.Select(ee => "(" + ee.ToString() + ")").ToList();
            int hhh = 0;
            foreach (var rrroot in root)
            {
                textt(listoflists, rrroot, hhh, strings);
            }

            writef(synopsis, pfi);

        }
        public List<string> final = new List<string>();

        public void writef(string synopsis, string pfi)
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
                dd.Add("<span class='synopsis'>" + synopsis.Replace("\r\n", "<br>") + "</span><br><br>");
            }
            
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
        }
        public void textt(List<List<string>> listoflists, string rootindx, int hhh, Dictionary<int, List<string>> strings)
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
                        textt(listoflists, l, hhh, strings);
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
        
        public int getuuididx(string uuid, JToken nodelist)
        {
            for (int i = 0; i < nodelist.Count(); i++)
            {
                if (nodelist[i]["UUID"]["value"].ToString() == uuid)
                    return i;
            }
            return 0;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                checkBox2.Enabled = false;
                checkBox2.Checked = false;
            }
            if (checkBox1.Checked == false)
            {
                checkBox2.Enabled = true;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                checkBox1.Enabled = false;
                checkBox1.Checked = false;
            }
            if (checkBox2.Checked == false)
            {
                checkBox1.Enabled = true;
            }
        }
    }
    public static class StringExtensions
    {
        public static string Repeat(this string s, int n)
            => new StringBuilder(s.Length * n).Insert(0, s, n).ToString();
    }
}
