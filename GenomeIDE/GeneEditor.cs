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
    public partial class GeneEditor : Form
    {
        private byte _value;

        public byte Value
        {
            get { return _value; }
            private set
            {
                _value = value;
                HexInput.Text = _value.ToString("X2");
                GeneSelector.SelectedItem = GeneLibrary.Find(_value);
            }
        }

        public GeneEditor()
        {
            InitializeComponent();
        }

        public GeneEditor(byte existing)
        {
            InitializeComponent();

            Value = existing;
        }

        private void GeneEditor_Load(object sender, EventArgs e)
        {
            GeneSelector.DataSource = GeneLibrary.List;
        }

        private void HexInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter)
                return;

            ValidateHexInput();
        }

        private bool ValidateHexInput()
        {
            try
            {
                var value = Convert.ToByte(HexInput.Text, 16);
                Value = value;
            }
            catch (OverflowException)
            {
                MessageBox.Show(
                    "'{0}' is not a valid hex-value".Inject(HexInput.Text),
                    "Parse error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void GeneSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedGene = GeneSelector.SelectedItem as Gene;

            if (selectedGene != null)
            {
                Value = selectedGene.MarkerFrom;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (ValidateHexInput())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
