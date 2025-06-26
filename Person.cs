using System.Collections.Generic;

namespace HelloFunctionApp
{
    public class Person
    {
        public string Name { get; set; }
    }
    public class Employee
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
    public static class FakeQueue
    {
        public static Queue<Employee> Employees = new Queue<Employee>();
    }
}
