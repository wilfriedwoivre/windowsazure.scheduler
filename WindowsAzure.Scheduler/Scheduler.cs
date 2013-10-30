using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace WindowsAzure.Scheduler
{
    public class Scheduler
    {
        private static Scheduler _instance;
        private string _storageConnectionString;
        private string _queueName = "";
        private readonly static JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects
        };

        internal Scheduler(string storageConnectionString)
        {
            this._storageConnectionString = storageConnectionString;
            SetQueueName();
        }

        private void SetQueueName()
        {
            try
            {
                var value = CloudConfigurationManager.GetSetting("WindowsAzure.Scheduler.QueueName");
                if (string.IsNullOrWhiteSpace(value))
                    value = "wilfriedwoivre4windowsazurescheduler";
                _queueName = value;
            }
            catch { }

        }

        public void AddAction(SchedulingAction actionToAdd)
        {
            if (actionToAdd == null)
            {
                throw new ArgumentNullException("actionToAdd");
            }

            var queue = GetQueue();
            queue.AddMessage(new CloudQueueMessage(Serialize(actionToAdd)), initialVisibilityDelay: actionToAdd.Delay);
        }

        /// <summary>
        /// Get an action
        /// </summary>
        /// <param name="application">You can specify an action to use the same scheduler in multiple applications</param>
        /// <returns>null if not action is application, else a action</returns>
        public SchedulingAction GetAction(string application = null)
        {
            var queue = GetQueue();
            var message = queue.GetMessage();

            if (message == null) return null;
            var result = Deserialize<SchedulingAction>(message.AsString);

            if (!string.IsNullOrWhiteSpace(application))
            {
                if (application != result.ApplicationName)
                    return null;
            }

            queue.DeleteMessage(message);

            return result;
        }

        /// <summary>
        /// Provides the static instance of a scheduler
        /// </summary>
        /// <param name="storageConnectionString">Table Storage is used to create a queue to schedule actions</param>
        public static Scheduler GetInstance(string storageConnectionString)
        {
            if (_instance == null)
            {
                EnsureStorageConnectionIsValid(storageConnectionString);
                _instance = new Scheduler(storageConnectionString);
            }

            return _instance;
        }

        /// <summary>
        /// Execute action and schedule her if delay is not null
        /// </summary>
        /// <param name="application"></param>
        public void ExecuteAction(string application = null)
        {
            var action = GetAction(application);

            if (action != null)
            {
                try
                {
                    action.DoWork();

                    if (action.Delay != null)
                    {
                        AddAction(action);
                    }
                }
                catch (Exception ex)
                {
                    AddAction(action);
                    throw ex;
                }
            }
        }

        private static void EnsureStorageConnectionIsValid(string storageConnectionString)
        {
            if (string.IsNullOrWhiteSpace(storageConnectionString))
                throw new ArgumentNullException("storageConnectionString", "Storage connection string is required");

            CloudStorageAccount storageAccount = null;
            if (!CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                throw new ArgumentException("Storage connection string is invalid");
            }

            var tableClient = storageAccount.CreateCloudTableClient();
            try
            {
                tableClient.ListTablesSegmented(string.Empty, 1, null);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Storage connection string is incorrect");
            }
        }

        private CloudQueue GetQueue()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_storageConnectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(_queueName);
            queue.CreateIfNotExists();

            return queue;
        }

        private string Serialize<T>(T entity)
        {
            return JsonConvert.SerializeObject(entity, _serializerSettings);
        }

        private T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, _serializerSettings);
        }
    }
}
