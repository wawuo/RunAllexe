using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace 批量启动工具
{
    public partial class mianForm : Form
    {
        int lineNumber = 1;
        RunApp runApp;
        Button RunButton;
        // Button StopButton;
        public mianForm()
        {
            InitializeComponent();
            dataGridView1.CellEndEdit += dataGridView1_CellEndEdit;
            dataGridView1.CellValidating += dataGridView1_CellValidating;
            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
            //dataGridView1.BackgroundColor = Color.Transparent;

            runApp = new RunApp(txtLog);
            StopButton.Enabled = false;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 允许选择一行
            //listBox1.SelectionMode = SelectionMode.One;
            // 或者，如果你想允许选择多行，可以使用下面的代码：
            //listBox1.SelectionMode = SelectionMode.MultiSimple;
            //listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            dataGridView1.Columns.Clear(); // 清除现有的列
            dataGridView1.Columns.Add("LineNumber", "序号"); // 添加 "行号" 列
            dataGridView1.Columns.Add("LineContent", "路径"); // 添加 "内容" 列
            dataGridView1.Columns.Add("InputBox", "延时"); // 添加 "输入框" 列
                                                          // 设置 "行号" 列的宽度为 50
            dataGridView1.Columns["LineNumber"].Width = 50;
            dataGridView1.Columns["LineNumber"].Resizable = DataGridViewTriState.False;

            // 设置 "内容" 列的宽度为 200
            dataGridView1.Columns["LineContent"].Width = 480;
            dataGridView1.Columns["LineContent"].Resizable = DataGridViewTriState.False;

            // 设置 "输入框" 列的宽度为 100
            dataGridView1.Columns["InputBox"].Width = 60;
            dataGridView1.Columns["InputBox"].Resizable = DataGridViewTriState.False;
            string filePath = "./conf.ini"; // 替换为你的文件路径

            if (!File.Exists(filePath))
            {
                // 如果文件不存在，就创建它
                File.Create(filePath).Close();
            }

            loadfile();
        }

        private void add_Click(object sender, EventArgs e)
        {
            read read = new read();
            read.SelectExeAndWritePathToConf();        
            loadfile();
        }

        private void delele_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                // 保存选中的索引
                int selectedIndex = dataGridView1.SelectedCells[0].RowIndex;

                // 从 DataGridView 中删除选中的行
                dataGridView1.Rows.RemoveAt(selectedIndex);

                // 从文件中删除选中的行
                string filePath = "./conf.ini"; // 替换为你的文件路径
                List<string> lines = File.ReadAllLines(filePath).ToList();
                lines.RemoveAt(selectedIndex);
                File.WriteAllLines(filePath, lines);
            }
        }

        private void refresh__Click(object sender, EventArgs e)
        {
            loadfile();
        }
   
        private void loadfile(int startLineNumber = 1)
        {
            dataGridView1.Rows.Clear(); // 清空 dataGridView1 中的内容
            string filePath = "./conf.ini"; // 替换为你的文件路径
             // int lineNumber = startLineNumber; // 使用传入的行号
             lineNumber = 1;
            using (StreamReader srlineload = new StreamReader(filePath))
            {
               
                string lineload;
                while ((lineload = srlineload.ReadLine()) != null)
                {
                    // 使用 # 分割每一行的内容
                    string[] parts = lineload.Split('#');
                    if (parts.Length == 2)
                    {
                        // 在 dataGridView1 中添加一行，包括行号、文本和一个空的输入框
                        dataGridView1.Rows.Add(lineNumber, parts[0], parts[1]);
                    }
                    else if (parts.Length == 1)
                    {
                        dataGridView1.Rows.Add(lineNumber, parts[0], "0");
                    }
                    lineNumber++; // 行号递增
                }
            }
            

        }
        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            
            if (e.ColumnIndex == 2) // 如果正在验证的是第三列（索引为2）
            {
                
                string input = e.FormattedValue.ToString();
                if (!string.IsNullOrEmpty(input)) // 如果输入不为空
                {
                    double newValue = 0;
                    if (!double.TryParse(input, out newValue) || newValue < 0)
                    {
                        e.Cancel = true; // 取消更改
                        MessageBox.Show("请输入非负数");
                    }
                    
                }
            }
           
        }

     

         private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
         {
             if (e.ColumnIndex == 2 && e.RowIndex < dataGridView1.Rows.Count) // 如果编辑的是第三列（索引为2）且行存在
             {
                 DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                 string newValue = cell.Value != null ? cell.Value.ToString() : "0"; // 如果单元格的值为 null，就默认为 "0"
                 string filePath = "./conf.ini"; // 替换为你的文件路径
                 List<string> lines = File.ReadAllLines(filePath).ToList();
                 if (e.RowIndex < lines.Count) // 如果行在 lines 列表中存在
                 {
                     // 检查行中是否已经有 #
                     int index = lines[e.RowIndex].IndexOf('#');
                     if (index != -1)
                     {
                         // 如果有，就替换掉 # 后面的数据
                         lines[e.RowIndex] = lines[e.RowIndex].Substring(0, index) + "#" + newValue;
                     }
                     else
                     {
                         // 如果没有，就在行尾添加新的数据
                         lines[e.RowIndex] += "#" + newValue;
                     }
       
                     File.WriteAllLines(filePath, lines);
                 }
             }
          
             
         }




        private async void run_Click(object sender, EventArgs e)
        {
            //RunApp runApp = new RunApp();
            // RunApp runApp = new RunApp(txtLog);
            RunButton.Enabled = false;  // 禁用按钮
            StopButton.Enabled = true;  // 开启stop按钮
            await runApp.RunAsync();
            


            //runApp.Run();
        }
     

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (RunButton.Enabled != false) 
            {
                MessageBox.Show("没有由此工具启动的正在运行的程序");
                RunButton.Enabled = true;
                return;
            }
           DialogResult MsgBoxResult;//设置对话框的返回值
           MsgBoxResult = System.Windows.Forms.MessageBox.Show("请踢出全部玩家再关闭，否则会出现玩家数据丢失的情况.", "重要", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);//定义对话框的按钮式样
           if (MsgBoxResult.ToString() == "Yes")//如果对话框的返回值是YES（按"Y"按钮）
           {
             runApp.StopAllProcesses();
                RunButton.Enabled = true;  // 启用按钮
                
                StopButton.Enabled = false;

            }
           if (MsgBoxResult.ToString() == "No")//如果对话框的返回值是NO（按"N"按钮）
           {
               //选择了No，继续
               return;
           } 
                  

           
        }

        private void button5_Click(object sender, EventArgs e)
        {
           

            runApp.Stop();
            runApp.StopAllProcesses();
            RunButton.Enabled = true;
            StopButton.Enabled = false;


        }
    }
}
