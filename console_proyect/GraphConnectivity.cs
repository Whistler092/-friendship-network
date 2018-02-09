using System;
using System.Collections.Generic;
using System.Linq;

namespace console_proyect
{
    public class GraphConnectivity
    {
        private List<Node> nodes;
        private string text;


        /// <summary>
        /// Initializes a new instance of the <see cref="T:console_proyect.GraphConnectivity"/> class.
        /// Cargo el array de Pruebas
        /// Creo el grafo a partir del array utilizando la teoría de grafos relacionados
        /// </summary>
        public GraphConnectivity()
        {
            LoadArray();
            CreateAsociations();
        }

        /// <summary>
        /// Creates the asociations.
        /// </summary>
        private void CreateAsociations()
        {
            // Recorro los nodos, contando las aristas pendientes por asociar
            foreach (var n in nodes)
            {
                int edgeNeed = Convert.ToInt16(n.name);
                if (n.edges != null && n.edges.Any())
                    edgeNeed = edgeNeed - n.edges.Count();
                Console.WriteLine("Node: " + n.name + " edgeNeed: " + edgeNeed);

                // Asocio los Nodos entre si.
                randomAsociate(n, edgeNeed);

                // Imprimo las asociaciones
                printCurrentGraph();
            }
        }

        /// <summary>
        /// Prints the current graph and edges.
        /// </summary>
        private void printCurrentGraph()
        {
            var result = true;
            Console.WriteLine("Estado Actual del Grafo");
            foreach (var n in nodes)
            {
                string currentNodes = "";
                if (n.edges != null)
                    n.edges.ToList().ForEach(i => currentNodes += "[" + i.idToString + "]");
                else
                    n.edges = new List<Node>();
                Console.WriteLine("[" + n.idToString + "]" + "Node: " + n.name + " Asociate with:" + currentNodes + " - "
                                  + (n.edges.Count() == Convert.ToInt16(n.name) ? "[Completado]" : n.edges.Count() + "/" + n.name));

                if (n.edges.Count() != Convert.ToInt16(n.name))
                    result = false;
            }
            Console.WriteLine("Para la entrada " + text + " El resultado es " + (result ? "1" : "0"));

        }

        /// <summary>
        /// Randoms the asociate.  
        /// </summary>
        /// <param name="exceptId">Except identifier.</param>
        /// <param name="edgeNeed">Edge need.</param>
        private void randomAsociate(Node exceptId, int edgeNeed)
        {
            // 1.Aquí se recorrerán primeros los nodos que nunca han sido asociados en orden decendente

            foreach (var n in nodes.Where(i => !i.visited).OrderByDescending(i => i.name))
            {
                // 2. Si la asociación está completa, termina la ejecución
                if (edgeNeed == 0)
                    break;

                // Evito que el nodo se asocie consigomismo
                if (n.id.Equals(exceptId.id))
                    continue;

                if (n.edges == null)
                    n.edges = new List<Node>();


                // Verifico que el nodo a postular, no tenga las asociaciones completas
                if (n.edges != null && n.edges.Count() < Convert.ToInt16(n.name))
                {
                    // Verifico que el nodo a postular ya no esté asociado inversamente conmigo
                    if (CheckNodesAlreadyAsociated(n.edges, exceptId))
                        continue;

                    // Se cataloga como un nodo ya catalogado y se agrega como asociado 
                    n.visited = true;
                    n.edges.Add(exceptId);

                    // Y como es una relacion bidireccional, agrego el nodo a postular a mis nodos.
                    if (exceptId.edges == null)
                        exceptId.edges = new List<Node>();
                    exceptId.edges.Add(n);

                    edgeNeed--;

                    Console.WriteLine("Parent : " + exceptId.name + "[" + exceptId.idToString + "]"
                                      + " Asociate with:" + n.name + "[" + n.idToString + "]");
                }
            }

            // 2. Aquí se recorrerán los nodos que ya han pasado por una primera asociación.
            foreach (var n in nodes.Where(i => i.visited).OrderByDescending(i => i.name))
            {
                if (edgeNeed == 0)
                    break;

                if (n.id.Equals(exceptId.id))
                    continue;

                if (n.edges == null)
                    n.edges = new List<Node>();

                if (n.edges != null && n.edges.Count() < Convert.ToInt16(n.name))
                {
                    if (CheckNodesAlreadyAsociated(n.edges, exceptId))
                        continue;

                    n.edges.Add(exceptId);

                    if (exceptId.edges == null)
                        exceptId.edges = new List<Node>();
                    exceptId.edges.Add(n);

                    edgeNeed--;

                    Console.WriteLine("Parent : " + exceptId.name + "[" + exceptId.idToString + "]"
                                      + " Asociate with:" + n.name + "[" + n.idToString + "]");

                }
            }
        }

        /// <summary>
        /// Checks the nodes already asociated.
        /// </summary>
        /// <returns><c>true</c>, if nodes already asociated was checked, <c>false</c> otherwise.</returns>
        /// <param name="edges">Edges.</param>
        /// <param name="exceptId">Except identifier.</param>
        private bool CheckNodesAlreadyAsociated(List<Node> edges, Node exceptId)
        {
            return edges.Any(i => i.id == exceptId.id);
        }

        /// <summary>
        /// Loads the array.
        /// Convierto cada Nodo en un Objeto con un ID unico y un numero de relaciones.
        /// Organizo en Nodo de forma Descendiente (De mayor a menor)
        /// </summary>
        private void LoadArray()
        {
            nodes = new List<Node>();
            Console.Write("Esciba la entrada separada por espacios. ej. 4 3 2 2 3 :");

            text = Console.ReadLine();
            if (string.IsNullOrEmpty(text) || !text.Split(' ').Any())
            {
                Console.Write("Introduzca datos validos.");
                return;
            }

            //text = "8 2 5 4 5 4 3 5 2";
            //text = "4 3 2 2 3";
            //text = "4 1 3 2 3";
            string[] nodesString = text.Split(' ');
            for (int i = 0; i < nodesString.Length; i++)
            {
                string currentNode = nodesString[i];
                if (i != 0)
                    nodes.Add(new Node { id = Guid.NewGuid(), name = currentNode });
            }
            nodes = nodes.OrderByDescending(i => i.name).ToList();
        }
    }

    public class Node
    {
        public Guid id
        {
            get;
            set;
        }
        public string idToString
        {
            get
            {
                return id.ToString().Substring(1, 5);
            }
        }
        public string name
        {
            get;
            set;
        }

        public bool visited
        {
            get;
            set;
        }

        public List<Node> edges
        {
            get;
            set;
        }
    }
}
