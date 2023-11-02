# myCitaviAddonCode
 This is my Citavi 6.8 addons repo

each folder is one single addon, including code and VS 2022 project file.



## SearchInActiveGroup

Advanced search in active group or category, it is just open advanced search form and fill the group/category name automaticly.

### SearchInActiveCategory

![SearchInActiveCategory](vx_images/SearchInActiveCategory.gif)

### SearchInActiveGroup

![SearchInActiveGroup](vx_images/SearchInActiveGroup.gif)

## ShowReferenceFromSearchForm

Show selected reference of search form in main form

![ShowReferenceFromSearchForm](vx_images/ShowReferenceFromSearchForm.gif)

## ShowReferenceFromTableView

Show active reference of table view in main form

![ShowReferenceFromTableView](vx_images/ShowReferenceFromTableView.gif)



## ScrollSpeedInPdfPreview

Fix the official bug

![ScrollSpeedInPdfPreview](vx_images/ScrollSpeedInPdfPreview.gif)

## DuplicateComparing

- [ ] **In processing**, compare duplicates like endnote

![img](vx_images/v2-54b25fe6df839ec8904d0261459347f6_r.jpg)



## SwitchToMainWindow

Switch back to MainWindow quickly from TableView.

## SwitchToTableView

Switch to TableView quickly from MainWindow.

## ReferenceGridFormWorkSpaceEditor

This add-on lets you save your selection of columns and groups as a work area.

This is the modified addon from official. I add the menu to the tool bar, it would be more convenient. 

Note: Copy all files to the Addon folder, no subfolder.

![image-20231102232340401](vx_images/image-20231102232340401.png) 

## TranslateGPTInCitavi

Open a chatGPT website, communicating and exchanging data between Citavi and web pages, so that we can batch translate Title and Abstract in Citavi, and ask chatGPT quicker. 

(Note: This method simulates browser access to the ChatGPT mirror website, which may be shut down at any time, requiring frequent adjustments.)

**Requirements**: 

Selenium.RC.3.1.0
Selenium.Support.4.14.1
Selenium.WebDriver.MSEdgeDriver.118.0.2088.41 -> Edge 118.0.2088.xx (x64)
Selenium.WebDriver.4.14.1
System.Drawing.Common.7.0.0; Newtonsoft.Json.13.0.1 (Install automaticly by Selenium)



