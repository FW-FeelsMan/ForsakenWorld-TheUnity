using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class HyperlinkText : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string linkURL = "http://localhost/UnityBackend/getCONDITIONS.php";
    [SerializeField] private TMP_Text textMeshPro;

    private void Awake()
    {
        textMeshPro = GetComponent<TMP_Text>();
        textMeshPro.text = "���������������� �����������";
        textMeshPro.enableWordWrapping = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Application.OpenURL(linkURL);
    }
}
