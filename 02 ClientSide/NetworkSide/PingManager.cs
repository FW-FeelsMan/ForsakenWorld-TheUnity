using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingManager : MonoBehaviour
{
    private string serverIpAddress;
    private List<int> pingTimes = new();
    
    public void Initialize(string ipAddress)
    {
        serverIpAddress = ipAddress;
        StartCoroutine(PingUpdate());
    }

    IEnumerator PingUpdate()
    {
        while (true)
        {
            Ping ping = new(serverIpAddress);
            yield return new WaitForSeconds(1f);

            while (!ping.isDone)
            {
                yield return null;
            }

            pingTimes.Add(ping.time);
        }
    }
}
