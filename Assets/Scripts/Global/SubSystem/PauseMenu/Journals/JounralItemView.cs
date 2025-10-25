using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace MVC
{
    public class JournalItemView : MonoBehaviour
    {
        [SerializeField]
        private Button button;

        [SerializeField]
        private TMP_Text titleText;

        // 缓存当前行对应的 journal key，用于点击时广播
        private string _key;

        public void Bind(JournalItem item)
        {
            // 记录对应的键，供点击回调使用
            _key = item.key;

            // 本地化
            titleText.GetComponent<LocalizedText>().SetKey(item.title, true);

            // 绑定按钮点击：点击后发布“选中该条目”的事件
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                EventBus.Publish(new EJournalSelected(_key));
            });
        }
    }
}
