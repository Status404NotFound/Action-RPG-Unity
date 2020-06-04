using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FR
{
    [CreateAssetMenu(menuName = "Event")]
    public class GameEvent : ScriptableObject
    {
        List<GameEventListener> listeners = new List<GameEventListener>();

        public void Register(GameEventListener l)
        {
            if (!listeners.Contains(l))
                listeners.Add(l);
        }

        public void UnRegister(GameEventListener l)
        {
            if (listeners.Contains(l))
                listeners.Remove(l);
        }

        public void Raise()
        {
            for (int i = 0; i < listeners.Count; i++)
            {
                listeners[i].Raise();
            }
        }
    }
}