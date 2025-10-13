// Assets/Scripts/Inventory/ItemView.cs
using TMPro;
using UnityEngine;

namespace MVC
{
    /// <summary>一行：左—名称，右—数量（用 prefab，两个 TMP 文本手动拖引用）</summary>
    public class ItemView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text nameText;

        [SerializeField]
        private TMP_Text countText;

        /// <summary>给该行设置显示内容（控制器负责把 id 转成展示名）</summary>
        public void Bind(string displayName, int count)
        {
            nameText.GetComponent<LocalizedText>().SetKey(displayName);

            if (nameText)
            {
                nameText.text = displayName ?? "";
            }
            if (countText)
            {
                countText.text = count.ToString();
            }
        }
    }
}
