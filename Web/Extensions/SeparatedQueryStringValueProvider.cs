using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;

namespace Web.Extensions
{
    public class SeparatedQueryStringValueProvider : QueryStringValueProvider
    {
        private readonly string _key;
        private readonly string _separator;
        private readonly IQueryCollection _values;

        public SeparatedQueryStringValueProvider(IQueryCollection values, CultureInfo culture)
            : base(null, values, culture)
        {
        }

        public SeparatedQueryStringValueProvider(string key, IQueryCollection values, string separator)
            : base(BindingSource.Query, values, CultureInfo.InvariantCulture)
        {
            _key = key;
            _values = values;
            _separator = separator;
        }

        public override ValueProviderResult GetValue(string key)
        {
            var result = base.GetValue(key);

            if (_key != null && _key != key)
            {
                return result;
            }

            if (result != ValueProviderResult.None &&
                result.Values.Any(x => x.IndexOf(_separator, StringComparison.OrdinalIgnoreCase) > 0))
            {
                var splitValues = new StringValues(result.Values
                    .SelectMany(x => x.Split(new[] { _separator }, StringSplitOptions.None)).ToArray());

                return new ValueProviderResult(splitValues, result.Culture);
            }

            return result;
        }
    }

    public class SeparatedQueryStringValueProviderFactory : IValueProviderFactory
    {
        private readonly string _separator;
        private readonly string _key;

        public SeparatedQueryStringValueProviderFactory(string separator) : this(null, separator) { }

        public SeparatedQueryStringValueProviderFactory(string key, string separator)
        {
            _key = key;
            _separator = separator;
        }

        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            context.ValueProviders.Insert(0,
                new SeparatedQueryStringValueProvider(_key, context.ActionContext.HttpContext.Request.Query,
                    _separator));
            return Task.CompletedTask;
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class SeparatedQueryStringAttribute : Attribute, IResourceFilter
    {
        private readonly SeparatedQueryStringValueProviderFactory _factory;

        public SeparatedQueryStringAttribute() : this(",") { }

        public SeparatedQueryStringAttribute(string separator)
        {
            _factory = new SeparatedQueryStringValueProviderFactory(separator);
        }

        public SeparatedQueryStringAttribute(string key, string separator)
        {
            _factory = new SeparatedQueryStringValueProviderFactory(key, separator);
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {

        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            context.ValueProviderFactories.Insert(0, _factory);
        }
    }

    [AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class CommaSeparatedAttribute : Attribute
    {

    }

    public class CommaSeparatedQueryStringConvention : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            foreach (var parameter in action.Parameters)
            {
                if (parameter.Attributes.OfType<CommaSeparatedAttribute>().Any() &&
                    !parameter.Action.Filters.OfType<SeparatedQueryStringAttribute>().Any())
                {
                    parameter.Action.Filters.Add(new SeparatedQueryStringAttribute(parameter.ParameterName, ","));
                }
            }
        }
    }
}
