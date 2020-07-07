using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GMM
{
    namespace Utils
    {
        public static class Tasks
        {
            public static async Task Blink()
            {
                await Task.Yield();
                // Task.Delay(TimeSpan.FromMilliseconds(1));
            }
        }
    }
}