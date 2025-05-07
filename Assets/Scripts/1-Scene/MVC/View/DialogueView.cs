using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace MVC.View
{
    public class DialogueView : MonoBehaviour
    {
        public Image img;
        public TextMeshProUGUI tmp;

        public void Render(Sprite sprite, string text)
        {
            if(sprite == null)
            {
                if (img.enabled)
                {
                    img.enabled = false;
                }
            }
            else
            {
                // disabled
                if (!img.enabled)
                {
                    img.enabled = true;
                }
                if(img.sprite != sprite)
                {
                    img.sprite = sprite;
                }
            }
            tmp.text = text;
        }
    }
}


