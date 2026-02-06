using Domain.Validation;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Application.Validation
{
    public static class ModelValidator
    {
        public static ValidationError[] Validate(object model)
        {
            var errors = new List<ValidationError>();

            if (model == null)
                return Array.Empty<ValidationError>();

            var properties = model.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var value = property.GetValue(model);

                var attributes = property
                    .GetCustomAttributes<ValidationAttribute>();

                foreach (var attribute in attributes)
                {
                    if (!attribute.IsValid(value))
                    {
                        errors.Add(new ValidationError(
                            property.Name,
                            attribute.ErrorMessage));
                    }
                }
            }

            return errors.ToArray();
        }
    }
}
