using UnityEngine;

public class LogProcessor : MonoBehaviour
{
    void Update()
    {
        ThreadSafeLogger.ProcessLogs();        
    }
}

