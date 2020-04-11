using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
 public    interface IModelT1Service
    {
        int AddModelT1(Models.ModelT1 model);
        int UpdateModelT1(Models.ModelT1 model);
        int DeleteModelT1(Models.ModelT1 model);
        List<Models.ModelT1> QueryModelList();
    }
}
