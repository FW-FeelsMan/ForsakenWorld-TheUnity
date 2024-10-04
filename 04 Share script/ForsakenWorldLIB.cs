using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

namespace ForsakenWorld
{
    public class Character : MonoBehaviour
    {
        public int CharacterId;
        public int UserId;
        public int ClassId;
        public int GenderId;
        public int RaceId;
        public int Level;
        public string Appearance;
        public int Currency;
        public int GoldCurrency;
        public int CharacterStatus;
        public string ClassName;
        public string GenderName;
        public string RaceName;

        public static string GetClassName(int classId)
        {
            return classId switch
            {
                1 => "Warrior",
                2 => "Cleric",
                3 => "Assassin",
                _ => "Unknown Class"
            };
        }

        public static string GetGenderName(int genderId)
        {
            return genderId switch
            {
                1 => "Male",
                2 => "Female",
                _ => "Unknown Gender"
            };
        }

        public static string GetRaceName(int raceId)
        {
            return raceId switch
            {
                1 => "Demon",
                2 => "Dworf",
                3 => "Elf",
                _ => "Unknown Race"
            };
        }
    }
    }

   /* public class PacketProcessor
    {
        private readonly Socket clientSocket;

        public PacketProcessor(Socket clientSocket)
        {
            this.clientSocket = clientSocket;
        }

        public async Task ProcessPacketAsync(byte[] packet, Dictionary<string, Action<object>> handlers)
        {
            try
            {
                var keyType = await Task.Run(() => SerializationUtils.DeserializeObject<string>(packet));  // добавлен await
                Debug.Log($"PacketProcessor: Обработка данных для ключа {keyType}");

                if (handlers.TryGetValue(keyType, out var handler))
                {
                    var dataObject = await Task.Run(() => SerializationUtils.DeserializeObject<object>(packet)); // добавлен await
                    if (dataObject is GlobalDataClasses.ServerResponseMessage responseMessage)
                    {
                        Debug.Log($"PacketProcessor: Получены данные: Ключ = {responseMessage.KeyType}, Сообщение = {responseMessage.Message}");
                    }
                    else
                    {
                        Debug.Log($"PacketProcessor: Получены данные: {dataObject}");
                    }

                    handler(dataObject);
                }
                else
                {
                    Debug.LogError($"PacketProcessor: Обработчик для ключа {keyType} не найден");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка обработки пакета: {ex.Message}");
            }
        }

        public async Task RequestTypeAsync(string keyType, object dataObject)
        {
            try
            {
                if (dataObject is GlobalDataClasses.ServerResponseMessage responseMessage)
                {
                    Debug.Log($"Отправка данных клиенту {clientSocket.RemoteEndPoint}: Ключ = {keyType}, Сообщение = {responseMessage.Message}");
                }
                else
                {
                    Debug.Log($"Отправка данных клиенту {clientSocket.RemoteEndPoint}: Ключ = {keyType}, Данные = {dataObject}");
                }

                byte[] requestData = await Task.Run(() => SerializationUtils.SerializeObject(dataObject));  // добавлен await
                await SendDataAsync(requestData);  // добавлен await
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка при отправке данных клиенту: {ex.Message}");
                SocketUtils.CloseSocket(clientSocket);
            }
        }

        private async Task SendDataAsync(byte[] data)
        {
            try
            {
                Debug.Log($"Sending data to client: {clientSocket.RemoteEndPoint}, Data size: {data.Length}");

                using NetworkStream networkStream = new(clientSocket);
                await networkStream.WriteAsync(data, 0, data.Length);  // добавлен await

                Debug.Log("Data successfully sent.");
            }
            catch (SocketException ex)
            {
                Debug.LogError($"SocketException while sending data: {ex.Message}");
                SocketUtils.CloseSocket(clientSocket);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error sending data: {ex.Message}");
                SocketUtils.CloseSocket(clientSocket);
            }
        }
    }

}

public static class SerializationUtils
{
    public static byte[] SerializeObject(object obj)
    {
        using MemoryStream memoryStream = new();
        var formatter = new BinaryFormatter();
        formatter.Serialize(memoryStream, obj);
        return memoryStream.ToArray();
    }

    public static T DeserializeObject<T>(byte[] data)
    {
        using MemoryStream memoryStream = new(data);
        var formatter = new BinaryFormatter();
        return (T)formatter.Deserialize(memoryStream);
    }
}

public static class SocketUtils
{
    public static void CloseSocket(Socket socket)
    {
        if (socket != null)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}
*/