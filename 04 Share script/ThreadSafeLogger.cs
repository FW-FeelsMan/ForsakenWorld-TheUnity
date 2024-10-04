using System;
using System.Collections.Generic;
using UnityEngine;

public static class ThreadSafeLogger
{
    private static readonly Queue<Action> logQueue = new();
    public static void Log(string message)
    {
        lock (logQueue)
        {
            logQueue.Enqueue(() => Debug.Log(message));
        }
    }
    public static void ProcessLogs()
    {
        lock (logQueue)
        {
            while (logQueue.Count > 0)
            {
                logQueue.Dequeue().Invoke();
            }
        }
    }
}
