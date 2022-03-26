using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CSVParser : MonoBehaviour
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static Dictionary<string, Dictionary<string, string>> Read(string file)
    {
        //Debug.Log(file);
        var dictionary = new Dictionary<string, Dictionary<string, string>>();
        TextAsset data = Resources.Load(file) as TextAsset;
        if (data == null)
            Debug.Log("NULL");
        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        //Debug.Log("lines \n" + lines);
        //foreach(var tmp in lines)
        //{
        //    Debug.Log(tmp);
        //}

        var headers = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            var column = values[0];
            var finalValues = new Dictionary<string, string>();
            if (values.Length == 0 || values[0] == "") continue;
            for (var j = 1; j < headers.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                finalValues.Add(headers[j], value);
            }
            dictionary.Add(column, finalValues);
        }
        return dictionary;
    }
}
