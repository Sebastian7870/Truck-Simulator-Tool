using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Json;

namespace Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Classes
{
    public class ShiftSchedule
    {
        private List<ShiftScheduleJson> list_ShiftScheduleJson = new List<ShiftScheduleJson>();

        private List<(DateTime, int, IndexType)> startDates = new List<(DateTime startDate, int count, IndexType)>();
        private List<(DateTime, int, IndexType)> endDates = new List<(DateTime endDate, int count, IndexType)>();
        private List<(DateTime, int, IndexType)> startPauses = new List<(DateTime startPause, int count, IndexType)>();
        private List<(DateTime, int, IndexType)> endPauses = new List<(DateTime endPause, int count, IndexType)>();


        private bool currentShiftHasPause;
        private bool shiftPaused;
        private int shiftCount;
        private DateTime[] currentShiftStartEnd;
        private DateTime nextShiftPauseStart;
        private DateTime nextShiftPauseEnd;
        private DateTime nextShiftEnd;
        private Tuple<DateTime, int, IndexType> nextShiftEvent = null;
        public bool HasShift
        {
            get
            {
                if (list_ShiftScheduleJson[(list_ShiftScheduleJson.Count - 1)].EndDate > DateTime.Now)
                    return true;
                else
                {
                    ResetShiftScheduleValues();
                    return false;
                }
            }
        }
        public bool CurrentShiftHasPause
        {
            get 
            { return currentShiftHasPause; }
        }
        public bool ShiftPaused
        {
            get { return shiftPaused; }
        }
        public int ShiftCount
        {
            get { return shiftCount; }
        }
        public DateTime[] CurrentShiftStartEnd
        {
            get { return currentShiftStartEnd; }
        }
        public DateTime NextShiftEnd
        {
            get { return nextShiftEnd; }
        }
        public DateTime NextShiftPauseStart
        {
            get { return nextShiftPauseStart; }
        }
        public DateTime NextShiftPauseEnd
        {
            get { return nextShiftPauseEnd; }
        }
        public Tuple<DateTime, int, IndexType> NextShiftEvent
        {
            get { return nextShiftEvent; }
        }

        public enum IndexType
        {
            startDate,
            endDate,
            startPause,
            endPause
        }

        public void Update()
        {
            if (HasShift)
            {
                // Set timespans
                int counter = 0;
                foreach (ShiftScheduleJson Item in list_ShiftScheduleJson)
                {
                    counter++;
                    if (Item.StartDate.Ticks > DateTime.Now.Ticks)
                        startDates.Add((Item.StartDate, counter, IndexType.startDate));
                    if (Item.EndDate.Ticks > DateTime.Now.Ticks)
                        endDates.Add((Item.EndDate, counter, IndexType.endDate));
                    if (Item.StartPause.Ticks > DateTime.Now.Ticks)
                        startPauses.Add((Item.StartPause, counter, IndexType.startPause));
                    if (Item.EndPause.Ticks > DateTime.Now.Ticks)
                        endPauses.Add((Item.EndPause, counter, IndexType.endPause));
                }

                // Get min value for nextShiftEvent
                nextShiftEvent = ReturnMinValueLists(startDates, endDates, startPauses, endPauses);

                Tuple<DateTime, int, IndexType> _tuple = ReturnMinValueLists(endDates);
                //Get min value for ShiftCount
                shiftCount = _tuple.Item2;

                //Get next ShiftEnd
                _tuple = ReturnMinValueLists(endDates);
                nextShiftEnd = _tuple.Item1;

                //Get next ShiftPause
                Tuple<DateTime, int, IndexType> _tuple2 = ReturnMinValueLists(startPauses);
                nextShiftPauseStart = _tuple2.Item1;
                Tuple<DateTime, int, IndexType> _tuple3 = ReturnMinValueLists(endPauses);
                nextShiftPauseEnd = _tuple2.Item1;

                if (_tuple2.Item1.Ticks < DateTime.Now.Ticks && _tuple3.Item1.Ticks > DateTime.Now.Ticks)
                    shiftPaused = true;
                if (_tuple3.Item2 != _tuple.Item2)
                    currentShiftHasPause = false;

                //Get current StartDate
                currentShiftStartEnd[0] = list_ShiftScheduleJson[(_tuple.Item2 - 1)].StartDate;
                currentShiftStartEnd[1] = list_ShiftScheduleJson[(_tuple.Item2 - 1)].EndDate;
            }
        }

        public void CreateShift(DateTime startDate, double durationDays, double shiftTimeHours, double shiftPauseTimeHours)
        {
            ResetShiftScheduleValues();

            double _pauseTime = 0.75; //(hardcoded)
            DateTime _startDate = startDate;
            int counter = 0;
            do
            {
                counter++;
                ShiftScheduleJson shiftSchedule = new ShiftScheduleJson
                {
                    Count = counter,
                    StartDate = _startDate,
                    EndDate = _startDate.AddHours(shiftTimeHours + _pauseTime),
                    StartPause = _startDate.AddHours(shiftTimeHours / 2),
                    EndPause = _startDate.AddHours((shiftTimeHours / 2) + _pauseTime)
                };
                list_ShiftScheduleJson.Add(shiftSchedule);
                _startDate.AddHours(shiftPauseTimeHours + shiftTimeHours + _pauseTime);
            }
            while (startDate < startDate.AddDays(durationDays));
        }

        public string ReturnShiftScheduleText()
        {
            string str = string.Empty;
            foreach (ShiftScheduleJson Item in list_ShiftScheduleJson)
            {
                str += $"{Item.Count}\n";
                str += $"Schichtbeginn_______: {Item.StartDate}      [{DateTimeFormatInfo.CurrentInfo.GetDayName(Item.StartDate.DayOfWeek)}]";
                str += $"Schichtpausenbeginn_: {Item.StartPause}        [{DateTimeFormatInfo.CurrentInfo.GetDayName(Item.StartPause.DayOfWeek)}]\n";
                str += $"Schichtpausenende___: {Item.EndPause}      [{DateTimeFormatInfo.CurrentInfo.GetDayName(Item.EndPause.DayOfWeek)}]";
                str += $"Schichtende_________: {Item.EndDate}       [{DateTimeFormatInfo.CurrentInfo.GetDayName(Item.EndDate.DayOfWeek)}\n";
                str += "###########################################################################";
            }
            return str;
        }

        private void ResetShiftScheduleValues()
        {
            list_ShiftScheduleJson.Clear();
            startDates.Clear();
            endDates.Clear();
            startPauses.Clear();
            endPauses.Clear();

            currentShiftHasPause = false;
            shiftPaused = false;
            shiftCount = 0;
            currentShiftStartEnd = null;
            nextShiftPauseStart = DateTime.MinValue;
            nextShiftPauseEnd = DateTime.MinValue;
            nextShiftEnd = DateTime.MinValue;
            nextShiftEvent = null;
        }

        private static Tuple<DateTime, int, IndexType> ReturnMinValueLists(params List<(DateTime, int, IndexType)>[] list)
        {
            List<(DateTime, int, IndexType)> allMinTimeSpans = new List<(DateTime, int, IndexType)>();
            foreach (List<(DateTime, int, IndexType)> Item in list)
            {
                Item.Sort((a, b) => a.Item2.CompareTo(b.Item2));
                allMinTimeSpans.Add(Item.First());
            }
            allMinTimeSpans.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            return allMinTimeSpans.First().ToTuple();
        }
    }
}
