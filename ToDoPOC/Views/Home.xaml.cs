using System;
using System.Collections.Generic;

using Xamarin.Forms;
using ToDoPOC.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ToDoPOC.Utilities;
using ToDoPOC.SQLitecode;

namespace ToDoPOC.Views
{
    public partial class Home : ContentPage
    {
        public ObservableCollection<TodoItemModel> TodoListItems{ get; set; }
        private OfflineDataService offlineDataService;

        public Home()
        {
            InitializeComponent();
            offlineDataService = new OfflineDataService();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            fetchToDoItems();
        }

        async void fetchToDoItems()
        {
            if (GlobalConstants.isOffline)
            {
                var fetchedItems = await offlineDataService.getTodoItems();
                TodoListItems = new ObservableCollection<TodoItemModel>(fetchedItems as List<TodoItemModel>);
                updateListView();
            }
            else
            {
                HttpClient client = new HttpClient();
                string responseString = await client.GetStringAsync("https://lab6consumerest.azurewebsites.net/api/todoitems");

                if (responseString != "")
                {
                    Console.WriteLine(responseString);
                    var result = JsonConvert.DeserializeObject<List<TodoItemModel>>(responseString);
                    TodoListItems = new ObservableCollection<TodoItemModel>(result as List<TodoItemModel>);
                    updateListView();
                    //storeLocally(result);
                }
            }
        }

        void updateListView()
        {
            notesLV.ItemsSource = null;
            notesLV.ItemsSource = TodoListItems;
        }

        //void storeLocally(List<TodoItemModel> toDoItems)
        //{

        //}

        async void onNewNoteBtnClick(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AddNote());
        }

        void onDeletBtnClick(object sender, EventArgs e)
        {
            Console.WriteLine("deletteee");
            var button = sender as Button;
            var clickedNote = button.BindingContext as TodoItemModel;
            deleteSelectedNote(clickedNote);
        }

        async void deleteSelectedNote(TodoItemModel selectedNote)
        {
            if (GlobalConstants.isOffline)
            {
                await offlineDataService.deleteTodoItem(selectedNote);
                await DisplayAlertWithHandler(null, "Note deleted successfully from LOCAL DB!", "ok", () =>
                {
                    Console.WriteLine("clicked okkk");
                    //Deleting selected note from list
                    TodoListItems.Remove(selectedNote);
                    updateListView();
                });
            }
            else
            {
                HttpClient client = new HttpClient();
                var responseString = await client.DeleteAsync("https://lab6consumerest.azurewebsites.net/api/todoitems/" + selectedNote.id);

                if (responseString.IsSuccessStatusCode)
                {
                    await DisplayAlertWithHandler(null, "Note deleted successfully!", "ok", () =>
                    {
                        Console.WriteLine("clicked okkk");
                        //Deleting selected note from list
                        TodoListItems.Remove(selectedNote);
                        updateListView();
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
