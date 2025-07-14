using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock_Sync
{
    internal interface IDataOperations
    {   
        bool Add();
        void FetchDetailId(int Id);
        bool Update(int id);
        bool Delete(int id);

    }
}
