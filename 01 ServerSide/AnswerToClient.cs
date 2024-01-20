using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using ForsakenWorld;
using UnityEngine;

public class AnswerToClient : MonoBehaviour
{
    public async Task ServerResponseWrapper(string _keyType, string _message)
    {
        await ServerResponseMessageObject(_keyType, _message);
    }

    public static async Task<byte[]> ServerResponseMessageObject(string _keyType, string _message)
    {
        var _dataObject = new GlobalDataClasses.ServerResponseMessage
        {
            KeyType = _keyType,
            Message = _message
        };
        Debug.Log($"Данные для отправки: {_keyType}, {_message}");
        return await RequestTypeAsync(_keyType, _dataObject);
    }
    public static async Task<byte[]> RequestTypeAsync(string keyType, object dataObject)
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

            Debug.Log($"Передаю закодированные данные для отправки. \n Данные: {BitConverter.ToString(requestData)}");
            await SocketServer.Instance.SendDataAsync(requestData);
            

            return requestData;
        }
        catch (Exception ex)
        {                   
            LogProcessor.ProcessLog(FWL.GetClassName(), ex);
            return null;
        }
    }
}
