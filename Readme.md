2025/5/6 - 防止忘了，用了DialogueModel存从文本.txt里面读到的话，中间一个方法LoadDialogue会通过LocalizationManager(用一个static存的当前语言)定位到具体路径/文本.txt

通过dialoguectl控制dialoguemodel，直接通过new创建一个并获得控制权，太痛苦了写代码，暂时还没写按键适配，本来想dialogueView加一个渐变效果的，目前只做了一个启动功能

打算写完对话功能，其他的再看吧。。

2025/5/13 - 之前写到DialogueCtl里面了，感觉实际上是PrologueDCtl，里面读取txt其实可以用一个string进行private赋值，但话又说回来了，我会用到这个Ctl很多次吗？不写死的话，我的感觉是我会写死，但是为了FYP要求放在一个字段里面了，还需要一个自定的字段，linemapping，就不需要写死了

2025/5/17 - 你能想象我又在日本人的网站上扒谱了吗，感觉我喜欢的音乐风格是八音盒，但是不搭。。和文字不搭

炼狱网址：https://note.com/search?q=from%3A%40rengokuteien&context=note&sort=popular

魔王：https://maou.audio/category/song/

效果音：https://pixabay.com/zh/sound-effects/search/%e6%8c%89%e9%94%ae%e9%9f%b3/

我其实挺想看诺艾尔的致谢名单，它用的曲是从哪里扒的，用网易云的听歌识曲能出来几首吗，感觉钢琴曲要多一些，找的曲子八音盒感觉不是很合适，诺艾尔很多曲子都很燃，主要是我一直觉得它的剧情本来就很燃（。我可能更需要静谧+平和但不需要太静谧的曲子，静谧会增加孤独感和神秘感，在我的理解里面

StreamingAsset存的二进制文件，像AudioMixer，放进Resource里面

已完成：TD:audiomanager，使用audiomixer

2025/5/22 - TD:想了一下这个应该先做，事件系统，一种方法是用timeline k帧，在想自己写一个，最开始prologuectl，是在start()里面响应，把这里的start改成startdialogue，手动控制，在游戏控制前加一个warning的效果，加入一个Scene1Ctl，让播放顺序warning->序章（已完成）

已完成：TD:每次对话下面有个小箭头，需要修改(让GPT先给我随便吧)：开头panel，背景



TD:分支对话/能够跳过显示 <-这个也不难

把GPT的拉上来：

下面给出思路和示例代码，展示如何在现有 `DialogueCtl` 基础上接入分支选项：

------

## 1. 对话数据结构扩展

在 `DialogueModel` 里，把对话脚本拆分成两类节点：

- **普通文本行**：显示一句话，接着自动推进
- **分支节点**：显示若干按钮，等待玩家选一条，然后跳到对应的行号

示例 `DialogueNode` 定义：

```csharp
public enum NodeType { Line, Choice }

public class ChoiceOption
{
    public string text;     // 按钮上的文字
    public int nextIndex;   // 选中后跳转到的节点索引
}

public class DialogueNode
{
    public NodeType type;
    public string line;             // type==Line 时使用
    public List<ChoiceOption> opts; // type==Choice 时使用
}
```

然后 `DialogueModel` 加载完脚本后，提供一个 `DialogueNode[] Nodes` 数组。

------

## 2. `DialogueCtl` 改动

1. **替换原来的 `model.Lines` 为 `model.Nodes`**
2. 在 `NextLine()` 里：
   - 如果 `node.type == Line`，继续原来的“打字机 + 下一行”流程
   - 如果 `node.type == Choice`，暂停按空格推进，调用 `view.RenderChoices(...)`，并传入所有按钮文字与回调
3. **添加一个回调处理**，当玩家点击某个选项时：
   - 隐藏按钮
   - 把 `index = choice.nextIndex`
   - 调用 `NextLine()` 继续

下面是核心示例代码（仅展示 `DialogueCtl`）：

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MVC.View;
using MVC.Model;
using InputSystem;

namespace MVC.Controller
{
    public class DialogueCtl : MonoBehaviour
    {
        [SerializeField] private DialogueView view;
        [SerializeField] private string dialogueTxt;
        [SerializeField] private float typeSpeed = 0.03f;

        private Sprite currentSprite;
        private int index;
        private DialogueModel model;
        private Coroutine typingCoroutine;
        private bool awaitingChoice = false;

        private void Start()
        {
            view.Render(null, null);
            model = new DialogueModel(dialogueTxt);
            index = 0;
            InputManager.Instance.OnAction += OnInputAction;
        }

        private void OnInputAction(InputAction action)
        {
            if (action != InputAction.DialogueClick || typingCoroutine != null || awaitingChoice)
                return;

            NextNode();
        }

        private void NextNode()
        {
            if (index < 0 || index >= model.Nodes.Length)
            {
                // 对话结束
                view.Render(null, null);
                InputManager.Instance.OnAction -= OnInputAction;
                return;
            }

            var node = model.Nodes[index];
            switch (node.type)
            {
                case NodeType.Line:
                    // 普通台词
                    typingCoroutine = StartCoroutine(TypeLine(node.line, () =>
                    {
                        typingCoroutine = null;
                        index++;
                    }));
                    break;

                case NodeType.Choice:
                    // 分支选项
                    awaitingChoice = true;
                    view.RenderChoices(node.opts, OnChoiceSelected);
                    break;
            }
        }

        // 打字机效果，doneCallback 在整句显示完后调用
        private IEnumerator TypeLine(string fullText, System.Action doneCallback)
        {
            view.Render(currentSprite, "");
            for (int i = 0; i < fullText.Length; i++)
            {
                view.Render(currentSprite, fullText.Substring(0, i + 1));
                yield return new WaitForSeconds(typeSpeed);
            }
            doneCallback?.Invoke();
        }

        // 玩家选了某个分支
        private void OnChoiceSelected(int choiceIndex)
        {
            var chosen = model.Nodes[index].opts[choiceIndex];
            view.HideChoices();
            awaitingChoice = false;
            index = chosen.nextIndex;
            NextNode();
        }

        private void OnDestroy()
        {
            InputManager.Instance.OnAction -= OnInputAction;
        }
    }
}
```

------

## 3. `DialogueView` 需要新增的方法

在你的视图里（伪代码示例）：

```csharp
public class DialogueView : MonoBehaviour
{
    // 旧的 Render(sprite, text) 保留
    public void Render(Sprite s, string text) { … }

    // 新增：渲染分支按钮
    // opts 包含每个选项的 text，callback 参数是点击后回调，传入选中按钮的下标
    public void RenderChoices(List<ChoiceOption> opts, System.Action<int> callback)
    {
        // 1. 显示按钮组（可以是 prefab pool）
        // 2. 为每个按钮设置显示文字 opts[i].text
        // 3. 挂 onClick = () => callback(i)
    }

    public void HideChoices()
    {
        // 隐藏或销毁所有分支按钮
    }
}
```

------

这样就完成了“普通台词 + 打字机 + 分支选择” 的一套流程。你只要：

1. 在脚本里按上述结构写好 `DialogueNode[]` 数据。
2. 在 `DialogueView` 中实现 `RenderChoices` / `HideChoices`。
3. 把分支 `ChoiceOption.nextIndex` 填成脚本中对应行的索引即可。

TD:改键

TD:暂停菜单

