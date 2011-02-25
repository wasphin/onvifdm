﻿#region License and Terms
//----------------------------------------------------------------------------------------------------------------
// Copyright (C) 2010 Synesis LLC and/or its subsidiaries. All rights reserved.
//
// Commercial Usage
// Licensees  holding  valid ONVIF  Device  Manager  Commercial  licenses may use this file in accordance with the
// ONVIF  Device  Manager Commercial License Agreement provided with the Software or, alternatively, in accordance
// with the terms contained in a written agreement between you and Synesis LLC.
//
// GNU General Public License Usage
// Alternatively, this file may be used under the terms of the GNU General Public License version 3.0 as published
// by  the Free Software Foundation and appearing in the file LICENSE.GPL included in the  packaging of this file.
// Please review the following information to ensure the GNU General Public License version 3.0 
// requirements will be met: http://www.gnu.org/copyleft/gpl.html.
// 
// If you have questions regarding the use of this file, please contact Synesis LLC at onvifdm@synesis.ru.
//----------------------------------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Permissions;
using System.Diagnostics;

using odm.utils.entities;
using odm.models;
using odm.utils;
using odm.utils.controls.wpfControls;

namespace odm.controls
{
    public partial class PropertyLiveVideo : BasePropertyControl{
		public override void ReleaseUnmanaged() {
			//_vidPlayer.ReleaseUnmanaged();
		}
		//VideoPlayerControl _vidPlayer;

		PropertyLiveVideoStrings _strings = new PropertyLiveVideoStrings();
		LiveVideoModel _devMod;

		public PropertyLiveVideo(LiveVideoModel devMod) {
            InitializeComponent();
			_devMod = devMod;

			this.Disposed += (sender, args) => {
				this.ReleaseAll();
			};

			Load += new EventHandler(PropertyLiveVideo_Load);
        }
		void PropertyLiveVideo_Load(object sender, EventArgs e) {
			_uriBox.ReadOnly = true;
			_uriBox.Text = _devMod.mediaUri;

			//Start Workaround
			try {
				CreateStandAloneVLC(_devMod.mediaUri, _devMod.encoderResolution);
				pBox = new UserPictureBox() { Dock = DockStyle.Fill};
				_wpfControl = new wpfViewer();
				_wpfHost.Child = _wpfControl;
				panel1.Controls.Add(pBox);
				_tmr = new Timer();
				_tmr.Interval = 10;
				_tmr.Tick += new EventHandler(_tmr_Tick);
				_tmr.Start();
			} catch (Exception err) {
				VideoOperationError(err.Message);
			}
			//Stop Workaround
			
			//_vidPlayer = new VideoPlayerControl(_devMod.mediaUri) { Dock = DockStyle.Fill };
			//panel1.Controls.Add(_vidPlayer);

			BackColor = ColorDefinition.colControlBackground;
			_title.BackColor = ColorDefinition.colTitleBackground;

			Localization();			
		}

		void Localization(){
			_title.CreateBinding(x => x.Text, _strings, x => x.title);
		}
		public override void ReleaseAll() {
			//if (_vidPlayer != null) {
			//    ReleaseUnmanaged();
			//    _vidPlayer.ReleaseAll();
			//}
			base.ReleaseAll();
		}
    }
}