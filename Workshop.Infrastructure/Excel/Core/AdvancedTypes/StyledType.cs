﻿using Workshop.Infrastructure.Models;

namespace Workshop.Infrastructure.Core
{
    public class StyledType<T> : IStyledType
    {
        public T Value { get; set; }
        public string Comment { get; set; }
        public StyleMetadata StyleMetadata { get; set; }
        public override string ToString()
        {
            return Value.ToString();
        }
        public object GetValue()
        {
            return Value;
        }

        public void SetValue(object value)
        {
            Value = (T)value;
        }
    }
}