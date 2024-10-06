using TMPro;
using UnityEngine;

namespace Michsky.UI.Frost
{
    public class AccountSwitchManager : MonoBehaviour
    {
        public TMP_InputField emailInput;
        public TMP_InputField passwordInput;
        private string emailKey = "SavedEmail";
        private string passwordKey = "SavedPassword";
        private void Awake()
        {
            LoadAccountData();
        }
        public void SaveData()
        {
            PlayerPrefs.SetString(emailKey, emailInput.text);
            PlayerPrefs.SetString(passwordKey, passwordInput.text);
            PlayerPrefs.Save();
        }

        private void LoadAccountData()
        {
            emailInput.text = PlayerPrefs.GetString(emailKey, "");
            passwordInput.text = PlayerPrefs.GetString(passwordKey, "");
        }

        public void ForgetData()
        {
            PlayerPrefs.DeleteKey(emailKey);
            PlayerPrefs.DeleteKey(passwordKey);
            emailInput.text = "";
            passwordInput.text = "";
        }
    }
}
