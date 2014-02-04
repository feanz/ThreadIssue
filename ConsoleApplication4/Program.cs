using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Experience.CloudFx.Framework;
using Microsoft.Experience.CloudFx.Framework.Storage;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;

namespace ConsoleApplication4
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				var process = new ActivityProcessor();
				process.Process();
			}
			catch (Exception ex)
			{
				throw;
			}

			Console.ReadLine();
		}
	}

	public class ActivityProcessor
	{
		public TableProcessor TableProcessor { get; set; }
		public ActivityProcessor()
		{
			TableProcessor = new TableProcessor();
		}

		public void Process()
		{
			Task.WhenAll(ProcessCacheLoop(),
				ProcessBloblLoop(),
				ProcessTableLoop());
		}

		public async Task ProcessCacheLoop()
		{
			while (true)
			{


				await TaskEx.Delay(TimeSpan.FromSeconds(1));
			}
		}

		public async Task ProcessBloblLoop()
		{
			while (true)
			{


				await TaskEx.Delay(TimeSpan.FromSeconds(2));
			}
		}

		public async Task ProcessTableLoop()
		{
			while (true)
			{
				TableProcessor.Process();

				await TaskEx.Delay(TimeSpan.FromSeconds(1));
			}
		}
	}

	public class TableProcessor : ActivityProcessorBase
	{
		private string CustomerTableName = "customer";

		public TableProcessor()
		{
			TableStorage.CreateTable(CustomerTableName);
		}

		public void Process()
		{
			Update();
		}

		private void Update()
		{
			var entities = new List<CustomerEntity>();

			for (int i = 0; i < 15; i++)
			{
				entities.Add(new CustomerEntity("Smtih", "John") { Email = "Email@email.com", PhoneNumber = "01434604675" });
			}

			TableStorage.Add(CustomerTableName, entities, CloudUtility.MaxTableEntityBatchSize);
		}
	}

	public class CustomerEntity : ICloudTableEntity
	{
		public string FirstName { get; set; }

		public CustomerEntity(string lastName, string firstName)
		{
			FirstName = firstName;
			PartitionKey = lastName;
			RowKey = Guid.NewGuid().ToString();
		}

		public CustomerEntity() { }

		public string Email { get; set; }

		public string PhoneNumber { get; set; }
		public string PartitionKey { get; set; }
		public string RowKey { get; set; }
	}

	public abstract class ActivityProcessorBase
	{
		private ReliableCloudTableStorage activityTableStorage;
		
		protected ReliableCloudTableStorage TableStorage
		{
			get
			{
				if (activityTableStorage == null)
				{
					var tableStorageAccount = CloudStorageAccount.DevelopmentStorageAccount;
					activityTableStorage = new ReliableCloudTableStorage(tableStorageAccount);
				}

				return activityTableStorage;
			}
		}
	}
}
