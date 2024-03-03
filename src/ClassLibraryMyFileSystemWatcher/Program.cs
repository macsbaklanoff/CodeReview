

string path = @"D:\testForFileSystemWatcher";
MyFileSystemWatcher file = new MyFileSystemWatcher(path);
file.Created += OnCreated;
file.Deleted += OnDeleted;
file.Renamed += OnRenamed;

void OnRenamed(object sender, RenamedEventArgs e)
{
    Console.WriteLine("Renamed: ");
    Console.WriteLine($"Old Name: {e.OldName}");
    Console.WriteLine($"New Name: {e.Name}");
    
}

void OnDeleted(object sender, FileSystemEventArgs e)
{
    Console.WriteLine($"Deleted {e.FullPath}");
}

void OnCreated(object sender, FileSystemEventArgs e)
{
    Console.WriteLine($"Created {e.FullPath}");
}

Console.ReadKey();