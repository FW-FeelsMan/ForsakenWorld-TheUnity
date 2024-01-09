using System;
using System.Management;
using UnityEngine;

public class HardwareID : MonoBehaviour
{
    public static string GetHardwareID()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }
}
