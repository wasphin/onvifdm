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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using odm.models;
using odm.onvif;
using System.Threading;
using System.Xml;
using odm.utils;
using System.Xml.Xsl;
using System.IO;

namespace odm.ui.controls {
	/// <summary>
	/// Interaction logic for GetDumpControl.xaml
	/// </summary>
	public partial class GetDumpControl : UserControl {
		public GetDumpControl() {
			InitializeComponent();
		}
		
		private static object gate = new object();
		private static XslCompiledTransform s_xml2html = null;
		private static XslCompiledTransform xml2html {
			get {
				lock (gate) {
					if (s_xml2html == null) {
						var xslt = new XslCompiledTransform();

						var xmlReaderSettings = new XmlReaderSettings() {
							DtdProcessing = DtdProcessing.Parse
						};
						XsltSettings xsltSettings = new XsltSettings() {
							EnableScript = false,
							EnableDocumentFunction = false
						};

						using (var xmlReader = XmlReader.Create(@"xml2html/XmlToHtml10Basic.xslt", xmlReaderSettings)) {
							xslt.Load(xmlReader, xsltSettings, new XmlUrlResolver());
							xmlReader.Close();
						}
						s_xml2html = xslt;
					}
				}
				return s_xml2html;
			}
		}
		DumpModel model;
		public void Init(DumpModel devModel) {
			model = devModel;
			var html = new StringBuilder();
			var writer = new StringWriter(html);
			xml2html.Transform(devModel.xmlDump, null, writer);
			xmlViewer.NavigateToString(html.ToString());
		}

		private void saveDump_Click(object sender, RoutedEventArgs e) {
			var dlg = new Microsoft.Win32.SaveFileDialog();
			dlg.Filter = "XML file|*.xml";
			dlg.ShowDialog();
			try {
				model.xmlDump.Save(dlg.FileName);
			} catch (Exception err){
				dbg.Error(err);
			}
		}
	}
}