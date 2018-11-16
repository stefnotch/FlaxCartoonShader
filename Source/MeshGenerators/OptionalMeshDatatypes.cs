using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonShader.Source.MeshGenerators
{
	[Flags]
	public enum OptionalMeshDataTypes
	{
		None = 0,
		Normals = 1,
		Tangents = 2,
		Uvs = 4,
		Colors = 8,
		All = Normals | Tangents | Uvs | Colors
	}
}