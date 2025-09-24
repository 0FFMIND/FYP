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

2025/9/25 - 之前两个场景切换的时候会出现Display 1 No camera rendering，因为场景1直接unload了而出来场景2，中间出现短暂的时间场景2还没有加载出来，场景中没有有效帧，需要写TransitionMgr，现在的逻辑是先进入黑屏，在纯黑屏的时候load新场景，等待新场景加载好后disable掉旧场景，会带来问题在黑屏的时候新场景和旧场景同时存在，场景出现了多个EventSystem

2025/9/26 - 在扩展Mgr的时候发现，继承Mono的单例类需要手动在子类中调用一次父类的ensureCreated函数，在singletonMB里面加入了一个启动器，确保饿汉式加载，从而不需要在子类中手动调用一次父类的函数，同时Fix了EventSystem的问题，删掉场景中的，挂在InputManger上

2025/9/27 - Fixed: keyRebind的时候禁止修改方向键，需要加入人物交互的逻辑，同时在交互的过程中人物禁止移动，需要重构原来的代码，用一个playerState管理，解耦出playerModel

TODO: 交互时出现光标

TODO: 打算加一个文字出来的时候稍微抖一下的效果，出现暂停菜单的时候需要阻止鼠标onDialogClick，出现暂停菜单的时候需要允许语言刷新，还有一种是禁止在对话的时候打开菜单

TODO: timeline(还没写)，任务系统

