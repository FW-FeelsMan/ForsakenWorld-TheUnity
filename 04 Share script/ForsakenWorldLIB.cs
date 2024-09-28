using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace ForsakenWorld
{
    public class Character
    {
        public int CharacterId { get; set; }
        public int UserId { get; set; }
        public int ClassId { get; set; }
        public int GenderId { get; set; }
        public int RaceId { get; set; }
        public int Level { get; set; }
        public string Appearance { get; set; }
        public int Currency { get; set; }
        public int GoldCurrency { get; set; }
        public int CharacterStatus { get; set; }

        public string ClassName { get; set; }
        public string GenderName { get; set; }
        public string RaceName { get; set; }

        public static string GetClassName(int classId)
        {
            return classId switch
            {
                1 => "Warrior",
                2 => "Cleric",
                3 => "Assassin",
                4 => "Jagernaut",
                5 => "Vampire",
                6 => "Witch",
                7 => "Ripper",
                8 => "Caster",
                9 => "Defender",
                10 => "Torturer",
                11 => "Archer",
                12 => "Mage",
                13 => "Paladin",
                14 => "Crosshear",
                15 => "Bard",
                16 => "Dark Knight",
                17 => "Necromancer",
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
                4 => "Frangor",
                5 => "Human",
                6 => "Likan",
                7 => "Vesperian",
                _ => "Unknown Race"
            };
        }
    }
    public class PacketProcessor
    {
        public static async Task ProcessPacketAsync(byte[] packet, Dictionary<string, Action<object>> handlers)
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
        }
        public static async Task SendDataAsync(Socket clientSocket, byte[] data)
        {
            try
            {
                using NetworkStream networkStream = new(clientSocket);
                await networkStream.WriteAsync(data, 0, data.Length);
                Logger.Log("Data sent successfully.", LogLevel.Info);
            }
            catch (Exception ex)
            {
                Logger.Log($"Error sending data: {ex.Message}", LogLevel.Error);
                //onFailure?.Invoke();
            }
        }
    }
}
