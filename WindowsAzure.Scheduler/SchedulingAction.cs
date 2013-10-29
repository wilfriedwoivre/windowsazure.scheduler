using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsAzure.Scheduler
{
    public abstract class SchedulingAction
    {
        /// <summary>
        /// Name of this action
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Delay to the next execution
        /// </summary>
        public TimeSpan? Delay { get; set; }

        /// <summary>
        /// Application Name (can be null, but i advise if you use the same table storage between multiple applications)
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Action to execute
        /// </summary>
        public abstract void DoWork();
    }
}
