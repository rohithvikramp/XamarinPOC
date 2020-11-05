using System;
using SQLite;

namespace ToDoPOC.Models
{
    [Table ("TodoItems")]
    public class TodoItemModel
    {
        [PrimaryKey]
        public string id { get; set; }
        public string name { get; set; }
        public string notes { get; set; }
        public bool done { get; set; }

        public TodoItemModel()
        {
        }
    }
}
