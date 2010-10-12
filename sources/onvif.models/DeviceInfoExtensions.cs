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
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Net;
using System.Net.NetworkInformation;

using nvc.onvif;
using dev = onvif.services.device;
using nvc.utils;

namespace nvc.models {

	public static class DeviceInfoExtensions {

				
		public static IObservable<string> GetName(this Session session) {
			return session.GetScopes().Select(x=>NvcHelper.GetName(x.Select(s=>s.ScopeItem)));
		}



		static IEnumerable<IObservable<Object>> SetNameImpl(Session session, string name, IObserver<Unit> observer) {
			
			var scope_prefix = NvcHelper.SynesisNameScope;
			dev::Scope[] scopes = null;
			yield return session.GetScopes().Handle(x=>scopes = x);

			DebugHelper.Assert(scopes != null);

			var use_onvif_scope = scopes
				.Where(x => x.ScopeDef == dev::ScopeDefinition.Configurable)
				.Any(x => x.ScopeItem.StartsWith(NvcHelper.OnvifNameScope));

			if (use_onvif_scope) {
				scope_prefix = NvcHelper.OnvifNameScope;
			}

			var name_scope = String.Concat(scope_prefix, Uri.EscapeDataString(name));
			var scopes_to_set = scopes
				.Where(x => x.ScopeDef == dev::ScopeDefinition.Configurable)
				.Select(x => x.ScopeItem)
				.Where(x => !x.StartsWith(scope_prefix))
				.Append(name_scope)
				.ToArray();
			yield return session.SetScopes(scopes_to_set).Idle();
		}

		public static IObservable<Unit> SetName(this Session session, string name) {
			return Observable.Iterate<Unit>(observer => SetNameImpl(session, name, observer));
		}

		

	};
}
