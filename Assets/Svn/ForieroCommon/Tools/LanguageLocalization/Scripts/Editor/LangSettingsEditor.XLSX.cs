using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelDataReader;
using ForieroEditor.Coroutines;
using ForieroEditor.Extensions;
using ForieroEngine.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public partial class LangSettingsEditor : Editor
{
    void DownloadXlsx(LangSettings.GoogleDoc googleDoc)
    {
        Debug.Log("Downloading : " + googleDoc.xlsxURL);

        EditorCoroutineStart.StartCoroutine(GetAndGetBytes(googleDoc.xlsxURL));

        IEnumerator GetAndGetBytes(string url)
        {
            using(var www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();

                if(www.result.HasError())
                {
                    Debug.LogError(www.error);
                    yield break;
                }

                string directory = Path.GetDirectoryName(googleDoc.xlsxAssetsPath.GetFullPathFromAssetPath());

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllBytes(googleDoc.xlsxAssetsPath.GetFullPathFromAssetPath(), www.downloadHandler.data);

                XLSXToDictionary(googleDoc);
                AssetDatabase.Refresh();
                updateDictionaries = true;
            }
        }
    }

    void XLSXToDictionary(LangSettings.GoogleDoc googleDoc)
    {
        using (var stream = File.Open(googleDoc.xlsxAssetsPath.GetFullPathFromAssetPath(), FileMode.Open, FileAccess.Read))
        {
            // Auto-detect format, supports:
            //  - Binary Excel files (2.0-2003 format; *.xls)
            //  - OpenXml Excel files (2007 format; *.xlsx)
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var dataSet = reader.AsDataSet();

                for (int t = 0; t < dataSet.Tables.Count; t++)
                {
                    var table = dataSet.Tables[t];
                    DataRow idsRow = null;

                    string result = "";

                    bool isDictionary = false;
                    int rowIndex = -1;

                    isDictionary = table.Rows.Count >= 1 && table.Columns.Count > 0 && !string.IsNullOrEmpty(table.Rows[0][0].GetContent()) && table.Rows[0][0].GetContent().ToUpper() == "ID";

                    if (!isDictionary)
                    {
                        Debug.LogError("MISSING ID COLUMN : " + googleDoc.xlsxAssetsPath.GetFullPathFromAssetPath());
                        continue;
                    }

                    for (int r = 0; r < table.Rows.Count; r++)
                    {
                        DataRow row = table.Rows[r];

                        if (r == 0) idsRow = row;

                        string rowResult = "";
                        rowIndex++;

                        bool appendRow = true;

                        string actor = "";
                        string append = "";
                        for (int c = 0; c < table.Columns.Count; c++)
                        {
                            append = "";

                            if (c == 0 && string.IsNullOrEmpty(row[c].GetContent()))
                            {
                                appendRow = false;
                                break;
                            }

                            string columnName = idsRow[c].GetContent().Replace(@"\n", "").Trim();

                            if (string.IsNullOrEmpty(columnName))
                            {
                                continue;
                            }

                            if (columnName == "ACTOR" && rowIndex > 0)
                            {
                                append = (row[c].GetContent());

                                if (!string.IsNullOrEmpty(append))
                                {
                                    actor = "[" + append + "]";
                                }
                                continue;
                            }
                            else
                            {
                                if (!Enum.TryParse<Lang.LanguageCode>(columnName, out var _))
                                {
                                    Debug.LogWarning("Ignoring column : " + columnName);
                                    continue;
                                }
                            }

                            append = c < table.Columns.Count ? row[c].GetContent() : "";

                            if (!string.IsNullOrEmpty(append))
                            {
                                if (!string.IsNullOrEmpty(actor))
                                {
                                    append = actor + append;
                                }
                            }

                            rowResult += (string.IsNullOrEmpty(rowResult) ? "" : ";") + (r == 0 ? append.Replace(@"\n", "").Trim() : append);
                        }

                        if (appendRow)
                        {
                            if (string.IsNullOrEmpty(result))
                            {
                                result = rowResult;
                            }
                            else
                            {
                                result = result + "\n" + rowResult;
                            }
                        }
                    }

                    string fileName = "";

                    if (googleDoc.prependFileName)
                    {
                        fileName = "Assets/Resources Localization/" + guid.stringValue + "/Dictionaries/" + googleDoc.fileName + "_" + table.TableName + ".txt";
                    }
                    else
                    {
                        fileName = "Assets/Resources Localization/" + guid.stringValue + "/Dictionaries/" + table.TableName + ".txt";
                    }

                    fileName = fileName.GetFullPathFromAssetPath();
                    string directory = Path.GetDirectoryName(fileName);

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    File.WriteAllText(fileName, result, System.Text.Encoding.UTF8);
                    Debug.Log("Saved dictionary : " + fileName);
                }
            }
        }
    }

    void XSLXUpdateDictionaries()
    {
        Dictionary<string, UpdateDictionaryItem> updateItems = new Dictionary<string, UpdateDictionaryItem>();

        foreach (var dictionary in this.o.dictionaries)
        {
            updateItems.Add(dictionary.aliasName, new UpdateDictionaryItem() { stored = dictionary.stored });
        }

        dictionaries.ClearArray();
        for (int i = 0; i < googleDocs.arraySize; i++)
        {
            LangSettings.GoogleDoc googleDoc = o.googleDocs[i];

            string fileName = googleDoc.xlsxAssetsPath.GetFullPathFromAssetPath();

            if (!System.IO.File.Exists(fileName))
            {
                Debug.LogError("FILE NOT EXISTS : " + fileName);
                break;
            }
           
            using (var stream = File.Open(googleDoc.xlsxAssetsPath.GetFullPathFromAssetPath(), FileMode.Open, FileAccess.Read))
            {

                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var dataSet = reader.AsDataSet();

                    for (int t = 0; t < dataSet.Tables.Count; t++)
                    {
                        var table = dataSet.Tables[t];
                        bool isDictionary = table.Rows.Count > 1 && table.Columns.Count > 0 && !string.IsNullOrEmpty(table.Rows[0][0].GetContent()) && table.Rows[0][0].GetContent().ToUpper() == "ID";

                        if (isDictionary)
                        {
                            dictionaries.InsertArrayElementAtIndex(0);
                            SerializedProperty dictionary = dictionaries.GetArrayElementAtIndex(0);
                            string aliasName = "";
                            if (googleDoc.prependFileName)
                            {
                                aliasName = Path.GetFileNameWithoutExtension(googleDoc.fileName + "_" + table.TableName);
                            }
                            else
                            {
                                aliasName = table.TableName;
                            }

                            dictionary.FindPropertyRelative("aliasName").stringValue = aliasName;

                            string assetPath = "Assets/Resources Localization/" + guid.stringValue + "/Dictionaries/" + aliasName + ".txt";
                            //string assetPathGuid = AssetDatabase.AssetPathToGUID(assetPath);
                            //Debug.Log(assetPath);
                            //Debug.Log(assetPathGuid);

                            dictionary.FindPropertyRelative("textAsset").objectReferenceValue = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath) as UnityEngine.Object;

                            if (updateItems.ContainsKey(aliasName))
                            {
                                dictionary.FindPropertyRelative("stored").enumValueIndex = (int)updateItems[aliasName].stored;
                            }
                        }
                    }
                }
            }
        }
    }
}
