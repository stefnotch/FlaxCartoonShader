using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace CartoonShader.Source
{
	public abstract class ScriptWithDisposer : Script
	{
		protected readonly Disposer _disposer = new Disposer();

		protected virtual void OnDisable()
		{
		}

		protected virtual void OnDestroy()
		{
		}
	}
}