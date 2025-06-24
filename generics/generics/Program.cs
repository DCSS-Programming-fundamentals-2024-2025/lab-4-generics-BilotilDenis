public class Person
{
    public int Id;
    public string Name;
}

public class Student : Person
{
    public void SubmitWork() => Console.WriteLine($"{Name} здає роботу.");
    public void SayName() => Console.WriteLine($"Я — {Name}.");
}

public class Teacher : Person
{
    public void GradeStudent(Student s) => Console.WriteLine($"Оцінювання {s.Name}.");
    public void ExpelStudent(Student s) => Console.WriteLine($"Відрахування {s.Name}.");
    public void ShowPresentStudents() => Console.WriteLine("Показати присутніх студентів.");
}

public interface IRepository<TEntity, TKey>
    where TEntity : class, new()
    where TKey : struct
{
    void Add(TKey id, TEntity entity);
    TEntity Get(TKey id);
    IEnumerable<TEntity> GetAll();
    void Remove(TKey id);
}

public interface IReadOnlyRepository<out TEntity, in TKey>
{
    TEntity Get(TKey id);
    IEnumerable<TEntity> GetAll();
}

public interface IWriteRepository<in TEntity, in TKey>
{
    void Add(TEntity entity);
    void Remove(TKey id);
}

public class InMemoryRepository<TEntity, TKey> : IRepository<TEntity, TKey>, IReadOnlyRepository<TEntity, TKey>, IWriteRepository<TEntity, TKey>
    where TEntity : class, new()
    where TKey : struct
{
    private Dictionary<TKey, TEntity> _store = new();

    public void Add(TKey id, TEntity entity) => _store[id] = entity;
    public TEntity Get(TKey id) => _store.TryGetValue(id, out var entity) ? entity : null;
    public IEnumerable<TEntity> GetAll() => _store.Values;
    public void Remove(TKey id) => _store.Remove(id);

    void IWriteRepository<TEntity, TKey>.Add(TEntity entity)
    {
        throw new NotImplementedException(); 
    }
}
public class Group
{
    public int Id { get; set; }
    public string Name { get; set; }
    private IRepository<Student, int> _students = new InMemoryRepository<Student, int>();

    public void AddStudent(Student s) => _students.Add(s.Id, s);
    public void RemoveStudent(int studentId) => _students.Remove(studentId);
    public IEnumerable<Student> GetAllStudents() => _students.GetAll();
    public Student FindStudent(int studentId) => _students.Get(studentId);
}

public class Faculty
{
    public int Id { get; set; }
    public string Name { get; set; }
    private IRepository<Group, int> _groups = new InMemoryRepository<Group, int>();

    public void AddGroup(Group g) => _groups.Add(g.Id, g);
    public void RemoveGroup(int id) => _groups.Remove(id);
    public IEnumerable<Group> GetAllGroups() => _groups.GetAll();
    public Group GetGroup(int id) => _groups.Get(id);

    public void AddStudentToGroup(int groupId, Student s) => _groups.Get(groupId)?.AddStudent(s);
    public void RemoveStudentFromGroup(int groupId, int studentId) => _groups.Get(groupId)?.RemoveStudent(studentId);
    public IEnumerable<Student> GetAllStudentsInGroup(int groupId) => _groups.Get(groupId)?.GetAllStudents();
    public Student FindStudentInGroup(int groupId, int studentId) => _groups.Get(groupId)?.FindStudent(studentId);
}

public class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        Faculty fpm = new Faculty { Id = 1, Name = "ФПМ" };

        Group kp41 = new Group { Id = 41, Name = "КП-41" };
        Group kp42 = new Group { Id = 42, Name = "КП-42" };
        Group kp43 = new Group { Id = 43, Name = "КП-43" };

        fpm.AddGroup(kp41);
        fpm.AddGroup(kp42);
        fpm.AddGroup(kp43);

        Student s1 = new Student { Id = 1, Name = "Іван" };
        Student s2 = new Student { Id = 2, Name = "Марія" };
        Student s3 = new Student { Id = 3, Name = "Петро" };

        fpm.AddStudentToGroup(41, s1);
        fpm.AddStudentToGroup(41, s2);
        fpm.AddStudentToGroup(42, s3);

        foreach (var student in fpm.GetAllStudentsInGroup(41))
        {
            Console.WriteLine($"Студент: {student.Name}");
        }
    }
}

