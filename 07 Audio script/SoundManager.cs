using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    /// <summary>
    /// ������ ���/����. ������ � �����-�����
    /// </summary>
    public Image soundIcon;
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;
    private bool isSoundOn;
    private const string SoundStateKey = "SoundState";
    public AudioSource audioSource;

    void Start()
    {
        LoadSoundState();
    }
    public void ToggleSound()
    {
        isSoundOn = !isSoundOn;

        if (isSoundOn)
        {
            soundIcon.sprite = soundOnSprite;
            audioSource.Play();
        }
        else
        {
            soundIcon.sprite = soundOffSprite;
            audioSource.Pause();
        }
        SaveSoundState();
    }

    private void SaveSoundState()
    {
        PlayerPrefs.SetInt(SoundStateKey, isSoundOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadSoundState()
    {
        // ��������� ��������� ��������� ����� �� PlayerPrefs
        isSoundOn = PlayerPrefs.GetInt(SoundStateKey, 1) == 1;

        // ��������� ����������� ������
        if (isSoundOn)
        {
            soundIcon.sprite = soundOnSprite;
            // �������� ���� (��� ��� ��������� �����)
        }
        else
        {
            soundIcon.sprite = soundOffSprite;
            // ��������� ���� (��� ��� ���������� �����)
        }
    }
}
