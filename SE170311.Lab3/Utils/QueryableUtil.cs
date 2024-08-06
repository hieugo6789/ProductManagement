using System.Linq.Expressions;

namespace SE170311.Lab3.Utils
{
    public static class QueryableUtil
    {
        public static IOrderedQueryable<T> OrderByMultipleFields<T>(this IQueryable<T> query, List<(string field, bool isDescending)> orderByFields)
        {
            if (!orderByFields.Any()) return (IOrderedQueryable<T>)query;

            var firstOrderBy = orderByFields[0];
            var orderedQuery = ApplyOrdering(query, firstOrderBy.field, firstOrderBy.isDescending);

            for (int i = 1; i < orderByFields.Count; i++)
            {
                var orderByField = orderByFields[i];
                orderedQuery = ApplyThenOrdering(orderedQuery, orderByField.field, orderByField.isDescending);
            }

            return orderedQuery;
        }

        private static IOrderedQueryable<T> ApplyOrdering<T>(IQueryable<T> query, string orderByField, bool isDescending)
        {
            // Tạo một parameter expression đại diện cho một tham số của kiểu T (p)
            var parameter = Expression.Parameter(typeof(T), "p");
            // Tạo một expression đại diện cho việc truy cập thuộc tính (property) của đối tượng T
            var property = Expression.Property(parameter, orderByField);
            // Tạo một biểu thức Lambda đại diện cho việc lấy giá trị của thuộc tính đó (p => p.OrderByField)
            var lambda = Expression.Lambda(property, parameter);

            var methodName = isDescending ? "OrderByDescending" : "OrderBy";
            // Lấy phương thức sắp xếp tương ứng từ lớp Queryable và tạo một generic method
            var method = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), property.Type);

            // Lấy phương thức sắp xếp tương ứng từ lớp Queryable và tạo một generic method
            return (IOrderedQueryable<T>)method.Invoke(null, new object[] { query, lambda });
        }

        private static IOrderedQueryable<T> ApplyThenOrdering<T>(IOrderedQueryable<T> query, string orderByField, bool isDescending)
        {
            var parameter = Expression.Parameter(typeof(T), "p");
            var property = Expression.Property(parameter, orderByField);
            var lambda = Expression.Lambda(property, parameter);

            var methodName = isDescending ? "ThenByDescending" : "ThenBy";
            var method = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), property.Type);

            return (IOrderedQueryable<T>)method.Invoke(null, new object[] { query, lambda });
        }
    }
}
