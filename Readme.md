2025/5/6 - 防止忘了，用了DialogueModel存从文本.txt里面读到的话，中间一个方法LoadDialogue会通过LocalizationManager(用一个static存的当前语言)定位到具体路径/文本.txt

通过dialoguectl控制dialoguemodel，直接通过new创建一个并获得控制权，太痛苦了写代码，暂时还没写按键适配，本来想dialogueView加一个渐变效果的，目前只做了一个启动功能

打算写完对话功能，其他的再看吧。。

2025/5/13 - 之前写到DialogueCtl里面了，感觉实际上是PrologueDCtl，里面读取txt其实可以用一个string进行private赋值，但话又说回来了，我会用到这个Ctl很多次吗？不写死的话，我的感觉是我会写死，但是为了FYP要求放在一个字段里面了，还需要一个自定的字段，linemapping，就不需要写死了

问题是感觉文本量有点小，需要多一点的

TD:audiomanager，使用audiomixer

TD:分支对话/能够跳过显示 <-

TD:改键
