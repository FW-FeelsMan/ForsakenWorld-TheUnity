using UnityEngine;
using System.IO;
using ForsakenWorld;

public class ConnectionDataLoader : MonoBehaviour
{
    public string serverIpAddress;
    public int serverPort;
    private const string ConfigFilePath = "config.ini";
    private const string ErrorMessageFileNotFound = "���� config.ini �� ������";
    public void LoadConnectionData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, ConfigFilePath);
        if (File.Exists(filePath))
        {
            using StreamReader reader = new(filePath);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("IP"))
                {
                    serverIpAddress = line.Split('=')[1].Trim();
                }
                else if (line.StartsWith("Port"))
                {
                    serverPort = int.Parse(line.Split('=')[1].Trim());
                }
            }
        }
        else
        {
            LogProcessor.ProcessLog(FWL.GetClassName(), ErrorMessageFileNotFound);
        }
    }
}
