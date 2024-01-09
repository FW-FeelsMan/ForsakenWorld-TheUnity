using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class IsOnCheckerRace : MonoBehaviour
{
    public GameObject[] raceObjects;

    private List<Toggle> activeToggles = new List<Toggle>();
    private List<Toggle> previousToggles = new List<Toggle>();

    void Update()
    {
        activeToggles.Clear();

        foreach (GameObject raceObject in raceObjects)
        {
            Toggle toggle = raceObject.GetComponent<Toggle>();

            if (toggle != null)
            {
                if (raceObject.activeSelf)
                {
                    activeToggles.Add(toggle);
                }
                else
                {
                    toggle.isOn = false; // Если игровой объект неактивен, снимаем галочку.
                }
            }
        }

        if (!ListsAreEqual(activeToggles, previousToggles))
        {
            int randomIndex = Random.Range(0, activeToggles.Count);
            Toggle randomToggle = activeToggles[randomIndex];
            randomToggle.isOn = true;

            // Обновляем предыдущий список
            previousToggles.Clear();
            previousToggles.AddRange(activeToggles);
        }
    }

    // Функция для сравнения двух списков
    private bool ListsAreEqual(List<Toggle> list1, List<Toggle> list2)
    {
        if (list1.Count != list2.Count)
        {
            return false;
        }

        for (int i = 0; i < list1.Count; i++)
        {
            if (list1[i] != list2[i])
            {
                return false;
            }
        }

        return true;
    }
}
