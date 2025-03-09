using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;

namespace Gym_Me
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        private DatabaseHelper dbHelper;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Login);

            // Initialize database helper
            dbHelper = new DatabaseHelper(this);

            // Get references to UI elements
            EditText emailInput = FindViewById<EditText>(Resource.Id.emailInput);
            EditText passwordInput = FindViewById<EditText>(Resource.Id.passwordInput);
            Button loginButton = FindViewById<Button>(Resource.Id.loginButton);
            TextView signUpLink = FindViewById<TextView>(Resource.Id.signUpLink);

            // Set up the login button click event
            loginButton.Click += (sender, e) =>
            {
                string email = emailInput.Text.Trim();
                string password = passwordInput.Text;

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    Toast.MakeText(this, "Please enter both email and password.", ToastLength.Short).Show();
                    return;
                }

                if (IsValidLogin(email, password))
                {
                    // Save user data in SharedPreferences
                    SaveUserToPreferences(email);

                    // Navigate to the main activity
                    Intent intent = new Intent(this, typeof(MainActivity));
                    StartActivity(intent);
                    Finish(); // Optionally close the login activity
                }
                else
                {
                    Toast.MakeText(this, "Invalid email or password.", ToastLength.Short).Show();
                }
            };


            // Navigate to SignUpActivity when the sign-up link is clicked
            signUpLink.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(SignUpActivity));
                StartActivity(intent);
            };
        }

        private bool IsValidLogin(string email, string password)
        {
            // Check if the login is for the "admin" user (if needed)
            if (email.Equals("admin", StringComparison.OrdinalIgnoreCase) && password == "123456789")
            {
                return true;
            }

            // Check if the user exists in the SQLite database
            return dbHelper.AuthenticateUser(email, password);
        }


        private void SaveUserToPreferences(string email)
        {
            var preferences = GetSharedPreferences("GymMePreferences", FileCreationMode.Private);
            var editor = preferences.Edit();
            editor.PutString("userEmail", email); // Save email to shared preferences
            editor.Apply();
        }
    }
}
