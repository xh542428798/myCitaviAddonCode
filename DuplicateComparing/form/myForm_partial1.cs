using System;
using System.Windows.Forms;
using SwissAcademic.Citavi;
using SwissAcademic.Controls;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using DiffPlex.WindowsForms.Controls;
using System.Drawing;
using System.Collections.Generic;

namespace DuplicateComparing
{
    public partial class myForm : FormBase
    {
        private void CompareStrings(string leftRef, string rightRef)
        {
            SideBySideDiffBuilder diffBuilder = new SideBySideDiffBuilder(new Differ());
            SideBySideDiffModel diffModel = diffBuilder.BuildDiffModel(leftRef, rightRef);

            this.panelLeft.Clear();
            this.panelRight.Clear();
            List<int> delelteInfo = new List<int>();
            foreach (var line in diffModel.OldText.Lines)
            {
                for (int i = 0; i < line.SubPieces.Count; i++)
                {
                    var change = line.SubPieces[i];
                    //}
                    //foreach (var change in line.SubPieces)
                    //{

                    // if (string.IsNullOrEmpty(change?.Text)) continue;
                    if (change.Type == ChangeType.Imaginary)
                    {
                        continue;
                    }
                    else if (change.Type == ChangeType.Deleted)
                    {
                        if (change.Text == " ")
                        {
                            this.panelLeft.SelectionBackColor = Color.Transparent;
                            this.panelLeft.AppendText(" ");
                            delelteInfo.Add(i);
                        }
                        else
                        {
                            this.panelLeft.SelectionBackColor = Color.Transparent;
                            this.panelLeft.SelectionColor = Color.Red;
                            this.panelLeft.AppendText(change.Text);
                        }


                    }
                    else if (change.Type == ChangeType.Inserted)
                    {
                        this.panelLeft.SelectionBackColor = Color.Transparent;
                        this.panelLeft.SelectionColor = Color.Green;
                        this.panelLeft.AppendText(change.Text);
                    }
                    else if (change.Type == ChangeType.Modified)
                    {
                        this.panelLeft.SelectionBackColor = Color.Transparent;
                        this.panelLeft.SelectionColor = Color.Yellow;
                        this.panelLeft.AppendText(change.Text);
                    }
                    else
                    {
                        this.panelLeft.SelectionBackColor = Color.Transparent;
                        this.panelLeft.SelectionColor = Color.Black;
                        this.panelLeft.AppendText(change.Text);
                    }
                }

                this.panelLeft.AppendText(Environment.NewLine);
            }

            foreach (var line in diffModel.NewText.Lines)
            {
                for (int i = 0; i < line.SubPieces.Count; i++)
                {
                    var change = line.SubPieces[i];
                    if (delelteInfo.Contains(i))
                    {
                        this.panelRight.SelectionBackColor = Color.Red;
                        this.panelRight.AppendText(" ");
                    }
                    //}
                    // if (string.IsNullOrEmpty(change?.Text)) continue;
                    if (change.Type == ChangeType.Imaginary)
                    {
                        continue;
                    }
                    else if (change.Type == ChangeType.Deleted)
                    {
                        if (change.Text == " ")
                        {
                            continue;
                            //this.panelRight.SelectionBackColor = Color.Red;
                            //this.panelRight.AppendText(" ");
                        }
                        else
                        {
                            this.panelRight.SelectionBackColor = Color.Transparent;
                            this.panelRight.SelectionColor = Color.Red;
                            this.panelRight.AppendText(change.Text);
                        }
                    }
                    else if (change.Type == ChangeType.Inserted)
                    {
                        this.panelRight.SelectionBackColor = Color.Transparent;
                        this.panelRight.SelectionColor = Color.Green;
                        this.panelRight.AppendText(change.Text);
                    }
                    else if (change.Type == ChangeType.Modified)
                    {
                        this.panelRight.SelectionBackColor = Color.Transparent;
                        this.panelRight.SelectionColor = Color.Yellow;
                        this.panelRight.AppendText(change.Text);
                    }
                    else
                    {
                        this.panelRight.SelectionBackColor = Color.Transparent;
                        this.panelRight.SelectionColor = Color.Black;
                        this.panelRight.AppendText(change.Text);
                    }
                }

                this.panelRight.AppendText(Environment.NewLine);
            }
        }
    }
}
