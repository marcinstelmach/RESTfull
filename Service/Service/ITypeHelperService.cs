using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Service
{
    public interface ITypeHelperService
    {
        bool TypeHasProperties<T>(string fields);
    }
}
