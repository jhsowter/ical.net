using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Ical.Net.Evaluation;
using Ical.Net.Interfaces.DataTypes;
using Ical.Net.Interfaces.General;
using Ical.Net.Serialization.iCalendar.Serializers.DataTypes;

namespace Ical.Net.DataTypes
{
    /// <summary>
    /// An iCalendar list of recurring dates (or date exclusions)
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class PeriodList : 
        EncodableDataType,
        IPeriodList
    {
        #region Private Fields

        private IList<IPeriod> _mPeriods = new List<IPeriod>();
        private string _mTzid;

        #endregion

        #region Public Properties

        public string TzId
        {
            get { return _mTzid; }
            set { _mTzid = value; }
        }

        protected IList<IPeriod> Periods
        {
            get { return _mPeriods; }
            set { _mPeriods = value; }
        }

        #endregion

        #region Constructors

        public PeriodList()
        {
            Initialize();
        }
        public PeriodList(string value) : this()
        {
            var serializer = new PeriodListSerializer();
            CopyFrom(serializer.Deserialize(new StringReader(value)) as ICopyable);
        }

        void Initialize()
        {
            SetService(new PeriodListEvaluator(this));
        }

        #endregion

        #region Overrides

        protected override void OnDeserializing(StreamingContext context)
        {
            base.OnDeserializing(context);

            Initialize();
        }

        public override bool Equals(object obj)
        {
            if (obj is IPeriodList)
            {
                var r = (IPeriodList)obj;

                var p1Enum = GetEnumerator();
                var p2Enum = r.GetEnumerator();

                while (p1Enum.MoveNext())
                {
                    if (!p2Enum.MoveNext())
                        return false;

                    if (!Equals(p1Enum.Current, p2Enum.Current))
                        return false;
                }

                if (p2Enum.MoveNext())
                    return false;

                return true;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            foreach (var p in this)
                hashCode ^= p.GetHashCode();
            return hashCode;
        }
 
        public override void CopyFrom(ICopyable obj)
        {
            base.CopyFrom(obj);
            if (obj is IPeriodList)
            {
                var rdt = (IPeriodList)obj;
                foreach (var p in rdt)
                    Add(p.Copy<IPeriod>());
            }
        }

        public override string ToString()
        {
            var serializer = new PeriodListSerializer();
            return serializer.SerializeToString(this);
        }

        #endregion

        #region Public Methods

        public List<Period> Evaluate(CalDateTime startDate, CalDateTime fromDate, CalDateTime endDate)
        {
            var periods = new List<Period>();

            if (startDate > fromDate)
                fromDate = startDate;

            if (endDate < fromDate ||
                fromDate > endDate)
                return periods;

            foreach (Period p in Periods)
                if (!periods.Contains(p))
                    periods.Add(p);

            return periods;
        }

        #endregion

        #region IPeriodList Members

        public virtual void Add(IDateTime dt)
        {
            Periods.Add(new Period(dt));
        }

        public virtual void Remove(IDateTime dt)
        {
            Periods.Remove(new Period(dt));
        }

        public IPeriod this[int index]
        {
            get
            {
                return _mPeriods[index];
            }
            set
            {
                _mPeriods[index] = value;
            }
        }

        #endregion

        #region IList<IPeriod> Members

        public virtual void Add(IPeriod item)
        {
            _mPeriods.Add(item);
        }

        public virtual void Clear()
        {
            _mPeriods.Clear();
        }

        public bool Contains(IPeriod item)
        {
            return _mPeriods.Contains(item);
        }

        public void CopyTo(IPeriod[] array, int arrayIndex)
        {
            _mPeriods.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _mPeriods.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(IPeriod item)
        {
            return _mPeriods.Remove(item);
        }

        public int IndexOf(IPeriod item)
        {
            return _mPeriods.IndexOf(item);
        }

        public void Insert(int index, IPeriod item)
        {
            _mPeriods.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _mPeriods.RemoveAt(index);
        }

        #endregion

        #region IEnumerable<IPeriod> Members

        public IEnumerator<IPeriod> GetEnumerator()
        {
            return _mPeriods.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _mPeriods.GetEnumerator();
        }

        #endregion
    }
}