# Pipes
## Introduction
Pipes is a procedural data transformation library inspired by web request middleware.

Pipes seeks to provide an abstract implementation of middleware-like procedural data processing that can be used in any data transformation context.

## Usage
To use a Pipe, you must provide it with an input type and an output type, then define which steps are to be executed in the pipeline via `PipeSegment` instances.

### Setting up PipeSegments
PipeSegments are invoked throughout a Pipe's lifecycle. They are given access to the original input object, the current output object, and a `next()` method which calls the next segment in the pipeline (or ends the processing if the current segment is the last in the chain). Generally, they are used to modify the output object, but can also perform other operations if desired.

#### Method 1: InlineSegments
Simple processing steps can be defined with a compact inline syntax. A PipeSegment can be defined inline in two ways:

- Using `PipeSegment.Takes<InputType, OutputType>()` to generate an `InlineSegment`, then using the `.Does()` method to define its behavior
- Providing `Pipe.Then()` with an anonymous method which describes the segment's behavior

```csharp
// Define a segment inline and store in a variable
var inlineDoubler = PipeSegment
  .Takes<int, int>()
  .Does((ref int input, ref int output, Action next) =>
  {
    // Double whatever output's value is at this point in the pipeline
    output *= 2;
    next();
  });

// ...

var pipe = Pipe
  .Takes<int, int>()
  // Define a segment inline that initially 
  // sets output's value to be equal to input's value
  .Then((ref int input, ref int output, Action next) =>
  {
    output = input;
    next();
  });
  // Add our inlineDoubler segment to the pipe
  .Then(inlineDoubler);
```
#### Method 2: Deriving PipeSegment
More complicated segments can be defined as their own class by deriving `PipeSegment<InputType, OutputType>`. Pipeline processing behavior is defined by overriding the `Process()` method.

```csharp
using System;

public class ToPositive : PipeSegment<int, int>
{
  protected override void Process(ref int input, ref int output, Action next)
  {
    output = Math.Abs(output);
    next();
  }
}
```
Once defined, this custom segment can be added to a Pipe in two ways:

```csharp
// Method 1: Add new ToPositive segment instance to Pipe using Pipe.Then<T>();
var pipe = Pipe
	.Takes<int, int>()
	.Then<ToPositive>();

// Method 2: Add existing ToPositive instance to Pipe using Pipe.Then();
var toPos = new ToPositive();

var pipe = Pipe
	.Takes<int, int>()
	.Then(toPos);
```
Complex segments can be derived from `PipeSegment` and instantiated before being added to a pipe. This allows for initialization and modification before adding it to the pipeline.

### Setting Up the Pipe

```csharp
var pipe = Pipe
  // Define input/output types
  .Takes<InputType, OutputType>()
  // Use pipeline class derived from PipeSegment
  .Then<CustomPipeSegment>()
  // Use PipeSegment referenced by pipeSegmentVar variable
  .Then(pipeSegmentVar)
  // Define a PipeSegment in-line
  .Then((ref InputType input, ref OutputType output, Action next) =>
  {
    // do something with input and/or output
    // Then call next() to invoke the next step in the pipeline
    next();
  });
```

Each `PipeSegment` recieves a reference to the original input object and the current output object that is moving through the pipeline. Each `PipeSegment` has an opportunity to make adjustments to either the initial input object and/or the output object being processed through the pipeline, or utilize traits of the input and/or output objects to perform their own separate tasks.

Finally, once a `Pipe` is defined and set up with its data process steps via chaining the `Then()` method, it can be run using the `In()` or `Out()` methods.

```csharp
// We can generate a new output object using Pipe.Out() *
var input = new InputType();
var result = pipe.Out<OutputType>(ref input);

// Or we can mutate existing objects/references with Pipe.In()
var input = new InputType();
var output = new OutputType();

pipe.In(ref input, ref output);

// * OutputType must be a reference type with a public parameter-less constructor in order to be produced by Pipe.Out(). Otherwise Pipe.In() must be used to process an existing OutputType reference.
```