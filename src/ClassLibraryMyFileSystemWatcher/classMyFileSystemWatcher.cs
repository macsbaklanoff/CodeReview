using System;

class MyFileSystemWatcher
{
    private static string _path;
    private static int CountFilesInDirectory = 0;
    private static HashSet<string> hashFiles = new HashSet<string>();

    public MyFileSystemWatcher()
    {
        _path = string.Empty;
    }
    public MyFileSystemWatcher(string path)
    {
        CheckPathValidity(path);
        _path = path;
        hashFiles = Directory.GetFiles(_path).ToHashSet();
        CountFilesInDirectory = hashFiles.Count;
    }

    //private FileSystemEventHandler? _onChangedHandler;
    private static FileSystemEventHandler? _onCreatedHandler;
    private static FileSystemEventHandler? _onDeletedHandler;
    private static RenamedEventHandler? _onRenamedHandler;

    private static void CheckPathValidity(string path)
    {
        if (path.Length == 0 || path == null || !Directory.Exists(path))
        {
            throw new ArgumentException("Wrong path or non-existent directory");
        }
    }

    public event FileSystemEventHandler? Created
    {
        add
        {
            _onCreatedHandler += value;
            TimerCallback _timerCallback = new TimerCallback(OnCreated);
            Timer timer = new Timer(_timerCallback, 0, 0, 2000);
        }
        remove
        {
            _onCreatedHandler -= value;
        }
    }
    public event FileSystemEventHandler? Deleted
    {
        add
        {
            _onDeletedHandler += value;
            TimerCallback _timerCallback = new TimerCallback(OnDeleted);
            Timer timer = new Timer(_timerCallback, 0, 0, 2000);
        }
        remove
        {
            _onDeletedHandler -= value;
        }
    }
    public event RenamedEventHandler? Renamed
    {
        add
        {
            _onRenamedHandler += value;
            TimerCallback _timerCallback = new TimerCallback(OnRenamed);
            Timer timer = new Timer(_timerCallback, 0, 0, 2000);
        }
        remove
        {
            _onRenamedHandler -= value;
        }
    }
    private static void OnCreated(object obj)
    {
        HashSet<string> tempHashFiles = Directory.GetFiles(_path).ToHashSet();
        int TempCountFiles = tempHashFiles.Count;
        bool flag = false;
        string name = string.Empty;

        foreach (string tempHashFile in tempHashFiles)
        {
            if (!hashFiles.Contains(tempHashFile))
            {
                flag = true;
                name = tempHashFile;
                break;
            }
        }
        if (TempCountFiles > CountFilesInDirectory && flag)
        {
            string[] temp = name.Split("\\").ToArray();
            CountFilesInDirectory = TempCountFiles;
            _onCreatedHandler?.Invoke(obj, new FileSystemEventArgs(new WatcherChangeTypes(), _path, temp[temp.Length - 1]));
        }
        hashFiles = Directory.GetFiles(_path).ToHashSet();
    }
    private static void OnDeleted(object obj)
    {
        HashSet<string> tempHashFiles = Directory.GetFiles(_path).ToHashSet();
        int TempCountFiles = tempHashFiles.Count;
        bool flag = false;
        string name = string.Empty;

        foreach (string hashFile in hashFiles)
        {
            if (!tempHashFiles.Contains(hashFile))
            {
                flag = true;
                name = hashFile;
                break;
            }
        }
        if (TempCountFiles < CountFilesInDirectory && flag)
        {
            string[] temp = name.Split("\\").ToArray();
            CountFilesInDirectory = TempCountFiles;
            _onDeletedHandler?.Invoke(obj, new FileSystemEventArgs(new WatcherChangeTypes(), _path, temp[temp.Length - 1]));
        }
        hashFiles = Directory.GetFiles(_path).ToHashSet();
    }
    private static void OnRenamed(object obj)
    {
        HashSet<string> tempHashFiles = Directory.GetFiles(_path).ToHashSet();
        int TempCountFiles = tempHashFiles.Count;
        bool flag = false;
        string name = string.Empty;

        foreach (var tempHashFile in tempHashFiles)
        {
            if (!hashFiles.Contains(tempHashFile))
            {
                flag = true;
                name = tempHashFile;
                break;
            }
        }
        if (TempCountFiles == CountFilesInDirectory && flag)
        {
            string[] tempNewName = name.Split("\\").ToArray();
            string[] tempOldName = SearchOldName(tempHashFiles).Split("\\").ToArray();
            CountFilesInDirectory = TempCountFiles;

            _onRenamedHandler?.Invoke(obj, new RenamedEventArgs(new WatcherChangeTypes(), _path,
                tempNewName[tempNewName.Length - 1], tempOldName[tempOldName.Length - 1]));
        }
        hashFiles = Directory.GetFiles(_path).ToHashSet();
    }
    private static string SearchOldName(HashSet<string> tempHashFiles)
    {
        List<string> tempListFiles = tempHashFiles.ToList();
        List<string> listFiles = hashFiles.ToList();
        string oldName = string.Empty;

        for (int i = 0; i < tempListFiles.Count && i < listFiles.Count; i++)
        {
            if (tempListFiles[i] != listFiles[i])
            {
                oldName = listFiles[i];
                break;
            }
        }
        return oldName;
    }
}
