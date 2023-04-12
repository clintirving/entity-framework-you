// // -----------------------------------------------------------------------
// // <copyright file="FilterServiceOfT.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using EfYou.DatabaseContext;
using EfYou.Extensions;
using EfYou.Model.Attributes;
using EfYou.Model.FilterExtensions;
using EfYou.Utilities;

namespace EfYou.Filters
{
    public class FilterService<T> : IFilterService<T> where T : class, new()
    {
        private const string OrderByDescending = " DESCENDING";
        private const string OrderBySeparator = ", ";

        public IQueryable<T> FilterResultsOnGet(IQueryable<T> query, List<dynamic> ids, IContext context)
        {
            return FilterResultsOnIdsFilter(query, ids, context);
        }
        
        public IQueryable<T> FilterResultsOnSearch(IQueryable<T> query, T filter, IContext context)
        {
            return AutoFilter(query, filter, context);
        }

        public virtual IQueryable<T> AddIncludes(IQueryable<T> query, List<string> includes, IContext context)
        {
            if (includes != null)
            {
                IQueryable<T> result = query;
                foreach (var include in includes)
                {
                    result = result.Include(include);
                }

                return result;
            }

            return query;
        }

        public virtual IQueryable<T> AddOrderBys(IQueryable<T> query, List<OrderBy> orderBys, IContext context)
        {
            if (orderBys != null && orderBys.Count != 0)
            {
                var ordering = string.Join(OrderBySeparator,
                    orderBys.Select(x => x.Descending ? x.ColumnName + OrderByDescending : x.ColumnName));
                return query.OrderBy(ordering);
            }

            return query.OrderBy(typeof(T).GetPrimaryKeyProperty().Name);
        }

        public virtual IQueryable<T> AddPaging(IQueryable<T> query, Paging paging, IContext context)
        {
            if (paging != null)
            {
                return query.Skip(paging.Count * paging.Page).Take(paging.Count);
            }

            return query;
        }

        public virtual IQueryable<IGrouping<long, List<long>>> AddPaging(IQueryable<IGrouping<long, List<long>>> query, Paging paging)
        {
            if (paging != null)
            {
                return query.Skip(paging.Count * paging.Page).Take(paging.Count);
            }

            return query;
        }

        public virtual IQueryable<IGrouping<long, List<long>>> AddAggregationFilter(IQueryable<T> query, List<string> groupBys, Paging paging,
            List<OrderBy> orderBys)
        {
            var groupByType = CreateAnonymousType(groupBys);
            var groupByTypeConstructor = groupByType.GetConstructors().First();
            var groupByTypeProperties = groupByType.GetProperties();

            var keySelectorLambda = CreateKeySelectorFunctionForGroupBy(groupByTypeProperties, groupByTypeConstructor);
            var resultSelectorLambda = CreateResultSelectorFunctionForGroupBy();
            var groupedQuery = ApplyGroupingToResultSet(query, paging, keySelectorLambda, resultSelectorLambda);
            var orderedQuery = ApplyOrderByToResultSet(groupedQuery, orderBys);
            var pagedQuery = AddPaging(orderedQuery, paging);

            return pagedQuery;
        }

        protected virtual IQueryable<T> FilterResultsOnIdsFilter(IQueryable<T> query, List<dynamic> ids, IContext context)
        {
            var primaryKeyProperty = typeof(T).GetPrimaryKeyProperty();

            var primaryKeyType = primaryKeyProperty.PropertyType;

            var primaryKeyName = primaryKeyProperty.Name;

            var containsQuery = string.Format("{0} in @0", primaryKeyName);

            if (primaryKeyType == typeof(short))
            {
                return query.Where(containsQuery, ids.Select(x => (short)x).ToList());
            }
            if (primaryKeyType == typeof(int))
            {
                return query.Where(containsQuery, ids.Select(x => (int)x).ToList());
            }
            if (primaryKeyType == typeof(long))
            {
                return query.Where(containsQuery, ids.Select(x => (long)x).ToList());
            }
            if (primaryKeyType == typeof(Guid))
            {
                return query.Where(containsQuery, ids.Select(x => (Guid)x).ToList());
            }

            throw new ApplicationException("To call this method, Primary Key of type T must be one of Int16, Int32, Int64, Guid");
        }

        private Type CreateAnonymousType(List<string> groupBys)
        {
            var anonymousClassService = new AnonymousClassService();
            var entityProperties = typeof(T).GetProperties().Where(x => groupBys.Contains(x.Name)).ToList();

            return anonymousClassService.CreateAnonymousType(entityProperties);
        }

        private Expression<Func<T, object>> CreateKeySelectorFunctionForGroupBy(PropertyInfo[] reducedFilterTypeProperties,
            ConstructorInfo reducedFilterTypeConstructor)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var arguments = new List<MemberExpression>();

            foreach (var reducedFilterTypePropertyInfo in reducedFilterTypeProperties)
            {
                arguments.Add(Expression.PropertyOrField(parameter, reducedFilterTypePropertyInfo.Name));
            }

            var function = Expression.New(reducedFilterTypeConstructor, arguments, reducedFilterTypeProperties);
            return Expression.Lambda<Func<T, object>>(function, parameter);
        }

        private Expression<Func<T, long>> CreateResultSelectorFunctionForGroupBy()
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var argument = Expression.PropertyOrField(parameter, "Id");
            var convert = Expression.Convert(argument, typeof(long));

            return Expression.Lambda<Func<T, long>>(convert, parameter);
        }


        private IQueryable<IGrouping<long, List<long>>> ApplyGroupingToResultSet(IQueryable<T> query, Paging paging,
            Expression<Func<T, object>> keySelectorLambda, Expression<Func<T, long>> resultSelectorLambda)
        {
            var groupedQuery = query.GroupBy(keySelectorLambda, resultSelectorLambda);
            return groupedQuery.GroupBy(x => x.Max(), x => x.OrderByDescending(y => y).ToList());
        }

        private IQueryable<IGrouping<long, List<long>>> ApplyOrderByToResultSet(IQueryable<IGrouping<long, List<long>>> query, List<OrderBy> orderBys)
        {
            var orderById = orderBys.FirstOrDefault(x => x.ColumnName == "Id");

            if (orderById != null && orderById.Descending)
            {
                return query.OrderByDescending(x => x.Key);
            }

            return query.OrderBy(x => x.Key);
        }

        public virtual IQueryable<T> AutoFilter(IQueryable<T> query, T filter, IContext context)
        {
            var filterType = filter.GetType();

            var filterProperties = filterType.GetDbProperties();

            foreach (var filterProperty in filterProperties)
            {
                var propertyType = filterProperty.PropertyType;

                if (propertyType.IsNumericOrEnumType())
                {
                    query = AddNumericFilterToQuery(query, filter, filterProperty);
                }
                else if (propertyType.IsDateType())
                {
                    query = AddDateFilterToQuery(query, filter, filterProperty);
                }
                else if (propertyType.IsStringType())
                {
                    query = AddStringFilterToQuery(query, filter, filterProperty);
                }
                else if (propertyType.IsNullableType())
                {
                    query = AddNullableFilterToQuery(query, filter, filterProperty);
                }
                else
                {
                    query = AddFilterToQuery(query, filter, filterProperty);
                }
            }

            query = AddFilterExtensionsToQuery(query, filter, filterType, filter);

            return query;
        }

        private IQueryable<T> AddFilterExtensionsToQuery(IQueryable<T> query, T filter, Type type, object filterProperty)
        {
            var filterExtensionProperties = type.GetProperties().Where(x => Attribute.IsDefined(x, typeof(FilterExtensionsAttribute)));

            foreach (var filterExtensionProperty in filterExtensionProperties)
            {
                var currentFilterProperty = filterExtensionProperty.GetValue(filterProperty);

                if (currentFilterProperty != null)
                {
                    var filterExtensionAttribute = filterExtensionProperty.GetCustomAttribute<FilterExtensionsAttribute>();

                    if (filterExtensionAttribute?.AppliedToProperty != null)
                    {
                        if (typeof(DateTimeRange).IsAssignableFrom(filterExtensionProperty.PropertyType))
                        {
                            query = AddDateTimeRangeToQuery(query, filter, currentFilterProperty, filterExtensionAttribute);
                        }
                        else if (typeof(NumberRange).IsAssignableFrom(filterExtensionProperty.PropertyType))
                        {
                            query = AddNumberRangeToQuery(query, filter, currentFilterProperty, filterExtensionAttribute);
                        }
                        else if (typeof(TimeSpanRange).IsAssignableFrom(filterExtensionProperty.PropertyType))
                        {
                            query = AddTimeSpanRangeToQuery(query, filter, currentFilterProperty, filterExtensionAttribute);
                        }
                        else if (filterExtensionProperty.PropertyType.IsGenericType &&
                                 filterExtensionProperty.PropertyType.GetGenericTypeDefinition() == typeof(CollectionContains<>))
                        {
                                query = AddCollectionContainsToQuery(query, filter, currentFilterProperty, filterExtensionAttribute);
                        }
                        else if (filterExtensionProperty.PropertyType.IsGenericType &&
                                 filterExtensionProperty.PropertyType.GetGenericTypeDefinition() == typeof(ListContains<>))
                        {
                            query = AddListContainsToQuery(query, filter, currentFilterProperty, filterExtensionAttribute);
                        }
                    }

                    query = AddFilterExtensionsToQuery(query, filter, filterExtensionProperty.PropertyType, currentFilterProperty);
                }
            }

            return query;
        }

        private IQueryable<T> AddCollectionContainsToQuery(IQueryable<T> query, T filter, object currentFilterProperty,
            FilterExtensionsAttribute filterExtensionAttribute)
        {
            var collection = (ICollection)currentFilterProperty;
            if (collection != null && collection.Count > 0)
            {
                var property = filter.GetType().GetProperty(filterExtensionAttribute.AppliedToProperty);
                query = ApplyCollectionContains(query, property, currentFilterProperty);
            }

            return query;
        }

        private IQueryable<T> AddListContainsToQuery(IQueryable<T> query, T filter, object currentFilterProperty,
            FilterExtensionsAttribute filterExtensionAttribute)
        {
            var list = (IList)currentFilterProperty;
            if (list != null && list.Count > 0)
            {
                var property = filter.GetType().GetProperty(filterExtensionAttribute.AppliedToProperty);
                query = ApplyListContains(query, property, currentFilterProperty);
            }

            return query;
        }

        private IQueryable<T> AddNumberRangeToQuery(IQueryable<T> query, T filter, object currentFilterProperty,
            FilterExtensionsAttribute filterExtensionAttribute)
        {
            var range = (NumberRange) currentFilterProperty;

            var property = filter.GetType().GetProperty(filterExtensionAttribute.AppliedToProperty);

            var propertyType = property.PropertyType;

            if (propertyType.IsNullableNumericOrEnumType())
            {
                var underlyingPropertyType = Nullable.GetUnderlyingType(property.PropertyType);

                if (underlyingPropertyType.IsEnum)
                {
                    query = ApplyGreaterThanOrEqualFilterToQuery(query, property,
                        Enum.ToObject(underlyingPropertyType, Convert.ToInt32(Math.Max(range.Min, int.MinValue))));
                    query = ApplyLessThanOrEqualFilterToQuery(query, property,
                        Enum.ToObject(underlyingPropertyType, Convert.ToInt32(Math.Min(range.Max, int.MaxValue))));
                }
                else
                {
                    query = ApplyGreaterThanOrEqualFilterToQuery(query, property,
                        Convert.ChangeType(range.Min, underlyingPropertyType));
                    query = ApplyLessThanOrEqualFilterToQuery(query, property,
                        Convert.ChangeType(range.Max, underlyingPropertyType));
                }
            }
            else
            {
                if (propertyType.IsEnum)
                {
                    query = ApplyGreaterThanOrEqualFilterToQuery(query, property,
                        Enum.ToObject(propertyType, Convert.ToInt32(Math.Max(range.Min, int.MinValue))));
                    query = ApplyLessThanOrEqualFilterToQuery(query, property,
                        Enum.ToObject(propertyType, Convert.ToInt32(Math.Min(range.Max, int.MaxValue))));
                }
                else
                {
                    query = ApplyGreaterThanOrEqualFilterToQuery(query, property, Convert.ChangeType(range.Min, propertyType));
                    query = ApplyLessThanOrEqualFilterToQuery(query, property, Convert.ChangeType(range.Max, propertyType));
                }
            }

            return query;
        }

        private IQueryable<T> AddTimeSpanRangeToQuery(IQueryable<T> query, T filter, object currentFilterProperty,
            FilterExtensionsAttribute filterExtensionAttribute)
        {
            var range = (TimeSpanRange) currentFilterProperty;

            var property = filter.GetType().GetProperty(filterExtensionAttribute.AppliedToProperty);

            if (property.PropertyType.IsNullableType())
            {
                // Only do this for nullable TimeSpan types. There's no "nice" way to represent a TimeSpan
                // as a don't care condition without it being nullable, so even if we had a range filter property
                // it would just use the exact value property in the filter every time anyway.

                query = ApplyGreaterThanOrEqualFilterToQuery(query, property,
                    Convert.ChangeType(range.Min, Nullable.GetUnderlyingType(property.PropertyType)));
                query = ApplyLessThanOrEqualFilterToQuery(query, property,
                    Convert.ChangeType(range.Max, Nullable.GetUnderlyingType(property.PropertyType)));
            }

            return query;
        }

        private IQueryable<T> AddDateTimeRangeToQuery(IQueryable<T> query, T filter, object currentFilterProperty,
            FilterExtensionsAttribute filterExtensionAttribute)
        {
            var dateTimeRange = (DateTimeRange) currentFilterProperty;

            var property = filter.GetType().GetProperty(filterExtensionAttribute.AppliedToProperty);

            if (property.PropertyType.IsNullableType())
            {
                query = ApplyGreaterThanOrEqualFilterToQuery(query, property,
                    Convert.ChangeType(dateTimeRange.After, Nullable.GetUnderlyingType(property.PropertyType)));
                query = ApplyLessThanOrEqualFilterToQuery(query, property,
                    Convert.ChangeType(dateTimeRange.Before, Nullable.GetUnderlyingType(property.PropertyType)));
            }
            else
            {
                query = ApplyGreaterThanOrEqualFilterToQuery(query, property, dateTimeRange.After);
                query = ApplyLessThanOrEqualFilterToQuery(query, property, dateTimeRange.Before);
            }

            return query;
        }

        private IQueryable<T> AddFilterToQuery(IQueryable<T> query, T filter, PropertyInfo filterProperty)
        {
            var propertyValue = filterProperty.GetValue(filter);
            query = ApplyEqualityFilterToQuery(query, filterProperty, propertyValue);
            return query;
        }

        private IQueryable<T> AddNullableFilterToQuery(IQueryable<T> query, T filter, PropertyInfo filterProperty)
        {
            var propertyValue = filterProperty.GetValue(filter);
            if (propertyValue != null)
            {
                query = ApplyEqualityFilterToQuery(query, filterProperty, propertyValue);
            }

            return query;
        }

        private IQueryable<T> AddStringFilterToQuery(IQueryable<T> query, T filter, PropertyInfo filterProperty)
        {
            var propertyValue = (string) filterProperty.GetValue(filter);
            if (!string.IsNullOrEmpty(propertyValue))
            {
                var filterAttribute = filterProperty.GetCustomAttribute<FilterAttribute>();
                var partialStringMatch = filterAttribute != null && filterAttribute.AllowPartialStringMatch;
                var forceCaseInsensitiveMatch = filterAttribute != null && filterAttribute.ForceCaseInsensitiveMatch;

                if (partialStringMatch)
                {
                    query = ApplyStringLikeFilterToQuery(query, filterProperty, propertyValue, forceCaseInsensitiveMatch);
                }
                else
                {
                    query = ApplyStringEqualityFilterToQuery(query, filterProperty, propertyValue, forceCaseInsensitiveMatch);
                }
            }

            return query;
        }

        private IQueryable<T> AddNumericFilterToQuery(IQueryable<T> query, T filter, PropertyInfo filterProperty)
        {
            var propertyValue = filterProperty.GetValue(filter);
            if (Convert.ToDouble(propertyValue) != 0)
            {
                query = ApplyEqualityFilterToQuery(query, filterProperty, propertyValue);
            }

            return query;
        }

        private IQueryable<T> AddDateFilterToQuery(IQueryable<T> query, T filter, PropertyInfo filterProperty)
        {
            var propertyValue = filterProperty.GetValue(filter);
            if (Convert.ToDateTime(propertyValue) != DateTime.MinValue)
            {
                query = ApplyEqualityFilterToQuery(query, filterProperty, propertyValue);
            }

            return query;
        }

        private IQueryable<T> ApplyLessThanOrEqualFilterToQuery(IQueryable<T> query, PropertyInfo filterProperty, object propertyValue)
        {
            var e = Expression.Parameter(typeof(T), "e");
            var m = Expression.MakeMemberAccess(e, filterProperty);
            var c = filterProperty.PropertyType.IsEnum || filterProperty.PropertyType.IsNullableType() && Nullable.GetUnderlyingType(filterProperty.PropertyType).IsEnum
                ? Expression.Constant((int) propertyValue, typeof(int))
                : Expression.Constant(propertyValue, filterProperty.PropertyType);
            var b = filterProperty.PropertyType.IsEnum || filterProperty.PropertyType.IsNullableType() && Nullable.GetUnderlyingType(filterProperty.PropertyType).IsEnum
                ? Expression.LessThanOrEqual(Expression.Convert(m, typeof(int)), c)
                : Expression.LessThanOrEqual(m, c);

            var lambda = Expression.Lambda<Func<T, bool>>(b, e);
            query = query.Where(lambda);
            return query;
        }

        private IQueryable<T> ApplyGreaterThanOrEqualFilterToQuery(IQueryable<T> query, PropertyInfo filterProperty, object propertyValue)
        {
            var e = Expression.Parameter(typeof(T), "e");
            var m = Expression.MakeMemberAccess(e, filterProperty);
            var c = filterProperty.PropertyType.IsEnum || filterProperty.PropertyType.IsNullableType() && Nullable.GetUnderlyingType(filterProperty.PropertyType).IsEnum
                ? Expression.Constant((int) propertyValue, typeof(int))
                : Expression.Constant(propertyValue, filterProperty.PropertyType);
            var b = filterProperty.PropertyType.IsEnum || filterProperty.PropertyType.IsNullableType() && Nullable.GetUnderlyingType(filterProperty.PropertyType).IsEnum
                ? Expression.GreaterThanOrEqual(Expression.Convert(m, typeof(int)), c)
                : Expression.GreaterThanOrEqual(m, c);

            var lambda = Expression.Lambda<Func<T, bool>>(b, e);
            query = query.Where(lambda);
            return query;
        }

        private IQueryable<T> ApplyEqualityFilterToQuery(IQueryable<T> query, PropertyInfo filterProperty, object propertyValue)
        {
            var e = Expression.Parameter(typeof(T), "e");
            var m = Expression.MakeMemberAccess(e, filterProperty);
            var c = Expression.Constant(propertyValue, filterProperty.PropertyType);
            var b = Expression.Equal(m, c);

            var lambda = Expression.Lambda<Func<T, bool>>(b, e);
            query = query.Where(lambda);
            return query;
        }

        private IQueryable<T> ApplyStringEqualityFilterToQuery(IQueryable<T> query, PropertyInfo filterProperty, string propertyValue, bool forceCaseInsensitiveMatch)
        {
            var e = Expression.Parameter(typeof(T), "e");
            var m = Expression.MakeMemberAccess(e, filterProperty);

            if (forceCaseInsensitiveMatch)
            {
                var l = Expression.Call(m, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                var c = Expression.Constant(propertyValue.ToLowerInvariant(), typeof(string));
                var b = Expression.Equal(l, c);
                var lambda = Expression.Lambda<Func<T, bool>>(b, e);
                query = query.Where(lambda);
            }
            else
            {
                var c = Expression.Constant(propertyValue, typeof(string));
                var b = Expression.Equal(m, c);
                var lambda = Expression.Lambda<Func<T, bool>>(b, e);
                query = query.Where(lambda);
            }

            return query;
        }

        private IQueryable<T> ApplyStringLikeFilterToQuery(IQueryable<T> query, PropertyInfo filterProperty, string propertyValue, bool forceCaseInsensitiveMatch)
        {
            var e = Expression.Parameter(typeof(T), "e");
            var m = Expression.MakeMemberAccess(e, filterProperty);

            if (forceCaseInsensitiveMatch)
            {
                var l = Expression.Call(m, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                var c = Expression.Constant(propertyValue.ToLowerInvariant(), typeof(string));
                var condition = Expression.Call(l, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), c);
                var lambda = Expression.Lambda<Func<T, bool>>(condition, e);
                query = query.Where(lambda);
            }
            else
            {
                var c = Expression.Constant(propertyValue, typeof(string));
                var condition = Expression.Call(m, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), c);
                var lambda = Expression.Lambda<Func<T, bool>>(condition, e);
                query = query.Where(lambda);
            }
            
            return query;
        }

        private IQueryable<T> ApplyCollectionContains(IQueryable<T> query, PropertyInfo filterProperty, object propertyValue)
        {
            var e = Expression.Parameter(typeof(T), "e");
            var m = Expression.MakeMemberAccess(e, filterProperty);
            var genericCollectionOfPropertyType = typeof(Collection<>).MakeGenericType(filterProperty.PropertyType);
            var c = Expression.Constant(propertyValue, genericCollectionOfPropertyType);
            var condition = Expression.Call(c, genericCollectionOfPropertyType.GetMethod("Contains"), m);
            var lambda = Expression.Lambda<Func<T, bool>>(condition, e);
            query = query.Where(lambda);
            return query;
        }

        private IQueryable<T> ApplyListContains(IQueryable<T> query, PropertyInfo filterProperty, object propertyValue)
        {
            var e = Expression.Parameter(typeof(T), "e");
            var m = Expression.MakeMemberAccess(e, filterProperty);
            var genericCollectionOfPropertyType = typeof(List<>).MakeGenericType(filterProperty.PropertyType);
            var c = Expression.Constant(propertyValue, genericCollectionOfPropertyType);
            var condition = Expression.Call(c, genericCollectionOfPropertyType.GetMethod("Contains"), m);
            var lambda = Expression.Lambda<Func<T, bool>>(condition, e);
            query = query.Where(lambda);
            return query;
        }
    }

    public static class FilterServiceExtensions
    {
        public static bool IsNullableType(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return true;
            }

            return false;
        }

        public static bool IsNullableNumericOrEnumType(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return IsNumericOrEnumType(Nullable.GetUnderlyingType(type));
            }

            return false;
        }
        
        public static bool IsNumericOrEnumType(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
            }

            return false;
        }

        public static bool IsStringType(this Type type)
        {
            return Type.GetTypeCode(type) == TypeCode.String;
        }

        public static bool IsDateType(this Type type)
        {
            return Type.GetTypeCode(type) == TypeCode.DateTime;
        }
    }
}