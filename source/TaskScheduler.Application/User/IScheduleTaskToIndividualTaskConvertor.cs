using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskScheduler.Domain;

namespace TaskScheduler.Application
{
    public interface IScheduleTaskToIndividualTaskConvertor
    {
        IEnumerable<IndividualTask> ToIndividualTasks(IEnumerable<ScheduleTask> scheduleTasks, DateTime dateStart, DateTime dateEnd);
    }
}
