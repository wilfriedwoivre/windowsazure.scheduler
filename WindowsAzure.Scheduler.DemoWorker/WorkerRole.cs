using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using WindowsAzure.Scheduler.DemoWorker.SamplesAction;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

namespace WindowsAzure.Scheduler.DemoWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("WindowsAzure.Scheduler.DemoWorker entry point called", "Information");

            var scheduler = Scheduler.GetInstance(CloudStorageAccount.DevelopmentStorageAccount.ToString());

            // Add sample action : 
            scheduler.AddAction(new Action1() { Name = "Action 1", Delay = new TimeSpan(0, 0, 30) });
            scheduler.AddAction(new Action2() { Name = "Action 2", Delay = new TimeSpan(0, 2, 0) });
            scheduler.AddAction(new Action3() { Name = "Action 3", Delay = new TimeSpan(0, 5, 0) });
            scheduler.AddAction(new Action4() { Name = "Action 4" });

            while (true)
            {
                try
                {
                    scheduler.ExecuteAction();
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Working", string.Format("Error{0}{1}", Environment.NewLine, ex));
                }
                Thread.Sleep(500);
                Trace.TraceInformation("Working", "Information");
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}
