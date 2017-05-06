﻿using System;
using System.Collections.Generic;
using Xunit;

namespace Handlebars.Core.Test
{
    public class HelperTests
    {
        [Fact]
        public void HelperWithLiteralArguments()
        {
            var engine = new HandlebarsEngine();
            engine.RegisterHelper("myHelper", (writer, context, args) => {
                var count = 0;
                foreach(var arg in args)
                {
                    writer.Write("\nThing {0}: {1}", ++count, arg);
                }
            });

            var source = "Here are some things: {{myHelper 'foo' 'bar'}}";

            var template = engine.Compile(source);

            var output = template.Render(new { });

            var expected = "Here are some things: \nThing 1: foo\nThing 2: bar";

            Assert.Equal(expected, output);
        }

        [Fact]
        public void HelperWithLiteralArgumentsWithQuotes()
        {
            var helperName = "helper-" + Guid.NewGuid(); //randomize helper name
            var engine = new HandlebarsEngine();
            engine.RegisterHelper(helperName, (writer, context, args) => {
                var count = 0;
                foreach(var arg in args)
                {
                    writer.WriteSafeString(
                        $"\nThing {++count}: {arg}");
                }
            });

            var source = "Here are some things: {{" + helperName + " 'My \"favorite\" movie' 'bar'}}";

            var template = engine.Compile(source);

            var output = template.Render(new { });

            var expected = "Here are some things: \nThing 1: My \"favorite\" movie\nThing 2: bar";

            Assert.Equal(expected, output);
        }

        [Fact]
        public void InversionNoKey()
        {
            var source = "{{^key}}No key!{{/key}}";
            var engine = new HandlebarsEngine();
            var template = engine.Compile(source);
            var output = template.Render(new { });
            var expected = "No key!";
            Assert.Equal(expected, output);
        }

        [Fact]
        public void InversionFalsy()
        {
            var source = "{{^key}}Falsy value!{{/key}}";
            var engine = new HandlebarsEngine();
            var template = engine.Compile(source);
            var data = new
            {
                key = false
            };
            var output = template.Render(data);
            var expected = "Falsy value!";
            Assert.Equal(expected, output);
        }

        [Fact]
        public void InversionEmptySequence()
        {
            var source = "{{^key}}Empty sequence!{{/key}}";
            var engine = new HandlebarsEngine();
            var template = engine.Compile(source);
            var data = new
                {
                    key = new string[] { }
                };
            var output = template.Render(data);
            var expected = "Empty sequence!";
            Assert.Equal(expected, output);
        }

        [Fact]
        public void InversionNonEmptySequence()
        {
            var source = "{{^key}}Empty sequence!{{/key}}";
            var engine = new HandlebarsEngine();
            var template = engine.Compile(source);
            var data = new
                {
                    key = new[] { "element" }
                };
            var output = template.Render(data);
            var expected = "";
            Assert.Equal(expected, output);
        }

        [Fact]
        public void BlockHelperWithArbitraryInversion()
        {
            var source = "{{#ifCond arg1 arg2}}Args are same{{else}}Args are not same{{/ifCond}}";

            var engine = new HandlebarsEngine();
            engine.RegisterHelper("ifCond", (writer, options, context, arguments) => {
                if(arguments[0] == arguments[1])
                {
                    options.Template(writer, (object)context);
                }
                else
                {
                    options.Inverse(writer, (object)context);
                }
            });

            var dataWithSameValues = new
                {
                    arg1 = "a",
                    arg2 = "a"
                };
            var dataWithDifferentValues = new
                {
                    arg1 = "a",
                    arg2 = "b"
                };

            var template = engine.Compile(source);

            var outputIsSame = template.Render(dataWithSameValues);
            var expectedIsSame = "Args are same";
            var outputIsDifferent = template.Render(dataWithDifferentValues);
            var expectedIsDifferent = "Args are not same";

            Assert.Equal(expectedIsSame, outputIsSame);
            Assert.Equal(expectedIsDifferent, outputIsDifferent);
        }

        [Fact]
        public void HelperWithNumericArguments()
        {
            var engine = new HandlebarsEngine();
            engine.RegisterHelper("myHelper", (writer, context, args) => {
                var count = 0;
                foreach(var arg in args)
                {
                    writer.Write("\nThing {0}: {1}", ++count, arg);
                }
            });

            var source = "Here are some things: {{myHelper 123 4567 -98.76}}";

            var template = engine.Compile(source);

            var output = template.Render(new { });

            var expected = "Here are some things: \nThing 1: 123\nThing 2: 4567\nThing 3: -98.76";

            Assert.Equal(expected, output);
        }

        [Fact]
        public void HelperWithHashArgument()
        {
            var engine = new HandlebarsEngine();
            engine.RegisterHelper("myHelper", (writer, context, args) => {
                var hash = (Dictionary<string, object>)args[2];
                foreach(var item in hash)
                {
                    writer.Write(" {0}: {1}", item.Key, item.Value);
                }
            });

            var source = "Here are some things:{{myHelper 'foo' 'bar' item1='val1' item2='val2'}}";

            var template = engine.Compile(source);

            var output = template.Render(new { });

            var expected = "Here are some things: item1: val1 item2: val2";

            Assert.Equal(expected, output);
        }
            
        [Fact]
        public void BlockHelperWithSubExpression()
        {
            var engine = new HandlebarsEngine();
            engine.RegisterHelper("isEqual", (writer, context, args) =>
            {
                writer.WriteSafeString(args[0].ToString() == args[1].ToString() ? "true" : null);
            });
        
            var source = "{{#if (isEqual arg1 arg2)}}True{{/if}}";
        
            var template = engine.Compile(source);
        
            var expectedIsTrue = "True";
            var outputIsTrue = template.Render(new { arg1 = 1, arg2 = 1 });
            Assert.Equal(expectedIsTrue, outputIsTrue);
        
            var expectedIsFalse = "";
            var outputIsFalse = template.Render(new { arg1 = 1, arg2 = 2 });
            Assert.Equal(expectedIsFalse, outputIsFalse);
        }

        [Fact]
        public void HelperWithSegmentLiteralArguments()
        {
            var engine = new HandlebarsEngine();
            engine.RegisterHelper("myHelper", (writer, context, args) => {
                var count = 0;
                foreach (var arg in args)
                {
                    writer.Write("\nThing {0}: {1}", ++count, arg);
                }
            });

            var source = "Here are some things: {{myHelper args.[0].arg args.[1].arg 'another argument'}}";

            var template = engine.Compile(source);

            var data = new
            {
                args = new[] { new { arg = "foo" }, new { arg = "bar" } }
            };

            var output = template.Render(data);

            var expected = "Here are some things: \nThing 1: foo\nThing 2: bar\nThing 3: another argument";

            Assert.Equal(expected, output);
        }

    }
}

