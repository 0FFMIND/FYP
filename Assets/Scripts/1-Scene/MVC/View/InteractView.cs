using System.Collections;
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

        [SerializeField]
        private Animator interactAnim;

        private void OnEnable()
        {
            EventBus.Subscribe<EInteract>(EnterStart);
            EventBus.Subscribe<EInteractEnd>(EnterEnd);
            Hide();
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EInteract>(EnterStart);
            EventBus.Unsubscribe<EInteractEnd>(EnterEnd);
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

        private void EnterEnd(EInteractEnd e)
        {
            StartCoroutine(OnEnd());
        }
        private IEnumerator OnEnd()
        {
            int interactLayer = 0;
            string interactAnimName = "InteractPanelExit";
            interactAnim.Play(interactAnimName, interactLayer, 0f);
            yield return new WaitForSeconds(0.1f);
            Hide();
        }


        private void EnterStart(EInteract e)
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
            StartCoroutine(OnStart());
        }

        private IEnumerator OnStart()
        {
            if (_model.index == 0)
            {
                _model.IsEntering = true;
                int interactLayer = 0;
                string interactAnimName = "InteractPanelEnter";
                interactAnim.Play(interactAnimName, interactLayer, 0f);
                yield return new WaitForSeconds(0.1f);
                _model.IsEntering = false;
            }
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
