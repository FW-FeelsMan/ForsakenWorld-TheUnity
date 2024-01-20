using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ForsakenWorld;
using UnityEngine;

public class DecodingDataFromServer : MonoBehaviour
{
    public void DeserializeAndHandleObject(byte[] data)
    {
        try
        {
            using MemoryStream memoryStream = new(data);
            var formatter = new BinaryFormatter();

            var dataServer = formatter.Deserialize(memoryStream);

            HandleDeserializedObject(dataServer);
        }
        catch (Exception ex)
        {
            LogProcessor.ProcessLog(FWL.GetClassName(), $"������ �������������� �������: {ex.Message}");
            Debug.Log($"������ �������������� �������: {ex.Message}");
        }
    }

    private void HandleDeserializedObject(object deserializedObject)
    {
        try
        {
            if (deserializedObject is GlobalDataClasses.ServerResponseMessage serverResponseMessage)
            {
                Debug.Log($"�������������� ������ ������: {serverResponseMessage.KeyType}, � ����������: {serverResponseMessage.Message}");
            }
            else
            {
                Debug.Log($"�������������� ������ ������������ ����: {deserializedObject.GetType().Name}");
            }
        }catch(Exception ex){
            Debug.Log($"������ �������������: {ex}");
        }
    }
}
