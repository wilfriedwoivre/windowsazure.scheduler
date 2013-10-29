using System;
using System.Diagnostics;

namespace WindowsAzure.Scheduler.DemoWorker.SamplesAction
{
    public class Action4 : SchedulingAction
    {
        public override void DoWork()
        {
            TableStorageLog.WriteLog(Name);
        }
    }
}