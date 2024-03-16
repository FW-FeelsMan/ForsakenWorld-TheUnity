using UnityEngine;

public class GlobalSettings 
{
    public static int ServerResponseTimeout = 50;
    public static int PingIntervalMilliseconds = 1000;
    public static void SetServerResponseTimeout(int value)
    {
        if (value >= 1 && value <= 5)
        {
            ServerResponseTimeout = value;
        }
        else
        {
            Debug.LogWarning("Попытка установки недопустимого значения для времени ожидания ответа от сервера.");
        }
    }
}
