using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOG_COMM
{
    public class UserRepository : FirestoreInterface<Users>
    {
        string collectionName = "USERS";
        BaseRepository repo;
        public UserRepository()
        {
            repo = new BaseRepository(collectionName);
        }
        public Users Add(Users record) => repo.Add(record);
        public bool Update(Users record) => repo.Update(record);
        public bool Delete(Users record) => repo.Delete(record);
        public Users Get(Users record) => repo.Get(record);
        public List<Users> GetAll() => repo.GetAll<Users>();
        public List<Users> GetUserWhereNaverId(string NaverId)
        {
 
            Query query = repo.fireStoreDb.Collection(collectionName).WhereEqualTo(nameof(Users.naverId), NaverId);
            return repo.QueryRecords<Users>(query);
        }
    }
}
