using UnityEngine;
using UnityEngine.UI;

public class TabPanelManager : MonoBehaviour
{
    public GameObject[] tabPanels;
    public Toggle[] tabToggles;

    private void Start()
    {
        ShowTab(0);

        for (int i = 0; i < tabToggles.Length; i++)
        {
            int index = i; 
            tabToggles[i].onValueChanged.AddListener((value) => ShowTab(index, value));
        }
    }

    private void ShowTab(int index, bool value = true)
    {
        if (index >= 0 && index < tabPanels.Length)
        {
            tabPanels[index].SetActive(value);
        }

        if (value)
        {
            for (int i = 0; i < tabPanels.Length; i++)
            {
                if (i != index)
                {
                    tabPanels[i].SetActive(false);
                    tabToggles[i].isOn = false;
                }
            }
        }
    }
}
