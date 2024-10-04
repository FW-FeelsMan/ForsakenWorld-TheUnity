
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
    private readonly Queue<Action> mainThreadQueue = new();

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
            string key = responseData.KeyType;
            string message = responseData.Message;

            

            switch (key)
            {
                case CommandKeys.SuccessfulLogin:
                    
                    ThreadSafeLogger.Log($"Добро пожаловать");
                    break;
                case CommandKeys.FailedLogin:
                    EnqueueMainThreadAction(() => UIManager.instance.DisplayAnswer(0, message));
                    break;
                case CommandKeys.SuccessfulRegistration:
                    EnqueueMainThreadAction(() => UIManager.instance.DisplayAnswer(2, message));
                    break;
                case CommandKeys.FailedRegistration:
                    EnqueueMainThreadAction(() => UIManager.instance.DisplayAnswer(0, message));
                    break;
                default:
                    ThreadSafeLogger.Log($"Unhandled key: {key}");
                    break;
            }
        }
        else
        {
            ThreadSafeLogger.Log("Invalid data object received.");
        }
    }

    private void EnqueueMainThreadAction(Action action)
    {
        lock (mainThreadQueue)
        {
            mainThreadQueue.Enqueue(action);
        }
    }

    private void Update()
    {
        lock (mainThreadQueue)
        {
            while (mainThreadQueue.Count > 0)
            {
                mainThreadQueue.Dequeue().Invoke();
            }
        }
    }

    public async Task ProcessPacketAsync(byte[] packet)
    {
        await Task.Run(() =>
        {
            try
            {
                using MemoryStream memoryStream = new(packet);
                var formatter = new BinaryFormatter();

                string keyType = (string)formatter.Deserialize(memoryStream);

                if (handlers.TryGetValue(keyType, out var handler))
                {
                    object dataObject = formatter.Deserialize(memoryStream);
                    
                    handler(dataObject);
                }
                else
                {
                    ThreadSafeLogger.Log($"No handler found for key: {keyType}");
                }
            }
            catch (Exception ex)
            {
                ThreadSafeLogger.Log($"Error processing packet: {ex.Message}");
            }
        });
    }
}