using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOG_COMM
{
    public class LogRepository : FirestoreInterface<App_logs>
    {
        string collectionName = "APP_LOGS";
        BaseRepository repo;
        public LogRepository()
        {
            repo = new BaseRepository(collectionName);
        }
        public App_logs Add(App_logs record) => repo.Add(record);
        public bool Update(App_logs record) => repo.Update(record);
        public bool Delete(App_logs record) => repo.Delete(record);
        public App_logs Get(App_logs record) => repo.Get(record);
        public List<App_logs> GetAll() => repo.GetAll<App_logs>();
        public List<App_logs> GetUserWhereNaverId(string NaverId)
        {
             Query query = repo.fireStoreDb.Collection(collectionName).WhereEqualTo(nameof(Users.naverId), NaverId);
            return repo.QueryRecords<App_logs>(query);
        }
 

    }
}
