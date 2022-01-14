using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.CSharp
{


    //struct S0
    //{
    //    public int X { get; set; } = 5;
    //}

    struct S1
    {
        public int X { get; set; } = 5;
        public S1() { }
    }

    struct S2
    {
        public int X { get; set; } = 5;
        public S2(string message) { }
    }

    struct S3
    {
        public int X { get; set; } = 5;
        public S3() { }

        public S3(string message) { }

    }

    struct S4
    {
        //public S3() 
        //{
        //    // WHat is Message?
        //}
        public S4(string msg) => Message = msg;

        public string Message { get; init; }
    }

    record struct RS0(string first, string last, string? optional = null)
    {
        public string? OptionalMessage { get; } = optional;
    }
    record struct RS1(string first, string last)
    {
        public string first { get; set; } = last;

        public RS1() : this("Fred", "George")
        {
            // What is "first" and "last"?
        }
    }

}
