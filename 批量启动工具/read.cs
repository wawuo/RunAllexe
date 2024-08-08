using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 批量启动工具
{
    internal class read
    {
        public void SelectExeAndWritePathToConf()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "exe files (*.exe)|*.exe";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string exePath = openFileDialog.FileName;
                WritePathToConf(exePath);
            }
        }

        private static void WritePathToConf(string path)
        {
            using (StreamWriter sw = new StreamWriter("conf.ini", true))
            {
                sw.WriteLine(path);
            }
        }



        public override string ToString()
        {
            return base.ToString();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
