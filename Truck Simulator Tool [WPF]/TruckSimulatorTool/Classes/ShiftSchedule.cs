using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
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
        private bool currentShiftIsActive;
        private bool shiftPaused;
        private int shiftCount;
        private DateTime[] currentShiftStartEnd = new DateTime[2];
        private DateTime nextShiftPauseStart;
        private DateTime nextShiftPauseEnd;
        private DateTime nextShiftEnd;
        private Tuple<DateTime, int, IndexType> nextShiftEvent = null;
        public List<ShiftScheduleJson> Getlist_ShiftScheduleJson
        {
            get { return list_ShiftScheduleJson; }
        }
        public bool HasShift
        {
            get
            {
                if (list_ShiftScheduleJson.Count > 0 && list_ShiftScheduleJson[(list_ShiftScheduleJson.Count - 1)].EndDate > DateTime.Now)
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
        public bool CurrentShiftIsActive
        {
            get { return currentShiftIsActive; }
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
                if (nextShiftEvent.Item3 == IndexType.startDate)
                    currentShiftIsActive = false;
                else
                    currentShiftIsActive = true;

                Tuple<DateTime, int, IndexType> _tuple = ReturnMinValueLists(endDates);
                //Get min value for ShiftCount
                shiftCount = _tuple.Item2;

                //Get next ShiftEnd
                nextShiftEnd = _tuple.Item1;

                //Get next ShiftPause
                Tuple<DateTime, int, IndexType> _tuple2 = ReturnMinValueLists(startPauses);
                nextShiftPauseStart = _tuple2.Item1;
                Tuple<DateTime, int, IndexType> _tuple3 = ReturnMinValueLists(endPauses);
                nextShiftPauseEnd = _tuple3.Item1;

                if (_tuple3.Item2 != _tuple.Item2)
                    currentShiftHasPause = false;
                else
                    currentShiftHasPause = true;

                if (currentShiftHasPause && list_ShiftScheduleJson[(_tuple.Item2 - 1)].StartPause < DateTime.Now && _tuple3.Item1 > DateTime.Now)
                    shiftPaused = true;
                else
                    shiftPaused = false;

                //Get current StartDate
                currentShiftStartEnd[0] = list_ShiftScheduleJson[(_tuple.Item2 - 1)].StartDate;
                currentShiftStartEnd[1] = list_ShiftScheduleJson[(_tuple.Item2 - 1)].EndDate;
            }
        }


        public void CreateShift(DateTime startDate, int durationDays, double shiftTimeHours, double shiftPauseTimeHours)
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
                _startDate = _startDate.AddHours(shiftPauseTimeHours + shiftTimeHours + _pauseTime);
            }
            while (_startDate < startDate.AddDays(durationDays));

            if (list_ShiftScheduleJson[(list_ShiftScheduleJson.Count - 1)].EndDate < DateTime.Now)
                MessageBox.Show("Das gewählte Startdatum liegt zu weit in der Vergangenheit. Bitte erstellen Sie einen Zeitgemäßen Schichtplan.", "Schicht abgelaufen!", MessageBoxButton.OK, MessageBoxImage.Information);
            // values will be resetted in HasShift method.
        }

        public void LoadShift(string path)
        {
            try
            {
                List<ShiftScheduleJson> _list_ShiftScheduleJson = new List<ShiftScheduleJson>(JsonConvert.DeserializeObject<List<ShiftScheduleJson>>(File.ReadAllText(path)));

                if (_list_ShiftScheduleJson.Count > 0 && _list_ShiftScheduleJson[(_list_ShiftScheduleJson.Count - 1)].EndDate > DateTime.Now)
                {
                    list_ShiftScheduleJson = _list_ShiftScheduleJson;
                }
                else
                {
                    MessageBox.Show("Die Datei konnte nicht geladen werden, weil der Zeitplan schon abgelaufen ist.", "Zeitplan abgelaufen!", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch
            {
                MessageBox.Show("Die Datei konnte nicht geladen werden, wahrscheinlich hatte Sie ein falsches Format.", "Falsches Format!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void DeleteShift()
        {
            ResetShiftScheduleValues();
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
            currentShiftStartEnd = new DateTime[2];
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
