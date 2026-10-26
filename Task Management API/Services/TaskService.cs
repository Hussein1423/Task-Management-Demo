using Microsoft.EntityFrameworkCore;
using Task_Management_API.Entities;
using Task_Management_API.Events;
using Task_Management_API.Messaging;

namespace Task_Management_API.Services
{
    public class TaskService : ITaskService
    {

        private readonly TaskDbContext _context;
        private readonly KafkaProducer _kafkaProducer;

        public TaskService(
            TaskDbContext context,
            KafkaProducer kafkaProducer)
        {
            _context = context;
            _kafkaProducer = kafkaProducer;
        }

        public async Task<List<TaskItem>> GetAll(CancellationToken cancellationToken)
        {
            var tasks = await _context.Tasks
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return tasks;
        }

            public async Task<TaskItem?> GetById(int id, CancellationToken cancellationToken)
            {
                return await _context.Tasks
                    .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            }

        public async Task<TaskItem> Create(
 TaskItem task,
 CancellationToken cancellationToken)
        {
            await _context.Tasks.AddAsync(task, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var taskCreatedEvent = new TaskCreatedEvent
            {
                TaskId = task.Id,
                Title = task.Title
            };

            await _kafkaProducer.PublishAsync(
                task.Id.ToString(),
                taskCreatedEvent,
                cancellationToken);

            return task;
        }


        public async Task<bool> Update(
            int id,
            TaskItem task,
            CancellationToken cancellationToken)
        {
            var existing = await GetById(id, cancellationToken);

            if (existing is null)
                return false;

            var oldData = new TaskItem
            {
                Id = existing.Id,
                Title = existing.Title,
                IsCompleted = existing.IsCompleted,
                Description = existing.Description
            };

            existing.Title = task.Title;
            existing.IsCompleted = task.IsCompleted;
            existing.Description = task.Description;

            await _context.SaveChangesAsync(cancellationToken);

            var taskUpdatedEvent = new TaskUpdatedEvent
            {
                OldData = oldData,
                NewData = new TaskItem
                {
                    Id = existing.Id,
                    Title = existing.Title,
                    IsCompleted = existing.IsCompleted,
                    Description = existing.Description
                }
            };

            await _kafkaProducer.PublishAsync(
                existing.Id.ToString(),
                taskUpdatedEvent,
                cancellationToken);

            return true;
        }


        public async Task<bool> Delete(int id, CancellationToken cancellationToken)
            {
                var task = await GetById(id, cancellationToken);

                if (task is null)
                    return false;

                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
    }
}
