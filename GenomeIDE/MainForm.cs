using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GenomeIDE
{
    public partial class MainForm : Form
    {
        DNA DNA { get; set; }

        public MainForm()
        {
            InitializeComponent();
            DNA = new DNA();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            GeneLibrary.Load();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LoadGenomeButton_Click(object sender, EventArgs e)
        {
            if (OpenDialog.ShowDialog(this) != DialogResult.OK) return;

            DNA = new DNA(OpenDialog.FileName);
            UpdateDNAView();
            UpdateGenomeEditor();
        }

        private void UpdateDNAView()
        {
            DnaView.Nodes.Clear();
            var i = 0;
            while (i < DNA.Size)
            {
                var g = DNA[i];
                var gene = GeneLibrary.Find(g);

                if ((gene != null) && DNA.Size >= i + gene.Size)
                {
                    DnaView.Nodes.Add(gene.CreateNode(DNA.GetFragment(i, gene.Size)));
                    i += gene.ArgumentBytes;
                }
                else
                    DnaView.Nodes.Add(new TreeNode(g.ToString("X2") + " (inactive)"));

                i++;
            }
        }

        private void UpdateGenomeEditor()
        {
            GenomeEditor.Items.Clear();
            GenomeEditor.Items.AddRange(DNA.Select(b => b.ToString("X2")).ToArray());
        }

        private void UpdateGenomeEditor(int index)
        {
            if (index < GenomeEditor.Items.Count)
                GenomeEditor.Items[index] = DNA[index].ToString("X2");
            else if (index == GenomeEditor.Items.Count)
            {
                GenomeEditor.Items.Add(DNA[index].ToString("X2"));
            }
            else
            {
                var newValues = DNA.GetFragment(GenomeEditor.Items.Count, index).Select(b => b.ToString("X2")).ToArray();
                GenomeEditor.Items.AddRange(newValues);
            }
        }

        private void SaveGenomeButton_Click(object sender, EventArgs e)
        {
            //TODO: Save genome to file
        }

        private void EditorContextMenu_Opening(object sender, CancelEventArgs e)
        {
            var hasSelectedItem = GenomeEditor.SelectedItem != null;
            EditGeneButton.Enabled = hasSelectedItem;
            RemoveGeneButton.Enabled = hasSelectedItem;
            InsertSubMenu.Visible = hasSelectedItem;
            InsertButton.Visible = !hasSelectedItem;
        }

        private void InsertButton_Click(object sender, EventArgs e)
        {
            var editor = new GeneEditor();
            if (editor.ShowDialog(this) == DialogResult.OK)
            {
                DNA.Add(editor.Value);
                UpdateGenomeEditor(GenomeEditor.Items.Count);
                UpdateDNAView();
            }
        }

        private void EditGeneButton_Click(object sender, EventArgs e)
        {
            var selected = Convert.ToByte(GenomeEditor.SelectedItem as string, 16);
            var editor = new GeneEditor(selected);
            if (editor.ShowDialog(this) == DialogResult.OK)
            {
                DNA[GenomeEditor.SelectedIndex] = editor.Value;
                UpdateGenomeEditor(GenomeEditor.SelectedIndex);
                UpdateDNAView();
            }
        }

        private void RemoveGeneButton_Click(object sender, EventArgs e)
        {
            DNA.RemoveAt(GenomeEditor.SelectedIndex);
            GenomeEditor.Items.RemoveAt(GenomeEditor.SelectedIndex);
            UpdateDNAView();
        }

        private void InsertBeforeButton_Click(object sender, EventArgs e)
        {
            InsertGene(GenomeEditor.SelectedIndex);
        }

        private void InsertGene(int index)
        {
            var editor = new GeneEditor();
            if (editor.ShowDialog(this) == DialogResult.OK)
            {
                DNA.Insert(index, editor.Value);

                if (index < GenomeEditor.Items.Count)
                    GenomeEditor.Items.Insert(index, DNA[index]);
                else
                    UpdateGenomeEditor(index);

                UpdateDNAView();
            }
        }

        private void InsertAfterButton_Click(object sender, EventArgs e)
        {
            InsertGene(GenomeEditor.SelectedIndex + 1);
        }

        private void SaveGeneLibraryButton_Click(object sender, EventArgs e)
        {
            GeneLibrary.Save();
        }


    }
}
