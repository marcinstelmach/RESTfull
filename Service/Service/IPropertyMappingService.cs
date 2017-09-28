using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Service
{
    public interface IPropertyMappingService
    {
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
        bool ValidMappingExistFor<TSource, TDestination>(string fields);
    }
}
