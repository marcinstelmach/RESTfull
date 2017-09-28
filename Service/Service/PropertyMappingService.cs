using System;
using System.Collections.Generic;
using System.Linq;
using Data.DTO;
using Data.Model;

namespace Service.Service
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private readonly Dictionary<string, PropertyMappingValue> _auhorPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Id", new PropertyMappingValue(new List<string> {"Id"})},
                {"Genre", new PropertyMappingValue(new List<string> {"Genre"})},
                {"Age", new PropertyMappingValue(new List<string> {"DateOfBirth"}, true)},
                {"Name", new PropertyMappingValue(new List<string> {"FirstName", "LastName"})}
            };

        private readonly IList<IPropertyMapping> _propertyMapping = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            _propertyMapping.Add(new PropertyMapping<AuthorDto, Author>(_auhorPropertyMapping));
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var matchingMapping = _propertyMapping.OfType<PropertyMapping<TSource, TDestination>>();
            if (matchingMapping.Count() == 1)
                return matchingMapping.First().MappingDictionary;

            throw new Exception(
                $"Cannot find exact property mapping instance for <{typeof(TSource)}, {typeof(TDestination)}>");
        }

        public bool ValidMappingExistFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
                return true;

            var fieldsAfterSplit = fields.Split(',');
            foreach (var field in fieldsAfterSplit)
            {
                var trimmedFiled = field.Trim();

                var indexOfFirstSpace = trimmedFiled.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ? trimmedFiled : trimmedFiled.Remove(indexOfFirstSpace);

                if (!propertyMapping.ContainsKey(propertyName))
                    return false;
            }
            return true;
        }
    }
}