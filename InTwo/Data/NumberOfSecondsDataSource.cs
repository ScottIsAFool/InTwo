using System;

namespace InTwo.Data
{
    public class NumberOfSecondsDataSource : LoopingDataSourceBase
    {
        private int _minValue;
        private int _maxValue;
        private int _increment;

        public NumberOfSecondsDataSource()
        {
            MaxValue = 30;
            MinValue = 1;
            Increment = 1;
            SelectedItem = 2;
        }

        public int MinValue
        {
            get
            {
                return _minValue;
            }
            set
            {
                if (value >= MaxValue)
                {
                    throw new ArgumentOutOfRangeException("value", "MinValue cannot be equal or greater than MaxValue");
                }
                _minValue = value;
            }
        }

        public int MaxValue
        {
            get
            {
                return _maxValue;
            }
            set
            {
                if (value <= MinValue)
                {
                    throw new ArgumentOutOfRangeException("value", "MaxValue cannot be equal or lower than MinValue");
                }
                _maxValue = value;
            }
        }

        public int Increment
        {
            get
            {
                return _increment;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("value", "Increment cannot be less than or equal to zero");
                }
                _increment = value;
            }
        }

        public override object GetNext(object relativeTo)
        {
            int nextValue = (int)relativeTo + Increment;
            if (nextValue > MaxValue)
            {
                nextValue = MinValue;
            }
            return nextValue;
        }

        public override object GetPrevious(object relativeTo)
        {
            int prevValue = (int)relativeTo - Increment;
            if (prevValue < MinValue)
            {
                prevValue = MaxValue;
            }
            return prevValue;
        }
    }
}
