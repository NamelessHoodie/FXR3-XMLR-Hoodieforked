using SoulsFormats;
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
        [STAThread]
        static void Main(string[] args)
        {
            FXR3 ReadFXR3XML(string xmlName)
            {
                using (var testStream = File.OpenRead(xmlName))
                {
                    var test = new XmlSerializer(typeof(FXR3));
                    var xmlReader = XmlReader.Create(testStream);


                    var fxr = (FXR3)test.Deserialize(xmlReader);

                    return fxr;
                }
            }

            void WriteFXR3XML(FXR3 fxr, string xmlName)
            {
                if (File.Exists(xmlName))
                    File.Delete(xmlName);

                using (var testStream = File.OpenWrite(xmlName))
                {
                    using (var xmlWriter = XmlWriter.Create(testStream, new XmlWriterSettings(){Indent = true,}))
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
            DOCMD_FXMLRDS3();
        }
    }
}
