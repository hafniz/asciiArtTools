﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCIIsome
{
    public static class Plotter
    {
        public static void Plot(ViewModel currentConfig)
        {
            Echo(currentConfig);
            if (currentConfig.ImgSource != null)
            {
                // (Re)plot the char graph
            }
        }

        private static void Echo(ViewModel currentConfig) => currentConfig.CharOut = "CharImgHeight: " + currentConfig.CharImgHeight + Environment.NewLine +
            "CharImgWidth: " + currentConfig.CharImgWidth + Environment.NewLine +
            "ImgSource: " + currentConfig.ImgSource + Environment.NewLine +
            "IsAspectRatioKept: " + currentConfig.IsAspectRatioKept + Environment.NewLine +
            "IsDynamicGrayscaleRangeEnabled: " + currentConfig.IsDynamicGrayscaleRangeEnabled + Environment.NewLine +
            "IsGrayscaleRangeInverted: " + currentConfig.IsGrayscaleRangeInverted + Environment.NewLine +
            "CurrentCharSet: " + currentConfig.CurrentCharSet;
    }
}