using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeConnect.GeneratorPreview.View
{
    public class PreviewWindowViewModel : INotifyPropertyChanged, ISetGeneratorName, ISetTargetName, ISetGeneratedCode, IShowAll
    {
        #region INotifyPropertyChanged stuff

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        private string _generatorName;
        public string GeneratorName
        {
            get
            {
                return _generatorName;
            }
            set
            {
                if (value != _generatorName)
                {
                    _generatorName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _targetName;
        public string TargetName
        {
            get
            {
                return _targetName;
            }
            set
            {
                if (value != _targetName)
                {
                    _targetName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _generatedCode;
        public string GeneratedCode
        {
            get
            {
                return _generatedCode;
            }
            set
            {
                if (value != _generatedCode)
                {
                    _generatedCode = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _errors;
        public string Errors
        {
            get
            {
                return _errors;
            }
            set
            {
                if (value != _errors)
                {
                    _errors = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
