// DecompilerFi decompiler from Assembly-CSharp.dll class: GooglePlayGames.Native.PInvoke.NearbyConnectionsManager
using AOT;
using GooglePlayGames.Native.Cwrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace GooglePlayGames.Native.PInvoke
{
	internal class NearbyConnectionsManager : BaseReferenceHolder
	{
		private static readonly string sServiceId = ReadServiceId();

		[CompilerGenerated]
		private static Func<IntPtr, NativeStartAdvertisingResult> _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Func<IntPtr, NativeConnectionRequest> _003C_003Ef__mg_0024cache1;

		[CompilerGenerated]
		private static NearbyConnectionTypes.StartAdvertisingCallback _003C_003Ef__mg_0024cache2;

		[CompilerGenerated]
		private static NearbyConnectionTypes.ConnectionRequestCallback _003C_003Ef__mg_0024cache3;

		[CompilerGenerated]
		private static Func<IntPtr, NativeConnectionResponse> _003C_003Ef__mg_0024cache4;

		[CompilerGenerated]
		private static NearbyConnectionTypes.ConnectionResponseCallback _003C_003Ef__mg_0024cache5;

		public string AppBundleId
		{
			get
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
					return @static.Call<string>("getPackageName", Array.Empty<object>());
				}
			}
		}

		public static string ServiceId => sServiceId;

		internal NearbyConnectionsManager(IntPtr selfPointer)
			: base(selfPointer)
		{
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			NearbyConnections.NearbyConnections_Dispose(selfPointer);
		}

		internal void SendUnreliable(string remoteEndpointId, byte[] payload)
		{
			NearbyConnections.NearbyConnections_SendUnreliableMessage(SelfPtr(), remoteEndpointId, payload, new UIntPtr((ulong)payload.Length));
		}

		internal void SendReliable(string remoteEndpointId, byte[] payload)
		{
			NearbyConnections.NearbyConnections_SendReliableMessage(SelfPtr(), remoteEndpointId, payload, new UIntPtr((ulong)payload.Length));
		}

		internal void StartAdvertising(string name, List<NativeAppIdentifier> appIds, long advertisingDuration, Action<long, NativeStartAdvertisingResult> advertisingCallback, Action<long, NativeConnectionRequest> connectionRequestCallback)
		{
			NearbyConnections.NearbyConnections_StartAdvertising(SelfPtr(), name, (from id in appIds
				select id.AsPointer()).ToArray(), new UIntPtr((ulong)appIds.Count), advertisingDuration, InternalStartAdvertisingCallback, Callbacks.ToIntPtr(advertisingCallback, NativeStartAdvertisingResult.FromPointer), InternalConnectionRequestCallback, Callbacks.ToIntPtr(connectionRequestCallback, NativeConnectionRequest.FromPointer));
		}

		[MonoPInvokeCallback(typeof(NearbyConnectionTypes.StartAdvertisingCallback))]
		private static void InternalStartAdvertisingCallback(long id, IntPtr result, IntPtr userData)
		{
			Callbacks.PerformInternalCallback("NearbyConnectionsManager#InternalStartAdvertisingCallback", Callbacks.Type.Permanent, id, result, userData);
		}

		[MonoPInvokeCallback(typeof(NearbyConnectionTypes.ConnectionRequestCallback))]
		private static void InternalConnectionRequestCallback(long id, IntPtr result, IntPtr userData)
		{
			Callbacks.PerformInternalCallback("NearbyConnectionsManager#InternalConnectionRequestCallback", Callbacks.Type.Permanent, id, result, userData);
		}

		internal void StopAdvertising()
		{
			NearbyConnections.NearbyConnections_StopAdvertising(SelfPtr());
		}

		internal void SendConnectionRequest(string name, string remoteEndpointId, byte[] payload, Action<long, NativeConnectionResponse> callback, NativeMessageListenerHelper listener)
		{
			NearbyConnections.NearbyConnections_SendConnectionRequest(SelfPtr(), name, remoteEndpointId, payload, new UIntPtr((ulong)payload.Length), InternalConnectResponseCallback, Callbacks.ToIntPtr(callback, NativeConnectionResponse.FromPointer), listener.AsPointer());
		}

		[MonoPInvokeCallback(typeof(NearbyConnectionTypes.ConnectionResponseCallback))]
		private static void InternalConnectResponseCallback(long localClientId, IntPtr response, IntPtr userData)
		{
			Callbacks.PerformInternalCallback("NearbyConnectionManager#InternalConnectResponseCallback", Callbacks.Type.Temporary, localClientId, response, userData);
		}

		internal void AcceptConnectionRequest(string remoteEndpointId, byte[] payload, NativeMessageListenerHelper listener)
		{
			NearbyConnections.NearbyConnections_AcceptConnectionRequest(SelfPtr(), remoteEndpointId, payload, new UIntPtr((ulong)payload.Length), listener.AsPointer());
		}

		internal void DisconnectFromEndpoint(string remoteEndpointId)
		{
			NearbyConnections.NearbyConnections_Disconnect(SelfPtr(), remoteEndpointId);
		}

		internal void StopAllConnections()
		{
			NearbyConnections.NearbyConnections_Stop(SelfPtr());
		}

		internal void StartDiscovery(string serviceId, long duration, NativeEndpointDiscoveryListenerHelper listener)
		{
			NearbyConnections.NearbyConnections_StartDiscovery(SelfPtr(), serviceId, duration, listener.AsPointer());
		}

		internal void StopDiscovery(string serviceId)
		{
			NearbyConnections.NearbyConnections_StopDiscovery(SelfPtr(), serviceId);
		}

		internal void RejectConnectionRequest(string remoteEndpointId)
		{
			NearbyConnections.NearbyConnections_RejectConnectionRequest(SelfPtr(), remoteEndpointId);
		}

		internal void Shutdown()
		{
			NearbyConnections.NearbyConnections_Stop(SelfPtr());
		}

		internal static string ReadServiceId()
		{
			UnityEngine.Debug.Log("Initializing ServiceId property!!!!");
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
				{
					string text = androidJavaObject.Call<string>("getPackageName", Array.Empty<object>());
					AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getPackageManager", Array.Empty<object>());
					AndroidJavaObject androidJavaObject3 = androidJavaObject2.Call<AndroidJavaObject>("getApplicationInfo", new object[2]
					{
						text,
						128
					});
					AndroidJavaObject androidJavaObject4 = androidJavaObject3.Get<AndroidJavaObject>("metaData");
					string text2 = androidJavaObject4.Call<string>("getString", new object[1]
					{
						"com.google.android.gms.nearby.connection.SERVICE_ID"
					});
					UnityEngine.Debug.Log("SystemId from Manifest: " + text2);
					return text2;
				}
			}
		}
	}
}
