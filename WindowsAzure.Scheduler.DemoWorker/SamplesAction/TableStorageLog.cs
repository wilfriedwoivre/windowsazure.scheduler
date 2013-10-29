using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Diagnostics.Management;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace WindowsAzure.Scheduler.DemoWorker.SamplesAction
{
    public static class TableStorageLog
    {
        public static void WriteLog(string value)
        {
            var csa = CloudStorageAccount.DevelopmentStorageAccount;

            var tableClient = csa.CreateCloudTableClient();
            var table = tableClient.GetTableReference("Logs");
            table.CreateIfNotExists();

            var insertOperation = TableOperation.Insert(new Entity() { PartitionKey = "Logs", RowKey = (DateTime.MaxValue.Ticks - DateTime.Now.Ticks).ToString(), Value = value, Date = DateTime.Now });

            table.Execute(insertOperation);
        }
    }


    public class Entity : TableEntity
    {
        public string Value { get; set; }

        public DateTime Date { get; set; }
    }
}
