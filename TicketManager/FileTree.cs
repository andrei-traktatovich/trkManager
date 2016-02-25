using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TicketManager
{
    public enum FileNodeType
    {
        File, Folder
    }

    public class FileTree 
    {
        public List<FileNode> Nodes = new List<FileNode>();
        private FileNode _rootNode;
        public FileNode RootNode { get { return _rootNode;} }

        public void CreateTreeFromPaths(IEnumerable<TicketDataModel.JobFile> files)
        {
            this.Nodes = new List<FileNode>();
            var rootNode = new FileNode(FileNodeType.Folder, "z:", this);
            this.Nodes.Add(rootNode);
            rootNode.Tree = this;
            _rootNode = rootNode;
            foreach (var f in files)
                rootNode.AddFile(f);
        }
    }
    public class FileNode
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public FileNodeType Type { get; set;}
        public int ParentID { get { return _ParentID; } set { _ParentID = value; }  }
        public int ID { get; set; }
        public FileTree Tree { get { return _Tree;} set { _Tree = value; }}
        private FileTree _Tree; 
        private int _ParentID;

        public FileNode(FileNodeType type, string name, FileTree tree)
        {
            _ParentID = -1;
            _Tree = tree;
            Name = name;
            this.ID = 0;
        }

        public FileNode(FileNodeType type, string name, FileNode parent)
        {
            if (parent == null)
                throw new ArgumentException("Родителя нет!!!");
            else _ParentID = parent.ID;
            this._Tree = parent.Tree;
            this.Name = name;
            this.ID = this._Tree.Nodes.Count();
        }

        public void AddFile(TicketDataModel.JobFile file)
        {
            string[] elements = file.FilePath.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);
            this.CreateChildrenFromPath(elements, this, 1);
        }
        
        private void CreateChildrenFromPath(string[] elements, FileNode node, int count)
        {
            
            if (count == elements.Length)
                return;

            string element = elements[count];

            FileNode newNode;
            var children = node.Children.ToList();
            var candidate = children.Where(x => x.Name == element).SingleOrDefault();
            if (children.Count() > 0 && candidate != null)
                newNode = candidate;
            else
            {
                newNode = new FileNode(FileNodeType.Folder, element, node);
                this.Tree.Nodes.Add(newNode);
            }

            CreateChildrenFromPath(elements, newNode, count + 1);
        }
        public void AddChild(FileNode newNode)
        {
            newNode.ParentID = this._ParentID;
            newNode.Tree = this._Tree;
        }

        public IEnumerable<FileNode> Children
        {
            get
            {
                var _Tree = this._Tree;
                return _Tree.Nodes.Where(x => x.ParentID == this.ID);
            }
        }
    }
}