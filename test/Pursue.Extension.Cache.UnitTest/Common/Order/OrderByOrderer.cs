using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Pursue.Extension.Cache.UnitTest.Init.Order
{
    public class OrderByOrderer : ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
        {
            var assemblyName = typeof(OrderByAttribute).AssemblyQualifiedName;
            var sortedMethods = new SortedDictionary<int, List<TTestCase>>();

            foreach (TTestCase testCase in testCases)
            {
                int priority = testCase.TestMethod.Method.GetCustomAttributes(assemblyName).FirstOrDefault()?.GetNamedArgument<int>(nameof(OrderByAttribute.Order)) ?? 0;

                GetOrCreate(sortedMethods, priority).Add(testCase);
            }

            foreach (TTestCase testCase in sortedMethods.Keys.SelectMany(o => sortedMethods[o].OrderBy(testCase => testCase.TestMethod.Method.Name)))
            {
                yield return testCase;
            }
        }

        private static TValue GetOrCreate<TKey, TValue>(SortedDictionary<TKey, TValue> dictionary, TKey key) where TKey : struct where TValue : new()
        {
            return dictionary.TryGetValue(key, out TValue result) ? result : (dictionary[key] = new TValue());
        }
    }
}