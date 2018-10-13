using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace CartoonShader.Source.MeshGenerators
{
	public interface IMeshGenerator
	{
		void Generate(Mesh mesh);
	}
}