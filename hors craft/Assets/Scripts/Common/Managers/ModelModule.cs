// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.ModelModule
using Common.Model;

namespace Common.Managers
{
	public abstract class ModelModule
	{
		protected Settings settings
		{
			get;
			private set;
		}

		protected ModelModuleContext context
		{
			get;
			private set;
		}

		protected ModelModuleContext contextIAP => context;

		public virtual void AssignSettings(Settings settings)
		{
			this.settings = settings;
		}

		public void AssignIAPContext(ModelModuleContext contextIAP)
		{
			context = contextIAP;
		}

		public abstract void FillModelDescription(ModelDescription descriptions);

		public abstract void OnModelDownloaded();
	}
}
