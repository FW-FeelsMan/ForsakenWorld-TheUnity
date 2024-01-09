using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public TMP_InputField messageInputField;
    public Transform messageList;
    public TMP_Text textMessagePrefab;
    public ScrollRect scrollRect;

    private void Awake()
    {
        LogProcessor.OnLogProcessed += AddMessage;

        foreach (Transform child in messageList)
        {
            Destroy(child.gameObject);
        }
    }


    private void Update()
    {
        if (!IsMouseOverUI())
        {
            ScrollToNewMessage();
        }
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (string.IsNullOrEmpty(messageInputField.text))
            {
                ActivateInputField();
            }
            else
            {
                AddMessage(messageInputField.text);
                messageInputField.text = "";

                if (!IsMouseOverUI())
                {
                    ScrollToNewMessage();
                }
            }
        }
    }

    private void ActivateInputField()
    {
        messageInputField.Select();
        messageInputField.ActivateInputField();
    }

    private void AddMessage(string message)
    {
        TMP_Text newMessage = Instantiate(textMessagePrefab, messageList);
        newMessage.text = message;
        ScrollToNewMessage();
    }

    private void ScrollToNewMessage()
    {
        if (!IsMouseOverUI())
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }

    private bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
