using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace GoldenCoastPlusRevived.Properties
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class GCPResources
	{
		internal GCPResources()
		{
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				bool flag = GCPResources.resourceMan == null;
				if (flag)
				{
					ResourceManager resourceManager = new ResourceManager("GoldenCoastPlusRevived.GCPResources", typeof(GCPResources).Assembly);
					GCPResources.resourceMan = resourceManager;
				}
				return GCPResources.resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return GCPResources.resourceCulture;
			}
			set
			{
				GCPResources.resourceCulture = value;
			}
		}

		internal static byte[] Aurelionite_s_Blessing
		{
			get
			{
				object @object = GCPResources.ResourceManager.GetObject("Aurelionite_s_Blessing", GCPResources.resourceCulture);
				return (byte[])@object;
			}
		}

		internal static byte[] Gold_Elite_Icon
		{
			get
			{
				object @object = GCPResources.ResourceManager.GetObject("Gold_Elite_Icon", GCPResources.resourceCulture);
				return (byte[])@object;
			}
		}

		internal static byte[] Golden_Knurl
		{
			get
			{
				object @object = GCPResources.ResourceManager.GetObject("Golden_Knurl", GCPResources.resourceCulture);
				return (byte[])@object;
			}
		}

		internal static byte[] Guardian_s_Eye
		{
			get
			{
				object @object = GCPResources.ResourceManager.GetObject("Guardian_s_Eye", GCPResources.resourceCulture);
				return (byte[])@object;
			}
		}

		internal static byte[] Titanic_Greatsword
		{
			get
			{
				object @object = GCPResources.ResourceManager.GetObject("Titanic_Greatsword", GCPResources.resourceCulture);
				return (byte[])@object;
			}
		}

		private static ResourceManager resourceMan;
		private static CultureInfo resourceCulture;
	}
}
