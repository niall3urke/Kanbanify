using Kanbanify.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace Kanbanify.Shared
{
    public partial class Calendar
    {
        // Parameters

        [Parameter] public Action<DateTime> DateChanged { get; set; }

        [Parameter] public bool ShowHeader { get; set; }

        // Fields

        private List<CalendarDayModel> days;
        private DateTime date;

        private string headerMonth;
        private string headerYear;
        private string headerDate;

        // Constructors

        protected override void OnInitialized()
        {
            days = new List<CalendarDayModel>();
            date = DateTime.Now;
            Update();
        }

        // Handlers

        void NextMonth()
        {
            int month = date.Month + 1;
            int year = date.Year;

            if (date.Month == 12)
            {
                month = 1;
                year++;
            }

            date = new DateTime(year, month, 1);
            Update();
        }
        void PrevMonth()
        {
            int month = date.Month - 1;
            int year = date.Year;

            if (date.Month == 1)
            {
                month = 12;
                year--;
            }

            date = new DateTime(year, month, 1);
            Update();
        }

        void DayClicked(CalendarDayModel day)
        {
            days.ForEach(d => d.IsSelected = false);
            day.IsSelected = true;

            var dt = new DateTime(date.Year, date.Month, day.Number);
            DateChanged?.Invoke(dt);

            StateHasChanged();
        }

        // Methods

        void Update()
        {
            PopulateCalendar();
            PopulateLabels();

            StateHasChanged();
        }

        void PopulateLabels()
        {
            headerDate = $"{date:ddd}, {date:MMM} {date.Day}";
            headerMonth = $"{date:MMM}, {date.Year}";
            headerYear = $"{date.Year}";
        }

        void PopulateCalendar()
        {
            // Empty the content before repopulating
            days.Clear();

            // Get the number of days in the month (1 - 31)
            int daysInMonth = DateTime.DaysInMonth(
                date.Year, date.Month);

            // Get the 1-based day of the week (1 - 7)
            int firstDayOffset = (int)(new DateTime(
                date.Year, date.Month, 1).DayOfWeek + 1);

            // If the first day is a sunday, reset back to 
            // the base. This will prevent an empty row 
            if (firstDayOffset == 7)
            {
                firstDayOffset = 0;
            }

            // Get the number of days plus the initial first day offset
            int totalDays = daysInMonth + firstDayOffset;

            // Loop from day 1 to the total number of days
            for (int i = 1; i <= totalDays; i++)
            {
                if (i <= firstDayOffset)
                {
                    // Just leave a blank space for "days" 
                    // preceeding the first day of the month
                    days.Add(new CalendarDayModel());
                    continue;
                }

                // The increment less the first day offset will
                // give us the current 1 - 31 day number
                int day = i - firstDayOffset;

                // Determine the type of button to display
                string buttonType = GetButtonType(day);

                // Add a new button to the days list
                days.Add(new CalendarDayModel(buttonType, day));
            }
        }

        string GetButtonType(int day)
        {
            if (DayIsWeekend(day))
                return "is-static";

            if (DayIsToday(day))
                return "is-secondary";

            return "is-white";
        }

        bool DayIsToday(int day)
        {
            var d1 = new DateTime(date.Year, date.Month, day);
            var d2 = DateTime.Now;

            if (d1.Day != d2.Day)
                return false;

            if (d1.Month != d2.Month)
                return false;

            if (d1.Year != d2.Year)
                return false;

            return true;
        }

        bool DayIsWeekend(int day)
        {
            var dt = new DateTime(date.Year, date.Month, day);
            return dt.DayOfWeek == DayOfWeek.Saturday ||
                dt.DayOfWeek == DayOfWeek.Sunday;
        }
    }
}
