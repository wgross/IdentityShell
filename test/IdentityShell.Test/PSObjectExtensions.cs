using Namotion.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Reflection;

namespace IdentityShell.Test
{
    public static class PSObjectExtensions
    {
        public static T As<T>(this PSObject obj) => (T)obj.ImmediateBaseObject;

        public static bool PropertyContains(this PSObject obj, string name)
            => obj.Properties.Any(p => p.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase));

        public static object Property(this PSObject obj, string name) => obj.Properties[name].Value;

        public static V Property<V>(this PSObject obj, string name) => (V)obj.Properties[name].Value;
    }

    public static class PowerShellExtensions
    {
        public class PowerShellCallBuilder<T>
        {
            private readonly PowerShell powershell;

            public PowerShellCallBuilder(PowerShell powershell)
            {
                this.powershell = powershell;
            }

            public PowerShellCallBuilder<T> AddParameter(Expression<Func<T, object>> parameterAccess, object value)
            {
                this.powershell.AddParameter(ParameterName(parameterAccess), value);
                return this;
            }

            public PowerShellCallBuilder<T> AddParameter(Expression<Func<T, object>> parameterAccess)
            {
                this.powershell.AddParameter(ParameterName(parameterAccess));
                return this;
            }

            public PowerShell End() => this.powershell;

            #region Extract parameter name

            private static string ParameterName(Expression<Func<T, object>> memberAccess) => InfoPath(memberAccess).Single().Name;

            private static IEnumerable<PropertyInfo> InfoPath(Expression<Func<T, object>> memberAccess)
            {
                if (memberAccess is null)
                    throw new ArgumentNullException(nameof(memberAccess));

                System.Linq.Expressions.Expression currentExpression = memberAccess;
                bool reachedPropertyAccess = false;
                do
                {
                    // parse the top part of the expepression ujntil member access is reached
                    // lambda -> convert -> member access

                    switch (currentExpression)
                    {
                        case LambdaExpression lambda:
                            currentExpression = lambda.Body;
                            break;

                        case UnaryExpression convert when (convert.NodeType == ExpressionType.Convert):
                            currentExpression = convert.Operand;
                            break;

                        case MemberExpression propertyAccess when propertyAccess.Member.MemberType == MemberTypes.Property:
                            // first property access expression reached -> brek the look
                            reachedPropertyAccess = true;
                            break;

                        default:
                            throw new InvalidOperationException($"property access contained unexpected expression: {currentExpression.NodeType}");
                    }
                }
                while (!reachedPropertyAccess);

                var pathToRoot = new Stack<PropertyInfo>();
                do
                {
                    switch (currentExpression)
                    {
                        case MemberExpression propertyAccess when propertyAccess.Member.MemberType == MemberTypes.Property:
                            pathToRoot.Push((PropertyInfo)propertyAccess.Member);
                            currentExpression = propertyAccess.Expression;
                            break;

                        default:
                            break;
                    }
                }
                while (currentExpression is MemberExpression);

                return pathToRoot.OfType<PropertyInfo>();
            }

            #endregion Extract parameter name
        }

        public static PowerShellCallBuilder<T> AddCommand<T>(this PowerShell ps) where T : PSCmdlet
        {
            return NewCommandBuilder<T>(ps);
        }

        public static PowerShell AddCommandEx<T>(this PowerShell powerShell, Action<PowerShellCallBuilder<T>> cmdBuilder = null) where T : PSCmdlet
        {
            PowerShellCallBuilder<T> builder = NewCommandBuilder<T>(powerShell);
            cmdBuilder?.Invoke(builder);
            return powerShell;
        }

        private static PowerShellCallBuilder<T> NewCommandBuilder<T>(PowerShell ps) where T : PSCmdlet
        {
            var cmdletAttribute = typeof(T).GetCustomAttributes(false).Single(a => a.GetType() == typeof(CmdletAttribute));
            return new PowerShellCallBuilder<T>(
               powershell: ps.AddCommand(
                   cmdlet: $"{cmdletAttribute.TryGetPropertyValue<string>(nameof(CmdletAttribute.VerbName))}-{cmdletAttribute.TryGetPropertyValue<string>(nameof(CmdletAttribute.NounName))}"));
        }
    }
}