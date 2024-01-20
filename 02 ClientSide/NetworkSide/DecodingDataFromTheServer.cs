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
            LogProcessor.ProcessLog(FWL.GetClassName(), $"Ошибка десериализации объекта: {ex.Message}");
            Debug.Log($"Ошибка десериализации объекта: {ex.Message}");
        }
    }

    private void HandleDeserializedObject(object deserializedObject)
    {
        try
        {
            if (deserializedObject is GlobalDataClasses.ServerResponseMessage serverResponseMessage)
            {
                Debug.Log($"Десериализован объект ключом: {serverResponseMessage.KeyType}, и сообщением: {serverResponseMessage.Message}");
            }
            else
            {
                Debug.Log($"Десериализован объект неизвестного типа: {deserializedObject.GetType().Name}");
            }
        }catch(Exception ex){
            Debug.Log($"Ошибка декодирования: {ex}");
        }
    }
}
