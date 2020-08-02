﻿using InTheHand.Net.Sockets;
using LightMasterMVVM.DbAssets;
using LightMasterMVVM.Models;
using Newtonsoft.Json;
using Npgsql;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using OxyPlot.Axes;
    using OxyPlot.Series;
using Websocket.Client;
using OxyPlot.Avalonia;
using LightMasterMVVM.Scripts;
using LightMasterMVVM.Views;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.EntityFrameworkCore.Internal;
using Avalonia.Threading;
using System.Runtime.CompilerServices;
using iMobileDevice.iDevice;
using iMobileDevice;
using iMobileDevice.Lockdown;
using System.IO.Ports;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.IO;

namespace LightMasterMVVM.ViewModels
{
    public class NotificationViewModel : ViewModelBase
    {
        private ObservableCollection<NotificationItem> notifications = new ObservableCollection<NotificationItem>()
        {
            new NotificationItem(){ BackgroundColor = "#2a7afa", NotificationText = "HI THERE1", NotificationTitle = "Test", NotificationId = 1, NotificationActive = true, timeAdded = DateTime.Now },
            new NotificationItem(){ BackgroundColor = "#2a7afa", NotificationText = "HI THERE2", NotificationTitle = "Test", NotificationId = 2, NotificationActive = true, timeAdded = DateTime.Now },
            new NotificationItem(){ BackgroundColor = "#2a7afa", NotificationText = "HI THERE3", NotificationTitle = "Test", NotificationId = 3, NotificationActive = true, timeAdded = DateTime.Now }
        };
        public ObservableCollection<NotificationItem> Notifications
        {
            get => notifications;
            set => SetProperty(ref notifications, value);
        }
        public async void CancelNotification(int id)
        {
            Notifications.Remove(Notifications.Where(x => x.NotificationId == id).FirstOrDefault());
        }
        public void SetTest()
        {
            Console.WriteLine("REST");
        }
        public async void AddNotification(string title, string message, string color)
        {
            List<NotificationItem> oldNotifications = Notifications.ToList();
            if(oldNotifications.Count == 4)
            {
                oldNotifications = oldNotifications.OrderBy(x => x.timeAdded).ToList();
                oldNotifications.Remove(oldNotifications.First());
            }
            oldNotifications.Add(new NotificationItem() { BackgroundColor = color, NotificationActive = true, NotificationId = new Random().Next(1, 5000), NotificationText = message, NotificationTitle = title, timeAdded = DateTime.Now });
            ObservableCollection<NotificationItem> newNotifications = new ObservableCollection<NotificationItem>(oldNotifications);
            Notifications = newNotifications;
        }
    }
    public class TBAViewModel : ViewModelBase
    {
        private bool userControlVisible = true;
        public bool UserControlVisible
        {
            get => userControlVisible;
            set => SetProperty(ref userControlVisible, value);
        }
        private int minMatch = 0;
        private int maxMatch = 0;
        public int MinMatch
        {
            get => minMatch;
            set => SetProperty(ref minMatch, value);
        }
        public int MaxMatch
        {
            get => minMatch;
            set => SetProperty(ref minMatch, value);
        }
        public async void TheBlueAlliance()
        {
            var TBACheck = new TBAChecking();
            await TBACheck.CheckCurrentMatchesToDB(MinMatch, MaxMatch);
        }

    }
    public class GraphViewModel : ViewModelBase
    {
        private bool userControlVisible = false;
        public PlotController customController { get; private set; }
        private int graphHeight = 650;
        private PlotModel dataPoints = new PlotModel();
        public int GraphHeight
        {
            get => graphHeight;
            set => SetProperty(ref graphHeight, value);
        }
        public PlotModel DataPoints
        {
            get => dataPoints;
            set => SetProperty(ref dataPoints, value);
        }
        public bool UserControlVisible
        {
            get => userControlVisible;
            set => SetProperty(ref userControlVisible, value);
        }
        public void CreateGraphOfData()
        {
            /*customController = new PlotController();
            customController.UnbindMouseDown(OxyMouseButton.Left);
            customController.BindMouseEnter(PlotCommands.HoverSnapTrack);
            DataPoints = null;
            DataPoints = new PlotModel
            {
                Title = "Total Power Cells",
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.RightTop,
                LegendOrientation = LegendOrientation.Vertical,
                LegendBorderThickness = 0
            };
            var lowerpc = new OxyPlot.Series.LineSeries { Title = "Lower Power Cells" };
            var outerpc = new OxyPlot.Series.LineSeries { Title = "Outer Power Cells" };
            var innerpc = new OxyPlot.Series.LineSeries { Title = "Inner Power Cells" };
            var missedpc = new OxyPlot.Series.LineSeries { Title = "Missed Power Cells" };
            lowerpc.Points.Add(new DataPoint(1, 100));
            lowerpc.Points.Add(new DataPoint(2, 100));
            lowerpc.Points.Add(new DataPoint(3, 100));
            lowerpc.Points.Add(new DataPoint(4, 100));
            outerpc.Points.Add(new DataPoint(1, 105));
            outerpc.Points.Add(new DataPoint(2, 105));
            outerpc.Points.Add(new DataPoint(3, 105));
            outerpc.Points.Add(new DataPoint(4, 105));
            innerpc.Points.Add(new DataPoint(1, 110));
            innerpc.Points.Add(new DataPoint(2, 110));
            innerpc.Points.Add(new DataPoint(3, 110));
            innerpc.Points.Add(new DataPoint(4, 110));
            missedpc.Points.Add(new DataPoint(1, 115));
            missedpc.Points.Add(new DataPoint(2, 115));
            missedpc.Points.Add(new DataPoint(3, 115));
            missedpc.Points.Add(new DataPoint(4, 115));
            DataPoints.Series.Add(lowerpc);
            DataPoints.Series.Add(outerpc);
            DataPoints.Series.Add(innerpc);
            DataPoints.Series.Add(missedpc);
            DataPoints.Axes.Add(new OxyPlot.Axes.LinearAxis());
            DataPoints.Axes.Add(new OxyPlot.Axes.LinearAxis());*/
        }
        public GraphViewModel()
        {
            using (var db = new ScoutingContext())
            {
                customController = new PlotController();
                customController.UnbindMouseDown(OxyMouseButton.Left);
                customController.BindMouseEnter(PlotCommands.HoverSnapTrack);
                DataPoints = new PlotModel
                {
                    Title = "Average PC Count",
                    LegendPlacement = LegendPlacement.Outside,
                    LegendPosition = LegendPosition.RightTop,
                    LegendOrientation = LegendOrientation.Vertical,
                    LegendBorderThickness = 0,
                };

                List<TeamMatch> dbMatches = new List<TeamMatch>();
                try
                {
                    dbMatches = db.Matches.Where(x => x.ClientSubmitted == true && x.EventCode == "test_env").ToList();
                }
                catch(Exception ex)
                {
                    dbMatches = new List<TeamMatch>();
                }
                List<int> selectedTeamNumbers = new List<int>();
                List<AveragePCCountModel> averagePCCountModels = new List<AveragePCCountModel>();

                foreach (var entry in dbMatches)
                {
                    AveragePCCountModel modelToAdd = new AveragePCCountModel();
                    if (!selectedTeamNumbers.Contains(entry.TeamNumber))
                    {
                        modelToAdd.TeamNumber = entry.TeamNumber;
                        selectedTeamNumbers.Add(entry.TeamNumber);
                        var inner = 0;
                        var outer = 0;
                        var lower = 0;
                        var total = 0;
                        foreach (var toadd in entry.PowerCellInner)
                        {
                            inner += toadd;
                            total += toadd;
                        }
                        foreach (var toadd in entry.PowerCellOuter)
                        {
                            outer += toadd;
                            total += toadd;
                        }
                        foreach (var toadd in entry.PowerCellLower)
                        {
                            lower += toadd;
                            total += toadd;
                        }
                        modelToAdd.AvgInnerPC = inner;
                        modelToAdd.AvgOuterPC = outer;
                        modelToAdd.AvgTotalPC = total;
                        modelToAdd.AvgLowerPC = lower;
                        modelToAdd.Matches++;
                        averagePCCountModels.Add(modelToAdd);
                    }
                    else
                    {
                        modelToAdd = averagePCCountModels.Where(x => x.TeamNumber == entry.TeamNumber).FirstOrDefault();
                        modelToAdd.TeamNumber = entry.TeamNumber;
                        var inner = modelToAdd.AvgInnerPC * modelToAdd.Matches;
                        var outer = modelToAdd.AvgOuterPC * modelToAdd.Matches;
                        var lower = modelToAdd.AvgLowerPC * modelToAdd.Matches;
                        var total = modelToAdd.AvgTotalPC * modelToAdd.Matches;
                        foreach (var toadd in entry.PowerCellInner)
                        {
                            inner += toadd;
                            total += toadd;
                        }
                        foreach (var toadd in entry.PowerCellOuter)
                        {
                            outer += toadd;
                            total += toadd;
                        }
                        foreach (var toadd in entry.PowerCellLower)
                        {
                            lower += toadd;
                            total += toadd;
                        }
                        modelToAdd.Matches++;
                        modelToAdd.AvgInnerPC = (int)Math.Ceiling((float)inner / (float)modelToAdd.Matches);
                        modelToAdd.AvgOuterPC = (int)Math.Ceiling((float)outer / (float)modelToAdd.Matches);
                        modelToAdd.AvgTotalPC = (int)Math.Ceiling((float)total / (float)modelToAdd.Matches);
                        modelToAdd.AvgLowerPC = (int)Math.Ceiling((float)lower / (float)modelToAdd.Matches);
                        averagePCCountModels.Remove(averagePCCountModels.Where(x => x.TeamNumber == entry.TeamNumber).FirstOrDefault());
                        averagePCCountModels.Add(modelToAdd);
                    }

                }

                var s1 = new OxyPlot.Series.BarSeries { Title = "Lower Power Cells", StrokeColor = OxyColors.Black, StrokeThickness = 1 };
                var s2 = new OxyPlot.Series.BarSeries { Title = "Outer Power Cells", StrokeColor = OxyColors.Black, StrokeThickness = 1 };
                var s3 = new OxyPlot.Series.BarSeries { Title = "Inner Power Cells", StrokeColor = OxyColors.Black, StrokeThickness = 1 };
                var categoryAxis = new OxyPlot.Axes.CategoryAxis { Position = AxisPosition.Left, AxisTickToLabelDistance = 2 };
                s1.IsStacked = true;
                s1.LabelPlacement = LabelPlacement.Inside;
                s1.FillColor = OxyColors.LightYellow;
                s1.LabelMargin = 5;
                s1.BaseValue = 1;
                s1.TextColor = OxyColors.Black;
                s1.LabelFormatString = "{0} PC";
                s2.LabelPlacement = LabelPlacement.Inside;
                s2.IsStacked = true;
                s2.LabelMargin = 5;
                s2.BaseValue = 1;
                s2.LabelFormatString = "{0} PC";
                s2.TextColor = OxyColors.White;
                s2.FillColor = OxyColors.LightSeaGreen;
                s3.IsStacked = true;
                s3.BaseValue = 1;
                s3.FillColor = OxyColors.LightBlue;
                s3.LabelPlacement = LabelPlacement.Inside;
                s3.LabelFormatString = "{0} PC";
                s3.LabelMargin = 5;
                s3.TextColor = OxyColors.Black;
                var i = 0;
                foreach (var team in averagePCCountModels.OrderBy(x => x.AvgTotalPC).ThenBy(x => x.AvgInnerPC).ThenBy(x => x.AvgOuterPC).ThenBy(x => x.AvgOuterPC))
                {

                    s1.Items.Add(new BarItem { Value = team.AvgLowerPC });
                    s2.Items.Add(new BarItem { Value = team.AvgOuterPC });
                    s3.Items.Add(new BarItem { Value = team.AvgInnerPC });
                    categoryAxis.Labels.Add(team.TeamNumber.ToString());
                    i++;
                }
                GraphHeight = (categoryAxis.Labels.Count * 50) + 100;
                var valueAxis = new OxyPlot.Axes.LinearAxis { Position = AxisPosition.Bottom, MinimumPadding = 0, MaximumPadding = 0.06, AbsoluteMinimum = 0 };
                DataPoints.Series.Add(s1);
                DataPoints.Series.Add(s2);
                DataPoints.Series.Add(s3);
                DataPoints.Axes.Add(categoryAxis);
                DataPoints.Axes.Add(valueAxis);
            }
            /*customController = new PlotController();
            customController.UnbindMouseDown(OxyMouseButton.Left);
            customController.BindMouseEnter(PlotCommands.HoverSnapTrack);
            DataPoints = new PlotModel
            {
                Title = "Total Power Cells",
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.RightTop,
                LegendOrientation = LegendOrientation.Vertical,
                LegendBorderThickness = 0,
            };

            var s1 = new OxyPlot.Series.BarSeries { Title = "Lower Power Cells", StrokeColor = OxyColors.Black, StrokeThickness = 1 };
            s1.IsStacked = true;
            s1.LabelPlacement = LabelPlacement.Inside;
            s1.FillColor = OxyColors.LightYellow;
            s1.LabelMargin = 5;
            s1.TextColor = OxyColors.Black;
            s1.LabelFormatString = "{0} PC";
            s1.Items.Add(new BarItem { Value = 1 });
            s1.Items.Add(new BarItem { Value = 2 });
            s1.Items.Add(new BarItem { Value = 3 });
            s1.Items.Add(new BarItem { Value = 4 });
            s1.Items.Add(new BarItem { Value = 4 });
            s1.Items.Add(new BarItem { Value = 4 });
            s1.Items.Add(new BarItem { Value = 6 });
            s1.Items.Add(new BarItem { Value = 8 });
            s1.Items.Add(new BarItem { Value = 8 });
            s1.Items.Add(new BarItem { Value = 8 });

            var s2 = new OxyPlot.Series.BarSeries { Title = "Outer Power Cells", StrokeColor = OxyColors.Black, StrokeThickness = 1 };
            s2.LabelPlacement = LabelPlacement.Inside;
            s2.IsStacked = true;
            s2.LabelMargin = 5;
            s2.LabelFormatString = "{0} PC";
            s2.TextColor = OxyColors.White;
            s2.FillColor = OxyColors.LightSeaGreen;
            s2.Items.Add(new BarItem { Value = 5 });
            s2.Items.Add(new BarItem { Value = 2 });
            s2.Items.Add(new BarItem { Value = 4 });
            s2.Items.Add(new BarItem { Value = 5 });
            s2.Items.Add(new BarItem { Value = 2 });
            s2.Items.Add(new BarItem { Value = 4 });
            s2.Items.Add(new BarItem { Value = 6 });
            s2.Items.Add(new BarItem { Value = 10 });
            s2.Items.Add(new BarItem { Value = 2 });
            s2.Items.Add(new BarItem { Value = 5 });

            var s3 = new OxyPlot.Series.BarSeries { Title = "Inner Power Cells", StrokeColor = OxyColors.Black, StrokeThickness = 1 };
            s3.IsStacked = true;
            s3.FillColor = OxyColors.LightBlue;
            s3.LabelPlacement = LabelPlacement.Inside;
            s3.LabelFormatString = "{0} PC";
            s3.LabelMargin = 5;
            s3.TextColor = OxyColors.Black;
            s3.Items.Add(new BarItem { Value = 1 });
            s3.Items.Add(new BarItem { Value = 2 });
            s3.Items.Add(new BarItem { Value = 3 });
            s3.Items.Add(new BarItem { Value = 4 });
            s3.Items.Add(new BarItem { Value = 4 });
            s3.Items.Add(new BarItem { Value = 4 });
            s3.Items.Add(new BarItem { Value = 6 });
            s3.Items.Add(new BarItem { Value = 6 });
            s3.Items.Add(new BarItem { Value = 3 });
            s3.Items.Add(new BarItem { Value = 10 });

            var categoryAxis = new OxyPlot.Axes.CategoryAxis { Position = AxisPosition.Left, AxisTickToLabelDistance = 2 };
            categoryAxis.Labels.Add("862");
            categoryAxis.Labels.Add("10176");
            categoryAxis.Labels.Add("1023");
            categoryAxis.Labels.Add("254");
            categoryAxis.Labels.Add("254");
            categoryAxis.Labels.Add("254");
            categoryAxis.Labels.Add("254");
            categoryAxis.Labels.Add("254");
            categoryAxis.Labels.Add("254");
            categoryAxis.Labels.Add("254");
            GraphHeight = categoryAxis.Labels.Count * 50;
            var valueAxis = new OxyPlot.Axes.LinearAxis { Position = AxisPosition.Bottom, MinimumPadding = 0, MaximumPadding = 0.06, AbsoluteMinimum = 0 };
            DataPoints.Series.Add(s1);
            DataPoints.Series.Add(s2);
            DataPoints.Series.Add(s3);
            DataPoints.Axes.Add(categoryAxis);
            DataPoints.Axes.Add(valueAxis);*/
        }
        public void SetGraphType()
        {
            using(var db = new ScoutingContext())
            {
                DataPoints.Series.Clear();
                DataPoints.Axes.Clear();

                List<TeamMatch> dbMatches = db.Matches.Where(x => x.ClientSubmitted == true && x.EventCode == "test_env").ToList();
                List<int> selectedTeamNumbers = new List<int>();
                List<AveragePCCountModel> averagePCCountModels = new List<AveragePCCountModel>();

                foreach(var entry in dbMatches)
                {
                    AveragePCCountModel modelToAdd = new AveragePCCountModel();
                    if (!selectedTeamNumbers.Contains(entry.TeamNumber))
                    {
                        modelToAdd.TeamNumber = entry.TeamNumber;
                        selectedTeamNumbers.Add(entry.TeamNumber);
                        var inner = 0;
                        var outer = 0;
                        var lower = 0;
                        var total = 0;
                        foreach(var toadd in entry.PowerCellInner)
                        {
                            inner += toadd;
                            total += toadd;
                        }
                        foreach (var toadd in entry.PowerCellOuter)
                        {
                            outer += toadd;
                            total += toadd;
                        }
                        foreach (var toadd in entry.PowerCellLower)
                        {
                            lower += toadd;
                            total += toadd;
                        }
                        modelToAdd.AvgInnerPC = inner;
                        modelToAdd.AvgOuterPC = outer;
                        modelToAdd.AvgTotalPC = total;
                        modelToAdd.AvgLowerPC = lower;
                        modelToAdd.Matches++;
                        averagePCCountModels.Add(modelToAdd);
                    }
                    else
                    {
                        modelToAdd = averagePCCountModels.Where(x => x.TeamNumber == entry.TeamNumber).FirstOrDefault();
                        modelToAdd.TeamNumber = entry.TeamNumber;
                        var inner = modelToAdd.AvgInnerPC * modelToAdd.Matches;
                        var outer = modelToAdd.AvgOuterPC * modelToAdd.Matches;
                        var lower = modelToAdd.AvgLowerPC * modelToAdd.Matches;
                        var total = modelToAdd.AvgTotalPC * modelToAdd.Matches;
                        foreach (var toadd in entry.PowerCellInner)
                        {
                            inner += toadd;
                            total += toadd;
                        }
                        foreach (var toadd in entry.PowerCellOuter)
                        {
                            outer += toadd;
                            total += toadd;
                        }
                        foreach (var toadd in entry.PowerCellLower)
                        {
                            lower += toadd;
                            total += toadd;
                        }
                        modelToAdd.Matches++;
                        modelToAdd.AvgInnerPC = (int)Math.Ceiling((float)inner / (float)modelToAdd.Matches);
                        modelToAdd.AvgOuterPC = (int)Math.Ceiling((float)outer / (float)modelToAdd.Matches);
                        modelToAdd.AvgTotalPC = (int)Math.Ceiling((float)total / (float)modelToAdd.Matches);
                        modelToAdd.AvgLowerPC = (int)Math.Ceiling((float)lower / (float)modelToAdd.Matches);
                        averagePCCountModels.Remove(averagePCCountModels.Where(x => x.TeamNumber == entry.TeamNumber).FirstOrDefault());
                        averagePCCountModels.Add(modelToAdd);
                    }
                    
                }

                var s1 = new OxyPlot.Series.BarSeries { Title = "Lower Power Cells", StrokeColor = OxyColors.Black, StrokeThickness = 1 };
                var s2 = new OxyPlot.Series.BarSeries { Title = "Outer Power Cells", StrokeColor = OxyColors.Black, StrokeThickness = 1 };
                var s3 = new OxyPlot.Series.BarSeries { Title = "Inner Power Cells", StrokeColor = OxyColors.Black, StrokeThickness = 1 };
                var categoryAxis = new OxyPlot.Axes.CategoryAxis { Position = AxisPosition.Left, AxisTickToLabelDistance = 2 };
                s1.IsStacked = true;
                s1.LabelPlacement = LabelPlacement.Inside;
                s1.FillColor = OxyColors.LightYellow;
                s1.LabelMargin = 5;
                s1.TextColor = OxyColors.Black;
                s1.LabelFormatString = "{0} PC";
                s2.LabelPlacement = LabelPlacement.Inside;
                s2.IsStacked = true;
                s2.LabelMargin = 5;
                s2.LabelFormatString = "{0} PC";
                s2.TextColor = OxyColors.White;
                s2.FillColor = OxyColors.LightSeaGreen;
                s3.IsStacked = true;
                s3.FillColor = OxyColors.LightBlue;
                s3.LabelPlacement = LabelPlacement.Inside;
                s3.LabelFormatString = "{0} PC";
                s3.LabelMargin = 5;
                s3.TextColor = OxyColors.Black;
                var i = 0;
                foreach(var team in averagePCCountModels.OrderByDescending(x => x.AvgTotalPC).ThenByDescending(x => x.AvgInnerPC).ThenByDescending(x => x.AvgOuterPC).ThenByDescending(x => x.AvgOuterPC))
                {
                    
                    s1.Items.Add(new BarItem { Value = team.AvgLowerPC });
                    s2.Items.Add(new BarItem { Value = team.AvgOuterPC });
                    s3.Items.Add(new BarItem { Value = team.AvgInnerPC });
                    categoryAxis.Labels.Add(team.TeamNumber.ToString());
                    i++;
                }
                GraphHeight = (categoryAxis.Labels.Count * 50)+100;
                var valueAxis = new OxyPlot.Axes.LinearAxis { Position = AxisPosition.Bottom, MinimumPadding = 0, MaximumPadding = 0.06, AbsoluteMinimum = 0 };
                DataPoints.Series.Add(s1);
                DataPoints.Series.Add(s2);
                DataPoints.Series.Add(s3);
                DataPoints.Axes.Add(categoryAxis);
                DataPoints.Axes.Add(valueAxis);
            }
            
        }
    }
    public class Item : BarItem
    {
        public string Label { get; set; }
        public double Value1 { get; set; }
        public double Value2 { get; set; }
        public double Value3 { get; set; }
    }
    public class MatchViewModel : ViewModelBase
    {
        
        private string testText = "abc";
        private bool red1MatchNotFilled = false;
        private bool red2MatchNotFilled = false;
        private bool red3MatchNotFilled = false;
        private bool blue1MatchNotFilled = false;
        private bool blue2MatchNotFilled = false;
        private bool blue3MatchNotFilled = false;
        private bool red1MatchEditable = false;
        private bool red2MatchEditable = false;
        private bool red3MatchEditable = false;
        private bool blue1MatchEditable = false;
        private bool blue2MatchEditable = false;
        private bool blue3MatchEditable = false;
        private ObservableCollection<LogEntry> logEntries = new ObservableCollection<LogEntry>();
        public TeamMatch originalR1 = new TeamMatch();
        public TeamMatch originalR2 = new TeamMatch();
        public TeamMatch originalR3 = new TeamMatch();
        public TeamMatch originalB1 = new TeamMatch();
        public TeamMatch originalB2 = new TeamMatch();
        public TeamMatch originalB3 = new TeamMatch();
        private TeamMatchView red1CurrentMatch = new TeamMatchView() { ScoutName = "No One" };
        private TeamMatchView red2CurrentMatch = new TeamMatchView() { ScoutName = "No One" };
        private TeamMatchView red3CurrentMatch = new TeamMatchView() { ScoutName = "No One" };
        private TeamMatchView blue1CurrentMatch = new TeamMatchView() { ScoutName = "No One" };
        private TeamMatchView blue2CurrentMatch = new TeamMatchView() { ScoutName = "No One" };
        private TeamMatchView blue3CurrentMatch = new TeamMatchView() { ScoutName = "No One" };
        private bool userControlVisible = false;
        public bool UserControlVisible
        {
            get => userControlVisible;
            set => SetProperty(ref userControlVisible, value);
        }
        public ObservableCollection<LogEntry> CurrentLogEntries
        {
            get => logEntries;
            set => SetProperty(ref logEntries, value);
        }
        public string TestText
        {
            get => testText;
            set => SetProperty(ref testText, value);
        }
        public TeamMatchView Red1CurrentMatch
        {
            get => red1CurrentMatch;
            set => SetProperty(ref red1CurrentMatch, value);
        }
        public TeamMatchView Red2CurrentMatch
        {
            get => red2CurrentMatch;
            set => SetProperty(ref red2CurrentMatch, value);
        }
        public TeamMatchView Red3CurrentMatch
        {
            get => red3CurrentMatch;
            set => SetProperty(ref red3CurrentMatch, value);
        }
        public TeamMatchView Blue1CurrentMatch
        {
            get => blue1CurrentMatch;
            set => SetProperty(ref blue1CurrentMatch, value);
        }
        public TeamMatchView Blue2CurrentMatch
        {
            get => blue2CurrentMatch;
            set => SetProperty(ref blue2CurrentMatch, value);
        }
        public TeamMatchView Blue3CurrentMatch
        {
            get => blue3CurrentMatch;
            set => SetProperty(ref blue3CurrentMatch, value);
        }
        public bool Red1MatchNotFilled
        {
            get => red1MatchNotFilled;
            set => SetProperty(ref red1MatchNotFilled, value);
        }
        public bool Red2MatchNotFilled
        {
            get => red2MatchNotFilled;
            set => SetProperty(ref red2MatchNotFilled, value);
        }
        public bool Red3MatchNotFilled
        {
            get => red3MatchNotFilled;
            set => SetProperty(ref red3MatchNotFilled, value);
        }
        public bool Blue1MatchNotFilled
        {
            get => blue1MatchNotFilled;
            set => SetProperty(ref blue1MatchNotFilled, value);
        }
        public bool Blue2MatchNotFilled
        {
            get => blue2MatchNotFilled;
            set => SetProperty(ref blue2MatchNotFilled, value);
        }
        public bool Blue3MatchNotFilled
        {
            get => blue3MatchNotFilled;
            set => SetProperty(ref blue3MatchNotFilled, value);
        }
        public bool Red1MatchEditable
        {
            get => red1MatchEditable;
            set => SetProperty(ref red1MatchEditable, value);
        }
        public bool Red2MatchEditable
        {
            get => red2MatchEditable;
            set => SetProperty(ref red2MatchEditable, value);
        }
        public bool Red3MatchEditable
        {
            get => red3MatchEditable;
            set => SetProperty(ref red3MatchEditable, value);
        }
        public bool Blue1MatchEditable
        {
            get => blue1MatchEditable;
            set => SetProperty(ref blue1MatchEditable, value);
        }
        public bool Blue2MatchEditable
        {
            get => blue2MatchEditable;
            set => SetProperty(ref blue2MatchEditable, value);
        }
        public bool Blue3MatchEditable
        {
            get => blue3MatchEditable;
            set => SetProperty(ref blue3MatchEditable, value);
        }
        public void GetLogsForCurrentEntry(string identifier)
        {
            TeamMatch selectedForLogs = new TeamMatch();
            switch (identifier)
            {
                case "R1":
                    selectedForLogs = originalR1;
                    break;
                case "R2":
                    selectedForLogs = originalR2;
                    break;
                case "R3":
                    selectedForLogs = originalR3;
                    break;
                case "B1":
                    selectedForLogs = originalB1;
                    break;
                case "B2":
                    selectedForLogs = originalB2;
                    break;
                case "B3":
                    selectedForLogs = originalB3;
                    break;
            }
            try
            {
                List<string> logsToParse = selectedForLogs.TapLogs.ToList();
                List<LogEntry> entriesToDisplay = new List<LogEntry>();
                foreach (var logToParse in logsToParse)
                {

                    int actionTimestamp = int.Parse(logToParse.Split(":")[0]);
                    int minutes = (int)Math.Floor((double)actionTimestamp / (double)60);
                    int seconds = actionTimestamp % 60;
                    string secondsview = "";
                    if (seconds < 10)
                    {
                        secondsview = "0" + seconds.ToString();
                    }
                    else
                    {
                        secondsview = seconds.ToString();
                    }
                    LogEntry newLogEntry = new LogEntry();
                    newLogEntry.TimeFormatted = minutes.ToString() + ":" + secondsview;
                    int actionIdentifier = int.Parse(logToParse.Split(":")[1]);
                    switch (actionIdentifier)
                    {
                        case 1:
                            newLogEntry.Description = "Scouting Entry Started";
                            newLogEntry.Background = "Aqua";
                            break;
                        case 2:
                            newLogEntry.Description = "Scouting Entry Finished";
                            newLogEntry.Background = "Aqua";
                            break;
                        case 9:
                            //TBI
                            newLogEntry.Description = "Scouting Entry Populated from DB, Already Completed";
                            newLogEntry.Background = "Pink";
                            break;
                        case 10:
                            newLogEntry.Description = "Ignore Time Notice";
                            newLogEntry.Background = "Pink";
                            break;
                        case 100:
                            newLogEntry.Description = "Scout Name Changed to " + logToParse.Split(":")[2];
                            newLogEntry.Background = "Purple";
                            break;
                        case 1001:
                            newLogEntry.Description = "Page Changed to 'Ready for Match'";
                            newLogEntry.Background = "Gray";
                            break;
                        case 1002:
                            newLogEntry.Description = "Page Changed to 'Autonomous'";
                            newLogEntry.Background = "Gray";
                            break;
                        case 1003:
                            newLogEntry.Description = "Page Changed to 'Tele-Op'";
                            newLogEntry.Background = "Gray";
                            break;
                        case 1004:
                            newLogEntry.Description = "Page Changed to 'Endgame'";
                            newLogEntry.Background = "Gray";
                            break;
                        case 1005:
                            newLogEntry.Description = "Page Changed to 'Confirm Form'";
                            newLogEntry.Background = "Gray";
                            break;
                        case 2000:
                            newLogEntry.Description = "Initiation Line Marked as ACHIEVED";
                            newLogEntry.Background = "Gray";
                            break;
                        case 2001:
                            newLogEntry.Description = "Initiation Line Marked as NOT ACHIEVED";
                            newLogEntry.Background = "Gray";
                            break;
                        case 3000:
                            newLogEntry.Description = "Created New Tele-Op Cycle";
                            newLogEntry.Background = "Gray";
                            break;
                        case 3010:
                            newLogEntry.Description = "Control Panel Rotation Marked as ACHIEVED";
                            newLogEntry.Background = "Gray";
                            break;
                        case 3011:
                            newLogEntry.Description = "Control Panel Rotation Marked as NOT ACHIEVED";
                            newLogEntry.Background = "Gray";
                            break;
                        case 3020:
                            newLogEntry.Description = "Control Panel Position Marked as ACHIEVED";
                            newLogEntry.Background = "Gray";
                            break;
                        case 3021:
                            newLogEntry.Description = "Control Panel Position Marked as NOT ACHIEVED";
                            newLogEntry.Background = "Gray";
                            break;
                        case 4000:
                            newLogEntry.Description = "Endgame Parking Marked as PARKED";
                            newLogEntry.Background = "Gray";
                            break;
                        case 4001:
                            newLogEntry.Description = "Endgame Parking Marked as NOT PARKED";
                            newLogEntry.Background = "Gray";
                            break;
                        case 4010:
                            newLogEntry.Description = "Endgame Attempted Marked as ATTEMPTED";
                            newLogEntry.Background = "Gray";
                            break;
                        case 4011:
                            newLogEntry.Description = "Endgame Attempted Marked as NOT ATTEMPTED";
                            newLogEntry.Background = "Gray";
                            break;
                        case 4020:
                            newLogEntry.Description = "Endgame Successful Marked as SUCCESSFUL";
                            newLogEntry.Background = "Gray";
                            break;
                        case 4021:
                            newLogEntry.Description = "Endgame Successful Marked as NOT SUCCESSFUL";
                            newLogEntry.Background = "Gray";
                            break;
                        case 4030:
                            newLogEntry.Description = "Endgame Balanced Marked as CONTRIBUTED";
                            newLogEntry.Background = "Gray";
                            break;
                        case 4031:
                            newLogEntry.Description = "Endgame Balanced Marked as NOT CONTRIBUTED";
                            newLogEntry.Background = "Gray";
                            break;
                        case 9000:
                            newLogEntry.Description = "Robot Marked as DISABLED";
                            newLogEntry.Background = "Gray";
                            break;
                        case 9001:
                            newLogEntry.Description = "Robot Marked as NOT DISABLED";
                            newLogEntry.Background = "Gray";
                            break;
                        default:
                            newLogEntry.Description = "Unknown Log";
                            newLogEntry.Background = "Black";
                            break;
                    }
                    entriesToDisplay.Add(newLogEntry);
                }
                CurrentLogEntries = new ObservableCollection<LogEntry>(entriesToDisplay);
            }catch(Exception e)
            {

            }
            

        }
        public void SaveChanges()
        {
            try
            {
                using (var db = new ScoutingContext())
                {
                    TeamMatch newRed1Match = new TeamMatch() { PowerCellInner = new int[21], PowerCellLower = new int[21], PowerCellOuter = new int[21], PowerCellMissed = new int[21] };
                    TeamMatch newRed2Match = new TeamMatch() { PowerCellInner = new int[21], PowerCellLower = new int[21], PowerCellOuter = new int[21], PowerCellMissed = new int[21] };
                    TeamMatch newRed3Match = new TeamMatch() { PowerCellInner = new int[21], PowerCellLower = new int[21], PowerCellOuter = new int[21], PowerCellMissed = new int[21] };
                    TeamMatch newBlue1Match = new TeamMatch() { PowerCellInner = new int[21], PowerCellLower = new int[21], PowerCellOuter = new int[21], PowerCellMissed = new int[21] };
                    TeamMatch newBlue2Match = new TeamMatch() { PowerCellInner = new int[21], PowerCellLower = new int[21], PowerCellOuter = new int[21], PowerCellMissed = new int[21] };
                    TeamMatch newBlue3Match = new TeamMatch() { PowerCellInner = new int[21], PowerCellLower = new int[21], PowerCellOuter = new int[21], PowerCellMissed = new int[21] };
                    newRed1Match.A_InitiationLine = Red1CurrentMatch.A_InitiationLine;
                    newRed1Match.ScoutName = Red1CurrentMatch.ScoutName;
                    newRed1Match.DisabledSeconds = Red1CurrentMatch.DisabledSeconds;
                    newRed1Match.EventCode = originalR1.EventCode;
                    newRed1Match.E_Balanced = Red1CurrentMatch.E_Balanced;
                    newRed1Match.E_ClimbAttempt = Red1CurrentMatch.E_ClimbAttempt;
                    newRed1Match.E_ClimbSuccess = Red1CurrentMatch.E_ClimbSuccess;
                    newRed1Match.E_Park = Red1CurrentMatch.E_Park;
                    newRed1Match.MatchID = originalR1.MatchID;
                    newRed1Match.MatchNumber = originalR1.MatchNumber;
                    newRed1Match.NumCycles = Red1CurrentMatch.NumCycles;
                    if (originalR1.PowerCellMissed != null)
                    {
                        newRed1Match.PowerCellInner = originalR1.PowerCellInner;
                        newRed1Match.PowerCellOuter = originalR1.PowerCellOuter;
                        newRed1Match.PowerCellLower = originalR1.PowerCellLower;
                        newRed1Match.PowerCellMissed = originalR1.PowerCellMissed;
                    }
                    newRed1Match.PowerCellInner[0] = Red1CurrentMatch.APowerCellInner;
                    newRed1Match.PowerCellOuter[0] = Red1CurrentMatch.APowerCellOuter;
                    newRed1Match.PowerCellLower[0] = Red1CurrentMatch.APowerCellLower;
                    newRed1Match.PowerCellMissed[0] = Red1CurrentMatch.APowerCellMissed;
                    newRed1Match.RobotPosition = originalR1.RobotPosition;
                    newRed1Match.TabletId = originalR1.TabletId;
                    newRed1Match.TeamNumber = Red1CurrentMatch.TeamNumber;
                    newRed1Match.TeamName = originalR1.TeamName;
                    newRed1Match.T_ControlPanelPosition = Red1CurrentMatch.T_ControlPanelPosition;
                    newRed1Match.T_ControlPanelRotation = Red1CurrentMatch.T_ControlPanelRotation;
                    var previousRed1 = db.Matches.Where(x => x.TabletId == newRed1Match.TabletId && x.MatchNumber == newRed1Match.MatchNumber && x.EventCode == newRed1Match.EventCode).FirstOrDefault();
                    if (previousRed1 != null)
                    {
                        db.Entry(previousRed1).CurrentValues.SetValues(newRed1Match);
                    }
                    newRed2Match.A_InitiationLine = Red2CurrentMatch.A_InitiationLine;
                    newRed2Match.ScoutName = Red2CurrentMatch.ScoutName;
                    newRed2Match.DisabledSeconds = Red2CurrentMatch.DisabledSeconds;
                    newRed2Match.EventCode = originalR2.EventCode;
                    newRed2Match.E_Balanced = Red2CurrentMatch.E_Balanced;
                    newRed2Match.E_ClimbAttempt = Red2CurrentMatch.E_ClimbAttempt;
                    newRed2Match.E_ClimbSuccess = Red2CurrentMatch.E_ClimbSuccess;
                    newRed2Match.E_Park = Red2CurrentMatch.E_Park;
                    newRed2Match.MatchID = originalR2.MatchID;
                    newRed2Match.MatchNumber = originalR2.MatchNumber;
                    newRed2Match.NumCycles = Red2CurrentMatch.NumCycles;
                    if (originalR2.PowerCellMissed != null)
                    {
                        newRed2Match.PowerCellInner = originalR2.PowerCellInner;
                        newRed2Match.PowerCellOuter = originalR2.PowerCellOuter;
                        newRed2Match.PowerCellLower = originalR2.PowerCellLower;
                        newRed2Match.PowerCellMissed = originalR2.PowerCellMissed;
                    }
                    newRed2Match.PowerCellInner[0] = Red2CurrentMatch.APowerCellInner;
                    newRed2Match.PowerCellOuter[0] = Red2CurrentMatch.APowerCellOuter;
                    newRed2Match.PowerCellLower[0] = Red2CurrentMatch.APowerCellLower;
                    newRed2Match.PowerCellMissed[0] = Red2CurrentMatch.APowerCellMissed;
                    newRed2Match.RobotPosition = originalR2.RobotPosition;
                    newRed2Match.TabletId = originalR2.TabletId;
                    newRed2Match.TeamNumber = Red2CurrentMatch.TeamNumber;
                    newRed2Match.TeamName = originalR2.TeamName;
                    newRed2Match.T_ControlPanelPosition = Red2CurrentMatch.T_ControlPanelPosition;
                    newRed2Match.T_ControlPanelRotation = Red2CurrentMatch.T_ControlPanelRotation;
                    var previousRed2 = db.Matches.Where(x => x.TabletId == newRed2Match.TabletId && x.MatchNumber == newRed2Match.MatchNumber && x.EventCode == newRed2Match.EventCode).FirstOrDefault();
                    if (previousRed2 != null)
                    {
                        db.Entry(previousRed2).CurrentValues.SetValues(newRed2Match);
                    }
                    newRed3Match.A_InitiationLine = Red3CurrentMatch.A_InitiationLine;
                    newRed3Match.ScoutName = Red3CurrentMatch.ScoutName;
                    newRed3Match.DisabledSeconds = Red3CurrentMatch.DisabledSeconds;
                    newRed3Match.EventCode = originalR3.EventCode;
                    newRed3Match.E_Balanced = Red3CurrentMatch.E_Balanced;
                    newRed3Match.E_ClimbAttempt = Red3CurrentMatch.E_ClimbAttempt;
                    newRed3Match.E_ClimbSuccess = Red3CurrentMatch.E_ClimbSuccess;
                    newRed3Match.E_Park = Red3CurrentMatch.E_Park;
                    newRed3Match.MatchID = originalR3.MatchID;
                    newRed3Match.MatchNumber = originalR3.MatchNumber;
                    newRed3Match.NumCycles = Red3CurrentMatch.NumCycles;
                    if (originalR3.PowerCellMissed != null)
                    {
                        newRed3Match.PowerCellInner = originalR3.PowerCellInner;
                        newRed3Match.PowerCellOuter = originalR3.PowerCellOuter;
                        newRed3Match.PowerCellLower = originalR3.PowerCellLower;
                        newRed3Match.PowerCellMissed = originalR3.PowerCellMissed;
                    }
                    newRed3Match.PowerCellInner[0] = Red3CurrentMatch.APowerCellInner;
                    newRed3Match.PowerCellOuter[0] = Red3CurrentMatch.APowerCellOuter;
                    newRed3Match.PowerCellLower[0] = Red3CurrentMatch.APowerCellLower;
                    newRed3Match.PowerCellMissed[0] = Red3CurrentMatch.APowerCellMissed;
                    newRed3Match.RobotPosition = originalR3.RobotPosition;
                    newRed3Match.TabletId = originalR3.TabletId;
                    newRed3Match.TeamNumber = Red3CurrentMatch.TeamNumber;
                    newRed3Match.TeamName = originalR3.TeamName;
                    newRed3Match.T_ControlPanelPosition = Red3CurrentMatch.T_ControlPanelPosition;
                    newRed3Match.T_ControlPanelRotation = Red3CurrentMatch.T_ControlPanelRotation;
                    var previousRed3 = db.Matches.Where(x => x.TabletId == newRed3Match.TabletId && x.MatchNumber == newRed3Match.MatchNumber && x.EventCode == newRed3Match.EventCode).FirstOrDefault();
                    if (previousRed3 != null)
                    {
                        db.Entry(previousRed3).CurrentValues.SetValues(newRed3Match);
                    }
                    newBlue1Match.A_InitiationLine = Blue1CurrentMatch.A_InitiationLine;
                    newBlue1Match.ScoutName = Blue1CurrentMatch.ScoutName;
                    newBlue1Match.DisabledSeconds = Blue1CurrentMatch.DisabledSeconds;
                    newBlue1Match.EventCode = originalB1.EventCode;
                    newBlue1Match.E_Balanced = Blue1CurrentMatch.E_Balanced;
                    newBlue1Match.E_ClimbAttempt = Blue1CurrentMatch.E_ClimbAttempt;
                    newBlue1Match.E_ClimbSuccess = Blue1CurrentMatch.E_ClimbSuccess;
                    newBlue1Match.E_Park = Blue1CurrentMatch.E_Park;
                    newBlue1Match.MatchID = originalB1.MatchID;
                    newBlue1Match.MatchNumber = originalB1.MatchNumber;
                    newBlue1Match.NumCycles = Blue1CurrentMatch.NumCycles;
                    if (originalB1.PowerCellMissed != null)
                    {
                        newBlue1Match.PowerCellInner = originalB1.PowerCellInner;
                        newBlue1Match.PowerCellOuter = originalB1.PowerCellOuter;
                        newBlue1Match.PowerCellLower = originalB1.PowerCellLower;
                        newBlue1Match.PowerCellMissed = originalB1.PowerCellMissed;
                    }
                    newBlue1Match.PowerCellInner[0] = Blue1CurrentMatch.APowerCellInner;
                    newBlue1Match.PowerCellOuter[0] = Blue1CurrentMatch.APowerCellOuter;
                    newBlue1Match.PowerCellLower[0] = Blue1CurrentMatch.APowerCellLower;
                    newBlue1Match.PowerCellMissed[0] = Blue1CurrentMatch.APowerCellMissed;
                    newBlue1Match.RobotPosition = originalB1.RobotPosition;
                    newBlue1Match.TabletId = originalB1.TabletId;
                    newBlue1Match.TeamNumber = Blue1CurrentMatch.TeamNumber;
                    newBlue1Match.TeamName = originalB1.TeamName;
                    newBlue1Match.T_ControlPanelPosition = Blue1CurrentMatch.T_ControlPanelPosition;
                    newBlue1Match.T_ControlPanelRotation = Blue1CurrentMatch.T_ControlPanelRotation;
                    var previousBlue1 = db.Matches.Where(x => x.TabletId == newBlue1Match.TabletId && x.MatchNumber == newBlue1Match.MatchNumber && x.EventCode == newBlue1Match.EventCode).FirstOrDefault();
                    if (previousBlue1 != null)
                    {
                        db.Entry(previousBlue1).CurrentValues.SetValues(newBlue1Match);
                    }
                    newBlue2Match.A_InitiationLine = Blue2CurrentMatch.A_InitiationLine;
                    newBlue2Match.ScoutName = Blue2CurrentMatch.ScoutName;
                    newBlue2Match.DisabledSeconds = Blue2CurrentMatch.DisabledSeconds;
                    newBlue2Match.EventCode = originalB2.EventCode;
                    newBlue2Match.E_Balanced = Blue2CurrentMatch.E_Balanced;
                    newBlue2Match.E_ClimbAttempt = Blue2CurrentMatch.E_ClimbAttempt;
                    newBlue2Match.E_ClimbSuccess = Blue2CurrentMatch.E_ClimbSuccess;
                    newBlue2Match.E_Park = Blue2CurrentMatch.E_Park;
                    newBlue2Match.MatchID = originalB2.MatchID;
                    newBlue2Match.MatchNumber = originalB2.MatchNumber;
                    newBlue2Match.NumCycles = Blue2CurrentMatch.NumCycles;
                    if (originalB2.PowerCellMissed != null)
                    {
                        newBlue2Match.PowerCellInner = originalB2.PowerCellInner;
                        newBlue2Match.PowerCellOuter = originalB2.PowerCellOuter;
                        newBlue2Match.PowerCellLower = originalB2.PowerCellLower;
                        newBlue2Match.PowerCellMissed = originalB2.PowerCellMissed;
                    }
                    newBlue2Match.PowerCellInner[0] = Blue2CurrentMatch.APowerCellInner;
                    newBlue2Match.PowerCellOuter[0] = Blue2CurrentMatch.APowerCellOuter;
                    newBlue2Match.PowerCellLower[0] = Blue2CurrentMatch.APowerCellLower;
                    newBlue2Match.PowerCellMissed[0] = Blue2CurrentMatch.APowerCellMissed;
                    newBlue2Match.RobotPosition = originalB2.RobotPosition;
                    newBlue2Match.TabletId = originalB2.TabletId;
                    newBlue2Match.TeamNumber = Blue2CurrentMatch.TeamNumber;
                    newBlue2Match.TeamName = originalB2.TeamName;
                    newBlue2Match.T_ControlPanelPosition = Blue2CurrentMatch.T_ControlPanelPosition;
                    newBlue2Match.T_ControlPanelRotation = Blue2CurrentMatch.T_ControlPanelRotation;
                    var previousBlue2 = db.Matches.Where(x => x.TabletId == newBlue2Match.TabletId && x.MatchNumber == newBlue2Match.MatchNumber && x.EventCode == newBlue2Match.EventCode).FirstOrDefault();
                    if (previousBlue2 != null)
                    {
                        db.Entry(previousBlue2).CurrentValues.SetValues(newBlue2Match);
                    }
                    newBlue3Match.A_InitiationLine = Blue3CurrentMatch.A_InitiationLine;
                    newBlue3Match.ScoutName = Blue3CurrentMatch.ScoutName;
                    newBlue3Match.DisabledSeconds = Blue3CurrentMatch.DisabledSeconds;
                    newBlue3Match.EventCode = originalB3.EventCode;
                    newBlue3Match.E_Balanced = Blue3CurrentMatch.E_Balanced;
                    newBlue3Match.E_ClimbAttempt = Blue3CurrentMatch.E_ClimbAttempt;
                    newBlue3Match.E_ClimbSuccess = Blue3CurrentMatch.E_ClimbSuccess;
                    newBlue3Match.E_Park = Blue3CurrentMatch.E_Park;
                    newBlue3Match.MatchID = originalB3.MatchID;
                    newBlue3Match.MatchNumber = originalB3.MatchNumber;
                    newBlue3Match.NumCycles = Blue3CurrentMatch.NumCycles;
                    if (originalB3.PowerCellMissed != null)
                    {
                        newBlue3Match.PowerCellInner = originalB3.PowerCellInner;
                        newBlue3Match.PowerCellOuter = originalB3.PowerCellOuter;
                        newBlue3Match.PowerCellLower = originalB3.PowerCellLower;
                        newBlue3Match.PowerCellMissed = originalB3.PowerCellMissed;
                    }
                    newBlue3Match.PowerCellInner[0] = Blue3CurrentMatch.APowerCellInner;
                    newBlue3Match.PowerCellOuter[0] = Blue3CurrentMatch.APowerCellOuter;
                    newBlue3Match.PowerCellLower[0] = Blue3CurrentMatch.APowerCellLower;
                    newBlue3Match.PowerCellMissed[0] = Blue3CurrentMatch.APowerCellMissed;
                    newBlue3Match.RobotPosition = originalB3.RobotPosition;
                    newBlue3Match.TabletId = originalB3.TabletId;
                    newBlue3Match.TeamNumber = Blue3CurrentMatch.TeamNumber;
                    newBlue3Match.TeamName = originalB3.TeamName;
                    newBlue3Match.T_ControlPanelPosition = Blue3CurrentMatch.T_ControlPanelPosition;
                    newBlue3Match.T_ControlPanelRotation = Blue3CurrentMatch.T_ControlPanelRotation;
                    var previousBlue3 = db.Matches.Where(x => x.TabletId == newBlue3Match.TabletId && x.MatchNumber == newBlue3Match.MatchNumber && x.EventCode == newBlue3Match.EventCode).FirstOrDefault();
                    if (previousBlue3 != null)
                    {
                        db.Entry(previousBlue3).CurrentValues.SetValues(newBlue3Match);
                    }
                    /*try
                    {
                        var previousitem = db.Matches.Where(x => x.TabletId == itemtouse.TabletId && x.MatchNumber == itemtouse.MatchNumber && x.EventCode == itemtouse.EventCode).FirstOrDefault();
                        if (previousitem == null)
                        {
                            itemtouse.MatchID = new Random().Next(1, 1000);
                            db.Matches.Add(itemtouse);
                        }
                        else
                        {
                            itemtouse.MatchID = previousitem.MatchID;
                            db.Entry(previousitem).CurrentValues.SetValues(itemtouse);
                        }


                    }
                    catch (NpgsqlException ex)
                    {
                        itemtouse.MatchID = new Random().Next(1, 1000);
                        db.Matches.Add(itemtouse);
                    }*/

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
    public class TabletViewModel : ViewModelBase
    {
        private List<string> testBTD = new List<string>();
        private string _text = "Test";
        private ObservableCollection<string> bluetoothBorderColors = new ObservableCollection<string>(new string[6] { "Gray", "Gray", "Gray", "Gray", "Gray", "Gray" }.ToList());
        private ObservableCollection<string> cableBorderColors = new ObservableCollection<string>(new string[6] { "Gray", "Gray", "Gray", "Gray", "Gray", "Gray" }.ToList());
        private ObservableCollection<string> batteryBorderColors = new ObservableCollection<string>(new string[6] { "Gray", "Gray", "Gray", "Gray", "Gray", "Gray" }.ToList());
        private ObservableCollection<string> bluetoothBackgroundColors = new ObservableCollection<string>(new string[6] { "LightGray", "LightGray", "LightGray", "LightGray", "LightGray", "LightGray" }.ToList());
        private ObservableCollection<string> cableBackgroundColors = new ObservableCollection<string>(new string[6] { "LightGray", "LightGray", "LightGray", "LightGray", "LightGray", "LightGray" }.ToList());
        private ObservableCollection<string> batteryBackgroundColors = new ObservableCollection<string>(new string[6] { "LightGray", "LightGray", "LightGray", "LightGray", "LightGray", "LightGray" }.ToList());
        private ObservableCollection<string> batteryAmounts = new ObservableCollection<string>(new string[6] { "Device Not Initialized", "Device Not Initialized", "Device Not Initialized", "Device Not Initialized", "Device Not Initialized", "Device Not Initialized" }.ToList());
        private ObservableCollection<string> lastPings = new ObservableCollection<string>(new string[6] { "Device Not Initialized", "Device Not Initialized", "Device Not Initialized", "Device Not Initialized", "Device Not Initialized", "Device Not Initialized" }.ToList());
        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }
        public List<string> TestBTD
        {
            get => testBTD;
            set => SetProperty(ref testBTD, value);
        }
        private bool userControlVisible = false;
        public bool UserControlVisible
        {
            get => userControlVisible;
            set => SetProperty(ref userControlVisible, value);
        }
        public ObservableCollection<string> BluetoothBorderColors
        {
            get => bluetoothBorderColors;
            set => SetProperty(ref bluetoothBorderColors, value);
        }
        public ObservableCollection<string> CableBorderColors
        {
            get => cableBorderColors;
            set => SetProperty(ref cableBorderColors, value);
        }
        public ObservableCollection<string> BatteryBorderColors
        {
            get => batteryBorderColors;
            set => SetProperty(ref batteryBorderColors, value);
        }
        public ObservableCollection<string> BluetoothBackgroundColors
        {
            get => bluetoothBackgroundColors;
            set => SetProperty(ref bluetoothBackgroundColors, value);
        }
        public ObservableCollection<string> CableBackgroundColors
        {
            get => cableBackgroundColors;
            set => SetProperty(ref cableBackgroundColors, value);
        }
        public ObservableCollection<string> BatteryBackgroundColors
        {
            get => batteryBackgroundColors;
            set => SetProperty(ref batteryBackgroundColors, value);
        }
        public ObservableCollection<string> BatteryAmounts
        {
            get => batteryAmounts;
            set => SetProperty(ref batteryAmounts, value);
        }
        public ObservableCollection<string> LastPings
        {
            get => lastPings;
            set => SetProperty(ref lastPings, value);
        }
        public void SetTest()
        {
            bluetoothBackgroundColors[0] = "Red";
            Text = "WORK";
        }

    }
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            ProcessTest();
            
        }
        private int currentMatchNum = 1;
        private string matchNumString = "Match 1";
        private GraphViewModel graphViewModel = new GraphViewModel();
        private TabletViewModel tabletViewModel = new TabletViewModel();
        private NotificationViewModel notificationViewModel = new NotificationViewModel();
        private MatchViewModel matchViewModel = new MatchViewModel();
        private string _text = "Initial text";
        private TBAViewModel tbaViewModel = new TBAViewModel();
        private bool userControlVisible = false;
        public WebsocketClient client = new WebsocketClient(new Uri("ws://localhost:8080"));
        public int testInt = 0;
        public int CurrentMatchNum
        {
            get => currentMatchNum;
            set => SetProperty(ref currentMatchNum, value);
        }
        public TabletViewModel TabletViewModel
        {
            get => tabletViewModel;
            set => SetProperty(ref tabletViewModel, value);
        }
        public NotificationViewModel NotificationViewModel
        {
            get => notificationViewModel;
            set => SetProperty(ref notificationViewModel, value);
        }
        public TBAViewModel TBAViewModel
        {
            get => tbaViewModel;
            set => SetProperty(ref tbaViewModel, value);
        }
        public GraphViewModel GraphViewModel
        {
            get => graphViewModel;
            set => SetProperty(ref graphViewModel, value);
        }
        public string MatchNumString
        {
            get => matchNumString;
            set => SetProperty(ref matchNumString, value);
        }
        public MatchViewModel MatchViewModel
        {
            get => matchViewModel;
            set => SetProperty(ref matchViewModel, value);
        }

        public string Text
        {
            get => _text;
            // SetProperty will trigger PropertyChanged event so the view is notified
            set => SetProperty(ref _text, value);
        }
        public bool UserControlVisible
        {
            get => userControlVisible;
            set => SetProperty(ref userControlVisible, value);
        }

        // Unlike WPF avalonia supports binding commands to view model methods. You can still use ICommand though
        public void ChangeText(object text)
        {
            Text = (string)text;
        }
        public void StartCheck()
        {
            testInt++;
            if (testInt < 2)
            {
                client.Start();
                Console.WriteLine(matchViewModel.Blue1CurrentMatch.TeamNumber.ToString());
                var exitEvent = new ManualResetEvent(false);
                var url = new Uri("ws://localhost:8080");

                client.ReconnectTimeout = null;
                /*client.ReconnectionHappened.Subscribe(info =>
                    Log.Information($"Reconnection happened, type: {info.Type}"));*/

                client.MessageReceived.Subscribe(msg =>
                {
                    string rawdata = msg.Text;
                    UseGivenData(rawdata);
                    
                });
                client.DisconnectionHappened.Subscribe(msg =>
                {
                    Console.WriteLine("Uh oh! I disconnected!");
                });
                
            }
            

            //Task.Run(() => client.Send("{ message }"));
        }
        public void UseGivenData(string rawdata)
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
            TabletViewModel.LastPings[tabletindex] = "Last ping at: " + DateTime.Now.ToShortTimeString();
            if (rawdata.Substring(3).StartsWith("S:"))
            {
                //S = Score
                NotificationViewModel.AddNotification("Scored", rawdata.Substring(0, 2).ToString() + " sent scores", "Green");
                TabletViewModel.BluetoothBackgroundColors[tabletindex] = "LightBlue";
                TabletViewModel.BluetoothBorderColors[tabletindex] = "Blue";
                var jsontodeserialize = rawdata.Substring(5);
                using (var db = new ScoutingContext())
                {
                    var itemstouse = JsonConvert.DeserializeObject<List<TeamMatch>>(jsontodeserialize);
                    foreach (var itemtouse in itemstouse)
                    {
                        try
                        {
                            var previousitem = db.Matches.Where(x => x.TabletId == itemtouse.TabletId && x.MatchNumber == itemtouse.MatchNumber && x.EventCode == itemtouse.EventCode).FirstOrDefault();
                            if (previousitem == null)
                            {
                                itemtouse.MatchID = new Random().Next(1, 1000);
                                db.Matches.Add(itemtouse);
                            }
                            else
                            {
                                if(previousitem.ClientLastSubmitted != itemtouse.ClientLastSubmitted)
                                {
                                    itemtouse.MatchID = previousitem.MatchID;
                                    db.Entry(previousitem).CurrentValues.SetValues(itemtouse);
                                }
                                
                            }


                        }
                        catch (NpgsqlException ex)
                        {
                            itemtouse.MatchID = new Random().Next(1, 1000);
                            db.Matches.Add(itemtouse);
                        }
                    }


                    db.SaveChanges();
                }
            }
            else if (rawdata.Substring(3).StartsWith("B:"))
            {
                //B = Battery Level
                var batterylevel = float.Parse(rawdata.Substring(5)) * 100;
                if (batterylevel > 80)
                {
                    TabletViewModel.BatteryBackgroundColors[tabletindex] = "LightGreen";
                    TabletViewModel.BatteryBorderColors[tabletindex] = "Green";
                }
                else if (batterylevel > 30 && batterylevel <= 80)
                {
                    TabletViewModel.BatteryBackgroundColors[tabletindex] = "LightSalmon";
                    TabletViewModel.BatteryBorderColors[tabletindex] = "DarkOrange";
                }
                else
                {
                    TabletViewModel.BatteryBackgroundColors[tabletindex] = "LightPink";
                    TabletViewModel.BatteryBorderColors[tabletindex] = "IndianRed";
                }
                TabletViewModel.BatteryAmounts[tabletindex] = Math.Round(batterylevel).ToString();

            }
            else if (rawdata.Substring(3).StartsWith("D:"))
            {
                //D = Successful Disconnection
                TabletViewModel.BluetoothBackgroundColors[tabletindex] = "LightGray";
                TabletViewModel.BluetoothBorderColors[tabletindex] = "Gray";
            }
            else if (rawdata.Substring(3).StartsWith("E:"))
            {
                //E = Immedient Communication
                TabletViewModel.BluetoothBackgroundColors[tabletindex] = "LightSalmon";
                TabletViewModel.BluetoothBorderColors[tabletindex] = "DarkOrange";
            }
            else if (rawdata.Substring(3).StartsWith("RD:"))
            {
                NotificationViewModel.AddNotification("Data Request", rawdata.Substring(0, 2).ToString() + " requested data", "Blue");
                var listOfMatchesToSend = new List<TeamMatch>()
                        {
                            new TeamMatch() { TeamNumber = 862, MatchNumber = 1, TabletId = rawdata.Substring(0,2).ToString(), PowerCellInner = new int[21], PowerCellOuter = new int[21], PowerCellLower = new int[21], PowerCellMissed = new int[21], EventCode = "test_env" },
                            new TeamMatch() { TeamNumber = 1023, MatchNumber = 2, TabletId = rawdata.Substring(0,2).ToString(), PowerCellInner = new int[21], PowerCellOuter = new int[21], PowerCellLower = new int[21], PowerCellMissed = new int[21], EventCode = "test_env" },
                            new TeamMatch() { TeamNumber = 2014, MatchNumber = 3, TabletId = rawdata.Substring(0,2).ToString(), PowerCellInner = new int[21], PowerCellOuter = new int[21], PowerCellLower = new int[21], PowerCellMissed = new int[21], EventCode = "test_env" },
                            new TeamMatch() { TeamNumber = 2020, MatchNumber = 4, TabletId = rawdata.Substring(0,2).ToString(), PowerCellInner = new int[21], PowerCellOuter = new int[21], PowerCellLower = new int[21], PowerCellMissed = new int[21], EventCode = "test_env" },
                            new TeamMatch() { TeamNumber = 3145, MatchNumber = 5, TabletId = rawdata.Substring(0,2).ToString(), PowerCellInner = new int[21], PowerCellOuter = new int[21], PowerCellLower = new int[21], PowerCellMissed = new int[21], EventCode = "test_env" },
                            new TeamMatch() { TeamNumber = 4005, MatchNumber = 6, TabletId = rawdata.Substring(0,2).ToString(), PowerCellInner = new int[21], PowerCellOuter = new int[21], PowerCellLower = new int[21], PowerCellMissed = new int[21], EventCode = "test_env" }
                        };
                string modelstring = JsonConvert.SerializeObject(listOfMatchesToSend);
                byte[] bytesToSend = Encoding.ASCII.GetBytes(modelstring);
                if (modelstring.Length > 480)
                {
                    int numberofmessages = (int)Math.Ceiling((float)modelstring.Length / (float)480);
                    var startidentifier = rawdata.Substring(0, 2).ToString() + ":MM:" + numberofmessages.ToString();
                    client.Send(startidentifier);
                    for (int i = numberofmessages; i > 0; i--)
                    {
                        var simplestring = rawdata.Substring(0, 2).ToString() + ":" + new string(modelstring.Skip((numberofmessages - i) * 480).Take(480).ToArray());
                        client.Send(simplestring);
                    }
                }
                else
                {
                    client.Send(modelstring);
                }
                TabletViewModel.BluetoothBackgroundColors[tabletindex] = "LightSalmon";
                TabletViewModel.BluetoothBorderColors[tabletindex] = "DarkOrange";
            }


            //Console.WriteLine(msg.Text);
        }
        public void GetBluetoothDevices()
        {
            BluetoothClient client = new BluetoothClient();
            List<string> items = new List<string>();
            BluetoothDeviceInfo[] devices = client.DiscoverDevices(255, true, true, true);
            foreach (BluetoothDeviceInfo d in devices)
            {
                items.Add(d.DeviceName);
                Console.WriteLine(d.DeviceName);
            }
            tabletViewModel.TestBTD = items;
        }
        public void ProcessTest()
        {
            Process startTCPforwarding = new Process();
            startTCPforwarding.StartInfo.WorkingDirectory = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "lightmaster"), "LightMasterHub");
            startTCPforwarding.StartInfo.FileName = "bash";
            string command = "npm start";
            startTCPforwarding.StartInfo.Arguments = "-c \"" + command + "\"";
            startTCPforwarding.StartInfo.UseShellExecute = false;
            startTCPforwarding.StartInfo.RedirectStandardOutput = true;
            startTCPforwarding.StartInfo.CreateNoWindow = true;
            startTCPforwarding.Start();
            startTCPforwarding.ErrorDataReceived += (s, e) =>
            {
                NotificationViewModel.AddNotification("Unavailable", "Bluetooth Service Not Available!", "Red");
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
                if(nummessagesreceived == 0)
                {
                    NotificationViewModel.AddNotification("Ready", "Bluetooth Service Ready!", "DeepPink");
                }
                string rawdata = msg.Text;
                UseGivenData(rawdata);
                nummessagesreceived++;
            });
            client.DisconnectionHappened.Subscribe(msg =>
            {
                Console.WriteLine("Uh oh! I disconnected!");
                nummessagesreceived = 0;
                NotificationViewModel.AddNotification("Not Ready", "Bluetooth Service Not Ready!", "Red");
            });
        }


        public void SeeMatches(int MatchNum)
        {
            tabletViewModel.UserControlVisible = false;
            matchViewModel.UserControlVisible = true;
            TBAViewModel.UserControlVisible = false;
            try
            {
                using (var db = new ScoutingContext())
                {
                    var r1selectedmatch = db.Matches.Where(x => x.TabletId == "R1" && x.EventCode == "test_env" && x.MatchNumber == MatchNum).FirstOrDefault();
                    if (r1selectedmatch == null)
                    {
                        MatchViewModel.Red1CurrentMatch = new TeamMatchView();
                        MatchViewModel.originalR1 = new TeamMatch();
                        MatchViewModel.Red1MatchNotFilled = true;
                        MatchViewModel.Red1MatchEditable = false;
                    }
                    else
                    {
                        MatchViewModel.Red1MatchEditable = true;
                        MatchViewModel.Red1MatchNotFilled = false;
                        MatchViewModel.originalR1 = r1selectedmatch;
                        var matchtoput = new TeamMatchView();
                        matchtoput.A_InitiationLine = r1selectedmatch.A_InitiationLine;
                        matchtoput.DisabledSeconds = r1selectedmatch.DisabledSeconds;
                        matchtoput.EventCode = r1selectedmatch.EventCode;
                        matchtoput.E_Balanced = r1selectedmatch.E_Balanced;
                        matchtoput.E_ClimbAttempt = r1selectedmatch.E_ClimbAttempt;
                        matchtoput.E_ClimbSuccess = r1selectedmatch.E_ClimbSuccess;
                        matchtoput.E_Park = r1selectedmatch.E_Park;
                        matchtoput.MatchNumber = r1selectedmatch.MatchNumber;
                        matchtoput.NumCycles = r1selectedmatch.NumCycles;
                        matchtoput.ScoutName = r1selectedmatch.ScoutName;
                        matchtoput.TeamNumber = r1selectedmatch.TeamNumber;
                        matchtoput.T_ControlPanelPosition = r1selectedmatch.T_ControlPanelPosition;
                        matchtoput.T_ControlPanelRotation = r1selectedmatch.T_ControlPanelRotation;
                        if (r1selectedmatch.PowerCellMissed != null)
                        {
                            matchtoput.APowerCellInner = r1selectedmatch.PowerCellInner[0];
                            matchtoput.APowerCellOuter = r1selectedmatch.PowerCellOuter[0];
                            matchtoput.APowerCellLower = r1selectedmatch.PowerCellLower[0];
                            matchtoput.APowerCellMissed = r1selectedmatch.PowerCellMissed[0];
                            foreach (var value in r1selectedmatch.PowerCellInner.Skip(1))
                            {
                                matchtoput.TPowerCellInner += value;
                            }
                            foreach (var value in r1selectedmatch.PowerCellOuter.Skip(1))
                            {
                                matchtoput.TPowerCellOuter += value;
                            }
                            foreach (var value in r1selectedmatch.PowerCellLower.Skip(1))
                            {
                                matchtoput.TPowerCellLower += value;
                            }
                            foreach (var value in r1selectedmatch.PowerCellMissed.Skip(1))
                            {
                                matchtoput.TPowerCellMissed += value;
                            }
                        }
                        MatchViewModel.Red1CurrentMatch = matchtoput;
                    }
                    //RED2
                    var r2selectedmatch = db.Matches.Where(x => x.TabletId == "R2" && x.EventCode == "test_env" && x.MatchNumber == MatchNum).FirstOrDefault();
                    if (r2selectedmatch == null)
                    {
                        MatchViewModel.Red2CurrentMatch = new TeamMatchView();
                        MatchViewModel.originalR2 = new TeamMatch();
                        MatchViewModel.Red2MatchNotFilled = true;
                        MatchViewModel.Red2MatchEditable = false;
                    }
                    else
                    {
                        MatchViewModel.Red2MatchEditable = true;
                        MatchViewModel.Red2MatchNotFilled = false;
                        MatchViewModel.originalR2 = r2selectedmatch;
                        var matchtoput = new TeamMatchView();
                        matchtoput.A_InitiationLine = r2selectedmatch.A_InitiationLine;
                        matchtoput.DisabledSeconds = r2selectedmatch.DisabledSeconds;
                        matchtoput.EventCode = r2selectedmatch.EventCode;
                        matchtoput.E_Balanced = r2selectedmatch.E_Balanced;
                        matchtoput.E_ClimbAttempt = r2selectedmatch.E_ClimbAttempt;
                        matchtoput.E_ClimbSuccess = r2selectedmatch.E_ClimbSuccess;
                        matchtoput.E_Park = r2selectedmatch.E_Park;
                        matchtoput.MatchNumber = r2selectedmatch.MatchNumber;
                        matchtoput.NumCycles = r2selectedmatch.NumCycles;
                        matchtoput.ScoutName = r2selectedmatch.ScoutName;
                        matchtoput.TeamNumber = r2selectedmatch.TeamNumber;
                        matchtoput.T_ControlPanelPosition = r2selectedmatch.T_ControlPanelPosition;
                        matchtoput.T_ControlPanelRotation = r2selectedmatch.T_ControlPanelRotation;
                        if (r2selectedmatch.PowerCellMissed != null)
                        {
                            matchtoput.APowerCellInner = r2selectedmatch.PowerCellInner[0];
                            matchtoput.APowerCellOuter = r2selectedmatch.PowerCellOuter[0];
                            matchtoput.APowerCellLower = r2selectedmatch.PowerCellLower[0];
                            matchtoput.APowerCellMissed = r2selectedmatch.PowerCellMissed[0];
                            foreach (var value in r2selectedmatch.PowerCellInner.Skip(1))
                            {
                                matchtoput.TPowerCellInner += value;
                            }
                            foreach (var value in r2selectedmatch.PowerCellOuter.Skip(1))
                            {
                                matchtoput.TPowerCellOuter += value;
                            }
                            foreach (var value in r2selectedmatch.PowerCellLower.Skip(1))
                            {
                                matchtoput.TPowerCellLower += value;
                            }
                            foreach (var value in r2selectedmatch.PowerCellMissed.Skip(1))
                            {
                                matchtoput.TPowerCellMissed += value;
                            }
                        }
                        MatchViewModel.Red2CurrentMatch = matchtoput;
                    }
                    //RED3
                    var r3selectedmatch = db.Matches.Where(x => x.TabletId == "R3" && x.EventCode == "test_env" && x.MatchNumber == MatchNum).FirstOrDefault();
                    if (r3selectedmatch == null)
                    {
                        MatchViewModel.Red3CurrentMatch = new TeamMatchView();
                        MatchViewModel.originalR3 = new TeamMatch();
                        MatchViewModel.Red3MatchNotFilled = true;
                        MatchViewModel.Red3MatchEditable = false;
                    }
                    else
                    {
                        MatchViewModel.Red3MatchNotFilled = false;
                        MatchViewModel.Red3MatchEditable = true;
                        MatchViewModel.originalR3 = r3selectedmatch;
                        var matchtoput = new TeamMatchView();
                        matchtoput.A_InitiationLine = r3selectedmatch.A_InitiationLine;
                        matchtoput.DisabledSeconds = r3selectedmatch.DisabledSeconds;
                        matchtoput.EventCode = r3selectedmatch.EventCode;
                        matchtoput.E_Balanced = r3selectedmatch.E_Balanced;
                        matchtoput.E_ClimbAttempt = r3selectedmatch.E_ClimbAttempt;
                        matchtoput.E_ClimbSuccess = r3selectedmatch.E_ClimbSuccess;
                        matchtoput.E_Park = r3selectedmatch.E_Park;
                        matchtoput.MatchNumber = r3selectedmatch.MatchNumber;
                        matchtoput.NumCycles = r3selectedmatch.NumCycles;
                        matchtoput.ScoutName = r3selectedmatch.ScoutName;
                        matchtoput.TeamNumber = r3selectedmatch.TeamNumber;
                        matchtoput.T_ControlPanelPosition = r3selectedmatch.T_ControlPanelPosition;
                        matchtoput.T_ControlPanelRotation = r3selectedmatch.T_ControlPanelRotation;
                        if (r3selectedmatch.PowerCellMissed != null)
                        {
                            matchtoput.APowerCellInner = r3selectedmatch.PowerCellInner[0];
                            matchtoput.APowerCellOuter = r3selectedmatch.PowerCellOuter[0];
                            matchtoput.APowerCellLower = r3selectedmatch.PowerCellLower[0];
                            matchtoput.APowerCellMissed = r3selectedmatch.PowerCellMissed[0];
                            foreach (var value in r3selectedmatch.PowerCellInner.Skip(1))
                            {
                                matchtoput.TPowerCellInner += value;
                            }
                            foreach (var value in r3selectedmatch.PowerCellOuter.Skip(1))
                            {
                                matchtoput.TPowerCellOuter += value;
                            }
                            foreach (var value in r3selectedmatch.PowerCellLower.Skip(1))
                            {
                                matchtoput.TPowerCellLower += value;
                            }
                            foreach (var value in r3selectedmatch.PowerCellMissed.Skip(1))
                            {
                                matchtoput.TPowerCellMissed += value;
                            }
                        }
                        MatchViewModel.Red3CurrentMatch = matchtoput;
                    }
                    //BLUE1
                    var b1selectedmatch = db.Matches.Where(x => x.TabletId == "B1" && x.EventCode == "test_env" && x.MatchNumber == MatchNum).FirstOrDefault();
                    if (b1selectedmatch == null)
                    {
                        MatchViewModel.Blue1CurrentMatch = new TeamMatchView();
                        MatchViewModel.originalB1 = new TeamMatch();
                        MatchViewModel.Blue1MatchNotFilled = true;
                        MatchViewModel.Blue1MatchEditable = false;
                    }
                    else
                    {
                        MatchViewModel.Blue1MatchNotFilled = false;
                        MatchViewModel.Blue1MatchEditable = true;
                        MatchViewModel.originalB1 = b1selectedmatch;
                        var matchtoput = new TeamMatchView();
                        matchtoput.A_InitiationLine = b1selectedmatch.A_InitiationLine;
                        matchtoput.DisabledSeconds = b1selectedmatch.DisabledSeconds;
                        matchtoput.EventCode = b1selectedmatch.EventCode;
                        matchtoput.E_Balanced = b1selectedmatch.E_Balanced;
                        matchtoput.E_ClimbAttempt = b1selectedmatch.E_ClimbAttempt;
                        matchtoput.E_ClimbSuccess = b1selectedmatch.E_ClimbSuccess;
                        matchtoput.E_Park = b1selectedmatch.E_Park;
                        matchtoput.MatchNumber = b1selectedmatch.MatchNumber;
                        matchtoput.NumCycles = b1selectedmatch.NumCycles;
                        matchtoput.ScoutName = b1selectedmatch.ScoutName;
                        matchtoput.TeamNumber = b1selectedmatch.TeamNumber;
                        matchtoput.T_ControlPanelPosition = b1selectedmatch.T_ControlPanelPosition;
                        matchtoput.T_ControlPanelRotation = b1selectedmatch.T_ControlPanelRotation;
                        if (b1selectedmatch.PowerCellMissed != null)
                        {
                            matchtoput.APowerCellInner = b1selectedmatch.PowerCellInner[0];
                            matchtoput.APowerCellOuter = b1selectedmatch.PowerCellOuter[0];
                            matchtoput.APowerCellLower = b1selectedmatch.PowerCellLower[0];
                            matchtoput.APowerCellMissed = b1selectedmatch.PowerCellMissed[0];
                            foreach (var value in b1selectedmatch.PowerCellInner.Skip(1))
                            {
                                matchtoput.TPowerCellInner += value;
                            }
                            foreach (var value in b1selectedmatch.PowerCellOuter.Skip(1))
                            {
                                matchtoput.TPowerCellOuter += value;
                            }
                            foreach (var value in b1selectedmatch.PowerCellLower.Skip(1))
                            {
                                matchtoput.TPowerCellLower += value;
                            }
                            foreach (var value in b1selectedmatch.PowerCellMissed.Skip(1))
                            {
                                matchtoput.TPowerCellMissed += value;
                            }
                        }
                        MatchViewModel.Blue1CurrentMatch = matchtoput;
                    }
                    //BLUE2
                    var b2selectedmatch = db.Matches.Where(x => x.TabletId == "B2" && x.EventCode == "test_env" && x.MatchNumber == MatchNum).FirstOrDefault();
                    if (b2selectedmatch == null)
                    {
                        MatchViewModel.Blue2CurrentMatch = new TeamMatchView();
                        MatchViewModel.originalB2 = new TeamMatch();
                        MatchViewModel.Blue2MatchNotFilled = true;
                        MatchViewModel.Blue2MatchEditable = false;
                    }
                    else
                    {
                        MatchViewModel.Blue2MatchNotFilled = false;
                        MatchViewModel.Blue2MatchEditable = true;
                        MatchViewModel.originalB2 = b2selectedmatch;
                        var matchtoput = new TeamMatchView();
                        matchtoput.A_InitiationLine = b2selectedmatch.A_InitiationLine;
                        matchtoput.DisabledSeconds = b2selectedmatch.DisabledSeconds;
                        matchtoput.EventCode = b2selectedmatch.EventCode;
                        matchtoput.E_Balanced = b2selectedmatch.E_Balanced;
                        matchtoput.E_ClimbAttempt = b2selectedmatch.E_ClimbAttempt;
                        matchtoput.E_ClimbSuccess = b2selectedmatch.E_ClimbSuccess;
                        matchtoput.E_Park = b2selectedmatch.E_Park;
                        matchtoput.MatchNumber = b2selectedmatch.MatchNumber;
                        matchtoput.NumCycles = b2selectedmatch.NumCycles;
                        matchtoput.ScoutName = b2selectedmatch.ScoutName;
                        matchtoput.TeamNumber = b2selectedmatch.TeamNumber;
                        matchtoput.T_ControlPanelPosition = b2selectedmatch.T_ControlPanelPosition;
                        matchtoput.T_ControlPanelRotation = b2selectedmatch.T_ControlPanelRotation;
                        if (b2selectedmatch.PowerCellMissed != null)
                        {
                            matchtoput.APowerCellInner = b2selectedmatch.PowerCellInner[0];
                            matchtoput.APowerCellOuter = b2selectedmatch.PowerCellOuter[0];
                            matchtoput.APowerCellLower = b2selectedmatch.PowerCellLower[0];
                            matchtoput.APowerCellMissed = b2selectedmatch.PowerCellMissed[0];
                            foreach (var value in b2selectedmatch.PowerCellInner.Skip(1))
                            {
                                matchtoput.TPowerCellInner += value;
                            }
                            foreach (var value in b2selectedmatch.PowerCellOuter.Skip(1))
                            {
                                matchtoput.TPowerCellOuter += value;
                            }
                            foreach (var value in b2selectedmatch.PowerCellLower.Skip(1))
                            {
                                matchtoput.TPowerCellLower += value;
                            }
                            foreach (var value in b2selectedmatch.PowerCellMissed.Skip(1))
                            {
                                matchtoput.TPowerCellMissed += value;
                            }
                        }
                        MatchViewModel.Blue2CurrentMatch = matchtoput;
                    }
                    //BLUE3
                    var b3selectedmatch = db.Matches.Where(x => x.TabletId == "B3" && x.EventCode == "test_env" && x.MatchNumber == MatchNum).FirstOrDefault();
                    if (b3selectedmatch == null)
                    {
                        MatchViewModel.Blue3CurrentMatch = new TeamMatchView();
                        MatchViewModel.originalB3 = new TeamMatch();
                        MatchViewModel.Blue3MatchNotFilled = true;
                        MatchViewModel.Blue3MatchEditable = false;
                    }
                    else
                    {
                        MatchViewModel.Blue3MatchEditable = true;
                        MatchViewModel.Blue3MatchNotFilled = false;
                        MatchViewModel.originalB3 = b3selectedmatch;
                        var matchtoput = new TeamMatchView();

                        matchtoput.A_InitiationLine = b3selectedmatch.A_InitiationLine;
                        matchtoput.DisabledSeconds = b3selectedmatch.DisabledSeconds;
                        matchtoput.EventCode = b3selectedmatch.EventCode;
                        matchtoput.E_Balanced = b3selectedmatch.E_Balanced;
                        matchtoput.E_ClimbAttempt = b3selectedmatch.E_ClimbAttempt;
                        matchtoput.E_ClimbSuccess = b3selectedmatch.E_ClimbSuccess;
                        matchtoput.E_Park = b3selectedmatch.E_Park;
                        matchtoput.MatchNumber = b3selectedmatch.MatchNumber;
                        matchtoput.NumCycles = b3selectedmatch.NumCycles;
                        matchtoput.ScoutName = b3selectedmatch.ScoutName;
                        matchtoput.TeamNumber = b3selectedmatch.TeamNumber;
                        matchtoput.T_ControlPanelPosition = b3selectedmatch.T_ControlPanelPosition;
                        matchtoput.T_ControlPanelRotation = b3selectedmatch.T_ControlPanelRotation;
                        if (b3selectedmatch.PowerCellMissed != null)
                        {
                            matchtoput.APowerCellInner = b3selectedmatch.PowerCellInner[0];
                            matchtoput.APowerCellOuter = b3selectedmatch.PowerCellOuter[0];
                            matchtoput.APowerCellLower = b3selectedmatch.PowerCellLower[0];
                            matchtoput.APowerCellMissed = b3selectedmatch.PowerCellMissed[0];
                            foreach (var value in b3selectedmatch.PowerCellInner.Skip(1))
                            {
                                matchtoput.TPowerCellInner += value;
                            }
                            foreach (var value in b3selectedmatch.PowerCellOuter.Skip(1))
                            {
                                matchtoput.TPowerCellOuter += value;
                            }
                            foreach (var value in b3selectedmatch.PowerCellLower.Skip(1))
                            {
                                matchtoput.TPowerCellLower += value;
                            }
                            foreach (var value in b3selectedmatch.PowerCellMissed.Skip(1))
                            {
                                matchtoput.TPowerCellMissed += value;
                            }
                        }

                        MatchViewModel.Blue3CurrentMatch = matchtoput;
                    }
                }
            }
            catch (Exception ex)
            {
                MatchViewModel.Red1MatchNotFilled = true;
                MatchViewModel.Red2MatchNotFilled = true;
                MatchViewModel.Red3MatchNotFilled = true;
                MatchViewModel.Blue1MatchNotFilled = true;
                MatchViewModel.Blue2MatchNotFilled = true;
                MatchViewModel.Blue3MatchNotFilled = true;
                MatchViewModel.Red1MatchEditable = false;
                MatchViewModel.Red2MatchEditable = false;
                MatchViewModel.Red3MatchEditable = false;
                MatchViewModel.Blue1MatchEditable = false;
                MatchViewModel.Blue2MatchEditable = false;
                MatchViewModel.Blue3MatchEditable = false;
                MatchViewModel.Red1CurrentMatch = new TeamMatchView();
                MatchViewModel.originalR1 = new TeamMatch();
                MatchViewModel.Red2CurrentMatch = new TeamMatchView();
                MatchViewModel.originalR2 = new TeamMatch();
                MatchViewModel.Red3CurrentMatch = new TeamMatchView();
                MatchViewModel.originalR3 = new TeamMatch();
                MatchViewModel.Blue1CurrentMatch = new TeamMatchView();
                MatchViewModel.originalB1 = new TeamMatch();
                MatchViewModel.Blue2CurrentMatch = new TeamMatchView();
                MatchViewModel.originalB2 = new TeamMatch();
                MatchViewModel.Blue3CurrentMatch = new TeamMatchView();
                MatchViewModel.originalB3 = new TeamMatch();
                Console.WriteLine(ex.ToString());
            }
            GraphViewModel.UserControlVisible = false;

        }
        public void SeeTablets()
        {
            matchViewModel.UserControlVisible = false;
            tabletViewModel.UserControlVisible = true;
            TBAViewModel.UserControlVisible = false;
            GraphViewModel.UserControlVisible = false;

            
        }
        public void SeeTBA()
        {
            matchViewModel.UserControlVisible = false;
            tabletViewModel.UserControlVisible = false;
            GraphViewModel.UserControlVisible = false;
            TBAViewModel.UserControlVisible = true;

        }
        public void SeeGraph()
        {
            matchViewModel.UserControlVisible = false;
            tabletViewModel.UserControlVisible = false;
            GraphViewModel = new GraphViewModel();
            GraphViewModel.UserControlVisible = true;
            TBAViewModel.UserControlVisible = false;

        }
        public void TryUSB()
        {
            ReadOnlyCollection<string> udids;
            int count = 0;

            var idevice = LibiMobileDevice.Instance.iDevice;
            var lockdown = LibiMobileDevice.Instance.Lockdown;
            var ret = idevice.idevice_get_device_list(out udids, ref count);

            if(!(ret == iDeviceError.NoDevice))
            {
                foreach(var udid in udids)
                {
                    iDeviceHandle deviceHandle;
                    idevice.idevice_new(out deviceHandle, udid);

                    LockdownClientHandle lockdownHandle;
                    lockdown.lockdownd_client_new_with_handshake(deviceHandle, out lockdownHandle, "LightScout").ThrowOnError();

                    string deviceName;
                    lockdown.lockdownd_get_device_name(lockdownHandle, out deviceName).ThrowOnError();

                    var error = idevice.idevice_connect(deviceHandle, 862, out iDeviceConnectionHandle connection);

                    

                    

                    ReceiveDataFromDevice(connection, idevice);

                    Console.WriteLine(deviceName);
                }
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

            Process startTCPforwarding = new Process();
            startTCPforwarding.StartInfo.FileName = "bash";
            string command = "adb reverse tcp:6000 tcp:6000";
            startTCPforwarding.StartInfo.Arguments = "-c \"" + command + "\"";
            startTCPforwarding.StartInfo.UseShellExecute = false;
            startTCPforwarding.StartInfo.RedirectStandardOutput = true;
            startTCPforwarding.StartInfo.CreateNoWindow = true;
            startTCPforwarding.ErrorDataReceived += (s,ea) =>
            {
                Console.WriteLine("There is no connected USB device that is valid!");
            };
            startTCPforwarding.Start();
            var theoutput = startTCPforwarding.StandardOutput.ReadToEnd().ToString();

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Loopback, 6000));
            byte[] testreceive = new byte[5000];
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
            int nummessagesreceived = 0;
            socket.Listen(100);
            Task.Run(() =>
            {
                while (true)
                {
                    startTCPforwarding.Start();
                }
            });
            socket.BeginAccept((ar) =>
            {
                var connectionAttempt = (Socket)ar.AsyncState;
                var connectedSocket = connectionAttempt.EndAccept(ar);
                Task.Run(() =>
                {
                    
                    while (true)
                    {
                        
                        try
                        {
                            byte[] givenBytes = new byte[90000];
                            int numbytessent = connectedSocket.Receive(givenBytes);
                            if (numbytessent <= 0) continue;
                            nummessagesreceived++;
                            byte[] finalBytes = givenBytes.Take(numbytessent).ToArray();
                            var stringmessage = Encoding.ASCII.GetString(finalBytes);
                            connectedSocket.Send(Encoding.ASCII.GetBytes("Got the message!"));
                            UseGivenData(stringmessage);
                            if (nummessagesreceived >= 2)
                            {
                                break;
                            }
                        }
                        catch(Exception ex)
                        {

                        }
                        



                    }
                });

            }, socket);
            
            Console.WriteLine("Test");

        }
        private void ReceiveDataFromDevice(iDeviceConnectionHandle connection, IiDeviceApi deviceApi)
        {
            byte[] inbytes = new byte[90000];
            Task.Run(() =>
            {
                while (true)
                {
                    inbytes = new byte[90000];
                    uint receivedBytes = 0;
                    deviceApi.idevice_connection_receive(connection, inbytes, (uint)inbytes.Length, ref receivedBytes);
                    if (receivedBytes <= 0) continue;
                    byte[] finalInBytes = inbytes.Take((int)receivedBytes).ToArray();
                    Console.WriteLine("Num of received bytes = " + receivedBytes.ToString());
                    UseGivenData(Encoding.ASCII.GetString(finalInBytes));
                    
                    //uint bytesSent = 0;
                    //deviceApi.idevice_connection_send(connection, Encoding.ASCII.GetBytes("Hijjjj"), (uint)Encoding.ASCII.GetBytes("Hijjjj").Length, ref bytesSent);
                }
            });
        }
        public void NextMatch()
        {

            CurrentMatchNum++;
            MatchNumString = "Match " + CurrentMatchNum.ToString();
            SeeMatches(CurrentMatchNum);
        }
        public void PrevMatch()
        {
            if (CurrentMatchNum > 1)
            {
                CurrentMatchNum--;
            }
            MatchNumString = "Match " + CurrentMatchNum.ToString();
            SeeMatches(CurrentMatchNum);
        }
    }
}
