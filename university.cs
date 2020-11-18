using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace PT1
{
   public interface IPerson
   {
      string FirstName { get; }
      string LastName { get; }
      string Patronymic { get; }
      DateTime Birthday { get; }
      int Age { get; }
   }
   public class Student : IPerson
   {
      public string FirstName { get; }
      public string LastName { get; }
      public string Patronymic { get; }
      public DateTime Birthday { get; }
      public byte Course { get; }
      public string Group { get; }
      public float AvgPoint { get; }
      public int Age => Birthday.CalculateAge();
      public Student(string firstName, string lastName,
         string patronymic, DateTime birthday, byte сourse, string group, float avgPoint)
      {
         FirstName = firstName;
         LastName = lastName;
         Patronymic = patronymic;
         Birthday = birthday;
         Course = сourse;
         Group = group;
         AvgPoint = avgPoint;
      }
      public static Student Parse(string text)
      {
         string[] s = text.Split(char.Parse(" "));
         return new Student(s[1], s[0], s[2], DateTime.Parse(s[3]),
            byte.Parse(s[4]), s[5], float.Parse(s[6]));
      }
      public override string ToString()
      {
         return $"{LastName} {FirstName} {Patronymic} {Age} {Course} {Group} {AvgPoint}";
      }
   }
   public class Teacher : IPerson
   {
      public enum EPosition : byte { Ассистент, Cтарший_преподаватель, Доцент, Профессор, Заведующий_кафедрой };
      public string FirstName { get; }
      public string LastName { get; }
      public string Patronymic { get; }
      public DateTime Birthday { get; }
      public string Department { get; }
      public EPosition Position { get; }
      public byte Experience { get; }
      public int Age => Birthday.CalculateAge();
      public Teacher(string firstName, string lastName,
         string patronymic, DateTime birthday, string department, string position, byte experience)
      {
         FirstName = firstName;
         LastName = lastName;
         Patronymic = patronymic;
         Birthday = birthday;
         Department = department;
         Experience = experience;
         switch (position)
         {
            case "Ассистент": Position = EPosition.Ассистент; break;
            case "Cтарший_преподаватель": Position = EPosition.Cтарший_преподаватель; break;
            case "Доцент": Position = EPosition.Доцент; break;
            case "Профессор": Position = EPosition.Профессор; break;
            case "Заведующий_кафедрой": Position = EPosition.Заведующий_кафедрой; break;
         }
      }
      public static Teacher Parse(string text)
      {
         string[] s = text.Split(char.Parse(" "));
         return new Teacher(s[1], s[0], s[2], DateTime.Parse(s[3]),
            s[4], s[5], byte.Parse(s[6]));
      }
      public override string ToString()
      {
         return $"{LastName} {FirstName} {Patronymic} {Age} {Department} {Position} {Experience}";
      }
   }
   interface IUnivesity
   {
      IEnumerable<IPerson> Persons { get; }
      IEnumerable<Student> Students { get; }
      IEnumerable<Teacher> Teachers { get; }
      void Add(IPerson person);
      void Remove(IPerson person);
      IEnumerable<IPerson> FindByLastName(string lastName);
      IEnumerable<Teacher> FindByDepartment(string text);
   }
   public class University : IUnivesity                                                    // Класс университета
   {
      List<Student> students;
      List<Teacher> teachers;
      public University()
      {
         students = new List<Student>();
         teachers = new List<Teacher>();
      }
      public IEnumerable<IPerson> Persons => new List<IPerson>().Concat(Students).Concat(Teachers);
      public IEnumerable<Student> Students => students;
      public IEnumerable<Teacher> Teachers => teachers;
      public void Add(IPerson person)                                                     // Добавить человека в университет
      {
         if (person is Student student)
            students.Add(student);
         if (person is Teacher teacher)
            teachers.Add(teacher);
      }
      public void Remove(IPerson person)                                                  // Удалить человека из университета
      {
         if (person is Student student)
            students.Remove(student);
         if (person is Teacher teacher)
            teachers.Remove(teacher);
      }
      public IEnumerable<IPerson> FindByLastName(string lastName) => Persons.Where(x => x.LastName == lastName);
      public IEnumerable<Teacher> FindByDepartment(string text) =>
          Teachers.Where(x => x.Department.Contains(text)).OrderBy(x => x.Position);
      public void SortByBirthdayStudents()
      {
         students = Students.OrderBy(x => x.Birthday).ToList();
      }
      public void SortByBirthdayTeachers()
      {
         teachers = Teachers.OrderBy(x => x.Birthday).ToList();
      }
      public List<IPerson> SortByBirthdayAll()
      {
         return Persons.OrderBy(x => x.Birthday).ToList();
      }
   }
   public static class Ageculc
   {
      public static int CalculateAge(this DateTime date)
      {
         return (DateTime.Now.Month > date.Month || (DateTime.Now.Month == date.Month && DateTime.Now.Day >= date.Day))
         ? DateTime.Today.Year - date.Year :
         DateTime.Today.Year - date.Year - 1;
      }
   }
   class Program
   {
      static void Main()
      {
         University university = new University();
         using (StreamReader fin = new StreamReader("inst.txt"))
         {
            string text;
            while ((text = fin.ReadLine()) != null)
               university.Add(Student.Parse(text));
         }
         using (StreamReader fin = new StreamReader("inpr.txt"))
         {
            string text;
            while ((text = fin.ReadLine()) != null)
               university.Add(Teacher.Parse(text));
         }

         Console.WriteLine("Выберете функцию: ");
         Console.WriteLine("<1> - добавить студента");
         Console.WriteLine("<2> - добавить преподавателя");
         Console.WriteLine("<3> - удалить студента");
         Console.WriteLine("<4> - удалить преподавателя");
         Console.WriteLine("<5> - найти человека по фамилий");
         Console.WriteLine("<6> - выдать список всех людей, отсортировав их по дате рождения");
         Console.WriteLine("<7> - выдать список всех учеников, отсортировав их по дате рождения");
         Console.WriteLine("<8> - выдать список всех преподавателей, отсортировав их по дате рождения");
         Console.WriteLine("<9> - выдать список всех преподавателей на указанной кафедре");
         Console.WriteLine("<10> - выйти");
         bool exit = false;
         do
         {
            Console.WriteLine("?");
            int num = int.Parse(Console.ReadLine());
            switch (num)
            {
               case 1:
                  Console.WriteLine("Введите данные о студенте;");
                  Console.WriteLine("ФИО, дата рождения, курс, группа, средний балл:");
                  university.Add(Student.Parse(Console.ReadLine()));
                  break;
               case 2:
                  Console.WriteLine("Введите данные о перподавателе;");
                  Console.WriteLine("ФИО, дата рождения, кафедра, должность, стаж работы:");
                  university.Add(Teacher.Parse(Console.ReadLine()));
                  break;
               case 3:
                  {
                     Console.WriteLine("Введите фамилию удаляемого студента:");
                     string str = Console.ReadLine();
                     List<Student> toremove = university.Students.Where(x => x.LastName == str).ToList();
                     foreach (var trm in toremove)
                        university.Remove(trm);
                     break;
                  }
               case 4:
                  {
                     Console.WriteLine("Введите фамилию удаляемого перподавателя:");
                     string str = Console.ReadLine();
                     List<Teacher> toremove = university.Teachers.Where(x => x.LastName == str).ToList();
                     foreach (var trm in toremove)
                        university.Remove(trm);

                     break;
                  }
               case 5:
                  Console.WriteLine("Введите фамилию:");
                  var ln = university.FindByLastName(Console.ReadLine());

                  if (ln.Any())
                     foreach (var human in ln)
                        Console.WriteLine(human.ToString());
                  else
                     Console.WriteLine("По вашему запросу ничего не найдено!");

                  break;
               case 6:
                  Console.WriteLine("Cписок всех людей:");

                  var people = university.SortByBirthdayAll(); ;

                  if (people.Any())
                     foreach (var human in people)
                        Console.WriteLine(human.ToString());
                  else
                     Console.WriteLine("В университете никто не числится!");

                  break;
               case 7:
                  Console.WriteLine("Cписок всех студентов:");
                  university.SortByBirthdayStudents();
                  var students = university.Students;

                  if (students.Any())
                     foreach (var human in university.Students)
                        Console.WriteLine(human.ToString());
                  else
                     Console.WriteLine("В университете никто не числится!");

                  break;
               case 8:
                  Console.WriteLine("Cписок всех учителей:");
                  university.SortByBirthdayTeachers();
                  var teachers = university.Teachers;

                  if (teachers.Any())
                     foreach (var human in university.Teachers)
                        Console.WriteLine(human.ToString());
                  else
                     Console.WriteLine("В университете никто не числится!");

                  break;
               case 9:
                  Console.WriteLine("Введите кафедру:");
                  var dep = university.FindByDepartment(Console.ReadLine()).ToList();

                  if (dep.Any())
                     foreach (var teacher in dep)
                        Console.WriteLine(teacher.ToString());
                  else
                     Console.WriteLine("По вашему запросу ничего не найдено");

                  break;
               case 10:
                  exit = true;
                  break;
               default:
                  Console.WriteLine("Неверный номер задания!");
                  Console.WriteLine("Выберете номер:");
                  num = int.Parse(Console.ReadLine());
                  break;
            }
         } while (!exit);
      }
   }
}