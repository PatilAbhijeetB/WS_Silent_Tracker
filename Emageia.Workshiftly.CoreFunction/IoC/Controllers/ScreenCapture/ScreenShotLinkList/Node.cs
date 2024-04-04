using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.CoreFunction.IoC.Controllers.ScreenCapture.ScreenShotLinkList
{
    public class Node<T>
    {
        private object data;
        private Node<T> next;
     

        public Node(object data, Node<T> next)
        {
            this.data = data;
            this.next = next;
        }

        public object Data
        {
            get { return this.data; }
            set { this.data = value; }
        }

        public Node<T> Next
        {
            get { return this.next; }
            set { this.next = value; }
        }
    }
}
