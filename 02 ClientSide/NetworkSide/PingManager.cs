using System;
using System.Threading.Tasks;
using UnityEngine;

public class PingManager : Singleton<PingManager>
{
    //public static PingManager instance;

    private void Awake()
    {
        instance = this;
    }

    public async void StartSendingPings()
    {
        while (true)
        {
            SendPing();
            await Task.Delay(GlobalSettings.PingIntervalMilliseconds);
        }
    }

    private void SendPing()
    {
        _ = RequestToServer.RequestTypeAsync(CommandKeys.GetPing, GlobalStrings.GetPingMessage);

        DateTime pingSentTime = DateTime.Now;
        //Debug.Log(pingSentTime);
        
    }
}
