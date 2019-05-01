using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DFC.Composite.Paths.Extensions
{
    public class HttpResponseBody<T>
    {
        public T Value { get; set; }
        public bool IsValid { get; set; }
    
        public IEnumerable<ValidationResult> ValidationResults { get; set; }
    }
}
