using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.CoreFunction.IoC.Controllers.ScreenCapture.ScreenShotLinkList
{
    public class LinkedLists<T>
    {
        private Node<T> head;
        private int count;

        public LinkedLists()
        {
            this.head = null;
            this.count = 0;
        }

        public bool Empty
        {
            get { return this.count == 0; }
        }

        public int Count
        {
            get { return this.count; }
        }

        public object this[int index]
        {
            get { return this.Get(index); }
        }

        public object Add(int index, object o)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("Index: " + index);
            if (index > count)
                index = count;
            Node<T> current = this.head;
            if (this.Empty || index == 0)
            {
                this.head = new Node<T>(o, this.head);
            }
            else
            {
                for (int i = 0; i < index - 1; i++)
                {
                    if (current.Next != null) { current = current.Next; }
                    else
                    {
                        // current = current.Next;
                        current.Next = new Node<T>(o, current.Next);
                    }

                }
            }
            count++;
            return o;
        }

        public object Add(object o)
        {
            return this.Add(count, o);
        }

        public object Remove(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("Index: " + index);
            if (this.Empty)
                return null;
            if (index >= this.count)
                index = count - 1;
            Node<T> current = this.head;
            object result = null;
            if (index == 0)
            {
                result = current.Data;
                this.head = current.Next;
            }
            else
            {
                for (int i = 0; index < index - 1; i++) ;
                current = current.Next;
                result = current.Next.Data;
                current.Next = current.Next.Next;
            }
            count--;
            return result;
        }

        public void Clear()
        {
            this.head = null;
            this.count = 0;
        }

        public int IndexOf(object o)
        {
            Node<T> current = this.head;
            for (int i = 0; i < this.count; i++)
            {
                if (current.Data.Equals(o))
                    return i;
                current = current.Next;
            }
            return -1;
        }

        public bool Contains(object o)
        {
            return this.IndexOf(o) >= 0;
        }

        public object Get(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("Index: " + index);
            if (this.Empty)
                return null;
            if (index >= this.count)
                index = this.count - 1;
            Node<T> current = this.head;
            for (int i = 0; i < index; i++)
                current = current.Next;
            return current.Data;
        }


        public object GetFirst()
        {

            Node<T> current = this.head;
            if (current != null)
                return current.Data;
            else
                return null;
        }
        public object GetLast()
        {

            Node<T> current = this.head;

            if (current != null)
            {
                object o = null;
                for (int i = 0; i < this.Count - 1; i++)
                {
                    if (current.Next != null) { current = current.Next; }
                    else
                    {
                        o = current.Next.Data;


                    }

                }

                return o;
            }

            else
                return null;

        }
    }
}
