using SoulsFormats;
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace FXR3_XMLR
{
    public class FXR3XMLRMain
    {
        [STAThread]
        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
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
                    XDocument XMLFXR3 = XDocument.Load(fileName);
                    if (XMLFXR3 == null)
                        return;
                    FXR3Enhanced ffx = FXR3EnhancedSerialization.XMLToFXR3(XMLFXR3);
                    if (ffx == null)
                        return;
                    ffx.Write(fileName.Substring(0, fileName.Length - 4));
                }
                else if (fileName.EndsWith(".fxr"))
                {
                    FXR3Enhanced ffx = FXR3Enhanced.Read(fileName);
                    if (ffx == null)
                        return;
                    FXR3EnhancedSerialization.FXR3ToXML(ffx).Save(fileName + ".xml");
                }
                else
                {
                    USAGE();
                    return;
                }
            }

            foreach (string path in args)
            {
                doArg(path);
            }
        }
    }
}