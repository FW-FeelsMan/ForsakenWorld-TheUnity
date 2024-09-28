using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ForsakenWorld;

public class DecodingDataFromServer : MonoBehaviour
{
    private readonly Dictionary<string, Action<object>> handlers = new();
    private readonly Queue<Action> mainThreadQueue = new();

    public DecodingDataFromServer()
    {
        PacketHandlers();
        Logger.CurrentLogLevel = LogLevel.Debug;
    }

    private void PacketHandlers()
    {
        RegisterResponse(CommandKeys.SuccessfulLogin, ResponseProcessing);
        RegisterResponse(CommandKeys.FailedLogin, ResponseProcessing);
        RegisterResponse(CommandKeys.SuccessfulRegistration, ResponseProcessing);
        RegisterResponse(CommandKeys.FailedRegistration, ResponseProcessing);

        Logger.Log("Packet handlers registered.", LogLevel.Debug);
    }

    private void RegisterResponse(string keyType, Action<object> handler)
    {
        handlers[keyType] = handler;
        Logger.Log($"Response handler registered for key: {keyType}", LogLevel.Debug);
    }

    private void ResponseProcessing(object dataObject)
    {
        if (dataObject is GlobalDataClasses.ServerResponseMessage responseData)
        {
            string key = responseData.KeyType;
            string message = responseData.Message;

            Logger.Log($"Received response: \nKey: {key}; \nMessage: {message}", LogLevel.Info);

            switch (key)
            {
                case CommandKeys.SuccessfulLogin:
                    EnqueueMainThreadAction(() => UIManager.instance.ShowMenu());
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
                    Logger.Log($"Unhandled key: {key}", LogLevel.Warning);
                    break;
            }
        }
        else
        {
            Logger.Log("Invalid data object received.", LogLevel.Warning);
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

    /*public async Task ProcessPacketAsync(byte[] packet)
    {
        await Task.Run(() =>
        {
            using MemoryStream memoryStream = new(packet);
            var formatter = new BinaryFormatter();

            string keyType = (string)formatter.Deserialize(memoryStream);

            if (handlers.TryGetValue(keyType, out var handler))
            {
                object dataObject = formatter.Deserialize(memoryStream);
                handler(dataObject);
            }
        });
    }*/
    public async Task ProcessPacketAsync(byte[] packet)
    {
        await PacketProcessor.ProcessPacketAsync(packet, handlers);
    }
}
