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
        public List<Users> GetUserWhere(string NaverId,string UserName)
        {
            Query query = null ;
            if (NaverId.Length > 0 && UserName.Length>0)
            {
                query = repo.fireStoreDb.Collection(collectionName).WhereEqualTo(nameof(Users.naverId), NaverId).WhereEqualTo(nameof(Users.user_name), UserName);
            }else if (NaverId.Length > 0 && UserName.Length == 0)
            {
                query = repo.fireStoreDb.Collection(collectionName).WhereEqualTo(nameof(Users.naverId), NaverId);
            }
            else if (NaverId.Length == 0 && UserName.Length > 0)
            {
                query = repo.fireStoreDb.Collection(collectionName).WhereEqualTo(nameof(Users.user_name), UserName);
            }
            else
            {
                query = repo.fireStoreDb.Collection(collectionName);
            }

              return repo.QueryRecords<Users>(query);
        }

    }
}
