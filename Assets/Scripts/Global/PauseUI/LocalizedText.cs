using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace MVC
{
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string key;
        private TMP_Text tmpText;

        private void OnEnable()
        {
            EventBus.Subscribe<ELanguageChanged>(OnLang);
            Refresh();
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ELanguageChanged>(OnLang);
        }

        private void Awake()
        {
            AutoWire();
            if (!tmpText)
            {
                Debug.LogWarning($"[LocalizedText] 缺少 Text/TMP_Text，已禁用: {name}");
                enabled = false;
                return;
            }
            if (string.IsNullOrWhiteSpace(key))
            {
                Debug.LogWarning($"[LocalizedText] 缺少 key，已禁用: {name}");
                enabled = false;
                return;
            }
        }

        private void OnLang(ELanguageChanged e) => Refresh();

        private void Refresh()
        {
            tmpText.text = LocalizationMgr.Instance.GetText(key);
        }

        // 自动查找同节点上的 TMP_Text 组件
        private void AutoWire()
        {
            if (!tmpText)
            {
                tmpText = GetComponent<TMP_Text>();
            }
        }
    }
}
