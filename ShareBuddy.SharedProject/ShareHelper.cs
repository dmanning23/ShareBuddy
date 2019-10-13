using MenuBuddy;
using Microsoft.Xna.Framework;
using System;
using ToastBuddyLib;
using System.Threading.Tasks;
#if ANDROID
using Uri = Android.Net.Uri;
using Android.App;
using Android.Content;
using Plugin.CurrentActivity;
#elif __IOS__
using UIKit;
using Foundation;
#endif

namespace ShareBuddy
{
	public class ShareHelper
	{
		Game Game { get; set; }

		public ShareHelper(Game game)
		{
			Game = game;
		}

		///
		/// Shares an image using the preferred method in Android or iOS.
		///
		/// Path.
		public async Task ShareImage(string path, string extratxt = "")
		{
			try
			{
#if ANDROID
				// Set up the Share Intent
				var shareIntent = new Intent(Intent.ActionSend);

				// Add Text to go along with it
				shareIntent.PutExtra(Intent.ExtraText, extratxt);

				// The link to the photo you want to share (i.e the path to the saved screenshot)
				var photoFile = new Java.IO.File(path);
				var uri = Uri.FromFile(photoFile);

				// Add the required info the the shareIntent
				shareIntent.PutExtra(Intent.ExtraStream, uri);
				shareIntent.SetType("image/*");
				shareIntent.AddFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.NewTask);

				// Now Send the Share Intent
				CrossCurrentActivity.Current.AppContext.StartActivity(Intent.CreateChooser(shareIntent, "Choose one"));

				//// Now Send the Share Intent
				//Application.Context.StartActivity(Intent.CreateChooser(shareIntent, "Choose one"));
#elif __IOS__
				UIApplication.SharedApplication.InvokeOnMainThread(delegate
				{
					var viewController = Game.Services.GetService<UIViewController>();

					var file = NSFileManager.DefaultManager.Contents(path);
					var tempUrl = NSUrl.CreateFileUrl(new string[] { path });
					var itemsToShare = new NSObject[] { tempUrl };

					UIActivityViewController activityViewController = new UIActivityViewController(itemsToShare, null);
					activityViewController.ExcludedActivityTypes = new NSString[] { };

					if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
					{
						activityViewController.PopoverPresentationController.SourceView = viewController.View;
						activityViewController.PopoverPresentationController.SourceRect = new CoreGraphics.CGRect((viewController.View.Bounds.Width / 2), (viewController.View.Bounds.Height / 4), 0, 0);
					}

					viewController.PresentViewController(activityViewController, true, null);
				});
#else
				var messageDisplay = Game.Services.GetService<IToastBuddy>();
				messageDisplay.ShowMessage($"Sharing not available on this platform", Color.Yellow);
#endif
			}
			catch (Exception ex)
			{
				var messageDisplay = Game.Services.GetService<IToastBuddy>();
				messageDisplay.ShowMessage($"Error sharing image", Color.Yellow);

				var screenManager = Game.Services.GetService<IScreenManager>();
				await screenManager.AddScreen(new OkScreen(ex.Message));
			}
		}
	}
}
