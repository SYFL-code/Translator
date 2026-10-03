[h3] 描述 [/h3]
由于原作者迟迟未更新 [url=https://steamcommunity.com/sharedfiles/filedetails/?id=3759456473 ] 原模组 [/url]，
所以制作了此版本，增加和调整了部分内容。

This is a mod that makes it easy for players to add translated names and descriptions
(or notes) to installed mods. It does not modify the mods themselves; it uses Rain World's
built-in translator, so removing this mod restores the original names and descriptions.
Language：中 | EN | FR | DE | IT | 日本語 | 한국어 | PT | РУ | ES

[h3] 功能 [/h3]
- 手动为任意模组添加自定义名称 / 简介
- 批量导出 / 导入所有模组的翻译
- 界面支持 10 种语言（中 | 英 | 法 | 德 | 意 | 日 | 韩 | 葡 | 俄 | 西）
- 仅修改模组**名称与简介**，不是通用字段翻译工具

[h3] 使用方法 [/h3]

[b]1. 快捷按钮（单个模组）[/b]
点击模组预览界面右上方的“重命名”按钮。
在打开的 temp.txt 中按提示编辑：
  · 第一行 = 翻译名称
  · 其余行 = 翻译描述（可用 <LINE> 表示换行）
保存并关闭文件，回到游戏再次点击“重命名”按钮完成应用。
若名称为空，则删除该模组的翻译并回退为原文。

[b]2. 批量翻译（推荐）[/b]
在模组设置页点击“批量翻译”，生成 ModRename_allMods.txt。
文件格式：
    [ModID]
    name=原始名称
    desc=原始描述
    trans_name=翻译名称
    trans_desc=翻译描述
逐项填写 trans_name / trans_desc 后保存关闭，
回到模组设置页点击“应用全部”即可。

[b]3. 手动索引（按 ID 操作）[/b]
在模组设置页输入目标模组 ID，点击“添加翻译”。
按提示编辑 temp.txt，保存关闭，然后点击“确认添加”。

[b]4. 复制他人翻译文件[/b]
可将他人的存档文件复制到：
C:\Users\{你的用户名}\AppData\LocalLow\Videocult\Rain World\ModConfigs\ModTranslatorSave_{语言}.txt
{语言} 可选：Chi / Eng / Fre / Ger / Ita / Jap / Kor / Por / Rus / Spa。

实现原理
通过游戏内翻译器以“翻译”的方式覆盖模组显示的名称与描述，未对模组本身进行任何修改。
移除此模组即可复原所有更改。

用户自定义翻译保存在：
- 运行存档：C:\Users\{你的用户名}\AppData\LocalLow\Videocult\Rain World\ModConfigs\ModTranslatorSave_{语言}.txt
- 模组随包翻译：<模组目录>\text\text_{语言}\strings.txt
- 自动备份：<模组目录>\backup\ModTranslatorSave_{语言}_{时间戳}.txt（最多保留 20 份）
原模组的存档 ly.ModRename_stringsSave.txt 会在首次启动时自动迁移并合并。

加载优先级
- 若需本模组的翻译优先于其他翻译模组：将本模组排在它们之上。
- 若只想补充其他翻译模组尚未涵盖的内容：将本模组排在它们之下。
由于翻译索引机制的限制，游戏将采用最后加载的那个翻译。

源代码：[url=https://github.com/SYFL-code/Translator ] GitHub [/url]