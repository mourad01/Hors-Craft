// DecompilerFi decompiler from Assembly-CSharp.dll class: ExceptionExtensions
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using UnityEngine;

public static class ExceptionExtensions
{
	private const string tab = "   ";

	private const string nl = "\r\n";

	public static string ToLogString(this Exception excep, StackFrame stackFrame, object AdditionalInfo)
	{
		StringBuilder stringBuilder = new StringBuilder();
		MethodBase method = stackFrame.GetMethod();
		stringBuilder.AppendFormat("Exception Location:{0}", "\r\n");
		stringBuilder.AppendFormat("{0}Namespace: {1}{2}", "   ", method.ReflectedType.Namespace, "\r\n");
		stringBuilder.AppendFormat("{0}Class Name: {1}{2}", "   ", method.ReflectedType.Name, "\r\n");
		stringBuilder.AppendFormat("{0}Method Name: {1}{2}", "   ", method.Name, "\r\n");
		stringBuilder.AppendFormat("Exception Information:{0}", "\r\n");
		stringBuilder.AppendFormat("{2}Message: {0}{1}", excep.Message, "\r\n", "   ");
		stringBuilder.AppendFormat("{2}Source: {0}{1}{1}", excep.Source, "\r\n", "   ");
		stringBuilder.AppendFormat("Stack Trace: {1} {0}{1}{1}", excep.StackTrace, "\r\n");
		if (excep.InnerException != null)
		{
			stringBuilder.AppendFormat("{1}{0}Inner Exception Info:{0}", "\r\n", "   ");
			stringBuilder.AppendFormat("{2}{2}Message: {0}{1}", excep.InnerException.Message, "\r\n", "   ");
			stringBuilder.AppendFormat("{2}{2}Source: {0}{1}", excep.InnerException.Source, "\r\n", "   ");
			stringBuilder.AppendFormat("{2}{2}Stack Trace: {0}{1}", excep.InnerException.StackTrace, "\r\n", "   ");
		}
		if (AdditionalInfo != null)
		{
			stringBuilder.AppendFormat("Additional Object Info{0}", "\r\n");
			stringBuilder.AppendFormat("{0}{1}{2}", "   ", ObjectDumper.GetObjectValue(AdditionalInfo), "\r\n");
		}
		stringBuilder.AppendFormat("Device Info:{0}", "\r\n");
		stringBuilder.AppendFormat("{2}Device OS: {0} {1}", SystemInfo.operatingSystem, "\r\n", "   ");
		stringBuilder.AppendFormat("{2}Device Model: {0} {1}", SystemInfo.deviceModel, "\r\n", "   ");
		stringBuilder.AppendFormat("{2}Device Type: {0} {1}", SystemInfo.deviceType, "\r\n", "   ");
		stringBuilder.AppendFormat("{2}Device Name: {0} {1}", SystemInfo.deviceName, "\r\n", "   ");
		stringBuilder.AppendFormat("{2}Device ID: {0} {1}", SystemInfo.deviceUniqueIdentifier, "\r\n", "   ");
		stringBuilder.AppendFormat("{2}Device Memory: {0} {1}", SystemInfo.systemMemorySize, "\r\n", "   ");
		return stringBuilder.ToString();
	}
}
