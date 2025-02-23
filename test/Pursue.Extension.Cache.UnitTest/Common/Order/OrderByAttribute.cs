using System;

namespace Pursue.Extension.Cache.UnitTest.Init
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class OrderByAttribute : Attribute
    {
        public int Order { get; private set; }

        public OrderByAttribute(int order)
        {
            Order = order;
        }
    }
}
