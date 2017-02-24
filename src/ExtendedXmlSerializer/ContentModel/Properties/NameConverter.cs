// MIT License
// 
// Copyright (c) 2016 Wojciech Nag�rski
//                    Michael DeMond
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Linq;
using ExtendedXmlSerialization.ContentModel.Converters;
using ExtendedXmlSerialization.ContentModel.Xml.Parsing;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.ContentModel.Properties
{
	class NameConverter : ConverterBase<ParsedName>, INameConverter
	{
		public static NameConverter Default { get; } = new NameConverter();
		NameConverter() : this(IdentityFormatter<ParsedName>.Default, ParsedNames.Default) {}

		readonly IFormatter<ParsedName> _formatter;
		readonly IParsedNames _names;
		readonly Func<ParsedName, string> _selector;

		public NameConverter(IFormatter<ParsedName> formatter, IParsedNames names)
		{
			_formatter = formatter;
			_names = names;
			_selector = Format;
		}

		public sealed override ParsedName Parse(string data) => _names.Get(data);

		public sealed override string Format(ParsedName instance)
		{
			var arguments = instance.GetArguments();
			var append = arguments.HasValue ? $"[{string.Join(",", arguments.Value.Select(_selector))}]" : null;
			var result = $"{_formatter.Get(instance)}{append}";
			return result;
		}
	}
}