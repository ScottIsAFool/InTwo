namespace ScottIsAFool.WindowsPhone.ViewModel
{
    public abstract class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {
        protected ViewModelBase()
        {
            if (!IsInDesignMode)
            {
                WireMessages();
            }
        }

        protected abstract void WireMessages();

        public bool ProgressIsVisible { get; set; }
        public string ProgressText { get; set; }
    }
}
