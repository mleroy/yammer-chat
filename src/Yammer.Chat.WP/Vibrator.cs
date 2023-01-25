using Microsoft.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core;

namespace Yammer.Chat.WP
{
    public class Vibrator : IVibrator
    {
        public void Vibrate(TimeSpan duration)
        {
            VibrateController.Default.Start(duration);
        }
    }
}
