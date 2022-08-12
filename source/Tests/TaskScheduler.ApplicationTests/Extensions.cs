using System.Collections.Generic;

namespace TaskScheduler.Application.Tests
{
    static class Extensions
    {
        public static IEnumerable<(TLeft, TRight)> LeftZip<TLeft, TRight>(this IEnumerable<TLeft> leftZip, IEnumerable<TRight> rightZip)
        {
            TRight rightDefault = default;
            var rightEnumerator = rightZip.GetEnumerator();

            foreach(var left in leftZip)
            {
                if (rightEnumerator.MoveNext())
                    rightDefault = rightEnumerator.Current;

                yield return new(left, rightDefault);
            }
        }
        public static IEnumerable<(TLeft, TRight)> RightZip<TLeft, TRight>(this IEnumerable<TLeft> leftZip, IEnumerable<TRight> rightZip)
        {
            TLeft leftDefault = default;
            var leftEnumerator = leftZip.GetEnumerator();

            foreach (var right in rightZip)
            {
                if (leftEnumerator.MoveNext())
                    leftDefault = leftEnumerator.Current;

                yield return (leftDefault, right);
            }
        }
        public static IEnumerable<(TLeft, TRight)> FullZip<TLeft, TRight>(this IEnumerable<TLeft> leftZip, IEnumerable<TRight> rightZip)
        {
            TRight rightDefault = default;
            var rightEnumerator = rightZip.GetEnumerator();

            TLeft leftDefault = default;
            var leftEnumerator = leftZip.GetEnumerator();

            while (true)
            {
                bool rightMoveNext = rightEnumerator.MoveNext();
                bool leftMoveNext = leftEnumerator.MoveNext();

                if (!(rightMoveNext || leftMoveNext))
                    yield break;

                if (rightMoveNext)
                    rightDefault = rightEnumerator.Current;

                if (leftMoveNext)
                    leftDefault = leftEnumerator.Current;

                yield return (leftDefault, rightDefault);
            }
        }
    }
}
