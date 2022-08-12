using FluentValidation;

namespace TaskScheduler.Model
{
    public class ScheduleTaskCreateModelValidator:AbstractValidator<ScheduleTaskCreateModel>
    {
        public ScheduleTaskCreateModelValidator()
        {
            RuleFor(task => task.Title).NotEmpty()
                .WithMessage("Title must not be empty.");
            RuleFor(task => task.Title).MaximumLength(255)
                .WithMessage("Title maximum Lenght is {MaxLength}.");

            RuleFor(task => task.TaskStart).LessThan(task => task.TaskEnd)
                .WithMessage("Tasks end must be after task start.");
            RuleFor(task => task.TaskStart).LessThanOrEqualTo(task => task.RepetitiveEnd)
                .When(task => task.IsRepetitive)
                .WithMessage("Task start should be betwen repetitive start and repetitive end. (Now Task starts before repetetive starts).");

            RuleFor(task => task.TaskPriority).IsInEnum()
                .WithMessage("Inappropiate Task Priority value.");

            RuleFor(task => task.RepetitivePeriod).IsInEnum()
                .When(task => task.IsRepetitive)
                .WithMessage("Inappropiate Task Period value.");
        }
    }
}
