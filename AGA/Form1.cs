using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace AGA
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            {
                Dictionary<string, string> Params = new Dictionary<string, string>()
    {
        { "", "" },
        { "", "" },
        { "", "" },
        { "", "" },
    };
               var answer = GetRequest("https://khakasia.bitrix24.ru/rest/1870/5l6u5im92ao6r42m/tasks.task.list?filter%5BGROUP_ID%5D=832&start=50./", Params).Result;
                Console.WriteLine(answer.ToString());
                Console.ReadLine();
            }
        }
        static async Task<HttpResponseMessage> GetRequest(string adress, Dictionary<string, string> Params)
        {
            HttpClient client = new HttpClient();
            Uri uri = new Uri(adress);
            var content = new FormUrlEncodedContent(Params);
            return await client.PostAsync(adress, content);
        }
    }
}
