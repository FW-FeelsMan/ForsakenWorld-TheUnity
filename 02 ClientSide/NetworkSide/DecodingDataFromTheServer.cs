using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using ForsakenWorld;
using UnityEngine;

public class DecodingDataFromServer : MonoBehaviour
{

    private readonly Dictionary<string, Action<object>> handlers = new();
    private readonly Queue<Action> mainThreadQueue = new Queue<Action>();

    public DecodingDataFromServer()
    {
        PacketHandlers();
    }

    private void PacketHandlers()
    {
        RegisterResponse(CommandKeys.SuccessfulLogin, ResponseProcessing);
        RegisterResponse(CommandKeys.FailedLogin, ResponseProcessing);

        RegisterResponse(CommandKeys.SuccessfulRegistration, ResponseProcessing);
        RegisterResponse(CommandKeys.FailedRegistration, ResponseProcessing);

    }

    private void RegisterResponse(string keyType, Action<object> handler)
    {
        handlers[keyType] = handler;
    }

    private void ResponseProcessing(object dataObject)
    {
        if (dataObject is GlobalDataClasses.ServerResponseMessage responseData)
        {
            string _key = responseData.KeyType;
            string _message = responseData.Message;

            Debug.Log($"��������: \n���� {_key}; \n���������: {_message}");

            switch (_key)
            {
                case CommandKeys.SuccessfulLogin:
                    mainThreadQueue.Enqueue(() => UIManager.instance.DisplayAnswer(2, _message));
                    break;
                case CommandKeys.FailedLogin:
                    mainThreadQueue.Enqueue(() => UIManager.instance.DisplayAnswer(0, _message));
                    break;

                case CommandKeys.SuccessfulRegistration:
                    mainThreadQueue.Enqueue(() => UIManager.instance.DisplayAnswer(2, _message));
                    break;
                case CommandKeys.FailedRegistration:
                    mainThreadQueue.Enqueue(() => UIManager.instance.DisplayAnswer(0, _message));
                    break;
            }
        }
    }

    private void Update()
    {
        while (mainThreadQueue.Count > 0)
        {
            mainThreadQueue.Dequeue().Invoke();
        }
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
                        LogProcessor.ProcessLog(FWL.GetClassName(), $"�� ������ ���������� ��� ���������� ����: {keyType}");
                        Debug.Log($"�� ������ ���������� ��� ���������� ����: {keyType}");
                    }
                }
                catch (Exception ex)
                {
                    LogProcessor.ProcessLog(FWL.GetClassName(), $"������ �������������� �������: {ex.Message}");
                    Debug.Log($"������ �������������� �������: {ex.Message}");
                }
            });
        }
        catch (Exception ex)
        {
            LogProcessor.ProcessLog(FWL.GetClassName(), $"������ ��������� ����������� ������: {ex.Message}");
            Debug.Log($"������ ��������� ����������� ������: {ex.Message}");
        }
    }
}
