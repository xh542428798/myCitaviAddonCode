﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DuplicateComparing;
using Infragistics.Win.UltraWinSchedule;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Shell;
using SwissAcademic.Controls;
//using SwissAcademic.Epub;

namespace DuplicateComparingAddon
{
    public class DuplicateComparing
        :
        CitaviAddOn<MainForm>
    {
        public override void OnHostingFormLoaded(MainForm mainForm)
        {
            mainForm.GetMainCommandbarManager().GetReferenceEditorCommandbar(MainFormReferenceEditorCommandbarId.Menu).GetCommandbarMenu(MainFormReferenceEditorCommandbarMenuId.References).InsertCommandbarButton(14, "Merge duplicates", "Merge the selected references");
            //button.Shortcut = (System.Windows.Forms.Shortcut)(System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.W);
            mainForm.GetReferenceEditorNavigationCommandbarManager().GetCommandbar(MainFormReferenceEditorNavigationCommandbarId.Toolbar).GetCommandbarMenu(MainFormReferenceEditorNavigationCommandbarMenuId.Tools).AddCommandbarButton("Merge duplicates", "Merge the selected references");
        }

        public override void OnBeforePerformingCommand(MainForm mainForm, BeforePerformingCommandEventArgs e)
        {
            SwissAcademic.Citavi.Project activeProject = Program.ActiveProjectShell.Project;
            
            switch (e.Key)
            {
                case "Merge duplicates":
                    {
                        e.Handled = true;
                        List<Reference> references = mainForm.GetSelectedReferences();
                        //string strLeft = "";
                        //string strRight = "";
                        if (references.Count != 2)
                        {
                            MessageBox.Show("Currently this script only supports merging two references. Please select two and try again.");
                        }
                        else
                        {
                            if (references[0].ReferenceType != references[1].ReferenceType)
                            {
                                MessageBox.Show("Currently this script only supports merging two references of the same type. Please convert and try again.");
                            }
                            else
                            {
                                //string originalTitle = references[0].Title;

                                // CreatedOn, check which one is older and then take that CreatedOn and CreatedBy
                                if (DateTime.Compare(references[0].CreatedOn, references[1].CreatedOn) > 0)
                                {
                                    // second reference is older,将referecne1改为更新的,0为更老更重要
                                    Reference newer = references[0];
                                    references[0] = references[1];
                                    references[1] = newer;
                                }
                                // 文本类
                                string result;
                                string strLeft; // = "";
                                string strRight; //= "";
                                string fieldName; // = "";
                                // Title
                                result = MergeOrCombine(mainForm, references[0].Title, references[1].Title);
                                if (result == "cancel"){ break;}
                                else
                                {
                                    references[0].Title = result;
                                }

                                // Abstract
                                result = MergeOrCombine(mainForm, references[0].Abstract.Text, references[1].Abstract.Text);
                                if (result == "cancel") { break; }
                                else
                                {
                                    references[0].Abstract.Text = result;
                                }
                                // AccessDate, take newer one
                                //TODO: accessdate would need to be parsed
                                // right now, we just check if there is one, we take it, otherwise we leave it empty.
                                if (references[0].AccessDate.Length < references[1].AccessDate.Length)
                                {
                                    references[0].AccessDate = references[1].AccessDate;
                                }

                                // Additions
                                result = MergeOrCombine(mainForm, references[0].Additions, references[1].Additions);
                                if (result == "cancel") { break; }
                                else
                                {
                                    references[0].Additions = result;
                                }

                                // CitationKey, check if CitationKeyUpdateType is 0 at one reference if yes, take that one
                                if ((references[0].CitationKeyUpdateType == UpdateType.Automatic) && (references[1].CitationKeyUpdateType == UpdateType.Manual))
                                {
                                    references[0].CitationKey = references[1].CitationKey;
                                    references[0].CitationKeyUpdateType = references[1].CitationKeyUpdateType;
                                }

                                // CoverPath
                                if (references[0].CoverPath.LinkedResourceType == LinkedResourceType.Empty)
                                {
                                    references[0].CoverPath = references[1].CoverPath;
                                }

                                // CustomFields (1-9)
                                for (int i = 1; i <= 9; i++)
                                {
                                    fieldName = "CustomField" + i;
                                    strLeft = references[0].GetType().GetProperty(fieldName).GetValue(references[0]).ToString();
                                    strRight = references[1].GetType().GetProperty(fieldName).GetValue(references[1]).ToString();
                                    result = MergeOrCombine(mainForm, strLeft, strRight);
                                    if (result == "cancel") { break; }
                                    else
                                    {
                                        references[0].GetType().GetProperty(fieldName).SetValue(references[0], result);
                                    }
                                }

                                // SeriesTitle, naive approach
                                if ((references[0].SeriesTitle == null) || (((references[0].SeriesTitle != null) && (references[1].SeriesTitle != null)) && (references[0].SeriesTitle.ToString().Length < references[1].SeriesTitle.ToString().Length)))
                                {
                                    references[0].SeriesTitle = references[1].SeriesTitle;
                                }
                                // 其他的string字段，批量进行
                                List<string> myfieldName = new List<string> { "ParallelTitle","Subtitle", "ShortTitle", "TranslatedTitle", "UniformTitle", "TitleSupplement", "PlaceOfPublication", "Date", "Date2", "Doi", "Edition", "Language" , "Notes", "Number" , "NumberOfVolumes" , "OnlineAddress", "OriginalCheckedBy", "OriginalPublication" , "Price", "PubMedId", "SourceOfBibliographicInformation", "StorageMedium", "TextLinks", "TitleInOtherLanguages", "Volume", "Year"};
                                foreach (string fieldName_i in myfieldName)
                                {
                                    strLeft = references[0].GetType().GetProperty(fieldName_i).GetValue(references[0]).ToString();
                                    strRight = references[1].GetType().GetProperty(fieldName_i).GetValue(references[1]).ToString();
                                    result = MergeOrCombine(mainForm, strLeft, strRight);
                                    if (result == "cancel") { break; }
                                    else
                                    {
                                        references[0].GetType().GetProperty(fieldName_i).SetValue(references[0], result);
                                    }
                                }
                                // "TableOfContents"
                                result = MergeOrCombine(mainForm, references[0].TableOfContents.Text, references[1].TableOfContents.Text, true);
                                if (result == "cancel") { break; }
                                else if (result == "left")
                                {
                                    references[0].TableOfContents.Text = references[0].TableOfContents.Text;
                                }
                                else if (result == "right")
                                {
                                    references[0].TableOfContents.Text = references[1].TableOfContents.Text;
                                }
                                else if (result == "combine")
                                {
                                    references[0].TableOfContents.Text = references[0].TableOfContents.Text + " // " + references[1].TableOfContents.Text;
                                }

                                //"Isbn"
                                result = MergeOrCombine(mainForm, references[0].Isbn.ToString(), references[1].Isbn.ToString(), true);
                                if (result == "cancel") { break; }
                                else if (result == "left")
                                {
                                    references[0].Isbn = references[0].Isbn.ToString();
                                }
                                else if (result == "right")
                                {
                                    references[0].Isbn = references[1].Isbn.ToString();
                                }
                                else if (result == "combine")
                                {
                                    references[0].Isbn = references[0].Isbn.ToString() + " // " + references[1].Isbn.ToString();
                                }

                                // Periodical, naive approach...      
                                result = MergeOrCombine(mainForm, references[0].Periodical.ToString(), references[1].Periodical.ToString(),true);
                                while (result == "combine")
                                {
                                    MessageBox.Show("Periodical can not combine!");
                                    result = MergeOrCombine(mainForm, references[0].Periodical.ToString(), references[1].Periodical.ToString());
                                }
                                if (result == "cancel") { break; }
                                else if (result == "left")
                                {
                                    references[0].Periodical = references[0].Periodical;
                                }
                                else if (result == "right")
                                {
                                    references[0].Periodical = references[1].Periodical;
                                }

                                // SpecificField (1-7)
                                for (int i = 1; i <= 7; i++)
                                {
                                    fieldName = "SpecificField" + i;
                                    strLeft = references[0].GetType().GetProperty(fieldName).GetValue(references[0]).ToString();
                                    strRight = references[1].GetType().GetProperty(fieldName).GetValue(references[1]).ToString();
                                    result = MergeOrCombine(mainForm, strLeft, strRight);
                                    if (result == "cancel") { break; }
                                    else
                                    {
                                        references[0].GetType().GetProperty(fieldName).SetValue(references[0], result);
                                    }

                                }

                                // Evaluation, naive approach...
                                result = MergeOrCombine(mainForm, references[0].Evaluation.Text, references[1].Evaluation.Text, true);
                                if (result == "cancel") { break; }
                                else if (result == "left")
                                {
                                    references[0].Evaluation.Text = references[0].Evaluation.Text;
                                }
                                else if (result == "right")
                                {
                                    references[0].Evaluation.Text = references[1].Evaluation.Text;
                                }
                                else if (result == "combine")
                                {
                                    references[0].Evaluation.Text = references[0].Evaluation.Text + " // " + references[1].Evaluation.Text;
                                }

                                // Rating (take average)
                                references[0].Rating = (short)Math.Floor((decimal)((references[0].Rating + references[1].Rating) / 2));

                                // HasLabel1 and HasLabel2
                                if (references[1].HasLabel1)
                                {
                                    references[0].HasLabel1 = references[1].HasLabel1;
                                }
                                if (references[1].HasLabel2)
                                {
                                    references[0].HasLabel2 = references[1].HasLabel2;
                                }

                                // 分类字段
                                
                                // Groups
                                strLeft = GetCategoryStr(references[0], "Group");
                                strRight = GetCategoryStr(references[1], "Group");
                                result = MergeOrCombine(mainForm, strLeft, strRight,true);
                                if (result == "cancel") { break; }
                                else if (result == "left")
                                {
                                    //continue;
                                }
                                else if (result == "right")
                                {
                                    references[0].Groups.Clear();
                                    references[0].Groups.AddRange(references[1].Groups);
                                }
                                else if (result == "combine")
                                {
                                    references[0].Groups.AddRange(references[1].Groups);
                                }

                                // Category
                                strLeft = GetCategoryStr(references[0], "Category");
                                strRight = GetCategoryStr(references[1], "Category");
                                result = MergeOrCombine(mainForm, strLeft, strRight, true);
                                if (result == "cancel") { break; }
                                else if (result == "left")
                                {
                                    //continue;
                                }
                                else if (result == "right")
                                {
                                    references[0].Categories.Clear();
                                    references[0].Categories.AddRange(references[1].Categories);
                                }
                                else if (result == "combine")
                                {
                                    references[0].Categories.AddRange(references[1].Categories);
                                }
                                // Locations
                                strLeft = GetCategoryStr(references[0], "Location");
                                strRight = GetCategoryStr(references[1], "Location");
                                result = MergeOrCombine(mainForm, strLeft, strRight, true,true);
                                if (result == "cancel") { break; }
                                else if (result == "left")
                                {
                                    //continue;
                                }
                                else if (result == "right")
                                {
                                    references[0].Locations.Clear();
                                    references[0].Locations.AddRange(references[1].Locations);
                                }
                                else if (result == "combine")
                                {
                                    references[0].Locations.AddRange(references[1].Locations);
                                }

                                // Keyword
                                strLeft = GetCategoryStr(references[0], "Keyword");
                                strRight = GetCategoryStr(references[1], "Keyword");
                                result = MergeOrCombine(mainForm, strLeft, strRight, true, true);
                                if (result == "cancel") { break; }
                                else if (result == "left")
                                {
                                    //continue;
                                }
                                else if (result == "right")
                                {
                                    references[0].Keywords.Clear();
                                    references[0].Keywords.AddRange(references[1].Keywords);
                                }
                                else if (result == "combine")
                                {
                                    references[0].Keywords.AddRange(references[1].Keywords);
                                }

                                //Author
                                strLeft = GetCategoryStr(references[0], "Author");
                                strRight = GetCategoryStr(references[1], "Author");
                                result = MergeOrCombine(mainForm, strLeft, strRight, true, true);
                                if (result == "cancel") { break; }
                                else if (result == "left")
                                {
                                    //continue;
                                }
                                else if (result == "right")
                                {
                                    references[0].Authors.Clear();
                                    references[0].Authors.AddRange(references[1].Authors);
                                }
                                else if (result == "combine")
                                {
                                    references[0].Authors.AddRange(references[1].Authors);
                                }

                                //ReferenceCollaborator
                                strLeft = GetCategoryStr(references[0], "Collaborator");
                                strRight = GetCategoryStr(references[1], "Collaborator");
                                result = MergeOrCombine(mainForm, strLeft, strRight, true, true);
                                if (result == "cancel") { break; }
                                else if (result == "left")
                                {
                                    //continue;
                                }
                                else if (result == "right")
                                {
                                    references[0].Collaborators.Clear();
                                    references[0].Collaborators.AddRange(references[1].Collaborators);
                                }
                                else if (result == "combine")
                                {
                                    references[0].Collaborators.AddRange(references[1].Collaborators);
                                }

                                //Editors
                                strLeft = GetCategoryStr(references[0], "Editor");
                                strRight = GetCategoryStr(references[1], "Editor");
                                result = MergeOrCombine(mainForm, strLeft, strRight, true, true);
                                if (result == "cancel") { break; }
                                else if (result == "left")
                                {
                                    //continue;
                                }
                                else if (result == "right")
                                {
                                    references[0].Editors.Clear();
                                    references[0].Editors.AddRange(references[1].Editors);
                                }
                                else if (result == "combine")
                                {
                                    references[0].Editors.AddRange(references[1].Editors);
                                }
                                //Organization
                                strLeft = GetCategoryStr(references[0], "Organization");
                                strRight = GetCategoryStr(references[1], "Organization");
                                result = MergeOrCombine(mainForm, strLeft, strRight, true, true);
                                if (result == "cancel") { break; }
                                else if (result == "left")
                                {
                                    //continue;
                                }
                                else if (result == "right")
                                {
                                    references[0].Organizations.Clear();
                                    references[0].Organizations.AddRange(references[1].Organizations);
                                }
                                else if (result == "combine")
                                {
                                    references[0].Organizations.AddRange(references[1].Organizations);
                                }

                                //OthersInvolved
                                strLeft = GetCategoryStr(references[0], "OthersInvolved");
                                strRight = GetCategoryStr(references[1], "OthersInvolved");
                                result = MergeOrCombine(mainForm, strLeft, strRight, true, true);
                                if (result == "cancel") { break; }
                                else if (result == "left")
                                {
                                    //continue;
                                }
                                else if (result == "right")
                                {
                                    references[0].OthersInvolved.Clear();
                                    references[0].OthersInvolved.AddRange(references[1].OthersInvolved);
                                }
                                else if (result == "combine")
                                {
                                    references[0].OthersInvolved.AddRange(references[1].OthersInvolved);
                                }

                                //Publishers
                                strLeft = GetCategoryStr(references[0], "Publishers");
                                strRight = GetCategoryStr(references[1], "Publishers");
                                result = MergeOrCombine(mainForm, strLeft, strRight, true, true);
                                if (result == "cancel") { break; }
                                else if (result == "left")
                                {
                                    //continue;
                                }
                                else if (result == "right")
                                {
                                    references[0].Publishers.Clear();
                                    references[0].Publishers.AddRange(references[1].Publishers);
                                }
                                else if (result == "combine")
                                {
                                    references[0].Publishers.AddRange(references[1].Publishers);
                                }

                                // Quotations
                                MessageBox.Show("Quotations would merge directly!");
                                references[0].Quotations.AddRange(references[1].Quotations);

                                // ReferenceCollaborator
                                foreach (Person collaborator in references[1].Collaborators)
                                {
                                    if (!references[0].Collaborators.Contains(collaborator))
                                    {
                                        references[0].Collaborators.Add(collaborator);
                                    }
                                }
                                // ReferenceEditor
                                foreach (Person editor in references[1].Editors)
                                {
                                    if (!references[0].Editors.Contains(editor))
                                    {
                                        references[0].Editors.Add(editor);
                                    }
                                }

                                // change crossreferences
                                foreach (EntityLink entityLink in references[1].EntityLinks)
                                {
                                    if (entityLink.Source == references[1])
                                    {
                                        entityLink.Source = references[0];
                                    }
                                    else if (entityLink.Target == references[1])
                                    {
                                        entityLink.Target = references[0];
                                    }
                                }

                                // DONE! remove second reference
                                activeProject.References.Remove(references[1]);
                                //activeProject.SaveAsync();
                                
                            }
                        }
                    }
                    break;

            }

            base.OnBeforePerformingCommand(mainForm, e);
        }
 
        private static string MergeOrCombine(MainForm mainform, string strLeft, string strRight,bool returnLRValue=false,bool doi = false)
        {
            strLeft = strLeft.Trim();
            strRight = strRight.Trim();
            if (!returnLRValue)
            {
                if (String.Compare(strLeft, strRight, false) == 0)
                {
                    // easy case, they are the same!
                    return strLeft;
                }
                else if (strLeft.Length == 0)
                {
                    return strRight;
                }
                else if (strRight.Length == 0)
                {
                    return strLeft;
                }
                else
                {
                    if (!doi)
                    {
                        using (var dialog = new ComparingForm(mainform, strLeft, strRight))
                        {
                            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                            {
                                string result = dialog.DialogResult; // 获取对话框返回的结果
                                if (result == "left")
                                {
                                    return strLeft;
                                    //references[0].Title = references[0].Title;
                                }
                                else if (result == "right")
                                {
                                    return strRight;
                                }
                                else if (result == "combine")
                                {
                                    return strLeft + " // " + strRight;
                                }
                                else
                                {
                                    return "cancel";
                                }
                            }
                            else
                            {
                                MessageBox.Show("Code error,dialog window did not open!");
                                return "cancel";
                            }
                        }
                    }
                    else
                    {
                        using (var dialog = new ComparingForm(mainform, strLeft, strRight))
                        {
                            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                            {
                                string result = dialog.DialogResult; // 获取对话框返回的结果
                                if (result == "left")
                                {
                                    return strLeft;
                                    //references[0].Title = references[0].Title;
                                }
                                else if (result == "right")
                                {
                                    return strRight;
                                }
                                else if (result == "combine")
                                {
                                    return strLeft + " // " + strRight;
                                }
                                else
                                {
                                    return "cancel";
                                }
                            }
                            else
                            {
                                MessageBox.Show("Code error,dialog window did not open!");
                                return "cancel";
                            }
                        }
                    }
                    
                }
            }
            else
            {
                
                if (String.Compare(strLeft, strRight, false) == 0)
                {
                    return "left";
                    // easy case, they are the same!
                }
                else if (strLeft.Length == 0)
                {
                    return "right";
                }
                else if (strRight.Length == 0)
                {
                    return "left";
                }
                else
                {
                    if (doi)
                    {
                        using (var dialog = new ComparingForm(mainform, strLeft, strRight,true))
                        {
                            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                            {
                                return dialog.DialogResult; // 获取对话框返回的结果
                            }
                            else
                            {
                                MessageBox.Show("Code error,dialog window did not open!");
                                return "cancel";
                            }
                        }
                    }
                    else
                    {
                        using (var dialog = new ComparingForm(mainform, strLeft, strRight))
                        {
                            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                            {
                                return dialog.DialogResult; // 获取对话框返回的结果
                            }
                            else
                            {
                                MessageBox.Show("Code error,dialog window did not open!");
                                return "cancel";
                            }
                        }
                    }
                    
                }
            }


            
        }


        public static string GetCategoryStr(Reference reference, string cateName)
        {
            string result="";
            
            switch (cateName)
            {
                
                case "Group":
                    List<Group> groupCategories = reference.Groups.ToList();
                    List<string> nameString = new List<string>(); // 创建一个空的 List<string>
                    foreach (Group mygroup in groupCategories)
                    {
                        if (reference.Groups == null) continue;
                        nameString.Add(mygroup.FullName);

                    }
                    // 将 List 转换为数组
                    result = GetStringFromArray(nameString);
                    break;
                case "Category":
                    List<Category> categoryRefCategories = reference.Categories.ToList();
                    List<string> nameString2 = new List<string>(); // 创建一个空的 List<string>
                    foreach (Category category in categoryRefCategories)
                    {
                        if (reference.Categories == null) continue;
                        nameString2.Add(category.FullName);

                    }
                    // 将 List 转换为数组
                    result = GetStringFromArray(nameString2);
                    break;
                case "Location":
                    List<Location> refLocation = reference.Locations.ToList();
                    List<string> nameString3 = new List<string>(); // 创建一个空的 List<string>
                    foreach (Location location in refLocation)
                    {
                        if (reference.Locations == null) continue;
                        nameString3.Add(location.FullName);
                    }
                    // 将 List 转换为数组
                    result = GetStringFromArray(nameString3);
                    break;
                case "Keyword":
                    List<Keyword> refKeywords = reference.Keywords.ToList();
                    List<string> nameString4 = new List<string>(); // 创建一个空的 List<string>
                    foreach (Keyword keyword in refKeywords)
                    {
                        if (reference.Keywords == null) continue;
                        nameString4.Add(keyword.FullName);

                    }
                    // 将 List 转换为数组后连接起来
                    result = GetStringFromArray(nameString4);
                    break;
                case "Author":
                    List<Person> refPersons = reference.Authors.ToList();
                    List<string> nameString5 = new List<string>(); // 创建一个空的 List<string>
                    foreach (Person person in refPersons)
                    {
                        if (reference.Authors == null) continue;
                        nameString5.Add(person.FullName);
                    }
                    // 将 List 转换为数组后连接起来
                    result = GetStringFromArray(nameString5);
                    break;
                case "Collaborator":
                    refPersons = reference.Collaborators.ToList();
                    nameString5 = new List<string>(); // 创建一个空的 List<string>
                    foreach (Person person in refPersons)
                    {
                        if (reference.Collaborators == null) continue;
                        nameString5.Add(person.FullName);
                    }
                    // 将 List 转换为数组后连接起来
                    result = GetStringFromArray(nameString5);
                    break;
                case "Editor": //ReferenceEditor
                    refPersons = reference.Editors.ToList();
                    nameString5 = new List<string>(); // 创建一个空的 List<string>
                    foreach (Person person in refPersons)
                    {
                        if (reference.Editors == null) continue;
                        nameString5.Add(person.FullName);
                    }
                    // 将 List 转换为数组后连接起来
                    result = GetStringFromArray(nameString5);
                    break;
                case "Organization": //ReferenceOrganization
                    refPersons = reference.Organizations.ToList();
                    nameString5 = new List<string>(); // 创建一个空的 List<string>
                    foreach (Person person in refPersons)
                    {
                        if (reference.Organizations == null) continue;
                        nameString5.Add(person.FullName);
                    }
                    // 将 List 转换为数组后连接起来
                    result = GetStringFromArray(nameString5);
                    break;
                case "OthersInvolved": //ReferenceOthersInvolved
                    refPersons = reference.OthersInvolved.ToList();
                    nameString5 = new List<string>(); // 创建一个空的 List<string>
                    foreach (Person person in refPersons)
                    {
                        if (reference.OthersInvolved == null) continue;
                        nameString5.Add(person.FullName);
                    }
                    // 将 List 转换为数组后连接起来
                    result = GetStringFromArray(nameString5);
                    break;
                case "Publishers": //ReferenceOthersInvolved
                    List<Publisher> refPublishers = reference.Publishers.ToList();
                    nameString5 = new List<string>(); // 创建一个空的 List<string>
                    foreach (Publisher publisher in refPublishers)
                    {
                        if (reference.OthersInvolved == null) continue;
                        nameString5.Add(publisher.FullName);
                    }
                    // 将 List 转换为数组后连接起来
                    result = GetStringFromArray(nameString5);
                    break;
            }
            return result;

        }

        public static string GetStringFromArray(List<string> nameString)
        {
            string result="";
            // 将 List 转换为数组
            string[] strings = nameString.ToArray();
            Array.Sort(strings);
            if (strings.Length > 0)
            {
                result = string.Join("\r\n", strings); // 如果有两个以上的字符串，用换行符连接它们
            };
            return result;

        }
    }
}

