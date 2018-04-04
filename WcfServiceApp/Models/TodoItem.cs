using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceApp.Models
{
    [DataContract]
    public class TodoItem
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Owner { get; set; }
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public bool Completed { get; set; }
        [DataMember]
        public DateTime DateModified { get; set; }
    }
}
