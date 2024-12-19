using System;
using System.Collections.Generic;

public class AVLTree<K, D> where K : IComparable<K>
{
    private class Node
    {
        public K Key;
        public D Data;
        public Node Left;
        public Node Right;
        public int Height;

        public Node(K key, D data)
        {
            Key = key;
            Data = data;
            Height = 1;
        }
    }

    private Node root;

    private int Height(Node node) => node?.Height ?? 0;

    private int BalanceFactor(Node node) => node == null ? 0 : Height(node.Left) - Height(node.Right);

    private Node RotateLeft(Node node)
    {
        Node newRoot = node.Right;
        node.Right = newRoot.Left;
        newRoot.Left = node;
        node.Height = Math.Max(Height(node.Left), Height(node.Right)) + 1;
        newRoot.Height = Math.Max(Height(newRoot.Left), Height(newRoot.Right)) + 1;
        return newRoot;
    }

    private Node RotateRight(Node node)
    {
        Node newRoot = node.Left;
        node.Left = newRoot.Right;
        newRoot.Right = node;
        node.Height = Math.Max(Height(node.Left), Height(node.Right)) + 1;
        newRoot.Height = Math.Max(Height(newRoot.Left), Height(newRoot.Right)) + 1;
        return newRoot;
    }

    private Node Balance(Node node)
    {
        if (BalanceFactor(node) > 1)
        {
            if (BalanceFactor(node.Left) < 0)
                node.Left = RotateLeft(node.Left);
            return RotateRight(node);
        }
        if (BalanceFactor(node) < -1)
        {
            if (BalanceFactor(node.Right) > 0)
                node.Right = RotateRight(node.Right);
            return RotateLeft(node);
        }
        return node;
    }

    public void Insert(K key, D data)
    {
        root = Insert(root, key, data);
    }

    private Node Insert(Node node, K key, D data)
    {
        if (node == null) return new Node(key, data);

        int comparison = key.CompareTo(node.Key);
        if (comparison < 0)
            node.Left = Insert(node.Left, key, data);
        else if (comparison > 0)
            node.Right = Insert(node.Right, key, data);
        else
            throw new InvalidOperationException("Key already exists.");

        node.Height = Math.Max(Height(node.Left), Height(node.Right)) + 1;
        return Balance(node);
    }

    public void Remove(K key)
    {
        root = Remove(root, key);
    }

    private Node Remove(Node node, K key)
    {
        if (node == null) return null;

        int comparison = key.CompareTo(node.Key);
        if (comparison < 0)
            node.Left = Remove(node.Left, key);
        else if (comparison > 0)
            node.Right = Remove(node.Right, key);
        else
        {
            if (node.Left == null || node.Right == null)
            {
                node = node.Left ?? node.Right;
            }
            else
            {
                Node minNode = GetMinNode(node.Right);
                node.Key = minNode.Key;
                node.Data = minNode.Data;
                node.Right = Remove(node.Right, minNode.Key);
            }
        }
        if (node == null) return null;

        node.Height = Math.Max(Height(node.Left), Height(node.Right)) + 1;
        return Balance(node);
    }

    private Node GetMinNode(Node node)
    {
        while (node.Left != null) node = node.Left;
        return node;
    }

    public D Search(K key)
    {
        Node node = Search(root, key);
        if (node == null) return default;
        return node.Data;
    }

    private Node Search(Node node, K key)
    {
        if (node == null) return null;

        int comparison = key.CompareTo(node.Key);
        if (comparison < 0) return Search(node.Left, key);
        if (comparison > 0) return Search(node.Right, key);
        return node;
    }

    public K GetMin()
    {
        if (root == null) throw new InvalidOperationException("Tree is empty.");
        return GetMinNode(root).Key;
    }
    public K GetMax()
    {
        if (root == null) throw new InvalidOperationException("Tree is empty.");
        Node node = root;
        while (node.Right != null) node = node.Right;
        return node.Key;
    }

    public int CountNodes()
    {
        return CountNodes(root);
    }

    private int CountNodes(Node node)
    {
        if (node == null) return 0;
        return 1 + CountNodes(node.Left) + CountNodes(node.Right);
    }

    public bool IsBalanced()
    {
        return IsBalanced(root);
    }

    private bool IsBalanced(Node node)
    {
        if (node == null) return true;

        int balance = BalanceFactor(node);
        if (balance < -1 || balance > 1) return false;

        return IsBalanced(node.Left) && IsBalanced(node.Right);
    }

    public int GetHeight()
    {
        return Height(root);
    }

    public override string ToString()
    {
        return ToString(root, "", true);
    }

    private string ToString(Node node, string indent, bool last)
    {
        if (node == null) return "";
        string result = indent + (last ? "R----" : "L----") + node.Key + "\n";
        result += ToString(node.Left, indent + (last ? "     " : "|    "), false);
        result += ToString(node.Right, indent + (last ? "     " : "|    "), true);
        return result;
    }

    public AVLTree<K, D> Copy()
    {
        AVLTree<K, D> newTree = new AVLTree<K, D>();
        newTree.root = Copy(root);
        return newTree;
    }

    private Node Copy(Node node)
    {
        if (node == null) return null;
        Node newNode = new Node(node.Key, node.Data);
        newNode.Left = Copy(node.Left);
        newNode.Right = Copy(node.Right);
        newNode.Height = node.Height;
        return newNode;
    }

    public void ReverseInOrderTraversal(Action<K> action)
    {
        ReverseInOrderTraversal(root, action);
    }

    private void ReverseInOrderTraversal(Node node, Action<K> action)
    {
        if (node == null) return;
        ReverseInOrderTraversal(node.Right, action);
        action(node.Key);
        ReverseInOrderTraversal(node.Left, action);
    }

   

    public List<K> GetKeysInRange(K min, K max)
    {
        List<K> result = new List<K>();
        GetKeysInRange(root, min, max, result);
        return result;
    }

    private void GetKeysInRange(Node node, K min, K max, List<K> result)
    {
        if (node == null) return;
        if (node.Key.CompareTo(min) > 0) GetKeysInRange(node.Left, min, max, result);
        if (node.Key.CompareTo(min) >= 0 && node.Key.CompareTo(max) <= 0)
            result.Add(node.Key);
        if (node.Key.CompareTo(max) < 0) GetKeysInRange(node.Right, min, max, result);
    }

    public bool IsSubtree(AVLTree<K, D> otherTree)
    {
        return IsSubtree(root, otherTree.root);
    }

    private bool IsSubtree(Node node, Node subTreeRoot)
    {
        if (node == null) return false;
        if (node.Key.Equals(subTreeRoot.Key) && IsEqual(node, subTreeRoot)) return true;
        return IsSubtree(node.Left, subTreeRoot) || IsSubtree(node.Right, subTreeRoot);
    }

    private bool IsEqual(Node node1, Node node2)
    {
        if (node1 == null && node2 == null) return true;
        if (node1 == null || node2 == null) return false;
        return node1.Key.Equals(node2.Key) && IsEqual(node1.Left, node2.Left) && IsEqual(node1.Right, node2.Right);
    }
}
public class Program
{
    public static void Main()
    {
        AVLTree<int, string> tree = new AVLTree<int, string>();

        while (true)
        {
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1 - Вставить элемент");
            Console.WriteLine("2 - Удалить элемент");
            Console.WriteLine("3 - Найти элемент");
            Console.WriteLine("4 - Получить минимальный элемент");
            Console.WriteLine("5 - Получить максимальный элемент");
            Console.WriteLine("6 - Подсчитать количество узлов");
            Console.WriteLine("7 - Проверить баланс");
            Console.WriteLine("8 - Получить высоту");
            Console.WriteLine("9 - Вывести дерево в строку");
            Console.WriteLine("10 - Копировать дерево");
            Console.WriteLine("11 - Обратный симметричный обход");
            Console.WriteLine("12 - Получить предшественника");
            Console.WriteLine("13 - Получить ключи в диапазоне");
            Console.WriteLine("14 - Проверить поддерево");
            Console.WriteLine("15 - Завершить работу");
            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    Console.Write("Введите ключ для вставки: ");
                    int insertKey = int.Parse(Console.ReadLine());
                    Console.Write("Введите данные для вставки: ");
                    string insertData = Console.ReadLine();
                    tree.Insert(insertKey, insertData);
                    Console.WriteLine("Элемент вставлен.");
                    break;

                case 2:
                    Console.Write("Введите ключ для удаления: ");
                    int removeKey = int.Parse(Console.ReadLine());
                    tree.Remove(removeKey);
                    Console.WriteLine("Элемент удален.");
                    break;

                case 3:
                    Console.Write("Введите ключ для поиска: ");
                    int searchKey = int.Parse(Console.ReadLine());
                    string searchResult = tree.Search(searchKey);
                    Console.WriteLine(searchResult == null ? "Элемент не найден." : $"Найден элемент: {searchResult}");
                    break;

                case 4:
                    Console.WriteLine($"Минимальный элемент: {tree.GetMin()}");
                    break;

                case 5:
                    Console.WriteLine($"Максимальный элемент: {tree.GetMax()}");
                    break;

                case 6:
                    Console.WriteLine($"Количество узлов в дереве: {tree.CountNodes()}");
                    break;

                case 7:
                    Console.WriteLine($"Дерево сбалансировано: {tree.IsBalanced()}");
                    break;

                case 8:
                    Console.WriteLine($"Высота дерева: {tree.GetHeight()}");
                    break;

                case 9:
                    Console.WriteLine("Дерево в виде строки:");
                    Console.WriteLine(tree.ToString());
                    break;

                case 10:
                    AVLTree<int, string> copiedTree = tree.Copy();
                    Console.WriteLine("Дерево скопировано.");
                    break;

                case 11:
                    Console.WriteLine("Обратный симметричный обход:");
                    tree.ReverseInOrderTraversal(key => Console.WriteLine(key));
                    break;

                case 12:
                   
                case 13:
                    Console.Write("Введите минимальный ключ: ");
                    int minKey = int.Parse(Console.ReadLine());
                    Console.Write("Введите максимальный ключ: ");
                    int maxKey = int.Parse(Console.ReadLine());
                    var keysInRange = tree.GetKeysInRange(minKey, maxKey);
                    Console.WriteLine("Ключи в диапазоне:");
                    foreach (var key in keysInRange)
                        Console.WriteLine(key);
                    break;

                case 14:
                    Console.Write("Введите ключ для проверки поддерева: ");
                    int subtreeKey = int.Parse(Console.ReadLine());
                    var subtreeTree = new AVLTree<int, string>();
                    Console.WriteLine($"Поддерево с ключом {subtreeKey} найдено: {tree.IsSubtree(subtreeTree)}");
                    break;

                case 15:
                    Console.WriteLine("Завершаем работу.");
                    return;

                default:
                    Console.WriteLine("Неверный выбор.");
                    break;
            }
        }
    }
}