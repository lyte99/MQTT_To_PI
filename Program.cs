using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using OSIsoft.AF.PI;
using OSIsoft.AF;
using OSIsoft.AF.Data;
using OSIsoft.AF.Asset;
using OSIsoft.AF.EventFrame;
using OSIsoft.AF.Time;

namespace mqtt_to_PI
{
    class Program
    {
        //global tag list so we dont have to check the server each time..
        public static List<string> Taglist { get; set; }

        static void Main(string[] args)
        {
            Taglist = new List<string>();
            Taglist.Add("something");

            //create client instance(s)

                //Andy's
            int AndsePort = 2222;
            string AndseHost = "";
            //string AndseHost = "192.168.1.164";
            MqttClient AndseClient = new MqttClient(AndseHost, AndsePort, false, null, null, MqttSslProtocols.None);

            //Andy's Stuff
            Andse_Connect(AndseClient);


                //Brian's
                //int MunkeePort = 2222;
                //string MunkeeHost = "";
                //MqttClient MunkeeClient = new MqttClient(MunkeeHost, MunkeePort, false, null, null, MqttSslProtocols.None);


                ////Brian's Stuff
                //Munkee_Connect(MunkeeClient);


                //MqttClient TestClient = TestClientCreation();
                //Test_Connect(TestClient);
            
        }

        private static MqttClient TestClientCreation()
        {
            //test
            int TestPort = 1883;
            string TestHost = "192.168.1.133";
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
            AndseClient.Connect(clientID, "USERNAME", "PASSWORD");


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
            MunkeeClient.Connect(clientID, "USERNAME", "PASSWORD");


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

            //CHECK FOR THE TAG THEN UPDATE IT
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


        static void UpdatePIPoint(string PIPointName, string value)
        {
            Console.WriteLine("Attemping PI Point Update for: " + PIPointName);
            bool usestring = false;

            try
            {
                //convert the value to a double
                double PIDbValue;
                    
                    if (double.TryParse(value, out PIDbValue))
                    {
                        PIDbValue = Convert.ToDouble(value);
                    }
                    else
                    {
                        usestring = true;
                    }


                //Get the default pi server
                PIServer MyPI = PIServers.GetPIServers().DefaultPIServer;

                //connect to the pi server
                MyPI.Connect();

                //set pi point
                OSIsoft.AF.PI.PIPoint UpdatePoint = OSIsoft.AF.PI.PIPoint.FindPIPoint(MyPI, PIPointName);


                //update the value
                if (!usestring)
                {
                   UpdatePoint.UpdateValue(new AFValue(PIDbValue), AFUpdateOption.Insert);
                }
                else
                {
                    UpdatePoint.UpdateValue(new AFValue(value), AFUpdateOption.Insert);
                }
                

                //disconnect from the pi server
                MyPI.Disconnect();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Saving PI Data for PI Point" + PIPointName + ".  Value: " + value.ToString() +".  Message: "  + e.Message);
            }



            Console.WriteLine("PI Value Saved");

        }

        static string CheckTag(string topic, string value)
        {
            
            //init
            string tagname = "";

            //turn the topic into a tag name
            string tagtopic = topic.Replace("/", ".");
            tagname = "MQTT" + tagtopic;




            
            //check for pi point

            //check this list first as to not hit the server each time.
            if (Taglist.Count() > 0 && !Taglist.Contains(tagname))
            {
                bool createstring = false;

                //Get the default pi server
                PIServer MyPI = PIServers.GetPIServers().DefaultPIServer;
                //connect to the pi server
                MyPI.Connect();


                try
                {
                    //it will check for the tag and throw an exception if it doesn't find it.  Not the fanciest way, but it works...
                    PIPoint CheckforPoint = PIPoint.FindPIPoint(MyPI, tagname);

                    Console.WriteLine("TagFound on server:" + tagname);

                    //if it hasn't thrown the exception add it to the list so we don't check the server each time.
                    Taglist.Add(tagname);



                }
                catch (Exception e)
                {
                    Console.WriteLine("Tag NOT Found:" + tagname);

                    //create the tag
                    try
                    {
                        //create the tag
                        MyPI.CreatePIPoint(tagname);
                        Console.WriteLine("Created tag: " + tagname);


                        PIPoint CheckforPoint = PIPoint.FindPIPoint(MyPI, tagname);

                        //string or float?
                        double PIDbValue;

                        if (double.TryParse(value, out PIDbValue))
                        {
                            PIDbValue = Convert.ToDouble(value);
                        }
                        else
                        {
                            createstring = true;
                        }

                        //set the pointsource
                        CheckforPoint.SetAttribute(PICommonPointAttributes.PointSource, "MQTT");

                        if (createstring)
                        { 
                            CheckforPoint.SetAttribute(PICommonPointAttributes.PointType, "String");
                        }
                        else
                        {
                            //default is Float32
                        }
                        
                        
                        
                        //save
                        CheckforPoint.SaveAttributes();


                    }
                    catch (Exception e1)
                    {
                        Console.WriteLine("Unable to Create tag: " + tagname);
                        Console.ReadKey();
                    }
                   

                    

                    

                    Taglist.Add(tagname);

                }

                //disconnect from the pi server
                MyPI.Disconnect();
            }
            else
            {
                Console.WriteLine("TagFound in list:" + tagname);
            }









            return tagname;
        }
    }
}
