using System;
using System.Diagnostics;

namespace WindowsAzure.Scheduler.DemoWorker.SamplesAction
{
    public class Action3 : SchedulingAction
    {
        public override void DoWork()
        {
            TableStorageLog.WriteLog(Name);
        }
    }
}