using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOG_COMM
{
    public interface FirestoreInterface<T>
    {
        T Get(T record);
        List<T> GetAll();
        T Add(T record);
        bool Update(T record);
        bool Delete(T record);
    }
}
