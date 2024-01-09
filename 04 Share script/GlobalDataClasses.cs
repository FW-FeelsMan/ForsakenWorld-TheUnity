using System;
using UnityEngine;

public class GlobalDataClasses : MonoBehaviour
{
    [Serializable]
    public struct UserDataObject
    {
        public string KeyType { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public string HardwareID { get; set; }
    }

    [Serializable]
    public class ServerResponseMessage 
    {
        public string KeyType { get; set; }
        public string Message { get; set; }
    }
}
