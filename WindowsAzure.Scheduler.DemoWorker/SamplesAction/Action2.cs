using System;
using System.Diagnostics;

namespace WindowsAzure.Scheduler.DemoWorker.SamplesAction
{
    public class Action2 : SchedulingAction
    {
        public override void DoWork()
        {
            TableStorageLog.WriteLog(Name);
        }
    }
}