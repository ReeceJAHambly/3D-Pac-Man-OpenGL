using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace OpenGL_Game.Components
{
    class ComponentEnemy : IComponent
    {
        //Ordered Size
        Dictionary<Vector3, Vector3[]> mNeighbourList;
        Vector3[] mNodes;
        Vector3 mCurrentDestination;
       
        public ComponentEnemy(Vector3[] posNodes, Dictionary<Vector3,Vector3[]> neighbourList)
        {
            mNodes = posNodes;
            mNeighbourList = neighbourList;
            mCurrentDestination = posNodes[mNodes.Length - 1];
        }

        public Vector3[] Nodes
        {
            get { return mNodes; }
            set { mNodes = value; }
        }

        public Dictionary<Vector3, Vector3[]> Neighbours
        {
            get { return mNeighbourList; }
            set { mNeighbourList = value; }
        }

        public Vector3 Destination
        {
            get { return mCurrentDestination; }
            set { mCurrentDestination = value; }
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_ENEMY; }
        }

        public void Close()
        {

        }
    }
}
