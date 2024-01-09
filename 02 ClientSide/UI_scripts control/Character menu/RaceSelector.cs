using UnityEngine;
using System.Collections.Generic;

public class RaceSelector : MonoBehaviour
{
    public GameObject[] classes; 

    public void SelectClass(string className)
    {
        List<int> activeIndices = new List<int>();

        if (className == "����")
        {
            activeIndices.AddRange(new int[] { 2, 4 });
        }
        else if (className == "�����")
        {
            activeIndices.AddRange(new int[] { 2, 4, 5 });
        }
        else if (className == "������")
        {
            activeIndices.AddRange(new int[] { 6, 4 });
        }
        else if (className == "�������")
        {
            activeIndices.AddRange(new int[] { 1, 4, 3 });
        }
        else if (className == "������")
        {
            activeIndices.AddRange(new int[] { 1, 6 });
        }
        else if (className == "������")
        {
            activeIndices.AddRange(new int[] { 4, 5, 6, 2 });
        }
        else if (className == "����")
        {
            activeIndices.AddRange(new int[] { 5, 6 });
        }
        else if (className == "�����������")
        {
            activeIndices.AddRange(new int[] { 4, 5, 2 });
        }
        else if (className == "��������")
        {
            activeIndices.AddRange(new int[] { 1, 3, 4 });
        }
        else if (className == "����������")
        {
            activeIndices.AddRange(new int[] { 0 });
        }
        else if (className == "������")
        {
            activeIndices.AddRange(new int[] { 2, 4, 5 });
        }
        else if (className == "���")
        {
            activeIndices.AddRange(new int[] { 4, 6 });
        }
        else if (className == "�������")
        {
            activeIndices.AddRange(new int[] { 2, 4 });
        }
        else if (className == "�������")
        {
            activeIndices.AddRange(new int[] { 3, 1 });
        }
        else if (className == "����")
        {
            activeIndices.AddRange(new int[] { 2, 4 });
        }
        else if (className == "������������")
        {
            activeIndices.AddRange(new int[] { 5, 6 });
        }
        else if (className == "���������")
        {
            activeIndices.AddRange(new int[] { 4, 6 });
        }
        for (int i = 0; i < classes.Length; i++)
        {
            if (activeIndices.Contains(i))
            {
                classes[i].SetActive(true); 
            }
            else
            {
                classes[i].SetActive(false); 
            }
        }
    }
}
