using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security;
using StructureInterfaces.Annotations;

namespace StructureInterfaces
{
    public sealed class AddinSettings : INotifyPropertyChanged
    {
        private string structure;
        private string username;
        private SecureString password;
        private string jiraurl;
        private string jql;

        public String Structure
        {
            get { return structure; }
            set
            {
                structure = value;
                OnPropertyChanged();
            }
        }

        public String Username
        {
            get { return username; }
            set
            {
                username = value;
                OnPropertyChanged();     
            }
        }

        public SecureString Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged();      
            }
        }

        public string JIRAURL
        {
            get { return jiraurl; }
            set
            {
                jiraurl = value; 
                OnPropertyChanged();
            }
        }

        public string JQL
        {
            get { return jql; }
            set
            {
                jql = value;
                OnPropertyChanged();
            }
        }

        public bool SavePassword { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}