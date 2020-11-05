using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToDoPOC.Models;

namespace ToDoPOC.SQLitecode
{
    public interface IOfflineDataService
    {
        Task InitializeAsync();
        Task addTodoItem(TodoItemModel todoItem);
        Task<List<TodoItemModel>> getTodoItems();
        Task deleteTodoItem(TodoItemModel todoItem);
    }
}
