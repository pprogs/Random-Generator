using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWPF
{

    public interface IRandomValue
    {
        void NextStep(int substeps, float distr = 0.2f);
        float NextSubStep(float distr = 0.1f);
    }

    public class SimpleRandomValue : IRandomValue
    {
        Random _rnd = new Random(DateTime.Now.Millisecond);

        float _from;
        float _to;

        public SimpleRandomValue(float from, float to)
        {
            _from = from;
            _to = to;
        }

        public void NextStep(int substeps, float distr = 0.2F)
        {
        }

        public float NextSubStep(float distr = 0.1F)
        {
            return _from + ((float)_rnd.NextDouble() * (_to - _from));
        }
    }

    public class RandomValue : IRandomValue
    {
        public float _amountFrom;
        public float _amountTo;
        public float _amountStep;
        public float _baseAmount;

        public float _toDistrib;

        public int _steps;
        public int _subSteps;

        Random _rnd = new Random(DateTime.Now.Millisecond);
        Func<float> _rndFunc;

        public RandomValue(float from, float to, int steps)
        {
            _amountFrom = from;
            _amountTo = to;
            _steps = steps;
            _amountStep = (_amountTo - _amountFrom) / (float)_steps;
            _baseAmount = _amountFrom;


            _rndFunc = new Func<float>(() => { return _amountFrom + (float)_rnd.NextDouble() * (_amountTo - _amountFrom); });
        }

        public void NextStep(int substeps, float distr = 0.2f )
        {
            _baseAmount += _amountStep;
            _toDistrib = _baseAmount + (float)((_rnd.NextDouble() - 0.5) * (_amountStep * distr));
            _subSteps = substeps;
        }

        public float NextSubStep(float distr = 0.1f )
        {
            return (_toDistrib / (float)_subSteps) + (float)((_rnd.NextDouble() - 0.5) * (_toDistrib / (float)_subSteps * distr));
        }

    }


    public class Tests
    {


        public static void DoSome()
        {
        
        }
    }

}
