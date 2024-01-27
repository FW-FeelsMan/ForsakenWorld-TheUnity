using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using ForsakenWorld;
using UnityEngine;

public class DecodingDataFromServer : MonoBehaviour
{
    private readonly Action<object> defaultHandler = (obj) => Debug.Log($"Unhandled object: {obj}");

    private readonly Dictionary<string, Action<object>> handlers = new();

    public DecodingDataFromServer()
    {
        PacketHandlers();
    }

    private void PacketHandlers()
    {
        RegisterResponse(CommandKeys.LoginRequest, HandleLoginResponse);
        RegisterResponse(CommandKeys.RegistrationRequest, HandleRegistrationResponse);
    }

    private void RegisterResponse(string keyType, Action<object> handler)
    {
        handlers[keyType] = handler;
    }

    private void HandleLoginResponse(object dataObject)
    {
        if (dataObject is GlobalDataClasses.ServerResponseMessage responseData)
        {
            string _key = responseData.KeyType;
            string _message = responseData.Message;

            Debug.Log($"Получено: \n Ключ {_key}; \n Сообщение: {_message}");
        }
    }
    private void HandleRegistrationResponse(object dataObject)
    {

    }

    public async Task ProcessPacketAsync(byte[] packet)
    {
        try
        {
            await Task.Run(() =>
            {
                using MemoryStream memoryStream = new(packet);
                var formatter = new BinaryFormatter();

                try
                {
                    string keyType = (string)formatter.Deserialize(memoryStream);

                    if (handlers.TryGetValue(keyType, out var handler))
                    {
                        object dataObject = formatter.Deserialize(memoryStream);
                        handler(dataObject);
                    }
                    else
                    {
                        LogProcessor.ProcessLog(FWL.GetClassName(), $"Не найден обработчик под полученный ключ: {keyType}");
                        Debug.Log($"Не найден обработчик под полученный ключ: {keyType}");
                    }
                }
                catch (Exception ex)
                {
                    LogProcessor.ProcessLog(FWL.GetClassName(), $"Ошибка десериализации объекта: {ex.Message}");
                    Debug.Log($"Ошибка десериализации объекта: {ex.Message}");
                }
            });
        }
        catch (Exception ex)
        {
            LogProcessor.ProcessLog(FWL.GetClassName(), $"Ошибка обработки полученного пакета: {ex.Message}");
            Debug.Log($"Ошибка обработки полученного пакета: {ex.Message}");
        }
    }
}
