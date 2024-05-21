using Microsoft.AspNetCore.Mvc;
using ToDoList.Models;
using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;

namespace ToDoList.Controllers
{
    public class TodoController : Controller
    {
        private readonly string csvFilePath = "todoItems.csv";
        private readonly string csvFilePathCompletedTasks = "doneItems.csv";

        private List<TodoItem> ReadTodoItemsFromCsv(string path)
        {
            List<TodoItem> todoItems = new List<TodoItem>();
            if (System.IO.File.Exists(path))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                };
                using (var reader = new StreamReader(path))
                using (var csv = new CsvReader(reader, config))
                {
                    todoItems = csv.GetRecords<TodoItem>().ToList();
                }
            }
            return todoItems;
        }

        private void WriteTodoItemsToCsv(List<TodoItem> todoItems,string path)
        {
            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(todoItems);
            }
        }

        public IActionResult Index()
        {
            var todoItems = ReadTodoItemsFromCsv(csvFilePath);
            return View(todoItems);
        }
        public IActionResult CompleteTask()
        {
            var completedTasks = ReadTodoItemsFromCsv(csvFilePathCompletedTasks);// Get the list of completed tasks from wherever they are stored
            return View(completedTasks);
        }

        [HttpPost]
        public IActionResult Create(string title, string description, int days, int hours ,int minutes,Priority priority)
        {
            var estimatedTime = new TimeSpan(days, hours, minutes);
            int nextId = 1;
            var todoItems = ReadTodoItemsFromCsv(csvFilePath);
            if (todoItems.Any())
            {
                nextId = todoItems.Max(t => t.id) + 1;
            }
            var newTodo = new TodoItem(nextId, title, description, estimatedTime, priority);
            todoItems.Add(newTodo);
            WriteTodoItemsToCsv(todoItems, csvFilePath);
            return RedirectToAction("Index");
        }

        public IActionResult StartTask(int id)
        {
            var todoItems = ReadTodoItemsFromCsv(csvFilePath);
            var todoItem = todoItems.FirstOrDefault(t => t.id == id);
            if (todoItem != null)
            {
                todoItem.StartTask();
                WriteTodoItemsToCsv(todoItems, csvFilePath);
                TempData["isStarted_" + id] = todoItem.isStarted;
            }
            return RedirectToAction("Index");
        }

        public IActionResult StopTask(int id)
        {

            var todoItems = ReadTodoItemsFromCsv(csvFilePath);
            var todoItem = todoItems.FirstOrDefault(t => t.id == id);
            bool initialIsStarted = ViewData["isStarted_"] as bool? ?? false;

            if (todoItem != null)
            {
                todoItem.StopTask();
                WriteTodoItemsToCsv(todoItems, csvFilePath);
                TempData["isStarted_" + id] = todoItem.isStarted;
            }
            return RedirectToAction("Index");
        }

        public IActionResult MarkComplete(int id)
        {
            var todoItems = ReadTodoItemsFromCsv(csvFilePath);
            var doneItems = ReadTodoItemsFromCsv(csvFilePathCompletedTasks);
            var todoItem = todoItems.FirstOrDefault(t => t.id == id);
            if (todoItem != null)
            {
                todoItem.CloseTask();
                doneItems.Add(todoItem);
                WriteTodoItemsToCsv(doneItems, csvFilePathCompletedTasks);
                Delete(id);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id, string description)
        {
            var todoItems = ReadTodoItemsFromCsv(csvFilePath);
            var todoItem = todoItems.FirstOrDefault(t => t.id == id);
            if (todoItem != null)
            {
                todoItem.description = description;
                WriteTodoItemsToCsv(todoItems, csvFilePath);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id) 
        {
            var todoItems = ReadTodoItemsFromCsv(csvFilePath);
            var todoItem = todoItems.FirstOrDefault(t => t.id == id);
            if (todoItem !=null) 
            {
                todoItems.Remove(todoItem);
                WriteTodoItemsToCsv(todoItems, csvFilePath);
               
            }
            return RedirectToAction("Index");   
        }

        public IActionResult UpdatePriority(int id,Priority priority)
        {
            var todoItems = ReadTodoItemsFromCsv(csvFilePath);
            var todoItem = todoItems.FirstOrDefault(t=>t.id == id);
            if (todoItem != null) 
            {
                todoItem.priority = priority;   
                WriteTodoItemsToCsv(todoItems,csvFilePath);
            }
            return RedirectToAction("Index");   
        }
    }
}
