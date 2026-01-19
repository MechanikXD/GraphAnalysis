using System;
using System.Threading;
using Analysis.Metrics;
using UnityEngine;

namespace Analysis.Local
{
    public class EigenvectorCentrality : LocalMetric
    {
        public int Iterations { get; set; } = 100;
        public float Tolerance { get; set; } = 1e-6f;
        
        public override float[] Process(GraphCache cache)
        {
            var x = new float[cache.Matrix.Length];
            for (var i = 0; i < cache.Matrix.Length; i++) x[i] = 1f;
            var xNew = new float[cache.Matrix.Length];

            for (var iter = 0; iter < Iterations; iter++)
            {
                for (var i = 0; i < cache.Matrix.Length; i++)
                {
                    var neighbors = cache.OutNeighbors[i];
                    var wts = cache.OutWeights[i];
                    var s = 0f;
                    for (var k = 0; k < neighbors.Length; k++) s += wts[k] * x[neighbors[k]];
                    xNew[i] = s;
                }

                var norm = 0f;
                for (var i = 0; i < cache.Matrix.Length; i++) norm += xNew[i] * xNew[i];
                norm = Mathf.Sqrt(norm);
                if (norm == 0f) break;
                for (var i = 0; i < cache.Matrix.Length; i++) xNew[i] /= norm;

                var diff = 0f;
                for (var i = 0; i < cache.Matrix.Length; i++) diff += Mathf.Abs(xNew[i] - x[i]);
                Array.Copy(xNew, x, cache.Matrix.Length);
                if (diff < Tolerance) break;
            }

            var outArr = new float[cache.Matrix.Length];
            for (var i = 0; i < cache.Matrix.Length; i++) outArr[i] = x[i];
            return outArr;
        }

        protected override float[] ProcessParallel(GraphCache cache, CancellationToken token)
        {
            var x = new float[cache.Matrix.Length];
            for (var i = 0; i < cache.Matrix.Length; i++) x[i] = 1f;
            var xNew = new float[cache.Matrix.Length];

            for (var iter = 0; iter < Iterations; iter++)
            {
                Array.Clear(xNew, 0, cache.Matrix.Length);
                for (var i = 0; i < cache.Matrix.Length; i++)
                {
                    var neighbors = cache.OutNeighbors[i];
                    var wts = cache.OutWeights[i];
                    var s = 0f;
                    for (var k = 0; k < neighbors.Length; k++) s += wts[k] * x[neighbors[k]];
                    xNew[i] = s;
                }

                var norm = 0f;
                for (var i = 0; i < cache.Matrix.Length; i++) norm += xNew[i] * xNew[i];
                norm = Mathf.Sqrt(norm);
                if (norm == 0f) break;
                for (var i = 0; i < cache.Matrix.Length; i++) xNew[i] /= norm;

                var diff = 0f;
                for (var i = 0; i < cache.Matrix.Length; i++) diff += Mathf.Abs(xNew[i] - x[i]);
                Array.Copy(xNew, x, cache.Matrix.Length);
                if (diff < Tolerance) break;
            }

            var outArr = new float[cache.Matrix.Length];
            for (var i = 0; i < cache.Matrix.Length; i++) outArr[i] = x[i];
            return outArr;
        }
    }
}