using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ImageFillController : MonoBehaviour
{
    public Image imageUnderline;
    public float fillSpeed = 0.5f;
    public float scaleMultiplier = 1.1f;
    public Toggle toggle;

    public GameObject toggleMale;
    Toggle _toggleMale;
    public GameObject toggleFemale;
    Toggle _toggleFemale;

    private bool isSelected = false;
    private Vector3 originalScaleClass;
    private RectTransform rectTransform;
    public bool prevIsOn = false;
    public bool maleIsOn = false;
    public bool femaleIsOn = false;

    public RaceSelector raceSelector;
    public string nameClass;

    private void Start()
    {
        toggle = GetComponent<Toggle>();

        try
        {
            _toggleMale = toggleMale.GetComponent<Toggle>();
            _toggleMale.interactable = false;

            _toggleFemale = toggleFemale.GetComponent<Toggle>();
            _toggleFemale.interactable = false;

        }catch(System.Exception )
        {

        }        

        imageUnderline.fillAmount = 0f;
        originalScaleClass = transform.localScale;
        rectTransform = GetComponent<RectTransform>();

        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    public void OnToggleValueChanged(bool isOn)
    {
        isSelected = isOn;
        
        if (isSelected && !prevIsOn)
        {
            StartCoroutine(Animate(1f, originalScaleClass * scaleMultiplier));
            _toggleMale.interactable = true;  
            _toggleFemale.interactable = true;
            raceSelector.SelectClass(nameClass);
        }
        else if (!isSelected && prevIsOn)
        {
            StartCoroutine(Animate(0f, originalScaleClass));
            _toggleMale.interactable = false;
            _toggleFemale.interactable = false;
        }

        prevIsOn = isSelected; 
    }

    private void Update()
    {
        bool isOn = toggle.isOn;
        maleIsOn = _toggleMale.isOn;
        femaleIsOn = _toggleFemale.isOn;

        if (isOn != prevIsOn)
        {
            OnToggleValueChanged(isOn);
        }
    }

    private IEnumerator Animate(float targetFill, Vector3 targetScale)
    {
        float startFill = imageUnderline.fillAmount;
        Vector3 startScale = rectTransform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < fillSpeed)
        {
            imageUnderline.fillAmount = Mathf.Lerp(startFill, targetFill, elapsedTime / fillSpeed);
            rectTransform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / fillSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        imageUnderline.fillAmount = targetFill;
        rectTransform.localScale = targetScale;
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
    }
}
