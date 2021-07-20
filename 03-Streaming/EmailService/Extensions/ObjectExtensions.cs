using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsNull(this object value) => value is null;

        public static bool IsNotNull(this object value) => !value.IsNull();
    }
}
