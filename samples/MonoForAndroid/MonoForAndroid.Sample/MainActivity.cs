using System.Collections.Generic;
using Android.App;
using Android.Content.PM;
using Android.Views;
using Android.Widget;
using Android.OS;
using ZXing;
using ZXing.Mobile;

namespace ZxingSharp.MonoForAndroid.Sample
{
	[Activity (Label = "ZXing.Mobile", MainLauncher = true, ConfigurationChanges=ConfigChanges.Orientation|ConfigChanges.KeyboardHidden)]
	public class Activity1 : Activity
	{
		Button buttonScanCustomView;
		Button buttonScanDefaultView;

		MobileBarcodeScanner scanner;
	
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			//Create a new instance of our Scanner
			scanner = new MobileBarcodeScanner(this);

			buttonScanDefaultView = this.FindViewById<Button>(Resource.Id.buttonScanDefaultView);
			buttonScanDefaultView.Click += delegate {
				
				//Tell our scanner to use the default overlay
				scanner.UseCustomOverlay = false;

				//We can customize the top and bottom text of the default overlay
				scanner.TopText = "Hold the camera up to the barcode\nAbout 6 inches away";
				scanner.BottomText = "Wait for the barcode to automatically scan!";

				//Start scanning
				scanner.Scan().ContinueWith((t) => {
					if (t.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
						HandleScanResult(t.Result);
				});
			};

			buttonScanCustomView = this.FindViewById<Button>(Resource.Id.buttonScanCustomView);
			buttonScanCustomView.Click += delegate {

				//Tell our scanner we want to use a custom overlay instead of the default
				scanner.UseCustomOverlay = true;

				//Inflate our custom overlay from a resource layout
				var zxingOverlay = LayoutInflater.FromContext(this).Inflate(Resource.Layout.ZxingOverlay, null);

				//Find the button from our resource layout and wire up the click event
				var flashButton = zxingOverlay.FindViewById<Button>(Resource.Id.buttonZxingFlash);
				flashButton.Click += (sender, e) => {
					//Tell our scanner to toggle the torch/flash
					scanner.ToggleTorch();
				};

				//Set our custom overlay
				scanner.CustomOverlay = zxingOverlay;

				//Start scanning!
				scanner.Scan().ContinueWith((t) => {
					if (t.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
						HandleScanResult(t.Result);
				});
			};
		}

		void HandleScanResult (ZXing.Result result)
		{
			string msg = "";

			if (result != null && !string.IsNullOrEmpty(result.Text))
				msg = "Found Barcode: " + result.Text;
			else
				msg = "Scanning Canceled!";

			this.RunOnUiThread(() => Toast.MakeText(this, msg, ToastLength.Short).Show());
		}
	}
}


