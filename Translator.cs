using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Networking;
using System.Net;
using System;

public class Translator : MonoBehaviour
{
    string Translate(string word, string fromLanguage, string toLanguage)
    {
        string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={fromLanguage}&tl={toLanguage}&dt=t&q={UnityWebRequest.EscapeURL(word)}";
        WebClient webClient = new WebClient {Encoding = System.Text.Encoding.UTF8};
        string result = webClient.DownloadString(url);
        try
        {
            result = result.Substring(4, result.IndexOf("\"", 4, StringComparison.Ordinal) - 4);
            return result;
        }
        catch
        {
            return "Error";
        }
    }

    void Start()
    {
        Debug.Log(Translate("Probably, we live in a four-dimensional spacetime", "en", "pl"));
        Debug.Log(Translate("Probably, we live in a four-dimensional spacetime", "en", "de"));
        Debug.Log(Translate("Probably, we live in a four-dimensional spacetime", "en", "it"));
    }
}