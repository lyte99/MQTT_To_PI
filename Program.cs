using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace mqtt_to_PI
{


    class Program
    {




        //global tag list so we dont have to check the server each time..
        //public static List<string> Taglist { get; set; }

        //Get the default pi server
        //public static PIServer MyPI = PIServers.GetPIServers().DefaultPIServer;

        static void Main(string[] args)
        {
            //Taglist = new List<string>();
            //Taglist.Add("something");

            //connect to the pi server
            //MyPI.Connect();


            //create client instance(s)


            //Andy's
            //int AndsePort = 8883;
            //string AndseHost = "";
            ////string AndseHost = "";
            ////cert path
            //string andseCertpath = @"\certs\andse\";
            ////system ca cert
            //X509Certificate AcaCert = new X509Certificate();
            //AcaCert.Import("mosq-andse-ca.crt");
            ////client cert
            //X509Certificate AclientCert = new X509Certificate();
            //AclientCert.Import("client.crt");
            ////TLS
            //MqttClient AndseClient = new MqttClient(AndseHost, AndsePort, true, AcaCert, AclientCert, MqttSslProtocols.TLSv1_2, new RemoteCertificateValidationCallback(ValidateServerCertificate));

            //////insecure 
            //////AndsePort = 1883;
            //////MqttClient AndseClient = new MqttClient(AndseHost, AndsePort, false, null, null, MqttSslProtocols.None);

            //////Andy's Stuff
            //Andse_Connect(AndseClient);


            //Brian's
            //int MunkeePort = 2222;
            //string MunkeeHost = "munkee.game-host.org";
            //MqttClient MunkeeClient = new MqttClient(MunkeeHost, MunkeePort, false, null, null, MqttSslProtocols.None);


            ////Brian's Stuff
            //Munkee_Connect(MunkeeClient);


            //TEC
            int TECPort = 1883;
            string TECHost = "tecmqttbroker.tecsystemsgroup.com";
            MqttClient TECClient = new MqttClient(TECHost, TECPort, false, null, null, MqttSslProtocols.None);

            //TEC Stuff
            TEC_Connect(TECClient);


            //MqttClient TestClient = TestClientCreation();
            //Test_Connect(TestClient);

            // MyPI.Disconnect();


        }

        private static MqttClient TestClientCreation()
        {
            //test
            int TestPort = 1883;
            string TestHost = "";
            MqttClient TestClient = new MqttClient(TestHost, TestPort, false, null, null, MqttSslProtocols.None);

            return TestClient;
            
        }

        private static bool Test_Connect(MqttClient TestClient)
        {
            bool connected = false;

            //register client
            TestClient.MqttMsgPublishReceived += TestClient_MqTTMsgPublishRecieved;


            string clientID = Guid.NewGuid().ToString();

            //client connection
            try
            {
                TestClient.Connect(clientID);

                Console.WriteLine("Connected to PI3-1");

                connected = true;

            }
            catch (Exception e)
            {
                DateTime plus30min = System.DateTime.Now;//.AddMinutes(30);
                bool failedtoconnect = true;

                while (failedtoconnect)
                {
                    Console.WriteLine("Trying to connect" + System.DateTime.Now.ToString());

                    if (DateTime.Compare(System.DateTime.Now, plus30min) == 0)
                    {
                        try
                        {
                            TestClient.Connect(clientID);

                            failedtoconnect = false;

                        }
                        catch (Exception z)
                        {

                            Console.WriteLine("Failed to Reconnected to PI3-1");

                            plus30min = System.DateTime.Now.AddSeconds(10);//.AddMinutes(30);

                            Console.WriteLine("Plus30min: " + plus30min.ToString());

                            Console.ReadKey();
                        }
                        

                    }


                }
            }

             //topic subscription(s)
           TestClient.Subscribe(new string[] { "#" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });

            TestClient.ConnectionClosed += Test_MqTTConnectionClosed;

            return connected;

        }

        private static void Test_MqTTConnectionClosed(object sender, EventArgs e)
        {
            DateTime plus30min = System.DateTime.Now;//.AddMinutes(30);
            bool failedtoconnect = true;

            MqttClient TestClient = TestClientCreation();


            Console.WriteLine("Disconnected from PI3-1");

            //try to connect back 1 time
            try
            {
                Test_Connect(TestClient);

                failedtoconnect = false;
            }
            catch (Exception e1)
            {

                Console.WriteLine("Failed to Reconnected to PI3-1");

                plus30min = System.DateTime.Now.AddSeconds(10);//.AddMinutes(30);
            }


            ////if still disconnected, try every 30 mins
            //while (failedtoconnect)
            //{
            //    if (System.DateTime.Now == plus30min)
            //    {
            //        try
            //        {
            //            Test_Connect(TestClient);

            //            failedtoconnect = false;

            //        }
            //        catch (Exception z)
            //        {

            //            Console.WriteLine("Failed to Reconnected to PI3-1");

            //            plus30min = System.DateTime.Now.AddMinutes(30);
            //        }
            //    }
           // }



        }

        private static void TestClient_MqTTMsgPublishRecieved(object sender, MqttMsgPublishEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Message);

            Console.WriteLine("PI3-1 Message Recieved");
            Console.WriteLine(message); 

        }

        private static void Andse_Connect(MqttClient AndseClient)
        {
            //register client
            AndseClient.MqttMsgPublishReceived += AndseClient_MqTTMsgPublishRecieved;


            string clientID = Guid.NewGuid().ToString();

            //client connection
            AndseClient.Connect(clientID, "", "");


            //topic subscription(s)
            AndseClient.Subscribe(new string[] { "#" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE }); 

        }

        private static void Munkee_Connect(MqttClient MunkeeClient)
        {
            //register client
            MunkeeClient.MqttMsgPublishReceived += Munkee_MqTTMsgPublishRecieved;

            MunkeeClient.ConnectionClosed += Munkee_MqTTConnectionClosed;

            string clientID = Guid.NewGuid().ToString();

            //client connection
            MunkeeClient.Connect(clientID, "", "");


            //topic subscription(s)
            MunkeeClient.Subscribe(new string[] { "#" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE }); 
             

        }



        private static void Munkee_MqTTConnectionClosed(object sender, EventArgs e)
        {
            //update the monitoring tag to trigger a notification
            UpdatePIPoint("PI.Munkee.MQTT.Monitor_From_App", "1");


        }


        static void AndseClient_MqTTMsgPublishRecieved(object sender, MqttMsgPublishEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Message);
            string topic = e.Topic.ToString();//Encoding.UTF8.GetString(e.Topic);
            

            Console.WriteLine("Mqtt Message Recieved from andse");

            Console.WriteLine("Topic: " + topic + ". Message: " + message);

            ////CHECK FOR THE TAG THEN UPDATE IT
            string tagname = CheckTag(topic, message);

            UpdatePIPoint(tagname, message);


            
    }


            
        static void Munkee_MqTTMsgPublishRecieved(object sender, MqttMsgPublishEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Message);
            string topic = e.Topic.ToString();//Encoding.UTF8.GetString(e.Topic);

            Console.WriteLine("Mqtt Message Recieved from munkee");

            Console.WriteLine("Topic: " + topic + ". Message: " + message);

            //CHECK FOR THE TAG THEN UPDATE IT
            string tagname = CheckTag(topic, message);

            UpdatePIPoint(tagname, message);


            //reset the monitoring alarm tag
            UpdatePIPoint("PI.Munkee.MQTT.Monitor_From_App", "0");



        }

        private static void TEC_Connect(MqttClient TECClient)
        {
            //register client
            TECClient.MqttMsgPublishReceived += TEC_MqTTMsgPublishRecieved;

            TECClient.ConnectionClosed += Munkee_MqTTConnectionClosed;

            string clientID = Guid.NewGuid().ToString();

            //client connection
            TECClient.Connect(clientID);


            //topic subscription(s)
            TECClient.Subscribe(new string[] { "#" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });


        }

        static void TEC_MqTTMsgPublishRecieved(object sender, MqttMsgPublishEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Message);
            string topic = e.Topic.ToString();//Encoding.UTF8.GetString(e.Topic);

            Console.WriteLine("Mqtt Message Recieved from TEC");

            Console.WriteLine("Topic: " + topic + ". Message: " + message);

            //CHECK FOR THE TAG THEN UPDATE IT
            string tagname = CheckTag(topic, message);

            UpdatePIPoint(tagname, message);



        }


        static void UpdatePIPoint(string PIPointName, string value)
        {
            Console.WriteLine("Attemping PI Point Update for: " + PIPointName);
            //bool usestring = false;

            try
            {
                //convert the value to a double
                double PIDbValue;
                    
                    if (double.TryParse(value, out PIDbValue))
                    {
                        //PIDbValue = Convert.ToDouble(value);
                    }
                    else
                    {
                        //usestring = true;
                        value = "'" + value + "'";
                    }


                //get the pipoint info

                //pi tag  search
                //string URL = "https://PISERVERNAME/piwebapi/search/query?q=name:" + PIPointName; //LEGACY WAY NO LONGER WORKS IN PIWEBAPI 2019 SP1 +;
                string URL = "https://PISERVERNAME/piwebapi/points?path=\\\\PISERVERNAME" + "\\" + PIPointName;

                //not using kerberos authentication
                NetworkCredential credentials = GetNetworkCredentials();

                //tag search
                Task<string> task = Task.Run(async () => await GetPIDataAsync(URL, credentials));
                task.Wait();
                string content = task.Result;
                PITagSearchResults.Root searchResults = JsonConvert.DeserializeObject<PITagSearchResults.Root>(content);


                //update pointsource since I can't seem to do it on create....
                string URL_UpdateTag = "https://PISERVERNAME/piwebapi/streams/" + searchResults.WebId + "/value";

                string jsonContent = @"{'Timestamp': '*','Good': true, 'Questionable': false, 'Value': " + value + "}";

                Task<string> task1 = Task.Run(async () => await PostPIDataAsync(URL_UpdateTag, credentials, jsonContent, "POST"));
                task1.Wait();

            }
            catch (Exception e)
            {
                Console.WriteLine("Error Saving PI Data for PI Point" + PIPointName + ".  Value: " + value.ToString() +".  Message: "  + e.Message);
            }


            //OLD CODE

            //try
            //{
            //    //convert the value to a double
            //    double PIDbValue;

            //    if (double.TryParse(value, out PIDbValue))
            //    {
            //        PIDbValue = Convert.ToDouble(value);
            //    }
            //    else
            //    {
            //        usestring = true;
            //    }


            //    //Get the default pi server
            //    PIServer MyPI = PIServers.GetPIServers().DefaultPIServer;

            //    //connect to the pi server
            //    //MyPI.Connect();

            //    //set pi point
            //    OSIsoft.AF.PI.PIPoint UpdatePoint = OSIsoft.AF.PI.PIPoint.FindPIPoint(MyPI, PIPointName);


            //    //update the value
            //    if (!usestring)
            //    {
            //        UpdatePoint.UpdateValue(new AFValue(PIDbValue), AFUpdateOption.Insert);
            //    }
            //    else
            //    {
            //        UpdatePoint.UpdateValue(new AFValue(value), AFUpdateOption.Insert);
            //    }


            //    //disconnect from the pi server
            //    //MyPI.Disconnect();
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("Error Saving PI Data for PI Point" + PIPointName + ".  Value: " + value.ToString() + ".  Message: " + e.Message);
            //}



            Console.WriteLine("PI Value Saved");

        }

        static string CheckTag(string topic, string value)
        {
            //init
            string tagName = "";

            //turn the topic into a tag name
            string tagtopic = topic.Replace("/", ".");
            tagName = "MQTT" + tagtopic;

            //check for pi point

            //pi tag  search
            //string URL = "https://PISERVERNAME/piwebapi/search/query?q=name:"+ tagName;  \\LEGACY WAY NO LONGER WORKS IN PIWEBAPI 2019 SP1 +;
            string URL = "https://PISERVERNAME/piwebapi/points?path=\\\\PISERVERNAME" + "\\" +tagName;
            //Console.WriteLine("URL: " + URL);

            //not using kerberos authentication
            NetworkCredential credentials = GetNetworkCredentials();

            //tag search
            Task<string> task = Task.Run(async () => await GetPIDataAsync(URL, credentials));
            task.Wait();
            string content = task.Result;
            PITagSearchResults.Root searchResults = JsonConvert.DeserializeObject<PITagSearchResults.Root>(content);

            //does the tag exsist? If not, create it
            if (searchResults.WebId == null)
            {
                Console.WriteLine("Tag NOT Found:" + tagName);

                string pointType = "Float32";

                //create the tag

                // string or float?
                double PIDbValue;

                if (double.TryParse(value, out PIDbValue))
                {
                    PIDbValue = Convert.ToDouble(value);
                }
                else
                {
                    pointType = "String";

                }


                ////batch controller  COULDN'T GET THE BATCH CONTROLLER TO UPDATE THE POINTSOURCE...
                string batch_URL = "https://PISERVERNAME/piwebapi/batch";

                //batch request string to create a tag, get its info(webID) and update its point source.  Couldn't get the pointsource to set on creation, jus wouldn't work
                //string jsonContent = @"{""CreateTag"" :{""Method"":""POST"",""Resource"":""https://PISERVERNAME/piwebapi/dataservers/F1DSUg9RgGAqQUap9GVsKYIDXwREVWRUxPUDI/points"",""Content"": ""{'Name': '" + tagName + @"','PointClass': 'classic','PointType': '"+ pointType + @"','Future': false}'}""},";
                string jsonContent = @"{""CreateTag"" :{""Method"":""POST"",""Resource"":""https://PISERVERNAME/piwebapi/dataservers/F1DSE7nLCGKZrUidwmI1mNzBhAVEVDUEkx/points"",""Content"": ""{'Name': '" + tagName + @"','PointClass': 'classic','PointType': '" + pointType + @"','Future': false}'}""},";
                
                jsonContent = jsonContent + @"""GetTagInfo"":{""Method"":""GET"",""ParentIDs"":[""CreateTag""],""Parameters"":[""$.CreateTag.Headers.Location""],""Resource"":""{0}""},";
                
                //this errors but it does seem to work..  Just seems to not update for a bit.
                jsonContent = jsonContent + @"""UpdatePointSource"":{""Method"":""PATCH"",""ParentIDs"":[""GetTagInfo""],""Parameters"":[""$.GetTagInfo.Content.WebId""],""Resource"": ""https://PISERVERNAME/piwebapi/points/{0}"", ""Content"": ""{ 'PointSource': 'MQTT'}""}}";

                //post content to run batch commands
                Task<string> task1 = Task.Run(async () => await PostPIDataAsync(batch_URL, credentials, jsonContent, "POST"));
                task1.Wait();

            }
            //old code

            ////check this list first as to not hit the server each time.
            //if (Taglist.Count() > 0 && !Taglist.Contains(tagname))
            //{
            //    bool createstring = false;

            //    ////Get the default pi server
            //    //PIServer MyPI = PIServers.GetPIServers().DefaultPIServer;
            //    ////connect to the pi server
            //    //MyPI.Connect();


            //    try
            //    {
            //        //it will check for the tag and throw an exception if it doesn't find it.  Not the fanciest way, but it works...
            //        PIPoint CheckforPoint = PIPoint.FindPIPoint(MyPI, tagname);

            //        Console.WriteLine("TagFound on server:" + tagname);

            //        //if it hasn't thrown the exception add it to the list so we don't check the server each time.
            //        Taglist.Add(tagname);



            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine("Tag NOT Found:" + tagname);

            //        //create the tag
            //        try
            //        {
            //            //create the tag
            //            MyPI.CreatePIPoint(tagname);
            //            Console.WriteLine("Created tag: " + tagname);


            //            PIPoint CheckforPoint = PIPoint.FindPIPoint(MyPI, tagname);

            //            //string or float?
            //            double PIDbValue;

            //            if (double.TryParse(value, out PIDbValue))
            //            {
            //                PIDbValue = Convert.ToDouble(value);
            //            }
            //            else
            //            {
            //                createstring = true;
            //            }

            //            //set the pointsource
            //            CheckforPoint.SetAttribute(PICommonPointAttributes.PointSource, "MQTT");

            //            if (createstring)
            //            { 
            //                CheckforPoint.SetAttribute(PICommonPointAttributes.PointType, "String");
            //            }
            //            else
            //            {
            //                //default is Float32
            //            }



            //            //save
            //            CheckforPoint.SaveAttributes();


            //        }
            //        catch (Exception e1)
            //        {
            //            Console.WriteLine("Unable to Create tag: " + tagname);
            //            Console.ReadKey();
            //        }






            //        Taglist.Add(tagname);

            //    }

            //    //disconnect from the pi server
            //    //MyPI.Disconnect();
            //}
            //else
            //{
            //    Console.WriteLine("TagFound in list:" + tagname);
            //}


            return tagName;
        }


        public static async Task<string> GetPIDataAsync(string url, NetworkCredential credentials)
        {

            HttpClientHandler clientHandler = new HttpClientHandler();

            //ignores SSL errors, we're using a self signed cert in dev.
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            clientHandler.Credentials = credentials;

            HttpClient _client = new HttpClient(clientHandler);


            string content = "";
            try
            {

                HttpResponseMessage response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    content = await response.Content.ReadAsStringAsync();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\tERROR {0}", ex.Message + ". Inner exception: " + ex.InnerException.Message);
            }

            return content;
        }

        public static async Task<string> PostPIDataAsync(string url, NetworkCredential credentials, string jsonContent, string Type)
        {

            HttpClientHandler clientHandler = new HttpClientHandler();

            //ignores SSL errors, we're using a self signed cert in dev.
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            clientHandler.Credentials = credentials;

            HttpClient _client = new HttpClient(clientHandler);

            string content = "";
            try
            {

                HttpResponseMessage response = new HttpResponseMessage();

                if (Type.ToUpper() == "POST")
                {
                    response = await _client.PostAsync(url, new StringContent(jsonContent, Encoding.UTF8, "application/json"));
                }
                else
                {
                    response = await _client.PutAsync(url, new StringContent(jsonContent, Encoding.UTF8, "application/json"));
                }


                if (response.IsSuccessStatusCode)
                {
                    content = await response.Content.ReadAsStringAsync();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("\tERROR {0}", ex.Message + ". Inner exception: " + ex.InnerException.Message);
            }

            return content;
        }


        static NetworkCredential GetNetworkCredentials()
        {

            NetworkCredential credentials = new NetworkCredential();

            credentials.UserName = "USERNAME";
            credentials.Password = "PASSWRORD";

            return credentials;

        }

        public static bool ValidateServerCertificate(
                object sender,
                X509Certificate certificate,
                X509Chain chain,
                SslPolicyErrors sslPolicyErrors)
        {
            //if (sslPolicyErrors == SslPolicyErrors.None)
            //    return true;

            //Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            //// Do not allow this client to communicate with unauthenticated servers.
            //return false;

            //alwo any cert
            return true;
        }

    }
}
