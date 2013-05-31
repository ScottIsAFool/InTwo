using System.Collections.Generic;
using System.Windows;
using Coding4Fun.Toolkit.Controls.Primitives;

namespace InTwo.Controls
{
    public class SecondsPickerBasePage : ValuePickerBasePage<int>
    {
        protected override IEnumerable<LoopingSelector> GetSelectorsOrderedByCulturePattern()
        {
            return default(List<LoopingSelector>);
        }

        protected override ValueWrapper<int> GetNewWrapper(int? value)
        {
            return default(ValueWrapper<int>);
        }

        public override void InitDataSource()
        {
        }

        public override void SetFlowDirection(FlowDirection flowDirection)
        {
        }
    }
}