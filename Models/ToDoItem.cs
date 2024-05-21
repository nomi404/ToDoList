using System;
using CsvHelper.Configuration.Attributes;
namespace ToDoList.Models
{
    public enum Priority
    {
        Low,
        Medium,
        High
    }
    public class TodoItem
    {

        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public DateTime? logTime { get; set; }
        public TimeSpan estimateTime { get; set; }
        public bool isClosed { get; set; }
        public DateTime? closeTime { get; set; }
        [Ignore]
        public bool isStarted { get; set; }
        public DateTime? startTime { get; set; } // Nullable DateTime
        public TimeSpan timeSpent { get; set; }

        public Priority priority { get; set; }  
        public TodoItem(int Id, string title, string description, TimeSpan estimateTime,Priority priority)
        {
            this.id = Id;
            this.title = title;
            this.description = description;
            this.logTime = DateTime.Now; // Set the current time when the task is created
            this.estimateTime = estimateTime;
            this.closeTime = DateTime.MinValue;
            this.startTime = DateTime.MinValue;
            this.isClosed = false; // Newly created tasks are not closed by default
            this.isStarted = false; // Task is not started by default
            this.timeSpent = TimeSpan.Zero; // Initialize time spent to zero
            this.priority = priority;   
        }
        public TodoItem(int id, string title, string description,DateTime logTime, TimeSpan estimateTime,bool isClosed,DateTime closeTime, DateTime startTime,TimeSpan timeSpent,Priority priority)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.logTime = logTime;
            this.estimateTime = estimateTime;
            this.isClosed = isClosed;
            this.closeTime = closeTime;
            this.startTime = startTime;
            this.timeSpent = timeSpent;
            this.priority = priority;
            
        }

        public string DisplayTimeSpent
        {
            get
            {
                return $"{(int)timeSpent.TotalHours:00h}:{timeSpent.Minutes:00m}:{timeSpent.Seconds:00s}";
            }
        }
        public string DisplayETimeSpent
        {
            get
            {
                return $"{(int)estimateTime.TotalHours:00h}:{estimateTime.Minutes:00m}:{estimateTime.Seconds:00s}";
            }
        }


        // Method to close the task
        public void CloseTask()
        {
            isClosed = true;
            closeTime = DateTime.Now; // Set the current time when the task is closed
        }

        // Method to start the task
        public void StartTask()
        {
            isStarted = true;
            startTime = DateTime.Now; // Set the current time when the task is started
        }

        // Method to stop the task
        public void StopTask()
        {
            isStarted = false;
            if (startTime.HasValue && startTime.Value != DateTime.MinValue)
            {
                // Calculate time spent on the task and add the time already spent
                timeSpent += (DateTime.Now - startTime.Value);
            }
        }
    }
}
