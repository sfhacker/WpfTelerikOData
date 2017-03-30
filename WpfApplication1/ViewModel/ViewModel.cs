
using System.Collections.Generic;
using System.ComponentModel;

namespace WpfApplication1.ViewModel
{
    public class ViewModel : INotifyPropertyChanged
    {
        private string queryName;
        private List<string> queryNames;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public string QueryName
        {
            get
            {
                return this.queryName;
            }
            set
            {
                if (this.queryName != value)
                {
                    this.queryName = value;
                    this.OnPropertyChanged("QueryName");
                }
            }
        }

        public List<string> QueryNames
        {
            get
            {
                if (this.queryNames == null)
                {
                    this.queryNames = new List<string>()
                                        {
                                            "Customers", "Employees", "Orders", "Products"
                                        };
                }

                return this.queryNames;
            }
        }
    }
}
