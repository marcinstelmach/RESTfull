using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Service.Service
{
    public class TypeHelperService
    {
        public bool TypeHasProperties<T>(string fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            var splitedFields = fields.Split(',');
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return splitedFields.All(field => properties.Any(prop => prop.Name == field.Trim()));
        }
    }
}
