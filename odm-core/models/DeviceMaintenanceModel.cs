﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using nvc.onvif;
using onvifdm.utils;

using dev = global::onvif.services.device;
using med = global::onvif.services.media;
using onvif.services.device;
using onvif.services.media;
using System.IO;
using System.Net;
using System.Net.Mime;
using nvc.rx;
using System.Threading;

namespace nvc.models {

	public class DeviceMaintenanceModel : ModelBase<DeviceMaintenanceModel> {
		public DeviceMaintenanceModel() {
			firmwareUpgradeSupported = false;
		}

		protected override IEnumerable<IObservable<object>> LoadImpl(onvif.Session session, IObserver<DeviceMaintenanceModel> observer) {
			GetDeviceInformationResponse info = null;
			yield return session.GetDeviceInformation().Handle(x => info = x);
			DebugHelper.Assert(info != null);

			DeviceObservable device = null;
			yield return session.GetDeviceClient().Handle(x => device = x);
			DebugHelper.Assert(device != null);

			//StartFirmwareUpgradeResponse upgradeInfo = null;
			//yield return device.StartFirmwareUpgrade().Handle(x => upgradeInfo = x).IgnoreError();

			//if (upgradeInfo != null) {
				firmwareUpgradeSupported = true;
				//firmwareUploadUri = upgradeInfo.UploadUri;
			//}
			currentFirmwareVersion = info.FirmwareVersion;

			NotifyPropertyChanged(x => x.firmwareUpgradeSupported);
			NotifyPropertyChanged(x => x.firmwareUploadUri);
			NotifyPropertyChanged(x => x.currentFirmwareVersion);
			
			if (observer != null) {
				observer.OnNext(this);
			}
		}

		public IObservable<string> Reboot() {
			return session.SystemReboot().ObserveOn(SynchronizationContext.Current);
		}

		protected override IEnumerable<IObservable<object>> ApplyChangesImpl(Session session, IObserver<DeviceMaintenanceModel> observer) {
			DeviceObservable device = null;
			yield return session.GetDeviceClient().Handle(x => device = x);
			DebugHelper.Assert(device != null);
			StartFirmwareUpgradeResponse upgradeInfo = null;
			yield return device.StartFirmwareUpgrade().Handle(x => upgradeInfo = x);
			DebugHelper.Assert(upgradeInfo != null);

			//if (upgradeInfo.UploadDelay > 0) {
			//    yield return Observable.Delay(upgradeInfo.UploadDelay);
			//}

			using (var fs = new FileStream(firmwarePath, FileMode.Open)) {
				
				var requestUri = new Uri(upgradeInfo.UploadUri);
				if (requestUri.Scheme != Uri.UriSchemeHttp) {
					throw new NotSupportedException(String.Format("specified protocol ({0}) not suppoted", requestUri.Scheme));
				}
				var request = (HttpWebRequest)HttpWebRequest.Create(requestUri);

				request.Method = WebRequestMethods.Http.Post;
				//request.Method = WebRequestMethods.File.UploadFile;
				//request.Method = WebRequestMethods.Ftp.UploadFile;
				request.MaximumResponseHeadersLength = 10 * 1024;
				//request.UserAgent = "Mozilla/4.0 (compatible; VAMS)";
				request.ContentType = MediaTypeNames.Application.Octet; //"application/octet-stream"
				//request.Headers.Add("Content-Transfer-Encoding: binary");
				request.ContentLength = fs.Length;
				//request.SendChunked = true;
				request.Timeout = 60*60*1000;//60min
				request.ReadWriteTimeout = 60*60*1000;//60min
				request.ProtocolVersion = HttpVersion.Version10;

				Stream uploadStream = null;
				yield return Observable.FromAsyncPattern<Stream>(request.BeginGetRequestStream, request.EndGetRequestStream)().Handle(x=>uploadStream =x);
				Exception sendError = null;
				
				try {
					yield return ObservableStream.Copy(fs, uploadStream).Idle().HandleError(err=>sendError = err);			
				} finally {
					uploadStream.Close();
				}
				
				if (sendError !=null) {
					throw sendError;
				}
				HttpWebResponse response = null;
				yield return Observable.FromAsyncPattern<WebResponse>(request.BeginGetResponse, request.EndGetResponse)().Handle(x => response = (HttpWebResponse)x);
				if (response.StatusCode != HttpStatusCode.OK) {
					response.Close();
					throw new Exception("upload failed");
				}
				DebugHelper.Assert(response != null);
				response.Close();

			};
						
			if (observer != null) {
				observer.OnNext(this);
			}	
		}
		public bool firmwareUpgradeSupported {get; private set;}
		public string firmwareUploadUri	{get; private set;}
		public string currentFirmwareVersion {get; private set;}
		public string firmwarePath {get;set;}
	}
}
