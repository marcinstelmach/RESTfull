using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Service.Helpers
{
    public static class ObjectExtensions
    {
        public static ExpandoObject ShapeData<TSource>(this TSource source, string fields)
        {
            if (source==null)
            {
                throw new ArgumentNullException("source");
            }
            var expandoObject = new ExpandoObject();

            

            if (string.IsNullOrWhiteSpace(fields))
            {
                var properties = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                properties.ToList().ForEach(prop => ((IDictionary<string, object>)expandoObject).Add(prop.Name, prop.GetValue(source)));
                return expandoObject;
            }

            var splitedFields = fields.Split(',');
            foreach (var splitedField in splitedFields)
            {
                var property = typeof(TSource).GetProperty(splitedField.Trim(), BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                if (property==null)
                {
                    throw new ArgumentException($"{splitedField} not found in {nameof(TSource)}");
                }
                ((IDictionary<string, object>)expandoObject).Add(property.Name, property.GetValue(source));
            }
            return expandoObject;

        }
    }
}
