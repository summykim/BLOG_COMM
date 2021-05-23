using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOG_COMM
{
    public class Base
    {
        public string Id { get; set; }
    }
    [FirestoreData]
    public class App_logs : Base
    {
        [FirestoreProperty]
        public Timestamp reg_dtm { get; set; }

        [FirestoreProperty]
        public String naverId { get; set; }
    }
    [FirestoreData]
    public class Users : Base
    {
        [FirestoreProperty]
        public string user_name { get; set; }
        [FirestoreProperty]
        public string user_desc { get; set; }
        [FirestoreProperty]
        public bool use_yn { get; set; }

        [FirestoreProperty]
        public string user_group { get; set; }
        [FirestoreProperty]
        public string naverId { get; set; }
        public string NotBeingSaved { get; set; }
    }


}
