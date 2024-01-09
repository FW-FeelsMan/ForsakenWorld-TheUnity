using UnityEngine;
using UnityEngine.UI;

public class ToggleGenderColor : MonoBehaviour
{
    public Toggle toggle;
    public Color onColor = Color.yellow;
    public Color offColor = Color.white;

    public Image toggleImage;

    private void Start()
    {
        toggle = gameObject.GetComponent<Toggle>(); 
        toggleImage = toggle.GetComponent<Image>();

        toggle.onValueChanged.AddListener(OnToggleValueChanged);

        OnToggleValueChanged(toggle.isOn);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (toggleImage != null)
        {
            toggleImage.color = isOn ? onColor : offColor;
        }
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
    }
}
