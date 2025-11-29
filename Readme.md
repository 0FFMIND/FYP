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

2025/10/22 - 画完了1-2的过场图，在进入过场动画时候的画面黑屏->切换camera用的回调函数写，有点在硬编码，修了打字机实时中英文切换在index == 0出现问题的BUG，顺便完善了一下handout - doc

2025/10/23 - 写了一下之后剧情的文本，把场景中的物体搭好了，需要完成日记系统

2025/10/24 - 正在写日记系统

2025/10/25 - 写完了日记系统

2025/10/27 - 修了一下跨场景的pausePanel出现的问题，现在场景也不是additive的load，而是正常的切换，因为没有叠加场景的需求

2025/10/29 - 修了一下pauseMenu的侧边栏，现在侧边栏改成互斥的了，通过toggle组实现

2025/10/30 - 遇见metalSign，加入了人物稍微跳一下，此时rigidbody改为kinematic

2025/11/1 - 修复了原来dialog加载中英文的问题，如果一个dialogctl一直出现在场景中，因为当它end()的时候会自动退订事件，那么重入startDialog不会重新订阅事件，onEnable和onDisable的时候会重新订阅/退订事件，写了一个新的shader，写完了菜单引导，holecolor，能出现遮罩的效果，写完了探索场景中的三个物体，进入到gotomeadow的状态

2025/11/2 - 做了一个很简单的开始菜单，修复了restartGame里面journal没法刷新到初始都为Hidden + Pending的值，因为在Setting里面把JournalSaveData置为empty，之后save，当游戏/Unity初始读取的时候，会使用JournalSaveAdapter，尝试解析JournalSaveData，当JournalSaveData.有效length == 0的时候，不会进入for循环，加了n == 0的特殊判断，写了cameraShake, cameraZoom的方法，提供给代码进行过场动画的摄像机行为

2025/11/6 - 之前做guidepanel的时候，总觉得中间被掏空了，实际上周围暗了中间没暗显出来中间特别亮，实际上shader没有任何问题，新画了一个表情，player-ashamed，加了buttonClick的UI音效，PauseMenuButton操作加了音效

2025/11/7 - 做完chapter1了，需要修复动画waitforrealtime的问题

2025/11/11 - 补完Chapter1的贴图，Chapter1等待润色，正在写Chapter2的剧本，稍微修改了一下英文翻译的问题

2025/11/12 - 稍微润色了一下Chapter1的剧本，还需要继续写

2025/11/19 - 把Chapter1的剧本写出来了，要写Chapter2

2025/11/20 - Chapter2的大概剧情已经写出来了，Chapter1也润色完成了，稍微让文字整体变得口语化一点了

2025/11/21 - 修改玩家的反馈，把默认加速方式设为了leftShift，并且着重修改了camera Follow的代码，https://github.com/0FFMIND/FYP/blob/b62b6f0ae290ae1485936e34290d329c17c81c6b/Assets/Scripts/1-Scene/MVC/Controller/CameraFollow.cs
以前是camera Follow是一个上帝类，什么都要做，因为当时midterm比较匆忙，只考虑了游戏正常运行，现在修改为一个cameraCtl，把follow, move, shake单独分为一个脚本，ctl作为中心控制器，提供对外的API，而具体行为通过委托调用脚本https://github.com/0FFMIND/FYP/tree/0f278d7624d96c501d05603fecb5fb476dd03f5e/Assets/Scripts/1-Scene/MVC/Controller/Camera

2025/11/22 - 修复了开头switch language的时候窗口分辨率txt没有switch的问题（key少了初始占位值），摄像机有时候抖动不出来，不知道是什么原因，把在Update的抖动偏移量换到LateUpdate里面了，可能是相机在Update里面有其他行为覆盖了

2025/11/23 - 重构代码，https://github.com/0FFMIND/FYP/blob/0f278d7624d96c501d05603fecb5fb476dd03f5e/Assets/Scripts/1-Scene/MVC/Controller/Scene1ArrivalCtl.cs
这里的问题是FSM，当管理的状态变多变得难以维护，修改后的代码：https://github.com/0FFMIND/FYP/tree/main/Assets/Scripts/1-Scene/MVC/Controller/Phase

2025/11/24 - 把代码迁移到VSCode上了，VSCode可以和Github集成，并且使用免费的Copilot自动生成的commit message，从而让commit更有信息，继续重构代码，修改了guidePanel仍然提示右shift move faster的问题，现在统一左shift移动，修复了meadow结束后的panTo位置问题，统一了camera的锚点，修复了之前Camera抖动不出来的问题，是因为follow也在update，覆盖了shake的update，修复了父camera brain和子类vcam的关系，现在人物移动的时候也可以抖动了

2025/11/25 - 修复了在meadow结束后播放铃声结束的时候动画推进会卡住的问题，是因为跑操的音乐有20MB，并且读取方式是一次读取并且解压缩，会阻塞U3D的主线程，把音频的加载改为streaming，流式加载解决问题，正在重构：https://github.com/0FFMIND/FYP/blob/b620ad88bab23c954b6f1b4693e440577328b3d8/Assets/Scripts/1-Scene/MVC/Controller/Dialog/DialogCtlBase.cs
首先删掉了无用代码，原来DialogCtlBase是God类，也会初始化arrow的行为，因此首先把arrow移到了单独的ArrowIndicator类里面，待进一步重构

2025/11/26 - 把之前逐行打字/揭露的过程移动到了新类TypeWriter，进一步把Dialog的God类解耦，之后把Arrow和TypeWriter还有DialogueView移到了DialogRenderer类里面，现在DialogCtl控制事件接收，DialogModel的储存和DialogRenderer的调用，并且移除了很多无用local变量，把每个对话对应的mapping移到派生类了，修复后的代码：https://github.com/0FFMIND/FYP/tree/5f5b96d7e6d590f395233080458973138619673b/Assets/Scripts/1-Scene/MVC/Controller/Dialog/Base

2025/11/27 - 正在重构：https://github.com/0FFMIND/FYP/blob/2707e79decb851b6877228a248f3e73b591f16ab/Assets/Scripts/1-Scene/MVC/Controller/Dialog/TimelineDialogCtl.cs，首先把之前所有的1234这种调用的逻辑统一成一个可复用函数StartDialogue(Scene1DialogueId id, Action onFinished)，并且用枚举的id增强代码的可读性，其次把Close()和CloseAndFinished()合成了一个代码，对话机结束的时候一定会调用finished事件，如果有特殊要求应该是在调用者地方实现

2025/11/28 - 继续修改TimelineDialogCtl，之前的枚举是Scene1DialogueId但是考虑到拓展，用了基类DialogueClipBase，一定会有mapping和text，拓展类加上各个不同场景的枚举id，现在方法签名StartDialogue(int clipId, Action onFinished)，让它更通用了，其次把EnterAnim的函数名改为易懂的，PlayEnterCode -> PlayScriptedEnterAnim,PlayExitCode->PlayScriptedExitAnim，也修改了TimelineDialogCtl的函数名，EnterAnim有更好的策略模式，但是不影响，它只用来播放我的panel入场和退场动画，也不需要之后的拓展，不需要细致重构





TODO: 重构

https://github.com/0FFMIND/FYP/blob/7c3ae58bbc6c53a396f874b1477fcf99a3d0b169/Assets/Scripts/1-Scene/Timeline/Scripts/PlayerMoveSignal.cs



【Chapter1的问题】

TODO: 我觉得打√只存在于这个物体的所有交互都做完，当前打√的逻辑只是说明这个物体被至少交互过一次，不然给玩家的指引不够明显

TODO: 需要修复动画waitforrealtime的问题

TODO: 动画离开的时候需要最后关一下门，门变黑，再走进去

TODO: 写drain行为

TODO: 修复重进scene的问题

这里可以插入图片

上上周的周三，我趁体育课自由活动的时间溜上来过
那天早上路过校门口的花店，老板正把过季的铃兰种球往垃圾袋里倒，圆溜溜的种球滚了一地
我这手欠的，实在是没忍住，就捡了一颗最饱满的
听店员在旁边嘀咕，说这些种球存放太久，都失去活性了
他越是这么说，我越是想试试
说实在的，我对养花种草完全不懂，完全是瞎猫碰上死耗子，胡乱往草甸里一埋
想着这么多天过去，说不定真有奇迹发生呢……

【设计Chapter2的问题】：

TODO: Chapter1的末尾

TODO：加入第二章后选章的时候可以加入我的STORY.md里面的内容，加入概要

TODO: 我有一个想法，说话的时候可以出现多个关键词，下面出现[] [] [] [] 很多个关键词框，上面是当前对话的人物，然后丢给当前对话的人物类似辩论环节进行剧情推进



TODO: 需要在售货机cg出现完后加一个判断的bool或者是什么，然后在选关的时候如果直接从ch2开始，基础的日记，然后判断有没有这个bool，有的话那就加一个售货机的日记

TODO: 想标题，潮汐将至时？潮汐可以改名叫寂静？沉眠?沉寂？永眠这种？还有落叶归根，落叶待归根，终将/需这种

要做什么？

勾取 ->

你将硬币成功地勾近了一点

你发力过猛，硬币从指尖弹开，滚向了远处

成功了！

你小心地将硬币勾到最近处，再稳稳捡起

离开



