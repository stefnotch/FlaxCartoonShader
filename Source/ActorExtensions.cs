using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace CartoonShader.Source
{
	public static class ActorExtensions
	{
		public static T TemporaryChild<T>(this Actor actor) where T : Actor
		{
			T child = actor.AddChild<T>();
			child.HideFlags = HideFlags.DontSave;

			return child;
		}

		public static StaticModel TemporaryStaticModel(this Actor actor)
		{
			// TODO: Do I have to dispose of the temp model?
			throw new NotImplementedException();
			Model tempModel = Content.CreateVirtualAsset<Model>();
			tempModel.SetupLODs(1);

			StaticModel modelActor = FlaxEngine.Object.New<StaticModel>();
			modelActor.HideFlags = HideFlags.DontSave;
			modelActor.Model = tempModel;

			return modelActor;
		}

		public static Mesh GetMesh(this StaticModel model, int meshIndex, int lodIndex = 0)
		{
			return model.Model.LODs[lodIndex].Meshes[meshIndex];
		}

		/// <summary>
		/// A depth first traversal of the actor tree
		/// </summary>
		/// <param name="start">Root actor</param>
		/// <param name="keepGoing">Should this actor and its children get returned as well?</param>
		public static IEnumerable<Actor> DepthFirst(this Actor start, Func<Actor, bool> keepGoing = null)
		{
			Stack<Actor> s = new Stack<Actor>();
			s.Push(start);
			while (s.Count > 0)
			{
				Actor actor = s.Pop();
				if (keepGoing == null || keepGoing(actor))
				{
					yield return actor;

					for (int i = 0; i < actor.ChildrenCount; i++)
					{
						s.Push(actor.GetChild(i));
					}
				}
			}
		}
	}
}