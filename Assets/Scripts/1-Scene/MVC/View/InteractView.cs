using TMPro;
using UnityEngine;
using Utils;

namespace MVC
{
    public class InteractView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI tmp;

        [SerializeField]
        private GameObject panel;
        private InteractModel _model;

        private void OnEnable()
        {
            EventBus.Subscribe<EInteract>(OnStart);
            EventBus.Subscribe<EInteractEnd>(OnEnd);
            Hide();
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EInteract>(OnStart);
            EventBus.Unsubscribe<EInteractEnd>(OnEnd);
        }

        private void Hide()
        {
            if (tmp)
            {
                tmp.text = string.Empty;
                tmp.gameObject.SetActive(false);
            }
            if (panel)
            {
                panel.SetActive(false);
            }
            _model = null;
        }

        private void OnEnd(EInteractEnd e)
        {
            Hide();
        }

        private void OnStart(EInteract e)
        {
            _model = e.Model;
            if (_model == null || _model.IsFinished)
            {
                Hide();
                return;
            }
            // …Ë÷√ø…º˚
            tmp.text = "";
            tmp.gameObject.SetActive(true);
            panel.SetActive(true);
            Render();
        }

        private void Render()
        {
            if (tmp == null)
            {
                return;
            }
            var text = _model?.Current ?? string.Empty;
            tmp.text = text;
        }
    }
}
