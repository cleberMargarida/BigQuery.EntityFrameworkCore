using System.Linq.Expressions;

namespace BigQuery.EntityFrameworkCore.Utils;

internal static class NodeTypeFinder
{
    internal static T? Find<T>(this Expression node, Expression<Func<OptionsToFind, bool>> optionsToFind) where T : Expression
    {
        var options = Options(optionsToFind);
        var finder = Find<T>(node, options);
        return finder?.ResultNode;
    }

    internal static List<T>? FindAll<T>(this Expression node) where T : Expression
    {
        var finder = Find<T>(node, new OptionsToFind());
        return finder?.ResultNodes;
    }

    static NodeTypeFinderInternal<T>? Find<T>(Expression node, OptionsToFind options) where T : Expression => typeof(T) switch
    {
        var t when t == typeof(BinaryExpression) => new BinaryNodeTypeFinder(node, options) as NodeTypeFinderInternal<T>,
        var t when t == typeof(BlockExpression) => new BlockNodeTypeFinder(node, options) as NodeTypeFinderInternal<T>,
        var t when t == typeof(ConditionalExpression) => new ConditionalNodeTypeFinder(node, options) as NodeTypeFinderInternal<T>,
        var t when t == typeof(ConstantExpression) => new ConstantNodeTypeFinder(node, options) as NodeTypeFinderInternal<T>,
        var t when t == typeof(DebugInfoExpression) => new DebugInfoNodeTypeFinder(node, options) as NodeTypeFinderInternal<T>,
        var t when t == typeof(DefaultExpression) => new DefaultNodeTypeFinder(node, options) as NodeTypeFinderInternal<T>,
        var t when t == typeof(DynamicExpression) => new DynamicNodeTypeFinder(node, options) as NodeTypeFinderInternal<T>,
        var t when t == typeof(GotoExpression) => new GotoNodeTypeFinder(node, options) as NodeTypeFinderInternal<T>,
        var t when t == typeof(IndexExpression) => new IndexNodeTypeFinder(node, options) as NodeTypeFinderInternal<T>,
        var t when t == typeof(InvocationExpression) => new InvocationNodeTypeFinder(node, options) as NodeTypeFinderInternal<T>,
        var t when t == typeof(LabelExpression) => new LabelNodeTypeFinder(node, options) as NodeTypeFinderInternal<T>,
        var t when t == typeof(LambdaExpression) => new LambdaNodeTypeFinder(node, options) as NodeTypeFinderInternal<T>,
        var t when t == typeof(ListInitExpression) => new ListInitNodeTypeFinder(node, options) as NodeTypeFinderInternal<T>,
        var t when t == typeof(LoopExpression) => new LoopNodeTypeFinder(node, options) as NodeTypeFinderInternal<T>,
        var t when t == typeof(MemberExpression) => new MemberNodeTypeFinder(node, options) as NodeTypeFinderInternal<T>,
        var t when t == typeof(MemberInitExpression) => new MemberInitNodeTypeFinder(node, options) as NodeTypeFinderInternal<T>,
        var t when t == typeof(MethodCallExpression) => new MethodCallNodeTypeFinder(node, options) as NodeTypeFinderInternal<T>,
        var t when t == typeof(NewExpression) => new NewNodeTypeFinder(node, options) as NodeTypeFinderInternal<T>,
        var t when t == typeof(NewArrayExpression) => new NewArrayNodeTypeFinder(node, options) as NodeTypeFinderInternal<T>,
        var t when t == typeof(ParameterExpression) => new ParameterNodeTypeFinder(node, options) as NodeTypeFinderInternal<T>,
        var t when t == typeof(RuntimeVariablesExpression) => new RuntimeVariablesNodeTypeFinder(node, options) as NodeTypeFinderInternal<T>,
        var t when t == typeof(SwitchExpression) => new SwitchNodeTypeFinder(node, options) as NodeTypeFinderInternal<T>,
        var t when t == typeof(TryExpression) => new TryNodeTypeFinder(node, options) as NodeTypeFinderInternal<T>,
        var t when t == typeof(TypeBinaryExpression) => new TypeBinaryNodeTypeFinder(node, options) as NodeTypeFinderInternal<T>,
        var t when t == typeof(UnaryExpression) => new UnaryNodeTypeFinder(node, options) as NodeTypeFinderInternal<T>,
        _ => null
    };

    abstract class NodeTypeFinderInternal<T> : ExpressionVisitor
        where T : Expression
    {
        public NodeTypeFinderInternal(Expression? node, OptionsToFind optionsToFind)
        {
            OptionsToFind = optionsToFind;
            Visit(node);
        }

        public OptionsToFind OptionsToFind { get; }
        public T? ResultNode { get; protected set; }
        public List<T> ResultNodes { get; protected set; } = new();

        protected void SetResult(T expression)
        {
            ResultNodes.Add(expression);

            if (OptionsToFind.StopOnFirst)
            {
                ResultNode ??= expression;
            }

            if (OptionsToFind.StopOnLast)
            {
                ResultNode = expression;
            }
        }
    }

    internal class OptionsToFind
    {
        public bool StopOnFirst { get; set; }
        public bool StopOnLast { get; set; }
    }

    static OptionsToFind Options<T>(Expression<Func<OptionsToFind, T>> expression)
    {
        var options = new OptionsToFind();
        var body = (MemberExpression)expression.Body;
        var property = (System.Reflection.PropertyInfo)body.Member;
        property.SetValue(options, true);
        return options;
    }

    class BinaryNodeTypeFinder : NodeTypeFinderInternal<BinaryExpression>
    {
        public BinaryNodeTypeFinder(Expression? node, OptionsToFind options) : base(node, options)
        {
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            SetResult(node);
            return base.VisitBinary(node);
        }
    }

    class BlockNodeTypeFinder : NodeTypeFinderInternal<BlockExpression>
    {
        public BlockNodeTypeFinder(Expression? node, OptionsToFind options) : base(node, options)
        {
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            SetResult(node);
            return base.VisitBlock(node);
        }
    }

    class ConditionalNodeTypeFinder : NodeTypeFinderInternal<ConditionalExpression>
    {
        public ConditionalNodeTypeFinder(Expression? node, OptionsToFind options) : base(node, options)
        {
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            SetResult(node);
            return base.VisitConditional(node);
        }
    }

    class ConstantNodeTypeFinder : NodeTypeFinderInternal<ConstantExpression>
    {
        public ConstantNodeTypeFinder(Expression? node, OptionsToFind options) : base(node, options)
        {
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            SetResult(node);
            return base.VisitConstant(node);
        }
    }

    class DebugInfoNodeTypeFinder : NodeTypeFinderInternal<DebugInfoExpression>
    {
        public DebugInfoNodeTypeFinder(Expression? node, OptionsToFind options) : base(node, options)
        {
        }

        protected override Expression VisitDebugInfo(DebugInfoExpression node)
        {
            SetResult(node);
            return base.VisitDebugInfo(node);
        }
    }

    class DefaultNodeTypeFinder : NodeTypeFinderInternal<DefaultExpression>
    {
        public DefaultNodeTypeFinder(Expression? node, OptionsToFind options) : base(node, options)
        {
        }

        protected override Expression VisitDefault(DefaultExpression node)
        {
            SetResult(node);
            return base.VisitDefault(node);
        }
    }

    class DynamicNodeTypeFinder : NodeTypeFinderInternal<DynamicExpression>
    {
        public DynamicNodeTypeFinder(Expression? node, OptionsToFind options) : base(node, options)
        {
        }

        protected override Expression VisitDynamic(DynamicExpression node)
        {
            SetResult(node);
            return base.VisitDynamic(node);
        }
    }

    class ExtensionNodeTypeFinder : NodeTypeFinderInternal<Expression>
    {
        public ExtensionNodeTypeFinder(Expression? node, OptionsToFind options) : base(node, options)
        {
        }

        protected override Expression VisitExtension(Expression node)
        {
            SetResult(node);
            return base.VisitExtension(node);
        }
    }

    class GotoNodeTypeFinder : NodeTypeFinderInternal<GotoExpression>
    {
        public GotoNodeTypeFinder(Expression? node, OptionsToFind options) : base(node, options)
        {
        }

        protected override Expression VisitGoto(GotoExpression node)
        {
            SetResult(node);
            return base.VisitGoto(node);
        }
    }

    class IndexNodeTypeFinder : NodeTypeFinderInternal<IndexExpression>
    {
        public IndexNodeTypeFinder(Expression? node, OptionsToFind options) : base(node, options)
        {
        }

        protected override Expression VisitIndex(IndexExpression node)
        {
            SetResult(node);
            return base.VisitIndex(node);
        }
    }

    class InvocationNodeTypeFinder : NodeTypeFinderInternal<InvocationExpression>
    {
        public InvocationNodeTypeFinder(Expression? node, OptionsToFind options) : base(node, options)
        {
        }

        protected override Expression VisitInvocation(InvocationExpression node)
        {
            SetResult(node);
            return base.VisitInvocation(node);
        }
    }

    class LabelNodeTypeFinder : NodeTypeFinderInternal<LabelExpression>
    {
        public LabelNodeTypeFinder(Expression? node, OptionsToFind options) : base(node, options)
        {
        }

        protected override Expression VisitLabel(LabelExpression node)
        {
            SetResult(node);
            return base.VisitLabel(node);
        }
    }

    class LambdaNodeTypeFinder : NodeTypeFinderInternal<LambdaExpression>
    {
        public LambdaNodeTypeFinder(Expression? node, OptionsToFind options) : base(node, options)
        {
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            SetResult(node);
            return base.VisitLambda(node);
        }
    }

    class ListInitNodeTypeFinder : NodeTypeFinderInternal<ListInitExpression>
    {
        public ListInitNodeTypeFinder(Expression? node, OptionsToFind options) : base(node, options)
        {
        }

        protected override Expression VisitListInit(ListInitExpression node)
        {
            SetResult(node);
            return base.VisitListInit(node);
        }
    }

    class LoopNodeTypeFinder : NodeTypeFinderInternal<LoopExpression>
    {
        public LoopNodeTypeFinder(Expression? node, OptionsToFind options) : base(node, options)
        {
        }

        protected override Expression VisitLoop(LoopExpression node)
        {
            SetResult(node);
            return base.VisitLoop(node);
        }
    }

    class MemberNodeTypeFinder : NodeTypeFinderInternal<MemberExpression>
    {
        public MemberNodeTypeFinder(Expression? node, OptionsToFind options) : base(node, options)
        {
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            SetResult(node);
            return base.VisitMember(node);
        }
    }

    class MemberInitNodeTypeFinder : NodeTypeFinderInternal<MemberInitExpression>
    {
        public MemberInitNodeTypeFinder(Expression? node, OptionsToFind options) : base(node, options)
        {
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            SetResult(node);
            return base.VisitMemberInit(node);
        }
    }

    class MethodCallNodeTypeFinder : NodeTypeFinderInternal<MethodCallExpression>
    {
        public MethodCallNodeTypeFinder(Expression? node, OptionsToFind options) : base(node, options)
        {
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            SetResult(node);
            return base.VisitMethodCall(node);
        }
    }

    class NewNodeTypeFinder : NodeTypeFinderInternal<NewExpression>
    {
        public NewNodeTypeFinder(Expression? node, OptionsToFind options) : base(node, options)
        {
        }

        protected override Expression VisitNew(NewExpression node)
        {
            SetResult(node);
            return base.VisitNew(node);
        }
    }

    class NewArrayNodeTypeFinder : NodeTypeFinderInternal<NewArrayExpression>
    {
        public NewArrayNodeTypeFinder(Expression? node, OptionsToFind options) : base(node, options)
        {
        }

        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            SetResult(node);
            return base.VisitNewArray(node);
        }
    }

    class ParameterNodeTypeFinder : NodeTypeFinderInternal<ParameterExpression>
    {
        public ParameterNodeTypeFinder(Expression? node, OptionsToFind options) : base(node, options)
        {
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            SetResult(node);
            return base.VisitParameter(node);
        }
    }

    class RuntimeVariablesNodeTypeFinder : NodeTypeFinderInternal<RuntimeVariablesExpression>
    {
        public RuntimeVariablesNodeTypeFinder(Expression? node, OptionsToFind options) : base(node, options)
        {
        }

        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            SetResult(node);
            return base.VisitRuntimeVariables(node);
        }
    }

    class SwitchNodeTypeFinder : NodeTypeFinderInternal<SwitchExpression>
    {
        public SwitchNodeTypeFinder(Expression? node, OptionsToFind options) : base(node, options)
        {
        }

        protected override Expression VisitSwitch(SwitchExpression node)
        {
            SetResult(node);
            return base.VisitSwitch(node);
        }
    }

    class TryNodeTypeFinder : NodeTypeFinderInternal<TryExpression>
    {
        public TryNodeTypeFinder(Expression? node, OptionsToFind options) : base(node, options)
        {
        }

        protected override Expression VisitTry(TryExpression node)
        {
            SetResult(node);
            return base.VisitTry(node);
        }
    }

    class TypeBinaryNodeTypeFinder : NodeTypeFinderInternal<TypeBinaryExpression>
    {
        public TypeBinaryNodeTypeFinder(Expression? node, OptionsToFind options) : base(node, options)
        {
        }

        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            SetResult(node);
            return base.VisitTypeBinary(node);
        }
    }

    class UnaryNodeTypeFinder : NodeTypeFinderInternal<UnaryExpression>
    {
        public UnaryNodeTypeFinder(Expression? node, OptionsToFind options) : base(node, options)
        {
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            SetResult(node);
            return base.VisitUnary(node);
        }
    }
}
