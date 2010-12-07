﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nvc.onvif;
using nvc.models;
using nvc.controlsUIProvider;

namespace nvc.controllers {
	public class PropertySystemLogController : BasePropertyController {
		public PropertySystemLogController() {

		}
		public override void CreateController(Session session, ChannelDescription chan) {
			CurrentChannel = chan;

			UIProvider.Instance.SystemLogProvider.InitView();
		}

		public override void ReleaseAll() {
			UIProvider.Instance.ReleaseSystemLogProvider();
		}

		protected override void ApplyChanges() { }
		protected override void CancelChanges() { }
		protected override void LoadControl() { }
	}
}
