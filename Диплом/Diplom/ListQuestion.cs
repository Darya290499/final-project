using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diplom
{
    public partial class ListQuestion: Form
    {
        // the InputBox
        private static ListQuestion question;
        // строка, которая будет возвращена форме запроса
        private static string returnString;
        public ListQuestion()
        {
            InitializeComponent();

            this.CenterToScreen();
        }

        public static string Show(string persBut, string docBut)
        {
            question = new ListQuestion();
            question.personal.Text = persBut;
            question.documents.Text = docBut;
            question.ShowDialog();
            return returnString;
        }

        private void personal_Click(object sender, EventArgs e)
        {
            returnString = "pers";
            question.Dispose();
        }

        private void documents_Click(object sender, EventArgs e)
        {
            returnString = "docs";
            question.Dispose();
        }

       /* private void ListQuestion_FormClosed(object sender, FormClosedEventArgs e)
        {
            returnString = string.Empty;
            question.Dispose();
        }*/
    }
}
