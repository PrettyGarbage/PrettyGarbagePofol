using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class CSVReader
{
    //protected readonly string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    private readonly string SPLIT_RE = @":";
    //protected readonly string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    protected readonly string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    protected readonly char[] TRIM_CHARS = { '\"' };

    public List<string[]> ReadCSV(string folderPath, string fileName)
    {
        if (!File.Exists(folderPath + fileName))
            return null;

        var textAsset = new TextAsset(File.ReadAllText(folderPath + fileName));
        var tableData = Regex.Split(textAsset.text, LINE_SPLIT_RE);
        var header = Regex.Split(tableData[0], SPLIT_RE);

        for (int headerIndex = 0; headerIndex < header.Length; headerIndex++)
        {
            header[headerIndex] = header[headerIndex].Trim();
        }

        List<string[]> tableValue = new List<string[]>();
        for (var csvDataIndex = 1; csvDataIndex < tableData.Length; csvDataIndex++)
        {
            var values = Regex.Split(tableData[csvDataIndex], SPLIT_RE);
            if (values.Length == 0 || values[0].Equals(string.Empty)) continue;

            string[] valueStr = new string[header.Length];

            for (var tableIndex = 0; tableIndex < header.Length && tableIndex < values.Length; tableIndex++)
            {
                valueStr[tableIndex] = values[tableIndex].TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
            }
            tableValue.Add(valueStr);
        }
        return tableValue;
    }
}
