// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
Console.WriteLine("Before: " + Thread.CurrentThread.ManagedThreadId);
var task =FunctionAsync();
task.Wait();
Console.WriteLine("After: " + Thread.CurrentThread.ManagedThreadId);


async Task FunctionAsync()
{
    Console.WriteLine("In Before: " + Thread.CurrentThread.ManagedThreadId);
    var client = new HttpClient();

    await Task.Delay(1000);
    Console.WriteLine("Result: " + Thread.CurrentThread.ManagedThreadId);
    await Task.Delay(1000);
    Console.WriteLine("In After: " + Thread.CurrentThread.ManagedThreadId);
}
