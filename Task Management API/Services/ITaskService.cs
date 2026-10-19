using Task_Management_API.Entities;

namespace Task_Management_API.Services
{
    public interface ITaskService
    {
        Task<List<TaskItem>> GetAll(CancellationToken cancellationToken);
        Task<TaskItem?> GetById(int id, CancellationToken cancellationToken);
        Task<TaskItem> Create(TaskItem task, CancellationToken cancellationToken);
        Task<bool> Update(int id, TaskItem task, CancellationToken cancellationToken);
        Task<bool> Delete(int id, CancellationToken cancellationToken);
    }
}
