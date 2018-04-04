using System.Collections.Generic;

namespace WcfServiceApp.Models
{
    public interface ITodoRepository
    {
        IList<TodoItem> ListAll();

        IList<TodoItem> ListByUser(string username);

        TodoItem Get(int id);

        TodoItem Add(TodoItem todoItem);

        TodoItem Update(TodoItem todoItem);

        void Delete(int id);
    }
}