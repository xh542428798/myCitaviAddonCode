# myCitaviAddonCode
This is my Citavi 6.8 addons repo

each folder is one single addon, including code and VS 2022 project file.

Any problem can contact me with QQ(3060191344) or email csuxiehui@outlook.com.

## CopyTextOfSelectedAnnotationAddon

**一键复制批注文本**
在Citavi的PDF预览中选中任意知识条目（高亮或下划线）。
点击工具栏“工具”菜单中的 “CopyTextOfSelectedAnnotation” 按钮，或使用快捷键 Shift + C。
该批注的完整文本内容将被精确提取并复制到剪贴板。

**一键生成Obsidian链接**
在Citavi的PDF预览中选中一个或多个知识条目。
点击工具栏“工具”菜单中的 “CopyFormattedLink” 按钮，或使用快捷键 Shift + Ctrl + O。
插件会自动生成一个格式化的Obsidian Markdown链接并复制到剪贴板，格式如下：

智能启用：插件按钮仅当你在PDF预览模式下且选中了支持的知识条目（如知识条目、任务项）时才会被激活，避免误操作。
路径解析：生成Obsidian链接时，插件会自动解析PDF在你的电脑上的绝对路径，并转换为Obsidian兼容的 file:/// 格式。
唯一标识：生成的链接末尾包含 KnowID，这是Citavi中知识条目的唯一ID，方便你未来进行更高级的自动化管理。
依赖环境：本插件需要Citavi环境正常运行，并且关联的PDF文件路径在Citavi项目中是有效的。

## SearchInActiveGroup

Advanced search in active group or category, it is just open advanced search form and fill the group/category name automaticly.

在活动的群组或分类中进行高级搜索，就是打开高级搜索界面并自动填写群组/分类名称。

### SearchInActiveCategory

![SearchInActiveCategory](vx_images/SearchInActiveCategory.gif)

### SearchInActiveGroup

![SearchInActiveGroup](vx_images/SearchInActiveGroup.gif)

## ShowReferenceFromSearchForm

Show selected reference of search form in main form。
在主界面中显示搜索界面中选择的参考文献。

![ShowReferenceFromSearchForm](vx_images/ShowReferenceFromSearchForm.gif)

## BatchModifyOfSearchForm

1. 在主界面中通过设置过滤器，在文献窗格显示搜索框中选中的文献。

2. 在主界面中通过一键按钮，对搜索框中选中的文献进行批量Category和Group赋予和删除操作

![image-20231104092440562](vx_images/image-20231104092440562.png)

## ShowReferenceFromTableView

Show active reference of table view in main form
在主界面中显示table视图界面中选择的参考文献。

![ShowReferenceFromTableView](vx_images/ShowReferenceFromTableView.gif)



## ScrollSpeedInPdfPreview

Fix the official bug
修复官方Bug
![ScrollSpeedInPdfPreview](vx_images/ScrollSpeedInPdfPreview.gif)

## DuplicateComparing

- [ ] **Done**, compare duplicates like endnote
- [ ] **完成**, 像endnote一样对比重复文件

效果如下：
![](vx_images/42724170397680.png)
![](vx_images/32762280735194.png)


## PDFThumbnailAddon
**使用缩略图导航：**
在Citavi中打开任意PDF文件。
在右侧的侧边栏中，点击新增的缩略图图标（通常是一个图片样式的图标）。
在打开的“缩略图”面板中，浏览页面，点击你想要跳转的页面缩略图。
主预览区域将立即跳转到该页。

**使用目录展开/收起功能：**
在侧边栏中切换到“书签”（目录）选项卡。
在目录列表的顶部，你会看到“▼ 全部展开”按钮。
点击它，整个目录将完全展开。按钮文字会变为“▲ 全部收起”。
再次点击，目录将收起，只显示顶级标题。

## PDFThumbnailGlm-4-flash
**实时划词翻译**
在Citavi的PDF预览界面中，用鼠标选中任意单词、句子或段落。
插件会自动将选中的文本捕获到侧边栏的“翻译”面板中。
点击“翻译”按钮，即可调用翻译API（默认配置为GLM模型）将文本翻译成中文。
翻译结果会清晰地显示在下方的“译文”区域。

**智能配置管理**
插件支持自定义API密钥和翻译提示词（Prompt），以适应不同领域的翻译需求（如医学、法律、计算机等）。
配置信息会自动保存在Citavi启动目录下的 TranslationConfig.json 文件中，下次启动时自动加载。
提供了“自动翻译”选项，开启后，选中文本将自动触发翻译，无需手动点击。

**目录一键展开/收起**
继承了前一个版本的实用功能，在PDF的“书签”（目录）选项卡顶部，提供“▼ 全部展开” / “▲ 全部收起” 按钮。
一键控制整个目录树的展开状态，让你在复杂的文献结构中快速导航。

## SwitchToMainWindow

Switch back to MainWindow quickly from TableView.

从TableView快速切换回MainWindow

## SwitchToTableView

Switch to TableView quickly from MainWindow.

从MainWindow快速切换到TableView

## ReferenceGridFormWorkSpaceEditor

This add-on lets you save your selection of columns and groups as a work area.

This is the modified addon from official. I add the menu to the tool bar, it would be more convenient.

这个插件允许您将列和分组的选择保存为一个工作区。

这是官方版本经过我修改的插件。我在工具栏上添加了菜单，这样更加方便。

Note: 将插件文件拷贝到 Addon folder, 没有二级文件夹.

![image-20231102232340401](vx_images/image-20231102232340401.png) 

 





## TranslateGPTInCitavi
> 删除，项目放弃

Open a chatGPT website, communicating and exchanging data between Citavi and web pages, so that we can batch translate Title and Abstract in Citavi, and ask chatGPT quicker. 
(Note: This method simulates browser access to the ChatGPT mirror website, which may be shut down at any time, requiring frequent adjustments.)

打开一个ChatGPT网站，实现Citavi和网页之间的通讯和数据交换，以便我们可以批量翻译Citavi中的标题和摘要，或者更快地向ChatGPT提问。
（注意：该方法模拟浏览器访问ChatGPT镜像网站，该网站可能随时关闭，需要经常进行调整。）

**Requirements**: 
Selenium.RC.3.1.0
Selenium.Support.4.14.1
Selenium.WebDriver.MSEdgeDriver.118.0.2088.41 -> Edge 118.0.2088.xx (x64)
Selenium.WebDriver.4.14.1
System.Drawing.Common.7.0.0; Newtonsoft.Json.13.0.1 (Install automaticly by Selenium)

![ScrollSpeedInPdfPreview](vx_images/ScrollSpeedInPdfPreview-1700828658584-1.gif)

## JumpToLastPositionAfterActionExecutionAddon
个附加功能在Citavi的PDF阅读器的工具菜单中添加了命令“跳转到上一个位置...”，快捷键为ALT+F3。如果文档中包含内部链接，并且通过点击内部链接进行了跳转，该命令就会生效。在执行跳转之前的最后位置会被保存，只要文档保持打开状态，这个位置就会一直被保存。

适配 Citavi 6.18

## SortKnowledgeItemsInSelectionAndCreateSubheadings
对Knowledge里选中的知识点按文献、PDF页码这两个方面进行排序，然后添加在每个文献的Knowledge前面添加一个subheading

## MacroManagerWithAutoRef
MacroManager插件加入AutoRef插件的功能，在MacroManager的edit和run运行的时候能够自动加载引用。
> 原版AutoRef只有先打开macros editor，然后open一个新的宏文件，才会自动加载引用，相对鸡肋。

