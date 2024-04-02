using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3_chart.viewmodel
{
    public class ErrorsViewModel : INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> propertyErrors = new Dictionary<string, List<string>>();

        public bool HasErrors => propertyErrors.Any();

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            return propertyErrors.GetValueOrDefault(propertyName, null);
        }

        public void AddError(string propertyName, string value)
        {
            if (!propertyErrors.ContainsKey(propertyName))
            {
                propertyErrors.Add(propertyName, new List<string>());
            }

            propertyErrors[propertyName].Add(value);
            OnErrorsChanged(propertyName);
        }

        public void ClearErrors(string? propertyName)
        {
            if (propertyErrors.Remove(propertyName))
            {
                OnErrorsChanged(propertyName);
            }
        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
