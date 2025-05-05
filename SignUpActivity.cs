
using Android.Content;


namespace Gym_Me
{
    [Activity(Label = "SignUpActivity")]
    public class SignUpActivity : Activity
    {
        private DatabaseHelper dbHelper;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SignUp);

            // Initialize database helper
            dbHelper = new DatabaseHelper(this);

            // Get references to UI elements
            EditText nameInput = FindViewById<EditText>(Resource.Id.nameInput);
            EditText emailInput = FindViewById<EditText>(Resource.Id.emailInput);
            EditText passwordInput = FindViewById<EditText>(Resource.Id.passwordInput);
            EditText confirmPasswordInput = FindViewById<EditText>(Resource.Id.confirmPasswordInput);
            Button signUpButton = FindViewById<Button>(Resource.Id.signUpButton);

            // Handle Sign-Up button click
            signUpButton.Click += (sender, e) =>
            {
                string name = nameInput.Text.Trim();
                string email = emailInput.Text.Trim();
                string password = passwordInput.Text;
                string confirmPassword = confirmPasswordInput.Text;

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) ||
                    string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
                {
                    Toast.MakeText(this, "All fields are required.", ToastLength.Short).Show();
                    return;
                }

                if (password != confirmPassword)
                {
                    Toast.MakeText(this, "Passwords do not match.", ToastLength.Short).Show();
                    return;
                }

                if (dbHelper.IsEmailRegistered(email))
                {
                    Toast.MakeText(this, "Email is already registered.", ToastLength.Short).Show();
                    return;
                }

                bool isAdded = dbHelper.AddUser(email, password, name);
                if (isAdded)
                {
                    Toast.MakeText(this, "Registration successful! Please log in.", ToastLength.Short).Show();

                    // Navigate back to LoginActivity
                    Intent intent = new Intent(this, typeof(LoginActivity));
                    StartActivity(intent);
                    Finish();
                }
                else
                {
                    Toast.MakeText(this, "Registration failed. Please try again.", ToastLength.Short).Show();
                }
            };
        }
    }
}
