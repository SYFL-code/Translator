[h3] 描述 [/h3]
由于原作者迟迟没有更新[url=https://steamcommunity.com/sharedfiles/filedetails/?id=3759456473] 模组 [/url]，而制作的模组。其增加更改了一些内容。

This is just a mod that makes it easy for Chinese players to add Chinese names and descriptions (i.e., add notes) to mods.

[h3] 功能概述 [/h3]
你可使用此模组手动为没有中文翻译的模组添加你自己翻译的中文。
现在可以让整个模组列表的模组全部都是中文了！

界面支持多语言；翻译文件中的译文语言不限（中、英、德、法等均可）。
只支持修改模组名称与简介，这不是一个模组字段翻译工具。

[h3] 使用方法 [/h3]
1.快捷按钮：点击模组预览界面右上方的重命名按钮。在打开文件中，按提示将你需要替换的名称和描述输入进去，保存关闭。（不需要特定格式，记得换行就好，以及不能出现字符：’|‘）回到游戏，再次点击重命名按钮确认修改。这时你应该能看见界面上的模组名称和描述已经被替换成了你在文本文件中输入的内容。
2.批量翻译：在此模组的模组设置页中，点击批量翻译。在打开文件中，按提示批量翻译（可以同时显示原文和译文）。编辑完后保存关闭，回到模组设置页点击“应用全部”。
3.手动索引：在此模组的模组设置页中，手动输入模组id，点击输入替换文本。在打开文件中，按提示输入，保存关闭。点击确认替换。
4.复制文件：如果你想的话，可以复制更改其他人的翻译文件到 C:\Users\你的用户名\AppData\LocalLow/Videocult/Rain World/ModConfigs\ModRename_stringsSave.txt，[url=https://github.com/SYFL-code/Translator/blob/main/mod/text/text_chi/strings.txt] 这里 [/url]是我的翻译文件。

[h3] 实现原理 [/h3]
通过游戏内翻译器以“翻译”的方式修改模组应该显示的名称与描述，未对模组本身进行修改。
全部的用户自定义替换全部都保存在 AppData/LocalLow/Videocult/Rain World/ModConfigs 这个路径下的ModRename_stringsSave.txt 文件文件中。

移除此模组可复原所有更改。

如果你需要确保通过此模组添加的替换优先级高于其他翻译模组，请将此模组排序置于其他翻译模组之上。
反之，如果您只是临时使用此模组补充如 模组翻译 模组尚未涵盖的模组名，可以使此模组较先加载（至于列表中其他翻译模组下方）因翻译索引的原因，游戏将会采用加载最晚的那一个。

[h3] 更新 [/h3]
1.0.0 修复了修改消失、手动索引无法翻译等的问题

[h3] 声明 [/h3]
源代码：https://github.com/SYFL-code/Translator
本模组未经原模组作者授权制作。如果您是原模组的作者，并希望您的模组从支持列表中移除，请联系我，我会立即处理。

[h3] English [/h3]
This mod makes it easy for players to add translated names and descriptions (or notes) to installed mods. It does not modify the mods themselves; it uses Rain World's built-in translator, so removing this mod restores the original names and descriptions.

[h3] Features [/h3]
- Add your own translations to mods that lack them.
- Batch export/import every installed mod's name and description.
- Manual mod-ID input for single mods.
- The mod UI is available in multiple languages.

[h3] How to use [/h3]
1. Quick rename: In the mod preview page, click the rename button in the top-right corner. Edit the opened temp file: the first line is the name, the remaining lines are the description. Save and close it, then click the rename button again to apply.
2. Batch translate: Open this mod's Remix settings page and click "Batch translate". Edit the exported file (it shows the original text and the current translation), save and close it, then click "Apply all".
3. Manual index: In the mod settings page, enter the target mod ID, click "Input replacement text", edit the opened temp file, save and close it, then click "Confirm replacement".
4. Copy a translation file: You can also copy a translation file to C:\Users\<YourUserName>\AppData\LocalLow\Videocult\Rain World\ModConfigs\ModRename_stringsSave.txt.

[h3] Notes [/h3]
- Lines starting with '#' are ignored when applying temp files.
- The character '|' cannot be used in the text you enter.
- All user replacements are saved in AppData/LocalLow/Videocult/Rain World/ModConfigs/ModRename_stringsSave.txt.
- To make this mod's replacements take priority over other translation mods, place this mod above them in the mod list. If you only want to fill gaps, place this mod below them (the last loaded translation wins).
