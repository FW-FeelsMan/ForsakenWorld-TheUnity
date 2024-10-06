
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Diagnostics;
using UnityEngine;

public class DecodingDataFromServer : MonoBehaviour
{
    private readonly Dictionary<string, Action<object>> handlers = new();
    private readonly Queue<Action> mainThreadQueue = new();
    private static Stopwatch pingTimer = new Stopwatch();
    public DecodingDataFromServer()
    {
        PacketHandlers();
    }

    private void PacketHandlers()
    {
        Response(CommandKeys.SuccessfulLogin, ResponseProcessing);
        Response(CommandKeys.FailedLogin, ResponseProcessing);
        Response(CommandKeys.SuccessfulRegistration, ResponseProcessing);
        Response(CommandKeys.FailedRegistration, ResponseProcessing);

        Response(CommandKeys.GetPong, ResponseProcessing);
    }

    private void Response(string keyType, Action<object> handler)
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
                    Task.Run(async () => await RequestToServer.SendPingMessage());
                    //ThreadSafeLogger.Log($"Добро пожаловать");
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
                case CommandKeys.GetPong:
                    pingTimer.Stop(); 
                    long pingTime = pingTimer.ElapsedMilliseconds;  
                    float infelicityTime = .1f;
                    ThreadSafeLogger.Log($"Пинг: {pingTime + infelicityTime} мс");
                    GlobalSettings.CurrentPing = pingTime + 1;

                    if (pingTime > GlobalSettings.MaxServerResponseTimeout)
                    {
                        UIManager.instance.DisplayAnswer(0, GlobalStrings.ErrorWaitingForResponse);
                    }  
                    else
                    {
                        Task.Run(async () => await RequestToServer.SendPingMessage());
                    }
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