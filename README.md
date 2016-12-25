# .NET tests with Tail Call Recursion optimization, in C# and F# #

This is a test solution which shows how tail calls optimized in its simplest version. The solution contains two projects: in C# and F#. Each project is a simple console application which calls recursive method (function) that doesn't have exit case, so should work eternally. 

Because of stack-related techological nature of a method (function) call, a limitation applied on a number of recursive calls. But in case of [Tail Call](https://en.wikipedia.org/wiki/Tail_call), recursion could be transformed to a simple iteration which doesn't have stack depth limitation. C# compiler doesn't have such optimization, but F# has. 

In .NET world there is one more compiler, language agnostic: JIT compiler, which compiles IL code to binaty code when running on a target computer. And JIT for x64 platform has optimization which recognizes tail calls on the fly and generates iteration instead of mehod call instructions.

## C# ##

C# compiler doesn't optimize output IL code so there is recursive call in the output assembly. From IL Spy:

```CS
	internal class Program
	{
		private static void Main(string[] args)
		{
			Console.WriteLine(Program.Recursive(0));
		}

		public static int Recursive(int i)
		{
			if (i % 1000 == 0)
			{
				Console.WriteLine(i);
			}
			return Program.Recursive(i + 1);
		}
	}
```

But JIT compiler has ability to optimize tail calls during execution for optimized code and x64 platform. 

|Configuration|Platform|Tail Optimized|
|-------------|--------|--------------|
|Debug        |AnyCPU  |No            |
|Debug        |x86     |No            |
|Debug        |x64     |No            |
|Release      |AnyCPU  |No            |
|Release      |x86     |No            |
|Release      |x64     |**Yes**       |

As we can see Release x64 configuration gives us an assembly that will run eternally on x64 platform. All other configurations will break soon after program start because of StackOverflowException. 

## F# ##

F# compiler makes actual IL code non-recursive for tail recursive calls. From IL Spy:

```CS
[CompilationMapping(SourceConstructFlags.Module)]
public static class Program
{
	public static a Recursive<a>(int i)
	{
		while (true)
		{
			if (i % 1000 == 0)
			{
				Console.WriteLine(i);
			}
			i++;
		}
	}

	[EntryPoint]
	public static int main(string[] argv)
	{
		Program.Recursive<Unit>(0);
		return 0;
	}
}
```

From decompiled code we can see that instead of recursive call, actual code has been changed to ```whule(true)``` loop which doesn't create new stack on each iteration.
Any build configuration for any platform gives an assembly which is tail-optimized on IL level.

|Configuration|Platform|Tail Optimized|
|-------------|--------|--------------|
|Debug        |AnyCPU  |**Yes**       |
|Debug        |x86     |**Yes**       |
|Debug        |x64     |**Yes**       |
|Release      |AnyCPU  |**Yes**       |
|Release      |x86     |**Yes**       |
|Release      |x64     |**Yes**       |


There is a special opcode in IL for tail calls: ```tail.``` It explicitly instructs JIT to optimize tail calls the same way it optimizes C# tail calls on Release x64 platform. This opcode never used by C# compiler. F# sometimes puts this opcode together with recursive calls, when recursive functoin is not as primitive as in the project's example.
