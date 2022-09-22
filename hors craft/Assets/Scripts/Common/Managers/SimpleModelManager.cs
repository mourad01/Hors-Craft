// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.SimpleModelManager
using Common.Model;
using System.Collections.Generic;

namespace Common.Managers
{
	public abstract class SimpleModelManager : AbstractModelManager
	{
		private class ExplicitImplementationModelModule : ModelModule
		{
			public delegate void FillModel(ModelDescription descriptions);

			public delegate void ModelDownloaded();

			public FillModel fillModelDelegate;

			public ModelDownloaded modelDownloadedDelegate;

			public override void FillModelDescription(ModelDescription descriptions)
			{
				fillModelDelegate(descriptions);
			}

			public override void OnModelDownloaded()
			{
				modelDownloadedDelegate();
			}
		}

		protected override List<ModelModule> ConstructModelModules()
		{
			List<ModelModule> list = new List<ModelModule>();
			ExplicitImplementationModelModule explicitImplementationModelModule = new ExplicitImplementationModelModule();
			explicitImplementationModelModule.fillModelDelegate = FillModelDescription;
			explicitImplementationModelModule.modelDownloadedDelegate = OnModelDownloaded;
			list.Add(explicitImplementationModelModule);
			return list;
		}

		protected abstract void FillModelDescription(ModelDescription descriptions);

		protected abstract void OnModelDownloaded();
	}
}
