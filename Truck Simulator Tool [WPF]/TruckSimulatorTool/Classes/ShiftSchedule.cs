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

        private List<(TimeSpan, int, IndexType)> startDates = new List<(TimeSpan startDate, int count, IndexType)>();
        private List<(TimeSpan, int, IndexType)> endDates = new List<(TimeSpan endDate, int count, IndexType)>();
        private List<(TimeSpan, int, IndexType)> startPauses = new List<(TimeSpan startPause, int count, IndexType)>();
        private List<(TimeSpan, int, IndexType)> endPauses = new List<(TimeSpan endPause, int count, IndexType)>();


        private bool hasShift;
        private int shiftCount;
        private DateTime nextShiftEnd;
        private Tuple<TimeSpan, int, IndexType> nextShiftEvent = null;
        public bool HasShift
        {
            get { return hasShift; }
            set { hasShift = value; }
        }
        public int ShiftCount
        {
            get { return shiftCount; }
            set { shiftCount = value; }
        }
        public DateTime NextShiftEnd
        {
            get { return nextShiftEnd; }
            set { nextShiftEnd = value; }
        }
        public Tuple<TimeSpan, int, IndexType> NextShiftEvent
        {
            get { return nextShiftEvent; }
            set { nextShiftEvent = value; }
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
            // Set timespans
            int counter = -1;
            foreach (ShiftScheduleJson Item in list_ShiftScheduleJson)
            {
                counter++;
                if (Item.StartDate.Ticks > DateTime.Now.Ticks)
                    startDates.Add((Item.StartDate.Subtract(DateTime.Now), counter, IndexType.startDate));
                if (Item.EndDate.Ticks > DateTime.Now.Ticks)
                    endDates.Add((Item.EndDate.Subtract(DateTime.Now), counter, IndexType.endDate));
                if (Item.StartPause.Ticks > DateTime.Now.Ticks)
                    startPauses.Add((Item.StartPause.Subtract(DateTime.Now), counter, IndexType.startPause));
                if (Item.EndPause.Ticks > DateTime.Now.Ticks)
                    endPauses.Add((Item.EndPause.Subtract(DateTime.Now), counter, IndexType.endPause));
            }

            // Get min value for nextShiftEvent
            NextShiftEvent = ReturnMinValueLists(startDates, endDates, startPauses, endPauses);

            Tuple<TimeSpan, int, IndexType> _tuple = ReturnMinValueLists(endDates);
            //Get min value for ShiftCount
            ShiftCount = _tuple.Item2;

            //Get next ShiftEnd
            _tuple = ReturnMinValueLists(endDates);
            NextShiftEnd = 

        }


        private static Tuple<TimeSpan, int, IndexType> ReturnMinValueLists(params List<(TimeSpan, int, IndexType)>[] list)
        {
            List<(TimeSpan, int, IndexType)> allMinTimeSpans = new List<(TimeSpan, int, IndexType)>();
            foreach (List<(TimeSpan, int, IndexType)> Item in list)
            {
                Item.Sort((a, b) => a.Item2.CompareTo(b.Item2));
                allMinTimeSpans.Add(Item.First());
            }
            allMinTimeSpans.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            return allMinTimeSpans.First().ToTuple();
        }
    }
}
