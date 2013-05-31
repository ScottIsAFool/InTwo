using System.Collections.Generic;
using System.Windows;
using Coding4Fun.Toolkit.Controls.Primitives;

namespace InTwo.Controls
{
    public partial class SecondsPicker : SecondsPickerBasePage
    {
        public SecondsPicker()
        {
            InitializeComponent();
            
        }
    }

    public class SecondsPickerBasePage : ValuePickerBasePage<int>
    {
        protected override IEnumerable<LoopingSelector> GetSelectorsOrderedByCulturePattern()
        {
        }

        protected override ValueWrapper<int> GetNewWrapper(int? value)
        {
        }

        public override void InitDataSource()
        {
        }

        public override void SetFlowDirection(FlowDirection flowDirection)
        {
        }
    }
}
