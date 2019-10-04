using System;
using System.Text;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Views.Accessibility;
using Android.AccessibilityServices;
using Android;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace UssdcodeRead
{
    [Activity(Label = "Tv Accessibility", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            var intentToAccessibility = new Intent(this, typeof(TVNavigatorService));
            StartService(intentToAccessibility);

            Button button = FindViewById<Button>(Resource.Id.myButton);

            button.Click += delegate
            {
                button.Text = string.Format("{0} TV Accessibility", count++);
            };
        }
    }
}


