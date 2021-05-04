using SoulsFormats;
using SoulsFormatsExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace ClusterFXR
{
    class Program
    {
        public static List<string> BinarySelect(List<string> entries, string selector)
        {
            selector = selector.ToUpper();
            if (selector.Length > 0)
            {
                var next = selector.Substring(0, 1);

                if (selector.Length == 1)
                    selector = "";
                else
                    selector = selector.Substring(1);


                if (next == "A" || next == "B")
                {
                    int totalCount = entries.Count;

                    if (totalCount == 1)
                        return entries;

                    int firstHalfCount = totalCount / 2;

                    List<string> firstHalf = new List<string>();
                    List<string> secondHalf = new List<string>();

                    for (int i = 0; i < firstHalfCount; i++)
                    {
                        firstHalf.Add(entries[0]);
                        entries.RemoveAt(0);
                    }

                    secondHalf = entries;

                    if (next == "A")
                    {
                        return BinarySelect(firstHalf, selector);
                    }
                    else
                    {
                        return BinarySelect(secondHalf, selector);
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid selector character. Each char must be A or B to select first or second half.");
                }
                
            }
            else
            {
                return entries;
            }
        }

        static Random rand;

        static float RandF(float min, float max)
        {
            return min + ((max - min) * (float)rand.NextDouble());
        }

        public static float config_randRange = 1;
        public static bool config_verboseinternalfiles = false;
        public static bool config_verbosefields = false;

        public static bool config_reduceseizure = false;
        public static float config_reduceSeizureRangeScale = 0.5f;

        public static bool config_shufflefields = false;
        public static bool config_replaceFieldsWithNewSequences = true;

        public static int SMALL_SFX_SIZE_CUTOFF = 80 * 1024; //30 KB

        public static float config_randMin = 0.7f;
        public static float config_randMax = 1.4f;

        public static float config_loopToSingle_MinDuration = 0.25f;
        public static float config_loopToSingle_MaxDuration = 2.0f;

        static float RandMeme(bool cantBeNegative)
        {
            //if (cantBeNegative)
            //    return RandF(1, 3f);
            //else
            //    return RandF(-3f, 3f);
            return RandF(config_randMin, config_randMax);
        }

        static Dictionary<Type, List<System.Reflection.FieldInfo>> fieldCache = new Dictionary<Type, List<System.Reflection.FieldInfo>>();

        //List<System.Reflection.FieldInfo> GetFXFieldsOfType(Type type)
        //{
        //    if (fieldCache.ContainsKey(type))
        //    {
        //        return fieldCache[type];
        //    }
        //    else
        //    {
        //        var fields = type.GetFields(System.Reflection.BindingFlags.Public
        //        | System.Reflection.BindingFlags.Instance)
        //        .Where(f => f.FieldType == typeof(FXR1.FXField)).ToList();
        //        fieldCache.Add(type, fields);
        //        return fields;
        //    }
        //}

        static List<System.Reflection.FieldInfo> GetAllFieldsOfType(Type type)
        {
            if (fieldCache.ContainsKey(type))
            {
                return fieldCache[type];
            }
            else
            {
                var fields = type.GetFields(System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.Instance).ToList();
                fieldCache.Add(type, fields);
                return fields;
            }
        }

        public static string GetShortIngameFileName(string fileName)
        {
            return GetFileNameWithoutAnyExtensions(GetFileNameWithoutDirectoryOrExtension(fileName));
        }

        private static readonly char[] _dirSep = new char[] { '\\', '/' };
        public static string GetFileNameWithoutDirectoryOrExtension(string fileName)
        {
            if (fileName.EndsWith("\\") || fileName.EndsWith("/"))
                fileName = fileName.TrimEnd(_dirSep);

            if (fileName.Contains("\\") || fileName.Contains("/"))
                fileName = fileName.Substring(fileName.LastIndexOfAny(_dirSep) + 1);

            if (fileName.Contains("."))
                fileName = fileName.Substring(0, fileName.LastIndexOf('.'));

            return fileName;
        }

        public static string GetFileNameWithoutAnyExtensions(string fileName)
        {
            var dirSepIndex = fileName.LastIndexOfAny(_dirSep);
            if (dirSepIndex >= 0)
            {
                var dotIndex = -1;
                bool doContinue = true;
                do
                {
                    dotIndex = fileName.LastIndexOf('.');
                    doContinue = dotIndex > dirSepIndex;
                    if (doContinue)
                        fileName = fileName.Substring(0, dotIndex);
                }
                while (doContinue);
            }
            else
            {
                var dotIndex = -1;
                bool doContinue = true;
                do
                {
                    dotIndex = fileName.LastIndexOf('.');
                    doContinue = dotIndex >= 0;
                    if (doContinue)
                        fileName = fileName.Substring(0, dotIndex);
                }
                while (doContinue);
            }

            return fileName;
        }

        static int IDFromFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return -1;
            var check = GetShortIngameFileName(fileName);
            while (!char.IsDigit(check[0]))
                check = check.Substring(1);
            return int.Parse(check);
        }

        [STAThread]
        static void Main(string[] args)
        {
            var exeFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            XmlFormatTemplate template = null;// XmlFormatTemplate.Read(exeFolder + "\\FXMLR_Template_FXR1.xml");

            //void WriteFXRXML(FXR1 fxr, string xmlPath)
            //{
            //    if (File.Exists(xmlPath))
            //        File.Delete(xmlPath);

            //    var test = new Polenter.Serialization.SharpSerializer();

            //    test.Serialize(fxr, xmlPath);
            //}

            //FXR1 ReadFXRXML(string xmlPath)
            //{
            //    var test = new XmlSerializer(typeof(FXR1));

            //    try
            //    {
            //        var fxr = (FXR1)test.Deserialize(xmlPath);

            //        return fxr;
            //    }
            //    catch (Exception exc)
            //    {
            //        Console.WriteLine($"Exception encountered while trying to repack XML into FFX:\n\n{exc.ToString()}"
            //            + $"\n\n\nPlease ensure that the value of the <{nameof(FXR1.FxrXmlFileVersion)}> element " +
            //            $"matches the value of this FXMLR build, which is: {FXR1.CURRENT_XML_FILE_VERSION}. Additionally, if the " +
            //            $"XML file does not have this element, it is a legacy file and will not work.");

            //        return null;
            //    }
            //}

            void WriteFXRXML(FXR1 fxr, string xmlPath)
            {
                //if (File.Exists(xmlPath))
                //    File.Delete(xmlPath);
                //using (var testStream = File.OpenWrite(xmlPath))
                //{
                //    var test = new XmlSerializer(typeof(FXR1));

                //    var xmlWriter = XmlWriter.Create(testStream, new XmlWriterSettings()
                //    {
                //        Indent = true,

                //    });

                //    test.Serialize(xmlWriter, fxr);
                //}

                fxr.WriteToXml(xmlPath);
            }

            FXR1 ReadFXRXML(string xmlPath)
            {
                //using (var testStream = File.OpenRead(xmlPath))
                //{
                //    var test = new XmlSerializer(typeof(FXR1));
                //    var xmlReader = XmlReader.Create(testStream);


                //    var fxr = (FXR1)test.Deserialize(xmlReader);

                //    return fxr;

                //    //try
                //    //{
                //    //    var fxr = (FXR1)test.Deserialize(xmlReader);

                //    //    return fxr;
                //    //}
                //    //catch (Exception exc)
                //    //{
                //    //    Console.WriteLine($"Exception encountered while trying to repack XML into FFX:\n\n{exc.ToString()}"
                //    //        + $"\n\n\nPlease ensure that the value of the <{nameof(FXR1.FxrXmlFileVersion)}> element " +
                //    //        $"matches the value of this FXMLR build, which is: {FXR1.CURRENT_XML_FILE_VERSION}. Additionally, if the " +
                //    //        $"XML file does not have this element, it is a legacy file and will not work.");

                //    //    return null;
                //    //}

                //}
                return FXR1.ReadFromXml(xmlPath);
            }

            void DOCMD_REPACKM99()
            {
                var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                string path_ffxbnd = Path.Combine(exeDir, $@"FRPG_SfxBnd_m99.ffxbnd.dcx");
                string path_assets_ffx = Path.Combine(exeDir, $@"m99_unpacked");
                string path_assets_flver = Path.Combine(exeDir, $@"m99_unpacked\Models");
                string path_assets_tpf = Path.Combine(exeDir, $@"m99_unpacked\Textures");

                if (args.Length >= 2)
                {
                    path_ffxbnd = Path.Combine(args[1], $@"FRPG_SfxBnd_m99.ffxbnd.dcx");
                    path_assets_ffx = Path.Combine(args[1], $@"m99_unpacked");
                    path_assets_flver = Path.Combine(args[1], $@"m99_unpacked\Models");
                    path_assets_tpf = Path.Combine(args[1], $@"m99_unpacked\Textures");
                }

                try
                {


                    var ffxbnd = BND3.Read(path_ffxbnd);
                    ffxbnd.Files.Clear();

                    var files_ffx = Directory.GetFiles(path_assets_ffx, "*.xml");
                    var files_flver = Directory.GetFiles(path_assets_flver, "*.flver");
                    var files_tpf = Directory.GetFiles(path_assets_tpf, "*.tpf");

                    int id = 0;

                    foreach (var f in files_ffx)
                    {
                        var fxr = ReadFXRXML(f);
                        fxr.Unflatten();

                        var sb = new StringBuilder(10);
                        sb.Append("f");
                        var shortFxrName = Path.GetFileNameWithoutExtension(f).ToLower();

                        for (int i = 1; i < shortFxrName.Length; i++)
                        {
                            if (char.IsDigit(shortFxrName[i]))
                                sb.Append(shortFxrName[i]);
                            else
                                break;
                        }


                        var fs = $@"N:\FRPG\data\Sfx\OutputData\Main\Effect_x64\{sb.ToString()}.ffx";
                        if (fs.ToUpper().EndsWith(".FFX.FFX"))
                        {
                            fs = fs.Substring(0, fs.Length - 4);
                        }
                        ffxbnd.Files.Add(new BinderFile(SoulsFormats.Binder.FileFlags.Flag1, id++, fs, fxr.Write()));
                    }

                    id = 100000;

                    foreach (var f in files_tpf)
                    {
                        var fs = $@"N:\FRPG\data\INTERROOT_x64\sfx\tex\{Path.GetFileNameWithoutExtension(f)}.tpf";
                        ffxbnd.Files.Add(new BinderFile(SoulsFormats.Binder.FileFlags.Flag1, id++, fs, File.ReadAllBytes(f)));
                    }

                    id = 200000;

                    foreach (var f in files_flver)
                    {
                        var fs = $@"N:\FRPG\data\INTERROOT_x64\sfx\model\{Path.GetFileNameWithoutExtension(f)}.flver";
                        ffxbnd.Files.Add(new BinderFile(SoulsFormats.Binder.FileFlags.Flag1, id++, fs, File.ReadAllBytes(f)));
                    }

                    ffxbnd.Write(path_ffxbnd);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            void DOCMD_FXMLR2()
            {
                void USAGE()
                {
                    Console.WriteLine("Usage: FXMLR2 < *.ffx | *.ffx.xml >");
                }
                if (args.Length < 1)
                {
                    USAGE();
                    return;
                }

                void doArg(string arg)
                {
                    string fileName = arg.ToLower();
                    if (fileName.EndsWith(".ffx.xml"))
                    {
                        var ffx = ReadFXRXML(fileName);
                        if (ffx == null)
                            // Error message already displayed.
                            return;
                        ffx.Unflatten();
                        ffx.Write(fileName.Substring(0, fileName.Length - 4));
                    }
                    else if (fileName.EndsWith(".ffx"))
                    {
                        //var ffx = FXR1.Read(fileName);
                        var ffx = FXR1.Read(fileName, template);
                        ffx.Flatten();
                        WriteFXRXML(ffx, fileName + ".xml");
                    }
                    else if (fileName == "repackm99")
                    {
                        DOCMD_REPACKM99();
                    }
                    else
                    {
                        USAGE();
                        return;
                    }
                }

                for (int i = 0; i < args.Length; i++)
                {
                    doArg(args[i]);
                }

               
            }

            FXR3 ReadFXR3XML(string xmlName)
            {
                using (var testStream = File.OpenRead(xmlName))
                {
                    var test = new XmlSerializer(typeof(FXR3));
                    var xmlReader = XmlReader.Create(testStream);


                    var fxr = (FXR3)test.Deserialize(xmlReader);

                    return fxr;

                    //try
                    //{
                    //    var fxr = (FXR1)test.Deserialize(xmlReader);

                    //    return fxr;
                    //}
                    //catch (Exception exc)
                    //{
                    //    Console.WriteLine($"Exception encountered while trying to repack XML into FFX:\n\n{exc.ToString()}"
                    //        + $"\n\n\nPlease ensure that the value of the <{nameof(FXR1.FxrXmlFileVersion)}> element " +
                    //        $"matches the value of this FXMLR build, which is: {FXR1.CURRENT_XML_FILE_VERSION}. Additionally, if the " +
                    //        $"XML file does not have this element, it is a legacy file and will not work.");

                    //    return null;
                    //}

                }
            }

            void WriteFXR3XML(FXR3 fxr, string xmlName)
            {
                if (File.Exists(xmlName))
                    File.Delete(xmlName);

                using (var testStream = File.OpenWrite(xmlName))
                {
                    using (var xmlWriter = XmlWriter.Create(testStream, new XmlWriterSettings()
                    {
                        Indent = true,
                    }))
                    {
                        var thing = new XmlSerializer(typeof(FXR3));
                        thing.Serialize(xmlWriter, fxr);
                    }
                }
            }

            void DOCMD_FXMLRDS3()
            {
                void USAGE()
                {
                    Console.WriteLine("Usage: FXMLRDS3 < *.fxr | *.fxr.xml >");
                }
                if (args.Length < 1)
                {
                    USAGE();
                    return;
                }

                void doArg(string arg)
                {
                    string fileName = arg.ToLower();
                    if (fileName.EndsWith(".fxr.xml"))
                    {
                        var ffx = ReadFXR3XML(fileName);
                        if (ffx == null)
                            // Error message already displayed.
                            return;
                        ffx.Write(fileName.Substring(0, fileName.Length - 4));
                    }
                    else if (fileName.EndsWith(".fxr"))
                    {
                        var ffx = FXR3.Read(fileName);
                        WriteFXR3XML(ffx, fileName + ".xml");
                    }
                    else if (fileName == "repackm99")
                    {
                        DOCMD_REPACKM99();
                    }
                    else
                    {
                        USAGE();
                        return;
                    }
                }

                for (int i = 0; i < args.Length; i++)
                {
                    doArg(args[i]);
                }


            }

           

            void TestNewXmlTemplateSystem()
            {
                //var fxr = FXR1.Read(@"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS REMASTERED\sfx\FRPG_SfxBnd_m99-ffxbnd-dcx\Sfx\OutputData\Main\Effect_x64\f1241100.ffx");
                var fxr = FXR1.Read(@"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS REMASTERED\sfx\FRPG_SfxBnd_m99-ffxbnd-dcx\Sfx\OutputData\Main\Effect_x64\f1241100.ffx", template);

                fxr.Write(@"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS REMASTERED\sfx\FRPG_SfxBnd_m99-ffxbnd-dcx\Sfx\OutputData\Main\Effect_x64\f1241100_testout.ffx");

                //fxr.Flatten();

                WriteFXRXML(fxr, @"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS REMASTERED\sfx\FRPG_SfxBnd_m99-ffxbnd-dcx\Sfx\OutputData\Main\Effect_x64\f1241100_testout.ffx.xml");

                fxr = ReadFXRXML(@"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS REMASTERED\sfx\FRPG_SfxBnd_m99-ffxbnd-dcx\Sfx\OutputData\Main\Effect_x64\f1241100_testout.ffx.xml");

                //fxr.Unflatten();

                //fxr.Flatten();

                WriteFXRXML(fxr, @"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS REMASTERED\sfx\FRPG_SfxBnd_m99-ffxbnd-dcx\Sfx\OutputData\Main\Effect_x64\f1241100_testout_resaved.ffx.xml");

                Console.WriteLine("hfgdghjfdchjfg");
            }

            void ds3_load_test()
            {
                

                void loadmeme(string fxrname)
                {
                    var fileName = $@"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS III\Game\sfx\frpg_sfxbnd_commoneffects_effect-ffxbnd-dcx\sfx\effect\{fxrname}.fxr";
                    var test = FXR3.Read(fileName);

                    if (File.Exists(fileName + ".xml"))
                        File.Delete(fileName + ".xml");

                    using (var testStream = File.OpenWrite(fileName + ".xml"))
                    {
                        using (var xmlWriter = XmlWriter.Create(testStream, new XmlWriterSettings()
                        {
                            Indent = true,
                        }))
                        {
                            var thing = new XmlSerializer(typeof(FXR3));
                            thing.Serialize(xmlWriter, test);
                        }
                    }
                }

                loadmeme("f000030000");
                loadmeme("f000221507");
                loadmeme("f001003001");
                loadmeme("f000030001");


                Console.WriteLine("fatcat");
            }

            //DOCMD_REPACKM99();
            //DOCMD_FXMLR2();

            //TestNewXmlTemplateSystem();
            //ds3_load_test();

            DOCMD_FXMLRDS3();
        }
    }
}
