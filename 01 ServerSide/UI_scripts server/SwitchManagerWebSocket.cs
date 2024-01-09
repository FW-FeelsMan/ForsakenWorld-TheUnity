using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Michsky.UI.Frost
{
    public class SwitchManagerWebSocket : MonoBehaviour
    {
        public Animator switchAnimator;
        private string onTransition = "Switch On";
        private string offTransition = "Switch Off";
        public bool isOn = false;
        public UnityEvent onEvent;
        public UnityEvent offEvent;
        public SocketServer _socketServer;

        private void Start()
        {
            switchAnimator.Play(offTransition);
            onEvent.Invoke();  
            AnimSwitchState();          
        }

        public void AnimSwitchState()
        {
            if (isOn)
            {
                switchAnimator.Play(onTransition);
                offEvent.Invoke();
                isOn = false;
                _socketServer.isOn = true; // Вкл
            }
            else
            {
                switchAnimator.Play(offTransition);
                onEvent.Invoke();
                isOn = true;
                _socketServer.isOn = false; // Выкл
            }
        }
    }
}
