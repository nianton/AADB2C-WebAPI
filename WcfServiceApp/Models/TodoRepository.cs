using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace WcfServiceApp.Models
{
    public class TodoInMemoryRepository : ITodoRepository
    {
        // In this service we're using an in-memory list to store tasks, just to keep things simple.
        // All of your tasks will be lost each time you run the service

        private readonly IList<TodoItem> _list;
        private int _idSeed = 0;

        public TodoInMemoryRepository()
        {
            _list = new List<TodoItem>();
        }

        public TodoItem Add(TodoItem todoItem)
        {
            todoItem.Id = GetNextId();
            _list.Add(todoItem);
            return todoItem;
        }

        public void Delete(int id)
        {
            var item = _list.SingleOrDefault(i => i.Id == id);
            if (item != null)
            {
                _list.Remove(item);
            }
        }

        public TodoItem Get(int id)
        {
            return _list.SingleOrDefault(i => i.Id == id);
        }

        public IList<TodoItem> ListAll()
        {
            return _list.ToArray();
        }

        public IList<TodoItem> ListByUser(string username)
        {
            return _list
                .Where(i => i.Owner.Equals(username, StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }

        public TodoItem Update(TodoItem todoItem)
        {
            var existingItem = _list.Single(i => i.Id == todoItem.Id);
            _list.Remove(existingItem);
            _list.Add(todoItem);

            return todoItem;
        }

        private int GetNextId()
        {
            return Interlocked.Increment(ref _idSeed);
        } 
    }
}