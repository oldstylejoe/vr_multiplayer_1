//Joe Snider
//10/16
//
//Thread interaction object. Taken from
//   http://answers.unity3d.com/questions/357033/unity3d-and-c-coroutines-vs-threading.html

using System.Collections;
using UnityEngine;

public class Threader
{
    private bool m_IsDone = true;
    private object m_Handle = new object();
    private System.Threading.Thread m_Thread = null;
    public bool IsDone
    {
        get
        {
            bool tmp;
            lock (m_Handle)
            {
                tmp = m_IsDone;
            }
            return tmp;
        }
        set
        {
            lock (m_Handle)
            {
                m_IsDone = value;
            }
        }
    }

    public virtual void Start()
    {
        m_Thread = new System.Threading.Thread(Run);
        m_Thread.Start();
    }
    public virtual void Abort()
    {
        m_Thread.Abort();
    }

    protected virtual void ThreadFunction() { }

    protected virtual void OnFinished() { }

    //check this before attempting to start a job
    public virtual bool Available()
    {
        return IsDone;
    }

    private void Run()
    {
        IsDone = false;
        ThreadFunction();
        IsDone = true;
    }
}
