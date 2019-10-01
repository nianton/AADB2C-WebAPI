using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Authorize]
    public class TasksController : ApiController
    {
        private static readonly ITodoRepository repository = new TodoInMemoryRepository();

        // API Scopes
        public static string ReadPermission = ConfigurationManager.AppSettings["api:ReadScope"];
        public static string WritePermission = ConfigurationManager.AppSettings["api:WriteScope"];

        [HttpGet]
        [Route("api/tasks/all")]
        [AllowAnonymous]
        public IEnumerable<TodoItem> GetAll()
        {
            return repository.ListAll();
        }


        public IEnumerable<TodoItem> Get()
        {
            HasRequiredScopes(ReadPermission);
            string owner = User.GetId();
            IEnumerable<TodoItem> userTasks = repository.ListByUser(owner);
            return userTasks;
        }

        public void Post(TodoItem task)
        {
            HasRequiredScopes(WritePermission);

            if (String.IsNullOrEmpty(task.Text))
                throw new WebException("Please provide a task description");

            task.Owner = User.GetId();
            task.Completed = false;
            task.DateModified = DateTime.UtcNow;
            repository.Add(task);
        }

        public void Delete(int id)
        {
            HasRequiredScopes(WritePermission);
            repository.Delete(id);
        }

        // Validate to ensure the necessary scopes are present.
        private void HasRequiredScopes(String permission)
        {        
            // HACK: No Scope checks for now ** CHANGE ACCORDINGLY **
            return;

            //if (User.Identity.AuthenticationType == Startup.AadAuthType)
            //{
            //    // TODO: Maybe check different set of claims for AAD authentication.
            //    // For now, we let the user access the resource for the demo app.
            //    return;
            //}

            //if (!User.HasPermission(permission))
            //{
            //    throw new HttpResponseException(new HttpResponseMessage
            //    {
            //        StatusCode = HttpStatusCode.Unauthorized,
            //        ReasonPhrase = $"The Scope claim does not contain the {permission} permission."
            //    });
            //}
        }
    }
}
