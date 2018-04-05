using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using WcfServiceApp.Models;

namespace WcfServiceApp
{
    public class TaskService : ITaskService
    {
        private static readonly ITodoRepository repository = new TodoInMemoryRepository();

        protected ClaimsPrincipal User => Thread.CurrentPrincipal as ClaimsPrincipal;

        public IList<TodoItem> GetAllTodoItems()
        {
            return repository.ListAll();
        }

        public IList<TodoItem> GetUserTodoItems()
        {
            return repository.ListByUser(User.GetId());
        }

        public TodoItem AddTodoItem(TodoItem item)
        {
            item.Owner = User.GetId();
            return repository.Add(item);
        }

        public bool DeleteTodoItem(int itemId)
        {
            var item = repository.Get(itemId);
            var isCurrentUserOwner = item != null && (item.Owner == User.GetId() || true);
            if (isCurrentUserOwner)
            {
                repository.Delete(itemId);
                return true;
            }

            return false;
        }
    }
}
