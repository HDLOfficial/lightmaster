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
using LightMasterMVVM.Scripts;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Reflection;
using System.Data.Common;

namespace LightMasterMVVM.Views
{
    public class MainWindow : Window
    {
        private MainWindowViewModel control = new MainWindowViewModel();
        public SwitchConfiguration configuration => new ConfigurationData().LoadData();
        private Button nav_see_matches = new Button();
        private Button nav_see_graph = new Button();
        private Button nav_see_tablets = new Button();
        private Button nav_see_tba = new Button();
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
        private Border actualgraphs = new Border();
        private Border tablets = new Border();
        private Border tba = new Border();
        private Border teams = new Border();
        private Border teamDetails = new Border();
        private Border matchDetails = new Border();
        public WebsocketClient client = new WebsocketClient(new Uri("ws://localhost:8080"));
        private string previouslySelectedName = "seeTablets";
        public iDeviceConnectionHandle connection;
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
            actualgraphs = this.Find<Border>("actualgraphs");
            tablets = this.Find<Border>("tablets");
            tba = this.Find<Border>("tba");
            teams = this.Find<Border>("teams");
            teamDetails = this.Find<Border>("teamDetails");
            matchDetails = this.Find<Border>("matchDetails");
            DataContext = control;
            ProcessTest();
            checkForInternet();
            TryUSB();
            if(ApplicationState.DBUsernameUsed == null || ApplicationState.DBUsernameUsed == "")
            {
                var loginwindow = new SignIn();
                loginwindow.Show();
            }
            NavMessenger.CreateNewGraph += async (TrackedProperty[] trackingProperties, TrackedProperty[] orderingProperties) =>
            {
                previouslySelectedName = null;
                control.GraphViewModel = new GraphViewModel(trackingProperties, orderingProperties);
                matches.Classes.Remove("show");
                graph.Classes.Remove("show");
                tablets.Classes.Remove("show");
                tba.Classes.Remove("show");
                teams.Classes.Remove("show");
                teamDetails.Classes.Remove("show");
                matchDetails.Classes.Remove("show");
                matches.Classes.Add("hide");
                graph.Classes.Add("hide");
                tablets.Classes.Add("hide");
                tba.Classes.Add("hide");
                teams.Classes.Add("hide");
                actualgraphs.Classes.Add("hide");
                teamDetails.Classes.Add("hide");
                matchDetails.Classes.Add("hide");
                nav_see_tablets.Classes.Remove("navbuttonselected");
                nav_see_graph.Classes.Remove("navbuttonselected");
                nav_see_tba.Classes.Remove("navbuttonselected");
                nav_see_matches.Classes.Remove("navbuttonselected");
                nav_see_teams.Classes.Remove("navbuttonselected");
                nav_see_tablets.Classes.Add("navbutton");
                nav_see_graph.Classes.Add("navbutton");
                nav_see_tba.Classes.Add("navbutton");
                nav_see_matches.Classes.Add("navbutton");
                nav_see_teams.Classes.Add("navbutton");
                await Task.Delay(100);
                matches.Opacity = 0;
                graph.Opacity = 0;
                tablets.Opacity = 0;
                tba.Opacity = 0;
                teams.Opacity = 0;
                teamDetails.Opacity = 0;
                matchDetails.Opacity = 0;
                matches.IsVisible = false;
                graph.IsVisible = false;
                tablets.IsVisible = false;
                actualgraphs.IsVisible = false;
                tba.IsVisible = false;
                teams.IsVisible = false;
                teamDetails.IsVisible = false;
                matchDetails.IsVisible = false;
                await Task.Delay(50);
                actualgraphs.IsVisible = true;
                actualgraphs.Classes.Remove("hide");
                actualgraphs.Classes.Add("show");
                await Task.Delay(100);
                actualgraphs.Opacity = 1;
            };
            NavMessenger.ShowMatchDetails += async (int matchnum, OriginalPage calledPage) =>
            {
                previouslySelectedName = null;
                control.MatchDetailsViewModel = new MatchDetailsViewModel(matchnum,calledPage);
                matches.Classes.Remove("show");
                graph.Classes.Remove("show");
                tablets.Classes.Remove("show");
                tba.Classes.Remove("show");
                teams.Classes.Remove("show");
                actualgraphs.Classes.Remove("show");
                teamDetails.Classes.Remove("show");
                matchDetails.Classes.Remove("show");
                matches.Classes.Add("hide");
                graph.Classes.Add("hide");
                tablets.Classes.Add("hide");
                tba.Classes.Add("hide");
                teams.Classes.Add("hide");
                teamDetails.Classes.Add("hide");
                matchDetails.Classes.Add("hide");
                nav_see_tablets.Classes.Remove("navbuttonselected");
                nav_see_graph.Classes.Remove("navbuttonselected");
                nav_see_tba.Classes.Remove("navbuttonselected");
                nav_see_matches.Classes.Remove("navbuttonselected");
                nav_see_teams.Classes.Remove("navbuttonselected");
                nav_see_tablets.Classes.Add("navbutton");
                nav_see_graph.Classes.Add("navbutton");
                nav_see_tba.Classes.Add("navbutton");
                nav_see_matches.Classes.Add("navbutton");
                nav_see_teams.Classes.Add("navbutton");
                await Task.Delay(100);
                matches.Opacity = 0;
                graph.Opacity = 0;
                tablets.Opacity = 0;
                tba.Opacity = 0;
                teams.Opacity = 0;
                teamDetails.Opacity = 0;
                matchDetails.Opacity = 0;
                matches.IsVisible = false;
                graph.IsVisible = false;
                tablets.IsVisible = false;
                tba.IsVisible = false;
                actualgraphs.IsVisible = false;
                teams.IsVisible = false;
                teamDetails.IsVisible = false;
                matchDetails.IsVisible = false;
                await Task.Delay(50);
                matchDetails.IsVisible = true;
                matchDetails.Classes.Remove("hide");
                matchDetails.Classes.Add("show");
                await Task.Delay(100);
                matchDetails.Opacity = 1;
            };
            NavMessenger.ShowTeamDetails += async (int team_instance_id, OriginalPage calledPage) =>
            {
                previouslySelectedName = null;
                control.TeamDetailsViewModel = new TeamDetailsViewModel(team_instance_id, calledPage);
                matches.Classes.Remove("show");
                graph.Classes.Remove("show");
                tablets.Classes.Remove("show");
                tba.Classes.Remove("show");
                teams.Classes.Remove("show");
                teamDetails.Classes.Remove("show");
                matchDetails.Classes.Remove("show");
                matches.Classes.Add("hide");
                graph.Classes.Add("hide");
                tablets.Classes.Add("hide");
                tba.Classes.Add("hide");
                teams.Classes.Add("hide");
                actualgraphs.Classes.Add("hide");
                teamDetails.Classes.Add("hide");
                matchDetails.Classes.Add("hide");
                nav_see_tablets.Classes.Remove("navbuttonselected");
                nav_see_graph.Classes.Remove("navbuttonselected");
                nav_see_tba.Classes.Remove("navbuttonselected");
                nav_see_matches.Classes.Remove("navbuttonselected");
                nav_see_teams.Classes.Remove("navbuttonselected");
                nav_see_tablets.Classes.Add("navbutton");
                nav_see_graph.Classes.Add("navbutton");
                nav_see_tba.Classes.Add("navbutton");
                nav_see_matches.Classes.Add("navbutton");
                nav_see_teams.Classes.Add("navbutton");
                await Task.Delay(100);
                matches.Opacity = 0;
                graph.Opacity = 0;
                tablets.Opacity = 0;
                tba.Opacity = 0;
                teams.Opacity = 0;
                teamDetails.Opacity = 0;
                matchDetails.Opacity = 0;
                matches.IsVisible = false;
                graph.IsVisible = false;
                tablets.IsVisible = false;
                actualgraphs.IsVisible = false;
                tba.IsVisible = false;
                teams.IsVisible = false;
                teamDetails.IsVisible = false;
                matchDetails.IsVisible = false;
                await Task.Delay(50);
                teamDetails.IsVisible = true;
                teamDetails.Classes.Remove("hide");
                teamDetails.Classes.Add("show");
                await Task.Delay(100);
                teamDetails.Opacity = 1;
            };
            NavMessenger.FromTeamDetails += async (OriginalPage page) =>
            {
                matches.Classes.Remove("show");
                graph.Classes.Remove("show");
                tablets.Classes.Remove("show");
                tba.Classes.Remove("show");
                teams.Classes.Remove("show");
                teamDetails.Classes.Remove("show");
                matchDetails.Classes.Remove("show");
                matches.Classes.Add("hide");
                graph.Classes.Add("hide");
                tablets.Classes.Add("hide");
                tba.Classes.Add("hide");
                teams.Classes.Add("hide");
                actualgraphs.Classes.Add("hide");
                teamDetails.Classes.Add("hide");
                matchDetails.Classes.Add("hide");
                nav_see_tablets.Classes.Remove("navbuttonselected");
                nav_see_graph.Classes.Remove("navbuttonselected");
                nav_see_tba.Classes.Remove("navbuttonselected");
                nav_see_matches.Classes.Remove("navbuttonselected");
                nav_see_teams.Classes.Remove("navbuttonselected");
                nav_see_tablets.Classes.Add("navbutton");
                nav_see_graph.Classes.Add("navbutton");
                nav_see_tba.Classes.Add("navbutton");
                nav_see_matches.Classes.Add("navbutton");
                nav_see_teams.Classes.Add("navbutton");
                await Task.Delay(100);
                matches.Opacity = 0;
                graph.Opacity = 0;
                tablets.Opacity = 0;
                tba.Opacity = 0;
                teams.Opacity = 0;
                teamDetails.Opacity = 0;
                matchDetails.Opacity = 0;
                matches.IsVisible = false;
                graph.IsVisible = false;
                tablets.IsVisible = false;
                tba.IsVisible = false;
                teams.IsVisible = false;
                actualgraphs.IsVisible = false;
                teamDetails.IsVisible = false;
                matchDetails.IsVisible = false;
                await Task.Delay(50);
                switch (page)
                {
                    case OriginalPage.StatsView:
                        matches.IsVisible = true;
                        matches.Classes.Remove("hide");
                        matches.Classes.Add("show");
                        nav_see_matches.Classes.Remove("navbutton");
                        nav_see_matches.Classes.Add("navbuttonselected");
                        await Task.Delay(100);
                        matches.Opacity = 1;
                        previouslySelectedName = matches.Name;
                        break;
                    case OriginalPage.TeamList:
                        teams.IsVisible = true;
                        teams.Classes.Remove("hide");
                        teams.Classes.Add("show");
                        nav_see_teams.Classes.Remove("navbutton");
                        nav_see_teams.Classes.Add("navbuttonselected");
                        await Task.Delay(100);
                        teams.Opacity = 1;
                        previouslySelectedName = teams.Name;
                        break;
                }
            };
            NavMessenger.FromMatchDetails += async (OriginalPage page) =>
            {
                
                matches.Classes.Remove("show");
                graph.Classes.Remove("show");
                tablets.Classes.Remove("show");
                tba.Classes.Remove("show");
                teams.Classes.Remove("show");
                teamDetails.Classes.Remove("show");
                matchDetails.Classes.Remove("show");
                matches.Classes.Add("hide");
                graph.Classes.Add("hide");
                tablets.Classes.Add("hide");
                tba.Classes.Add("hide");
                teams.Classes.Add("hide");
                actualgraphs.Classes.Add("hide");
                teamDetails.Classes.Add("hide");
                matchDetails.Classes.Add("hide");
                nav_see_tablets.Classes.Remove("navbuttonselected");
                nav_see_graph.Classes.Remove("navbuttonselected");
                nav_see_tba.Classes.Remove("navbuttonselected");
                nav_see_matches.Classes.Remove("navbuttonselected");
                nav_see_teams.Classes.Remove("navbuttonselected");
                nav_see_tablets.Classes.Add("navbutton");
                nav_see_graph.Classes.Add("navbutton");
                nav_see_tba.Classes.Add("navbutton");
                nav_see_matches.Classes.Add("navbutton");
                nav_see_teams.Classes.Add("navbutton");
                await Task.Delay(100);
                matches.Opacity = 0;
                graph.Opacity = 0;
                tablets.Opacity = 0;
                tba.Opacity = 0;
                teams.Opacity = 0;
                teamDetails.Opacity = 0;
                matchDetails.Opacity = 0;
                matches.IsVisible = false;
                graph.IsVisible = false;
                tablets.IsVisible = false;
                tba.IsVisible = false;
                teams.IsVisible = false;
                actualgraphs.IsVisible = false;
                teamDetails.IsVisible = false;
                matchDetails.IsVisible = false;
                await Task.Delay(50);
                switch (page)
                {
                    case OriginalPage.StatsView:
                        matches.IsVisible = true;
                        matches.Classes.Remove("hide");
                        matches.Classes.Add("show");
                        nav_see_matches.Classes.Remove("navbutton");
                        nav_see_matches.Classes.Add("navbuttonselected");
                        await Task.Delay(100);
                        matches.Opacity = 1;
                        previouslySelectedName = matches.Name;
                        break;
                    case OriginalPage.TeamList:
                        teams.IsVisible = true;
                        teams.Classes.Remove("hide");
                        teams.Classes.Add("show");
                        nav_see_teams.Classes.Remove("navbutton");
                        nav_see_teams.Classes.Add("navbuttonselected");
                        await Task.Delay(100);
                        teams.Opacity = 1;
                        previouslySelectedName = teams.Name;
                        break;
                }
            };
            NavMessenger.NotificationEventAcceptance += async (args) =>
            {
                control.NotificationViewModel.UserRequests.Remove(args);
                control.NotificationViewModel.Notifications.Remove(control.NotificationViewModel.Notifications.Where(x => x.NotificationId == args.NotificationId).FirstOrDefault());
                switch (args.TypeOfRequest)
                {

                }
            };
            NavMessenger.ShowWindow += async (window, isdialogue) =>
            {
                if (isdialogue)
                {
                    window.ShowDialog(this);
                }
                else
                {
                    window.Show();
                }
                
            };
            /*NavMessenger.NotificationSeconds += (seconds, notificationId) =>
            {
                Console.WriteLine("Text");
                var secondselapsed = 0;
                DispatcherTimer timer = new DispatcherTimer();
                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Tick += (s,e) =>
                {
                    
                    control.NotificationViewModel.Notifications.ToArray()[control.NotificationViewModel.Notifications.IndexOf(control.NotificationViewModel.Notifications.Where(x => x.NotificationId == notificationId).FirstOrDefault())].SecondsLeft = seconds - secondselapsed;
                    if(secondselapsed >= seconds)
                    {
                        timer.Stop();
                    }
                    secondselapsed++;
                    
                };
                
                // Start the timer.  Note that this call can be made from any thread.
                timer.Start();
            };*/

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
                teamDetails.Classes.Remove("show");
                matchDetails.Classes.Remove("show");
                matches.Classes.Add("hide");
                graph.Classes.Add("hide");
                actualgraphs.Classes.Add("hide");
                tablets.Classes.Add("hide");
                tba.Classes.Add("hide");
                teams.Classes.Add("hide");
                teamDetails.Classes.Add("hide");
                matchDetails.Classes.Add("hide");
                nav_see_tablets.Classes.Remove("navbuttonselected");
                nav_see_graph.Classes.Remove("navbuttonselected");
                nav_see_tba.Classes.Remove("navbuttonselected");
                nav_see_matches.Classes.Remove("navbuttonselected");
                nav_see_teams.Classes.Remove("navbuttonselected");
                nav_see_tablets.Classes.Add("navbutton");
                nav_see_graph.Classes.Add("navbutton");
                nav_see_tba.Classes.Add("navbutton");
                nav_see_matches.Classes.Add("navbutton");
                nav_see_teams.Classes.Add("navbutton");
                await Task.Delay(100);
                matches.Opacity = 0;
                graph.Opacity = 0;
                tablets.Opacity = 0;
                tba.Opacity = 0;
                teams.Opacity = 0;
                teamDetails.Opacity = 0;
                matchDetails.Opacity = 0;
                matches.IsVisible = false;
                graph.IsVisible = false;
                tablets.IsVisible = false;
                actualgraphs.IsVisible = false;
                tba.IsVisible = false;
                teams.IsVisible = false;
                teamDetails.IsVisible = false;
                matchDetails.IsVisible = false;
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

                /*var idevice = LibiMobileDevice.Instance.iDevice;
                var lockdown = LibiMobileDevice.Instance.Lockdown;
                idevice.idevice_event_subscribe((ref iDeviceEvent deviceEvent, IntPtr b) =>
                {
                    //checkForInternet();
                    try
                    {
                        iDeviceHandle deviceHandle;
                        idevice.idevice_new(out deviceHandle, deviceEvent.udidString);

                        LockdownClientHandle lockdownHandle;
                        lockdown.lockdownd_client_new_with_handshake(deviceHandle, out lockdownHandle, "LightScout").ThrowOnError();

                        string deviceName;
                        lockdown.lockdownd_get_device_name(lockdownHandle, out deviceName).ThrowOnError();
                        Task.Run(async () => {
                            while (true)
                            {
                                var error = idevice.idevice_connect(deviceHandle, 862, out iDeviceConnectionHandle connection2);
                                if (error == iDeviceError.NoDevice || error == iDeviceError.SslError)
                                {
                                    Console.WriteLine("UWU");
                                    break;
                                }
                                if (error == iDeviceError.Success)
                                {
                                    ReceiveDataFromDevice(connection2, idevice);
                                    break;
                                }
                            }
                        });
                            
                        


                        
                    }
                    catch (Exception ex)
                    {
                        
                    }


                }
                , new IntPtr(862));*/
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
                        //connection.Dispose();
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
                    Dispatcher.UIThread.Post(async () => {
                        bluetooth_status.Classes.Add("hide");
                        await Task.Delay(100);
                        bluetooth_status.Opacity = 0;
                        bluetooth_status.IsVisible = false;
                        control.NotificationViewModel.AddNotification("Unavailable", "The LightSwitch Bluetooth service is not available on this device!", "#fe5b5b", null);
                    });
                    
                };
                var exitEvent = new ManualResetEvent(false);
                var url = new Uri("ws://localhost:9958");
                client.Url = new Uri("ws://localhost:9958");
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
                            control.NotificationViewModel.AddNotification("Ready", "The Bluetooth service is operational and ready to relay messages!", "#F64A8A", null);
                        });
                        
                    }
                    else
                    {
                        var rawstring = msg.Text;
                        UseGivenDataBeta(rawstring, true);
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
                        control.NotificationViewModel.AddNotification("Not Ready", "We disconnected from the service. Attempting to reconnect...", "#fe5b5b", null);
                    });
                    
                });
            }
            catch (Exception ex)
            {
                Dispatcher.UIThread.Post(async () => {
                    bluetooth_status.Classes.Add("hide");
                    await Task.Delay(100);
                    bluetooth_status.Opacity = 0;
                    bluetooth_status.IsVisible = false;
                    control.NotificationViewModel.AddNotification("Unavailable", "The LightSwitch Bluetooth service is not available on this device!", "#fe5b5b", null);
                });
                
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
        public void UseGivenDataBeta(string rawdata, bool bluetooth)
        {
            AuthenticationLevel Lock = AuthenticationLevel.Authorized;
            AuthenticationLevel CurrentAuthLevel = AuthenticationLevel.Administrator;
            /*
             FORMAT STRING EXAMPLE:

             R1:a-12345678:01:@12:00*Y>>(data)

            RETURN EXAMPLE:

            S!0^a-12345678>>ABCD

             */
            string tabletid = rawdata.Substring(0, 2);
            TimeSpan timeOfRequest = TimeSpan.Parse(rawdata.Substring(17, 5));
            string uniqueId = rawdata.Substring(3, 10);
            TabletOS deviceOS = (rawdata.Substring(3, 1) == "i") ? TabletOS.iOS : TabletOS.Android;
            int messageType = int.Parse(rawdata.Substring(14, 2));
            string responseExpectation = rawdata.Substring(23, 1);
            string data = rawdata.Substring(26);
            using(var db = new ScoutingContext()) 
            {
                if (CurrentAuthLevel >= Lock)
                {
                    var instanceOfTablet = db.TabletInstances.Where(x => x.Identifier == uniqueId).FirstOrDefault();
                    if(instanceOfTablet == null)
                    {
                        instanceOfTablet = new TabletInstance() { Identifier = uniqueId, AuthenticationLevel = 1, ColorId = tabletid, LastCommunicated = DateTime.Now, DiagnosticReportReceived = DateTime.Now };
                        try
                        {
                            db.TabletInstances.Add(instanceOfTablet);
                        }
                        catch(Exception ex)
                        {

                        }
                        
                    }
                    db.SaveChanges();
                    Console.WriteLine("Authenticated");
                    instanceOfTablet.LastCommunicated = DateTime.Now;
                    switch (messageType)
                    {
                        case 0:
                            //CRIT EMERGENCY
                            break;
                        case 1:
                            //TECH EMERGENCY
                            break;
                        case 2:
                            control.TabletViewModel.Tablets.ToArray()[control.TabletViewModel.Tablets.IndexOf(control.TabletViewModel.Tablets.Where(x => x.Identifier == uniqueId).FirstOrDefault())].BatteryLevel = data + "%";
                            break;
                        case 4:
                            var diagnosticreport = JsonConvert.DeserializeObject<DiagnosticReport>(data);
                            if(diagnosticreport.currentConnection != Connectivity.None)
                            {
                                control.NotificationViewModel.AddNotification("Check Internet", "Tablet with UID " + uniqueId.ToString() + " has their WiFi on! Please change this!", "#fe5b5b", null);
                            }
                            instanceOfTablet.LastCommunicated = DateTime.Now;
                            db.TabletInstances.Update(instanceOfTablet);
                            break;
                        case 5:
                            //DEVICE LOG REPORT
                            break;
                        case 6:
                            //AUTHORIZATION REQUEST
                            break;
                        case 7:
                            //ADMINISTRATOR REQUEST
                            break;
                        case 10:
                            
                            var itemstouse = JsonConvert.DeserializeObject<List<IO_TeamMatch>>(data);
                            foreach (var itemtouse in itemstouse)
                            {
                                try
                                {
                                    var previousitem = db.Matches.Where(x => x.TabletId == itemtouse.TabletId && x.MatchNumber == itemtouse.MatchNumber && x.EventCode == itemtouse.EventCode).FirstOrDefault();
                                    if (previousitem == null)
                                    {
                                        TeamMatch newTeamMatch = new TeamMatch() { DefenseFor = itemtouse.DefenseFor, DefenseAgainst = itemtouse.DefenseAgainst, LoadCoordinates = itemtouse.LoadCoordinates, ShotCoordinates = itemtouse.ShotCoordinates, CycleTime = itemtouse.CycleTime, AlliancePartners = itemtouse.AlliancePartners, A_InitiationLine = itemtouse.A_InitiationLine, ClientLastSubmitted = itemtouse.ClientLastSubmitted, ClientSubmitted = true, DisabledSeconds = itemtouse.DisabledSeconds, IsQualifying = itemtouse.IsQualifying, EventCode = itemtouse.EventCode, E_Balanced = itemtouse.E_Balanced, E_ClimbAttempt = itemtouse.E_ClimbAttempt, E_ClimbSuccess = itemtouse.E_ClimbSuccess, E_Park = itemtouse.E_Park, MatchID = new Random().Next(1, 1000000), MatchNumber = itemtouse.MatchNumber, NumCycles = itemtouse.NumCycles, PowerCellInner = itemtouse.PowerCellInner, PowerCellLower = itemtouse.PowerCellLower, PowerCellMissed = itemtouse.PowerCellMissed, PowerCellOuter = itemtouse.PowerCellOuter, RobotPosition = itemtouse.RobotPosition, ScoutName = itemtouse.ScoutName, TabletId = itemtouse.TabletId, TapLogs = itemtouse.TapLogs, TeamName = itemtouse.TeamName, T_ControlPanelPosition = itemtouse.T_ControlPanelPosition, T_ControlPanelRotation = itemtouse.T_ControlPanelRotation };
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
                                            TeamMatch newTeamMatch = new TeamMatch() { DefenseFor = itemtouse.DefenseFor, DefenseAgainst = itemtouse.DefenseAgainst, LoadCoordinates = itemtouse.LoadCoordinates, ShotCoordinates = itemtouse.ShotCoordinates, CycleTime = itemtouse.CycleTime, AlliancePartners = itemtouse.AlliancePartners, A_InitiationLine = itemtouse.A_InitiationLine, ClientLastSubmitted = itemtouse.ClientLastSubmitted, ClientSubmitted = true, DisabledSeconds = itemtouse.DisabledSeconds, IsQualifying = itemtouse.IsQualifying, EventCode = previousitem.EventCode, E_Balanced = itemtouse.E_Balanced, E_ClimbAttempt = itemtouse.E_ClimbAttempt, E_ClimbSuccess = itemtouse.E_ClimbSuccess, E_Park = itemtouse.E_Park, MatchID = previousitem.MatchID, MatchNumber = previousitem.MatchNumber, NumCycles = itemtouse.NumCycles, PowerCellInner = itemtouse.PowerCellInner, PowerCellLower = itemtouse.PowerCellLower, PowerCellMissed = itemtouse.PowerCellMissed, PowerCellOuter = itemtouse.PowerCellOuter, RobotPosition = itemtouse.RobotPosition, ScoutName = itemtouse.ScoutName, TabletId = itemtouse.TabletId, TapLogs = itemtouse.TapLogs, TeamName = itemtouse.TeamName, T_ControlPanelPosition = itemtouse.T_ControlPanelPosition, T_ControlPanelRotation = itemtouse.T_ControlPanelRotation };
                                            try
                                            {
                                                newTeamMatch.TrackedTeam = previousitem.TrackedTeam;
                                                newTeamMatch.team_instance_id = previousitem.team_instance_id;
                                            }
                                            catch (Exception ex)
                                            {
                                                FRCTeamModel newTeamInDb = new FRCTeamModel() { event_key = itemtouse.EventCode, team_number = itemtouse.TeamNumber };
                                                db.FRCTeams.Add(newTeamInDb);
                                                db.SaveChanges();
                                                newTeamMatch.TrackedTeam = newTeamInDb;
                                                newTeamMatch.team_instance_id = db.FRCTeams.Where(x => x.team_number == newTeamInDb.team_number && x.event_key == newTeamInDb.event_key).FirstOrDefault().team_instance_id;
                                            }

                                            db.Entry<TeamMatch>(previousitem).CurrentValues.SetValues(newTeamMatch);
                                            db.SaveChanges();
                                        }

                                    }


                                }
                                catch (NpgsqlException ex)
                                {
                                    TeamMatch newTeamMatch = new TeamMatch() { DefenseFor = itemtouse.DefenseFor, DefenseAgainst = itemtouse.DefenseAgainst, LoadCoordinates = itemtouse.LoadCoordinates, ShotCoordinates = itemtouse.ShotCoordinates, CycleTime = itemtouse.CycleTime, AlliancePartners = itemtouse.AlliancePartners, A_InitiationLine = itemtouse.A_InitiationLine, ClientLastSubmitted = itemtouse.ClientLastSubmitted, ClientSubmitted = true, DisabledSeconds = itemtouse.DisabledSeconds, IsQualifying = itemtouse.IsQualifying, EventCode = itemtouse.EventCode, E_Balanced = itemtouse.E_Balanced, E_ClimbAttempt = itemtouse.E_ClimbAttempt, E_ClimbSuccess = itemtouse.E_ClimbSuccess, E_Park = itemtouse.E_Park, MatchID = new Random().Next(1, 1000000), MatchNumber = itemtouse.MatchNumber, NumCycles = itemtouse.NumCycles, PowerCellInner = itemtouse.PowerCellInner, PowerCellLower = itemtouse.PowerCellLower, PowerCellMissed = itemtouse.PowerCellMissed, PowerCellOuter = itemtouse.PowerCellOuter, RobotPosition = itemtouse.RobotPosition, ScoutName = itemtouse.ScoutName, TabletId = itemtouse.TabletId, TapLogs = itemtouse.TapLogs, TeamName = itemtouse.TeamName, T_ControlPanelPosition = itemtouse.T_ControlPanelPosition, T_ControlPanelRotation = itemtouse.T_ControlPanelRotation };
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
                            control.NotificationViewModel.AddNotification("Scored!", ((instanceOfTablet.TabletName != null && instanceOfTablet.TabletName != "") ? instanceOfTablet.TabletName : instanceOfTablet.Identifier) + " has submitted scores under color " + instanceOfTablet.ColorId, "#07d0de", null);

                            db.SaveChanges();
                            break;
                        case 11:
                            var listofofficialmatches = JsonConvert.DeserializeObject<List<TBA_Match>>(data);
                            foreach (var om in listofofficialmatches)
                            {
                                if (db.TBAMatches.Find(om.key) != null)
                                {
                                    try
                                    {
                                        var previousentry = db.TBAMatches.Find(om.key);
                                        previousentry.rawjson = JsonConvert.SerializeObject(om);
                                        db.TBAMatches.Update(previousentry);
                                    }
                                    catch (Exception ex)
                                    {
                                        TBA_DB_Model newDBEntry = new TBA_DB_Model() { event_key = om.event_key, match_number = om.match_number, key = om.key, rawjson = JsonConvert.SerializeObject(om) };
                                        if (om.comp_level == "qm")
                                        {
                                            newDBEntry.isqualifying = true;
                                        }
                                        else
                                        {
                                            newDBEntry.isqualifying = false;
                                        }
                                        db.TBAMatches.Add(newDBEntry);
                                    }

                                }
                                else
                                {
                                    TBA_DB_Model newDBEntry = new TBA_DB_Model() { event_key = om.event_key, match_number = om.match_number, key = om.key, rawjson = JsonConvert.SerializeObject(om) };
                                    if (om.comp_level == "qm")
                                    {
                                        newDBEntry.isqualifying = true;
                                    }
                                    else
                                    {
                                        newDBEntry.isqualifying = false;
                                    }
                                    db.TBAMatches.Add(newDBEntry);
                                }


                            }
                            db.SaveChanges();
                            break;
                        case 12:
                            //TBA MATCHUP REPORT
                            break;
                        case 13:
                            //CHANGE IDENTIFIER NOTICE
                            break;
                        case 20:
                            
                            GetEventCode eventConfig = new GetEventCode();
                            var listOfDBMatchesToSend = db.Matches.Where(x => x.EventCode == configuration.EventCode && x.TabletId == rawdata.Substring(0, 2)).ToList();
                            var listOfMatchesToSend = new List<IO_TeamMatch>();
                            foreach (var dbm in listOfDBMatchesToSend.OrderBy(x => x.MatchNumber))
                            {
                                var correspondingteam = db.FRCTeams.Find(dbm.team_instance_id);
                                var alliancepartner1 = db.FRCTeams.Find(dbm.AlliancePartners[0]);
                                var alliancepartner2 = db.FRCTeams.Find(dbm.AlliancePartners[1]);
                                listOfMatchesToSend.Add(new IO_TeamMatch() { AlliancePartners = new int[2] { alliancepartner1.team_number, alliancepartner2.team_number }, EventCode = dbm.EventCode, TeamNumber = correspondingteam.team_number, MatchNumber = dbm.MatchNumber, TabletId = dbm.TabletId, PowerCellInner = new int[21], PowerCellOuter = new int[21], PowerCellLower = new int[21], PowerCellMissed = new int[21] });
                            }
                            string modelstring = JsonConvert.SerializeObject(listOfMatchesToSend);
                            byte[] bytesToSend = Encoding.ASCII.GetBytes(modelstring);
                            if (modelstring.Length > 480)
                            {
                                int numberofmessages = (int)Math.Ceiling((float)modelstring.Length / (float)480);
                                var startidentifier = "L!2^" + uniqueId.ToString() + ">>" + numberofmessages.ToString();
                                client.Send(startidentifier);
                                for (int i = numberofmessages; i > 0; i--)
                                {
                                    var simplestring = "F!2^" + uniqueId + ">>" + new string(modelstring.Skip((numberofmessages - i) * 480).Take(480).ToArray());
                                    client.Send(simplestring);
                                }
                            }
                            else
                            {
                                client.Send("S!2^" + uniqueId + ">>" + modelstring);
                            }
                            control.NotificationViewModel.AddNotification("Data Request", rawdata.Substring(0, 2).ToString() + " requested data", "#00A572", null);
                            break;
                        case 21:
                            //CONFIG REQUEST
                            break;
                        case 22:
                            //SYNC AUTH LEVEL
                            break;
                        case 40:
                            var passedDBConfig = JsonConvert.DeserializeObject<DBConfiguraton>(data);
                            bool successful = true;
                            try
                            {

                            }
                            catch(Exception ex)
                            {
                                successful = false;
                            }
                            if (successful)
                            {
                                var keygenerated = Guid.NewGuid();
                                if (ApplicationState.DBAuthTokens.ContainsKey(uniqueId))
                                {
                                    ApplicationState.DBAuthTokens.Remove(uniqueId);
                                }
                                ApplicationState.DBAuthTokens.Add(uniqueId, new DBAuthorizationToken() { Token = keygenerated, TimesAccessed = 0, Assigned = DateTime.Now, Expires = DateTime.Now + TimeSpan.FromHours(1) });

                                client.Send("S!2^" + uniqueId + ">>" + keygenerated.ToString());
                            }

                            break;
                        case 41:
                            if (ApplicationState.DBAuthTokens.ContainsKey(uniqueId))
                            {
                                var selectedDBToken = ApplicationState.DBAuthTokens.GetValueOrDefault(uniqueId);
                                if(DateTime.Now > selectedDBToken.Expires)
                                {
                                    ApplicationState.DBAuthTokens.GetValueOrDefault(uniqueId).TimesAccessed += 1;
                                    var parseddata = JsonConvert.DeserializeObject<ExternalDBQuery>(data);
                                    var allteamsselectedbydata = db.FRCTeams.Where(x => parseddata.IncludedTeams.Contains(x.ToString())).ToList();
                                    List<ExternalDBQueryResult> sendBackList = new List<ExternalDBQueryResult>();
                                    List<GraphTeamMatchView> rawTeamDatas = new List<GraphTeamMatchView>();
                                    foreach(var team in allteamsselectedbydata)
                                    {
                                        var allmatchesinteam = db.Matches.Where(x => x.team_instance_id == team.team_instance_id && x.ClientSubmitted == true).ToList();
                                        GraphTeamMatchView useForAdding = new GraphTeamMatchView();
                                        useForAdding.TeamNumber = team.team_number;
                                        foreach(var match in allmatchesinteam)
                                        {
                                            useForAdding.TInnerPC += match.PowerCellInner.Sum() - match.PowerCellInner[0];
                                            useForAdding.TOuterPC += match.PowerCellInner.Sum() - match.PowerCellInner[0];
                                            useForAdding.TLowerPC += match.PowerCellInner.Sum() - match.PowerCellInner[0];
                                            useForAdding.TMissedPC += match.PowerCellMissed.Sum() - match.PowerCellMissed[0];
                                            useForAdding.AInnerPC += match.PowerCellInner[0];
                                            useForAdding.AOuterPC += match.PowerCellInner[0];
                                            useForAdding.ALowerPC += match.PowerCellInner[0];
                                            useForAdding.AMissedPC += match.PowerCellMissed[0];
                                            useForAdding.BalanceRate += match.E_Balanced ? 1 : 0;
                                            useForAdding.ClimbRate += match.E_ClimbSuccess ? 1 : 0;
                                            useForAdding.ParkRate += match.E_Park ? 1 : 0;
                                            useForAdding.TotalInnerPC += match.PowerCellInner.Sum();
                                            useForAdding.TotalOuterPC += match.PowerCellInner.Sum();
                                            useForAdding.TotalLowerPC += match.PowerCellInner.Sum();
                                            useForAdding.TotalMissedPC += match.PowerCellMissed.Sum();
                                            useForAdding.CycleTime += match.CycleTime;
                                            useForAdding.DefenseRate += match.DefenseFor ? 1 : 0;
                                            useForAdding.Disabled += match.DisabledSeconds;
                                            useForAdding.NumCycles += match.NumCycles;
                                        }
                                        if(allmatchesinteam.Count > 0)
                                        {
                                            useForAdding.TInnerPC += useForAdding.TInnerPC / allmatchesinteam.Count;
                                            useForAdding.TOuterPC += useForAdding.TOuterPC / allmatchesinteam.Count;
                                            useForAdding.TLowerPC += useForAdding.TLowerPC / allmatchesinteam.Count;
                                            useForAdding.TMissedPC += useForAdding.TMissedPC / allmatchesinteam.Count;
                                            useForAdding.AInnerPC += useForAdding.AInnerPC / allmatchesinteam.Count;
                                            useForAdding.AOuterPC += useForAdding.AOuterPC / allmatchesinteam.Count;
                                            useForAdding.ALowerPC += useForAdding.ALowerPC / allmatchesinteam.Count;
                                            useForAdding.AMissedPC += useForAdding.AMissedPC / allmatchesinteam.Count;
                                            useForAdding.BalanceRate += useForAdding.BalanceRate / allmatchesinteam.Count;
                                            useForAdding.ClimbRate += useForAdding.ClimbRate / allmatchesinteam.Count;
                                            useForAdding.ParkRate += useForAdding.ParkRate / allmatchesinteam.Count;
                                            useForAdding.TotalInnerPC += useForAdding.TotalInnerPC / allmatchesinteam.Count;
                                            useForAdding.TotalOuterPC += useForAdding.TotalOuterPC / allmatchesinteam.Count;
                                            useForAdding.TotalLowerPC += useForAdding.TotalLowerPC / allmatchesinteam.Count;
                                            useForAdding.TotalMissedPC += useForAdding.TotalMissedPC / allmatchesinteam.Count;
                                            useForAdding.CycleTime += useForAdding.CycleTime / allmatchesinteam.Count;
                                            useForAdding.DefenseRate += useForAdding.DefenseRate / allmatchesinteam.Count;
                                            useForAdding.Disabled += useForAdding.Disabled / allmatchesinteam.Count;
                                            useForAdding.NumCycles += useForAdding.NumCycles / allmatchesinteam.Count;
                                        }
                                        rawTeamDatas.Add(useForAdding);
                                    }
                                    var trackstring = "";
                                    foreach (var rawordertype in parseddata.OrderByProperties)
                                    {
                                        string trackingtype = rawordertype;
                                        
                                        if (trackstring != "")
                                        {
                                            trackstring = trackstring + ", ";
                                        }
                                        trackstring = trackstring + trackingtype;
                                    }
                                    rawTeamDatas = OrderByHelper.OrderBy(rawTeamDatas, trackstring).ToList();
                                    foreach (var team in rawTeamDatas)
                                    {
                                        ExternalDBQueryResult toAdd = new ExternalDBQueryResult() { TeamNumber = team.TeamNumber };
                                        foreach (var sortingtype in parseddata.ReturnProperties)
                                        {
                                            PropertyInfo pinfo = typeof(GraphTeamMatchView).GetProperty(sortingtype);
                                            toAdd.CorrespondingResults.Add((double)pinfo.GetValue(team, null));
                                        }
                                        sendBackList.Add(toAdd);
                                    }
                                    client.Send("S!2^" + uniqueId + ">>" + JsonConvert.SerializeObject(sendBackList));
                                }
                                else
                                {
                                    client.Send("S!2^" + uniqueId + ">>" + "401");
                                    ApplicationState.DBAuthTokens.Remove(uniqueId);
                                }
                            }
                            else
                            {
                                client.Send("S!2^" + uniqueId + ">>" + "401");
                            }
                            break;
                        case 42:
                            if (ApplicationState.DBAuthTokens.ContainsKey(uniqueId))
                            {
                                var selectedDBToken = ApplicationState.DBAuthTokens.GetValueOrDefault(uniqueId);
                                if (DateTime.Now > selectedDBToken.Expires)
                                {
                                    ApplicationState.DBAuthTokens.GetValueOrDefault(uniqueId).TimesAccessed += 1;
                                    //SET DATA
                                }
                                else
                                {
                                    client.Send("S!2^" + uniqueId + ">>" + "401");
                                    ApplicationState.DBAuthTokens.Remove(uniqueId);
                                }
                            }
                            else
                            {
                                client.Send("S!2^" + uniqueId + ">>" + "401");
                            }
                            break;
                        case 43:
                            try
                            {
                                ApplicationState.DBAuthTokens.Remove(uniqueId);
                            }
                            catch(Exception ex)
                            {

                            }
                            break;

                    }
                }
                else
                {
                    Console.WriteLine("Not Powerful Enough");
                }
                control.TabletViewModel.UpdateValues();
            }
            
            Console.WriteLine("Test Parse Completed");
            
        }
        public enum TabletOS
        {
            iOS,
            Android
        }
        public enum AuthenticationLevel
        {
            Unauthorized,
            Guest,
            Authorized,
            Administrator
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
            //control.TabletViewModel.LastPings[tabletindex] = "Last ping at: " + DateTime.Now.ToShortTimeString();
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
                control.NotificationViewModel.AddNotification("Scored", datawithoutid.Substring(0, 2).ToString() + " sent scores", "Green", null);
                //control.TabletViewModel.BluetoothBackgroundColors[tabletindex] = "LightBlue";
                //control.TabletViewModel.BluetoothBorderColors[tabletindex] = "Blue";
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
                                TeamMatch newTeamMatch = new TeamMatch() { DefenseFor = itemtouse.DefenseFor, DefenseAgainst = itemtouse.DefenseAgainst, LoadCoordinates = itemtouse.LoadCoordinates, ShotCoordinates = itemtouse.ShotCoordinates, CycleTime = itemtouse.CycleTime, AlliancePartners = itemtouse.AlliancePartners, A_InitiationLine = itemtouse.A_InitiationLine, ClientLastSubmitted = itemtouse.ClientLastSubmitted, ClientSubmitted = true, DisabledSeconds = itemtouse.DisabledSeconds, IsQualifying = itemtouse.IsQualifying, EventCode = itemtouse.EventCode, E_Balanced = itemtouse.E_Balanced, E_ClimbAttempt = itemtouse.E_ClimbAttempt, E_ClimbSuccess = itemtouse.E_ClimbSuccess, E_Park = itemtouse.E_Park, MatchID = new Random().Next(1, 1000000), MatchNumber = itemtouse.MatchNumber, NumCycles = itemtouse.NumCycles, PowerCellInner = itemtouse.PowerCellInner, PowerCellLower = itemtouse.PowerCellLower, PowerCellMissed = itemtouse.PowerCellMissed, PowerCellOuter = itemtouse.PowerCellOuter, RobotPosition = itemtouse.RobotPosition, ScoutName = itemtouse.ScoutName, TabletId = itemtouse.TabletId, TapLogs = itemtouse.TapLogs, TeamName = itemtouse.TeamName, T_ControlPanelPosition = itemtouse.T_ControlPanelPosition, T_ControlPanelRotation = itemtouse.T_ControlPanelRotation };
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
                                    TeamMatch newTeamMatch = new TeamMatch() { DefenseFor = itemtouse.DefenseFor, DefenseAgainst = itemtouse.DefenseAgainst, LoadCoordinates = itemtouse.LoadCoordinates, ShotCoordinates = itemtouse.ShotCoordinates, CycleTime = itemtouse.CycleTime, AlliancePartners = itemtouse.AlliancePartners, A_InitiationLine = itemtouse.A_InitiationLine, ClientLastSubmitted = itemtouse.ClientLastSubmitted, ClientSubmitted = true, DisabledSeconds = itemtouse.DisabledSeconds, IsQualifying = itemtouse.IsQualifying, EventCode = previousitem.EventCode, E_Balanced = itemtouse.E_Balanced, E_ClimbAttempt = itemtouse.E_ClimbAttempt, E_ClimbSuccess = itemtouse.E_ClimbSuccess, E_Park = itemtouse.E_Park, MatchID = previousitem.MatchID, MatchNumber = previousitem.MatchNumber, NumCycles = itemtouse.NumCycles, PowerCellInner = itemtouse.PowerCellInner, PowerCellLower = itemtouse.PowerCellLower, PowerCellMissed = itemtouse.PowerCellMissed, PowerCellOuter = itemtouse.PowerCellOuter, RobotPosition = itemtouse.RobotPosition, ScoutName = itemtouse.ScoutName, TabletId = itemtouse.TabletId, TapLogs = itemtouse.TapLogs, TeamName = itemtouse.TeamName, T_ControlPanelPosition = itemtouse.T_ControlPanelPosition, T_ControlPanelRotation = itemtouse.T_ControlPanelRotation };
                                    try
                                    {
                                        newTeamMatch.TrackedTeam = previousitem.TrackedTeam;
                                        newTeamMatch.team_instance_id = previousitem.team_instance_id;
                                    }
                                    catch (Exception ex)
                                    {
                                        FRCTeamModel newTeamInDb = new FRCTeamModel() { event_key = itemtouse.EventCode, team_number = itemtouse.TeamNumber };
                                        db.FRCTeams.Add(newTeamInDb);
                                        db.SaveChanges();
                                        newTeamMatch.TrackedTeam = newTeamInDb;
                                        newTeamMatch.team_instance_id = db.FRCTeams.Where(x => x.team_number == newTeamInDb.team_number && x.event_key == newTeamInDb.event_key).FirstOrDefault().team_instance_id;
                                    }

                                    db.Entry<TeamMatch>(previousitem).CurrentValues.SetValues(newTeamMatch);
                                    db.SaveChanges();
                                }

                            }


                        }
                        catch (NpgsqlException ex)
                        {
                            TeamMatch newTeamMatch = new TeamMatch() { DefenseFor = itemtouse.DefenseFor, DefenseAgainst = itemtouse.DefenseAgainst, LoadCoordinates = itemtouse.LoadCoordinates, ShotCoordinates = itemtouse.ShotCoordinates, CycleTime = itemtouse.CycleTime, AlliancePartners = itemtouse.AlliancePartners, A_InitiationLine = itemtouse.A_InitiationLine, ClientLastSubmitted = itemtouse.ClientLastSubmitted, ClientSubmitted = true, DisabledSeconds = itemtouse.DisabledSeconds, IsQualifying = itemtouse.IsQualifying, EventCode = itemtouse.EventCode, E_Balanced = itemtouse.E_Balanced, E_ClimbAttempt = itemtouse.E_ClimbAttempt, E_ClimbSuccess = itemtouse.E_ClimbSuccess, E_Park = itemtouse.E_Park, MatchID = new Random().Next(1, 1000000), MatchNumber = itemtouse.MatchNumber, NumCycles = itemtouse.NumCycles, PowerCellInner = itemtouse.PowerCellInner, PowerCellLower = itemtouse.PowerCellLower, PowerCellMissed = itemtouse.PowerCellMissed, PowerCellOuter = itemtouse.PowerCellOuter, RobotPosition = itemtouse.RobotPosition, ScoutName = itemtouse.ScoutName, TabletId = itemtouse.TabletId, TapLogs = itemtouse.TapLogs, TeamName = itemtouse.TeamName, T_ControlPanelPosition = itemtouse.T_ControlPanelPosition, T_ControlPanelRotation = itemtouse.T_ControlPanelRotation };
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
                    //control.TabletViewModel.BatteryBackgroundColors[tabletindex] = "LightGreen";
                    //control.TabletViewModel.BatteryBorderColors[tabletindex] = "Green";
                }
                else if (batterylevel > 30 && batterylevel <= 80)
                {
                    //control.TabletViewModel.BatteryBackgroundColors[tabletindex] = "LightSalmon";
                    //control.TabletViewModel.BatteryBorderColors[tabletindex] = "DarkOrange";
                }
                else
                {
                    //control.TabletViewModel.BatteryBackgroundColors[tabletindex] = "LightPink";
                    //control.TabletViewModel.BatteryBorderColors[tabletindex] = "IndianRed";
                }
                //control.TabletViewModel.BatteryAmounts[tabletindex] = Math.Round(batterylevel).ToString();

            }
            else if (rawdata.Substring(3).StartsWith("D:"))
            {
                //D = Successful Disconnection
                //control.TabletViewModel.BluetoothBackgroundColors[tabletindex] = "LightGray";
                //control.TabletViewModel.BluetoothBorderColors[tabletindex] = "Gray";
            }
            else if (rawdata.Substring(3).StartsWith("FS:"))
            {
                //FS = Official FIRST Scores
                string datawithoutid = rawdata;
                int deviceid = 1000;
                if (bluetooth)
                {
                    datawithoutid = rawdata.Substring(0, 6) + rawdata.Substring(11);
                    deviceid = int.Parse(rawdata.Substring(6, 4));
                }
                var listofofficialmatches = JsonConvert.DeserializeObject<List<TBA_Match>>(datawithoutid.Substring(6));
                using(var db = new ScoutingContext())
                {
                    foreach (var om in listofofficialmatches)
                    {
                        if(db.TBAMatches.Find(om.key) != null)
                        {
                            try
                            {
                                var previousentry = db.TBAMatches.Find(om.key);
                                previousentry.rawjson = JsonConvert.SerializeObject(om);
                                db.TBAMatches.Update(previousentry);
                            }catch(Exception ex)
                            {
                                TBA_DB_Model newDBEntry = new TBA_DB_Model() { event_key = om.event_key, match_number = om.match_number, key = om.key, rawjson = JsonConvert.SerializeObject(om) };
                                if (om.comp_level == "qm")
                                {
                                    newDBEntry.isqualifying = true;
                                }
                                else
                                {
                                    newDBEntry.isqualifying = false;
                                }
                                db.TBAMatches.Add(newDBEntry);
                            }
                            
                        }
                        else
                        {
                            TBA_DB_Model newDBEntry = new TBA_DB_Model() { event_key = om.event_key, match_number = om.match_number, key = om.key, rawjson = JsonConvert.SerializeObject(om) };
                            if (om.comp_level == "qm")
                            {
                                newDBEntry.isqualifying = true;
                            }
                            else
                            {
                                newDBEntry.isqualifying = false;
                            }
                            db.TBAMatches.Add(newDBEntry);
                        }
                        

                    }
                    db.SaveChanges();
                }
                
                Console.WriteLine("Successfully gotten");
            }
            else if (rawdata.Substring(3).StartsWith("E:"))
            {
                //E = Immedient Communication
                //control.TabletViewModel.BluetoothBackgroundColors[tabletindex] = "LightSalmon";
                //control.TabletViewModel.BluetoothBorderColors[tabletindex] = "DarkOrange";
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

                control.NotificationViewModel.AddNotification("Data Request", rawdata.Substring(0, 2).ToString() + " requested data", "Blue", null);
                GetEventCode eventConfig = new GetEventCode();
                using(var db = new ScoutingContext())
                {
                    var listOfDBMatchesToSend = db.Matches.Where(x => x.EventCode == configuration.EventCode && x.TabletId == rawdata.Substring(0, 2)).ToList();
                    var listOfMatchesToSend = new List<IO_TeamMatch>();
                    foreach (var dbm in listOfDBMatchesToSend.OrderBy(x => x.MatchNumber))
                    {
                        var correspondingteam = db.FRCTeams.Find(dbm.team_instance_id);
                        var alliancepartner1 = db.FRCTeams.Find(dbm.AlliancePartners[0]);
                        var alliancepartner2 = db.FRCTeams.Find(dbm.AlliancePartners[1]);
                        listOfMatchesToSend.Add(new IO_TeamMatch() { AlliancePartners = new int[2] { alliancepartner1.team_number, alliancepartner2.team_number }, EventCode = dbm.EventCode, TeamNumber = correspondingteam.team_number, MatchNumber = dbm.MatchNumber, TabletId = dbm.TabletId, PowerCellInner = new int[21], PowerCellOuter = new int[21], PowerCellLower = new int[21], PowerCellMissed = new int[21] });
                    }
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
                    //control.TabletViewModel.BluetoothBackgroundColors[tabletindex] = "LightSalmon";
                    //control.TabletViewModel.BluetoothBorderColors[tabletindex] = "DarkOrange";
                }
                
            }


            //Console.WriteLine(msg.Text);
        }
    }
}
