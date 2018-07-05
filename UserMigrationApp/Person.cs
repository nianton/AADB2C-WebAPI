using System;

namespace UserMigrationApp
{
    public class Person
    {
        public int PersonID { get; set; }

        public string FullName { get; set; }

        public string PreferredName { get; set; }

        public string SearchName { get; set; }

        public bool IsPermittedToLogon { get; set; }

        public string LogonName { get; set; }

        public bool IsExternalLogonProvider { get; set; }

        public byte[] HashedPassword { get; set; }

        public bool IsSystemUser { get; set; }

        public bool IsEmployee { get; set; }

        public bool IsSalesperson { get; set; }

        public string UserPreferences { get; set; }

        public string PhoneNumber { get; set; }

        public string FaxNumber { get; set; }

        public string EmailAddress { get; set; }

        public byte[] Photo { get; set; }

        public string CustomFields { get; set; }

        public string OtherLanguages { get; set; }

        public int LastEditedBy { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }

        public string Title { get; set; }
    }
}
