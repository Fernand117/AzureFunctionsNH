using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Azure.Messaging.EventHubs.Consumer;
using AzureFunctionsNH.Models;
using Newtonsoft.Json;

namespace AzureFunctionsNH.Services
{
    internal class IoTHub
    {
        private static readonly string consumerGroup = "dv-iot-test-group";
        private static readonly string deviceID = "dv-iot-test";
        private static readonly string connectionString = $"Endpoint=sb://ihsuprodblres061dednamespace.servicebus.windows.net/;SharedAccessKeyName=iothubowner;SharedAccessKey=/MtSqKBVqnPSdoWKW7sKOItCPcd4a7j/x4PwD2gnKBk=;EntityPath=iothub-ehub-dev-iot-te-25136175-05eac7bc52";

        private static ThreadStart IoTHubThreadStart;
        private static Thread IoTHubThread;

        public static DateTimeOffset EventHubOffset = DateTime.Now;
        public static bool isEventHubOffsetInit = false;
        public static List<IoTHubData> IoTHubDataList = new();
        
        public static void RunProcess()
        {
            try
            {
                if (IoTHubThread == null)
                {
                    IoTHubThreadStart = new ThreadStart(IoTHubProcess);
                    IoTHubThread = new Thread(IoTHubThreadStart);
                    IoTHubThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }

        private static async void IoTHubProcess()
        {
            try
            {
                while (true)
                {
                    if (isEventHubOffsetInit)
                    {
                        await using EventHubConsumerClient consumerClient = new EventHubConsumerClient(consumerGroup, connectionString);

                        EventPosition eventPosition = EventPosition.FromEnqueuedTime(EventHubOffset);
                        string partitionId = (await consumerClient.GetPartitionIdsAsync())[0];

                        await foreach (PartitionEvent partitionEvent in consumerClient.ReadEventsFromPartitionAsync(
                                           partitionId, eventPosition))
                        {
                            partitionEvent.Data.SystemProperties.TryGetValue("iothub-connection-device-id",
                                out object currectDeviceId);

                            if (currectDeviceId.Equals(deviceID))
                            {
                                DateTime dateTime = partitionEvent.Data.EnqueuedTime.LocalDateTime;

                                IoTHubData ioTHubData = new IoTHubData();
                                IoTHubData lastIoTData = IoTHubDataList.LastOrDefault();
                                string json = Encoding.UTF8.GetString(partitionEvent.Data.Body.ToArray());

                                if (dateTime > (lastIoTData != null ? lastIoTData.dateTime.AddSeconds(3) : DateTime.MinValue))
                                {
                                    ioTHubData = JsonConvert.DeserializeObject<IoTHubData>(json);
                                    ioTHubData.dateTime = dateTime;
                                    IoTHubDataList.Add(ioTHubData);
                                }
                                else
                                {
                                    ioTHubData = lastIoTData;
                                    IoTHubData newIoTHubData = JsonConvert.DeserializeObject<IoTHubData>(json);

                                    foreach (Value value in newIoTHubData.values)
                                    {
                                        ioTHubData.values.Add(value);
                                    }
                                }

                                EventHubOffset = partitionEvent.Data.EnqueuedTime;
                            }
                        }
                        
                        await consumerClient.DisposeAsync();
                    }
                    
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR_IOT_PROCCESS: " + ex.Message);
            }
            finally
            {
                IoTHubThread = null;
                Thread.Sleep(1000);
                RunProcess();
            }
        }
    }
}