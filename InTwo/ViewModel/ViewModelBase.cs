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

        public void SetProgressBar(string text = "", bool isVisible = true)
        {
            ProgressIsVisible = isVisible;
            ProgressText = text;
        }

        public bool ProgressIsVisible { get; set; }
        public string ProgressText { get; set; }
    }
}
