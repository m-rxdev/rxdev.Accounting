using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.Model;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace rxdev.Accounting.App.ViewModels;

public abstract class DateFilteredGridViewModel<TEntity, TAdapter> :
    GridViewModel<TEntity, TAdapter>
    where TEntity : Entity, new()
    where TAdapter : EntityAdapter, new()
{
    private readonly Expression<Func<TEntity, DateTime>> _dateSelector;

    protected DateFilteredGridViewModel(
        IServiceProvider serviceProvider,
        Expression<Func<TEntity, DateTime>> dateSelector) 
        : base(serviceProvider)
    {
        _dateSelector = dateSelector;
    }

    //protected override IQueryable<TEntity> GetQuery(bool tracking = false)
    //{
    //    DateTime startDate = new(NavigationService.SelectedYear, 1, 1);
    //    DateTime endDate = new(NavigationService.SelectedYear + 1, 1, 1);

    //    var property = (PropertyInfo)((MemberExpression)_dateSelector.Body).Member;

    //    ParameterExpression parameter = Expression.Parameter(typeof(TEntity));

    //    var expression = Expression.Lambda<Func<TEntity, bool>>(
    //        Expression.AndAlso(
    //            Expression.GreaterThanOrEqual(Expression.Property(parameter, property), Expression.Constant(startDate, typeof(DateTime))),
    //            Expression.LessThan(Expression.Property(parameter, property), Expression.Constant(endDate, typeof(DateTime)))),
    //        parameter);

    //    return base.GetQuery(tracking).Where(expression);
    //}
    protected override IQueryable<TEntity> GetQuery(bool tracking = false)
    {
        DateTime startDate = new(NavigationService.SelectedYear, 1, 1);
        DateTime endDate = new(NavigationService.SelectedYear + 1, 1, 1);

        ParameterExpression parameter = Expression.Parameter(typeof(TEntity));
        MemberExpression accessor = GetMemberExpression((MemberExpression)_dateSelector.Body, parameter);

        Expression<Func<TEntity, bool>> expression = Expression.Lambda<Func<TEntity, bool>>(
            Expression.AndAlso(
                Expression.GreaterThanOrEqual(accessor, Expression.Constant(startDate, typeof(DateTime))),
                Expression.LessThan(accessor, Expression.Constant(endDate, typeof(DateTime)))),
            parameter);

        return base.GetQuery(tracking).Where(expression);
    }

    private static MemberExpression GetMemberExpression(MemberExpression expression, ParameterExpression parameter)
    {
        if (expression.Expression is MemberExpression p)
            return Expression.Property(GetMemberExpression(p, parameter), (PropertyInfo)expression.Member);
        else
            return Expression.Property(parameter, (PropertyInfo)expression.Member);
    }
}
