using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WT5000WaveFormCapture.Services {
    /// <summary>
    /// 创建csv文件，写入csv(默认第一列为Date)。注：App.RelativePath、App.FileName变量必须在App.xaml.cs有
    /// </summary>
    public class WriteToCSV {

        public static void CreateFile(string header) {
            string fileCreateTime = DateTime.Now.ToString("yyyyMMdd HHmmss");

            App.FileName = App.RelativePath + "/DataFile" + fileCreateTime + ".csv";
            if (!Directory.Exists(App.RelativePath)) {
                Directory.CreateDirectory(App.RelativePath);
            }

            if (!File.Exists(App.FileName)) {
                StreamWriter sw = new StreamWriter(App.FileName, true, Encoding.UTF8);
                //header格式应为eg："时间,编号,姓名,年龄"
                sw.WriteLine(header);
                sw.Flush();
                sw.Close();
            }
        }

        /// <summary>
        /// 传参DIctionary，其中字典的值对应header的一列
        /// </summary>
        /// <param name="dict"></param>
        public static void WriteCsv(Dictionary<string, List<string>> dict) {
            StreamWriter swl = new StreamWriter(App.FileName, true, Encoding.UTF8);
            for (int i = 0; i < dict.Values.Max(d => d.Count); i++) {

                string line = string.Join(",", dict.Values.Select(d => d.ElementAtOrDefault(i).ToString()));
                swl.WriteLine(line);


            }
            swl.Flush();
            swl.Close();
        }
    }
}
