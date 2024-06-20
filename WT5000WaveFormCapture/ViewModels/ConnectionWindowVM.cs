using Communicator;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using WT5000WaveFormCapture.Models;

namespace WT5000WaveFormCapture.ViewModels {
    internal class ConnectionWindowVM : BindableBase {
        public ConnectionWindowVM() {
            this.ConnectionModel = new ConnectionModel {
                ComboSelect = "USBTMC3",
                ConnectStatus = "Connect"
            };
            ImageSource = "pack://application:,,,/Icons/Disconnect.png";
            ConnectWay = new List<string>() { "USBTMC3", "VXI11" };

            SearchClickCommand = new DelegateCommand(SearchDevice);
            ConnectClickCommand = new DelegateCommand(ConnectDevice);
            FileClickCommand = new DelegateCommand(ChooseFilePath);
        }


        private string _info;

        public string Info {
            get { return _info; }
            set { SetProperty(ref _info, value); }
        }


        private string _imageSource;

        public string ImageSource {
            get { return _imageSource; }
            set { SetProperty(ref _imageSource, value); }
        }

        private ConnectionModel _connectionModel;

        public ConnectionModel ConnectionModel {
            get { return _connectionModel; }
            set { SetProperty(ref _connectionModel, value); }
        }

        private List<string> _connectWay;

        public List<string> ConnectWay {
            get { return _connectWay; }
            set { SetProperty(ref _connectWay, value); }
        }


        public DelegateCommand SearchClickCommand { get; set; }
        public DelegateCommand ConnectClickCommand { get; set; }
        public DelegateCommand FileClickCommand { get; set; }


        public bool IsSaveFileButton = false;



        public void SearchDevice() {
            if (App.WT5k == null) {
                if (ConnectionModel.ComboSelect == "USBTMC3") {
                    App.WT5k = new Connection((int)Connection.wire.USBTMC3, ConnectionModel.SerialNum);
                }

                else {
                    App.WT5k = new Connection((int)Connection.wire.VXI11, ConnectionModel.SerialNum);
                }
            }
            List<string> dev = App.WT5k.SearchDevice();
            if (dev.Count > 0) {
                for (int i = 0; i < dev.Count; i++) {
                    ConnectionModel.SerialNum = dev[i];
                }

                if (ConnectionModel.ComboSelect == "USBTMC3") {
                    App.WT5k = new Connection((int)Connection.wire.USBTMC3, ConnectionModel.SerialNum);
                }
                else {
                    App.WT5k = new Connection((int)Connection.wire.VXI11, ConnectionModel.SerialNum);
                }
            }
            else {
                ConnectionModel.SerialNum = "";
                System.Windows.MessageBox.Show("No Connection.Please check and search again");
            }
        }

        private void ConnectDevice() {
            if (App.WT5k == null) {
                if (ConnectionModel.ComboSelect == "USBTMC3") {
                    App.WT5k = new Connection((int)Connection.wire.USBTMC3, ConnectionModel.SerialNum);
                }
                else {
                    App.WT5k = new Connection((int)Connection.wire.VXI11, ConnectionModel.SerialNum);
                }
            }
            if (App.WT5k.IsConnected != true) {

                App.WT5k.Connect();
                if (App.WT5k.IsConnected == true) {
                    ConnectionModel.ConnectStatus = "Disconnect";

                    string idn = App.WT5k.RemoteCTRL("*IDN?");
                    Info = idn;
                    ImageSource = "pack://application:,,,/Icons/Connect.png";
                }
                else {
                    string errorcode = App.WT5k.ReportError();
                    System.Windows.MessageBox.Show("No Connection.Error Info：" + errorcode, "警告", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                }
            }
            else {
                ConnectionModel.ConnectStatus = "Connect";
                App.WT5k.Finish();
                ImageSource = "pack://application:,,,/Icons/Disconnect.png";
                App.WT5k = null;
            }
        }

        private void ChooseFilePath() {
            IsSaveFileButton = true;
            FolderBrowserDialog dialog = new FolderBrowserDialog {
                SelectedPath = @"C:"
            };
            dialog.ShowDialog();
            App.RelativePath = dialog.SelectedPath;
            Info = App.RelativePath;
        }


    }
}
