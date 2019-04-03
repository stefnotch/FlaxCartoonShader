using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace CartoonShader.Source.RenderingPipeline.RenderingOutput
{
	public class RenderOutputToMaterial : Script
	{
		[Serialize]
		private int _affectedEntry = -1;

		// TODO: Figure out how to serialize a RenderOutput
		[NoSerialize]
		public RenderOutput RenderOutput;

		[Serialize]
		public bool SetAllEntries = true;

		[NoSerialize]
		public int AffectedEntry
		{
			get => SetAllEntries ? -1 : _affectedEntry;
			set
			{
				_affectedEntry = value;
				SetAllEntries = value == -1 ? true : false;
			}
		}

		public MaterialBase Material;

		public void Start()
		{
			if (RenderOutput == null) return;

			Scripting.InvokeOnUpdate(() =>
			{
				if (this.Actor is StaticModel staticModel)
				{
					Material.WaitForLoaded();
					Debug.Log(RenderOutput.RenderTarget);
					Material.GetParam("Image").Value = RenderOutput.RenderTarget; // TODO: Image or MaterialRenderInputs.DefaultInputName
					staticModel.Entries[0].Material = Material;
				}

				if (this.Actor is AnimatedModel animatedModel)
				{
					Material.WaitForLoaded();
					Material.GetParam("Image").Value = RenderOutput.RenderTarget;
					animatedModel.Entries[0].Material = Material;
				}
			});
		}
	}
}