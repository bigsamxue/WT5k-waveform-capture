using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using LineSeries = OxyPlot.Series.LineSeries;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WT5000WaveFormCapture.Models;
using System.Threading;
using WT5000WaveFormCapture.Services;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Text.RegularExpressions;
//using OxyPlot.Wpf;

namespace WT5000WaveFormCapture.ViewModels {
    internal class WaveFormCaptureWindowVM : BindableBase {
        private ObservableCollection<CheckBoxItem> checkBoxVoltageItems;

        public ObservableCollection<CheckBoxItem> CheckBoxVoltageItems {
            get { return checkBoxVoltageItems; }
            set { SetProperty(ref checkBoxVoltageItems, value); }
        }

        private ObservableCollection<CheckBoxItem> checkBoxCurrentItems;

        public ObservableCollection<CheckBoxItem> CheckBoxCurrentItems {
            get { return checkBoxCurrentItems; }
            set { SetProperty(ref checkBoxCurrentItems, value); }
        }

        private ObservableCollection<CheckBoxItem> checkBoxToqSpeItems;

        public ObservableCollection<CheckBoxItem> CheckBoxToqSpeItems {
            get { return checkBoxToqSpeItems; }
            set { SetProperty(ref checkBoxToqSpeItems, value); }
        }

        private ObservableCollection<CheckBoxItem> checkBoxAuxItems;

        public ObservableCollection<CheckBoxItem> CheckBoxAuxItems {
            get { return checkBoxAuxItems; }
            set { SetProperty(ref checkBoxAuxItems, value); }
        }

        public Dictionary<string, List<string>> Dict { get; set; }
        public string CheckInfo { get; set; }

        private PlotModel _model;

        public PlotModel Model {
            get { return _model; }
            set { SetProperty(ref _model, value); }
        }


        public List<LineSeries> LineSeriesList { get; set; }

        public List<List<double>> DoubleWaveData { get; set; }


        private int count;

        public int Count {
            get { return count; }
            set { SetProperty(ref count, value); }
        }



        public WriteToCSV WriteToCSV { get; set; }

        public DelegateCommand StartCommand { get; set; }
        public DelegateCommand DisplayCountCommand { get; set; }

        public WaveFormCaptureWindowVM() {

            Model = new PlotModel() { Title = "WaveFormCapture" };

            WriteToCSV = new WriteToCSV();


            CheckBoxVoltageItems = new ObservableCollection<CheckBoxItem> {
                new CheckBoxItem{Content="U1"},
                new CheckBoxItem{Content="U2"},
                new CheckBoxItem{Content="U3"},
                new CheckBoxItem{Content="U4"},
                new CheckBoxItem{Content="U5"},
                new CheckBoxItem{Content="U6"},
                new CheckBoxItem{Content="U7"},
            };
            CheckBoxCurrentItems = new ObservableCollection<CheckBoxItem> {
                new CheckBoxItem{Content="I1"},
                new CheckBoxItem{Content="I2"},
                new CheckBoxItem{Content="I3"},
                new CheckBoxItem{Content="I4"},
                new CheckBoxItem{Content="I5"},
                new CheckBoxItem{Content="I6"},
                new CheckBoxItem{Content="I7"},
            };
            CheckBoxToqSpeItems = new ObservableCollection<CheckBoxItem> {
                new CheckBoxItem{Content="Torq1"},
                new CheckBoxItem{Content="Torq2"},
                new CheckBoxItem{Content="Torq3"},
                new CheckBoxItem{Content="Torq4"},
                new CheckBoxItem{Content="Speed1"},
                new CheckBoxItem{Content="Speed2"},
                new CheckBoxItem{Content="Speed3"},
                new CheckBoxItem{Content="Speed4"},
            };
            CheckBoxAuxItems = new ObservableCollection<CheckBoxItem> {
                new CheckBoxItem{Content="Aux1"},
                new CheckBoxItem{Content="Aux2"},
                new CheckBoxItem{Content="Aux3"},
                new CheckBoxItem{Content="Aux4"},
                new CheckBoxItem{Content="Aux5"},
                new CheckBoxItem{Content="Aux6"},
                new CheckBoxItem{Content="Aux7"},
                new CheckBoxItem{Content="Aux8"},
            };

            StartCommand = new DelegateCommand(GetWaveFormAndWriteCSV);
            DisplayCountCommand = new DelegateCommand(DisplayCheckedCount);
        }

        /// <summary>
        /// 获取CheckBox选中项，在前面添加"Date",即string="Date,U1,I2..."
        /// </summary>
        /// <returns></returns>
        public string GetCheckBoxItem() {
            string info = "";
            if (CheckBoxVoltageItems != null && CheckBoxVoltageItems.Count > 0) {
                var listU = CheckBoxVoltageItems.Where(p => p.IsChecked);
                if (listU.Count() > 0) {
                    foreach (var U in listU) {
                        info += U.Content + ",";
                    }

                }
            }
            if (CheckBoxCurrentItems != null && CheckBoxCurrentItems.Count > 0) {
                var listI = CheckBoxCurrentItems.Where(p => p.IsChecked);
                if (listI.Count() > 0) {
                    foreach (var I in listI) {
                        info += I.Content + ",";
                    }
                }
            }
            if (CheckBoxToqSpeItems != null && CheckBoxToqSpeItems.Count > 0) {
                var listTS = CheckBoxToqSpeItems.Where(p => p.IsChecked);
                if (listTS.Count() > 0) {
                    foreach (var TS in listTS) {
                        info += TS.Content + ",";
                    }
                }
            }
            if (CheckBoxAuxItems != null && CheckBoxAuxItems.Count > 0) {
                var listAux = CheckBoxAuxItems.Where(p => p.IsChecked);
                if (listAux.Count() > 0) {
                    foreach (var Aux in listAux) {
                        info += Aux.Content + ",";
                    }
                }
            }
            if (!string.IsNullOrEmpty(info)) {
                info = info.TrimEnd(',');
                info = "Date," + info;
            }
            return info;
        }

        /// <summary>
        /// 将Date和数据的存入字典，CheckBoxInfo作为键，获取的WaveForm为值(WT默认波形共2002点)
        /// </summary>
        /// <param name="checkInfoArray"></param>
        /// <returns></returns> 
        public Dictionary<string, List<string>> GetItemData(string info) {
            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
            string[] infoArray = new string[40];
            infoArray = info.Split(',');
            //Array.Resize(ref infoArray, infoArray.Length + 1);
            //Array.Copy(infoArray, 0, infoArray, 1, infoArray.Length - 1);
            //infoArray[0] = "Date";
            foreach (var item in infoArray) {
                if (item != "Date") {

                    App.WT5k.RemoteCTRL($":WAVEFORM:TRACE {item}");
                    string itemData = App.WT5k.RemoteCTRL(":WAVeform:SEND?");
                    if (!string.IsNullOrEmpty(itemData)) {
                        double[] doubleitemData = ConvertValue.ValueConvert(itemData);
                        List<string> list = new List<string>();
                        list = doubleitemData.Select(d => d.ToString()).ToList();
                        dict.Add(item, list);
                    }
                }
                else {
                    string date = App.WT5k.RemoteCTRL(":SYSTem:DATE?");
                    date = Regex.Replace(date, @"[\r\n]", "");
                    string time = App.WT5k.RemoteCTRL(":SYSTem:TIME?");
                    time = Regex.Replace(time, @"[\r\n]", "");
                    string dateTime = date + " " + time;
                    List<string> list = new List<string>();
                    list.Add(dateTime);
                    for (int i = 0; i < 2001; i++) {
                        list.Add("");
                    }
                    dict.Add(item, list);
                }
            }
            return dict;
        }

        public void SaveWTWaveToCSV() {
            CheckInfo = GetCheckBoxItem();
            Dict = new Dictionary<string, List<string>>();
            Dict = GetItemData(CheckInfo);
            WriteToCSV.CreateFile(CheckInfo);
            WriteToCSV.WriteCsv(Dict);
        }


        public void PlotItemData(Dictionary<string, List<string>> dict) {
            ClearCurve();
            Model = new PlotModel() { Title = "WaveFormCapture" };
            //LineSeriesList = new List<LineSeries>();
            //DoubleWaveData = new List<List<double>>();

            //for (int i = 1; i < dict.Count; i++) {
            //    var series = new LineSeries { Title = $"{dict.Keys.ElementAt(i)}", MarkerType = MarkerType.None, Smooth = true };
            //    Model.Series.Add(series);
            //    LineSeriesList.Add(series);
            //}
            //for (int i = 0; i < dict.Values.Max(d => d.Count); i++) {
            //    List<double> list = new List<double>();
            //    list = dict.Values.ElementAt(i).Select(s => double.Parse(s)).ToList();
            //    DoubleWaveData.Add(list);
            //}

            for (int i = 1; i < dict.Count; i++) {
                var series = new LineSeries { Title = $"{dict.Keys.ElementAt(i)}", MarkerType = MarkerType.None, Smooth = true };

                List<double> list = new List<double>();
                list = dict.Values.ElementAt(i).Select(s => double.Parse(s)).ToList();
                for (int j = 0; j < list.Count; j++) {
                    series.Points.Add(new DataPoint(j, list[j]));
                    //series.Points.Add(DateTimeAxis.CreateDataPoint(dateTime, list[j]));
                    Thread.Sleep(1);
                }
                Model.Series.Add(series);
            }

            Model.InvalidatePlot(true);

        }

        public void ClearCurve() {
            //遍历，清除所有之前绘制的曲线
            foreach (var lineSer in Model.Series) {
                ((LineSeries)lineSer).Points.Clear();
            }
            //清除完曲线之后，重新刷新坐标轴

            Model.InvalidatePlot(true);
            Model.Series.Clear();
            Thread.Sleep(10);
        }

        public void GetWaveFormAndWriteCSV() {
            if (App.WT5k == null || App.WT5k.IsConnected != true) {
                MessageBox.Show("请先连接仪器", "警告", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
            }
            else {
                //Get waveform and save csv
                SaveWTWaveToCSV();
                PlotItemData(Dict);
            }
        }

        public void DisplayCheckedCount() {
            Count = CheckBoxVoltageItems.Count(p => p.IsChecked == true) + CheckBoxCurrentItems.Count(p => p.IsChecked == true) + CheckBoxToqSpeItems.Count(p => p.IsChecked == true) + checkBoxAuxItems.Count(p => p.IsChecked == true);
        }

        public class CheckBoxItem : BindableBase {
            private bool isChecked;

            public bool IsChecked {
                get { return isChecked; }
                set { SetProperty(ref isChecked, value); }
            }
            private string content;

            public string Content {
                get { return content; }
                set { SetProperty(ref content, value); }
            }


        }
    }
}
