﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Handlebars.Core.Compiler.Structure;
#if netstandard

#endif

namespace Handlebars.Core.Compiler.Translation.Expressions
{
    internal class BoolishConverter : HandlebarsExpressionVisitor
    {
        public static Expression Convert(Expression expr, CompilationContext context)
        {
            return new BoolishConverter(context).Visit(expr);
        }

        private BoolishConverter(CompilationContext context)
            : base(context)
        {
        }

        protected override Expression VisitBoolishExpression(BoolishExpression bex)
        {
            return Expression.Call(
#if netstandard
                new Func<object, bool>(HandlebarsUtils.IsTruthyOrNonEmpty).GetMethodInfo(),
#else
                new Func<object, bool>(HandlebarsUtils.IsTruthyOrNonEmpty).Method,
#endif
                Visit(bex.ConditionExpression));
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            return Expression.Block(
                node.Type,
                node.Variables,
                node.Expressions.Select(expr => Visit(expr)));
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            return Expression.MakeUnary(
                node.NodeType,
                Visit(node.Operand),
                node.Type);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            return Expression.Call(
                Visit(node.Object),
                node.Method,
                node.Arguments.Select(n => Visit(n)));
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            return Expression.Condition(
                Visit(node.Test),
                Visit(node.IfTrue),
                Visit(node.IfFalse));
        }
    }
}

