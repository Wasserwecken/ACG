using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Framework
{
    public static class TimeSystem
    {
        private static readonly Stopwatch _totalWatch;
        private static readonly Stopwatch _deltaWatch;

        /// <summary>
        /// 
        /// </summary>
        static TimeSystem()
        {
            _totalWatch = new Stopwatch();
            _deltaWatch = new Stopwatch();

            _totalWatch.Start();
            _deltaWatch.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Update(ref ShaderTime timeData)
        {
            timeData.Total = _totalWatch.ElapsedMilliseconds / 1000f;
            timeData.Delta = _deltaWatch.ElapsedMilliseconds / 1000f;

            timeData.TotalSin = MathF.Sin(timeData.Total);
            timeData.TotalSin01 = timeData.TotalSin * 0.5f + 0.5f;

            _deltaWatch.Restart();
        }
    }
}
