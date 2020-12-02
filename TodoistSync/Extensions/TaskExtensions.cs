using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoistSync.Extensions
{
    public static class TaskExtensions
    {
        public static async Task<List<T>> ToListAsync<T>(this Task<IEnumerable<T>> enumerableTask)
        {
            var enumerable = await enumerableTask;
            return enumerable.ToList();
        }
    }
}