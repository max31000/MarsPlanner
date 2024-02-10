using Models.Navigation;
using QuikGraph;

namespace Components.Navigation
{
    public struct GlobalNavigationDataComponent
    {
        public UndirectedGraph<NavigationNode, Edge<NavigationNode>> NavigationGrid { get; set; }
        
        
    }
}