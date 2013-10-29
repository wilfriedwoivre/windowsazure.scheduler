using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsAzure.Scheduler.DemoWorker.SamplesAction
{
    public class Action1 : SchedulingAction
    {
        public override void DoWork()
        {
            TableStorageLog.WriteLog(Name);
        }
    }
}
