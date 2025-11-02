using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace MVC
{
    public class JournalItemView : PauseMenuToggleView
    {
        // 缓存当前行对应的 journal key，用于点击时广播
        private string _key;

        // 绑定数据并指定所属 ToggleGroup
        public void Bind(JournalItem item, ToggleGroup group)
        {
            // 记录对应的键，供点击回调使用
            _key = item.key;

            // 检查对象是否为空
            if (text == null)
            {
                Debug.LogError("JournalItemView: text 为 null!");
                return;
            }

            this.text.gameObject.GetComponent<LocalizedText>().SetKey(item.title, true);

            // 调用基类的 Bind 方法
            Bind(group);

            // 添加 Journal 特定的事件
            OnSelected += () =>
            {
                // Journal 特定的选中事件
                EventBus.Publish(new EJournalSelected(_key));
            };
        }
    }
}
