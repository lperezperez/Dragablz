namespace Dragablz
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using Dragablz.Core;
    /// <summary></summary>
    /// <remarks>
    ///     In supporting .Net 4.0 we don't have access to SystemParameters.WindowGlassBrush, and even then the opacity is not provided, so this class wraps up a few issues around here.
    /// </remarks>
    public static class DragablzColors
    {
        #region Fields
        //TODO: listen to changes from the OS to provide updates
        public static Brush WindowGlassBrush = DragablzColors.GetWindowGlassBrush();
        public static Brush WindowGlassBalancedBrush = DragablzColors.GetBalancedWindowGlassBrush();
        public static Brush WindowInactiveBrush = DragablzColors.GetWindowInactiveBrush();
        #endregion
        #region Methods
        private static bool VistaLess => Environment.OSVersion.Version.Major < 6;
        private static Color BlendColor(Color color1, Color color2, double percentage)
        {
            percentage = Math.Min(100, Math.Max(0, percentage));
            return Color.FromRgb(DragablzColors.BlendColorChannel(color1.R, color2.R, percentage), DragablzColors.BlendColorChannel(color1.G, color2.G, percentage), DragablzColors.BlendColorChannel(color1.B, color2.B, percentage));
        }
        private static byte BlendColorChannel(double channel1, double channel2, double channel2Percentage) => Math.Min((byte)Math.Round(channel1 + (channel2 - channel1) * channel2Percentage / 100D), (byte)255);
        private static Brush GetBalancedWindowGlassBrush()
        {
            if (VistaLess)
                return SystemColors.ActiveCaptionBrush;
            var colorizationParams = new Native.DWMCOLORIZATIONPARAMS();
            Native.DwmGetColorizationParameters(ref colorizationParams);
            var frameColor = DragablzColors.ToColor(colorizationParams.ColorizationColor);
            var blendedColor = DragablzColors.BlendColor(frameColor, SystemColors.ControlColor, 100f - colorizationParams.ColorizationColorBalance);
            return new SolidColorBrush(blendedColor);
        }
        private static Brush GetWindowGlassBrush()
        {
            if (VistaLess)
                return SystemColors.ActiveCaptionBrush;
            var colorizationParams = new Native.DWMCOLORIZATIONPARAMS();
            Native.DwmGetColorizationParameters(ref colorizationParams);
            var frameColor = DragablzColors.ToColor(colorizationParams.ColorizationColor);
            return new SolidColorBrush(frameColor);
        }
        private static Brush GetWindowInactiveBrush() => new SolidColorBrush(SystemColors.MenuBarColor);
        private static Color ToColor(uint value) => Color.FromArgb(255, (byte)(value >> 16), (byte)(value >> 8), (byte)value);
        #endregion
    }
}