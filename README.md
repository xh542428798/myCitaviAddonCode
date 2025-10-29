# myCitaviAddonCode
This is my Citavi 6.8 addons repo

each folder is one single addon, including code and VS 2022 project file.

Any problem can contact me with QQ(3060191344) or email csuxiehui@outlook.com.

## CopyLocationPathToClipboard

一个简单的 Citavi 插件，用于快速将一个或多个附件的本地文件路径复制到剪贴板。

功能
当你在 Citavi 中管理参考文献的附件时，经常需要获取文件的本地路径用于其他操作（例如，在文件管理器中打开、在命令行中使用、或在其他程序中引用）。这个插件提供了一个便捷的“一键复制”功能。

一键复制：在附件的右键菜单中添加了一个新按钮。
多选支持：可以同时选中多个附件，一次性将所有路径复制到剪贴板，每个路径占一行。
路径格式化：复制的路径会被双引号（"）括起来，方便在命令行等环境中直接使用。
使用方法
在 Citavi 的参考文献视图中，右键点击一个或多个电子附件。
在弹出的上下文菜单中，选择 “将文件路径复制到剪贴板”。
文件的完整本地路径就会被复制到你的系统剪贴板中，你可以直接粘贴到任何地方。

## KnowledgePanelShowPDF
一个强大的 Citavi 插件，旨在打通知识项目与 PDF 预览之间的壁垒，实现精准、即时的导航和高亮。

**功能**
你是否厌倦了在 Citavi 的知识项目列表和 PDF 预览窗口之间来回切换，手动寻找对应的注释位置？这个插件解决了这个痛点，它提供了以下核心功能：

**一键跳转**：在知识组织器中右键点击任何知识项目，选择“在当前视图的右边栏预览PDF”，即可立即在右侧预览面板中打开并跳转到该知识项目对应的 PDF 位置。
实时联动：
在知识组织器中点击不同的知识项目，右侧的 PDF 预览会自动跳转到相应位置。
在参考文献编辑器的引文列表中点击不同的引文，右侧的 PDF 预览也会自动跳转。
智能预览：当点击的知识项目关联的 PDF 尚未在预览中打开时，插件会自动在预览面板中加载它。

**使用方法**
手动跳转：
在知识组织器中，右键点击一个知识项目。
在弹出的菜单中选择 “在当前视图的右边栏预览PDF”。

**自动联动**：
知识组织器：只需在列表中单击不同的知识项目，右侧的 PDF 就会自动跟随跳转。
参考文献编辑器：在引文列表中单击不同的引文，右侧的 PDF 也会自动跟随跳转。
**工作原理**
该插件通过订阅 Citavi 内部控件的列表项变更事件（ActiveListItemChanged）来实现实时联动。当用户在列表中切换选择时，插件会捕获这个事件，并执行以下操作：
获取当前选中的知识项目或引文。
查找与之关联的 PDF 注释（Annotation）。
调用 Citavi 的 PDF 预览控件，使其直接跳转到该注释的位置。

## TextMarkerGrayWithoutKnowledge
一个轻量级的 Citavi 插件，允许你在 PDF 中创建临时的、不会保存为知识项目的高亮标记。

**功能**
在阅读和批注 PDF 时，我们有时只想用颜色标记一下重点，但并不想将其作为正式的知识项目保存到 Citavi 数据库中。这个插件就是为了满足这个需求而生的。

**临时高亮**：在 PDF 中选中文本后，点击插件按钮或使用快捷键 B，即可创建一个灰色的高亮标记。
非知识项目：创建的高亮不会出现在你的知识组织器中，也不会与任何参考文献关联，它纯粹是 PDF 文件上的一个视觉标记。
**智能显示**：插件按钮只在 PDF 预览窗口且选中的是文本时才显示，保持界面整洁。
**快捷键支持**：使用快捷键 B 可以快速创建临时高亮，提升阅读效率。
**使用方法**
在 Citavi 的 PDF 预览窗口中，用鼠标选中你想要标记的文本。
执行以下任一操作：
点击预览工具栏中的 “用灰色高亮但不保存Knowledge” 按钮。
直接按下键盘快捷键 B。
选中的文本将被标记为灰色高亮。这个标记仅用于视觉提示，不会被同步到 Citavi 的知识库中。

**适用场景**
临时阅读：在初次浏览文献时，快速标记感兴趣的部分，稍后决定是否要创建正式的知识项目。
分类标记：用不同颜色（配合其他高亮工具）区分不同类型的信息，而不污染知识库。
草稿批注：在整理思路时，随意标记，无需担心产生冗余的知识项目。


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

