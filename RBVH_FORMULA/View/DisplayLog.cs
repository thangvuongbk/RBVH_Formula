using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

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
            const string TCSHEET_NAME = "Testcases";

            //
            Excel.Application xlExcelApp;
            Excel.Workbook xlWorkBookSrc;
            xlExcelApp = new Excel.Application();
            xlWorkBookSrc = xlExcelApp.Workbooks.Open(txtFormulaTD.Text.ToString());
            xlExcelApp.Visible = true;

            RBVH_FORMULA.Control.FormulaProcess l_ControlProcess = new Control.FormulaProcess();
            bResult = l_ControlProcess.StatementHandling(xlWorkBookSrc, TCSHEET_NAME, txtCode.Text.ToString(), ref g_log); //D:\THANGVUONG\Tool_2015\FORMULA_ESS2\FORMULA_ESS2\bin\Release\123.c
            //bResult = l_ControlProcess.StatementHandling(xlWorkBookSrc, TCSHEET_NAME, @"C:\Users\VV81HC\Documents\Visual Studio 2013\Projects\RBVH_FORMULA\RBVH_FORMULA\123.c", ref g_log);
            DEBUG(g_log);
            DEBUG("-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*");
            if (bResult)
            {
                MessageBox.Show("Done Successfully");
                DEBUG("Done Successfully");
            }
            else
            {
                MessageBox.Show("Done With Error. Please check");
                DEBUG("Done With Error. Please check");
            }

        }

        private void btnTDBrowser_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.RestoreDirectory = true;
            openFile.DefaultExt = "Excel File";
            openFile.Filter = "Excel files (*.xlsm)|*.xlsm|All files (*.*)|*.*";
            openFile.CheckFileExists = true;
            openFile.CheckPathExists = true;
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                txtFormulaTD.Text = openFile.FileName;
               // btnOK.Enabled = true;
            }
            else
            {
              //  btnOK.Enabled = false;
            }
        }
        public void eventCCodeBrowseClick(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.RestoreDirectory = true;
            openFile.DefaultExt = "C Source";
            openFile.Filter = "C_Code_Template (*.c)|(*.c; *.txt) | All files (*.*)|*.*";
            openFile.CheckFileExists = true;
            openFile.CheckPathExists = true;
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                txtCode.Text = openFile.FileName;
                //btnOK.Enabled = true;
            }
            else
            {
                //btnOK.Enabled = false;
            }
        }


        private void eventCancelClick(object sender, EventArgs e)
        {
            DialogResult drCancel = MessageBox.Show("Are you sure?!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (drCancel == DialogResult.Yes)
            {               
                this.Dispose();
                this.Close();
            }
        }
    }
}
