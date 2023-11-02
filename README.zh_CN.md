# myCitaviAddonCode
This is my Citavi 6.8 addons repo

each folder is one single addon, including code and VS 2022 project file.



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

## ShowReferenceFromTableView

Show active reference of table view in main form
在主界面中显示table视图界面中选择的参考文献。

![ShowReferenceFromTableView](vx_images/ShowReferenceFromTableView.gif)



## ScrollSpeedInPdfPreview

Fix the official bug
修复官方Bug
![ScrollSpeedInPdfPreview](vx_images/ScrollSpeedInPdfPreview.gif)

## DuplicateComparing

- [ ] **In processing**, compare duplicates like endnote
- [ ] **进行中**, 像endnote一样对比重复文件
![img](vx_images/v2-54b25fe6df839ec8904d0261459347f6_r.jpg)

## TranslateGPTInCitavi

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



