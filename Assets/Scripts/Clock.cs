//be sure to add this to exactly one object (like the main camera) that will persist for the whole game
//  (make a blank object if you have to).
//Need to call the awake function.

using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using System;

public class Clock : MonoBehaviour
{

    [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi)]
    public static extern void GetSystemTimePreciseAsFileTime(out long filetime);

    private static string subjName;
    void Awake()
    {
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

    public static void write(string str)
    {
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
        file.WriteLine(str);

        file.Close();
    }

    //automate the time stamping. Slight loss of precision is possible (but unlikely).
    public static void markEvent(string str)
    {
        long fTest;
        GetSystemTimePreciseAsFileTime(out fTest);
        System.DateTime dt = new System.DateTime(1601, 01, 01).AddTicks(fTest);
        dt = dt.ToLocalTime();
        write(str + " " + subjName + " " + dt.ToString("yyyy-MM-dd HH:mm:ss.ffffff") + " " + fTest.ToString());
    }

}