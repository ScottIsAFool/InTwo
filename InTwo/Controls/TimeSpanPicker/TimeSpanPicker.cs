using System;
using System.Windows;
using Coding4Fun.Toolkit.Controls;

namespace InTwo.Controls.TimeSpanPicker
{
    public class TimeSpanPicker : ValuePickerBase<TimeSpan>
    {
        /// <summary>
        /// Initializes a new instance of the TimePicker control.
        /// </summary>
        public TimeSpanPicker()
        {
            DefaultStyleKey = typeof(TimeSpanPicker);

            Value = TimeSpan.FromSeconds(2);
            Max = TimeSpan.FromSeconds(30);
            Step = TimeSpan.FromSeconds(1);
			DialogTitle = "CHOOSE SECONDS TO GUESS IN";

        }

        /// <summary>
        /// Gets or sets the Max Value
        /// </summary>
        public TimeSpan Max
        {
            get { return (TimeSpan)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        /// <summary>
        /// Identifies the Max Property
        /// </summary>
        public static readonly DependencyProperty MaxProperty = DependencyProperty.Register(
            "Max", typeof(TimeSpan), typeof(ValuePickerBase<TimeSpan>), null);


        /// <summary>
        /// Gets or sets the Value Step
        /// </summary>
        public TimeSpan Step
        {
            get { return (TimeSpan)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        /// <summary>
        /// Identifies the Max Property
        /// </summary>
        public static readonly DependencyProperty StepProperty = DependencyProperty.Register(
            "Step", typeof(TimeSpan), typeof(ValuePickerBase<TimeSpan>), null);

        /// <summary>
        /// Initializes Value, Max, Step when vanigating to the new page
        /// </summary>
        /// <param name="page"></param>
        protected override void NavigateToNewPage(object page)
        {
            var tsPage = page as InTwo.Controls.TimeSpanPicker.ITimeSpanPickerPage<TimeSpan>;

            if (tsPage != null)
            {
                tsPage.Max = Max;
                tsPage.IncrementStep = Step;
            }

            base.NavigateToNewPage(page);
        }


    }
}
