using System.Threading.Tasks;

namespace ScriptService.Helpers {
    
    /// <summary>
    /// helper methods for tasks
    /// </summary>
    public class Tasks {
        
        /// <summary>
        /// awaits execution of a task and returns result if available
        /// </summary>
        /// <param name="task">task to await</param>
        /// <returns>result of task if available</returns>
        public static object AwaitTask(Task task) {
            if (task.Status == TaskStatus.Created)
                task.Start();

            task.Wait();
            
            if (!task.GetType().IsGenericType)
                return null;

            return task.GetType().GetProperty("Result")?.GetValue(task);
        }
    }
}