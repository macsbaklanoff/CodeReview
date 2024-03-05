namespace ClassLibraryMyFileSystemWatcher
{
    class MyFileSystemWatcher
    {
        private readonly string _path;
        private HashSet<string> hashFiles = new HashSet<string>();

        public MyFileSystemWatcher(string path)
        {
            CheckPathValidity(path);
            _path = path;
            hashFiles = Directory.GetFiles(_path).ToHashSet();
            TimerCallback _timerCallback = new TimerCallback(Test);
            Timer timer = new Timer(_timerCallback, 0, 0, 2500);
        }

        private static Action<List<string>>? _onCreatedHandler;
        private static Action<List<string>>? _onDeletedHandler;
        private static Action<List<string>>? _onRenamedHandler;

        private static void CheckPathValidity(string path)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                throw new ArgumentException("Wrong path or non-existent directory");
            }
        }

        private void Test(object obj)
        {
            var afterHashFiles = Directory.GetFiles(_path).ToHashSet(); //текущее состояние директории
            if (afterHashFiles.Count > hashFiles.Count)
            {
                _onCreatedHandler?.Invoke(OnCreated(ref afterHashFiles));
            }
            else if (afterHashFiles.Count < hashFiles.Count)
            {
                _onDeletedHandler?.Invoke(OnDeleted(ref afterHashFiles));
            }
            else
            {
                if (afterHashFiles.SetEquals(hashFiles)) {
                    return; //если имена файлов (и их кол-во)
                }
                _onRenamedHandler?.Invoke(OnRenamed(ref afterHashFiles));
            }
            hashFiles = afterHashFiles;

        }

        private List<string> OnCreated(ref HashSet<string> afterHashFiles)
        {
            return afterHashFiles.Except(hashFiles).ToList(); //создали файл(ы). название(я) = мн-во текущей - мн-во предыдущей
        }

        private List<string> OnDeleted(ref HashSet<string> afterHashFiles)
        {
            return hashFiles.Except(afterHashFiles).ToList();//удалили файл(ы). название(я) = мн-во предыдущей - мн-во текущей
        }

        private List<string> OnRenamed(ref HashSet<string> afterHashFiles)
        {
            //нахождение переименованного файла путем вычитания каждого мн-ва из их пересечения
            var temp = afterHashFiles.Union(hashFiles).ToList();
            var oldName = temp.Except(afterHashFiles).ToList();
            var newName = temp.Except(hashFiles).ToList();
            return oldName.Union(newName).ToList();
        }

        public event Action<List<string>>? Created
        {
            add
            {
                _onCreatedHandler += value;
                
            }
            remove
            {
                _onCreatedHandler -= value;
            }
        }
        public event Action<List<string>>? Deleted
        {
            add
            {
                _onDeletedHandler += value;
            }
            remove
            {
                _onDeletedHandler -= value;
            }
        }
        public event Action<List<string>>? Renamed
        {
            add
            {
                _onRenamedHandler += value;
            }
            remove
            {
                _onRenamedHandler -= value;
            }
        }
       
    }
}