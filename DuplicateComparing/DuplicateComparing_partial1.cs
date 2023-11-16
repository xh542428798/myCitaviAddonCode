using System;
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
    public partial class DuplicateComparing
        :
        CitaviAddOn<MainForm>
    {

        private static string MergeOrCombine(MainForm mainform, string strLeft, string strRight, string titleText, bool returnLRword = false, bool diffplex = true)
        {
            strLeft = strLeft.Trim();
            strRight = strRight.Trim();
            if (returnLRword)
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
                    using (var dialog = new myForm(mainform, strLeft, strRight, titleText, Comparing: diffplex))
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
            else
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
                    using (var dialog = new myForm(mainform, strLeft, strRight, titleText, Comparing: diffplex))
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
                                return strLeft + Environment.NewLine + strRight;
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


        public static string GetCategoryStr(Reference reference, string cateName)
        {
            string result = "";

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
            string result = "";
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
