using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RBVH_FORMULA.View
{
    public partial class DisplayLog : Form
    {
        public DisplayLog()
        {
            InitializeComponent();
            DEBUG("Welcome");
            DEBUG("-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*");
        }
        public void DEBUG(string msg)
        {                    
           // this.logView.Text += msg + Environment.NewLine;
            if(this.InvokeRequired)
            {
                this.Invoke(new Action<string>(DEBUG), new object[] { Text });
                return;
            }
            this.logView.Text += msg + Environment.NewLine;
        }
        /// <summary>
        /// On event update the text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myrichTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                logView.SelectionStart = logView.Text.Length; //Set the current caret position at the end
                logView.ScrollToCaret(); //Now scroll it automatically                        
            }
            catch (Exception exp)
            {
                MessageBox.Show("Insufficient memory to continue the execution of the program." + Environment.NewLine + exp.Message);
            }
        }

        private void RBVHMain(object sender, EventArgs e)
        {
            string g_log = null;
            bool bResult = false;   
            RBVH_FORMULA.Control.FormulaProcess l_ControlProcess = new Control.FormulaProcess();
            bResult = l_ControlProcess.StatementHandling(@"C:\Users\VV81HC\Documents\new_4.h", ref g_log);
            DEBUG(g_log);
            DEBUG("-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*");
            if (bResult)
            {
                DEBUG("Done Successufly");
            }
            else DEBUG("Done With Error. Please check");

        }
    }
}
