using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace AGA
{
    public partial class Form1 : Form
    {
        private List<Task> tasks; // Список всех задач, загруженных из API

        private TextBox directionTextBox;
        private TextBox responsibleTextBox;
        private TextBox stageTextBox;
        private TextBox deadlineTextBox;

        public Form1()
        {
            InitializeComponent();
            InitializeFilterControls();
        }

        private void InitializeFilterControls()
        {
            // Создание текстовых полей для фильтров с подписями
            TableLayoutPanel filterPanel = new TableLayoutPanel();
            filterPanel.RowCount = 1;
            filterPanel.ColumnCount = 8;
            filterPanel.Dock = DockStyle.Top;
            filterPanel.AutoSize = true;
            this.Controls.Add(filterPanel);

            filterPanel.Controls.Add(new Label { Text = "Направление:", AutoSize = true }, 0, 0);
            directionTextBox = new TextBox();
            directionTextBox.TextChanged += ApplyFilters;
            directionTextBox.Tag = "Direction"; // Тег для идентификации колонки
            filterPanel.Controls.Add(directionTextBox, 1, 0);

            filterPanel.Controls.Add(new Label { Text = "Ответственный:", AutoSize = true }, 2, 0);
            responsibleTextBox = new TextBox();
            responsibleTextBox.TextChanged += ApplyFilters;
            responsibleTextBox.Tag = "Responsible"; // Тег для идентификации колонки
            filterPanel.Controls.Add(responsibleTextBox, 3, 0);

            filterPanel.Controls.Add(new Label { Text = "Стадия:", AutoSize = true }, 4, 0);
            stageTextBox = new TextBox();
            stageTextBox.TextChanged += ApplyFilters;
            stageTextBox.Tag = "Stage"; // Тег для идентификации колонки
            filterPanel.Controls.Add(stageTextBox, 5, 0);

            filterPanel.Controls.Add(new Label { Text = "Срок:", AutoSize = true }, 6, 0);
            deadlineTextBox = new TextBox();
            deadlineTextBox.TextChanged += ApplyFilters;
            deadlineTextBox.Tag = "Deadline"; // Тег для идентификации колонки
            filterPanel.Controls.Add(deadlineTextBox, 7, 0);
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Получаем задачи из сервиса...");
            string url = "https://khakasia.bitrix24.ru/rest/1870/5l6u5im92ao6r42m/tasks.task.list?filter[GROUP_ID]=832";
            TaskFetcher taskFetcher = new TaskFetcher();
            tasks = await taskFetcher.FetchAllTasksAsync(url);

            dataGridView1.DataSource = tasks;
        }

        private void ApplyFilters(object sender, EventArgs e)
        {
            List<Task> filteredTasks = tasks;

            if (!string.IsNullOrEmpty(directionTextBox.Text))
            {
                filteredTasks = filteredTasks.Where(t => t.Direction.IndexOf(directionTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            if (!string.IsNullOrEmpty(responsibleTextBox.Text))
            {
                filteredTasks = filteredTasks.Where(t => t.Responsible.IndexOf(responsibleTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            if (!string.IsNullOrEmpty(stageTextBox.Text))
            {
                filteredTasks = filteredTasks.Where(t => t.Stage.IndexOf(stageTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            if (!string.IsNullOrEmpty(deadlineTextBox.Text))
            {
                filteredTasks = filteredTasks.Where(t => t.Deadline.IndexOf(deadlineTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            dataGridView1.DataSource = filteredTasks;
        }
    }

    public class Task
    {
        public string Direction { get; set; }
        public string Consumer { get; set; }
        public string Id { get; set; }
        public string Project { get; set; }
        public string Priority { get; set; }
        public string Responsible { get; set; }
        public string Stage { get; set; }
        public string Deadline { get; set; }
    }

    public class TaskFetcher
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<List<Task>> FetchAllTasksAsync(string initialUrl)
        {
            List<Task> allTasks = new List<Task>();
            string url = initialUrl;
            int start = 0;

            while (true)
            {
                HttpResponseMessage response = await client.GetAsync($"{url}&start={start}");
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(responseBody);
                JArray tasksArray = (JArray)json["result"]["tasks"];

                foreach (var item in tasksArray)
                {
                    Task task = new Task
                    {
                        Direction = "Улучшение", // Пример значения, можно изменить по необходимости
                        Consumer = "1 ОСНОВНЫЕ ПРОЦЕССЫ", // Пример значения, можно изменить по необходимости
                        Id = (string)item["id"],
                        Project = (string)item["title"],
                        Priority = (string)item["priority"],
                        Responsible = (string)item["responsible"]["name"],
                        Stage = "Готово", // Пример значения, можно изменить по необходимости
                        Deadline = "" // Пример значения, можно изменить по необходимости
                    };

                    allTasks.Add(task);
                }

                if (json["next"] == null)
                {
                    break;
                }

                int next = (int)json["next"];
                if (next == start || tasksArray.Count == 0)
                {
                    break;
                }
                start = next;
            }

            return allTasks;
        }
    }
}
