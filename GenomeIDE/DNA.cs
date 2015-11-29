using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenomeIDE
{
    public class DNA: IListSource, IEnumerable<byte>
    {
        public DNA(string filename = null)
        {
            if (filename != null)
            {
                var lines = File.ReadAllLines(filename);
                _data = lines.Select(l => Convert.ToByte(l.Substring(0, 2), 16)).ToList();
            }
            else
            {
                _data = new List<byte>();
            }
        }

        private readonly List<byte> _data;

        public int Size
        {
            get { return _data.Count; }
        }

        public byte this[int index]
        {
            get
            {
                return Get(index);
            }
            set
            {
                Set(index, value);
            }
        }

        public byte Get(int index)
        {
            return _data[index];
        }

        public void Set(int index, byte value)
        {
            _data[index] = value;
        }

        public void Add(byte value)
        {
            _data.Add(value);
        }

        public void Add(IEnumerable<byte> values)
        {
            _data.AddRange(values);
        }

        public bool CanGetFragment(int start, int length)
        {
            return start + length <= Size;
        }

        public List<byte> GetFragment(int start, int length)
        {
            var fragment = new List<byte>();

            for (int i = start; i < start + length; i++)
                fragment.Add(_data[i]);

            return fragment;
        }

        public void Save(string filename)
        {
            File.WriteAllLines(filename, _data.Select(b => b.ToString("X2")));
        }

        public IList GetList()
        {
            return _data;
        }

        public bool ContainsListCollection { get { return true; } }
        public IEnumerator<byte> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _data).GetEnumerator();
        }

        public void RemoveAt(int index)
        {
            _data.RemoveAt(index);
        }

        public void Insert(int index, byte value)
        {
            _data.Insert(index, value);
        }
    }
}
