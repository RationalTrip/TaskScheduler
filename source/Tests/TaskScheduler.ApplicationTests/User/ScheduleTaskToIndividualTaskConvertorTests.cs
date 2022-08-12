using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskScheduler.Domain;

namespace TaskScheduler.Application.Tests
{
    [TestClass()]
    public class ScheduleTaskToIndividualTaskConvertorTests
    {
        [TestMethod()]
        [DataTestMethod()]
        [DynamicData(nameof(ToIndividualTasksArgs), DynamicDataSourceType.Method)]
        public void ToIndividualTasksTest(IEnumerable<ScheduleTask> scheduleTasks, DateTime dateStart, DateTime dateEnd, IEnumerable<IndividualTask> expectedResult)
        {
            //arrange
            IScheduleTaskToIndividualTaskConvertor taskConvertor = new ScheduleTaskToIndividualTaskConvertor();

            //act
            var actual = taskConvertor.ToIndividualTasks(scheduleTasks, dateStart, dateEnd);

            //assert
            CollectionAssert.AreEquivalent(expectedResult.ToArray(), actual.ToArray());
        }

        static IEnumerable<object> ToIndividualTasksArgs() =>
            ScheduleTaskToIndividualTaskConvertorTestData.GetRepetitiveIndividualTasksArgs()
            .Concat(ScheduleTaskToIndividualTaskConvertorTestData.GetRepetitiveAndNonRepetitiveIndividualTasksArgs())
            .Concat(ScheduleTaskToIndividualTaskConvertorTestData.GetNonRepetitiveIndividualTasksArgs());
    }
}