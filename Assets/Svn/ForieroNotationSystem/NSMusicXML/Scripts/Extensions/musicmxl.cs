/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.IO;
using System.Text;
using ForieroEngine.Extensions.Zip;
using ForieroEngine.Music.MusicXML.Xsd;
using Ionic.Zip;
using UnityEngine;

namespace ForieroEngine.Music.MusicXML.MXL
{
    public static partial class MusicMXLExtensions
    {
        public const string MXL_META = "META-INF/container.xml";
        public const int ZIP_LEAD_BYTES = 0x04034b50;
        public const string MXL_META_XML = 
            "<?xml version=\"1.0\" encoding='UTF-8' standalone='no' ?> " +
            "<container>" +
                "<rootfiles>" +
                    "<rootfile full-path=\"[FILE_NAME]\" />"+
                "</rootfiles>"+
            "</container>";
        
        public static bool IsZip(this byte[] data)
        {
            if (data == null || data.Length < 4) return false;
            return (BitConverter.ToInt32(data, 0) == ZIP_LEAD_BYTES);
        }
        
        private static byte[] MXL_META_XML_BYTES(string fileName) => Encoding.UTF8.GetBytes(MXL_META_XML.Replace("[FILE_NAME]", fileName));
    
        public static byte[] Zip(this byte[] musicxmlBytes, string musicxmlFileName)
        {
            var xmlFileName = Path.ChangeExtension(musicxmlFileName, ".xml");
            var zipFile = new ZipFile();
            var containerxmlBytes = MXL_META_XML_BYTES(xmlFileName);
            zipFile.AddEntry("META-INF/container.xml", containerxmlBytes);
            zipFile.AddEntry(xmlFileName, musicxmlBytes);
            using var ms = new MemoryStream();
            zipFile.Save(ms);
            return ms.ToArray();
        }
       
        public static byte[] Unzip(this byte[] mxlBytes)
        {            
            var bytes = Array.Empty<byte>();

            var zipFile = ZipFile.Read(new MemoryStream(mxlBytes));

            if (zipFile.ContainsEntry(MXL_META))
            {
                var containerStream = new MemoryStream();

                zipFile[MXL_META].Extract(containerStream);
                containerStream.Position = 0;

                container container = null;
                container = container.Load(new MemoryStream(containerStream.GetAllBytes().RemoveZeros()));

                foreach (var rootFile in container.rootfiles)
                {
                    if (zipFile.ContainsEntry(rootFile.fullpath))
                    {
                        var xmlStream = new MemoryStream();
                        zipFile[rootFile.fullpath].Extract(xmlStream);
                        xmlStream.Position = 0;
                        bytes = xmlStream.GetAllBytes().RemoveZeros();
                        break;
                    }
                }
            }
            else { Debug.LogError("MXL META-INF not found"); }
            return bytes;
        }
    }
}
