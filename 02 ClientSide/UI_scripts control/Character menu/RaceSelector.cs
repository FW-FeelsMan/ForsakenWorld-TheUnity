using UnityEngine;
using System.Collections.Generic;

public class RaceSelector : MonoBehaviour
{
    public GameObject[] classes; 

    public void SelectClass(string className)
    {
        List<int> activeIndices = new List<int>();

        if (className == "Воин")
        {
            activeIndices.AddRange(new int[] { 2, 4 });
        }
        else if (className == "Прист")
        {
            activeIndices.AddRange(new int[] { 2, 4, 5 });
        }
        else if (className == "Убийца")
        {
            activeIndices.AddRange(new int[] { 6, 4 });
        }
        else if (className == "Берсерк")
        {
            activeIndices.AddRange(new int[] { 1, 4, 3 });
        }
        else if (className == "Вампир")
        {
            activeIndices.AddRange(new int[] { 1, 6 });
        }
        else if (className == "Ведьма")
        {
            activeIndices.AddRange(new int[] { 4, 5, 6, 2 });
        }
        else if (className == "Жнец")
        {
            activeIndices.AddRange(new int[] { 5, 6 });
        }
        else if (className == "Заклинатель")
        {
            activeIndices.AddRange(new int[] { 4, 5, 2 });
        }
        else if (className == "Защитник")
        {
            activeIndices.AddRange(new int[] { 1, 3, 4 });
        }
        else if (className == "Истязатель")
        {
            activeIndices.AddRange(new int[] { 0 });
        }
        else if (className == "Лучник")
        {
            activeIndices.AddRange(new int[] { 2, 4, 5 });
        }
        else if (className == "Маг")
        {
            activeIndices.AddRange(new int[] { 4, 6 });
        }
        else if (className == "Паладин")
        {
            activeIndices.AddRange(new int[] { 2, 4 });
        }
        else if (className == "Стрелок")
        {
            activeIndices.AddRange(new int[] { 3, 1 });
        }
        else if (className == "Бард")
        {
            activeIndices.AddRange(new int[] { 2, 4 });
        }
        else if (className == "ТемныйРыцарь")
        {
            activeIndices.AddRange(new int[] { 5, 6 });
        }
        else if (className == "Некромант")
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
