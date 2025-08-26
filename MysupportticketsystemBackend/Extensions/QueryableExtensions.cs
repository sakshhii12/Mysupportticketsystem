using MysupportticketsystemBackend.Models;
using System.Linq; // <-- Make sure you have this using statement

namespace MysupportticketsystemBackend.Extensions
{
    public static class QueryableExtensions
    {
        // Extension method to filter by status (no changes here)
        public static IQueryable<Ticket> WhereIfStatus(this IQueryable<Ticket> query, string? status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return query;
            }
            if (Enum.TryParse<TicketStatus>(status, true, out var statusEnum))
            {
                return query.Where(t => t.Status == statusEnum);
            }
            return query;
        }

        // Extension method to filter by priority (no changes here)
        public static IQueryable<Ticket> WhereIfPriority(this IQueryable<Ticket> query, string? priority)
        {
            if (string.IsNullOrEmpty(priority))
            {
                return query;
            }
            if (Enum.TryParse<TicketPriority>(priority, true, out var priorityEnum))
            {
                return query.Where(t => t.Priority == priorityEnum);
            }
            return query;
        }

        // Extension method to search the title and description (added the '!' null-forgiving operator)
        public static IQueryable<Ticket> Search(this IQueryable<Ticket> query, string? searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return query;
            }
            var lowerCaseTerm = searchTerm.Trim().ToLower();
            // Added '!' to Title and Description
            return query.Where(t => t.Title!.ToLower().Contains(lowerCaseTerm) ||
                                     t.Description!.ToLower().Contains(lowerCaseTerm));
        }

        // REPLACED the old generic sorting method with this new, specific one.
        public static IQueryable<Ticket> ApplySort(this IQueryable<Ticket> query, string? sortBy, string? sortOrder)
        {
            // If no sort criteria is provided, sort by the newest tickets by default.
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return query.OrderByDescending(t => t.CreatedAt);
            }

            bool isDescending = !string.IsNullOrEmpty(sortOrder) && sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase);

            // We use a switch statement to handle the allowed sort fields.
            switch (sortBy.ToLower())
            {
                case "priority":
                    query = isDescending ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority);
                    break;
                case "status":
                    query = isDescending ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status);
                    break;
                case "createdat":
                default: // If the sortBy value is anything else, we default to sorting by CreatedAt.
                    query = isDescending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt);
                    break;
            }

            return query;
        }
    }
}