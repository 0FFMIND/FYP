using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Manager
{
    public class EventSystemHost : MonoBehaviour
    {
        private string childName = "EventSystem";
        private EventSystem _ownedES;

        public void Init(Transform parent)
        {
            if (transform.parent != parent)
            {
                transform.SetParent(parent, false);
            }
            EnsureEventSystemExists();
        }

        private void EnsureEventSystemExists()
        {
            var go = new GameObject(childName);
            go.transform.SetParent(transform, false);
            _ownedES = go.AddComponent<EventSystem>();
            go.AddComponent<StandaloneInputModule>();
        }
    }
}
