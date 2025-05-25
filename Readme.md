致谢名单：GPTo4-mini Audacity(变调/调整音量) Procreate

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

已完成：TD:每次对话下面有个小箭头，需要修改(我也画不动但是GPT的也不好看)：开头panel，背景

2025/5/23 - 已完成：TD: 加一个提示的声音，修改warning的UI panel

2025/5/24 - 已完成：背景和脚本重新修改，对话触发事件

TD：背景的那个框，



TD:分支对话/能够跳过显示 <-这个也不难

TD:改键

TD:暂停菜单

