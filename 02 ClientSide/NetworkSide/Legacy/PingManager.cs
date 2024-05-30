using System.Threading.Tasks;

public class PingManager : Singleton<PingManager>
{
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

    public async void SendPing()
    {
        GlobalDataClasses.RequestFromUser pingRequest = new GlobalDataClasses.RequestFromUser
        {
            GetPing = GlobalStrings.GetPingMessage
        };

        _ = RequestToServer.RequestTypeAsync(CommandKeys.GetPing, pingRequest);
    }

}
