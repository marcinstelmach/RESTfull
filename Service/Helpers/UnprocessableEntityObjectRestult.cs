using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Service.Helpers
{
    public class UnprocessableEntityObjectRestult : ObjectResult
    {
        public UnprocessableEntityObjectRestult(ModelStateDictionary modelState) : base(new SerializableError(modelState))
        {
            if (modelState == null)
                throw new ArgumentNullException(nameof(modelState));

            StatusCode = 422;
        }
    }
}