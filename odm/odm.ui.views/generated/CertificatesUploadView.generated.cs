﻿
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Windows.Input;
using odm.infra;
namespace odm.ui.activities {
	
	public partial class CertificatesUploadView{
		
		#region Model definition
		
		public interface IModelAccessor{
			string selection{get;set;}
			
		}
		public class Model: IModelAccessor, INotifyPropertyChanged{
			
			public Model(
				string[] items
			){
				
				this.items = items;
			}
			private Model(){
			}
			

			public static Model Create(
				string[] items,
				string selection
			){
				var _this = new Model();
				
				_this.items = items;
				_this.origin.selection = selection;
				_this.RevertChanges();
				
				return _this;
			}
		
				private SimpleChangeTrackable<string> m_selection;
				public string[] items{get;private set;}

			private class OriginAccessor: IModelAccessor {
				private Model m_model;
				public OriginAccessor(Model model) {
					m_model = model;
				}
				string IModelAccessor.selection {
					get {return m_model.m_selection.origin;}
					set {m_model.m_selection.origin = value;}
				}
				
			}
			private PropertyChangedEventHandler cb;
			private object sync = new object();
			public event PropertyChangedEventHandler PropertyChanged {
				add {
					lock(sync){
						cb += value;
					}
				}
				remove {
					lock(sync){
						cb -= value;
					}
				}
			}
			private void NotifyPropertyChanged(string propertyName){
				PropertyChangedEventHandler cb_copy = null;
				lock(sync){
					if(cb!=null){
						cb_copy = cb.Clone() as PropertyChangedEventHandler;
					}
				}
				if (cb_copy != null) {
					cb_copy(this, new PropertyChangedEventArgs(propertyName));
				}
			}
			
			public string selection  {
				get {return m_selection.current;}
				set {
					if(m_selection.current != value) {
						m_selection.current = value;
						NotifyPropertyChanged("selection");
					}
				}
			}
			
			public void AcceptChanges() {
				m_selection.AcceptChanges();
				
			}

			public void RevertChanges() {
				m_selection.RevertChanges();
				
			}

			public bool isModified {
				get {
					if(m_selection.isModified)return true;
					
					return false;
				}
			}

			public IModelAccessor current {
				get {return this;}
				
			}

			public IModelAccessor origin {
				get {return new OriginAccessor(this);}
				
			}
		}
			
		#endregion
	
		#region Result definition
		public abstract class Result{
			private Result() { }
			
			public abstract T Handle<T>(
				
				Func<T> upload,
				Func<T> activate,
				Func<T> deActivate
			);
	
			public bool IsUpload(){
				return AsUpload() != null;
			}
			public virtual Upload AsUpload(){ return null; }
			public class Upload : Result {
				public Upload(){
					
				}
				
				public override Upload AsUpload(){ return this; }
				
				public override T Handle<T>(
				
					Func<T> upload,
					Func<T> activate,
					Func<T> deActivate
				){
					return upload(
						
					);
				}
	
			}
			
			public bool IsActivate(){
				return AsActivate() != null;
			}
			public virtual Activate AsActivate(){ return null; }
			public class Activate : Result {
				public Activate(){
					
				}
				
				public override Activate AsActivate(){ return this; }
				
				public override T Handle<T>(
				
					Func<T> upload,
					Func<T> activate,
					Func<T> deActivate
				){
					return activate(
						
					);
				}
	
			}
			
			public bool IsDeActivate(){
				return AsDeActivate() != null;
			}
			public virtual DeActivate AsDeActivate(){ return null; }
			public class DeActivate : Result {
				public DeActivate(){
					
				}
				
				public override DeActivate AsDeActivate(){ return this; }
				
				public override T Handle<T>(
				
					Func<T> upload,
					Func<T> activate,
					Func<T> deActivate
				){
					return deActivate(
						
					);
				}
	
			}
			
		}
		#endregion

		public ICommand UploadCommand{ get; private set; }
		public ICommand ActivateCommand{ get; private set; }
		public ICommand DeActivateCommand{ get; private set; }
		
		IActivityContext<Result> activityContext = null;
		SingleAssignmentDisposable activityCancellationSubscription = new SingleAssignmentDisposable();
		bool activityCompleted = false;
		//activity has been completed
		event Action OnCompleted = null;
		//activity has been failed
		event Action<Exception> OnError = null;
		//activity has been completed successfully
		event Action<Result> OnSuccess = null;
		//activity has been canceled
		event Action OnCancelled = null;
		
		public CertificatesUploadView(Model model, IActivityContext<Result> activityContext) {
			this.activityContext = activityContext;
			if(activityContext!=null){
				activityCancellationSubscription.Disposable = 
					activityContext.RegisterCancellationCallback(() => {
						EnsureAccess(() => {
							CompleteWith(() => {
								if(OnCancelled!=null){
									OnCancelled();
								}
							});
						});
					});
			}
			Init(model);
		}

		
		public void EnsureAccess(Action action){
			if(!CheckAccess()){
				Dispatcher.Invoke(action);
			}else{
				action();
			}
		}

		public void CompleteWith(Action cont){
			if(!activityCompleted){
				activityCompleted = true;
				cont();
				if(OnCompleted!=null){
					OnCompleted();
				}
				activityCancellationSubscription.Dispose();
			}
		}
		public void Success(Result result) {
			CompleteWith(() => {
				if(activityContext!=null){
					activityContext.Success(result);
				}
				if(OnSuccess!=null){
					OnSuccess(result);
				}
			});
		}
		public void Error(Exception error) {
			CompleteWith(() => {
				if(activityContext!=null){
					activityContext.Error(error);
				}
				if(OnError!=null){
					OnError(error);
				}
			});
		}
		public void Cancel() {
			CompleteWith(() => {
				if(activityContext!=null){
					activityContext.Cancel();
				}
				if(OnCancelled!=null){
					OnCancelled();
				}
			});
		}
		public void Success(Func<Result> resultFactory) {
			CompleteWith(() => {
				var result = resultFactory();
				if(activityContext!=null){
					activityContext.Success(result);
				}
				if(OnSuccess!=null){
					OnSuccess(result);
				}
			});
		}
		public void Error(Func<Exception> errorFactory) {
			CompleteWith(() => {
				var error = errorFactory();
				if(activityContext!=null){
					activityContext.Error(error);
				}
				if(OnError!=null){
					OnError(error);
				}
			});
		}
		public void Cancel(Action action) {
			CompleteWith(() => {
				action();
				if(activityContext!=null){
					activityContext.Cancel();
				}
				if(OnCancelled!=null){
					OnCancelled();
				}
			});
		}
		
	}
}
