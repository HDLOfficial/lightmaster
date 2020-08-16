﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using iMobileDevice;
using iMobileDevice.iDevice;
using iMobileDevice.Lockdown;
using LightMasterMVVM.DbAssets;
using LightMasterMVVM.Models;
using LightMasterMVVM.ViewModels;
using Newtonsoft.Json;
using Npgsql;
using ReactiveUI;
using SharpDX.Direct2D1;
using SharpDX.Text;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using Websocket.Client;

namespace LightMasterMVVM.Views
{
    public class MainWindow : Window
    {
        private MainWindowViewModel control = new MainWindowViewModel();
        private Button nav_see_matches = new Button();
        private Button nav_see_graph = new Button();
        private Button nav_see_tablets = new Button();
        private Button nav_see_tba = new Button();
        private Button nav_try_usb = new Button();
        private Button nav_see_teams = new Button();
        private Border bor_nav_see_matches = new Border();
        private Border bor_nav_see_graph = new Border();
        private Border bor_nav_see_tablets = new Border();
        private Border bor_nav_see_tba = new Border();
        private Border bor_nav_try_usb = new Border();
        private Border bor_nav_see_teams = new Border();
        private Border bluetooth_status = new Border();
        private Border usb_status = new Border();
        private Border internet_status = new Border();
        private Border database_status = new Border();
        private Button match_up = new Button();
        private Button match_down = new Button();
        private Border matches = new Border();
        private Border graph = new Border();
        private Border tablets = new Border();
        private Border tba = new Border();
        private Border teams = new Border();
        public WebsocketClient client = new WebsocketClient(new Uri("ws://localhost:8080"));
        private string previouslySelectedName = "seeTablets";
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif      

            
            nav_see_matches = this.Find<Button>("seeMatches");
            nav_see_matches.Click += NavigationChange;
            nav_see_graph = this.Find<Button>("seeGraph");
            nav_see_graph.Click += NavigationChange;
            nav_see_tablets = this.Find<Button>("seeTablets");
            nav_see_tablets.Click += NavigationChange;
            nav_see_tba = this.Find<Button>("seeTBA");
            nav_see_tba.Click += NavigationChange;
            nav_try_usb = this.Find<Button>("tryUSB");
            nav_try_usb.Click += TryUSBClick;
            nav_see_teams = this.Find<Button>("seeTeams");
            nav_see_teams.Click += NavigationChange;
            match_up = this.Find<Button>("matchUp");
            match_up.Click += MatchChange;
            match_down = this.Find<Button>("matchDown");
            match_down.Click += MatchChange;
            bluetooth_status = this.Find<Border>("bluetoothActive");
            usb_status = this.Find<Border>("usbActive");
            internet_status = this.Find<Border>("internetActive");
            database_status = this.Find<Border>("databaseActive");
            matches = this.Find<Border>("matches");
            graph = this.Find<Border>("graphs");
            tablets = this.Find<Border>("tablets");
            tba = this.Find<Border>("tba");
            teams = this.Find<Border>("teams");
            DataContext = control;
            ProcessTest();
            checkForInternet();
            
            

        }
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }
        private void MatchChange(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Button pressedButton = sender as Button;
            if(pressedButton.Name == "matchUp")
            {
                control.NextMatch();
                match_down.IsVisible = true;
            }
            else
            {
                control.PrevMatch();
                if(control.CurrentMatchNum < 2)
                {
                    match_down.IsVisible = false;
                }
            }
        }

        private async void NavigationChange(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            checkForInternet();
            Button pressedButton = sender as Button;
            if(previouslySelectedName != pressedButton.Name)
            {
                matches.Classes.Remove("show");
                graph.Classes.Remove("show");
                tablets.Classes.Remove("show");
                tba.Classes.Remove("show");
                teams.Classes.Remove("show");
                matches.Classes.Add("hide");
                graph.Classes.Add("hide");
                tablets.Classes.Add("hide");
                tba.Classes.Add("hide");
                teams.Classes.Add("hide");
                nav_see_tablets.Classes.Remove("navbuttonselected");
                nav_see_graph.Classes.Remove("navbuttonselected");
                nav_see_tba.Classes.Remove("navbuttonselected");
                nav_see_matches.Classes.Remove("navbuttonselected");
                nav_see_teams.Classes.Remove("navbuttonselected");
                nav_try_usb.Classes.Remove("navbuttonselected");
                nav_see_tablets.Classes.Add("navbutton");
                nav_see_graph.Classes.Add("navbutton");
                nav_see_tba.Classes.Add("navbutton");
                nav_see_matches.Classes.Add("navbutton");
                nav_see_teams.Classes.Add("navbutton");
                nav_try_usb.Classes.Add("navbutton");
                await Task.Delay(100);
                matches.Opacity = 0;
                graph.Opacity = 0;
                tablets.Opacity = 0;
                tba.Opacity = 0;
                teams.Opacity = 0;
                matches.IsVisible = false;
                graph.IsVisible = false;
                tablets.IsVisible = false;
                tba.IsVisible = false;
                teams.IsVisible = false;
                await Task.Delay(50);
                switch (pressedButton.Name)
                {
                    case "seeMatches":
                        matches.IsVisible = true;
                        matches.Classes.Remove("hide");
                        matches.Classes.Add("show");
                        nav_see_matches.Classes.Remove("navbutton");
                        nav_see_matches.Classes.Add("navbuttonselected");
                        await Task.Delay(100);
                        matches.Opacity = 1;
                        break;
                    case "seeGraph":
                        graph.IsVisible = true;
                        graph.Classes.Remove("hide");
                        graph.Classes.Add("show");
                        nav_see_graph.Classes.Remove("navbutton");
                        nav_see_graph.Classes.Add("navbuttonselected");
                        await Task.Delay(100);
                        graph.Opacity = 1;
                        break;
                    case "seeTablets":
                        tablets.IsVisible = true;
                        tablets.Classes.Remove("hide");
                        tablets.Classes.Add("show");
                        nav_see_tablets.Classes.Remove("navbutton");
                        nav_see_tablets.Classes.Add("navbuttonselected");
                        await Task.Delay(100);
                        tablets.Opacity = 1;
                        break;
                    case "seeTBA":
                        tba.IsVisible = true;
                        tba.Classes.Remove("hide");
                        tba.Classes.Add("show");
                        nav_see_tba.Classes.Remove("navbutton");
                        nav_see_tba.Classes.Add("navbuttonselected");
                        await Task.Delay(100);
                        tba.Opacity = 1;
                        break;
                    case "seeTeams":
                        teams.IsVisible = true;
                        teams.Classes.Remove("hide");
                        teams.Classes.Add("show");
                        nav_see_teams.Classes.Remove("navbutton");
                        nav_see_teams.Classes.Add("navbuttonselected");
                        await Task.Delay(100);
                        teams.Opacity = 1;
                        break;
                }
                previouslySelectedName = pressedButton.Name;
            }
        }
        private async void TryUSBClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            TryUSB();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            
        }
        public async void TryUSB()
        {
            try
            {
                usb_status.Classes.Add("show");
                await Task.Delay(100);
                usb_status.Opacity = 1;
                await Task.Delay(500);
                ReadOnlyCollection<string> udids;
                int count = 0;

                var idevice = LibiMobileDevice.Instance.iDevice;
                var lockdown = LibiMobileDevice.Instance.Lockdown;
                idevice.idevice_event_subscribe((ref iDeviceEvent deviceEvent, IntPtr b) =>
                {
                    checkForInternet();
                    try
                    {
                        iDeviceHandle deviceHandle;
                        idevice.idevice_new(out deviceHandle, deviceEvent.udidString);

                        LockdownClientHandle lockdownHandle;
                        lockdown.lockdownd_client_new_with_handshake(deviceHandle, out lockdownHandle, "LightScout").ThrowOnError();

                        string deviceName;
                        lockdown.lockdownd_get_device_name(lockdownHandle, out deviceName).ThrowOnError();

                        var error = idevice.idevice_connect(deviceHandle, 862, out iDeviceConnectionHandle connection);





                        ReceiveDataFromDevice(connection, idevice);
                    }
                    catch (Exception ex)
                    {

                    }


                }
                , new IntPtr(862));
            }
            catch (Exception ex)
            {
                usb_status.Classes.Add("hide");
                await Task.Delay(100);
                usb_status.Opacity = 0;
                usb_status.IsVisible = false;
                Console.WriteLine("iOS USB Fatal Failure");
            }
            try
            {
                Process startTCPforwarding = new Process();
                startTCPforwarding.StartInfo.FileName = "bash";
                string command = "adb reverse tcp:6000 tcp:6000";
                startTCPforwarding.StartInfo.Arguments = "-c \"" + command + "\"";
                startTCPforwarding.StartInfo.UseShellExecute = false;
                startTCPforwarding.StartInfo.RedirectStandardOutput = true;
                startTCPforwarding.StartInfo.CreateNoWindow = true;
                startTCPforwarding.ErrorDataReceived += (s, ea) =>
                {
                    Console.WriteLine("There is no connected USB device that is valid!");
                };
                startTCPforwarding.Start();
                var theoutput = startTCPforwarding.StandardOutput.ReadToEnd().ToString();

                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(new IPEndPoint(IPAddress.Loopback, 6000));
                byte[] testreceive = new byte[5000];
            }
            catch (Exception ex)
            {
                Console.WriteLine("Android USB Fatal Failure");
                usb_status.Classes.Add("hide");
                await Task.Delay(100);
                usb_status.Opacity = 0;
                usb_status.IsVisible = false;
            }


            /*string[] ports = SerialPort.GetPortNames();
            
            var _SerialPort = new SerialPort();
            
            Console.WriteLine("The following serial ports were found:");
            // Display each port name to the console.             
            foreach (string port in ports)
            {
                Console.WriteLine(port);
                if (port.StartsWith("/dev/tty.usbmodem"))
                {
                    _SerialPort.PortName = port;
                    _SerialPort.BaudRate = 115200;
                    _SerialPort.Parity = Parity.None;
                    _SerialPort.DataBits = 8;
                    _SerialPort.StopBits = StopBits.One;
                    _SerialPort.Handshake = Handshake.None;

                    _SerialPort.Open();

                    _SerialPort.DataReceived += (s, a) =>
                    {
                        Console.WriteLine(((SerialPort)s).ReadExisting());
            
                    };
                    _SerialPort.Write(Encoding.ASCII.GetBytes("HI"), 0, Encoding.ASCII.GetBytes("HI").Length);
                }
            }*/

            /*Process usbTest = new Process();
            usbTest.StartInfo.FileName = "bash";
            string command = "adb devices";
            usbTest.StartInfo.Arguments = "-c \"" + command + "\"";
            usbTest.StartInfo.UseShellExecute = false;
            usbTest.StartInfo.RedirectStandardOutput = true;
            usbTest.StartInfo.CreateNoWindow = true;
            usbTest.Start();
            var theoutput = usbTest.StandardOutput.ReadToEnd().ToString();

            foreach(var devicefound in theoutput.Split("\n").Skip(1))
            {
                if(devicefound.Length > 15)
                {
                    Console.WriteLine(devicefound.Substring(0, 16));
                }
                
            }*/


            /*socket.BeginReceive(testreceive, 0, testreceive.Length, SocketFlags.None, (ar) =>
            {
                testreceive = new byte[5000];
                int numbytessent = 0;
                foreach(var singlebyte in testreceive)
                {
                    if(singlebyte != 0)
                    {
                        numbytessent++;
                    }
                }
                var convertedstring = Encoding.ASCII.GetString(testreceive, 0, numbytessent);
                Console.WriteLine(convertedstring);

            }, socket);*/
            //MAKE A TASK.RUN SYSTEM


        }
        private void ReceiveDataFromDevice(iDeviceConnectionHandle connection, IiDeviceApi deviceApi)
        {
            byte[] inbytes = new byte[90000];
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        inbytes = new byte[90000];
                        uint receivedBytes = 0;
                        deviceApi.idevice_connection_receive(connection, inbytes, (uint)inbytes.Length, ref receivedBytes);
                        if (receivedBytes <= 0) continue;
                        byte[] finalInBytes = inbytes.Take((int)receivedBytes).ToArray();
                        string rawdata = Encoding.ASCII.GetString(finalInBytes);
                        string[] datapackets = rawdata.Split("~");
                        foreach (var packet in datapackets)
                        {
                            Console.WriteLine(rawdata);
                            UseGivenData(packet, false);
                        }

                    }
                    catch (Exception ex)
                    {
                        connection.Close();
                        connection.Dispose();
                        break;
                    }


                    //uint bytesSent = 0;
                    //deviceApi.idevice_connection_send(connection, Encoding.ASCII.GetBytes("Hijjjj"), (uint)Encoding.ASCII.GetBytes("Hijjjj").Length, ref bytesSent);
                }
            });
        }
        public async void ProcessTest()
        {

            Process startTCPforwarding = new Process();
            startTCPforwarding.StartInfo.WorkingDirectory = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "lightmaster"), "LightMasterHub");
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                startTCPforwarding.StartInfo.FileName = "cmd.exe";
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                startTCPforwarding.StartInfo.FileName = "bash";
            }

            string command = "npm start";
            startTCPforwarding.StartInfo.Arguments = "-c \"" + command + "\"";
            startTCPforwarding.StartInfo.UseShellExecute = false;
            startTCPforwarding.StartInfo.RedirectStandardOutput = true;
            startTCPforwarding.StartInfo.CreateNoWindow = true;
            try
            {
                startTCPforwarding.Start();
                startTCPforwarding.ErrorDataReceived += async (s, e) =>
                {
                    bluetooth_status.Classes.Add("hide");
                    await Task.Delay(100);
                    bluetooth_status.Opacity = 0;
                    bluetooth_status.IsVisible = false;
                    control.NotificationViewModel.AddNotification("Unavailable", "Bluetooth Service Not Available!", "Red");
                };
                var exitEvent = new ManualResetEvent(false);
                var url = new Uri("ws://localhost:9959");
                client.Url = new Uri("ws://localhost:9959");
                client.ReconnectTimeout = null;
                client.ErrorReconnectTimeout = TimeSpan.FromSeconds(5);
                client.Start();
                /*client.ReconnectionHappened.Subscribe(info =>
                    Log.Information($"Reconnection happened, type: {info.Type}"));*/
                int nummessagesreceived = 0;
                
                client.MessageReceived.Subscribe(msg =>
                {
                    //checkForInternet();
                    if (nummessagesreceived == 0)
                    {
                        Dispatcher.UIThread.Post(async () => {
                            bluetooth_status.Classes.Add("show");
                            await Task.Delay(100);
                            bluetooth_status.Opacity = 1;
                        });
                        control.NotificationViewModel.AddNotification("Ready", "Bluetooth Service Ready!", "DeepPink");
                    }
                    else
                    {
                        string rawdata = msg.Text;
                        UseGivenData(rawdata, true);
                    }


                    nummessagesreceived++;
                });
                client.DisconnectionHappened.Subscribe(async msg =>
                {
                    Console.WriteLine("Uh oh! I disconnected!");
                    nummessagesreceived = 0;
                    Dispatcher.UIThread.Post(async () => {
                        bluetooth_status.Classes.Add("hide");
                        await Task.Delay(100);
                        bluetooth_status.Opacity = 0;
                    });
                    control.NotificationViewModel.AddNotification("Not Ready", "Bluetooth Service Not Ready!", "Red");
                });
            }
            catch (Exception ex)
            {
                bluetooth_status.Classes.Add("hide");
                await Task.Delay(100);
                bluetooth_status.Opacity = 0;
                bluetooth_status.IsVisible = false;
                control.NotificationViewModel.AddNotification("Unavailable", "Bluetooth Service Not Available!", "Red");
            }
            try
            {
                database_status.Classes.Add("show");
                await Task.Delay(100);
                database_status.Opacity = 1;
                await Task.Delay(500);
                using (var db = new ScoutingContext())
                {
                    var listoftest = db.Matches.ToList();
                }
            }
            catch (Exception ex)
            {
                database_status.Classes.Add("hide");
                await Task.Delay(100);
                database_status.Opacity = 0;
            }

        }
        public async void checkForInternet()
        {
            if (CheckForInternetConnection())
            {
                if (internet_status.Opacity != 1)
                {
                    internet_status.Classes.Add("show");
                    await Task.Delay(100);
                    internet_status.Opacity = 1;
                }
            }
            else
            {
                if (internet_status.Opacity != 0)
                {
                    internet_status.Classes.Add("hide");
                    await Task.Delay(100);
                    internet_status.Opacity = 1;
                }
            }
        }
        public void UseGivenData(string rawdata, bool bluetooth)
        {
            int tabletindex = 0;
            if (rawdata.StartsWith("R1"))
            {
                tabletindex = 3;
            }
            else if (rawdata.StartsWith("R2"))
            {
                tabletindex = 4;
            }
            else if (rawdata.StartsWith("R3"))
            {
                tabletindex = 5;
            }
            else if (rawdata.StartsWith("B1"))
            {
                tabletindex = 0;
            }
            else if (rawdata.StartsWith("B2"))
            {
                tabletindex = 1;
            }
            else if (rawdata.StartsWith("B3"))
            {
                tabletindex = 2;
            }
            control.TabletViewModel.LastPings[tabletindex] = "Last ping at: " + DateTime.Now.ToShortTimeString();
            if (rawdata.Substring(3).StartsWith("S:"))
            {
                string datawithoutid = rawdata;
                int deviceid = 1000;
                if (bluetooth)
                {
                    datawithoutid = rawdata.Substring(0, 5) + rawdata.Substring(10);
                    deviceid = int.Parse(rawdata.Substring(5, 4));
                }
                //S = Score
                control.NotificationViewModel.AddNotification("Scored", datawithoutid.Substring(0, 2).ToString() + " sent scores", "Green");
                control.TabletViewModel.BluetoothBackgroundColors[tabletindex] = "LightBlue";
                control.TabletViewModel.BluetoothBorderColors[tabletindex] = "Blue";
                var jsontodeserialize = datawithoutid.Substring(5);
                using (var db = new ScoutingContext())
                {
                    var itemstouse = JsonConvert.DeserializeObject<List<IO_TeamMatch>>(jsontodeserialize);
                    foreach (var itemtouse in itemstouse)
                    {
                        try
                        {
                            var previousitem = db.Matches.Where(x => x.TabletId == itemtouse.TabletId && x.MatchNumber == itemtouse.MatchNumber && x.EventCode == itemtouse.EventCode).FirstOrDefault();
                            if (previousitem == null)
                            {
                                TeamMatch newTeamMatch = new TeamMatch() { AlliancePartners = itemtouse.AlliancePartners, A_InitiationLine = itemtouse.A_InitiationLine, ClientLastSubmitted = itemtouse.ClientLastSubmitted, ClientSubmitted = true, DisabledSeconds = itemtouse.DisabledSeconds, IsQualifying = itemtouse.IsQualifying, EventCode = itemtouse.EventCode, E_Balanced = itemtouse.E_Balanced, E_ClimbAttempt = itemtouse.E_ClimbAttempt, E_ClimbSuccess = itemtouse.E_ClimbSuccess, E_Park = itemtouse.E_Park, MatchID = new Random().Next(1, 1000000), MatchNumber = itemtouse.MatchNumber, NumCycles = itemtouse.NumCycles, PowerCellInner = itemtouse.PowerCellInner, PowerCellLower = itemtouse.PowerCellLower, PowerCellMissed = itemtouse.PowerCellMissed, PowerCellOuter = itemtouse.PowerCellOuter, RobotPosition = itemtouse.RobotPosition, ScoutName = itemtouse.ScoutName, TabletId = itemtouse.TabletId, TapLogs = itemtouse.TapLogs, TeamName = itemtouse.TeamName, T_ControlPanelPosition = itemtouse.T_ControlPanelPosition, T_ControlPanelRotation = itemtouse.T_ControlPanelRotation };
                                try
                                {
                                    if (db.FRCTeams.Find(itemtouse.TeamNumber, itemtouse.EventCode) != null)
                                    {
                                        newTeamMatch.TrackedTeam = db.FRCTeams.Find(itemtouse.TeamNumber, itemtouse.EventCode);
                                    }
                                    else
                                    {
                                        FRCTeamModel newTeamInDb = new FRCTeamModel() { event_key = itemtouse.EventCode, team_number = itemtouse.TeamNumber };
                                        db.FRCTeams.Add(newTeamInDb);
                                        db.SaveChanges();
                                        newTeamMatch.TrackedTeam = newTeamInDb;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    FRCTeamModel newTeamInDb = new FRCTeamModel() { event_key = itemtouse.EventCode, team_number = itemtouse.TeamNumber };
                                    db.FRCTeams.Add(newTeamInDb);
                                    db.SaveChanges();
                                    newTeamMatch.TrackedTeam = newTeamInDb;
                                }

                                itemtouse.MatchID = new Random().Next(1, 100000);
                                db.Matches.Add(newTeamMatch);
                            }
                            else
                            {
                                if (previousitem.ClientLastSubmitted != itemtouse.ClientLastSubmitted)
                                {
                                    TeamMatch newTeamMatch = new TeamMatch() { AlliancePartners = itemtouse.AlliancePartners, A_InitiationLine = itemtouse.A_InitiationLine, ClientLastSubmitted = itemtouse.ClientLastSubmitted, ClientSubmitted = true, DisabledSeconds = itemtouse.DisabledSeconds, IsQualifying = itemtouse.IsQualifying, EventCode = previousitem.EventCode, E_Balanced = itemtouse.E_Balanced, E_ClimbAttempt = itemtouse.E_ClimbAttempt, E_ClimbSuccess = itemtouse.E_ClimbSuccess, E_Park = itemtouse.E_Park, MatchID = previousitem.MatchID, MatchNumber = previousitem.MatchNumber, NumCycles = itemtouse.NumCycles, PowerCellInner = itemtouse.PowerCellInner, PowerCellLower = itemtouse.PowerCellLower, PowerCellMissed = itemtouse.PowerCellMissed, PowerCellOuter = itemtouse.PowerCellOuter, RobotPosition = itemtouse.RobotPosition, ScoutName = itemtouse.ScoutName, TabletId = itemtouse.TabletId, TapLogs = itemtouse.TapLogs, TeamName = itemtouse.TeamName, T_ControlPanelPosition = itemtouse.T_ControlPanelPosition, T_ControlPanelRotation = itemtouse.T_ControlPanelRotation };
                                    try
                                    {
                                        newTeamMatch.TrackedTeam = previousitem.TrackedTeam;
                                    }
                                    catch (Exception ex)
                                    {
                                        FRCTeamModel newTeamInDb = new FRCTeamModel() { event_key = itemtouse.EventCode, team_number = itemtouse.TeamNumber };
                                        db.FRCTeams.Add(newTeamInDb);
                                        db.SaveChanges();
                                        newTeamMatch.TrackedTeam = newTeamInDb;
                                    }

                                    db.Matches.Update(newTeamMatch);
                                    db.SaveChanges();
                                }

                            }


                        }
                        catch (NpgsqlException ex)
                        {
                            TeamMatch newTeamMatch = new TeamMatch() { AlliancePartners = itemtouse.AlliancePartners, A_InitiationLine = itemtouse.A_InitiationLine, ClientLastSubmitted = itemtouse.ClientLastSubmitted, ClientSubmitted = true, DisabledSeconds = itemtouse.DisabledSeconds, IsQualifying = itemtouse.IsQualifying, EventCode = itemtouse.EventCode, E_Balanced = itemtouse.E_Balanced, E_ClimbAttempt = itemtouse.E_ClimbAttempt, E_ClimbSuccess = itemtouse.E_ClimbSuccess, E_Park = itemtouse.E_Park, MatchID = new Random().Next(1, 1000000), MatchNumber = itemtouse.MatchNumber, NumCycles = itemtouse.NumCycles, PowerCellInner = itemtouse.PowerCellInner, PowerCellLower = itemtouse.PowerCellLower, PowerCellMissed = itemtouse.PowerCellMissed, PowerCellOuter = itemtouse.PowerCellOuter, RobotPosition = itemtouse.RobotPosition, ScoutName = itemtouse.ScoutName, TabletId = itemtouse.TabletId, TapLogs = itemtouse.TapLogs, TeamName = itemtouse.TeamName, T_ControlPanelPosition = itemtouse.T_ControlPanelPosition, T_ControlPanelRotation = itemtouse.T_ControlPanelRotation };
                            try
                            {
                                if (db.FRCTeams.Find(itemtouse.TeamNumber, itemtouse.EventCode) != null)
                                {
                                    newTeamMatch.TrackedTeam = db.FRCTeams.Find(itemtouse.TeamNumber, itemtouse.EventCode);
                                }
                                else
                                {
                                    FRCTeamModel newTeamInDb = new FRCTeamModel() { event_key = itemtouse.EventCode, team_number = itemtouse.TeamNumber };
                                    db.FRCTeams.Add(newTeamInDb);
                                    db.SaveChanges();
                                    newTeamMatch.TrackedTeam = newTeamInDb;
                                }
                            }
                            catch (Exception smex)
                            {
                                FRCTeamModel newTeamInDb = new FRCTeamModel() { event_key = itemtouse.EventCode, team_number = itemtouse.TeamNumber };
                                db.FRCTeams.Add(newTeamInDb);
                                db.SaveChanges();
                                newTeamMatch.TrackedTeam = newTeamInDb;
                            }

                            itemtouse.MatchID = new Random().Next(1, 100000);
                            db.Matches.Add(newTeamMatch);
                        }
                    }


                    db.SaveChanges();
                }
            }
            else if (rawdata.Substring(3).StartsWith("B:"))
            {
                //B = Battery Level
                string datawithoutid = rawdata;
                int deviceid = 1000;
                if (bluetooth)
                {
                    datawithoutid = rawdata.Substring(0, 5) + rawdata.Substring(10);
                    deviceid = int.Parse(rawdata.Substring(5, 4));
                }

                var batterylevel = float.Parse(datawithoutid.Substring(5)) * 100;
                if (batterylevel > 80)
                {
                    control.TabletViewModel.BatteryBackgroundColors[tabletindex] = "LightGreen";
                    control.TabletViewModel.BatteryBorderColors[tabletindex] = "Green";
                }
                else if (batterylevel > 30 && batterylevel <= 80)
                {
                    control.TabletViewModel.BatteryBackgroundColors[tabletindex] = "LightSalmon";
                    control.TabletViewModel.BatteryBorderColors[tabletindex] = "DarkOrange";
                }
                else
                {
                    control.TabletViewModel.BatteryBackgroundColors[tabletindex] = "LightPink";
                    control.TabletViewModel.BatteryBorderColors[tabletindex] = "IndianRed";
                }
                control.TabletViewModel.BatteryAmounts[tabletindex] = Math.Round(batterylevel).ToString();

            }
            else if (rawdata.Substring(3).StartsWith("D:"))
            {
                //D = Successful Disconnection
                control.TabletViewModel.BluetoothBackgroundColors[tabletindex] = "LightGray";
                control.TabletViewModel.BluetoothBorderColors[tabletindex] = "Gray";
            }
            else if (rawdata.Substring(3).StartsWith("E:"))
            {
                //E = Immedient Communication
                control.TabletViewModel.BluetoothBackgroundColors[tabletindex] = "LightSalmon";
                control.TabletViewModel.BluetoothBorderColors[tabletindex] = "DarkOrange";
            }
            else if (rawdata.Substring(3).StartsWith("RD:"))
            {
                string datawithoutid = rawdata;
                int deviceid = 1000;
                if (bluetooth)
                {
                    datawithoutid = rawdata.Substring(0, 6) + rawdata.Substring(11);
                    deviceid = int.Parse(rawdata.Substring(6, 4));
                }

                control.NotificationViewModel.AddNotification("Data Request", rawdata.Substring(0, 2).ToString() + " requested data", "Blue");
                var listOfMatchesToSend = new List<IO_TeamMatch>()
                        {
                            new IO_TeamMatch() { TeamNumber = 862, MatchNumber = 1, TabletId = rawdata.Substring(0,2).ToString(), PowerCellInner = new int[21], PowerCellOuter = new int[21], PowerCellLower = new int[21], PowerCellMissed = new int[21], EventCode = "test_env", AlliancePartners = new int[2]{ 5567,4456 } },
                            new IO_TeamMatch() { TeamNumber = 1023, MatchNumber = 2, TabletId = rawdata.Substring(0,2).ToString(), PowerCellInner = new int[21], PowerCellOuter = new int[21], PowerCellLower = new int[21], PowerCellMissed = new int[21], EventCode = "test_env", AlliancePartners = new int[2]{ 5567,4456 } },
                            new IO_TeamMatch() { TeamNumber = 2014, MatchNumber = 3, TabletId = rawdata.Substring(0,2).ToString(), PowerCellInner = new int[21], PowerCellOuter = new int[21], PowerCellLower = new int[21], PowerCellMissed = new int[21], EventCode = "test_env", AlliancePartners = new int[2]{ 5567,4456 } },
                            new IO_TeamMatch() { TeamNumber = 2020, MatchNumber = 4, TabletId = rawdata.Substring(0,2).ToString(), PowerCellInner = new int[21], PowerCellOuter = new int[21], PowerCellLower = new int[21], PowerCellMissed = new int[21], EventCode = "test_env", AlliancePartners = new int[2]{ 5567,4456 } },
                            new IO_TeamMatch() { TeamNumber = 3145, MatchNumber = 5, TabletId = rawdata.Substring(0,2).ToString(), PowerCellInner = new int[21], PowerCellOuter = new int[21], PowerCellLower = new int[21], PowerCellMissed = new int[21], EventCode = "test_env", AlliancePartners = new int[2]{ 5567,4456 } },
                            new IO_TeamMatch() { TeamNumber = 4005, MatchNumber = 6, TabletId = rawdata.Substring(0,2).ToString(), PowerCellInner = new int[21], PowerCellOuter = new int[21], PowerCellLower = new int[21], PowerCellMissed = new int[21], EventCode = "test_env", AlliancePartners = new int[2]{ 5567,4456 } }
                        };
                string modelstring = JsonConvert.SerializeObject(listOfMatchesToSend);
                byte[] bytesToSend = Encoding.ASCII.GetBytes(modelstring);
                if (modelstring.Length > 480)
                {
                    int numberofmessages = (int)Math.Ceiling((float)modelstring.Length / (float)480);
                    var startidentifier = rawdata.Substring(0, 2).ToString() + ":MM:" + deviceid.ToString() + ":" + numberofmessages.ToString();
                    client.Send(startidentifier);
                    for (int i = numberofmessages; i > 0; i--)
                    {
                        var simplestring = rawdata.Substring(0, 2).ToString() + ":" + deviceid.ToString() + ":" + new string(modelstring.Skip((numberofmessages - i) * 480).Take(480).ToArray());
                        client.Send(simplestring);
                    }
                }
                else
                {
                    client.Send(deviceid.ToString() + ":" + modelstring);
                }
                control.TabletViewModel.BluetoothBackgroundColors[tabletindex] = "LightSalmon";
                control.TabletViewModel.BluetoothBorderColors[tabletindex] = "DarkOrange";
            }


            //Console.WriteLine(msg.Text);
        }
    }
}
