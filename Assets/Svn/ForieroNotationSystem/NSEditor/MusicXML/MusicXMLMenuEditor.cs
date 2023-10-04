using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ForieroEditor.CommandLine;
using ForieroEditor.Extensions;
using MoreLinq;
using UnityEditor;
using File = UnityEngine.Windows.File;

public static class MusicXMLMenuEditor
{
    [MenuItem("Foriero/NS/MusicXML/Update Schemas")]
    public static void UpdateMusicXMLSchema()
    {
        var url = "https://github.com/w3c/musicxml/trunk/schema";
        var dir = Directory.GetCurrentDirectory() + "/Assets";
        var d = Path.GetDirectoryName(Directory.GetFiles(dir, "to31.xsl", SearchOption.AllDirectories)[0]);
        var d_xsd = d.Replace("/schema", "/schema_for_xsd_exe/");
        GitHub.GetRepositoryFiles(url, d.GetAssetPathFromFullPath() + "/", (s) =>
        {
            var modFiles = Directory.GetFiles(d, "*.mod*");
            modFiles.ForEach(File.Delete);
            AssetDatabase.Refresh();
        });
    }
    
    [MenuItem("Foriero/NS/MusicXML/Prepare XSD")]
    public static void PrepareMusicXMLXSD()
    {
        var dir = Directory.GetCurrentDirectory() + "/Assets";
        var d = Path.GetDirectoryName(Directory.GetFiles(dir, "to31.xsl", SearchOption.AllDirectories)[0]);
        var d_xsd = d.Replace("/schema", "/schema_for_xsd_exe/");

        ReplaceAttributeGroupRef(Path.Combine(d, "musicxml.xsd"), Path.Combine(d_xsd, "musicxml.xsd"));
        ReplaceAttributeGroupRef(Path.Combine(d, "xlink.xsd"), Path.Combine(d_xsd, "xlink.xsd"));
        ReplaceAttributeGroupRef(Path.Combine(d, "midixml.xsd"), Path.Combine(d_xsd, "midixml.xsd"));
        ReplaceAttributeGroupRef(Path.Combine(d, "MIDIEvents10.xsd"), Path.Combine(d_xsd, "MIDIEvents10.xsd"));
        ReplaceAttributeGroupRef(Path.Combine(d, "xml.xsd"), Path.Combine(d_xsd, "xml.xsd"));
        ReplaceAttributeGroupRef(Path.Combine(d, "container.xsd"), Path.Combine(d_xsd, "container.xsd"));
        AssetDatabase.Refresh();
    }
   
    public static void ReplaceAttributeGroupRef(string inputFile, string outputFile)
    {
        XDocument wsdl = XDocument.Load(inputFile);

        IEnumerable<XElement> attributeGroupDefs =
            wsdl.Root.Descendants("{http://www.w3.org/2001/XMLSchema}attributeGroup")
                .Where(w => w.Attribute("name") != null)
                .Select(x => x);

        foreach (
            XElement r in
            wsdl.Root.Descendants("{http://www.w3.org/2001/XMLSchema}attributeGroup")
                .Where(w => w.Attribute("ref") != null))
        {
            string refValue = r.Attribute("ref").Value;

            foreach (XElement d in attributeGroupDefs)
            {
                string defValue = d.Attribute("name").Value;
                if (refValue == defValue)
                {
                    IEnumerable<XElement> s =
                        d.Elements("{http://www.w3.org/2001/XMLSchema}attribute").Select(x => x);
                    foreach (XElement e in s)
                    {
                        r.AddBeforeSelf(e);
                    }
                    break;
                }
            }
        }

        wsdl.Root.Descendants("{http://www.w3.org/2001/XMLSchema}attributeGroup")
            .Where(w => w.Attribute("ref") != null)
            .Remove();
        wsdl.Save(outputFile);
    }
}
