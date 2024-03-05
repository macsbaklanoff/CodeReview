using ClassLibraryMyFileSystemWatcher;

string path = @"TestForMyFileSystemWatcher";
MyFileSystemWatcher file = new MyFileSystemWatcher(path);
file.Created += OnCreated;
file.Deleted += OnDeleted;
file.Renamed += OnRenamed;

void OnRenamed(List<string> names)
{
    Console.WriteLine("Renamed: ");
    Console.WriteLine($"Old: {names[0]}");
    Console.WriteLine($"New: {names[1]}");
}

void OnDeleted(List<string> names)
{
    Console.WriteLine($"Deleted: ");
    names.ForEach(x => Console.WriteLine(x));
}

void OnCreated(List<string> names)
{
    Console.Write("Created: ");
    names.ForEach(x => Console.WriteLine(x));
}

Console.ReadKey();