﻿using Calendar.Models.Enums;
using System.ComponentModel;

namespace Calendar.Models
{
    public class Tasks
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [DisplayName("Description")]
        public string TaskDescription { get; set; }

        [DisplayName("Recurrance frequency")]
        public RecurranceFrequencyEnum RecurranceFrequency { get; set; }

        public int DueDays { get; set; }

        public BusinessDaysEnum BusinessDays { get; set; }

        public DueCompletionEnum DueCompletion { get; set; }

        [DisplayName("Start date")]
        public DateTime StartDate { get; set; }
    }
}
