using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace gt.Collections.OrderedList
{
    public class ReflectionHelper
    {
        public static IEnumerable<string> GetPropertyChain<T, P>(Expression<Func<T, P>> expr)
        {
            MemberExpression me;
            switch (expr.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    var ue = expr.Body as UnaryExpression;
                    me = ((ue != null) ? ue.Operand : null) as MemberExpression;
                    break;
                default:
                    me = expr.Body as MemberExpression;
                    break;
            }

            while (me != null)
            {
                var propertyName = me.Member.Name;                
                yield return propertyName;

                me = me.Expression as MemberExpression;
            }
        }
    }
}