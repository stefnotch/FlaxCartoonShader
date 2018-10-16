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