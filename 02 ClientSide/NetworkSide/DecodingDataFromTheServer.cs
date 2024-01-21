using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ForsakenWorld;
using UnityEngine;

[Serializable]
public class ServerResponseMessageWrapper
{
    public string KeyType { get; set; }
    public GlobalDataClasses.ServerResponseMessage Message { get; set; }
}

public class DecodingDataFromServer : MonoBehaviour
{
    public static void DeserializeAndHandleObject(byte[] data)
    {
        try
        {
            using MemoryStream memoryStream = new(data);
            var formatter = new BinaryFormatter();
            var serverResponseWrapper = formatter.Deserialize(memoryStream) as ServerResponseMessageWrapper;

            if (serverResponseWrapper != null)
            {
                HandleDeserializedObject(serverResponseWrapper);
            }
            else
            {
                Debug.Log("Ошибка: Неверный формат данных.");
            }
        }
        catch (Exception ex)
        {
            LogProcessor.ProcessLog(FWL.GetClassName(), $"Ошибка десериализации объекта: {ex.Message}");
            Debug.Log($"Ошибка десериализации объекта: {ex.Message}");
        }
    }

    private static void HandleDeserializedObject(ServerResponseMessageWrapper serverResponseWrapper)
    {
        try
        {
            if (serverResponseWrapper != null)
            {
                Debug.Log($"Десериализован объект ключом: {serverResponseWrapper.KeyType}, и сообщением: {serverResponseWrapper.Message.Message}");
            }
            else
            {
                Debug.Log($"Десериализован объект ключом: {serverResponseWrapper.KeyType}, и сообщением: null");
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Ошибка декодирования: {ex}");
        }
    }
}
