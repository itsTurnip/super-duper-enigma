using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Terminal
{
    public class PropertiesConsole
    {
        public Form form;
        private TabControl tabControl;
        private GroupBox groupBox;
        private ComboBox comboBoxChangeColor;
        private RadioButton radioButtonFontColor;
        private RadioButton radioButtonBackColor;
        private Button buttonOK;
        private Button buttonCancel;
        private GroupBox groupBoxTransparent;
        private TrackBar trackBarTransparent;
        private Label labelZeroPer;
        private Label labelHundrPer;
        private TextBox textBoxDispPer;

        [DllImport("user32.dll")]
        private static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool GetLayeredWindowAttributes(IntPtr hWnd, out uint crKey, out byte bAlpha, out uint dwFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetConsoleWindow();

        public ConsoleColor DefaultBackgroundProperties { get; set; } = ConsoleColor.Black;

        public ConsoleColor DefaultForegroundProperties { get; set; } = ConsoleColor.Gray;

        public PropertiesConsole()
        {
            var transparentConsole = GetLayeredWindowAttributes(GetConsoleWindow(), out uint crKey, out byte bAlpha, out uint dwFlags) ? bAlpha * 100 / 0xFF : 100;

            Application.EnableVisualStyles();
            form = new Form()
            {
                FormBorderStyle = FormBorderStyle.FixedToolWindow,
                AutoSize = true,
                Text = "Properties",
                ShowIcon = false,
                StartPosition = FormStartPosition.CenterScreen,
            };
            tabControl = new TabControl()
            {
                Dock = DockStyle.Top,
                Size = new Size(300, 235)
            };
            groupBox = new GroupBox()
            {
                Location = new Point(3, 0),
                Size = new Size(143, 79),
                AutoSize = true
            };
            radioButtonFontColor = new RadioButton()
            {
                Location = new Point(6, 42),
                AutoSize = true,
                Text = "Font Color",
                Checked = true
            };
            radioButtonBackColor = new RadioButton()
            {
                Location = new Point(6, 19),
                AutoSize = true,
                Text = "Background Color"
            };
            comboBoxChangeColor = new ComboBox()
            {
                Location = new Point(152, 6),
                Size = new Size(110, 21)
            };
            buttonOK = new Button()
            {
                Location = new Point(102, 240),
                Text = "OK"
            };
            buttonCancel = new Button()
            {
                Location = new Point(192, 240),
                Text = "Cancel"
            };
            groupBoxTransparent = new GroupBox()
            {
                Text = "Transparent",
                Location = new Point(3, 100),
                Size = new Size(270, 69)
            };
            trackBarTransparent = new TrackBar()
            {
                Location = new Point(33, 19),
                Size = new Size(154, 45),
                TickFrequency = 10,
                Maximum = 100,
                Value = transparentConsole
            };
            labelZeroPer = new Label()
            {
                Text = "0%",
                Location = new Point(6, 19),
                Size = new Size(21, 13)
            };
            labelHundrPer = new Label()
            {
                Text = "100%",
                Location = new Point(193, 19),
                Size = new Size(33, 13)
            };
            textBoxDispPer = new TextBox()
            {
                BackColor = SystemColors.Control,
                Location = new Point(241, 16),
                Size = new Size(25, 20),
                ReadOnly = true,
                Text = $"{transparentConsole}"
            };

            AddItems();
            AddEvents();
        }

        private void AddItems()
        {
            tabControl.TabPages.Add("Colors");
            groupBox.Controls.Add(radioButtonBackColor);
            groupBox.Controls.Add(radioButtonFontColor);
            groupBoxTransparent.Controls.Add(trackBarTransparent);
            groupBoxTransparent.Controls.Add(labelZeroPer);
            groupBoxTransparent.Controls.Add(labelHundrPer);
            groupBoxTransparent.Controls.Add(textBoxDispPer);
            tabControl.TabPages[0].Controls.Add(groupBox);
            tabControl.TabPages[0].Controls.Add(comboBoxChangeColor);
            tabControl.TabPages[0].Controls.Add(groupBoxTransparent);
            comboBoxChangeColor.Items.AddRange(Enum.GetValues(typeof(ConsoleColor)).Cast<object>().ToArray());
            comboBoxChangeColor.SelectedItem = Console.ForegroundColor;
            form.Controls.Add(buttonOK);
            form.Controls.Add(buttonCancel);
            form.Controls.Add(tabControl);
        }

        private void AddEvents()
        {
            radioButtonFontColor.CheckedChanged += RadioButtonColor_CheckedChanged;
            radioButtonBackColor.CheckedChanged += RadioButtonColor_CheckedChanged;
            buttonOK.Click += ButtonOK_Click;
            buttonCancel.Click += ButtonCancel_Click;
            trackBarTransparent.ValueChanged += TrackBarTransparent_ValueChanged;
        }

        private void TrackBarTransparent_ValueChanged(object sender, EventArgs e)
        {
            IntPtr handle = GetConsoleWindow();
            SetLayeredWindowAttributes(handle, 0, Convert.ToByte(Math.Ceiling((double)trackBarTransparent.Value * 0xFF / 100)), 0x2);
            textBoxDispPer.Text = $"{trackBarTransparent.Value}";
        }

        private void RadioButtonColor_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonFontColor.Checked)
            {
                comboBoxChangeColor.SelectedItem = Console.ForegroundColor;
            }
            if (radioButtonBackColor.Checked)
            {
                comboBoxChangeColor.SelectedItem = Console.BackgroundColor;
            }
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            if (radioButtonFontColor.Checked)
            {
                DefaultForegroundProperties = (ConsoleColor)comboBoxChangeColor.SelectedItem;
            }
            if (radioButtonBackColor.Checked)
            {
                DefaultBackgroundProperties = (ConsoleColor)comboBoxChangeColor.SelectedItem;
            }
            form.DialogResult = DialogResult.OK;
            form.Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            form.DialogResult = DialogResult.Cancel;
            form.Close();
        }
    }
}