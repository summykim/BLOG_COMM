using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BLOG_COMM
{
    public class BaseRepository
    {
        private string collectionName;
        public FirestoreDb fireStoreDb;
        public BaseRepository(string CollectionName)
        {
            string filepath = Environment.CurrentDirectory+@"\marbling-ce26e-firebase-adminsdk-7mhey-84514381bb.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            fireStoreDb = FirestoreDb.Create("marbling-ce26e");
            collectionName = CollectionName;
        }
        public T Add<T>(T record) where T : Base
        {
            CollectionReference colRef = fireStoreDb.Collection(collectionName);
            DocumentReference doc = colRef.AddAsync(record).GetAwaiter().GetResult();
            record.Id = doc.Id;
            return record;
        }
        public bool Update<T>(T record) where T : Base
        {
            DocumentReference recordRef = fireStoreDb.Collection(collectionName).Document(record.Id);
            recordRef.SetAsync(record, SetOptions.MergeAll).GetAwaiter().GetResult();
            return true;
        }
        public bool Delete<T>(T record) where T : Base
        {
            DocumentReference recordRef = fireStoreDb.Collection(collectionName).Document(record.Id);
            recordRef.DeleteAsync().GetAwaiter().GetResult();
            return true;
        }
        public T Get<T>(T record) where T : Base
        {
            DocumentReference docRef = fireStoreDb.Collection(collectionName).Document(record.Id);
            DocumentSnapshot snapshot = docRef.GetSnapshotAsync().GetAwaiter().GetResult();
            if (snapshot.Exists)
            {
                T usr = snapshot.ConvertTo<T>();
                usr.Id = snapshot.Id;
                return usr;
            }
            else
            {
                return null;
            }
        }
        public List<T> GetAll<T>() where T : Base
        {
            Query query = fireStoreDb.Collection(collectionName);
            QuerySnapshot querySnapshot = query.GetSnapshotAsync().GetAwaiter().GetResult();
            List<T> list = new List<T>();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                if (documentSnapshot.Exists)
                {
                    Dictionary<string, object> city = documentSnapshot.ToDictionary();
                    string json = JsonConvert.SerializeObject(city);
                    T newItem = JsonConvert.DeserializeObject<T>(json);
                    newItem.Id = documentSnapshot.Id;
                    list.Add(newItem);
                }
            }
            return list;
        }
        public List<T> QueryRecords<T>(Query query) where T : Base
        {
            QuerySnapshot querySnapshot = query.GetSnapshotAsync().GetAwaiter().GetResult();
            List<T> list = new List<T>();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                if (documentSnapshot.Exists)
                {
                    Dictionary<string, object> user = documentSnapshot.ToDictionary();
                    string json = JsonConvert.SerializeObject(user);
                    T newItem = JsonConvert.DeserializeObject<T>(json);
                    newItem.Id = documentSnapshot.Id;
                    list.Add(newItem);
                }
            }
            return list;
        }
    }
}