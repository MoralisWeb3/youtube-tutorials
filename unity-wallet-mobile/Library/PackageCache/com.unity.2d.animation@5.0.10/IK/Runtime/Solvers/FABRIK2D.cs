using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.U2D.IK
{
    /// <summary>
    /// Structure to store FABRIK Chain data.
    /// </summary>
    [MovedFrom("UnityEngine.Experimental.U2D.IK")]
    public struct FABRIKChain2D
    {
        /// <summary>
        /// Returns the first element's position.
        /// </summary>
        public Vector2 first
        {
            get { return positions[0]; }
        }

        /// <summary>
        /// Returns the last element's position.
        /// </summary>
        public Vector2 last
        {
            get { return positions[positions.Length - 1]; }
        }

        /// <summary>
        /// Position of the origin.
        /// </summary>
        public Vector2 origin;
        
        /// <summary>
        /// Position of the target which is used to indicate the desired position for the Effector.
        /// </summary>
        public Vector2 target;
        
        /// <summary>
        /// Target position's tolerance (squared).
        /// </summary>
        public float sqrTolerance;
        
        /// <summary>
        /// Array of chain positions.
        /// </summary>
        public Vector2[] positions;
        
        /// <summary>
        /// Array of chain lengths.
        /// </summary>
        public float[] lengths;
        
        /// <summary>
        /// Sub-Chain indices.
        /// </summary>
        public int[] subChainIndices;
        
        /// <summary>
        /// Array of world positions.
        /// </summary>
        public Vector3[] worldPositions;
    }

    /// <summary>
    /// Utility for 2D Forward And Backward Reaching Inverse Kinematics (FABRIK) IK Solver.
    /// </summary>
    public static class FABRIK2D
    {
        /// <summary>
        /// Solve IK based on FABRIK
        /// </summary>
        /// <param name="targetPosition">Target position.</param>
        /// <param name="solverLimit">Solver iteration count.</param>
        /// <param name="tolerance">Target position's tolerance.</param>
        /// <param name="lengths">Length of the chains.</param>
        /// <param name="positions">Chain positions.</param>
        /// <returns>Returns true if solver successfully completes within iteration limit. False otherwise.</returns>
        public static bool Solve(Vector2 targetPosition, int solverLimit, float tolerance, float[] lengths, ref Vector2[] positions)
        {
            int last = positions.Length - 1;
            int iterations = 0;
            float sqrTolerance = tolerance * tolerance;
            float sqrDistanceToTarget = (targetPosition - positions[last]).sqrMagnitude;
            Vector2 originPosition = positions[0];
            while (sqrDistanceToTarget > sqrTolerance)
            {
                Forward(targetPosition, lengths, ref positions);
                Backward(originPosition, lengths, ref positions);
                sqrDistanceToTarget = (targetPosition - positions[last]).sqrMagnitude;
                if (++iterations >= solverLimit)
                    break;
            }

            // Return whether positions have changed
            return iterations != 0;
        }
        
        /// <summary>
        /// Solve IK based on FABRIK.
        /// </summary>
        /// <param name="solverLimit">Solver iteration count.</param>
        /// <param name="chains">FABRIK chains.</param>
        /// <returns>True if solver successfully completes within iteration limit. False otherwise.</returns>
        public static bool SolveChain(int solverLimit, ref FABRIKChain2D[] chains)
        {
            // Do a quick validation of the end points that it has not been solved
            if (ValidateChain(chains))
                return false;

            // Validation failed, solve chain
            for (int iterations = 0; iterations < solverLimit; ++iterations)
            {
                SolveForwardsChain(0, ref chains);
                // Break if solution is solved
                if (!SolveBackwardsChain(0, ref chains))
                    break;
            }
            return true;
        }

        static bool ValidateChain(FABRIKChain2D[] chains)
        {
            foreach (var chain in chains)
            {
                if (chain.subChainIndices.Length == 0 && (chain.target - chain.last).sqrMagnitude > chain.sqrTolerance)
                    return false;
            }
            return true;
        }

        static void SolveForwardsChain(int idx, ref FABRIKChain2D[] chains)
        {
            var target = chains[idx].target;
            if (chains[idx].subChainIndices.Length > 0)
            {
                target = Vector2.zero;
                for (int i = 0; i < chains[idx].subChainIndices.Length; ++i)
                {
                    var childIdx = chains[idx].subChainIndices[i];
                    SolveForwardsChain(childIdx, ref chains);
                    target += chains[childIdx].first;
                }
                target = target / chains[idx].subChainIndices.Length;
            }
            Forward(target, chains[idx].lengths, ref chains[idx].positions);
        }

        static bool SolveBackwardsChain(int idx, ref FABRIKChain2D[] chains)
        {
            bool notSolved = false;
            Backward(chains[idx].origin, chains[idx].lengths, ref chains[idx].positions);
            for (int i = 0; i < chains[idx].subChainIndices.Length; ++i)
            {
                var childIdx = chains[idx].subChainIndices[i];
                chains[childIdx].origin = chains[idx].last;
                notSolved |= SolveBackwardsChain(childIdx, ref chains);
            }
            // Check if end point has reached the target
            if (chains[idx].subChainIndices.Length == 0)
            {
                notSolved |= (chains[idx].target - chains[idx].last).sqrMagnitude > chains[idx].sqrTolerance;
            }
            return notSolved;
        }

        static void Forward(Vector2 targetPosition, float[] lengths, ref Vector2[] positions)
        {
            var last = positions.Length - 1;
            positions[last] = targetPosition;
            for (int i = last - 1; i >= 0; --i)
            {
                var r = positions[i + 1] - positions[i];
                var l = lengths[i] / r.magnitude;
                var position = (1f - l) * positions[i + 1] + l * positions[i];
                positions[i] = position;
            }
        }

        static void Backward(Vector2 originPosition, float[] lengths, ref Vector2[] positions)
        {
            positions[0] = originPosition;
            var last = positions.Length - 1;
            for (int i = 0; i < last; ++i)
            {
                var r = positions[i + 1] - positions[i];
                var l = lengths[i] / r.magnitude;
                var position = (1f - l) * positions[i] + l * positions[i + 1];
                positions[i + 1] = position;
            }
        }

        // For constraints
        static Vector2 ValidateJoint(Vector2 endPosition, Vector2 startPosition, Vector2 right, float min, float max)
        {
            var localDifference = endPosition - startPosition;
            var angle = Vector2.SignedAngle(right, localDifference);
            var validatedPosition = endPosition;
            if (angle < min)
            {
                var minRotation = Quaternion.Euler(0f, 0f, min);
                validatedPosition = startPosition + (Vector2)(minRotation * right * localDifference.magnitude);
            }
            else if (angle > max)
            {
                var maxRotation = Quaternion.Euler(0f, 0f, max);
                validatedPosition = startPosition + (Vector2)(maxRotation * right * localDifference.magnitude);
            }
            return validatedPosition;
        }
    }
}
