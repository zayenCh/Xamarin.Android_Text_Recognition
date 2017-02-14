using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Gms.Vision;
using Android.Gms.Vision.Texts;
using Android.Graphics;
using Android.Runtime;
using System;
using Android.Support.V4.App;
using Android;
using Android.Content.PM;
using Android.Util;
using Java.Lang;

namespace TextRecognition
{
	[Activity(Label = "TextRecognition", MainLauncher = true, Icon = "@mipmap/icon", Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
	public class MainActivity : AppCompatActivity, ISurfaceHolderCallback, Detector.IProcessor
	{
		private SurfaceView cameraView;
		private TextView textView;
		private CameraSource cameraSource;
		private const int requestCameraPersiionID = 1001;

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
		{
			switch (requestCode)
			{
				case requestCameraPersiionID:
					{
						if (grantResults[0] == Permission.Granted)
						{
							cameraSource.Start(cameraView.Holder);
						}
					}
					break;
			}
		}


		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main);
			cameraView = FindViewById<SurfaceView>(Resource.Id.surfaceView);
			textView = FindViewById<TextView>(Resource.Id.textView);

			TextRecognizer textRecog = new TextRecognizer.Builder(ApplicationContext).Build();
			if (!textRecog.IsOperational)
			{
				Log.Error("Main activity","depencecies are not activated");
			}
			else
			{
				cameraSource = new CameraSource.Builder(ApplicationContext, textRecog)
										   .SetFacing(CameraFacing.Back)
										   .SetRequestedPreviewSize(1280, 1024)
										   .SetRequestedFps(2.0f)
										   .SetAutoFocusEnabled(true)
										   .Build();

				cameraView.Holder.AddCallback(this);
				textRecog.SetProcessor(this);
			}




		}

		public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
		{
			
		}

		public void SurfaceCreated(ISurfaceHolder holder)
		{
			if (ActivityCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted)
			{
				ActivityCompat.RequestPermissions(this, new string[] { Android.Manifest.Permission.Camera }, requestCameraPersiionID);
				return;
			}
			cameraSource.Start(cameraView.Holder);
		}

		public void SurfaceDestroyed(ISurfaceHolder holder)
		{
			cameraSource.Stop();
		}

		public void ReceiveDetections(Detector.Detections detections)
		{
			SparseArray items = detections.DetectedItems;
			if (items.Size() != 0)
			{
				textView.Post(() =>
				{
				StringBuilder strBuilder = new StringBuilder();
				for (int i = 0; i < items.Size(); ++i)
				{
					strBuilder.Append(((TextBlock)items.ValueAt(i)).Value);
				strBuilder.Append("\n");
			}
			textView.Text = strBuilder.ToString();
		});
			}
		}

		public void Release()
		{
			throw new NotImplementedException();
		}
	}
}

