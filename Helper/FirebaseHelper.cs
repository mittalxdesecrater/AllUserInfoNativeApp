using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllUserInfoNativeApp.Helper
{
    public class FirebaseHelper
    {
        FirebaseClient firebase = new FirebaseClient("https://userdatainfonative-default-rtdb.firebaseio.com/");

        public async Task<List<User>> GetAllUsers()
        {
            return (await firebase
                .Child("Users")
                .OnceAsync<User>())
                .Select(item => new User
                {
                    Id = item.Object.Id,
                    Name = item.Object.Name,
                    PhoneNumber = item.Object.PhoneNumber,
                    Email = item.Object.Email,
                    ProfilePicture = item.Object.ProfilePicture,
                    DateOfBirth = item.Object.DateOfBirth
                }).ToList();
        }

        public async Task AddUser(User user)
        {
            await firebase
                .Child("Users")
                .PostAsync(user);
        }

        public async Task UpdateUser(User user)
        {
            var toUpdateUser = (await firebase
                .Child("Users")
                .OnceAsync<User>())
                .FirstOrDefault(a => a.Object.Id == user.Id);

            await firebase
                .Child("Users")
                .Child(toUpdateUser.Key)
                .PutAsync(user);
        }

        public async Task DeleteUser(int id)
        {
            var toDeleteUser = (await firebase
                .Child("Users")
                .OnceAsync<User>())
                .FirstOrDefault(a => a.Object.Id == id);
            await firebase.Child("Users").Child(toDeleteUser.Key).DeleteAsync();
        }
    }

}