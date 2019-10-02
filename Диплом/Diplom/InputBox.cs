using System;
using System.Windows.Forms;

namespace Diplom
{
    public partial class InputBox : Form
    {
        // the InputBox
        private static InputBox newInputBox;
        // строка, которая будет возвращена форме запроса
        private static string returnString;

        public InputBox()
        {
            InitializeComponent();
            this.CenterToScreen();
        }
        public static string Show()
        {
            newInputBox = new InputBox();
           // newInputBox.label1.Text = inputBoxText;
            newInputBox.ShowDialog();
            return returnString;
        }

        private void yes_Click(object sender, EventArgs e)
        {
            returnString = textBox1.Text;
            newInputBox.Dispose();
        }

        private void no_Click(object sender, EventArgs e)
        {
            returnString = string.Empty;
            newInputBox.Dispose();
        }
    }
}
