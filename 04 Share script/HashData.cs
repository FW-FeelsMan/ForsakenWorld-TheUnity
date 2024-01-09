using UnityEngine;
using System;
using System.Security.Cryptography;
using System.Text;

public class HashData : MonoBehaviour
{
    static string salt = "!@#$%^&";
    public static string Hashing(string data)
    {
        SHA256 sha256 = SHA256.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(data + salt); 
        byte[] hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
