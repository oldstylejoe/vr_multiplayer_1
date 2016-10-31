//be sure to add this to exactly one object (like the main camera) that will persist for the whole game
//  (make a blank object if you have to).
//Need to call the awake function.

// Edits made: Folder and File are now made automatically if they do not exist. 

using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using System;
using System.Collections;

public class Clock : MonoBehaviour
{

    [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi)]
    public static extern void GetSystemTimePreciseAsFileTime(out long filetime);
    public static string StrBuffer;
    public static Clock instance;

    private static string subjName;
    private static bool Writing = false;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogError("Error: Too many clocks.");
        }

        subjName = "default";

        // Create filePath if it does not exist
        (new FileInfo("c:/DataLogs/")).Directory.Create();

        if (File.Exists("c:/DataLogs/subject_id.txt"))
        {
            foreach (string l in File.ReadAllLines("c:/DataLogs/subject_id.txt"))
            {
                if (l.Length > 1)
                {
                    subjName = l;
                }
            }
        }
        else
        {
            File.CreateText("c:/DataLogs/subject_id.txt");
        }
    }

    public static IEnumerator write(string str)
    {
        StrBuffer += str;

        if (Writing)
            yield break;

        Writing = true;

        while (sizeof(char) * StrBuffer.Length < 10000)
        {
            yield return null;
        }

        var t = DateTime.Now;

        // Create filePath if it does not exist
        (new FileInfo("c:/DataLogs/vive_multiplayer_1/")).Directory.Create();

        string fname = "c:/DataLogs/vive_multiplayer_1/multiplayer_log_";
        fname += subjName + "_";
        fname += t.Day.ToString() + "_";
        fname += t.Month.ToString() + "_";
        fname += t.Year.ToString() + "_";
        fname += t.Hour.ToString() + ".txt";

        System.IO.StreamWriter file = new System.IO.StreamWriter(fname, true);
        file.WriteLine(StrBuffer);

        file.Close();

        StrBuffer = "";
        Writing = false;
    }

    //automate the time stamping. Slight loss of precision is possible (but unlikely).
    public static void markEvent(string str)
    {
        long fTest;
        GetSystemTimePreciseAsFileTime(out fTest);
        System.DateTime dt = new System.DateTime(1601, 01, 01).AddTicks(fTest);
        dt = dt.ToLocalTime();
        instance.StartCoroutine(write(str + " " + subjName + " " + dt.ToString("yyyy-MM-dd HH:mm:ss.ffffff") + " " + fTest.ToString() + Environment.NewLine));
    }
}