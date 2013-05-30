namespace ScottIsAFool.WindowsPhone.ViewModel
{
    public abstract class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {
        public bool ProgressIsVisible { get; set; }
        public string ProgressText { get; set; }
    }
}
