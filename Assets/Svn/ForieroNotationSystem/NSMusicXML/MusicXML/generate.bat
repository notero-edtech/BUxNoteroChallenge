xsd /c /f "schema_for_xsd_exe/musicxml.xsd" "schema_for_xsd_exe/xlink.xsd" "schema_for_xsd_exe/midixml.xsd" "schema_for_xsd_exe/xml.xsd" /o:"output" /n:ForieroEngine.Music.MusicXML.Xsd

cd output

rename "xml.cs" "musicxml_xlink_xml.cs"

cscript ..\replace.vbs "musicxml_xlink_xml.cs" "[System.ComponentModel.DesignerCategoryAttribute('code')]" ""
cd ..


csc /t:library /out:"output/musicxml_xlink_xml.dll" "output/musicxml_xlink_xml.cs"

sgen /a:"output/musicxml_xlink_xml.dll" /f 
del "output\musicxml_xlink_xml.cs"
 

xsd /c /f "schema_for_xsd_exe/container.xsd" /o:"output" /n:ForieroEngine.Music.MusicXML.Xsd
cd output

rename "container.cs" "musicxml_container.cs"

cscript ..\replace.vbs "musicxml_container.cs" "[System.ComponentModel.DesignerCategoryAttribute('code')]" ""
cd ..

csc /t:library /out:"output/musicxml_container.dll" "output/musicxml_container.cs"

sgen /a:"output/musicxml_container.dll" /f
del "output\musicxml_container.cs"
