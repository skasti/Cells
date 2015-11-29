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
        }

        private void UpdateDNAView()
        {
            DnaView.Nodes.Clear();
            var i = 0;
            while (i < DNA.Size)
            {
                var g = DNA[i];
                var gene = GeneLibrary.Find(g);

                if (gene != null)
                {
                    DnaView.Nodes.Add(gene.CreateNode(DNA.GetFragment(i, gene.ArgumentBytes + 1)));
                    i += gene.ArgumentBytes;
                }
                else
                    DnaView.Nodes.Add(new TreeNode(g.ToString("X2") + " (inactive)"));

                i++;
            }
        }

        private void SaveGenomeButton_Click(object sender, EventArgs e)
        {
            //TODO: Save genome to file
        }
    }
}
