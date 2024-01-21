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
                Debug.Log("������: �������� ������ ������.");
            }
        }
        catch (Exception ex)
        {
            LogProcessor.ProcessLog(FWL.GetClassName(), $"������ �������������� �������: {ex.Message}");
            Debug.Log($"������ �������������� �������: {ex.Message}");
        }
    }

    private static void HandleDeserializedObject(ServerResponseMessageWrapper serverResponseWrapper)
    {
        try
        {
            if (serverResponseWrapper != null)
            {
                Debug.Log($"�������������� ������ ������: {serverResponseWrapper.KeyType}, � ����������: {serverResponseWrapper.Message.Message}");
            }
            else
            {
                Debug.Log($"�������������� ������ ������: {serverResponseWrapper.KeyType}, � ����������: null");
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"������ �������������: {ex}");
        }
    }
}
