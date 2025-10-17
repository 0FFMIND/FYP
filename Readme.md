EventSystem：

在`GameEvents`里面定义每种事件 `Event`，由一个全局静态的 `EventBus` 用 `Dictionary<Event, Delegate> table_` 把`T(Event)`映射到 `Delegate`上，订阅者用 `Subscribe<T>(callback)` 把回调加入该`table_ [T]`，发布者用 `Publish<T>(data)` 从`table_[T]`里 `Invoke(all callback and pass data)`

SettingsMgr:

存入SettingsData，在游戏一开始的时候调用Load()，然后通过EventBus broadcast ESettingsChanged事件，InputMgr, LocalizationMgr, AudioMgr接收，并且修改自己的private副本，方便Mgr内部访问而不用每次需要数据的时候都query SettingsMgr，之后若外部想要修改SettingsData的数据，则会发送请求给SettingsMgr，它接受请求后修改SettingsData并且Save()，Save后broadcast一次，让其他mgr的副本得到更新

使用过的软件：GPTo5 Audacity(变调/调整音量) Procreate

2025/5/6 - 用了DialogueModel存从文本.txt里面读到的话，中间一个方法LoadDialogue会通过LocalizationManager(用一个static存的当前语言)定位到具体路径/文本.txt

2025/5/13 - 写了DialogueCtl

2025/5/17 - 在找素材

炼狱网址：https://note.com/search?q=from%3A%40rengokuteien&context=note&sort=popular

魔王：https://maou.audio/category/song/

效果音：https://pixabay.com/zh/sound-effects/search/%e6%8c%89%e9%94%ae%e9%9f%b3/

StreamingAsset存的二进制文件，像AudioMixer，放进Resource里面

2025/5/22 - 在游戏控制前加一个warning的效果，加入一个Scene1Ctl，让播放顺序warning->序章

2025/5/23 - 已完成：加一个提示的声音，修改warning的UI panel

2025/5/26 - 本地化系统，暂时用excel配表

2025/8/14 - reformat了一下，写了一个很小的bgm控制滑块

找到了更方便搜索的音乐库：https://www.aigei.com/music/class

2025/8/15 - 稍微搭了下主场景，在写暂停菜单，需要加入数据持久化

2025/8/16 - 加入了简单的人物移动逻辑，用的代码控制动画机，感觉要方便一点，等待加入跑步逻辑

2025/8/17 - 事件由EventBus分发，目前Volume，Language写好了，写了UIText本地化逻辑

2025/9/2 - Fixed，之前没有应用保存的setting修改language，集成了inputManager到EventBus，新加了EventBus按key唤醒重载，无参Action重载

2025/9/4 - keycode change改键，菜单暂时写完

2025/9/5 - 完成人物疾跑（用keyup和keydown事件，通过eventbus publish实现，keyup加速，keydown元素，加了碰撞体后将人物移动放到fixedUpdate

2025/9/8 - 加入了SettingsData的model层，并且统一通过ESettingsChanged事件通知其他Mgr修改本地副本，完成人物移速，跑步速度写到settings里面修改

2025/9/9 - 给localizationTxt加入key，支持{key}的替换，加入了一个-+的UI表示增减，写了warningPanel的双语，好用的bgm素材网站：https://musmus.main.jp/music_img1_06.html, 把抖动的打字机展示改了实现

2025/9/10 - Fixed: 因为SettingsMgr在Awake里面就Publish过一次事件了，其他订阅者在OnEnable里面订阅，Publish的时间早于订阅的时间，需要添加粘性重放，Fixed 奔跑速度scrollbar绑定错，Fixed BGM问题，需要将audioMixer的方法放到Start

2025/9/21 - 之前两个场景切换的时候会出现Display 1 No camera rendering，因为场景1直接unload了而出来场景2，中间出现短暂的时间场景2还没有加载出来，场景中没有有效帧，需要写TransitionMgr，现在的逻辑是先进入黑屏，在纯黑屏的时候load新场景，等待新场景加载好后disable掉旧场景，会带来问题在黑屏的时候新场景和旧场景同时存在，场景出现了多个EventSystem

2025/9/22 - 在扩展Mgr的时候发现，继承Mono的单例类需要手动在子类中调用一次父类的ensureCreated函数，在singletonMB里面加入了一个启动器，确保饿汉式加载，从而不需要在子类中手动调用一次父类的函数，同时Fix了EventSystem的问题，删掉场景中的，挂在InputManger上

2025/9/23 - Fixed: keyRebind的时候禁止修改方向键，需要加入人物交互的逻辑，同时在交互的过程中人物禁止移动，需要重构原来的代码，用一个playerState管理，解耦出playerModel

2025/9/25 - Fixed: 更新playerCtl时中间的一个object null bug，Fixed：之前Mgr重写过带来PauseMenu的一点问题，正在写timeline，并且原来的dialog部分可以解耦，打算写一个Base类，估计要写几天了，还要在改键中能改TypeSpeed

2025/9/26 - 完成 DialogCtlBase类，处理不同场景需要用到dialog打字机但是服用代码的场景；在Base处“对话面板自下而上浮入”动画；并且把TypeSpeed 设置为可修改；

2025/9/27 - 写了菜单的恢复默认，感觉还是需要用虚拟相机控制，下载了cinemachine插件，修改了人物移动的bug，把rb的插值设为None了，正在写timeline，加了signal，每次发signal切换动画/行为/打字，人物和Anim加了可以用代码控制，在做过场动画，Fixed：文字框渐变动画的问题

2025/9/28 - 统一游戏内可随时 Pause；并在Pause 后停止 InputManager 的自定义输入（不影响菜单鼠键）；修复向下小箭头位置偏移；新增表情气泡（EmoteBubble）

2025/9/30 - 修复“狂点鼠标在面板加载时吞掉第一句”问题；加入 `_isEntering` 锁，面板加载完成后才允许点击直接展示文字，修复了普通类反复订阅EventBus的问题，EventBus内部加入自动去重，修复了忘记关掉对话小箭头的问题，修复摄像头会摄到贴图边界问题，加入了摄像头的boundaries

2025/10/3 - 做了任务开始弹出来的动画，并且加了GuideDialogCtl，让任务显示的文字也同正常的打字机一样显示

2025/10/4 - 重构代码，将director是否播放移动到了Scene1ArrivalCtl里面，决定将这个Ctl作为Guide控制器，用state控制当前状态，进入board时加入短暂的暂停

2025/10/5 - 继续重构代码，把dialog进来的不同的入场，有的用代码控制有的用动画机控制放进了EnterAnim，在dialogctl处委托EnterAnim执行Anim播放行为，改了下panel的进场动画，写了InteractDialogCtl，复用DialogBase的打字机，并且加了如果有人物的反应的话关闭panel，play人物动画+开启第二段打字机，之后自定义行为可以用UnityEvent

2025/10/6 - 完成场景美术，现在需要调整贴图之间的遮挡关系，在Graphics里面设置了custom axis，Y轴在下面的会先渲染，人物的pivot也需要改为在脚下，在interact ctl中加入y offset，从而可以修改interact ray的位置，确保交互射线在人物正中央

2025/10/7 - 修改了遮挡关系，当人物出现在障碍物侧面的时候用placeObstacle脚本，优化遮挡关系，并且写了一个批量import multiply的图像不改变相对位置到游戏中的脚本

2025/10/9 - 使用sprite平移的方法做了外发光，发光使用一个pureColor的shader，人物脚下阴影也写了一个shadowShader做正片叠底的效果，完成人物像素图像，重新调整了下Timeline动画节奏

2025/10/10 - 稍微搭了下场景，并且把当前的scene1ctl的进度进入到OpenMenu了，正在重构菜单，Fixed：当加入动画后快速打开会有问题

2025/10/11 - 在设置菜单中加入了分辨率和屏幕大小修改支持

2025/10/12 - 把回调事件放到interactModel里面了，isImportant, isTalked放到interactCtl，现在interactCtl支持NthInteract，并且展示第n次的对话，做了背包持久化，和InventoryMgr管理背包，在PauseMenu里面做背包展示，先将简单的硬币item加入到了背包

2025/10/14 - 修了下interactCtl射线问题，并且把interactctl结束后可以移动人物，播放emote的回调放在了如果有inventory事件，先执行inventory事件，再做interactctl的回调，改了下之前在dialog上做的Eact，改成用UnityAction进行回调，并且rebind modal也出现了奇怪的转义，\\\\n和\n，修了一下，现在pausepanel是实时的中英文切换，实现了出现暂停菜单时允许语言刷新，修了一下暂停菜单localizationKey报空的问题

2025/10/15 - 在inventoryMgr里面加入了物品查询和扣除的方法，正在搭售货机行为，加了choicePanel

2025/10/16 - 搭完售货机行为了



正在做：

TODO: 搭场景

TODO:推进剧情



TODO: timeline+任务系统(还没写完)

TODO: 重构菜单，加入“现状确认”的风格，视觉上像一张学生手册的内页

- **标题**：`[ 今日待办 ]` （用学生最熟悉的便签形式）
- **内容**：
  - `- 安全抵达天台 √`
  - `- 搞清楚这个天台到底有什么特别。`
  - `- （上次来的时候……好像发生过什么？记忆很模糊……）`
