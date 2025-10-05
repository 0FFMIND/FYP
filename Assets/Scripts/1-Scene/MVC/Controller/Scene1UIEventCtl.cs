using System.Collections;
using System.Collections.Generic;
using Manager;
using TMPro;
using UnityEngine;

namespace MVC
{
    public class Scene1UIEventCtl : MonoBehaviour
    {
        public void playSFX(string s)
        {
            AudioManager.Instance.PlaySFX(s);
        }
    }
}
