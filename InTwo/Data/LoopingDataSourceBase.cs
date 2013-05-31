using System;
using System.Windows.Controls;
using Coding4Fun.Toolkit.Controls.Primitives;

namespace InTwo.Data
{
    public abstract class LoopingDataSourceBase : ILoopingSelectorDataSource
    {
        private object _selectedItem;

        #region ILoopingSelectorDataSource Members

        public abstract object GetNext(object relativeTo);

        public abstract object GetPrevious(object relativeTo);

        public object SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                // this will use the Equals method if it is overridden for the data source item class
                if (!Equals(_selectedItem, value))
                {
                    // save the previously selected item so that we can use it 
                    // to construct the event arguments for the SelectionChanged event
                    object previousSelectedItem = _selectedItem;
                    _selectedItem = value;
                    // fire the SelectionChanged event
                    OnSelectionChanged(previousSelectedItem, _selectedItem);
                }
            }
        }

        public bool IsEmpty { get; private set; }
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        protected virtual void OnSelectionChanged(object oldSelectedItem, object newSelectedItem)
        {
            EventHandler<SelectionChangedEventArgs> handler = SelectionChanged;
            if (handler != null)
            {
                handler(this, new SelectionChangedEventArgs(new[] { oldSelectedItem }, new[] { newSelectedItem }));
            }
        }

        #endregion
    }
}