using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace photoel2
{
    class Model
    {
        const double current_amplification = 1000000000;

        public Model()
        {
            Current = 0;
            recalc_all();
        }

        public delegate void CurrentChangedDelegate(double current);
        public event CurrentChangedDelegate CurrentChanged;

        public double Voltage
        {
            get { return _voltage; }
            set {
                _voltage = value;
                recalc_current();
            }
        }

        public double Distance
        {
            get { return _distance; }
            set {
                _distance = value;
                recalc_all();
            }
        }

        public Constants.Filter Filter
        {
            get { return _filter; }
            set { _filter = value; recalc_all(); }
        }
        public Constants.Metal Metal
        {
            get { return _metal; }
            set { _metal = value; recalc_all(); }
        }

        public double Current
        {
            get; private set;
        }

        private void recalc_all()
        {
            recalc_total_in_power();
            recalc_stop_voltage();
            recalc_max_current();
            recalc_coefficients();
            recalc_current();
        }

        private void recalc_current()
        {
            double max_lambda = HC / _metal.work_function;

            double current = 0;

            if (lambda <= max_lambda && Voltage >= -_stop_voltage)
            {
                double qfactor = Constants.QFactor;

                if (_voltage < (1 - qfactor) * _stop_voltage)
                    current = _slope * _voltage + _intercept;
                else if (_voltage < (1 + qfactor) * _stop_voltage)
                    current = (_quadratic * _voltage + _linear) * _voltage + _constant;
                else
                    current = _max_current;
            }

            current = current * current_amplification;

            if (current != Current)
            {
                Current = current;
                if (CurrentChanged != null)
                    CurrentChanged(current);
            }
        }

        private void recalc_total_in_power()
        {
            _total_in_power = Constants.SourcePower * Constants.Efficiency * Constants.ElectrodeArea / (4.0 * Math.PI * _distance * _distance);
        }

        private void recalc_max_current()
        {
            double power_lambda = _total_in_power * _filter.intensity_factor;
            double ph_energy = HC / lambda;
            _max_current = power_lambda / ph_energy;
        }

        private void recalc_stop_voltage()
        {
            _stop_voltage = Math.Max(0, HC / lambda - _metal.work_function);
        }

        private void recalc_coefficients()
        {
            _intercept = _max_current / 2;
            _slope = _intercept / _stop_voltage;

            double qfactor = Constants.QFactor;
            _quadratic = - _max_current / (8.0 * qfactor * _stop_voltage * _stop_voltage);
            _linear = _max_current * (1.0 + qfactor) / (4.0 * qfactor * _stop_voltage);
            _constant = _max_current * (1.0 - (1.0 + qfactor) * (1.0 + qfactor) / (8.0 * qfactor));
        }

        private double lambda { get { return _filter.wave_length* 10;} }

        const double H = 4.14E-15;          //Plank const
        const double C = 3.0E8;             //Light velocity
        const double HC = 12420;            //Energy const
        const double QE = 1.6E-19;			//Charge of an electron

        private double _voltage;
        private double _distance;
        private Constants.Filter _filter = Constants.DefaultFilter;
        private Constants.Metal _metal = Constants.DefaultMetal;

        // calculated
        private double _total_in_power;
        private double _stop_voltage;
        private double _max_current;
        private double _intercept, _slope, _quadratic, _linear, _constant;

    }
}

