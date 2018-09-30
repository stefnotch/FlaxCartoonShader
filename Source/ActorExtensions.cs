using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace CartoonShader
{
	public static class ActorExtensions
	{
		public static T TemporaryChild<T>(this Actor actor) where T : Actor
		{
			T child = actor.AddChild<T>();
			child.HideFlags = HideFlags.DontSave;

			return child;
		}
	}
}