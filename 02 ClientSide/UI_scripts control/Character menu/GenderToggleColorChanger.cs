using UnityEngine;
using UnityEngine.UI;

public class GenderToggleColorChanger : MonoBehaviour
{
    public Toggle classToggle;
    public Toggle maleToggle;
    public Toggle femaleToggle;
    public Image maleImage;
    public Image femaleImage;
    ToggleGroup toggleGroup;

    public Color orangeColor = Color.yellow;
    private Color whiteColor = Color.white;

    private void Start()
    {
        toggleGroup = GetComponent<ToggleGroup>();
    }
    void Update()
    {
        if (!classToggle.isOn)
        {
            toggleGroup.enabled = false;
            maleToggle.isOn = false;
            femaleToggle.isOn = false;
        }

        if (classToggle.isOn)
        {
            toggleGroup.enabled = true;
            if (femaleToggle.isOn)
            {
                femaleImage.color = orangeColor;
            }
            else
            {
                femaleImage.color = whiteColor;
            }

            if (maleToggle.isOn)
            {
                maleImage.color = orangeColor;
            }
            else
            {
                maleImage.color = whiteColor;
            }
        }
        else
        {
            femaleImage.color = whiteColor;
            maleImage.color = whiteColor;
        }
    }
}
