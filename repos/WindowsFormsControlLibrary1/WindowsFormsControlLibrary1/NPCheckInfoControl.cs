using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace NPCustomWinFormControl
{
    public delegate void DeleteHandler(int index);
    public delegate bool CheckIPHandler(string ip);
    public partial class NPCheckInfoControl: UserControl
    {
        public NPCheckInfoControl()
        {
            InitializeComponent();
            _initial();
            this.checkIPTextBox.Focus();
        }

        private RadioButton[] checkRadioList;
        private CheckBox[] checkCheckBoxList;
        public bool hasError;
        public DeleteHandler deleteHandler;
        public CheckIPHandler checkIPHandler;
        public int index;

        public void setTitle(string title)
        {
           this.outStoreConfigGroupBox.Text = title;
        }

        public void setAddress(string address)
        {
            this.checkIPTextBox.Text = address;
            
        }

        public void setPortNum(int num)
        {
            foreach (RadioButton rb in checkRadioList)
            {
                if (int.Parse(rb.Text) == num)
                {
                    rb.Checked = true;
                }
                else
                {
                    rb.Checked = false;
                }
            }
        }

        public void setUsedPort(JArray usedPort)
        {
            if(usedPort == null)
            {
                return;
            }
            foreach (CheckBox cb in checkCheckBoxList)
            {
                int[] array = usedPort.ToObject<int[]>();
                if (Array.IndexOf<int>(array, cb.TabIndex + 1) != -1)
                {
                    cb.Checked = true;
                }
                else
                {
                    cb.Checked = false;
                }
            }
        }

        private void _initial()
        {
            checkRadioList = new RadioButton[]
{
                this.checkRadio1,this.checkRadio2,this.checkRadio3,this.checkRadio4,this.checkRadio5,this.checkRadio6
};

            checkCheckBoxList = new CheckBox[]
            {
                this.checkCheckBox1,this.checkCheckBox2,this.checkCheckBox3,this.checkCheckBox4,this.checkCheckBox5,this.checkCheckBox6,this.checkCheckBox7,this.checkCheckBox8,this.checkCheckBox9,this.checkCheckBox10,this.checkCheckBox11,this.checkCheckBox12,this.checkCheckBox13,this.checkCheckBox14,this.checkCheckBox15,this.checkCheckBox16
            };

        }

        private void NPCheckInfoControl_Enter(object sender, EventArgs e)
        {

        }

        private void showEmptyWarningIfNeeded(TextBox txtBox, System.ComponentModel.CancelEventArgs e)
        {
            if (txtBox == null) return;
            e.Cancel = (txtBox.Text == string.Empty);
            if (string.IsNullOrEmpty(txtBox.Text))
            {
                errorProvider1.SetError(txtBox, "不能为空");
                hasError = true;
            }
            else
            {
                errorProvider1.SetError(txtBox, null);
                hasError = false;
            }
        }

        private void checkIPTextBox_Validating(object sender, CancelEventArgs e)
        {
            var textBox = sender as TextBox;
            showEmptyWarningIfNeeded(textBox, e);
            if (!isValidateIP(textBox.Text))
            {
                errorProvider1.SetError(textBox, "请输入有效的IP地址");
                hasError = true;
            }else if(isIPExist(textBox.Text)){
                errorProvider1.SetError(textBox, "该读写器地址已存在");
                hasError = true;
            }
            else
            {
                errorProvider1.SetError(textBox, null);
                hasError = false;
            }
        }

        private void checkIPTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (isValidateIP(tb.Text))
            {
                portsCountGroupBox2.Enabled = true;
            }

            else
            {
                portsCountGroupBox2.Enabled = false;
            }
            foreach (RadioButton rb in checkRadioList)
            {
                rb.Checked = false;
            }
            clearCheckBoxs(true);
        }

        private bool isValidateIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        private bool isIPExist(string ip)
        {
            bool exist = false;
            if (checkIPHandler != null)
            {
                exist = checkIPHandler(ip);
            }
            return exist;
        }

        // 清空端口选择状态
        private void clearCheckBoxs(bool hidden)
        {
            foreach (CheckBox cb in checkCheckBoxList)
            {
                cb.Checked = false;
                if (hidden)
                {
                    cb.Visible = false;
                }
            }

            foreach (CheckBox cb in checkCheckBoxList)
            {
                cb.Enabled = true;
            }
        }

        // 盘点端口数选择
        private void checkRadio_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            clearCheckBoxs(false);
            if (rb.Checked == false) return;
            showPartOfCheckBoxs(int.Parse(rb.Text));
        }

        // 控制端口选项显示个数
        private void showPartOfCheckBoxs(int portNum)
        {
            foreach (CheckBox cb in checkCheckBoxList)
            {
                cb.Visible = cb.TabIndex + 1 <= portNum ? true : false;
            }
        }

        private void portsCountGroupBox2_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(this.BackColor);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if(deleteHandler != null)
            {
                deleteHandler(this.index);
            }
        }
 
        public string getIpAddress()
        {
            return this.checkIPTextBox.Text;
        }
        
        public int getPortNum()
        {
            int portNum = 0;
            foreach (RadioButton rb in checkRadioList)
            {
                if (rb.Checked == true)
                {
                    portNum = int.Parse(rb.Text);
                }
            }
            return portNum;
        }

        public JArray getUsedPort()
        {
            JArray usedPort = new JArray();
            foreach (CheckBox cb in checkCheckBoxList)
            {
                if (cb.Checked == true)
                {
                    usedPort.Add(cb.TabIndex + 1);
                }
            }
            return usedPort;
        }

        public void _getFocus()
        {
            this.checkIPTextBox.Select();
        }
    }
}
