//Joe Snider
//10/16
//
//Datalogging for unity with threading. Pretty hacked.
//Requires write access to a c:\DataLogs directory.

using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections;
using System.IO;

public class Job : Threader
{
    public string inData;
    public string subjName = "asdf";

    protected override void ThreadFunction()
    {
        var t = System.DateTime.Now;
        string fname = "c:/DataLogs/log_";
        fname += subjName + "_";
        fname += t.Day.ToString() + "_";
        fname += t.Month.ToString() + "_";
        fname += t.Year.ToString() + "_";
        fname += t.Hour.ToString() + ".txt";

        System.IO.StreamWriter file = new System.IO.StreamWriter(fname, true);
        file.Write(inData);

        file.Close();
    }
}

public class LogHandler : MonoBehaviour {

    [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi)]
    public static extern void GetSystemTimePreciseAsFileTime(out long filetime);

    Job myJob;
    private static string data = "";
    const int RECORDING_FILE_BUFFER_SIZE = 1000;
    private static string subjName = "subject0";

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


    // Use this for initialization
    void Start () {
        myJob = new Job();
    }
	
	// Update is called once per frame
	void Update () {
        if (data.Length > RECORDING_FILE_BUFFER_SIZE && myJob.Available())
        {
            myJob.inData = data;
            data = "";
            myJob.Start();
        }
    }

    public static void write(string str)
    {
        data += str;
    }

    public static void markEvent(string str)
    {
        long fTest;
        GetSystemTimePreciseAsFileTime(out fTest);
        System.DateTime dt = new System.DateTime(1601, 01, 01).AddTicks(fTest);
        dt = dt.ToLocalTime();
        write(str + " " + subjName + " " + dt.ToString("yyyy-MM-dd HH:mm:ss.ffffff") + " " + fTest.ToString() + "\n" + System.Environment.NewLine);
    }
}
