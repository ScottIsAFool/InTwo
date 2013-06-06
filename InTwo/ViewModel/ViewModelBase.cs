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

        public virtual void WireMessages()
        {
            
        }

        public void SetProgressBar(string text)
        {
            ProgressIsVisible = true;
            ProgressText = text;
        }

        public void SetProgressBar()
        {
            ProgressIsVisible = false;
            ProgressText = string.Empty;
        }

        public bool ProgressIsVisible { get; set; }
        public string ProgressText { get; set; }
    }
}
