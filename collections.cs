//reference:https://www.dotnetperls.com/collections
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Reflection;
using UnityEditor;

class Test
{
	int[] _array;
	public Test()
	{
		Debug.Log("Test()");
		_array = new int[10];
	}
	public int Length
	{
		get
		{
			return _array.Length;
		}
	}
}


public class collections : MonoBehaviour 
{
	public Dropdown dropdown;

	static void ClearConsole()
	{
		var assembly = Assembly.GetAssembly(typeof(SceneView));
		var type = assembly.GetType("UnityEditor.LogEntries");
		var method = type.GetMethod("Clear");
		method.Invoke(new object(), null);
	}

	static void collection_ArrayList() 
	{
		ClearConsole ();
		ArrayList list = new ArrayList(); //This collection dynamically resizes
		list.Add("One");
		list.Add("Two");
		list.Add("Three");
		foreach (string s in list)
			Debug.Log (s);
	}

	static void collection_BitArray()
	{
		ClearConsole ();
		bool[] array = new bool[5];
		array[0] = true;
		array[1] = false; 
		array[2] = true;
		array[3] = false;
		array[4] = true;
		BitArray bitArray = new BitArray(array); //To performs bitwise operations
		foreach (bool bit in bitArray)
		{
			Debug.Log(bit); //Error -> Unity shows only first and second value
		}
	}

	void collection_ConcurrentBag()
	{
		ClearConsole ();
		ConcurrentBag<int> bag = new ConcurrentBag<int>(); //For multiple threads
		bag.Add(1);
		bag.Add(2);
		bag.Add(3);

		int result;
		if (bag.TryPeek(out result))
		{
			Debug.Log("TryPeek: "+ result);
		}

		if (bag.TryTake(out result))
		{
			Debug.Log("TryTake: "+ result);
		}

		if (bag.TryPeek(out result))
		{
			Debug.Log("TryPeek: "+ result);
		}
	}

	void collection_ConcurrentDictionary ()
	{
		ClearConsole ();
		//Allows multiple threads to access a Dictionary instance. With it, you get a thread-safe, hash-based lookup table.
		var con = new ConcurrentDictionary<string, int>();
		con.TryAdd("cat", 1);
		con.TryAdd("dog", 2);
		con.TryUpdate("cat", 200, 4);
		con.TryUpdate("cat", 100, 1);
		Debug.Log(con["cat"]);
	}

	void collection_ConcurrentQueue() 
	{
		ClearConsole ();
		ConcurrentQueue<int> queue = new ConcurrentQueue<int>(); //Use queues in threads
		queue.Enqueue(10);
		queue.Enqueue(20);
		Debug.Log(string.Join(",", queue.ToArray()));
		int resultPeek;
		if (queue.TryPeek(out resultPeek))
		{
			Debug.Log("TryPeek result:" + resultPeek);
		}
		int resultDequeue;
		if (queue.TryDequeue(out resultDequeue))
		{
			Debug.Log("TryDeque result:" + resultDequeue);
		}
		Debug.Log(string.Join(",", queue.ToArray()));
	}

	void collection_ConcurrentStack()
	{
		ClearConsole ();
		int[] elements = { 50, 10, 0 };
		ConcurrentStack<int> stack = new ConcurrentStack<int>(elements);
		Debug.Log(string.Join(",", stack.ToArray()));
		stack.Push(2000);
		Debug.Log(string.Join(",", stack.ToArray()));
		int resultPeek;
		if (stack.TryPeek(out resultPeek))
		{
			Debug.Log("TryPeek result:" + resultPeek);
		}
		int resultPop;
		if (stack.TryPop(out resultPop))
		{
			Debug.Log("TryPop result:" + resultPop);
		}
		Debug.Log(string.Join(",", stack.ToArray()));
	}

	void collection_Dictionary ()
	{
		ClearConsole ();
		Dictionary<string, int> d = new Dictionary<string, int>()
		{
			{"cat", 2},
			{"dog", 1},
			{"llama", 0},
			{"iguana", -1}
		};
		foreach (KeyValuePair<string, int> pair in d)
		{
			Debug.Log(pair.Key+" "+ pair.Value);
		}
		foreach (var pair in d)
		{
			Debug.Log( pair.Key+" "+ pair.Value);
		}
	}

	void collection_DictionaryEntry()
	{
		ClearConsole ();
		//DictionaryEntry is used with Hashtable. The Hashtable collection provides a way to access objects based on a key. 
		//But it does not implement advanced logic for iterating over the contents. We use DictionaryEntry in a foreach-loop.
		Hashtable hashtable = new Hashtable();
		hashtable.Add(1, "one");
		hashtable.Add(2, "two");
		hashtable.Add(3, "three");
		foreach (DictionaryEntry entry in hashtable)
		{
			Debug.Log(entry.Key+" "+entry.Value);
		}
	}

	void collection_HashSet()
	{
		ClearConsole ();
		// This is an optimized set collection. It helps eliminates duplicate strings or elements in an array.
		//It is a set that hashes its contents.
		string[] array1 ={"cat","dog","cat","leopard","tiger","cat"};
		Debug.Log(string.Join(",", array1));
		var hash = new HashSet<string>(array1);
		string[] array2 = hash.ToArray();
		Debug.Log(string.Join(",", array2));
	}

	void collection_Hashtable()
	{
		ClearConsole ();
		// This optimizes lookups. It computes a hash of each key you add. 
		//It then uses this hash code to look up the element very quickly.
		Hashtable hashtable = new Hashtable();
		hashtable.Add(1, "Sandy");
		hashtable.Add(2, "Bruce");
		hashtable.Add(3, "Fourth");
		hashtable.Add(10, "July");
		int count = hashtable.Count;
		Debug.Log(count);
		hashtable.Clear();
		Debug.Log(hashtable.Count);
	}

	void collection_HybridDictionary()
	{
		ClearConsole ();
		//HybridDictionary attempts to optimize Hashtable. It implements a linked list and hash table data structure,
		//switching over to the second from the first when the number of elements increases past a certain threshold.
		HybridDictionary hybrid = new HybridDictionary();
		hybrid.Add("cat", 1);
		hybrid.Add("dog", 2);
		hybrid["rat"] = 0;
		int value1 = (int)hybrid["cat"];
		object value2 = hybrid["notfound"];
		object value3 = hybrid["dog"];
		int count1 = hybrid.Count;
		Debug.Log(value1);
		Debug.Log(value2 == null);
		Debug.Log(value3);
		Debug.Log(count1);
	}

	void struct_KeyValuePair()
	{
		ClearConsole ();
		//In C# a KeyValuePair struct (often used inside dictionaries) joins 2 things together.
		var list = new List<KeyValuePair<string, int>>();
		list.Add(new KeyValuePair<string, int>("Cat", 1));
		list.Add(new KeyValuePair<string, int>("Dog", 2));
		list.Add(new KeyValuePair<string, int>("Rabbit", 4));
		foreach (var element in list)
		{
			Debug.Log(element);
		}
	}


	void type_Lazy()
	{
		ClearConsole ();
		//Lazy instantiation delays certain tasks. It typically improves the startup time of a C# application. 
		//This has always been possible to implement. But the .NET Framework now offers the Lazy type, which provides this feature.
		Lazy<Test> lazy = new Lazy<Test>();
		Debug.Log("IsValueCreated "+ lazy.IsValueCreated);
		Test test = lazy.Value;
		Debug.Log("IsValueCreated "+ lazy.IsValueCreated);
		Debug.Log("Length "+ test.Length);
	}

	void collection_LinkedList()
	{
		ClearConsole ();
		//This generic type allows fast inserts and removes. It implements a classic linked list. Each object is separately allocated.
		LinkedList<string> linked = new LinkedList<string>();
		linked.AddLast("cat");
		linked.AddLast("dog");
		linked.AddLast("man");
		linked.AddFirst("first");
		foreach (var item in linked)
		{
			Debug.Log(item);
		}
	}

	void collection_List()
	{
		ClearConsole ();
		List<string> list = new List<string>();
		list.Add("anchovy");
		list.Add("barracuda");
		list.Add("bass");
		list.Add("viperfish");
		list.Reverse();
		foreach (string value in list)
		{
			Debug.Log(value);
		}
	}

	void collection_ListDictionary()
	{
		ClearConsole ();
		//In small collections, it will be faster than a Hashtable with the same data.
		ListDictionary list = new ListDictionary();
		list.Add("dot", 1);
		list.Add("net", 2);
		list.Add("perls", 3);
		Debug.Log("ListDictionary.Count: "+ list.Count);
	}

	void collection_NameValueCollection()
	{
		ClearConsole ();
		//NameValueCollection allows many values for one key. It is found in System.Collections.Specialized. 
		//It does not provide excellent performance. Other collections are likely to be faster for your program.
		NameValueCollection collection = new NameValueCollection();
		collection.Add("Sam", "Dot Net Perls");
		collection.Add("Bill", "Microsoft");
		collection.Add("Bill", "White House");
		collection.Add("Sam", "IBM");
		foreach (string key in collection.AllKeys) // <-- No duplicates returned.
		{
			Debug.Log(key);
		}
	}

	void collection_Queue()
	{
		ClearConsole ();
		Queue<int> collection = new Queue<int>();
		collection.Enqueue(5);
		collection.Enqueue(6);
		foreach (int value in collection)
		{
			Debug.Log(value);
		}
	}

	void collection_ReadOnlyCollection()
	{
		ClearConsole ();
		//ReadOnlyCollection makes an array or List read-only. With this type from System.Collections.ObjectModel,
		//we provide a collection of elements that cannot be changed. But it may be possible to change some elements themselves.
		List<int> list = new List<int>();
		list.Add(1);
		list.Add(3);
		list.Add(5);
		ReadOnlyCollection<int> read = new ReadOnlyCollection<int>(list);
		foreach (int value in read)
		{
			Debug.Log("read: "+ value);
		}
		int[] array = new int[3];
		read.CopyTo(array, 0);
		foreach (int value in array)
		{
			Debug.Log("array: "+value);
		}
		int count = read.Count;
		bool contains = read.Contains(-1);
		int index = read.IndexOf(3);
		Debug.Log(count+" "+contains+" "+index);
	}

	void collection_SortedDictionary()
	{
		ClearConsole ();
		SortedDictionary<string, int> sort = new SortedDictionary<string, int>();
		sort.Add("zebra", 5);
		sort.Add("cat", 2);
		sort.Add("dog", 9);
		sort.Add("mouse", 4);
		sort.Add("programmer", 100);
		if (sort.ContainsKey("dog"))
		{
			Debug.Log(true);
		}
		if (sort.ContainsKey("zebra"))
		{
			Debug.Log(true);
		}
		Debug.Log(sort.ContainsKey("ape"));
		int v;
		if (sort.TryGetValue("programmer", out v))
		{
			Debug.Log(v);
		}
		foreach (KeyValuePair<string, int> p in sort)
		{
			Debug.Log(p.Key+" "+p.Value);
		}
	}

	void collection_SortedList()
	{
		ClearConsole ();
		SortedList<string, int> sorted = new SortedList<string, int>();
		sorted.Add("perls", 3);
		sorted.Add("dot", 1);
		sorted.Add("net", 2);
		bool contains1 = sorted.ContainsKey("java");
		Debug.Log("contains java = " + contains1);
		int value;
		if (sorted.TryGetValue("perls", out value))
		{
			Debug.Log("perls key is = " + value);
		}
		Debug.Log("dot key is = " + sorted["dot"]);
		foreach (var pair in sorted)
		{
			Debug.Log(pair);
		}
		int index1 = sorted.IndexOfKey("net");
		Debug.Log("index of net (key) = " + index1);
		int index2 = sorted.IndexOfValue(3);
		Debug.Log("index of 3 (value) = " + index2);
		Debug.Log("count is = " + sorted.Count);
	}

	void collection_SortedSet()
	{
		ClearConsole ();
		SortedSet<string> set = new SortedSet<string>();
		set.Add("perls");
		set.Add("net");
		set.Add("dot");
		set.Add("sam");
		set.Remove("sam");
		foreach (string val in set) 
		{
			Debug.Log(val);
		}
	}

	void collection_Stack()
	{
		ClearConsole ();
		Stack<int> stack = new Stack<int>();
		stack.Push(100);
		stack.Push(1000);
		stack.Push(10000);
		Debug.Log("--- Stack contents ---");
		foreach (int i in stack)
		{
			Debug.Log(i);
		}
	}

	void collection_StringDictionary()
	{
		ClearConsole ();
		StringDictionary dict = new StringDictionary();
		dict.Add("cat", "feline");
		dict.Add("dog", "canine");
		Debug.Log(dict["cat"]);
		Debug.Log(dict["test"] == null);
		Debug.Log(dict.ContainsKey("cat"));
		Debug.Log(dict.ContainsKey("puppet"));
		Debug.Log(dict.ContainsValue("feline"));
		Debug.Log(dict.ContainsValue("perls"));
		Debug.Log(dict.IsSynchronized);
		foreach (DictionaryEntry entry in dict)
		{
			Debug.Log(entry.Key+" "+entry.Value);
		}
		foreach (string key in dict.Keys)
		{
			Debug.Log(key);
		}
		foreach (string value in dict.Values)
		{
			Debug.Log(value);
		}
		dict.Remove("cat");
		Debug.Log(dict.Count);
		dict.Clear();
		Debug.Log(dict.Count);
	}

	void collection_Tuple()
	{
		ClearConsole ();
		Tuple<int, string, bool> tuple = new Tuple<int, string, bool>(1, "cat", true);
		if (tuple.Item1 == 1)
		{
			Debug.Log(tuple.Item1);
		}
		if (tuple.Item2 == "dog")
		{
			Debug.Log(tuple.Item2);
		}
		if (tuple.Item3)
		{
			Debug.Log(tuple.Item3);
		}
	}

	delegate void options();

	void DoSomething(int arg0)
	{		
		List<options> options = new List<options>();
		options.Add(collection_ArrayList);
		options.Add(collection_BitArray);
		options.Add(collection_ConcurrentBag);
		options.Add(collection_ConcurrentDictionary);
		options.Add(collection_ConcurrentQueue);
		options.Add(collection_ConcurrentStack);
		options.Add(collection_Dictionary);
		options.Add(collection_DictionaryEntry);
		options.Add(collection_HashSet);
		options.Add(collection_Hashtable);
		options.Add(collection_HybridDictionary);
		options.Add(struct_KeyValuePair);
		options.Add(type_Lazy);
		options.Add(collection_LinkedList);
		options.Add(collection_List);
		options.Add(collection_ListDictionary);
		options.Add(collection_NameValueCollection);
		options.Add(collection_Queue);
		options.Add(collection_ReadOnlyCollection);
		options.Add(collection_SortedDictionary);
		options.Add(collection_SortedList);
		options.Add(collection_SortedSet);
		options.Add(collection_Stack);
		options.Add(collection_StringDictionary);
		options.Add(collection_Tuple);
		options[arg0]();
	}

	void Start () 
	{
		dropdown.onValueChanged.AddListener(DoSomething);
	}
		
}
