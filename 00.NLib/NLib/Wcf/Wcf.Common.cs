#define ENABLE_SERVER_ICOMM_OBJ
#define ENABLE_CLIENT_ICOMM_OBJ

#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-07-22
=================
- Wcf Framework is redesign.
  - Add supports ICommunicationObject for WcfServiceClient<I>.
  - Add Helper protected method to call Wcf function in WcfServiceClient<I>.
======================================================================================================================
Update 2015-07-20
=================
- Wcf Framework is redesign.
  - WcfServiceHost and WcfBasicServer is redesigned and rename some related classes.
  - Add supports ICommunicationObject for WcfServiceHost.
  - WcfServiceHost supports InstanceContextMode.Single, InstanceContextMode.PerCall
	and InstanceContextMode.PerSession.
  - Add Wcf Extension methods class.
  - Add Wcf Utils class.
  - Add supports MEX Binding.
  - Add supports IncludeExceptionDetailInFaults behavior.
======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace NLib.Wcf
{
	#region BindingTypes

	/// <summary>
	/// Wcf Service Binding Types.
	/// </summary>
	[DataContract]
	public enum BindingTypes : byte
	{
		/// <summary>
		/// Pipe
		/// </summary>
		[EnumMember]
		Pipe,
		/// <summary>
		/// Tcp
		/// </summary>
		[EnumMember]
		Tcp,
		/// <summary>
		/// Http
		/// </summary>
		[EnumMember]
		Http
	}

	#endregion

	#region WcfAddress

	/// <summary>
	/// Wcf Service Address. Used for keep information that need to interchange between
	/// server host and client channel.
	/// </summary>
	[DataContract]
	public class WcfAddress
	{
		#region Internal Variables

		private BindingTypes _bindType = BindingTypes.Pipe;
		private string _hostName = "localhost";
		private int _portNumber = 0;
		private string _serviveName = string.Empty;

		#endregion

		#region Public Methods

		/// <summary>
		/// Get EndPoint Address.
		/// </summary>
		/// <returns>
		/// Returns the end point address that used for create ServiceHost or 
		/// Client Proxy access.
		/// </returns>
		public string GetEndPointAddress()
		{
			string result = string.Empty;
			switch (_bindType)
			{
				case BindingTypes.Http:
					#region Http
					{
						if (_portNumber == 0)
						{
							result = string.Format("http://{0}/{1}",
								_hostName, _serviveName);
						}
						else
						{
							result = string.Format("http://{0}:{1}/{2}",
								_hostName, _portNumber, _serviveName);
						}
					}
					#endregion
					break;
				case BindingTypes.Tcp:
					#region Tcp
					{
						if (_portNumber == 0)
						{
							result = string.Format("net.tcp://{0}/{1}",
								_hostName, _serviveName);
						}
						else
						{
							result = string.Format("net.tcp://{0}:{1}/{2}",
								_hostName, _portNumber, _serviveName);
						}
					}
					#endregion
					break;
				case BindingTypes.Pipe:
				default:
					#region Pipe (default)
					{
						if (_portNumber == 0)
						{
							result = string.Format("net.pipe://{0}/{1}",
								_hostName, _serviveName);
						}
						else
						{
							result = string.Format("net.pipe://{0}:{1}/{2}",
								_hostName, _portNumber, _serviveName);
						}
					}
					#endregion
					break;
			}
			// return the result.
			return result;
		}
		/// <summary>
		/// Gets Channel Binding.
		/// </summary>
		/// <returns>Returns instance of Channels's Binding instance.</returns>
		public System.ServiceModel.Channels.Binding GetChannelBinding()
		{
			System.ServiceModel.Channels.Binding result = null;
			switch (_bindType)
			{
				case BindingTypes.Http:
					{
						BasicHttpBinding binding = new BasicHttpBinding();
						// set max all value
						binding.MaxBufferSize = Int32.MaxValue;
						binding.MaxReceivedMessageSize = Int32.MaxValue;
						binding.MaxBufferPoolSize = Int32.MaxValue;
						binding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
						binding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue;
						binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
						binding.ReaderQuotas.MaxDepth = 32;
						binding.ReaderQuotas.MaxNameTableCharCount = Int32.MaxValue;
						// set as result
						result = binding;
					}
					break;
				case BindingTypes.Tcp:
					{
						NetTcpBinding binding = new NetTcpBinding();
						// set max all value
						binding.MaxBufferSize = Int32.MaxValue;
						binding.MaxReceivedMessageSize = Int32.MaxValue;
						binding.MaxBufferPoolSize = Int32.MaxValue;
						binding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
						binding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue;
						binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
						binding.ReaderQuotas.MaxDepth = 32;
						binding.ReaderQuotas.MaxNameTableCharCount = Int32.MaxValue;
						// set as result
						result = binding;
					}
					break;
				case BindingTypes.Pipe:
				default:
					{
						NetNamedPipeBinding binding = new NetNamedPipeBinding();
						// set max all value
						binding.MaxBufferSize = Int32.MaxValue;
						binding.MaxReceivedMessageSize = Int32.MaxValue;
						binding.MaxBufferPoolSize = Int32.MaxValue;
						binding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
						binding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue;
						binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
						binding.ReaderQuotas.MaxDepth = 32;
						binding.ReaderQuotas.MaxNameTableCharCount = Int32.MaxValue;
						// set as result
						result = binding;
					}
					break;
			}
			return result;
		}
		/// <summary>
		/// Gets Mex Binding.
		/// </summary>
		/// <returns>Returns instance of Mex's Binding instance.</returns>
		public System.ServiceModel.Channels.Binding GetMexBinding()
		{
			System.ServiceModel.Channels.Binding result = null;
			switch (_bindType)
			{
				case BindingTypes.Http:
					{
						result = MetadataExchangeBindings.CreateMexHttpBinding();
					}
					break;
				case BindingTypes.Tcp:
					{
						result = MetadataExchangeBindings.CreateMexTcpBinding();
					}
					break;
				case BindingTypes.Pipe:
				default:
					{
						result = MetadataExchangeBindings.CreateMexNamedPipeBinding();                        
					}
					break;
			}
			return result;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or set service binding type.
		/// </summary>
		[DataMember]
		public BindingTypes BindingType
		{
			get { return _bindType; }
			set
			{
				if (_bindType != value)
				{
					_bindType = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets host name. like localhost.
		/// </summary>
		[DataMember]
		public string HostName
		{
			get { return _hostName; }
			set
			{
				if (_hostName != value)
				{
					_hostName = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets Port Number. used zero if not need portnumber in binding address.
		/// </summary>
		[DataMember]
		public int PortNumber
		{
			get { return _portNumber; }
			set
			{
				if (_portNumber != value)
				{
					_portNumber = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets service's name that is suffix after base address.
		/// </summary>
		[DataMember]
		public string ServiceName
		{
			get { return _serviveName; }
			set
			{
				if (_serviveName != value)
				{
					_serviveName = value;
				}
			}
		}

		#endregion
	}

	#endregion

	#region WcfUtils
	
	/// <summary>
	/// Wcf Common utils class.
	/// </summary>
	public class WcfUtils
	{
		#region Constructor (Hide)
		
		/// <summary>
		/// Constructor.
		/// </summary>
		private WcfUtils() : base() { }

		#endregion

		#region Static Method(s)

		#endregion
	}

	#endregion

	#region WcfExtensionMethods

	/// <summary>
	/// The Wcf Extension methods class.
	/// </summary>
	public static class WcfExtensionMethods
	{
		#region IsImplements
		
		/// <summary>
		/// Checks is target instance type is implements the interface type.
		/// </summary>
		/// <typeparam name="T">The Type of Instance to check.</typeparam>
		/// <param name="instance">The of Instance to check.</param>
		/// <param name="interfaceType">The interface type.</param>
		/// <returns>Returns true if the target instance type is implements the interface type.</returns>
		public static bool IsImplements<T>(this T instance, Type interfaceType)
		{
			bool result = false;

			if (null == instance || null == interfaceType || !interfaceType.IsInterface)
				return result;

			string intfTypeName = interfaceType.FullName;
			// Find interface
			Type objIntfType = instance.GetType().GetInterface(intfTypeName, true);
			result = (null != objIntfType && objIntfType == interfaceType);

			return result;
		}

		#endregion

		#region HasServiceContract

		/// <summary>
		/// Checks if the specificed type is contains ServiceContract attribute.
		/// </summary>
		/// <param name="interfaceType">The type to checks is contains ServiceContract attribute.</param>
		/// <returns>Returns true  if interfaceType is interface and has ServiceContract attribute.</returns>
		public static bool HasServiceContract(this Type interfaceType)
		{
			bool result = false;
			if (null != interfaceType && interfaceType.IsInterface)
			{
				object[] attrs = interfaceType.GetCustomAttributes(true);

				foreach (object attr in attrs)
				{
					if (attr is ServiceContractAttribute)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		#endregion
	}

	#endregion

	#region WcfServiceHost<I>

	/// <summary>
	/// Wcf Generic Service Host.
	/// </summary>
	/// <typeparam name="I">The Wcf interface type.</typeparam>
#if ENABLE_SERVER_ICOMM_OBJ
	public class WcfServiceHost<I> : IDisposable, ICommunicationObject
#else
	public class WcfServiceHost<I> : IDisposable
#endif
	{
		#region Internal Variables

		private WcfAddress _address = new WcfAddress();
		private object _instance = null;
		private Type _servIntfType = null;
		private ServiceHost _host = null;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public WcfServiceHost()
			: base()
		{
		}
		/// <summary>
		/// Destructor.
		/// </summary>
		~WcfServiceHost()
		{
			Dispose(false);
			_address = null;
		}

		#endregion

		#region Private Methods

		#region Event Raisers

		private void RaiseOnShutdown()
		{
			if (null != OnShutdown)
			{
				ApplicationManager.Instance.Invoke(OnShutdown, this, EventArgs.Empty);
			}
		}

		#endregion

		#endregion

		#region ICommunicationObject Implements
#if ENABLE_SERVER_ICOMM_OBJ
		/// <summary>
		/// Causes a communication object to transition immediately from its current state into the 
		/// closed state.
		/// </summary>
		void ICommunicationObject.Abort()
		{
			if (null != _host)
			{
				_host.Abort();
			}
		}
		/// <summary>
		/// Begins an asynchronous operation to close a communication object with a specified timeout.
		/// </summary>
		/// <param name="timeout"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		IAsyncResult ICommunicationObject.BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
		{
			if (null != _host)
			{
				return _host.BeginClose(timeout, callback, state);
			}
			else return null;
		}
		/// <summary>
		/// Begins an asynchronous operation to close a communication object.
		/// </summary>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		IAsyncResult ICommunicationObject.BeginClose(AsyncCallback callback, object state)
		{
			if (null != _host)
			{
				return _host.BeginClose(callback, state);
			}
			else return null;
		}
		/// <summary>
		/// Begins an asynchronous operation to open a communication object within a specified interval 
		/// of time.
		/// </summary>
		/// <param name="timeout"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		IAsyncResult ICommunicationObject.BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
		{
			if (null != _host)
			{
				return _host.BeginOpen(timeout, callback, state);
			}
			else return null;
		}
		/// <summary>
		/// Begins an asynchronous operation to open a communication object.
		/// </summary>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		IAsyncResult ICommunicationObject.BeginOpen(AsyncCallback callback, object state)
		{
			if (null != _host)
			{
				return _host.BeginOpen(callback, state);
			}
			else return null;
		}
		/// <summary>
		/// Causes a communication object to transition from its current state into the closed state.
		/// </summary>
		/// <param name="timeout"></param>
		void ICommunicationObject.Close(TimeSpan timeout)
		{
			if (null != _host)
			{
				_host.Close(timeout);
			}
		}
		/// <summary>
		/// Causes a communication object to transition from its current state into the closed state.
		/// </summary>
		void ICommunicationObject.Close()
		{
			if (null != _host)
			{
				_host.Close();
			}
		}
		/// <summary>
		/// Completes an asynchronous operation to close a communication object.
		/// </summary>
		/// <param name="result"></param>
		void ICommunicationObject.EndClose(IAsyncResult result)
		{
			if (null != _host)
			{
				_host.EndClose(result);
			}
		}
		/// <summary>
		/// Completes an asynchronous operation to open a communication object.
		/// </summary>
		/// <param name="result"></param>
		void ICommunicationObject.EndOpen(IAsyncResult result)
		{
			if (null != _host)
			{
				_host.EndOpen(result);
			}
		}
		/// <summary>
		/// Causes a communication object to transition from the created state into the opened state 
		/// within a specified interval of time.
		/// </summary>
		/// <param name="timeout"></param>
		void ICommunicationObject.Open(TimeSpan timeout)
		{
			if (null != _host)
			{
				_host.Open(timeout);
			}
		}
		/// <summary>
		/// Causes a communication object to transition from the created state into the opened state.
		/// </summary>
		void ICommunicationObject.Open()
		{
			if (null != _host)
			{
				_host.Open();
			}
		}
		/// <summary>
		/// Gets the current state of the communication-oriented object.
		/// </summary>
		CommunicationState ICommunicationObject.State
		{
			get
			{
				if (null != _host)
					return _host.State;
				else return CommunicationState.Closed;
			}
		}
		/// <summary>
		/// Occurs when the communication object completes its transition from the closing state 
		/// into the closed state.
		/// </summary>
		event EventHandler ICommunicationObject.Closed
		{
			add { if (null != _host) _host.Closed += value; }
			remove { if (null != _host) _host.Closed -= value; }
		}
		/// <summary>
		/// Occurs when the communication object first enters the closing state.
		/// </summary>
		event EventHandler ICommunicationObject.Closing
		{
			add { if (null != _host) _host.Closing += value; }
			remove { if (null != _host) _host.Closing -= value; }
		}
		/// <summary>
		/// Occurs when the communication object first enters the faulted state.
		/// </summary>
		event EventHandler ICommunicationObject.Faulted
		{
			add { if (null != _host) _host.Faulted += value; }
			remove { if (null != _host) _host.Faulted -= value; }
		}
		/// <summary>
		/// Occurs when the communication object completes its transition 
		/// from the opening state into the opened state.
		/// </summary>
		event EventHandler ICommunicationObject.Opened
		{
			add { if (null != _host) _host.Opened += value; }
			remove { if (null != _host) _host.Opened -= value; }
		}
		/// <summary>
		/// Occurs when the communication object first enters the opening state.
		/// </summary>
		event EventHandler ICommunicationObject.Opening
		{
			add { if (null != _host) _host.Opening += value; }
			remove { if (null != _host) _host.Opening -= value; }
		}
#endif
		#endregion

		#region Public Methods

		#region Dispose

		/// <summary>
		/// Dispose.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}
		/// <summary>
		/// Dispose.
		/// </summary>
		/// <param name="disposing">True if in disposing process.</param>
		protected virtual void Dispose(bool disposing)
		{
			Shutdown();
			if (disposing)
			{
			}
		}

		#endregion

		#region Start

		/// <summary>
		/// Start service.
		/// </summary>
		public void Start()
		{
			MethodBase med = MethodBase.GetCurrentMethod();

			#region Checks

			if (null != _host)
			{
				// Already started
				med.Info("Service already started.");
				return;
			}
			if (null == _address)
			{
				// Already started
				med.Err("The WcfAddress instance is null.");
				return;
			}
			if (string.IsNullOrWhiteSpace(_address.HostName) ||
				string.IsNullOrWhiteSpace(_address.ServiceName))
			{
				// No service name assigned.
				med.Err("No host name or service name assigned.");
				return;
			}
			// Check has service contract attribute on the interface type.
			if (!this.WcfInterfaceType.HasServiceContract())
			{
				med.Err("The interface is not has service contract attribute.");
				return;
			}

			// Checks instance is implements the interface or not.
			bool isImplementInterface = _instance.IsImplements(this.WcfInterfaceType);

			if (null == _instance || !isImplementInterface)
			{
				med.Err("No instance of specificed service contract interface assigned.");
				return;
			}

			#endregion

			System.ServiceModel.Channels.Binding channel = null;
			System.ServiceModel.Channels.Binding mex = null;
			string endPointAddr = string.Empty;

			try
			{
				if (null != _instance)
				{
					// find channel binding type.
					channel = _address.GetChannelBinding();
					mex = _address.GetMexBinding();
					// find end point address
					endPointAddr = _address.GetEndPointAddress();

					// create new service host
					// 1. Required The implementation class has attribute InstanceContextMode.Single only
					//_host = new ServiceHost(_instance);
					// 2. For General purpose.
					_host = new ServiceHost(_instance.GetType()); // The Uri will add via end point
				}

				if (null != _host && null != channel &&
					!string.IsNullOrWhiteSpace(endPointAddr))
				{
					#region Add End Point
					
					// Add service end point
					_host.AddServiceEndpoint(_servIntfType, channel, endPointAddr);

					#endregion

					#region Enable MEX

					// Check to see if the service host already has a ServiceMetadataBehavior
					ServiceMetadataBehavior smb = _host.Description.Behaviors
						.Find<ServiceMetadataBehavior>();
					// If not, add one
					if (smb == null)
					{
						smb = new ServiceMetadataBehavior();
						smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
						// Enable HttpGet to retrived wsdl schema for Http binding only.
						if (this.Url.BindingType == BindingTypes.Http)
						{
							smb.HttpGetEnabled = true;
							smb.HttpGetUrl = new Uri(endPointAddr);
						}
						else
						{
							smb.HttpGetEnabled = false;
						}
						_host.Description.Behaviors.Add(smb);
					}
					// Add MEX endpoint
					_host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, mex,
						endPointAddr + @"/mex");

					#endregion

					#region Enable IncludeExceptionDetailInFaults

					// Enable Debug Behavior for Http only.
					if (this.Url.BindingType == BindingTypes.Http)
					{
						// Check to see if the service host already has a ServiceDebugBehavior
						ServiceDebugBehavior debug = _host.Description.Behaviors
							.Find<ServiceDebugBehavior>();

						// if not found - add behavior with setting turned on 
						if (debug == null)
						{
							_host.Description.Behaviors.Add(new ServiceDebugBehavior()
							{
								IncludeExceptionDetailInFaults = true
							});
						}
						else
						{
							// make sure setting is turned ON
							if (!debug.IncludeExceptionDetailInFaults)
							{
								debug.IncludeExceptionDetailInFaults = true;
							}
						}
					}

					#endregion
					
					// open connection
					_host.Open();
				}
			}
			catch (Exception ex)
			{
				med.Err(ex);
				Shutdown();
			}
		}

		#endregion

		#region Shutdown

		/// <summary>
		/// Shutdown service.
		/// </summary>
		public void Shutdown()
		{
			MethodBase med = MethodBase.GetCurrentMethod();

			RaiseOnShutdown();

			if (null != _host)
			{
				try
				{
					_host.Close();
				}
				catch (Exception ex)
				{
					med.Err(ex);
				}
				finally
				{
					try { _host.Abort(); }
					catch { }
					finally { }
				}
			}
			// reset host
			_host = null;
			// reset instance.
			_instance = null;
		}

		#endregion

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets Wcf Service Interface Type.
		/// </summary>
		public Type WcfInterfaceType
		{
			get
			{
				if (null == _servIntfType)
				{
					_servIntfType = typeof(I);
				}
				return _servIntfType;
			}
		}
		/// <summary>
		/// Gets or sets current service instnace that implement actual service interface.
		/// </summary>
		public object ServiceInstance
		{
			get { return _instance; }
			set { _instance = value; }
		}
		/// <summary>
		/// Gets internal service host instance.
		/// </summary>
		public ServiceHost Host { get { return _host; } }
		/// <summary>
		/// Checks is service host is running.
		/// </summary>
		public bool IsRunning
		{
			get { return null != _host; }
		}
		/// <summary>
		/// Access Service Address instance.
		/// </summary>
		public WcfAddress Url
		{
			get { return _address; }
		}

		#endregion

		#region Public Events

		/// <summary>
		/// OnShutdown event.
		/// </summary>
		public event System.EventHandler OnShutdown;

		#endregion
	}

	#endregion

	#region WcfServiceStatus

	/// <summary>
	/// Provide basic Wcf service status.
	/// </summary>
	[DataContract]
	public class WcfServiceStatus
	{
		/// <summary>
		/// Checks is service is still alive.
		/// </summary>
		[DataMember]
		public bool IsAlive { get; set; }
	}

	#endregion

	#region IWcfBasicService

	/// <summary>
	/// IWcfBasicService interface.
	/// </summary>
	[ServiceContract]
	public interface IWcfBasicService
	{
		/// <summary>
		/// Gets current service status.
		/// </summary>
		/// <returns>Returns the service status information.</returns>
		[OperationContract]
		WcfServiceStatus GetServiceStatus();
	}

	#endregion

	#region WcfBasicServer<T>

	/// <summary>
	/// The Wcf Basic Server.
	/// </summary>
	/// <typeparam name="T">The instance type.</typeparam>
	/// <typeparam name="I">The Wcf interface type.</typeparam>
	public abstract class WcfBasicServer<T, I> : IDisposable
	{
		#region Internal Variables

		private WcfServiceHost<I> _host = null;
		private T _serviceInst = default(T);

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public WcfBasicServer()
			: base()
		{
			_host = new WcfServiceHost<I>();
			_host.OnShutdown += new EventHandler(_host_OnShutdown);

			Initialized(_host);
		}
		/// <summary>
		/// Destructor.
		/// </summary>
		~WcfBasicServer()
		{
			Dispose();
		}

		#endregion

		#region Private Methods

		#region Host Handlers

		void _host_OnShutdown(object sender, EventArgs e)
		{
			RaiseOnShutdown();
		}

		#endregion

		#region Event Raisers

		private void RaiseOnShutdown()
		{
			if (null != OnShutdown)
			{
				ApplicationManager.Instance.Invoke(OnShutdown, this, EventArgs.Empty);
			}
		}

		#endregion

		#region Initialized/OnInitialized

		/// <summary>
		/// Main Initialized methods.
		/// </summary>
		/// <param name="host">The host instance.</param>
		private void Initialized(WcfServiceHost<I> host)
		{
			Type servIntf = typeof(I);
			if (servIntf.HasServiceContract())
			{
				// current classs is implements service contract interface.
				host.ServiceInstance = _serviceInst;
			}
			OnInitialized(host);
		}
		/// <summary>
		/// OnInitialized method. Overrides to init optional data during create host instance.
		/// </summary>
		/// <param name="host">The host instance.</param>
		protected virtual void OnInitialized(WcfServiceHost<I> host)
		{

		}

		#endregion

		#endregion

		#region Public Methods

		#region Dispose

		/// <summary>
		/// Dispose.
		/// </summary>
		public void Dispose()
		{
			if (null != _host)
			{
				_host.OnShutdown -= new EventHandler(_host_OnShutdown);
				_host.Dispose();
			}
			_host = null;
		}

		#endregion

		#region Start

		/// <summary>
		/// Start.
		/// </summary>
		public void Start()
		{
			MethodBase med = MethodBase.GetCurrentMethod();

			if (null != _host)
			{
				_host.ServiceInstance = this.ServiceInstance;
				_host.Start();
			}
			else
			{
				// host is null or already disposed
				med.Info("host is null or already disposed.");
			}
		}

		#endregion

		#region Shutdown

		/// <summary>
		/// Shutdown.
		/// </summary>
		public void Shutdown()
		{
			MethodBase med = MethodBase.GetCurrentMethod();

			if (null != _host)
			{
				_host.Shutdown();
			}
			else
			{
				// host is null or already disposed
				med.Info("host is null or already disposed.");
			}
		}

		#endregion

		#endregion

		#region Public Properties

		/// <summary>
		/// Checks is service host is running.
		/// </summary>
		public bool IsRunning
		{
			get { return null != _host && _host.IsRunning; }
		}
		/// <summary>
		/// Checks is host instance is already disposed.
		/// </summary>
		public bool IsDisposed
		{
			get { return null != _host; }
		}
		/// <summary>
		/// Access Service Address instance.
		/// </summary>
		public WcfAddress Url
		{
			get
			{
				MethodBase med = MethodBase.GetCurrentMethod();
				if (null == _host)
				{
					med.Info("host is null or already disposed.");
					return null; // host is null or already disposed
				}
				return _host.Url;
			}
		}
		/// <summary>
		/// Gets or sets current service instnace that implement actual service interface.
		/// </summary>
		public T ServiceInstance
		{
			get
			{
				MethodBase med = MethodBase.GetCurrentMethod();
				if (null == _host)
				{
					med.Info("host is null or already disposed.");
					return default(T); // host is null or already disposed
				}
				if (null == _host.ServiceInstance)
				{
					Initialized(_host);
				}
				return _serviceInst;
			}
			set
			{
				MethodBase med = MethodBase.GetCurrentMethod();
				if (null == _host)
				{
					med.Info("host is null or already disposed.");
					return; // host is null or already disposed
				}
				_serviceInst = value;
				if (null == _host.ServiceInstance)
				{
					Initialized(_host);
				}
			}
		}
		/// <summary>
		/// Gets internal service host instance.
		/// </summary>
		public ServiceHost Host 
		{ 
			get 
			{
				if (null != _host)
					return _host.Host;
				else return null;
			} 
		}

		#endregion

		#region Public Events

		/// <summary>
		/// OnShutdown event.
		/// </summary>
		public event System.EventHandler OnShutdown;

		#endregion
	}

	#endregion

	#region WcfServiceClient<I>

	/// <summary>
	/// Wcf Service Client.
	/// </summary>
	/// <typeparam name="I">The service interface type.</typeparam>
#if ENABLE_CLIENT_ICOMM_OBJ
	public class WcfServiceClient<I> : IDisposable, ICommunicationObject
#else
	public class WcfServiceClient<I> : IDisposable
#endif
	{
		#region Internal Variables

		private WcfAddress _address = new WcfAddress();
		private ChannelFactory<I> _factory = null;
		private I _channel = default(I);

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public WcfServiceClient()
			: base()
		{
		}
		/// <summary>
		/// Destructor.
		/// </summary>
		~WcfServiceClient()
		{
			Dispose(false);
			_address = null;
		}

		#endregion

		#region Private Methods

		#region Event Raisers

		private void RaiseOnDisconnecting()
		{
			if (null != OnDisconnecting)
			{
				ApplicationManager.Instance.Invoke(OnDisconnecting, this, EventArgs.Empty);
			}
		}

		#endregion

		#endregion

		#region ICommunicationObject Implements
#if ENABLE_CLIENT_ICOMM_OBJ
		/// <summary>
		/// Causes a communication object to transition immediately from its current state into the 
		/// closed state.
		/// </summary>
		void ICommunicationObject.Abort()
		{
			if (null != _factory)
			{
				_factory.Abort();
			}
		}
		/// <summary>
		/// Begins an asynchronous operation to close a communication object with a specified timeout.
		/// </summary>
		/// <param name="timeout"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		IAsyncResult ICommunicationObject.BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
		{
			if (null != _factory)
			{
				return _factory.BeginClose(timeout, callback, state);
			}
			else return null;
		}
		/// <summary>
		/// Begins an asynchronous operation to close a communication object.
		/// </summary>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		IAsyncResult ICommunicationObject.BeginClose(AsyncCallback callback, object state)
		{
			if (null != _factory)
			{
				return _factory.BeginClose(callback, state);
			}
			else return null;
		}
		/// <summary>
		/// Begins an asynchronous operation to open a communication object within a specified interval 
		/// of time.
		/// </summary>
		/// <param name="timeout"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		IAsyncResult ICommunicationObject.BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
		{
			if (null != _factory)
			{
				return _factory.BeginOpen(timeout, callback, state);
			}
			else return null;
		}
		/// <summary>
		/// Begins an asynchronous operation to open a communication object.
		/// </summary>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		IAsyncResult ICommunicationObject.BeginOpen(AsyncCallback callback, object state)
		{
			if (null != _factory)
			{
				return _factory.BeginOpen(callback, state);
			}
			else return null;
		}
		/// <summary>
		/// Causes a communication object to transition from its current state into the closed state.
		/// </summary>
		/// <param name="timeout"></param>
		void ICommunicationObject.Close(TimeSpan timeout)
		{
			if (null != _factory)
			{
				_factory.Close(timeout);
			}
		}
		/// <summary>
		/// Causes a communication object to transition from its current state into the closed state.
		/// </summary>
		void ICommunicationObject.Close()
		{
			if (null != _factory)
			{
				_factory.Close();
			}
		}
		/// <summary>
		/// Completes an asynchronous operation to close a communication object.
		/// </summary>
		/// <param name="result"></param>
		void ICommunicationObject.EndClose(IAsyncResult result)
		{
			if (null != _factory)
			{
				_factory.EndClose(result);
			}
		}
		/// <summary>
		/// Completes an asynchronous operation to open a communication object.
		/// </summary>
		/// <param name="result"></param>
		void ICommunicationObject.EndOpen(IAsyncResult result)
		{
			if (null != _factory)
			{
				_factory.EndOpen(result);
			}
		}
		/// <summary>
		/// Causes a communication object to transition from the created state into the opened state 
		/// within a specified interval of time.
		/// </summary>
		/// <param name="timeout"></param>
		void ICommunicationObject.Open(TimeSpan timeout)
		{
			if (null != _factory)
			{
				_factory.Open(timeout);
			}
		}
		/// <summary>
		/// Causes a communication object to transition from the created state into the opened state.
		/// </summary>
		void ICommunicationObject.Open()
		{
			if (null != _factory)
			{
				_factory.Open();
			}
		}
		/// <summary>
		/// Gets the current state of the communication-oriented object.
		/// </summary>
		CommunicationState ICommunicationObject.State
		{
			get
			{
				if (null != _factory)
					return _factory.State;
				else return CommunicationState.Closed;
			}
		}
		/// <summary>
		/// Occurs when the communication object completes its transition from the closing state 
		/// into the closed state.
		/// </summary>
		event EventHandler ICommunicationObject.Closed
		{
			add { if (null != _factory) _factory.Closed += value; }
			remove { if (null != _factory) _factory.Closed -= value; }
		}
		/// <summary>
		/// Occurs when the communication object first enters the closing state.
		/// </summary>
		event EventHandler ICommunicationObject.Closing
		{
			add { if (null != _factory) _factory.Closing += value; }
			remove { if (null != _factory) _factory.Closing -= value; }
		}
		/// <summary>
		/// Occurs when the communication object first enters the faulted state.
		/// </summary>
		event EventHandler ICommunicationObject.Faulted
		{
			add { if (null != _factory) _factory.Faulted += value; }
			remove { if (null != _factory) _factory.Faulted -= value; }
		}
		/// <summary>
		/// Occurs when the communication object completes its transition 
		/// from the opening state into the opened state.
		/// </summary>
		event EventHandler ICommunicationObject.Opened
		{
			add { if (null != _factory) _factory.Opened += value; }
			remove { if (null != _factory) _factory.Opened -= value; }
		}
		/// <summary>
		/// Occurs when the communication object first enters the opening state.
		/// </summary>
		event EventHandler ICommunicationObject.Opening
		{
			add { if (null != _factory) _factory.Opening += value; }
			remove { if (null != _factory) _factory.Opening -= value; }
		}
#endif
		#endregion

		#region Public Methods

		#region Dispose

		/// <summary>
		/// Dispose.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}
		/// <summary>
		/// Dispose.
		/// </summary>
		/// <param name="disposing">True if in disposing state.</param>
		protected virtual void Dispose(bool disposing)
		{
			Disconnect();
			if (disposing)
			{
			}
		}

		#endregion

		#region Connect

		/// <summary>
		/// Connect to service service and create proxy channel.
		/// </summary>
		public void Connect()
		{
			MethodBase med = MethodBase.GetCurrentMethod();
			if (null != _factory)
			{
				med.Info("The ChannelFactory is already created.");
				return; // already created.
			}

			// find channel binding type.
			System.ServiceModel.Channels.Binding channel = _address.GetChannelBinding();
			// find end point address
			string endPointAddr = _address.GetEndPointAddress();

			_factory = new ChannelFactory<I>(
				 channel,
				 new EndpointAddress(endPointAddr));
			_channel = _factory.CreateChannel();
		}

		#endregion

		#region Disconnect

		/// <summary>
		/// Disconnect from server service and release channel.
		/// </summary>
		public void Disconnect()
		{
			MethodBase med = MethodBase.GetCurrentMethod();

			if (null != _channel)
			{
				RaiseOnDisconnecting();
			}
			_channel = default(I);
			if (null != _factory)
			{
				try
				{
					_factory.Close();
				}
				catch (Exception ex)
				{
					med.Err(ex, "Shutdown process detected error when close ChannelFactory.");
				}
				finally
				{
					_factory.Abort();
				}
			}
			_factory = null;
		}

		#endregion

		#endregion

		#region Public Properties

		/// <summary>
		/// Access Service Address instance.
		/// </summary>
		public WcfAddress Url
		{
			get { return _address; }
		}
		/// <summary>
		/// Gets Channel instance.
		/// </summary>
		public I Channel
		{
			get
			{
				return _channel;
			}
		}
		/// <summary>
		/// Checks is connnected to server service.
		/// </summary>
		public bool IsConnected
		{
			get { return null != _factory && _channel != null; }
		}

		#endregion

		#region Public Events

		/// <summary>
		/// OnDisconnecting event.
		/// </summary>
		public event System.EventHandler OnDisconnecting;

		#endregion
	}

	#endregion

	#region WCFBasicClient<I>

	/// <summary>
	/// Wcf Basic Client&lt;I&gt;
	/// </summary>
	/// <typeparam name="I">The server service interface type.</typeparam>
	public abstract class WcfBasicClient<I> : WcfServiceClient<I>
	{
		#region Private Methods

		delegate void VoidDelegate();

		/// <summary>
		/// Internal class.
		/// </summary>
		protected class CallState
		{
			/// <summary>
			/// Constructor.
			/// </summary>
			public CallState()
				: base()
			{
				this.Timeout = false;
				this.Error = null;
				this.Completed = false;
			}
			/// <summary>
			/// Gets or sets timeout.
			/// </summary>
			public bool Timeout { get; set; }
			/// <summary>
			/// Gets or sets exception object.
			/// </summary>
			public Exception Error { get; set; }
			/// <summary>
			/// Gets or sets completed state.
			/// </summary>
			public bool Completed { get; set; }
		}
		/// <summary>
		/// Execute action.
		/// </summary>
		/// <param name="work">The Action delegate.</param>
		/// <param name="timeout">Timeout in millisecond.</param>
		/// <returns>Returns CallState instance</returns>
		protected CallState Call(Action work, int timeout = 10000)
		{
			this.Connect();

			CallState state = new CallState();
			bool isError = false;
			DateTime startTime = DateTime.Now;

			#region Call Method Delegate Code

			VoidDelegate callMethod = delegate()
			{
				try
				{
					work();

					if (null != state)
					{
						state.Completed = true; // set exetute state to finished.
					}
				}
				catch (Exception ex)
				{
					isError = true;
					if (null != state)
					{
						state.Error = ex; // set error
						state.Completed = true; // set exetute state to finished.
					}
				}
			};

			#endregion

			#region Create Task to call method

			var tokenSource = new CancellationTokenSource();
			CancellationToken ct = tokenSource.Token;
			var task = Task.Factory.StartNew(() =>
			{
				tokenSource.Token.ThrowIfCancellationRequested();

				callMethod();

				if (ct.IsCancellationRequested)
				{
					try
					{
						tokenSource.Token.ThrowIfCancellationRequested();
					}
					catch (Exception ex)
					{
						// Operation is cancle
						Console.WriteLine(ex);
					}
				}
			}, ct);

			#endregion

			#region Check Is finished and update progress if required

			while (null != state && !state.Completed && /*!state.Canceled && */!isError)
			{
				TimeSpan tElapse = DateTime.Now - startTime;

				ApplicationManager.Instance.DoEvents();

				if (tElapse.TotalMilliseconds > timeout)
				//if (null != state && state.Canceled)
				{
					//Console.WriteLine("Cancel");
					Console.WriteLine("Timeout");
					try { tokenSource.Cancel(); }
					catch { }
					break;
				}

				Thread.Sleep(20);
			}

			#endregion

			this.Disconnect();

			return state;
		}

		#endregion
	}

	#endregion
}
