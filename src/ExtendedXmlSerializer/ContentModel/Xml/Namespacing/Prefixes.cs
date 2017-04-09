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

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Xml.Namespacing
{
	sealed class Prefixes : TableSource<string, string>, IPrefixes
	{
		public static Prefixes Default { get; } = new Prefixes();

		Prefixes() : this(WellKnownNamespaces.Default) {}

		readonly IDictionary<Assembly, IIdentity> _known;

		public Prefixes(IDictionary<Assembly, IIdentity> known)
			: this(known, known.Values.ToDictionary(x => x.Identifier, x => x.Name)) {}

		public Prefixes(IDictionary<Assembly, IIdentity> known, IDictionary<string, string> names) : base(names)
		{
			_known = known;
		}

		public string Get(TypeInfo parameter) => _known.Get(parameter.Assembly)?.Name;
	}
}