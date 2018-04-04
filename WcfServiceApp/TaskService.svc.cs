using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfServiceApp.Models;

namespace WcfServiceApp
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class TaskService : ITaskService
    {
        private static readonly ITodoRepository repository = new TodoInMemoryRepository();

        public IList<TodoItem> GetAllTodoItems()
        {
            return repository.ListAll();
        }

        public IList<TodoItem> GetUserTodoItems()
        {
            return repository.ListByUser("");
        }

        public TodoItem AddTodoItem(TodoItem item)
        {
            return repository.Add(item);
        }

        public bool DeleteTodoItem(int itemId)
        {
            var item = repository.Get(itemId);
            var isCurrentUserOwner = item != null && (item.Owner == "" || true);
            if (isCurrentUserOwner)
            {
                repository.Delete(itemId);
                return true;
            }

            return false;
        }
    }
}
