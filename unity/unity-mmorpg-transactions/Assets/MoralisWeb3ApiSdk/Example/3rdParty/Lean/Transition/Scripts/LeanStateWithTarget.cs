using UnityEngine;

namespace Lean.Transition
{
	/// <summary>This class stores additional base data for transitions that modify a target UnityEngine.Object (most do).</summary>
	public abstract class LeanStateWithTarget<T> : LeanState
		where T : Object
	{
		/// <summary>This is the target of the transition. For most transition methods this will be the component that will be modified.</summary>
		public T Target;

		public override Object GetTarget()
		{
			return Target;
		}

		public override int CanFill
		{
			get
			{
				return Target != null ? 1 : 0;
			}
		}

		public override void Fill()
		{
			if (Target != null)
			{
				FillWithTarget();
			}
		}

		public virtual void FillWithTarget()
		{
		}

		public override void Begin()
		{
			if (Target != null)
			{
				BeginWithTarget();
			}
		}

		public virtual void BeginWithTarget()
		{
		}

		public override void Update(float progress)
		{
			if (Target != null)
			{
				UpdateWithTarget(progress);
			}
		}

		public virtual void UpdateWithTarget(float progress)
		{
		}
	}
}