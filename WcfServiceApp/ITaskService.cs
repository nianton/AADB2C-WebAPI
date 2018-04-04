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
    [ServiceContract]
    public interface ITaskService
    {
        [OperationContract]
        IList<TodoItem> GetAllTodoItems();

        [OperationContract]
        IList<TodoItem> GetUserTodoItems();

        [OperationContract]
        TodoItem AddTodoItem(TodoItem item);

        [OperationContract]
        bool DeleteTodoItem(int itemId);
    }  
}
