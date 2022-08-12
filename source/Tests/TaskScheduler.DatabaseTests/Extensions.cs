using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskScheduler.Domain;

namespace TaskScheduler.Database.Tests
{
    static class Extensions
    {
        /// <summary>
        /// <para>Database contains 10 user with UserId = {1-10}, Login = "user{1-10}", Password = "Password{1-10}"</para>
        /// 
        /// <para>Every user owner of 12 tasks, which is indicated by "TaskNum". Each task has: <br/>
        ///        TaskId      = {OwnerId}{TaskNum} <br/>
        ///        Link        = "Task{TaskId}",    <br/>
        ///        Start Date  = 18:00 {OwnerId}:{TaskNum}:2022     <br/>
        ///        End Date    = 19:00 {OwnerId}:{TaskNum}:2022     <br/>
        ///        Is Repetitive   = ({OwnerId} + {TaskNum}) % 2 == 0   <br/>
        ///        Repetitive Peritor  = 1 day                          <br/>
        ///        Repetitive Duration = whole {TaskNum} month          <br/>
        ///       Participated user is every user which ({OwnerId} + {TaskNum}) % {UserId} == 0 </para>
        ///       </summary>
        public static void SetDatabaseContext(this TaskSchedulerContext context, string tasksJsonFname)
        {
            context.ScheduleTasks.AddRange(GeneratedDbCommon.GenerateDbByTasks());
            context.SaveChanges();
        }

        public static bool IsEquivalent(this ScheduleTask scheduleTask, ScheduleTask compareWith)
        {
            if (scheduleTask.TaskId != compareWith.TaskId)
                return false;

            if (scheduleTask.Description != compareWith.Description)
                return false;

            if (scheduleTask.Title != compareWith.Title)
                return false;

            if (scheduleTask.IsRepetitive != compareWith.IsRepetitive)
                return false;

            if (scheduleTask.Link != compareWith.Link)
                return false;

            if (scheduleTask.Owner?.LoginAuth?.Login != compareWith.Owner?.LoginAuth?.Login)
                return false;

            if (scheduleTask.RepetitiveEnd != compareWith.RepetitiveEnd)
                return false;

            if (scheduleTask.RepetitiveStart != compareWith.RepetitiveStart)
                return false;

            if (scheduleTask.IsRepetitive != compareWith.IsRepetitive)
                return false;

            if (scheduleTask.TaskPriority != compareWith.TaskPriority)
                return false;

            if (scheduleTask.TaskStart != compareWith.TaskStart)
                return false;

            if (scheduleTask.TaskEnd != compareWith.TaskEnd)
                return false;

            return scheduleTask.Participants?.Count == compareWith.Participants?.Count;
        }
        public static IEnumerable<(TLeft, TRight)> LeftZip<TLeft, TRight>(this IEnumerable<TLeft> leftZip, IEnumerable<TRight> rightZip)
        {
            TRight rightDefault = default;
            var rightEnumerator = rightZip.GetEnumerator();

            foreach (var left in leftZip)
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
