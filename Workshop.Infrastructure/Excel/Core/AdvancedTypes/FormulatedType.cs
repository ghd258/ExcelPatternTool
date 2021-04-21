﻿namespace Workshop.Infrastructure.Core
{

    public class FormulatedType<T> : IFormulatedType
    {
        public T Value { get; set; }
        public string Formula { get; set; }

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