using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using ToDoPOC.Utilities;
using ToDoPOC.SQLitecode;
using ToDoPOC.Models;

namespace ToDoPOC
{
    public partial class AddNote : ContentPage
    {
        private OfflineDataService offlineDataService;

        public AddNote()
        {
            InitializeComponent();
            offlineDataService = new OfflineDataService();
        }

        async void onCancelBtnClick(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        async void onDoneBtnClick(object sender, EventArgs e)
        {
            Console.WriteLine(nameEntry.Text);
            Console.WriteLine(notesEntry.Text);

            Random generator = new Random();
            string randomumber = generator.Next(100000, 999999).ToString();
            Console.WriteLine(randomumber);

            if (GlobalConstants.isOffline)
            {
                var newNote = new TodoItemModel
                {
                    id = randomumber,
                    name = nameEntry.Text,
                    notes = notesEntry.Text,
                };

                await offlineDataService.addTodoItem(newNote);
                await DisplayAlertWithHandler(null, "Note added successfully on LOCAL DB!", "ok", async () =>
                {
                    Console.WriteLine("added okkk");
                    //popping back to list screen
                    await Navigation.PopModalAsync();
                });
            }
            else
            {
                var newNote = new
                {
                    id = randomumber,
                    name = nameEntry.Text,
                    notes = notesEntry.Text,
                };
                var jsonNote = JsonConvert.SerializeObject(newNote);
                var content = new StringContent(jsonNote, Encoding.UTF8, "application/json");

                //pushing new note
                var myHttpClient = new HttpClient();
                HttpResponseMessage response = await myHttpClient.PostAsync("https://lab6consumerest.azurewebsites.net/api/todoitems", content);
                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlertWithHandler(null, "Note added successfully!", "ok", async () =>
                    {
                        Console.WriteLine("added okkk");
                        //popping back to list screen
                        await Navigation.PopModalAsync();
                    });
                }
                else
                {
                    await DisplayAlert(null, "Something went wrong!", "ok");
                }
            }
        }


        public async Task DisplayAlertWithHandler(string message,
            string title,
            string buttonText,
            Action afterHideCallback)
        {
            await DisplayAlert(
                title,
                message,
                buttonText);

            afterHideCallback?.Invoke();
        }
    }
}
