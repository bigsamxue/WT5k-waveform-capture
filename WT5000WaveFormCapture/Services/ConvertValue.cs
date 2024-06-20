using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WT5000WaveFormCapture.Services {
    public static class ConvertValue {
        public static double[] ValueConvert(string oriData) {
            if (oriData.Contains("Error") | string.IsNullOrWhiteSpace(oriData)) return null;
            return Array.ConvertAll<string, double>(oriData.Split(','), double.Parse);
        }
    }
}
