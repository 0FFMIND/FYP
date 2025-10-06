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

2025/10/5 - 继续重构代码，把dialog进来的不同的入场，有的用代码控制有的用动画机控制放进了EnterAnim，在dialogctl处委托EnterAnim执行Anim播放行为，改了下panel的进场动画

TODO: 交互时出现外发光

TODO: 打算加一个文字出来的时候稍微抖一下的效果(不知道需不需要)，出现暂停菜单的时候需要允许语言刷新(没写)

TODO: timeline+任务系统(还没写)

TODO: 暂停菜单允许/禁止全屏，调整分辨率

TODO: 重构菜单，需要一级菜单，二级菜单，加入背包等

我在做rpg游戏，请你客观地描述，我需要10版不同但是润色过的语气，简洁

座椅：

看上去无人打理的长椅，它的椅脚以及周围的地面都长出了青苔

公告栏：

一个边角的地方有些磨坏但看起来十分结实的铁制公告栏

上面的字迹被淡淡青苔遮住，但依旧可以辨认：

为保安全，请勿倚靠或攀爬防护栏

自言自语：

这里特意挂了牌子提醒，看来这栏杆确实靠不住

我可不想因为自己的好奇心，明天就登上校园新闻头条

还是别给保洁阿姨添麻烦了

房门：

一道通往楼下的铁门，门旁的墙上挂着正在运转的空调外机。仔细看去，铁门的门轴上卡着一块新放的扁石头

自言自语：

还是不要动这扇门比较好......要是把石头弄掉了，真不敢想象要是一阵风刮过来，把这铁门给合上了，后果得多麻烦

右边的盆栽：

并排放着的三盆植物，最左侧一盆的叶片呈现出健康的翠绿色。中间与右侧盆栽的叶片则显得薄而发黄

自言自语：

右边这两盆，叶子是黄的，杆子是紫色的。是品种就长这样，还是生病了？感觉没左边那盆绿的看着健康，原来植物也会缺钙，感觉右边的这几盆一直被左边这盆挡着，根本晒不到太阳啊







松动的天台地砖 物品描述： 脚下有一块地砖明显松动，踩上去会发出“咯噔”声。

被遗弃的盆栽 物品描述： 一个破损的陶土花盆，里面只剩干裂的泥土和几根枯死的植物根茎。 交互反应： “谁会在天台种花？估计是哪个社团失败的计划吧。完全枯死了，和我们班窗台那盆一模一样……果然，没有什么是能在这里活下来的。”

还在插电的售货机

石头



