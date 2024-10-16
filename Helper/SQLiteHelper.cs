using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AllUserInfoNativeApp.Helper
{
    public class SQLiteHelper
    {
        private SQLiteConnection _database;

        public SQLiteHelper(string dbPath)
        {
            _database = new SQLiteConnection(dbPath);
            _database.CreateTable<User>();
        }

        public List<User> GetAllUsers()
        {
            return _database.Table<User>().ToList();
        }

        public int SaveUser(User user)
        {
            return _database.Insert(user);
        }

        public int UpdateUser(User user)
        {
            return _database.Update(user);
        }

        public int DeleteUser(User user)
        {
            return _database.Delete(user);
        }
    }

}