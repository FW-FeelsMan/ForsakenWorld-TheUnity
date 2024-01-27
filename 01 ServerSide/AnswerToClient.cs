using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using System.Threading.Tasks;

public class AnswerToClient 
{
    private readonly Socket clientSocket;

    public AnswerToClient(Socket clientSocket)
    {
        this.clientSocket = clientSocket;
    }

    public async Task ServerResponseWrapper(string keyType, string message)
    {
        await ServerResponseMessageObject(keyType, message);
    }

    public async Task ServerResponseMessageObject(string keyType, string message)
    {
        var dataObject = new GlobalDataClasses.ServerResponseMessage
        {
            KeyType = keyType,
            Message = message
        };

        await RequestTypeAsync(keyType, dataObject);
    }

    private async Task RequestTypeAsync(string keyType, object dataObject)
    {
        try
        {
            byte[] requestData;
            var formatter = new BinaryFormatter();

            using (MemoryStream memoryStream = new())
            {
                formatter.Serialize(memoryStream, keyType);
                formatter.Serialize(memoryStream, dataObject);

                memoryStream.Position = 0;
                string debugKeyType = (string)formatter.Deserialize(memoryStream);
                object debugDataObject = formatter.Deserialize(memoryStream);

                await memoryStream.FlushAsync();
                requestData = memoryStream.ToArray();
            }

            await SendDataAsync(requestData);
        }
        catch (Exception ex)
        {
            
        }
    }

    private async Task SendDataAsync(byte[] data)
    {
        try
        {
            using NetworkStream networkStream = new(clientSocket);
            await networkStream.WriteAsync(data, 0, data.Length);
           
        }
        catch (Exception ex)
        {      
            CloseClientSocket();
        }
    }

    private void CloseClientSocket()
    {
        try
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
        catch (Exception closeEx)
        {
            
        }
    }

}
