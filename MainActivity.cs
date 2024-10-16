using AllUserInfoNativeApp.Helper;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System.Collections.Generic;
using System;

using Android.App;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using System.IO;

namespace AllUserInfoNativeApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        RecyclerView recyclerView;
        UserAdapter userAdapter;
        SQLiteHelper sqliteHelper;
        FirebaseHelper firebaseHelper;
        List<User> users;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
           // Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            recyclerView.SetLayoutManager(new LinearLayoutManager(this));

            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "users.db3");
            sqliteHelper = new SQLiteHelper(dbPath);
            firebaseHelper = new FirebaseHelper();

            LoadUsersFromSQLite();
            LoadUsersFromFirebase();

            userAdapter = new UserAdapter(users);
            recyclerView.SetAdapter(userAdapter);

            userAdapter.ItemClick += OnItemClick;
            userAdapter.DeleteClick += OnDeleteClick;
        }
        //public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        //{
        //    Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        //    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        //}

        private void LoadUsersFromSQLite()
        {
            users = sqliteHelper.GetAllUsers();
        }

        private async void LoadUsersFromFirebase()
        {
            var firebaseUsers = await firebaseHelper.GetAllUsers();
            users.AddRange(firebaseUsers); // Merge Firebase users with SQLite users
            userAdapter.NotifyDataSetChanged();
        }

        private void OnItemClick(object sender, int position)
        {
            // Edit user logic
            var user = users[position];
            // Open a new activity for editing
        }

        private void OnDeleteClick(object sender, int position)
        {
            var user = users[position];
            sqliteHelper.DeleteUser(user);
            users.RemoveAt(position);
            userAdapter.NotifyItemRemoved(position);
        }
    }


    public class UserAdapter : RecyclerView.Adapter
    {
        public List<User> Users { get; set; }
        public event EventHandler<int> ItemClick;
        public event EventHandler<int> DeleteClick;

        public UserAdapter(List<User> users)
        {
            Users = users;
        }

        public override int ItemCount => Users.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            UserViewHolder vh = holder as UserViewHolder;
            var user = Users[position];

            // Bind data to views.
            vh.NameTextView.Text = user.Name;
            vh.EmailTextView.Text = user.Email;
            vh.DateOfBirthTextView.Text = user.DateOfBirth.ToShortDateString();

            // Load image into ImageView (you can use Glide or Picasso library for loading image).
            if (!string.IsNullOrEmpty(user.ProfilePicture))
            {
                // Assuming ProfilePicture is a URI or path to the image.
                var imageUri = Android.Net.Uri.Parse(user.ProfilePicture);
                vh.ProfilePictureImageView.SetImageURI(imageUri);
            }

            // Click event for editing.
            vh.ItemView.Click += (sender, e) => ItemClick?.Invoke(sender, position);

            // Click event for deleting.
            vh.DeleteButton.Click += (sender, e) => DeleteClick?.Invoke(sender, position);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context)
                .Inflate(Resource.Layout.user_list_item, parent, false);

            return new UserViewHolder(itemView);
        }
    }

    public class UserViewHolder : RecyclerView.ViewHolder
    {
        public TextView NameTextView { get; private set; }
        public TextView EmailTextView { get; private set; }
        public TextView DateOfBirthTextView { get; private set; }
        public ImageView ProfilePictureImageView { get; private set; }
        public Button DeleteButton { get; private set; }

        public UserViewHolder(View itemView) : base(itemView)
        {
            NameTextView = itemView.FindViewById<TextView>(Resource.Id.textName);
            EmailTextView = itemView.FindViewById<TextView>(Resource.Id.textEmail);
            DateOfBirthTextView = itemView.FindViewById<TextView>(Resource.Id.textDOB);
            ProfilePictureImageView = itemView.FindViewById<ImageView>(Resource.Id.imageProfile);
            DeleteButton = itemView.FindViewById<Button>(Resource.Id.btnDelete);
        }
    }

}