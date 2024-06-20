﻿using Communicator;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WT5000WaveFormCapture {
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application {
        public static Connection WT5k { get; set; }
        public static string FileName { get; set; }
        public static string RelativePath = @"../File Folder";
    }
}
