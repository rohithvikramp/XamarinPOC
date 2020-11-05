using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SQLite;
using ToDoPOC.Models;

namespace ToDoPOC.SQLitecode
{
    public class OfflineDataService : IOfflineDataService
    {
        private SQLiteAsyncConnection sqliteDBConnection;

        public OfflineDataService()
        {
            var task = Task.Run(async () => await InitializeAsync());
            task.Wait();
        }

        public async Task addTodoItem(TodoItemModel todoItem)
        {
            if(todoItem != null)
            {
                _ = await sqliteDBConnection.InsertAsync(todoItem);
            }
        }

        public async Task deleteTodoItem(TodoItemModel todoItem)
        {
            if (todoItem != null)
            {
                _ = await sqliteDBConnection.DeleteAsync(todoItem);
            }
        }

        public Task<List<TodoItemModel>> getTodoItems()
        {
            return sqliteDBConnection.Table<TodoItemModel>().ToListAsync();
        }

        public async Task InitializeAsync()
        {
            if (sqliteDBConnection != null) return;
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "databases");
            System.IO.Directory.CreateDirectory(path);
            Console.WriteLine("DATABASE PATH : ");
            Console.WriteLine(path);

            sqliteDBConnection = new SQLiteAsyncConnection(Path.Combine(path, "TodoSQLiteDB.db3"));
            _ = await sqliteDBConnection.CreateTableAsync<TodoItemModel>();
        }
    }
}
