using UnityEngine;
using UnityEngine.Events;

namespace FR
{
    public class GameEventListener : MonoBehaviour
    {
        public GameEvent targetEvent;
        public UnityEvent response;

        private void OnEnable()
        {
            targetEvent.Register(this);
        }

        private void OnDisable()
        {
            targetEvent.UnRegister(this);
        }

        public void Raise()
        {
            response.Invoke();
        }
    }
}