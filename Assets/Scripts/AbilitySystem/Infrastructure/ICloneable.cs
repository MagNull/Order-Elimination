﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.Infrastructure
{
    public interface ICloneable<T> : ICloneable
    {
        public int CloneId { get; }
        public new T Clone();
        object ICloneable.Clone() => Clone();
    }
}
